using System.Data;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// IMonitorRepository no banco principal, com Dapper (docs/PLANO-WEBAPP.md, passo 11): auditing.* e monitor.* da empresa
    /// logada. Toda leitura é WITH (NOLOCK), para não segurar o sp_Log_Start nem o bulk insert de Records; as contagens saem
    /// prontas do SQL. A lista de jobs (com a última execução) e os totais por período passam por um cache de 30 s, e
    /// qualquer escrita daqui o invalida. A saúde, o reconhecimento e a ordem são os mesmos do MockMonitorRepository.
    /// Mensagens: o nível vem de auditing.Messages.Level (1 informação, 2 aviso, 3 erro), da proposta de nível
    /// (src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md): só o erro põe o job em saúde de erro; o aviso é contado à parte.
    /// </summary>
    public class SqlMonitorRepository(MainDatabase database, IMemoryCache cache, IModuleParametersRepository parameters, LoggedCompany loggedCompany, ILogger<SqlMonitorRepository> logger) : IMonitorRepository
    {
        // até a fase 6 (autenticação), quem reconhece é um nome fixo, como no mock
        private const string User = "operador";
        private static readonly TimeSpan AcknowledgementLifetime = TimeSpan.FromHours(24);
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromSeconds(30);
        private const string Masked = "••••••••••••••••";

        // mensagens lidas de uma vez para agrupar por causa em C# (Logs de Erros); passou disso, o resto fica de fora e vai
        // para o log
        private const int MaxGroupedMessages = 20_000;

        private const string JobsKey = "Monitor:Jobs";
        private const string TotalsKey = "Monitor:Totals";

        // muda a cada escrita: as chaves do cache levam a versão, e as entradas antigas expiram sozinhas
        private static int _version;

        // a empresa de For(empresa) ou, sem ele, a do seletor do topo (os jobs e os logs são dela) e a holding dela (imagens e
        // parâmetros de apresentação)
        private string? _company;

        private string Company => string.IsNullOrEmpty(_company) ? loggedCompany.DocCompany : _company;

        public IMonitorRepository For(string company) =>
            new SqlMonitorRepository(database, cache, parameters, loggedCompany, logger) { _company = company };

        private string Holder => loggedCompany.Holder;

        // a aplicação de cada job do catálogo: a integração está na árvore do módulo, e a procedure na do job que a chama
        // (auditing.fn_CatalogTree, a mesma da contratação de módulo)
        private const string JobAppsCte = """
            WITH JobApps AS (
                SELECT t.IdJob, t.IdType, MIN(a.IdParent) AS IdApp
                FROM [auditing].[Apps] a WITH (NOLOCK)
                CROSS APPLY [auditing].[fn_CatalogTree](a.IdParent) t
                GROUP BY t.IdJob, t.IdType
            )
            """;

        // registros, mensagens de erro (nível 3) e de aviso (nível 2) de uma execução l
        private const string ExecutionCounts = """
            CROSS APPLY (
                SELECT COUNT(*) AS Records
                FROM [auditing].[Records] r WITH (NOLOCK)
                WHERE r.Company = l.Company AND r.IdJob = l.IdJob AND r.IdType = l.IdType AND r.Execution = l.Execution
            ) rc
            CROSS APPLY (
                SELECT
                    COALESCE(SUM(CASE WHEN m.[Level] = 3 THEN 1 ELSE 0 END), 0) AS Errors,
                    COALESCE(SUM(CASE WHEN m.[Level] = 2 THEN 1 ELSE 0 END), 0) AS Warnings
                FROM [auditing].[Messages] m WITH (NOLOCK)
                WHERE m.Company = l.Company AND m.IdJob = l.IdJob AND m.IdType = l.IdType AND m.Execution = l.Execution AND m.[Level] > 1
            ) ec
            """;

        private const string SelectJobs = $"""
            {JobAppsCte}
            SELECT
                cat.IdParent,
                COALESCE(ja.IdApp, cat.IdParent) AS IdApp,
                j.IdJob,
                j.IdType,
                ty.JobType,
                cat.Title,
                cat.[Description],
                j.IntervalTimeInMinutes,
                j.IsActive,
                j.RunningDailyAt,
                j.Queue,
                last.IdExecution,
                last.Execution,
                last.ExecutionParent,
                last.StartDate,
                last.EndDate,
                COALESCE(counts.Records, 0) AS Records,
                COALESCE(counts.Errors, 0) AS Errors,
                COALESCE(counts.Warnings, 0) AS Warnings,
                ack.Execution AS AckExecution,
                ack.Note AS AckNote,
                ack.[User] AS AckUser,
                ack.[At] AS AckAt,
                ack.ExpiresAt AS AckExpiresAt
            FROM
                [auditing].[Jobs] j WITH (NOLOCK)
                JOIN [auditing].[Catalog] cat WITH (NOLOCK) ON cat.IdJob = j.IdJob AND cat.IdType = j.IdType
                JOIN [auditing].[Types] ty WITH (NOLOCK) ON ty.IdType = j.IdType
                LEFT JOIN JobApps ja ON ja.IdJob = j.IdJob AND ja.IdType = j.IdType
                OUTER APPLY (
                    SELECT TOP (1) l.Company, l.IdJob, l.IdType, l.IdExecution, l.Execution, l.ExecutionParent, l.StartDate, l.EndDate
                    FROM [auditing].[Logs] l WITH (NOLOCK)
                    WHERE l.Company = j.Company AND l.IdJob = j.IdJob AND l.IdType = j.IdType
                    ORDER BY l.StartDate DESC
                ) last
                OUTER APPLY (
                    SELECT rc.Records, ec.Errors, ec.Warnings
                    FROM (SELECT last.Company, last.IdJob, last.IdType, last.Execution) l
                    {ExecutionCounts}
                    WHERE last.Execution IS NOT NULL
                ) counts
                LEFT JOIN [monitor].[JobAcknowledgement] ack WITH (NOLOCK) ON ack.Company = j.Company AND ack.IdJob = j.IdJob AND ack.IdType = j.IdType
            WHERE
                j.Company = @company
            """;

        private const string SelectApps = """
            SELECT IdParent, Title, [Description] FROM [auditing].[Apps] WITH (NOLOCK) ORDER BY Title;
            SELECT IdParent, Holder, ContentType, [Data] FROM [monitor].[AppImage] WITH (NOLOCK) WHERE Holder IN (@holder, '*');
            """;

        // execuções por job e faixa de tempo: @width segundos por faixa a partir de @start
        private const string SelectTotals = $"""
            SELECT
                l.IdJob,
                l.IdType,
                b.Bucket,
                COUNT(*) AS Executions,
                SUM(CASE WHEN ec.Errors > 0 THEN 1 ELSE 0 END) AS FailedExecutions,
                SUM(CAST(rc.Records AS bigint)) AS Records,
                SUM(ec.Errors) AS Errors,
                SUM(ec.Warnings) AS Warnings,
                MAX(CASE WHEN ec.Errors > 0 THEN l.StartDate END) AS LastErrorAt
            FROM
                [auditing].[Logs] l WITH (NOLOCK)
                CROSS APPLY (SELECT CAST(DATEDIFF_BIG(SECOND, @start, l.StartDate) / @width AS int) AS Bucket) b
                {ExecutionCounts}
            WHERE
                l.Company = @company
                AND l.StartDate >= @start
                AND l.StartDate < @end
            GROUP BY
                l.IdJob, l.IdType, b.Bucket
            """;

        private const string SelectExecutionColumns = """
            l.IdExecution, l.IdJob, l.IdType, l.Execution, l.ExecutionParent, l.StartDate, l.EndDate, rc.Records, ec.Errors, ec.Warnings
            """;

        // mensagens do filtro da tela Logs de Erros (MessageFilter monta os parâmetros)
        private const string MessagesFrom = $"""
            FROM
                [auditing].[Messages] m WITH (NOLOCK)
                JOIN [auditing].[Logs] l WITH (NOLOCK) ON l.Company = m.Company AND l.IdJob = m.IdJob AND l.IdType = m.IdType AND l.Execution = m.Execution
                JOIN [auditing].[Jobs] j WITH (NOLOCK) ON j.Company = m.Company AND j.IdJob = m.IdJob AND j.IdType = m.IdType
                JOIN [auditing].[Catalog] cat WITH (NOLOCK) ON cat.IdJob = j.IdJob AND cat.IdType = j.IdType
                LEFT JOIN JobApps ja ON ja.IdJob = m.IdJob AND ja.IdType = m.IdType
            WHERE
                m.Company = @company
                AND l.StartDate >= @since
                AND m.[Level] >= @minLevel
                AND (@idApp IS NULL OR COALESCE(ja.IdApp, cat.IdParent) = @idApp)
                AND (@idJob IS NULL OR (m.IdJob = @idJob AND m.IdType = @idType))
                AND (@search IS NULL
                    OR m.[Message] COLLATE Latin1_General_100_CI_AI LIKE @search
                    OR m.ExceptionMessage COLLATE Latin1_General_100_CI_AI LIKE @search)
            """;

        private const string MessageColumns = """
            m.IdExecutionMessage,
            m.IdJob,
            m.IdType,
            m.Execution,
            CAST(m.[Level] AS int) AS [Level],
            m.[Message],
            m.SqlCommand,
            m.ExceptionMessage,
            m.ExceptionStackTrace,
            COALESCE(ja.IdApp, cat.IdParent) AS IdApp,
            cat.Title AS JobTitle,
            l.StartDate AS ExecutionStartDate
            """;

        public async Task<IReadOnlyList<AppSummaryModel>> GetAppSummariesAsync(Period period, CancellationToken cancellationToken = default)
        {
            var now = MainDatabase.LocalNow();
            var jobs = await JobsAsync(cancellationToken);
            var (apps, images) = await AppsAsync(cancellationToken);

            // o início do período, na hora cheia do minuto: a mesma chave de cache para quem abre a tela no mesmo minuto
            var end = MinuteOf(now).AddMinutes(1);
            var start = end - period.ToTimeSpan();
            var totals = await TotalsAsync(start, end, 1, cancellationToken);

            return apps
                .Select(app => (App: app, Jobs: jobs.Where(x => x.IdApp == app.IdParent).ToList()))
                .Where(x => x.Jobs.Count > 0)
                .Select(x =>
                {
                    var keys = x.Jobs.Select(job => (job.IdJob, job.IdType)).ToHashSet();
                    var executions = totals.Where(t => keys.Contains((t.IdJob, t.IdType))).ToList();

                    return new AppSummaryModel
                    {
                        IdParent = x.App.IdParent,
                        Title = x.App.Title,
                        Description = x.App.Description,
                        ImageUrl = ImageUrlOf(x.App.IdParent, images),
                        Integrations = x.Jobs.Count(job => job.IdType == 2),
                        Procedures = x.Jobs.Count(job => job.IdType == 1),
                        ActiveJobs = x.Jobs.Count(job => job.IsActive),
                        Executions = executions.Sum(t => t.Executions),
                        FailedExecutions = executions.Sum(t => t.FailedExecutions),
                        Records = Clamp(executions.Sum(t => t.Records)),
                        Warnings = executions.Sum(t => t.Warnings),
                        Errors = executions.Sum(t => t.Errors),
                        LastErrorAt = executions.Max(t => t.LastErrorAt),
                        FailingJobs = x.Jobs.Count(job => Health(job, now) == JobHealth.Error)
                    };
                })
                .ToList();
        }

        public async Task<IReadOnlyList<AppBucketsModel>> GetExecutionBucketsAsync(Period period, int buckets, bool previousWindow = false, CancellationToken cancellationToken = default)
        {
            var now = MainDatabase.LocalNow();
            var jobs = await JobsAsync(cancellationToken);
            var (apps, _) = await AppsAsync(cancellationToken);

            // faixas alinhadas à hora (ou ao dia) cheia: a última termina na próxima hora cheia depois de agora (como no mock)
            var width = period.ToTimeSpan() / buckets;
            var alignment = width >= TimeSpan.FromDays(1) ? TimeSpan.FromDays(1) : width >= TimeSpan.FromHours(1) ? TimeSpan.FromHours(1) : width;
            var end = new DateTime((now.Ticks + alignment.Ticks - 1) / alignment.Ticks * alignment.Ticks);
            if (previousWindow)
                end -= width * buckets;
            var start = end - width * buckets;

            var totals = await TotalsAsync(start, end, buckets, cancellationToken);
            var appOf = jobs.ToDictionary(x => (x.IdJob, x.IdType), x => x.IdApp);

            return apps
                .Where(app => jobs.Any(x => x.IdApp == app.IdParent))
                .Select(app =>
                {
                    var counts = Enumerable.Range(0, buckets)
                        .Select(i => new ExecutionBucketModel { Start = start + width * i, End = start + width * (i + 1) })
                        .ToArray();

                    foreach (var total in totals.Where(t => appOf.GetValueOrDefault((t.IdJob, t.IdType)) == app.IdParent))
                    {
                        var bucket = counts[Math.Clamp(total.Bucket, 0, buckets - 1)];
                        bucket.Executions += total.Executions;
                        bucket.FailedExecutions += total.FailedExecutions;
                        bucket.Errors += total.Errors;
                        bucket.Records = Clamp(bucket.Records + total.Records);
                    }

                    return new AppBucketsModel { IdApp = app.IdParent, Buckets = counts };
                })
                .ToList();
        }

        public async Task<IReadOnlyList<JobModel>> GetJobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default)
        {
            var now = MainDatabase.LocalNow();

            return (await JobsAsync(cancellationToken))
                .Where(x => idApp is null || x.IdApp == idApp)
                .Where(x => idType is null || x.IdType == idType)
                .Select(x => new JobModel
                {
                    Company = Company,
                    IdParent = x.IdParent ?? 0,
                    IdApp = x.IdApp ?? 0,
                    IdJob = x.IdJob,
                    IdType = x.IdType,
                    JobType = x.JobType,
                    Title = x.Title,
                    Description = x.Description,
                    IntervalTimeInMinutes = x.IntervalTimeInMinutes,
                    IsActive = x.IsActive,
                    RunningDailyAt = DailyAt(x.RunningDailyAt),
                    Queue = x.Queue,
                    LastExecution = LastExecution(x),
                    Health = Health(x, now),
                    Acknowledgement = Acknowledgement(x, now)
                })
                .ToList();
        }

        public async Task<PagedResult<ExecutionModel>> GetExecutionsAsync(int idJob, int idType, Period period, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var since = MainDatabase.LocalNow() - period.ToTimeSpan();

            await using var connection = await database.OpenAsync(cancellationToken);
            using var reader = await connection.QueryMultipleAsync(new CommandDefinition($"""
                SELECT COUNT(*)
                FROM [auditing].[Logs] l WITH (NOLOCK)
                WHERE l.Company = @company AND l.IdJob = @idJob AND l.IdType = @idType AND l.StartDate >= @since;

                SELECT {SelectExecutionColumns}
                FROM [auditing].[Logs] l WITH (NOLOCK)
                {ExecutionCounts}
                WHERE l.Company = @company AND l.IdJob = @idJob AND l.IdType = @idType AND l.StartDate >= @since
                ORDER BY l.StartDate DESC, l.IdExecution DESC
                OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
                """, new { company = Company, idJob, idType, since, skip = page * pageSize, take = pageSize }, cancellationToken: cancellationToken));

            var total = await reader.ReadSingleAsync<int>();
            var items = (await reader.ReadAsync<ExecutionModel>()).ToList();

            return new PagedResult<ExecutionModel> { Items = items, TotalCount = total };
        }

        public async Task<ExecutionModel?> GetExecutionAsync(Guid execution, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ExecutionModel>(new CommandDefinition($"""
                SELECT TOP (1) {SelectExecutionColumns}
                FROM [auditing].[Logs] l WITH (NOLOCK)
                {ExecutionCounts}
                WHERE l.Execution = @execution AND l.Company = @company
                """, new { company = Company, execution }, cancellationToken: cancellationToken));
        }

        public async Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid execution, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var messages = await connection.QueryAsync<MessageModel>(new CommandDefinition($"""
                {JobAppsCte}
                SELECT {MessageColumns}
                FROM
                    [auditing].[Logs] l WITH (NOLOCK)
                    JOIN [auditing].[Messages] m WITH (NOLOCK) ON m.Company = l.Company AND m.IdJob = l.IdJob AND m.IdType = l.IdType AND m.Execution = l.Execution
                    JOIN [auditing].[Jobs] j WITH (NOLOCK) ON j.Company = l.Company AND j.IdJob = l.IdJob AND j.IdType = l.IdType
                    JOIN [auditing].[Catalog] cat WITH (NOLOCK) ON cat.IdJob = j.IdJob AND cat.IdType = j.IdType
                    LEFT JOIN JobApps ja ON ja.IdJob = l.IdJob AND ja.IdType = l.IdType
                WHERE
                    l.Execution = @execution AND l.Company = @company
                ORDER BY
                    m.IdExecutionMessage
                """, new { company = Company, execution }, cancellationToken: cancellationToken));

            return await WithAppTitles(messages.ToList(), cancellationToken);
        }

        public async Task<PagedResult<RecordModel>> GetRecordsAsync(Guid execution, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            using var reader = await connection.QueryMultipleAsync(new CommandDefinition("""
                SELECT COUNT(*)
                FROM
                    [auditing].[Logs] l WITH (NOLOCK)
                    JOIN [auditing].[Records] r WITH (NOLOCK) ON r.Company = l.Company AND r.IdJob = l.IdJob AND r.IdType = l.IdType AND r.Execution = l.Execution
                WHERE
                    l.Execution = @execution AND l.Company = @company;

                SELECT r.IdRecord, r.IdJob, r.IdType, r.Execution, r.FieldKeyValue, r.RecordText
                FROM
                    [auditing].[Logs] l WITH (NOLOCK)
                    JOIN [auditing].[Records] r WITH (NOLOCK) ON r.Company = l.Company AND r.IdJob = l.IdJob AND r.IdType = l.IdType AND r.Execution = l.Execution
                WHERE
                    l.Execution = @execution AND l.Company = @company
                ORDER BY
                    r.IdRecord
                OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
                """, new { company = Company, execution, skip = page * pageSize, take = pageSize }, cancellationToken: cancellationToken));

            var total = await reader.ReadSingleAsync<int>();
            var items = (await reader.ReadAsync<RecordModel>()).ToList();

            return new PagedResult<RecordModel> { Items = items, TotalCount = total };
        }

        public async Task<IReadOnlyList<HeartbeatModel>> GetHeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default)
        {
            var now = MainDatabase.LocalNow();
            var from = now - period.ToTimeSpan();
            var (apps, _) = await AppsAsync(cancellationToken);
            var titles = apps.ToDictionary(x => x.IdParent, x => x.Title);

            // a visão geral mostra só integrações ativas e com agenda (decisão de 31/10/2026), as falhas antes de qualquer Ok
            return (await JobsAsync(cancellationToken))
                .Where(x => x.IsActive && x.IdType == 2 && !IsOnDemand(x))
                .Select(x => (Job: x, Health: Health(x, now), Acknowledgement: Acknowledgement(x, now)))
                .Where(x => x.Health is JobHealth.Error or JobHealth.Late || x.Job.StartDate >= from)
                .OrderBy(x => SeverityRank(x.Health, x.Acknowledgement))
                .ThenByDescending(x => x.Job.StartDate ?? DateTime.MinValue)
                .Take(top)
                .Select(x => new HeartbeatModel
                {
                    Company = Company,
                    IdApp = x.Job.IdApp ?? 0,
                    AppTitle = titles.GetValueOrDefault(x.Job.IdApp ?? 0),
                    IdJob = x.Job.IdJob,
                    IdType = x.Job.IdType,
                    JobType = x.Job.JobType,
                    JobTitle = x.Job.Title,
                    StartDate = x.Job.StartDate,
                    EndDate = x.Job.EndDate,
                    Records = x.Job.Records,
                    Health = x.Health ?? JobHealth.Ok,
                    HasWarnings = x.Job.Warnings > 0,
                    IntervalTimeInMinutes = x.Job.IntervalTimeInMinutes,
                    RunningDailyAt = DailyAt(x.Job.RunningDailyAt),
                    Acknowledgement = x.Acknowledgement
                })
                .ToList();
        }

        // Erro, Atrasado, Rodando, falhas reconhecidas e por fim Ok
        private static int SeverityRank(JobHealth? health, AcknowledgementModel? acknowledgement) => health switch
        {
            JobHealth.Error or JobHealth.Late when acknowledgement is not null => 3,
            JobHealth.Error => 0,
            JobHealth.Late => 1,
            JobHealth.Running => 2,
            _ => 4
        };

        public async Task<PagedResult<MessageModel>> GetMessagesByLevelAsync(MessageQuery query, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            // com causa, a chave é calculada em C#: lê as mensagens do filtro, separa a página e só então busca o texto inteiro
            if (query.GroupKey is not null)
            {
                var ids = (await MessageRowsAsync(query, cancellationToken))
                    .Where(x => MessageGrouping.Key(x.Level, x.Message) == query.GroupKey)
                    .Select(x => x.IdExecutionMessage)
                    .ToList();

                return new PagedResult<MessageModel>
                {
                    Items = await MessagesByIdAsync(ids.Skip(page * pageSize).Take(pageSize).ToList(), cancellationToken),
                    TotalCount = ids.Count
                };
            }

            await using var connection = await database.OpenAsync(cancellationToken);
            using var reader = await connection.QueryMultipleAsync(new CommandDefinition($"""
                {JobAppsCte}
                SELECT COUNT(*)
                {MessagesFrom};

                {JobAppsCte}
                SELECT {MessageColumns}
                {MessagesFrom}
                ORDER BY l.StartDate DESC, m.IdExecutionMessage DESC
                OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
                """, MessageFilter(query, new { skip = page * pageSize, take = pageSize }), cancellationToken: cancellationToken));

            var total = await reader.ReadSingleAsync<int>();
            var items = (await reader.ReadAsync<MessageModel>()).ToList();

            return new PagedResult<MessageModel> { Items = await WithAppTitles(items, cancellationToken), TotalCount = total };
        }

        public async Task<IReadOnlyList<MessageJobSummaryModel>> GetMessageJobsAsync(MessageQuery query, CancellationToken cancellationToken = default)
        {
            var now = MainDatabase.LocalNow();
            var jobs = (await JobsAsync(cancellationToken)).ToDictionary(x => (x.IdJob, x.IdType));

            return (await MessageRowsAsync(query, cancellationToken))
                .GroupBy(x => (x.IdJob, x.IdType))
                .Select(group =>
                {
                    var job = jobs.GetValueOrDefault(group.Key);
                    var latest = group.First();

                    return new MessageJobSummaryModel
                    {
                        Company = Company,
                        IdApp = job?.IdApp ?? 0,
                        IdJob = group.Key.IdJob,
                        IdType = group.Key.IdType,
                        JobTitle = job?.Title,
                        Errors = group.Count(x => x.Level == MessageLevel.Error),
                        Warnings = group.Count(x => x.Level == MessageLevel.Warning),
                        Causes = group.Select(x => MessageGrouping.Key(x.Level, x.Message)).Distinct().Count(),
                        LastErrorAt = latest.StartDate,
                        LastMessage = latest.Message,
                        IsActive = job?.IsActive ?? false,
                        CurrentHealth = job is null ? null : Health(job, now),
                        IntervalTimeInMinutes = job?.IntervalTimeInMinutes ?? 0,
                        RunningDailyAt = DailyAt(job?.RunningDailyAt),
                        LastStartDate = job?.StartDate,
                        LastEndDate = job?.EndDate,
                        Acknowledgement = job is null ? null : Acknowledgement(job, now)
                    };
                })
                // ainda falhando primeiro, depois os recuperados; em cada bloco, a ocorrência mais recente primeiro
                .OrderBy(x => x.CurrentHealth == JobHealth.Error && x.Acknowledgement is null ? 0 : 1)
                .ThenByDescending(x => x.LastErrorAt)
                .ThenBy(x => x.IdJob)
                .ToList();
        }

        public async Task<IReadOnlyList<MessageGroupModel>> GetMessageGroupsAsync(MessageQuery query, CancellationToken cancellationToken = default)
        {
            return (await MessageRowsAsync(query, cancellationToken))
                .GroupBy(x => MessageGrouping.Key(x.Level, x.Message))
                .Select(group => new MessageGroupModel
                {
                    Key = group.Key,
                    Level = group.First().Level,
                    Example = group.First().Message,
                    Count = group.Count(),
                    FirstAt = group.Min(x => x.StartDate) ?? default,
                    LastAt = group.Max(x => x.StartDate) ?? default
                })
                .OrderByDescending(x => x.LastAt)
                .ToList();
        }

        public async Task<IReadOnlyList<TypeModel>> GetTypesAsync(CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var types = await connection.QueryAsync<TypeModel>(new CommandDefinition(
                "SELECT IdType, JobType FROM [auditing].[Types] WITH (NOLOCK) ORDER BY IdType", cancellationToken: cancellationToken));

            return types.ToList();
        }

        public async Task<IReadOnlyList<ModuleSettingModel>> GetModuleSettingsAsync(int idApp, CancellationToken cancellationToken = default)
        {
            var settings = new List<ModuleSettingModel>();

            // credenciais e parâmetros da tabela *Parameters do módulo, a linha da empresa; segredo só mascarado
            if (ModuleDefinitions.Find(idApp) is { } definition
                && await parameters.GetParametersAsync(idApp, Company, cancellationToken) is { } row)
            {
                settings.AddRange(definition.Fields.Select(field => new ModuleSettingModel
                {
                    Label = field.Label,
                    Value = field.Secret
                        ? row.SecretsSet.Contains(field.Column) ? Masked : null
                        : row.Values.GetValueOrDefault(field.Column),
                    IsSecret = field.Secret
                }));
            }

            // parâmetros de apresentação e acesso (monitor.AppSetting): o da holding substitui o padrão ('*') de mesmo nome
            await using var connection = await database.OpenAsync(cancellationToken);
            var appSettings = await connection.QueryAsync<(string Name, string? Value, bool IsSecret)>(new CommandDefinition("""
                SELECT s.[Name], s.[Value], s.IsSecret
                FROM [monitor].[AppSetting] s WITH (NOLOCK)
                WHERE s.IdParent = @idApp
                    AND (s.Holder = @holder
                        OR (s.Holder = '*' AND NOT EXISTS (
                            SELECT 1 FROM [monitor].[AppSetting] o WITH (NOLOCK) WHERE o.IdParent = s.IdParent AND o.Holder = @holder AND o.[Name] = s.[Name])))
                ORDER BY s.[Name]
                """, new { idApp, holder = Holder }, cancellationToken: cancellationToken));

            settings.AddRange(appSettings.Select(x => new ModuleSettingModel
            {
                Label = x.Name,
                Value = x.IsSecret && x.Value is not null ? Masked : x.Value,
                IsSecret = x.IsSecret
            }));

            return settings;
        }

        public async Task SetJobActiveAsync(int idJob, int idType, bool isActive, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var updated = await connection.ExecuteAsync(new CommandDefinition(
                "UPDATE [auditing].[Jobs] SET IsActive = @isActive WHERE Company = @company AND IdJob = @idJob AND IdType = @idType",
                new { company = Company, idJob, idType, isActive }, cancellationToken: cancellationToken));

            Changed();
            if (updated == 0)
                throw JobNotFound(idJob, idType);
        }

        public async Task SetModuleActiveAsync(int idApp, bool isActive, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE j
                SET IsActive = @isActive
                FROM
                    [auditing].[Jobs] j
                    JOIN [auditing].[fn_CatalogTree](@idApp) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType
                WHERE
                    j.Company = @company
                """, new { company = Company, idApp, isActive }, cancellationToken: cancellationToken));

            Changed();
        }

        public async Task SetJobScheduleAsync(int idJob, int idType, int intervalTimeInMinutes, TimeOnly? runningDailyAt, string queue, CancellationToken cancellationToken = default)
        {
            if (intervalTimeInMinutes < 0)
                throw new ArgumentOutOfRangeException(nameof(intervalTimeInMinutes), "O intervalo não pode ser negativo.");

            // com hora diária, o intervalo é 1440 (CK_Jobs_RunningDailyAt); a fila segue o CK_Jobs_Queue
            await using var connection = await database.OpenAsync(cancellationToken);
            var updated = await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE [auditing].[Jobs]
                SET IntervalTimeInMinutes = @interval, RunningDailyAt = @dailyAt, Queue = @queue
                WHERE Company = @company AND IdJob = @idJob AND IdType = @idType
                """,
                new
                {
                    company = Company,
                    idJob,
                    idType,
                    interval = runningDailyAt is null ? intervalTimeInMinutes : 1440,
                    dailyAt = runningDailyAt?.ToTimeSpan(),
                    queue = queue.Trim()
                }, cancellationToken: cancellationToken));

            Changed();
            if (updated == 0)
                throw JobNotFound(idJob, idType);
        }

        public async Task<IReadOnlyList<SchedulingHostModel>> GetSchedulingHostsAsync(CancellationToken cancellationToken = default)
        {
            // as linhas sem heartbeat há mais de 2 min já não contam: o próximo heartbeat de qualquer host as apaga
            await using var connection = await database.OpenAsync(cancellationToken);
            var hosts = await connection.QueryAsync<SchedulingHostModel>(new CommandDefinition("""
                SELECT HostType, COUNT(*) AS Instances, MIN(StartedAt) AS StartedAt, MAX(LastHeartbeat) AS LastHeartbeat
                FROM [auditing].[SchedulingHosts] WITH (NOLOCK)
                WHERE LastHeartbeat >= DATEADD(MINUTE, -2, SYSUTCDATETIME())
                GROUP BY HostType
                ORDER BY MIN(StartedAt), HostType
                """, cancellationToken: cancellationToken));

            return hosts.ToList();
        }

        public async Task<IReadOnlyList<string>> GetQueuesAsync(CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var queues = await connection.QueryAsync<string>(new CommandDefinition(
                "SELECT DISTINCT Queue FROM [auditing].[Jobs] WITH (NOLOCK) WHERE Company = @company ORDER BY Queue",
                new { company = Company }, cancellationToken: cancellationToken));

            return queues.ToList();
        }

        public async Task AcknowledgeAsync(int idJob, int idType, string? note, CancellationToken cancellationToken = default)
        {
            // reconhece a última execução do job (ou a falta dela) e substitui um reconhecimento anterior
            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition($"""
                DECLARE @now datetime = {MainDatabase.Now};
                DECLARE @execution uniqueidentifier = (
                    SELECT TOP (1) Execution
                    FROM [auditing].[Logs] WITH (NOLOCK)
                    WHERE Company = @company AND IdJob = @idJob AND IdType = @idType
                    ORDER BY StartDate DESC);

                DELETE FROM [monitor].[JobAcknowledgement] WHERE Company = @company AND IdJob = @idJob AND IdType = @idType;

                INSERT INTO [monitor].[JobAcknowledgement] (IdJob, IdType, Holder, Company, Execution, Note, [User], [At], ExpiresAt)
                VALUES (@idJob, @idType, @holder, @company, @execution, @note, @user, @now, DATEADD(MINUTE, @lifetime, @now));
                """,
                new
                {
                    holder = Holder,
                    company = Company,
                    idJob,
                    idType,
                    note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                    user = User,
                    lifetime = (int)AcknowledgementLifetime.TotalMinutes
                }, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
            Changed();
        }

        public async Task RemoveAcknowledgementAsync(int idJob, int idType, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM [monitor].[JobAcknowledgement] WHERE Company = @company AND IdJob = @idJob AND IdType = @idType",
                new { company = Company, idJob, idType }, cancellationToken: cancellationToken));

            Changed();
        }

        /// <summary>
        /// O host ainda não lê pedidos de execução do banco (docs/PLANO-WEBAPP.md, 11.5.2: colunas em auditing.Jobs ou a
        /// tabela monitor.JobRunRequest, a decidir com quem cuida do host). Até lá, "Executar agora" só existe no mock.
        /// </summary>
        public Task RequestRunAsync(int idJob, int idType, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("\"Executar agora\" ainda não existe com o banco: o host não lê pedidos de execução (docs/PLANO-WEBAPP.md, 11.5.2).");

        public async Task SetAppImageAsync(int idApp, string contentType, byte[] data, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition($"""
                DELETE FROM [monitor].[AppImage] WHERE IdParent = @idApp AND Holder = @holder;
                INSERT INTO [monitor].[AppImage] (IdParent, Holder, ContentType, [Data], UpdatedAt) VALUES (@idApp, @holder, @contentType, @data, {MainDatabase.Now});
                """, new { idApp, holder = Holder, contentType, data }, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
            Changed();
        }

        public async Task RemoveAppImageAsync(int idApp, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM [monitor].[AppImage] WHERE IdParent = @idApp AND Holder = @holder",
                new { idApp, holder = Holder }, cancellationToken: cancellationToken));

            Changed();
        }

        // jobs da empresa com a última execução e o reconhecimento, do cache
        private async Task<IReadOnlyList<JobRow>> JobsAsync(CancellationToken cancellationToken) =>
            (await cache.GetOrCreateAsync($"{JobsKey}:{Company}:{Volatile.Read(ref _version)}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheLifetime;

                await using var connection = await database.OpenAsync(cancellationToken);
                var rows = await connection.QueryAsync<JobRow>(new CommandDefinition(SelectJobs, new { company = Company }, cancellationToken: cancellationToken));
                return (IReadOnlyList<JobRow>)rows.ToList();
            }))!;

        private async Task<(IReadOnlyList<AppModel> Apps, IReadOnlyList<ImageRow> Images)> AppsAsync(CancellationToken cancellationToken) =>
            await cache.GetOrCreateAsync($"Monitor:Apps:{Holder}:{Volatile.Read(ref _version)}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheLifetime;

                await using var connection = await database.OpenAsync(cancellationToken);
                using var reader = await connection.QueryMultipleAsync(new CommandDefinition(SelectApps, new { holder = Holder }, cancellationToken: cancellationToken));
                var apps = (await reader.ReadAsync<AppModel>()).ToList();
                var images = (await reader.ReadAsync<ImageRow>()).ToList();
                return ((IReadOnlyList<AppModel>)apps, (IReadOnlyList<ImageRow>)images);
            });

        private async Task<IReadOnlyList<TotalRow>> TotalsAsync(DateTime start, DateTime end, int buckets, CancellationToken cancellationToken) =>
            (await cache.GetOrCreateAsync($"{TotalsKey}:{Company}:{Volatile.Read(ref _version)}:{start:O}:{end:O}:{buckets}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheLifetime;

                // faixa de largura inteira em segundos; com uma faixa só, todas as execuções caem na 0
                var width = Math.Max(1L, (long)Math.Ceiling((end - start).TotalSeconds / buckets));

                await using var connection = await database.OpenAsync(cancellationToken);
                var rows = await connection.QueryAsync<TotalRow>(new CommandDefinition(SelectTotals,
                    new { company = Company, start, end, width }, commandTimeout: 60, cancellationToken: cancellationToken));
                return (IReadOnlyList<TotalRow>)rows.ToList();
            }))!;

        // mensagens do filtro sem o texto longo (SqlCommand, exceção), para agrupar por causa e por job em C#
        private async Task<IReadOnlyList<MessageRow>> MessageRowsAsync(MessageQuery query, CancellationToken cancellationToken)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var rows = (await connection.QueryAsync<MessageRow>(new CommandDefinition($"""
                {JobAppsCte}
                SELECT TOP (@max)
                    m.IdExecutionMessage,
                    m.IdJob,
                    m.IdType,
                    CAST(m.[Level] AS int) AS [Level],
                    m.[Message],
                    l.StartDate
                {MessagesFrom}
                ORDER BY l.StartDate DESC, m.IdExecutionMessage DESC
                """, MessageFilter(query, new { max = MaxGroupedMessages + 1 }), commandTimeout: 60, cancellationToken: cancellationToken))).ToList();

            if (rows.Count > MaxGroupedMessages)
            {
                logger.LogWarning("Logs de Erros: mais de {Max} mensagens no filtro {Query}; as mais antigas ficaram fora dos totais e das causas.", MaxGroupedMessages, query);
                rows.RemoveAt(rows.Count - 1);
            }

            return rows;
        }

        private async Task<IReadOnlyList<MessageModel>> MessagesByIdAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken)
        {
            if (ids.Count == 0)
                return [];

            await using var connection = await database.OpenAsync(cancellationToken);
            var messages = (await connection.QueryAsync<MessageModel>(new CommandDefinition($"""
                {JobAppsCte}
                SELECT {MessageColumns}
                FROM
                    [auditing].[Messages] m WITH (NOLOCK)
                    JOIN [auditing].[Logs] l WITH (NOLOCK) ON l.Company = m.Company AND l.IdJob = m.IdJob AND l.IdType = m.IdType AND l.Execution = m.Execution
                    JOIN [auditing].[Jobs] j WITH (NOLOCK) ON j.Company = m.Company AND j.IdJob = m.IdJob AND j.IdType = m.IdType
                    JOIN [auditing].[Catalog] cat WITH (NOLOCK) ON cat.IdJob = j.IdJob AND cat.IdType = j.IdType
                    LEFT JOIN JobApps ja ON ja.IdJob = m.IdJob AND ja.IdType = m.IdType
                WHERE
                    m.Company = @company AND m.IdExecutionMessage IN @ids
                """, new { company = Company, ids }, cancellationToken: cancellationToken))).ToDictionary(x => x.IdExecutionMessage);

            // na ordem da página (a da consulta de causas), não na do banco
            return await WithAppTitles(ids.Where(messages.ContainsKey).Select(id => messages[id]).ToList(), cancellationToken);
        }

        private async Task<IReadOnlyList<MessageModel>> WithAppTitles(List<MessageModel> messages, CancellationToken cancellationToken)
        {
            var (apps, _) = await AppsAsync(cancellationToken);
            var titles = apps.ToDictionary(x => x.IdParent, x => x.Title);

            foreach (var message in messages)
            {
                message.Company = Company;
                message.AppTitle = message.IdApp is int idApp ? titles.GetValueOrDefault(idApp) : null;
            }

            return messages;
        }

        // parâmetros de MessagesFrom; a busca vira LIKE com os curingas do texto escapados
        private DynamicParameters MessageFilter(MessageQuery query, object extra)
        {
            var search = string.IsNullOrWhiteSpace(query.Search)
                ? null
                : "%" + query.Search.Trim().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";

            var parameters = new DynamicParameters(extra);
            parameters.AddDynamicParams(new
            {
                company = Company,
                since = MainDatabase.LocalNow() - query.Period.ToTimeSpan(),
                minLevel = (int)query.MinLevel,
                idApp = query.IdApp,
                idJob = query.IdJob,
                idType = query.IdType,
                search = new DbString { Value = search, IsAnsi = false, Length = 4000 }
            });

            return parameters;
        }

        /// <summary>
        /// Saúde pela última execução, com 1 × o intervalo de prazo (a mesma regra da MockTimeline): nula para job inativo ou
        /// sob demanda que ainda não rodou.
        /// </summary>
        private static JobHealth? Health(JobRow job, DateTime now)
        {
            if (!job.IsActive)
                return null;

            if (job.Execution is null)
                return job.IntervalTimeInMinutes > 0 ? JobHealth.Late : null;

            if (job.EndDate is null)
                return JobHealth.Running;

            if (job.Errors > 0)
                return JobHealth.Error;

            if (job.IntervalTimeInMinutes > 0 && now - job.StartDate > TimeSpan.FromMinutes(job.IntervalTimeInMinutes))
                return JobHealth.Late;

            return JobHealth.Ok;
        }

        // o reconhecimento vale para a execução reconhecida e até expirar
        private static AcknowledgementModel? Acknowledgement(JobRow job, DateTime now)
        {
            if (job.AckAt is not DateTime at || job.AckExpiresAt is not DateTime expiresAt)
                return null;

            if (job.AckExecution != job.Execution || now >= expiresAt)
                return null;

            return new AcknowledgementModel { Execution = job.AckExecution, Note = job.AckNote, User = job.AckUser, At = at, ExpiresAt = expiresAt };
        }

        private static ExecutionModel? LastExecution(JobRow job) => job.Execution is not Guid execution
            ? null
            : new ExecutionModel
            {
                IdExecution = job.IdExecution ?? 0,
                IdJob = job.IdJob,
                IdType = job.IdType,
                Execution = execution,
                ExecutionParent = job.ExecutionParent,
                StartDate = job.StartDate ?? default,
                EndDate = job.EndDate,
                Records = job.Records,
                Errors = job.Errors,
                Warnings = job.Warnings
            };

        // a mesma regra de JobModel.IsOnDemand
        private static bool IsOnDemand(JobRow job) => job.IdType == 2 && job.IntervalTimeInMinutes == 0 && job.RunningDailyAt is null;

        private static TimeOnly? DailyAt(TimeSpan? value) => value is TimeSpan time ? TimeOnly.FromTimeSpan(time) : null;

        // a imagem própria da empresa ou a padrão do módulo ('*'); sem nenhuma, o card mostra as iniciais
        private static string? ImageUrlOf(int idApp, IReadOnlyList<ImageRow> images)
        {
            var image = images.FirstOrDefault(x => x.IdParent == idApp && x.Holder != "*") ?? images.FirstOrDefault(x => x.IdParent == idApp);
            return image is null ? null : $"data:{image.ContentType};base64,{Convert.ToBase64String(image.Data)}";
        }

        private static DateTime MinuteOf(DateTime now) => now.AddTicks(-(now.Ticks % TimeSpan.TicksPerMinute));

        private static int Clamp(long value) => (int)Math.Min(value, int.MaxValue);

        private static void Changed() => Interlocked.Increment(ref _version);

        private InvalidOperationException JobNotFound(int idJob, int idType) =>
            new($"Job {idJob} (tipo {idType}) não existe para a empresa {Company}.");

        private sealed class JobRow
        {
            public int? IdParent { get; set; }
            public int? IdApp { get; set; }
            public int IdJob { get; set; }
            public int IdType { get; set; }
            public string? JobType { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public int IntervalTimeInMinutes { get; set; }
            public bool IsActive { get; set; }
            public TimeSpan? RunningDailyAt { get; set; }
            public string? Queue { get; set; }
            public long? IdExecution { get; set; }
            public Guid? Execution { get; set; }
            public Guid? ExecutionParent { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int Records { get; set; }
            public int Errors { get; set; }
            public int Warnings { get; set; }
            public Guid? AckExecution { get; set; }
            public string? AckNote { get; set; }
            public string? AckUser { get; set; }
            public DateTime? AckAt { get; set; }
            public DateTime? AckExpiresAt { get; set; }
        }

        private sealed class TotalRow
        {
            public int IdJob { get; set; }
            public int IdType { get; set; }
            public int Bucket { get; set; }
            public int Executions { get; set; }
            public int FailedExecutions { get; set; }
            public long Records { get; set; }
            public int Errors { get; set; }
            public int Warnings { get; set; }
            public DateTime? LastErrorAt { get; set; }
        }

        private sealed class MessageRow
        {
            public long IdExecutionMessage { get; set; }
            public int IdJob { get; set; }
            public int IdType { get; set; }
            public MessageLevel Level { get; set; }
            public string? Message { get; set; }
            public DateTime? StartDate { get; set; }
        }

        private sealed class ImageRow
        {
            public int IdParent { get; set; }
            public string Holder { get; set; } = "";
            public string ContentType { get; set; } = "";
            public byte[] Data { get; set; } = [];
        }
    }
}
