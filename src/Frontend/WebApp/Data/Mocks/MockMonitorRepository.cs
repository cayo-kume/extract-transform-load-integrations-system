using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Implementação falsa do IMonitorRepository: os jobs reais do MockCatalog e execuções geradas pelo MockTimeline.
    /// Registrada como singleton, porque não guarda estado por usuário e mantém um cache curto dos resumos. A MockTimeline é
    /// a mesma do MockOnDemandRunner, para uma chamada da tela Sob Demanda virar uma execução do job.
    /// </summary>
    internal class MockMonitorRepository : IMonitorRepository
    {
        private static readonly IReadOnlyList<TypeModel> Types =
        [
            new() { IdType = 1, JobType = "Procedure" },
            new() { IdType = 2, JobType = "Integração" }
        ];

        // até a fase 6 (autenticação), quem reconhece e quem pede execução é um nome fixo
        private const string User = "operador";
        private static readonly TimeSpan AcknowledgementLifetime = TimeSpan.FromHours(24);

        // o host lê os pedidos de "Executar agora" a cada ciclo da agenda; no mock, a execução começa 1 minuto depois
        private static readonly TimeSpan HostPickup = TimeSpan.FromMinutes(1);

        private readonly MockTimeline _timeline;
        private readonly object _cacheLock = new();
        private readonly object _stateLock = new();
        private readonly double _failureRate;

        // imagens próprias por aplicação (a da empresa logada); quem não tem aqui usa a imagem padrão do módulo
        private readonly Dictionary<int, (string ContentType, byte[] Data)> _images = new();
        private readonly Dictionary<(int IdJob, int IdType), AcknowledgementModel> _acknowledgements = new();
        private readonly Dictionary<(int IdJob, int IdType), RunRequestModel> _runRequests = new();

        // agenda alterada pelo diálogo de configurações; no mock, muda o que as telas mostram, mas as execuções continuam
        // geradas pela agenda do catálogo (quem passaria a rodar na agenda nova é o host)
        private readonly Dictionary<(int IdJob, int IdType), (int Interval, TimeOnly? DailyAt, string Queue)> _schedules = new();

        private (Period Period, DateTime Minute, IReadOnlyList<AppSummaryModel> Result)? _summariesCache;
        private readonly Dictionary<(Period Period, int Buckets, bool Previous), (DateTime Minute, IReadOnlyList<AppBucketsModel> Result)> _bucketsCache = new();

        /// <summary>
        /// MockMonitorRepository:FailureRate (de 0 a 1) faz uma parte das chamadas falhar, para testar o tratamento de erro
        /// das telas sem banco. Fica 0 no appsettings.json.
        /// </summary>
        public MockMonitorRepository(IConfiguration configuration, MockTimeline timeline)
        {
            _failureRate = configuration.GetValue<double>("MockMonitorRepository:FailureRate");
            _timeline = timeline;
            _company = configuration["LoggedCompany:DocCompany"] is { Length: > 0 } doc ? doc : "44065634000106";
        }

        // as execuções geradas são todas da empresa da configuração (a mesma do MockCompanyStore); as outras empresas da
        // holding não têm execução no mock: For devolve uma visão vazia delas
        private readonly string _company;

        public IMonitorRepository For(string company) =>
            string.IsNullOrEmpty(company) || company == _company ? this : new MockEmptyCompanyMonitor(this, company);

        public async Task<IReadOnlyList<AppSummaryModel>> GetAppSummariesAsync(Period period, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;
            var minute = MinuteOf(now);

            lock (_cacheLock)
            {
                if (_summariesCache is { } cache && cache.Period == period && cache.Minute == minute)
                    return cache.Result;
            }

            var from = now - period.ToTimeSpan();
            var result = _timeline.Apps.Values
                .OrderBy(x => x.Title)
                .Select(app =>
                {
                    var jobs = _timeline.Jobs.Where(x => x.IdApp == app.IdParent).ToList();
                    var executions = jobs.SelectMany(x => _timeline.Between(x, from, now, now)).ToList();

                    return new AppSummaryModel
                    {
                        IdParent = app.IdParent,
                        Title = app.Title,
                        Description = app.Description,
                        ImageUrl = ImageUrlOf(app.IdParent),
                        Integrations = jobs.Count(x => x.Catalog.IdType == 2),
                        Procedures = jobs.Count(x => x.Catalog.IdType == 1),
                        ActiveJobs = jobs.Count(x => x.IsActive),
                        Executions = executions.Count,
                        FailedExecutions = executions.Count(x => x.Errors > 0),
                        Records = executions.Sum(x => x.Records),
                        Warnings = executions.Sum(x => x.Warnings),
                        Errors = executions.Sum(x => x.Errors),
                        LastErrorAt = executions.Where(x => x.Errors > 0).Select(x => (DateTime?)x.StartDate).Max(),
                        FailingJobs = jobs.Count(x => _timeline.Health(x, _timeline.Last(x, now), now) == JobHealth.Error)
                    };
                })
                .ToList();

            lock (_cacheLock)
                _summariesCache = (period, minute, result);

            return result;
        }

        public async Task<IReadOnlyList<AppBucketsModel>> GetExecutionBucketsAsync(Period period, int buckets, bool previousWindow = false, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;
            var minute = MinuteOf(now);
            var key = (period, buckets, previousWindow);

            lock (_cacheLock)
            {
                if (_bucketsCache.TryGetValue(key, out var cache) && cache.Minute == minute)
                    return cache.Result;
            }

            // faixas alinhadas à hora (ou ao dia) cheia: a última termina na próxima hora cheia depois de agora
            var width = period.ToTimeSpan() / buckets;
            var alignment = width >= TimeSpan.FromDays(1) ? TimeSpan.FromDays(1) : width >= TimeSpan.FromHours(1) ? TimeSpan.FromHours(1) : width;
            var end = new DateTime((now.Ticks + alignment.Ticks - 1) / alignment.Ticks * alignment.Ticks);
            if (previousWindow)
                end -= width * buckets;
            var start = end - width * buckets;

            var result = _timeline.Apps.Values
                .OrderBy(x => x.Title)
                .Select(app =>
                {
                    var counts = Enumerable.Range(0, buckets)
                        .Select(i => new ExecutionBucketModel { Start = start + width * i, End = start + width * (i + 1) })
                        .ToArray();

                    foreach (var execution in _timeline.Jobs.Where(x => x.IdApp == app.IdParent).SelectMany(x => _timeline.Between(x, start, end, now)))
                    {
                        var bucket = counts[Math.Clamp((int)((execution.StartDate - start) / width), 0, buckets - 1)];
                        bucket.Executions++;
                        bucket.FailedExecutions += execution.Errors > 0 ? 1 : 0;
                        bucket.Errors += execution.Errors;
                        bucket.Records += execution.Records;
                    }

                    return new AppBucketsModel { IdApp = app.IdParent, Buckets = counts };
                })
                .ToList();

            lock (_cacheLock)
                _bucketsCache[key] = (minute, result);

            return result;
        }

        public async Task<IReadOnlyList<JobModel>> GetJobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;

            return _timeline.Jobs
                .Where(x => idApp is null || x.IdApp == idApp)
                .Where(x => idType is null || x.Catalog.IdType == idType)
                .Select(x =>
                {
                    var last = _timeline.Last(x, now);
                    var schedule = ScheduleOf(x);

                    return new JobModel
                    {
                        Company = _company,
                        IdParent = x.Catalog.IdParent,
                        IdApp = x.IdApp,
                        IdJob = x.Catalog.IdJob,
                        IdType = x.Catalog.IdType,
                        JobType = JobType(x.Catalog.IdType),
                        Title = x.Catalog.Title,
                        Description = x.Catalog.Description,
                        IntervalTimeInMinutes = schedule.Interval,
                        IsActive = x.IsActive,
                        RunningDailyAt = schedule.DailyAt,
                        Queue = schedule.Queue,
                        LastExecution = last is null ? null : ToModel(last),
                        Health = _timeline.Health(x, last, now),
                        Acknowledgement = AcknowledgementOf(x, last, now),
                        RunRequest = RunRequestOf(x, now)
                    };
                })
                .ToList();
        }

        public async Task<PagedResult<ExecutionModel>> GetExecutionsAsync(int idJob, int idType, Period period, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var job = _timeline.Find(idJob, idType);
            if (job is null)
                return new PagedResult<ExecutionModel>();

            var now = DateTime.Now;
            var executions = _timeline.Between(job, now - period.ToTimeSpan(), now, now).ToList();
            executions.Reverse();

            return new PagedResult<ExecutionModel>
            {
                Items = executions.Skip(page * pageSize).Take(pageSize).Select(ToModel).ToList(),
                TotalCount = executions.Count
            };
        }

        public async Task<ExecutionModel?> GetExecutionAsync(Guid execution, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var found = _timeline.Decode(execution, DateTime.Now);
            return found is null ? null : ToModel(found);
        }

        public async Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid execution, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var found = _timeline.Decode(execution, DateTime.Now);
            return found is null ? [] : MockTexts.Messages(found, AppTitle(found.Job));
        }

        public async Task<PagedResult<RecordModel>> GetRecordsAsync(Guid execution, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var found = _timeline.Decode(execution, DateTime.Now);
            if (found is null)
                return new PagedResult<RecordModel>();

            return new PagedResult<RecordModel>
            {
                Items = Enumerable.Range(page * pageSize, Math.Max(0, Math.Min(pageSize, found.Records - page * pageSize)))
                    .Select(i => MockTexts.Record(found, i))
                    .ToList(),
                TotalCount = found.Records
            };
        }

        public async Task<IReadOnlyList<HeartbeatModel>> GetHeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;
            var from = now - period.ToTimeSpan();

            // a visão geral mostra só integrações ativas e com agenda (decisão de 31/10/2026), as falhas antes de qualquer Ok
            return _timeline.Jobs
                .Where(x => x.IsActive && x.Catalog.IdType == 2 && !IsOnDemand(x))
                .Select(x =>
                {
                    var last = _timeline.Last(x, now);
                    return (Job: x, Last: last, Health: _timeline.Health(x, last, now), Acknowledgement: AcknowledgementOf(x, last, now));
                })
                .Where(x => x.Health is JobHealth.Error or JobHealth.Late || (x.Last is not null && x.Last.StartDate >= from))
                .OrderBy(x => SeverityRank(x.Health, x.Acknowledgement))
                .ThenByDescending(x => x.Last?.StartDate ?? DateTime.MinValue)
                .Take(top)
                .Select(x => new HeartbeatModel
                {
                    Company = _company,
                    IdApp = x.Job.IdApp,
                    AppTitle = AppTitle(x.Job),
                    IdJob = x.Job.Catalog.IdJob,
                    IdType = x.Job.Catalog.IdType,
                    JobType = JobType(x.Job.Catalog.IdType),
                    JobTitle = x.Job.Catalog.Title,
                    StartDate = x.Last?.StartDate,
                    EndDate = x.Last?.EndDate,
                    Records = x.Last?.Records ?? 0,
                    Health = x.Health ?? JobHealth.Ok,
                    HasWarnings = x.Last?.Warnings > 0,
                    IntervalTimeInMinutes = x.Job.Catalog.IntervalTimeInMinutes,
                    RunningDailyAt = x.Job.Catalog.RunningDailyAt,
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

        public async Task<IReadOnlyList<MessageJobSummaryModel>> GetMessageJobsAsync(MessageQuery query, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;

            return Messages(query, now)
                .GroupBy(x => x.Execution.Job)
                .Select(group =>
                {
                    var job = group.Key;
                    var last = _timeline.Last(job, now);
                    var latest = group.First();

                    return new MessageJobSummaryModel
                    {
                        Company = _company,
                        IdApp = job.IdApp,
                        IdJob = job.Catalog.IdJob,
                        IdType = job.Catalog.IdType,
                        JobTitle = job.Catalog.Title,
                        Errors = group.Count(x => x.Message.Level == MessageLevel.Error),
                        Warnings = group.Count(x => x.Message.Level == MessageLevel.Warning),
                        Causes = group.Select(x => MessageGrouping.Key(x.Message)).Distinct().Count(),
                        LastErrorAt = latest.Execution.StartDate,
                        LastMessage = latest.Message.Message,
                        IsActive = job.IsActive,
                        CurrentHealth = _timeline.Health(job, last, now),
                        IntervalTimeInMinutes = job.Catalog.IntervalTimeInMinutes,
                        RunningDailyAt = job.Catalog.RunningDailyAt,
                        LastStartDate = last?.StartDate,
                        LastEndDate = last?.EndDate,
                        Acknowledgement = AcknowledgementOf(job, last, now)
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
            await Delay(cancellationToken);

            return Messages(query, DateTime.Now)
                .GroupBy(x => MessageGrouping.Key(x.Message))
                .Select(group => new MessageGroupModel
                {
                    Key = group.Key,
                    Level = group.First().Message.Level,
                    Example = group.First().Message.Message,
                    Count = group.Count(),
                    FirstAt = group.Min(x => x.Execution.StartDate),
                    LastAt = group.Max(x => x.Execution.StartDate)
                })
                .OrderByDescending(x => x.LastAt)
                .ToList();
        }

        public async Task<PagedResult<MessageModel>> GetMessagesByLevelAsync(MessageQuery query, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;

            // com busca ou causa, é preciso gerar o texto de todas as mensagens para filtrar
            if (!string.IsNullOrWhiteSpace(query.Search) || query.GroupKey is not null)
            {
                var all = Messages(query, now).Select(x => x.Message).ToList();
                return new PagedResult<MessageModel>
                {
                    Items = all.Skip(page * pageSize).Take(pageSize).ToList(),
                    TotalCount = all.Count
                };
            }

            var executions = ExecutionsWithMessages(query, now).ToList();

            // cada execução tem um erro ou de 1 a 3 avisos: conta as mensagens sem gerar o texto de todas
            int Count(MockExecution x) => (query.MinLevel <= MessageLevel.Warning ? x.Warnings : 0) + x.Errors;

            var skip = page * pageSize;
            var items = new List<MessageModel>();

            foreach (var execution in executions)
            {
                var count = Count(execution);
                if (skip >= count)
                {
                    skip -= count;
                    continue;
                }

                foreach (var message in MessagesOf(execution, query.MinLevel).Skip(skip))
                {
                    items.Add(message);

                    if (items.Count == pageSize)
                        break;
                }

                skip = 0;
                if (items.Count == pageSize)
                    break;
            }

            return new PagedResult<MessageModel>
            {
                Items = items,
                TotalCount = executions.Sum(Count)
            };
        }

        // execuções com mensagens do nível pedido no período, da mais recente para a mais antiga
        private IEnumerable<MockExecution> ExecutionsWithMessages(MessageQuery query, DateTime now)
        {
            var from = now - query.Period.ToTimeSpan();

            return _timeline.Jobs
                .Where(x => query.IdApp is null || x.IdApp == query.IdApp)
                .Where(x => query.IdJob is null || (x.Catalog.IdJob == query.IdJob && x.Catalog.IdType == query.IdType))
                .SelectMany(x => _timeline.Between(x, from, now, now))
                .Where(x => x.Errors > 0 || (query.MinLevel <= MessageLevel.Warning && x.Warnings > 0))
                .OrderByDescending(x => x.StartDate);
        }

        // as mensagens do filtro, com o texto gerado, da execução mais recente para a mais antiga
        private IEnumerable<(MockExecution Execution, MessageModel Message)> Messages(MessageQuery query, DateTime now) =>
            ExecutionsWithMessages(query, now)
                .SelectMany(execution => MessagesOf(execution, query.MinLevel).Select(message => (execution, message)))
                .Where(x => MessageGrouping.Matches(x.message, query.Search))
                .Where(x => query.GroupKey is null || MessageGrouping.Key(x.message) == query.GroupKey);

        private IEnumerable<MessageModel> MessagesOf(MockExecution execution, MessageLevel minLevel)
        {
            var appTitle = AppTitle(execution.Job);

            foreach (var message in MockTexts.Messages(execution, appTitle).Where(x => x.Level >= minLevel))
            {
                message.Company = _company;
                message.IdApp = execution.Job.IdApp;
                message.AppTitle = appTitle;
                message.JobTitle = execution.Job.Catalog.Title;
                message.ExecutionStartDate = execution.StartDate;
                yield return message;
            }
        }

        public async Task<IReadOnlyList<TypeModel>> GetTypesAsync(CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);
            return Types;
        }

        public async Task<IReadOnlyList<ModuleSettingModel>> GetModuleSettingsAsync(int idApp, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);
            return ModuleSettings.GetValueOrDefault(idApp, []);
        }

        private const string Masked = "••••••••••••••••";

        // credenciais e parâmetros globais por módulo, no formato dos scripts de Parameters; valores de credencial mascarados.
        // Base URL e Homolog URL não entram: a aba Parâmetros edita as duas no formulário
        private static readonly Dictionary<int, IReadOnlyList<ModuleSettingModel>> ModuleSettings = new()
        {
            [30] =
            [
                new() { Label = "Token da API", Value = Masked, IsSecret = true },
                new() { Label = "Timeout das chamadas (segundos)", Value = "60" }
            ],
            [50] =
            [
                new() { Label = "API Key", Value = Masked, IsSecret = true },
                new() { Label = "Timeout das chamadas (segundos)", Value = "60" }
            ],
            [70] =
            [
                new() { Label = "Chave da API", Value = Masked, IsSecret = true },
                new() { Label = "URL base da API", Value = "https://api.core.dcg.com.br" },
                new() { Label = "Timeout das chamadas (segundos)", Value = "90" }
            ],
            [80] =
            [
                new() { Label = "Chave do ERP", Value = Masked, IsSecret = true },
                new() { Label = "Chave do B2C", Value = Masked, IsSecret = true },
                new() { Label = "Timeout das chamadas (segundos)", Value = "120" },
                new() { Label = "Tamanho do lote (Bulk Insert)", Value = "5000" }
            ],
            [90] =
            [
                new() { Label = "Timeout das procedures (segundos)", Value = "600" },
                new() { Label = "Tamanho do lote (Bulk Insert)", Value = "10000" }
            ],
            [130] =
            [
                new() { Label = "Client Id", Value = Masked, IsSecret = true },
                new() { Label = "Client Secret", Value = Masked, IsSecret = true },
                new() { Label = "Timeout das chamadas (segundos)", Value = "120" }
            ]
        };

        public async Task SetJobActiveAsync(int idJob, int idType, bool isActive, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            FindJob(idJob, idType).ActiveOverride = isActive;
            Changed();
        }

        public async Task SetModuleActiveAsync(int idApp, bool isActive, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            foreach (var job in _timeline.Jobs.Where(x => x.IdApp == idApp))
                job.ActiveOverride = isActive;

            Changed();
        }

        public async Task SetJobScheduleAsync(int idJob, int idType, int intervalTimeInMinutes, TimeOnly? runningDailyAt, string queue, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            FindJob(idJob, idType);
            if (intervalTimeInMinutes < 0)
                throw new ArgumentOutOfRangeException(nameof(intervalTimeInMinutes), "O intervalo não pode ser negativo.");

            lock (_stateLock)
                _schedules[(idJob, idType)] = (runningDailyAt is null ? intervalTimeInMinutes : 1440, runningDailyAt, queue.Trim());

            Changed();
        }

        public async Task<IReadOnlyList<string>> GetQueuesAsync(CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            return _timeline.Jobs.Select(x => ScheduleOf(x).Queue).Distinct().Order().ToList();
        }

        // um WebApi agendando há horas e um Worker no ar em espera, para a tela Hosts de Agenda mostrar o aviso
        public async Task<IReadOnlyList<SchedulingHostModel>> GetSchedulingHostsAsync(CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.UtcNow;
            return
            [
                new SchedulingHostModel { HostType = "Hangfire", Instances = 1, StartedAt = now.AddHours(-3), LastHeartbeat = now.AddSeconds(-20) },
                new SchedulingHostModel { HostType = "Quartz", Instances = 1, StartedAt = now.AddMinutes(-40), LastHeartbeat = now.AddSeconds(-35) }
            ];
        }

        // a mesma regra de JobModel.IsOnDemand, com a agenda alterada pelo diálogo
        private bool IsOnDemand(MockJob job) =>
            job.Catalog.IdType == 2 && ScheduleOf(job) is { Interval: 0, DailyAt: null };

        private (int Interval, TimeOnly? DailyAt, string Queue) ScheduleOf(MockJob job)
        {
            lock (_stateLock)
            {
                return _schedules.TryGetValue((job.Catalog.IdJob, job.Catalog.IdType), out var schedule)
                    ? schedule
                    : (job.Catalog.IntervalTimeInMinutes, job.Catalog.RunningDailyAt, job.Catalog.Queue);
            }
        }

        public async Task AcknowledgeAsync(int idJob, int idType, string? note, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var job = FindJob(idJob, idType);
            var now = DateTime.Now;

            lock (_stateLock)
            {
                _acknowledgements[(idJob, idType)] = new AcknowledgementModel
                {
                    Execution = _timeline.Last(job, now)?.Execution,
                    Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                    User = User,
                    At = now,
                    ExpiresAt = now + AcknowledgementLifetime
                };
            }

            Changed();
        }

        public async Task RemoveAcknowledgementAsync(int idJob, int idType, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            lock (_stateLock)
                _acknowledgements.Remove((idJob, idType));

            Changed();
        }

        public async Task RequestRunAsync(int idJob, int idType, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var job = FindJob(idJob, idType);
            var now = DateTime.Now;

            lock (_stateLock)
            {
                // um pedido pendente por job: pedir de novo não enfileira outra execução
                if (RunRequestOf(job, now) is not null)
                    return;

                _runRequests[(idJob, idType)] = new RunRequestModel { RequestedAt = now, RequestedBy = User };
                _timeline.AddManualRun(job, now + HostPickup);
            }

            Changed();
        }

        public async Task SetAppImageAsync(int idApp, string contentType, byte[] data, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            lock (_stateLock)
                _images[idApp] = (contentType, data);

            Changed();
        }

        public async Task RemoveAppImageAsync(int idApp, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            lock (_stateLock)
                _images.Remove(idApp);

            Changed();
        }

        // o reconhecimento vale para a execução reconhecida e até expirar; depois disso, some sozinho
        private AcknowledgementModel? AcknowledgementOf(MockJob job, MockExecution? last, DateTime now)
        {
            var key = (job.Catalog.IdJob, job.Catalog.IdType);

            lock (_stateLock)
            {
                if (!_acknowledgements.TryGetValue(key, out var acknowledgement))
                    return null;

                if (acknowledgement.Execution == last?.Execution && now < acknowledgement.ExpiresAt)
                    return acknowledgement;

                _acknowledgements.Remove(key);
                return null;
            }
        }

        // o pedido fica pendente até começar uma execução depois dele
        private RunRequestModel? RunRequestOf(MockJob job, DateTime now)
        {
            var key = (job.Catalog.IdJob, job.Catalog.IdType);

            lock (_stateLock)
            {
                if (!_runRequests.TryGetValue(key, out var request))
                    return null;

                if (!job.ManualRuns.Any(x => x >= request.RequestedAt && x <= now))
                    return request;

                _runRequests.Remove(key);
                return null;
            }
        }

        private MockJob FindJob(int idJob, int idType) =>
            _timeline.Find(idJob, idType) ?? throw new InvalidOperationException($"Job {idJob} (tipo {idType}) não existe no catálogo.");

        // qualquer escrita invalida o cache dos resumos, para o card refletir a mudança na próxima carga
        private void Changed()
        {
            lock (_cacheLock)
            {
                _summariesCache = null;
                _bucketsCache.Clear();
            }
        }

        private string? ImageUrlOf(int idApp)
        {
            lock (_stateLock)
            {
                return _images.TryGetValue(idApp, out var image)
                    ? $"data:{image.ContentType};base64,{Convert.ToBase64String(image.Data)}"
                    : null;
            }
        }

        private string AppTitle(MockJob job) => _timeline.Apps[job.IdApp].Title;

        private static string? JobType(int idType) => Types.FirstOrDefault(x => x.IdType == idType)?.JobType;

        private static DateTime MinuteOf(DateTime now) => now.AddTicks(-(now.Ticks % TimeSpan.TicksPerMinute));

        private static ExecutionModel ToModel(MockExecution execution) => new()
        {
            IdExecution = (long)(MockTimeline.Hash(execution.Job, execution.K, 0) % 900_000_000_000) + 1,
            IdJob = execution.Job.Catalog.IdJob,
            IdType = execution.Job.Catalog.IdType,
            Execution = execution.Execution,
            StartDate = execution.StartDate,
            EndDate = execution.EndDate,
            Records = execution.Records,
            Warnings = execution.Warnings,
            Errors = execution.Errors
        };

        // o mesmo atraso que o banco terá, para o SpaceLoading aparecer; com FailureRate, às vezes falha como o banco falharia
        private async Task Delay(CancellationToken cancellationToken)
        {
            await Task.Delay(Random.Shared.Next(150, 400), cancellationToken);

            if (_failureRate > 0 && Random.Shared.NextDouble() < _failureRate)
                throw new InvalidOperationException("Falha simulada do banco (MockMonitorRepository:FailureRate).");
        }
    }
}
