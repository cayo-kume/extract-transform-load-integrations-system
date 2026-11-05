using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// ICompanyRepository no banco principal, com Dapper (sem API e sem Shared): INSERT e UPDATE direto em auditing.Holders,
    /// auditing.Companies e auditing.Jobs (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md).
    /// </summary>
    public class SqlCompanyRepository(MainDatabase database) : ICompanyRepository
    {
        // os jobs dos próprios hosts de agenda (o 14000): toda holding tem os seus, na empresa que é a própria holding
        private const int HostsApp = 140;

        public async Task<IReadOnlyList<HolderModel>> GetHoldersAsync(CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var rows = await connection.QueryAsync<HolderModel>(new CommandDefinition("""
                SELECT h.Document AS Holder, h.FantasyName, (SELECT COUNT(*) FROM [auditing].[Companies] c WHERE c.Holder = h.Document) AS Companies
                FROM [auditing].[Holders] h
                ORDER BY COALESCE(h.FantasyName, h.Document)
                """, cancellationToken: cancellationToken));

            return rows.ToList();
        }

        public async Task CreateHolderAsync(string holder, string? fantasyName, UserInput firstUser, CancellationToken cancellationToken = default)
        {
            var key = Document(holder, "da holding");
            var name = Blank(fantasyName);

            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await EnsureFreeAsync(connection, transaction, key, cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO [auditing].[Holders] (Document, FantasyName) VALUES (@key, @name);
                INSERT INTO [auditing].[Companies] (Company, Holder, FantasyName) VALUES (@key, @key, @name);
                """, new { key = Ansi(key), name }, transaction, cancellationToken: cancellationToken));

            var jobs = await CatalogJobsAsync(connection, transaction, HostsApp, cancellationToken);
            await InsertJobsAsync(connection, transaction, key, key, HostsApp, jobs, cancellationToken);

            // o primeiro usuário do painel, na empresa que é a própria holding: sem ele, ninguém entraria. Nasce Gestor (ou
            // Administrador, se for o primeiro do banco)
            await SqlUserRepository.InsertAsync(connection, transaction, firstUser, key, profile: null, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }

        public async Task UpdateHolderAsync(string holder, string? fantasyName, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var updated = await connection.ExecuteAsync(new CommandDefinition(
                "UPDATE [auditing].[Holders] SET FantasyName = @name WHERE Document = @holder",
                new { holder = Ansi(ModuleDefinitions.Digits(holder)), name = Blank(fantasyName) }, cancellationToken: cancellationToken));

            if (updated == 0)
                throw new CompanyRegistrationException($"A holding {holder} não está cadastrada.");
        }

        public async Task<IReadOnlyList<CompanyModel>> GetCompaniesAsync(string holder, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(holder);
            await using var connection = await database.OpenAsync(cancellationToken);

            var companies = (await connection.QueryAsync<CompanyModel>(new CommandDefinition("""
                SELECT Company, Holder, FantasyName
                FROM [auditing].[Companies]
                WHERE Holder = @holder
                ORDER BY CASE WHEN Company = Holder THEN 0 ELSE 1 END, COALESCE(FantasyName, Company)
                """, new { holder = Ansi(key) }, cancellationToken: cancellationToken))).ToList();

            // em que módulos cada empresa tem parâmetros: um SELECT por tabela *Parameters que existe no banco. Tabela e
            // IdParent vêm de ModuleDefinitions, nunca da tela
            var existing = await MainDatabase.ExistingTablesAsync(connection);
            var queries = ModuleDefinitions.All
                .Where(x => existing.Contains(x.Table))
                .Select(x => $"SELECT DISTINCT {x.IdParent} AS IdParent, CAST(p.doc_company AS varchar(20)) AS DocCompany FROM {x.Table} p JOIN [auditing].[Companies] c ON c.Company = p.doc_company WHERE c.Holder = @holder")
                .ToList();

            if (queries.Count > 0)
            {
                var modules = await connection.QueryAsync<(int IdParent, string DocCompany)>(new CommandDefinition(
                    string.Join(" UNION ALL ", queries), new { holder = Ansi(key) }, cancellationToken: cancellationToken));
                var byCompany = modules.ToLookup(x => x.DocCompany, x => x.IdParent);

                foreach (var company in companies)
                    company.Modules = byCompany[company.Company].Order().ToList();
            }

            return companies;
        }

        public async Task CreateCompanyAsync(string holder, string company, string? fantasyName, CancellationToken cancellationToken = default)
        {
            var holderKey = ModuleDefinitions.Digits(holder);
            var key = Document(company, "da empresa");

            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                "SELECT COUNT(*) FROM [auditing].[Holders] WHERE Document = @holder", new { holder = Ansi(holderKey) }, transaction, cancellationToken: cancellationToken));
            if (exists == 0)
                throw new CompanyRegistrationException($"A holding {holderKey} não está cadastrada.");

            await EnsureFreeAsync(connection, transaction, key, cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition(
                "INSERT INTO [auditing].[Companies] (Company, Holder, FantasyName) VALUES (@key, @holder, @name)",
                new { key = Ansi(key), holder = Ansi(holderKey), name = Blank(fantasyName) }, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }

        public async Task UpdateCompanyAsync(string company, string? fantasyName, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var updated = await connection.ExecuteAsync(new CommandDefinition(
                "UPDATE [auditing].[Companies] SET FantasyName = @name WHERE Company = @company",
                new { company = Ansi(ModuleDefinitions.Digits(company)), name = Blank(fantasyName) }, cancellationToken: cancellationToken));

            if (updated == 0)
                throw NotFound(company);
        }

        public async Task<IReadOnlyList<CompanyAppModel>> GetAppsAsync(string company, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);

            // os jobs do módulo são os da árvore do catálogo (integrações e as procedures que elas chamam)
            var rows = await connection.QueryAsync<CompanyAppModel>(new CommandDefinition("""
                SELECT
                    a.IdParent,
                    a.Title,
                    a.[Description],
                    COUNT(j.IdJob) AS Jobs,
                    COALESCE(SUM(CASE WHEN j.IsActive = 1 THEN 1 ELSE 0 END), 0) AS ActiveJobs
                FROM
                    [auditing].[Apps] a
                    OUTER APPLY [auditing].[fn_CatalogTree](a.IdParent) t
                    LEFT JOIN [auditing].[Jobs] j ON j.IdJob = t.IdJob AND j.IdType = t.IdType AND j.Company = @company
                GROUP BY
                    a.IdParent, a.Title, a.[Description]
                ORDER BY
                    a.Title
                """, new { company = Ansi(ModuleDefinitions.Digits(company)) }, cancellationToken: cancellationToken));

            return rows.ToList();
        }

        public async Task<IReadOnlyList<CatalogJobModel>> GetCatalogJobsAsync(int idParent, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            return await CatalogJobsAsync(connection, null, idParent, cancellationToken);
        }

        public async Task<int> AddAppAsync(string company, int idParent, IReadOnlyList<CatalogJobModel> jobs, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);

            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var holder = await connection.ExecuteScalarAsync<string?>(new CommandDefinition(
                "SELECT Holder FROM [auditing].[Companies] WHERE Company = @company", new { company = Ansi(key) }, transaction, cancellationToken: cancellationToken))
                ?? throw NotFound(company);

            // a agenda da tela por cima da sugerida; o que não veio da tela fica com a sugestão
            var chosen = jobs.ToDictionary(x => (x.IdJob, x.IdType));
            var schedule = (await CatalogJobsAsync(connection, transaction, idParent, cancellationToken))
                .Select(x => chosen.TryGetValue((x.IdJob, x.IdType), out var job) ? WithCatalog(job, x) : x)
                .ToList();

            var created = await InsertJobsAsync(connection, transaction, holder, key, idParent, schedule, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return created;
        }

        public async Task RemoveAppAsync(string company, int idParent, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE j SET IsActive = 0
                FROM [auditing].[Jobs] j
                JOIN [auditing].[fn_CatalogTree](@idParent) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType
                WHERE j.Company = @company
                """, new { company = Ansi(ModuleDefinitions.Digits(company)), idParent }, cancellationToken: cancellationToken));
        }

        private static async Task<List<CatalogJobModel>> CatalogJobsAsync(SqlConnection connection, IDbTransaction? transaction, int idParent, CancellationToken cancellationToken)
        {
            var rows = await connection.QueryAsync<CatalogJobModel>(new CommandDefinition("""
                SELECT c.IdJob, c.IdType, c.Title, c.[Description], c.IsOnDemand
                FROM [auditing].[fn_CatalogTree](@idParent) t
                JOIN [auditing].[Catalog] c ON c.IdJob = t.IdJob AND c.IdType = t.IdType
                ORDER BY c.IdType DESC, c.IdJob
                """, new { idParent }, transaction, cancellationToken: cancellationToken));

            return rows.Select(JobAgenda.Suggest).ToList();
        }

        // os jobs que faltam para a empresa, inativos, com a agenda de cada um; os que já existem ficam como estão
        private static async Task<int> InsertJobsAsync(SqlConnection connection, IDbTransaction transaction, string holder, string company, int idParent, IReadOnlyList<CatalogJobModel> jobs, CancellationToken cancellationToken)
        {
            foreach (var job in jobs)
            {
                var error = ScheduleError(job);
                if (error is not null)
                    throw new CompanyRegistrationException($"{job.IdJob} {job.Title}: {error}");
            }

            var agenda = JsonSerializer.Serialize(jobs.Select(x => new
            {
                x.IdJob,
                x.IdType,
                x.IntervalTimeInMinutes,
                RunningDailyAt = x.RunningDailyAt?.ToString("HH:mm:ss"),
                x.Queue
            }));

            return await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO [auditing].[Jobs] (IdJob, IdType, Holder, Company, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive)
                SELECT t.IdJob, t.IdType, @holder, @company, a.IntervalTimeInMinutes, a.RunningDailyAt, a.Queue, 0
                FROM [auditing].[fn_CatalogTree](@idParent) t
                JOIN OPENJSON(@agenda) WITH (IdJob int, IdType int, IntervalTimeInMinutes int, RunningDailyAt time(0), Queue nvarchar(50)) a
                    ON a.IdJob = t.IdJob AND a.IdType = t.IdType
                WHERE NOT EXISTS (
                    SELECT 1 FROM [auditing].[Jobs] j
                    WHERE j.IdJob = t.IdJob AND j.IdType = t.IdType AND j.Holder = @holder AND j.Company = @company)
                """, new { holder = Ansi(holder), company = Ansi(company), idParent, agenda }, transaction, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// A agenda de um job antes de gravar: a mesma regra das CHECK de auditing.Jobs e do agendador.
        /// </summary>
        public static string? ScheduleError(CatalogJobModel job)
        {
            if (job.IsOnDemand)
                return job.IntervalTimeInMinutes == 0 && job.RunningDailyAt is null ? null : "job sob demanda não tem agenda.";

            if (job.RunningDailyAt is not null && job.IntervalTimeInMinutes != 1440)
                return "o job diário precisa do intervalo de 1440 minutos.";

            if (job.IntervalTimeInMinutes <= 0)
                return "o intervalo precisa ser maior que zero.";

            if (string.IsNullOrEmpty(job.Queue) || job.Queue.Length > 50 || job.Queue.Any(c => !(char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c) || c is '_' or '-')))
                return "a fila só aceita minúsculas, dígitos, _ e -.";

            return null;
        }

        // título, descrição e sob demanda vêm sempre do catálogo; da tela, só a agenda
        private static CatalogJobModel WithCatalog(CatalogJobModel chosen, CatalogJobModel catalog) => new()
        {
            IdJob = catalog.IdJob,
            IdType = catalog.IdType,
            Title = catalog.Title,
            Description = catalog.Description,
            IsOnDemand = catalog.IsOnDemand,
            IntervalTimeInMinutes = catalog.IsOnDemand ? 0 : chosen.IntervalTimeInMinutes,
            RunningDailyAt = catalog.IsOnDemand ? null : chosen.RunningDailyAt,
            Queue = string.IsNullOrWhiteSpace(chosen.Queue) ? catalog.Queue : chosen.Queue.Trim()
        };

        // um CNPJ é de uma holding só, e a própria holding é uma empresa: CNPJ já em Companies é recusado
        private static async Task EnsureFreeAsync(SqlConnection connection, IDbTransaction transaction, string key, CancellationToken cancellationToken)
        {
            var found = await connection.QueryFirstOrDefaultAsync<CompanyModel>(new CommandDefinition(
                "SELECT Company, Holder, FantasyName FROM [auditing].[Companies] WHERE Company = @key", new { key = Ansi(key) }, transaction, cancellationToken: cancellationToken));

            if (found is not null)
                throw new CompanyRegistrationException(found.IsHolder
                    ? $"A holding e a empresa {found.Label} ({key}) já estão cadastradas."
                    : $"O CNPJ {key} já é uma empresa da holding {found.Holder}.", CompanyRegistrationException.AlreadyRegistered);
        }

        private static string Document(string value, string what)
        {
            var key = ModuleDefinitions.Digits(value);
            if (key.Length is not (11 or 14))
                throw new CompanyRegistrationException($"O CNPJ {what} precisa ter 14 dígitos (ou 11, de CPF).");

            return key;
        }

        private static DbString Ansi(string value) => new() { Value = value, IsAnsi = true, Length = 20 };

        private static CompanyRegistrationException NotFound(string company) =>
            new($"A empresa {company} não está cadastrada.", 50002);

        private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
