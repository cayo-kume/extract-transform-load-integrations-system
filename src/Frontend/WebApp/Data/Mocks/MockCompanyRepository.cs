using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Holdings, empresas, contratações e parâmetros em memória, compartilhados pelos dois repositórios falsos do cadastro.
    /// Segredos e hashes de senha ficam aqui, mas nunca saem pelos repositórios. Começa com a holding da configuração
    /// (NEWBLOOMERS), duas marcas, a holding fictícia Abacartes, de uma empresa só, e o usuário admin (senha admin1234) na
    /// holding da configuração, para entrar no painel com o mock.
    /// </summary>
    public class MockCompanyStore
    {
        public readonly Lock Gate = new();

        /// <summary>
        /// CNPJ da holding → nome fantasia.
        /// </summary>
        public Dictionary<string, string?> Holders { get; } = [];

        /// <summary>
        /// CNPJ da empresa → empresa (com a holding).
        /// </summary>
        public Dictionary<string, CompanyModel> Companies { get; } = [];

        /// <summary>
        /// (empresa, job, tipo) → ativo, dos jobs contratados aqui (a empresa da configuração vem dos jobs do mock do monitor).
        /// </summary>
        public Dictionary<(string Company, int IdJob, int IdType), bool> Jobs { get; } = [];

        /// <summary>
        /// (módulo, empresa) → colunas e quando foi gravada.
        /// </summary>
        public Dictionary<(int IdParent, string Company), (Dictionary<string, string?> Values, DateTime UpdatedAt)> Parameters { get; } = [];

        /// <summary>
        /// Usuários do painel (monitor.Users), com o hash da senha.
        /// </summary>
        public List<(UserModel User, string PasswordHash)> Users { get; } = [];

        /// <summary>
        /// A empresa cujos jobs são os do mock do monitor.
        /// </summary>
        public string MonitorCompany { get; }

        public MockCompanyStore(IConfiguration configuration)
        {
            var now = DateTime.Now;
            var holder = configuration["LoggedCompany:DocCompany"] is { Length: > 0 } doc ? doc : "44065634000106";
            var name = configuration["LoggedCompany:Name"] is { Length: > 0 } configured ? configured : "NEWBLOOMERS";
            MonitorCompany = holder;

            Holders[holder] = name;
            Add(holder, holder, name);
            Add("11111111000111", holder, "Marca A");
            Add("22222222000122", holder, "Marca B");

            Holders["33333333000133"] = "Abacartes";
            Add("33333333000133", "33333333000133", "Abacartes");

            Users.Add((new UserModel { IdUser = 1, Company = holder, Name = "Administrador", UserName = "admin", Email = "admin@exemplo.com.br", CreatedAt = now.AddDays(-30) },
                UserAccounts.Hash("admin1234")));

            Parameters[(130, holder)] = (new() { ["client_id"] = "newbloomers-api", ["client_secret"] = "mock", ["username"] = "integracao", ["password"] = "mock" }, now.AddDays(-30));
            Parameters[(80, holder)] = (new() { ["grupo"] = "NEWBLOOMERS", ["erp_chave"] = Guid.NewGuid().ToString(), ["erp_user_name"] = "linx_export", ["erp_password"] = "mock" }, now.AddDays(-25));
            Parameters[(30, "11111111000111")] = (new() { ["token"] = Guid.NewGuid().ToString() }, now.AddDays(-20));
            Parameters[(50, "11111111000111")] = (new() { ["token"] = Guid.NewGuid().ToString() }, now.AddDays(-10));
            Parameters[(30, "22222222000122")] = (new() { ["token"] = Guid.NewGuid().ToString() }, now.AddDays(-20));

            void Add(string company, string holding, string? fantasyName) =>
                Companies[company] = new CompanyModel { Company = company, Holder = holding, FantasyName = fantasyName };
        }
    }

    /// <summary>
    /// ICompanyRepository em memória (MockCompanyStore), com as mesmas regras do SqlCompanyRepository. O catálogo vem do
    /// MockCatalog e a agenda sugerida, do JobAgenda (o .csv embutido), como no banco.
    /// </summary>
    public class MockCompanyRepository(MockCompanyStore store, IMonitorRepository monitor) : ICompanyRepository
    {
        private const int HostsApp = 140;

        public Task<IReadOnlyList<HolderModel>> GetHoldersAsync(CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
            {
                IReadOnlyList<HolderModel> holders = store.Holders
                    .Select(x => new HolderModel { Holder = x.Key, FantasyName = x.Value, Companies = store.Companies.Values.Count(c => c.Holder == x.Key) })
                    .OrderBy(x => x.Label)
                    .ToList();

                return Task.FromResult(holders);
            }
        }

        public Task CreateHolderAsync(string holder, string? fantasyName, UserInput firstUser, CancellationToken cancellationToken = default)
        {
            var key = Document(holder, "da holding");

            lock (store.Gate)
            {
                EnsureFree(key);
                var user = MockUserRepository.NewUser(store, firstUser, key);

                store.Holders[key] = Blank(fantasyName);
                store.Companies[key] = new CompanyModel { Company = key, Holder = key, FantasyName = Blank(fantasyName) };
                AddJobs(key, HostsApp);
                store.Users.Add(user);
            }

            return Task.CompletedTask;
        }

        public Task UpdateHolderAsync(string holder, string? fantasyName, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(holder);

            lock (store.Gate)
            {
                if (!store.Holders.ContainsKey(key))
                    throw new CompanyRegistrationException($"A holding {holder} não está cadastrada.");

                store.Holders[key] = Blank(fantasyName);
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<CompanyModel>> GetCompaniesAsync(string holder, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(holder);

            lock (store.Gate)
            {
                IReadOnlyList<CompanyModel> companies = store.Companies.Values
                    .Where(x => x.Holder == key)
                    .OrderBy(x => x.IsHolder ? 0 : 1)
                    .ThenBy(x => x.Label)
                    .Select(x => new CompanyModel
                    {
                        Company = x.Company,
                        Holder = x.Holder,
                        FantasyName = x.FantasyName,
                        Modules = store.Parameters.Keys.Where(p => p.Company == x.Company).Select(p => p.IdParent).Order().ToList()
                    })
                    .ToList();

                return Task.FromResult(companies);
            }
        }

        public Task CreateCompanyAsync(string holder, string company, string? fantasyName, CancellationToken cancellationToken = default)
        {
            var holderKey = ModuleDefinitions.Digits(holder);
            var key = Document(company, "da empresa");

            lock (store.Gate)
            {
                if (!store.Holders.ContainsKey(holderKey))
                    throw new CompanyRegistrationException($"A holding {holderKey} não está cadastrada.");

                EnsureFree(key);
                store.Companies[key] = new CompanyModel { Company = key, Holder = holderKey, FantasyName = Blank(fantasyName) };
            }

            return Task.CompletedTask;
        }

        public Task UpdateCompanyAsync(string company, string? fantasyName, CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
                Find(company).FantasyName = Blank(fantasyName);

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<CompanyAppModel>> GetAppsAsync(string company, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);
            var monitorJobs = key == store.MonitorCompany ? await monitor.GetJobsAsync(idApp: null, idType: null, cancellationToken) : [];

            lock (store.Gate)
            {
                Find(key);

                return MockCatalog.Apps
                    .OrderBy(x => x.Title)
                    .Select(app =>
                    {
                        var tree = Tree(app.IdParent);

                        // a empresa da configuração é a dos jobs do mock do monitor; as outras, o que foi contratado aqui
                        var (count, active) = key == store.MonitorCompany
                            ? (monitorJobs.Count(x => x.IdApp == app.IdParent), monitorJobs.Count(x => x.IdApp == app.IdParent && x.IsActive))
                            : (tree.Count(x => store.Jobs.ContainsKey((key, x.IdJob, x.IdType))), tree.Count(x => store.Jobs.GetValueOrDefault((key, x.IdJob, x.IdType))));

                        return new CompanyAppModel { IdParent = app.IdParent, Title = app.Title, Description = app.Description, Jobs = count, ActiveJobs = active };
                    })
                    .ToList();
            }
        }

        public Task<IReadOnlyList<CatalogJobModel>> GetCatalogJobsAsync(int idParent, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<CatalogJobModel>>(CatalogJobs(idParent));

        public Task<int> AddAppAsync(string company, int idParent, IReadOnlyList<CatalogJobModel> jobs, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);

            foreach (var job in jobs)
            {
                var error = Sql.SqlCompanyRepository.ScheduleError(job);
                if (error is not null)
                    throw new CompanyRegistrationException($"{job.IdJob} {job.Title}: {error}");
            }

            lock (store.Gate)
            {
                Find(key);

                // a empresa da configuração já tem todos os jobs no mock do monitor
                return Task.FromResult(key == store.MonitorCompany ? 0 : AddJobs(key, idParent));
            }
        }

        public Task RemoveAppAsync(string company, int idParent, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);

            lock (store.Gate)
            {
                Find(key);

                foreach (var job in Tree(idParent).Where(x => store.Jobs.ContainsKey((key, x.IdJob, x.IdType))))
                    store.Jobs[(key, job.IdJob, job.IdType)] = false;
            }

            return Task.CompletedTask;
        }

        // dentro do lock
        private int AddJobs(string company, int idParent)
        {
            var created = 0;
            foreach (var job in Tree(idParent))
            {
                if (store.Jobs.TryAdd((company, job.IdJob, job.IdType), false))
                    created++;
            }

            return created;
        }

        // os jobs do módulo e, abaixo deles, as procedures que eles chamam (auditing.fn_CatalogTree)
        private static List<CatalogJob> Tree(int idParent)
        {
            var tree = MockCatalog.Jobs.Where(x => x.IdParent == idParent).ToList();
            var parents = tree.Select(x => x.IdJob).ToHashSet();
            tree.AddRange(MockCatalog.Jobs.Where(x => x.IdType == 1 && parents.Contains(x.IdParent) && !tree.Contains(x)));
            return tree;
        }

        private static List<CatalogJobModel> CatalogJobs(int idParent) => Tree(idParent)
            .OrderByDescending(x => x.IdType)
            .ThenBy(x => x.IdJob)
            .Select(x => JobAgenda.Suggest(new CatalogJobModel
            {
                IdJob = x.IdJob,
                IdType = x.IdType,
                Title = x.Title,
                Description = x.Description,
                IsOnDemand = x.IdType == 2 && x.IntervalTimeInMinutes == 0 && x.RunningDailyAt is null
            }))
            .ToList();

        // dentro do lock: um CNPJ é de uma holding só, e a própria holding é uma empresa
        private void EnsureFree(string key)
        {
            if (store.Companies.TryGetValue(key, out var found))
                throw new CompanyRegistrationException(found.IsHolder
                    ? $"A holding e a empresa {found.Label} ({key}) já estão cadastradas."
                    : $"O CNPJ {key} já é uma empresa da holding {found.Holder}.", CompanyRegistrationException.AlreadyRegistered);
        }

        // dentro do lock
        private CompanyModel Find(string company) =>
            store.Companies.TryGetValue(ModuleDefinitions.Digits(company), out var found)
                ? found
                : throw new CompanyRegistrationException($"A empresa {company} não está cadastrada.", 50002);

        private static string Document(string? value, string what)
        {
            var key = ModuleDefinitions.Digits(value);
            return key.Length is 11 or 14 ? key : throw new CompanyRegistrationException($"O CNPJ {what} precisa ter 14 dígitos (ou 11, de CPF).");
        }

        private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    /// <summary>
    /// IModuleParametersRepository em memória (MockCompanyStore), com as mesmas regras do SqlModuleParametersRepository.
    /// </summary>
    public class MockModuleParametersRepository(MockCompanyStore store) : IModuleParametersRepository
    {
        public Task<ModuleParametersModel?> GetParametersAsync(int idParent, string company, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            var key = ModuleDefinitions.Digits(company);

            lock (store.Gate)
            {
                return Task.FromResult(store.Parameters.TryGetValue((idParent, key), out var row)
                    ? ToModel(definition, key, row.Values, row.UpdatedAt)
                    : null);
            }
        }

        public Task SaveParametersAsync(int idParent, string company, ModuleParametersInput input, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            var key = ModuleDefinitions.Digits(company);

            lock (store.Gate)
            {
                if (!store.Companies.ContainsKey(key))
                    throw new CompanyRegistrationException($"A empresa {key} não está cadastrada.", 50002);

                var current = store.Parameters.TryGetValue((idParent, key), out var row) ? row.Values : [];
                var error = ModuleDefinitions.Validate(definition, input, SecretsSet(definition, current));
                if (error is not null)
                    throw new CompanyRegistrationException(error);

                var values = new Dictionary<string, string?>(current);
                foreach (var field in definition.Fields)
                {
                    var value = input.Values.GetValueOrDefault(field.Column)?.Trim();

                    // segredo vazio mantém o gravado; o resto grava o que veio (vazio vira nulo)
                    if (field.Secret && string.IsNullOrEmpty(value))
                        continue;

                    values[field.Column] = string.IsNullOrEmpty(value) ? null : value;
                }

                store.Parameters[(idParent, key)] = (values, DateTime.Now);
            }

            return Task.CompletedTask;
        }

        private static IReadOnlySet<string> SecretsSet(ModuleDefinition definition, IReadOnlyDictionary<string, string?> values) =>
            definition.Fields.Where(x => x.Secret && !string.IsNullOrEmpty(values.GetValueOrDefault(x.Column))).Select(x => x.Column).ToHashSet();

        private static ModuleParametersModel ToModel(ModuleDefinition definition, string doc, IReadOnlyDictionary<string, string?> values, DateTime updatedAt) => new()
        {
            IdParent = definition.IdParent,
            DocCompany = doc,
            Values = definition.Fields.Where(x => !x.Secret).ToDictionary(x => x.Column, x => values.GetValueOrDefault(x.Column)),
            SecretsSet = SecretsSet(definition, values),
            UpdatedAt = updatedAt
        };
    }
}
