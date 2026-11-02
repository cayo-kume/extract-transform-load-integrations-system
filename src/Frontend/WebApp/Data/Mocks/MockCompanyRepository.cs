using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Empresas, CNPJs, contratações e parâmetros em memória, compartilhados pelos dois repositórios falsos do cadastro.
    /// Segredos ficam aqui, mas nunca saem pelos repositórios. Começa com a empresa logada (NEWBLOOMERS) e duas marcas.
    /// </summary>
    public class MockCompanyStore
    {
        public readonly Lock Gate = new();

        public Dictionary<string, CompanyModel> Companies { get; } = [];

        /// <summary>
        /// CNPJ de parâmetros → (holding, nome, criado em).
        /// </summary>
        public Dictionary<string, (string Holding, string? Name, DateTime CreatedAt)> HoldingCompanies { get; } = [];

        /// <summary>
        /// (empresa, módulo) → (jobs, ativos) das empresas que não são a logada (a logada vem dos jobs do mock do monitor).
        /// </summary>
        public Dictionary<(string Company, int IdParent), (int Jobs, int Active)> Apps { get; } = [];

        /// <summary>
        /// (módulo, CNPJ) → colunas e quando foi gravada.
        /// </summary>
        public Dictionary<(int IdParent, string DocCompany), (Dictionary<string, string?> Values, DateTime UpdatedAt)> Parameters { get; } = [];

        public MockCompanyStore()
        {
            var now = DateTime.Now;
            var holding = LoggedCompany.DocCompany;

            Companies[holding] = new CompanyModel { Company = holding, Name = LoggedCompany.Name, TradeName = "NewBloomers", CreatedAt = now.AddDays(-30), UpdatedAt = now.AddDays(-30) };
            Companies["12345678000195"] = new CompanyModel { Company = "12345678000195", Name = "Cliente Exemplo Ltda", TradeName = "Exemplo", IsActive = false, CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-2) };

            // marcas fictícias da holding; a própria holding entra porque tem parâmetros (Totvs)
            HoldingCompanies[holding] = (holding, LoggedCompany.Name, now.AddDays(-30));
            HoldingCompanies["11111111000111"] = (holding, "Marca A", now.AddDays(-30));
            HoldingCompanies["22222222000122"] = (holding, "Marca B", now.AddDays(-30));

            Parameters[(130, holding)] = (new() { ["client_id"] = "newbloomers-api", ["client_secret"] = "mock", ["username"] = "integracao", ["password"] = "mock" }, now.AddDays(-30));
            Parameters[(30, "11111111000111")] = (new() { ["token"] = Guid.NewGuid().ToString() }, now.AddDays(-20));
            Parameters[(30, "22222222000122")] = (new() { ["token"] = Guid.NewGuid().ToString() }, now.AddDays(-20));
            Parameters[(50, "11111111000111")] = (new() { ["token"] = Guid.NewGuid().ToString() }, now.AddDays(-10));
            Parameters[(80, holding)] = (new() { ["grupo"] = "NEWBLOOMERS", ["erp_chave"] = Guid.NewGuid().ToString(), ["erp_user_name"] = "linx_export", ["erp_password"] = "mock" }, now.AddDays(-25));
        }
    }

    /// <summary>
    /// ICompanyRepository em memória (MockCompanyStore). O catálogo de módulos e os jobs da empresa logada vêm do
    /// IMonitorRepository, para a tela de cadastro bater com as outras telas.
    /// </summary>
    public class MockCompanyRepository(MockCompanyStore store, IMonitorRepository monitor) : ICompanyRepository
    {
        public Task<IReadOnlyList<CompanyModel>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
                return Task.FromResult<IReadOnlyList<CompanyModel>>(store.Companies.Values.OrderBy(x => x.Name).Select(Copy).ToList());
        }

        public Task<CompanyModel?> GetCompanyAsync(string company, CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
                return Task.FromResult(store.Companies.TryGetValue(ModuleDefinitions.Digits(company), out var found) ? Copy(found) : null);
        }

        public Task CreateCompanyAsync(CompanyModel company, CancellationToken cancellationToken = default)
        {
            var key = ValidCompany(company.Company);
            var name = Required(company.Name, "A razão social");

            lock (store.Gate)
            {
                // as mesmas regras de auditing.sp_Add_Company (50003) e da chave de monitor.Company
                if (store.HoldingCompanies.TryGetValue(key, out var link) && link.Holding != key)
                    throw new CompanyRegistrationException($"O CNPJ {key} já é da holding {link.Holding}.", 50003);
                if (store.Companies.ContainsKey(key))
                    throw new CompanyRegistrationException($"A empresa {key} já está cadastrada.");

                var now = DateTime.Now;
                store.Companies[key] = new CompanyModel { Company = key, Name = name, TradeName = Blank(company.TradeName), IsActive = true, CreatedAt = now, UpdatedAt = now };
            }

            return Task.CompletedTask;
        }

        public Task UpdateCompanyAsync(string company, string name, string? tradeName, CancellationToken cancellationToken = default)
        {
            var required = Required(name, "A razão social");

            lock (store.Gate)
            {
                var found = Find(company);
                found.Name = required;
                found.TradeName = Blank(tradeName);
                found.UpdatedAt = DateTime.Now;
            }

            return Task.CompletedTask;
        }

        public Task SetCompanyActiveAsync(string company, bool isActive, CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
            {
                var found = Find(company);
                found.IsActive = isActive;
                found.UpdatedAt = DateTime.Now;
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<HoldingCompanyModel>> GetHoldingCompaniesAsync(string company, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);

            lock (store.Gate)
            {
                IReadOnlyList<HoldingCompanyModel> result = store.HoldingCompanies
                    .Where(x => x.Value.Holding == key)
                    .OrderBy(x => x.Key)
                    .Select(x => new HoldingCompanyModel
                    {
                        Company = x.Key,
                        Name = x.Value.Name,
                        CreatedAt = x.Value.CreatedAt,
                        Modules = store.Parameters.Keys.Where(p => p.DocCompany == x.Key).Select(p => p.IdParent).OrderBy(p => p).ToList()
                    })
                    .ToList();

                return Task.FromResult(result);
            }
        }

        public async Task<IReadOnlyList<CompanyAppModel>> GetAppsAsync(string company, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);
            var apps = await monitor.GetAppSummariesAsync(Period.Last24Hours, cancellationToken);
            var jobs = await monitor.GetJobsAsync(idApp: null, idType: null, cancellationToken);

            lock (store.Gate)
            {
                Find(key);

                return apps
                    .OrderBy(x => x.Title)
                    .Select(app =>
                    {
                        // a empresa logada é a dos jobs do mock do monitor; as outras, o que foi contratado aqui
                        var (count, active) = key == LoggedCompany.DocCompany
                            ? (jobs.Count(x => x.IdApp == app.IdParent), jobs.Count(x => x.IdApp == app.IdParent && x.IsActive))
                            : store.Apps.GetValueOrDefault((key, app.IdParent));

                        return new CompanyAppModel { IdParent = app.IdParent, Title = app.Title, Description = app.Description, Jobs = count, ActiveJobs = active };
                    })
                    .ToList();
            }
        }

        public async Task<int> AddAppAsync(string company, int idParent, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);
            var catalog = (await monitor.GetJobsAsync(idApp: idParent, idType: null, cancellationToken)).Count;

            lock (store.Gate)
            {
                Find(key);

                // a empresa logada já tem todos os jobs no mock do monitor
                if (key == LoggedCompany.DocCompany)
                    return 0;

                var (jobs, active) = store.Apps.GetValueOrDefault((key, idParent));
                store.Apps[(key, idParent)] = (catalog, active);
                return catalog - jobs;
            }
        }

        public Task RemoveAppAsync(string company, int idParent, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);

            lock (store.Gate)
            {
                Find(key);

                if (store.Apps.TryGetValue((key, idParent), out var app))
                    store.Apps[(key, idParent)] = (app.Jobs, 0);
            }

            return Task.CompletedTask;
        }

        // dentro do lock
        private CompanyModel Find(string company) =>
            store.Companies.TryGetValue(ModuleDefinitions.Digits(company), out var found)
                ? found
                : throw new CompanyRegistrationException($"A empresa {company} não está cadastrada.", 50002);

        private static string ValidCompany(string? company)
        {
            var key = ModuleDefinitions.Digits(company);
            return key.Length is 11 or 14 ? key : throw new CompanyRegistrationException("O CNPJ precisa ter 14 dígitos (ou 11, de CPF).");
        }

        private static string Required(string? value, string label) =>
            string.IsNullOrWhiteSpace(value) ? throw new CompanyRegistrationException($"{label} é obrigatória.") : value.Trim();

        private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static CompanyModel Copy(CompanyModel x) => new()
        {
            Company = x.Company,
            Name = x.Name,
            TradeName = x.TradeName,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        };
    }

    /// <summary>
    /// IModuleParametersRepository em memória (MockCompanyStore), com as mesmas regras do SqlModuleParametersRepository.
    /// </summary>
    public class MockModuleParametersRepository(MockCompanyStore store) : IModuleParametersRepository
    {
        public Task<IReadOnlyList<ModuleParametersModel>> GetParametersAsync(int idParent, string company, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            var key = ModuleDefinitions.Digits(company);

            lock (store.Gate)
            {
                IReadOnlyList<ModuleParametersModel> result = store.Parameters
                    .Where(x => x.Key.IdParent == idParent && store.HoldingCompanies.TryGetValue(x.Key.DocCompany, out var link) && link.Holding == key)
                    .OrderBy(x => x.Key.DocCompany)
                    .Select(x => ToModel(definition, x.Key.DocCompany, x.Value.Values, x.Value.UpdatedAt))
                    .ToList();

                return Task.FromResult(result);
            }
        }

        public Task SaveParametersAsync(int idParent, string company, ModuleParametersInput input, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            var key = ModuleDefinitions.Digits(company);
            var doc = ModuleDefinitions.Digits(input.DocCompany);

            lock (store.Gate)
            {
                if (!store.Companies.ContainsKey(key))
                    throw new CompanyRegistrationException($"A empresa {key} não está cadastrada.", 50002);

                // as regras de auditing.sp_Add_Holding_Company
                if (store.HoldingCompanies.TryGetValue(doc, out var link) && link.Holding != key)
                    throw new CompanyRegistrationException($"O CNPJ {doc} já é da holding {link.Holding}.", 50003);
                if (doc != key && store.Companies.ContainsKey(doc))
                    throw new CompanyRegistrationException($"O CNPJ {doc} é de outra empresa do painel.", 50004);

                var current = store.Parameters.TryGetValue((idParent, doc), out var row) ? row.Values : [];
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

                if (!store.HoldingCompanies.ContainsKey(doc))
                    store.HoldingCompanies[doc] = (key, string.IsNullOrWhiteSpace(input.DocCompanyName) ? null : input.DocCompanyName.Trim(), DateTime.Now);

                store.Parameters[(idParent, doc)] = (values, DateTime.Now);
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
