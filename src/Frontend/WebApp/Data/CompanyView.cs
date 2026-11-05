using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Um módulo do catálogo que alguma empresa do escopo ainda não contratou, com as empresas que faltam.
    /// </summary>
    public sealed record AvailableApp(AppSummaryModel App, IReadOnlyList<CompanyModel> Missing);

    /// <summary>
    /// O painel das empresas que a tela mostra: a escolhida no seletor (LoggedCompany.ScopeCompanies) ou, na Visão Geral
    /// (Summed), a holding com todas as empresas dela (LoggedCompany.SummedCompanies). O repositório lê uma empresa por vez
    /// (IMonitorRepository.For), e aqui as leituras são juntadas: as listas com o CNPJ de cada linha (Company) e os resumos
    /// das aplicações somados.
    /// </summary>
    public class CompanyView(IMonitorRepository monitor, ICompanyRepository companies, LoggedCompany loggedCompany)
    {
        private bool _summed;

        /// <summary>
        /// A mesma visão somando as empresas da holding, quando a holding está escolhida (a Visão Geral).
        /// </summary>
        public CompanyView Summed => new(monitor, companies, loggedCompany) { _summed = true };

        private IReadOnlyList<CompanyModel> Scope => _summed ? loggedCompany.SummedCompanies : loggedCompany.ScopeCompanies;

        /// <summary>
        /// O repositório de uma empresa da linha (a ação grava nela); empresa que o usuário não vê é recusada.
        /// </summary>
        public IMonitorRepository For(string company)
        {
            if (!string.IsNullOrEmpty(company) && !loggedCompany.CanSee(company))
                throw new InvalidOperationException($"A empresa {company} não é visível para o usuário logado.");

            return monitor.For(company);
        }

        /// <summary>
        /// Os módulos do catálogo: os contratados por alguma empresa do escopo primeiro, com os números somados, e depois os
        /// que nenhuma contratou (IsContracted falso). Para quem vê a holding inteira, cada módulo traz as empresas que o
        /// contrataram (AppSummaryModel.Companies), mesmo fora do escopo, para os chips do card.
        /// </summary>
        public async Task<List<AppSummaryModel>> SummariesAsync(Period period, CancellationToken cancellationToken = default)
        {
            var scope = Scope.Select(x => x.Company).ToHashSet();
            var chipsFrom = loggedCompany.SeesWholeHolding ? loggedCompany.VisibleCompanies : Scope;

            var merged = new List<(CompanyModel Company, List<AppSummaryModel> Apps, IReadOnlyList<CompanyAppModel> Contracts)>();
            foreach (var company in chipsFrom.Concat(Scope).DistinctBy(x => x.Company))
            {
                var apps = await companies.GetAppsAsync(company.Company, cancellationToken);
                IReadOnlyList<AppSummaryModel> summaries = scope.Contains(company.Company)
                    ? await monitor.For(company.Company).GetAppSummariesAsync(period, cancellationToken)
                    : [];
                merged.Add((company, CompanyApps.Merge(summaries, apps, company.Company, loggedCompany.Holder), apps));
            }

            // Hosts e DataWarehouse são da holding: só aparecem com ela no escopo, mesmo que os chips venham dela
            var catalog = merged.SelectMany(x => x.Apps).GroupBy(x => x.IdParent)
                .Where(x => !CompanyApps.IsHoldingApp(x.Key) || scope.Contains(loggedCompany.Holder));
            var result = new List<AppSummaryModel>();
            foreach (var app in catalog)
            {
                var inScope = merged.Where(x => scope.Contains(x.Company.Company)).SelectMany(x => x.Apps.Where(a => a.IdParent == app.Key && a.IsContracted)).ToList();
                var first = app.First();

                result.Add(new AppSummaryModel
                {
                    IdParent = app.Key,
                    Title = first.Title,
                    Description = first.Description,
                    ImageUrl = app.Select(x => x.ImageUrl).FirstOrDefault(x => x is not null),
                    Integrations = inScope.Sum(x => x.Integrations),
                    Procedures = inScope.Sum(x => x.Procedures),
                    ActiveJobs = inScope.Sum(x => x.ActiveJobs),
                    Executions = inScope.Sum(x => x.Executions),
                    FailedExecutions = inScope.Sum(x => x.FailedExecutions),
                    Records = inScope.Sum(x => x.Records),
                    Warnings = inScope.Sum(x => x.Warnings),
                    Errors = inScope.Sum(x => x.Errors),
                    LastErrorAt = inScope.Max(x => x.LastErrorAt),
                    FailingJobs = inScope.Sum(x => x.FailingJobs),
                    IsContracted = inScope.Count > 0,
                    // removida da Visão Geral pela empresa escolhida (na da holding, pela holding)
                    IsHidden = merged.FirstOrDefault(x => x.Company.Company == loggedCompany.DocCompany).Contracts?.FirstOrDefault(a => a.IdParent == app.Key)?.IsHidden == true,
                    // ativo pela contratação (jobs ligados agora), que existe também para as empresas fora do escopo
                    Companies = merged
                        .Where(x => x.Apps.Any(a => a.IdParent == app.Key && a.IsContracted))
                        .Select(x => new AppCompanyModel
                        {
                            Company = x.Company.Company,
                            Label = loggedCompany.CompanyLabel(x.Company.Company),
                            IsActive = x.Contracts.FirstOrDefault(a => a.IdParent == app.Key)?.ActiveJobs > 0,
                            IsHolder = x.Company.IsHolder
                        })
                        .ToList()
                });
            }

            return [.. result.Where(x => x.IsContracted).OrderBy(x => x.Title), .. result.Where(x => !x.IsContracted).OrderBy(x => x.Title)];
        }

        /// <summary>
        /// Os módulos que alguma empresa do escopo ainda não contratou, com as empresas que faltam, para o Adicionar aplicação.
        /// </summary>
        public async Task<List<AvailableApp>> AvailableAsync(CancellationToken cancellationToken = default)
        {
            var missing = new Dictionary<int, (AppSummaryModel App, List<CompanyModel> Companies)>();
            foreach (var company in Scope)
            {
                var apps = CompanyApps.Merge([], await companies.GetAppsAsync(company.Company, cancellationToken), company.Company, loggedCompany.Holder);
                foreach (var app in apps.Where(x => !x.IsContracted))
                {
                    if (!missing.TryGetValue(app.IdParent, out var entry))
                        missing[app.IdParent] = entry = (app, []);
                    entry.Companies.Add(company);
                }
            }

            return missing.Values.OrderBy(x => x.App.Title).Select(x => new AvailableApp(x.App, x.Companies)).ToList();
        }

        public async Task<List<JobModel>> JobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default)
        {
            var jobs = new List<JobModel>();
            foreach (var company in Scope)
                jobs.AddRange((await monitor.For(company.Company).GetJobsAsync(idApp, idType, cancellationToken)).Where(x => Shows(x.IdApp, company)));
            return jobs;
        }

        public async Task<List<HeartbeatModel>> HeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default)
        {
            var rows = new List<HeartbeatModel>();
            foreach (var company in Scope)
                rows.AddRange((await monitor.For(company.Company).GetHeartbeatAsync(period, top, cancellationToken)).Where(x => Shows(x.IdApp, company)));
            return rows;
        }

        /// <summary>
        /// As faixas de execução de cada aplicação, somadas entre as empresas do escopo (as faixas são as mesmas em todas).
        /// </summary>
        public async Task<IReadOnlyList<AppBucketsModel>> BucketsAsync(Period period, int buckets, bool previousWindow, CancellationToken cancellationToken = default)
        {
            var all = new List<AppBucketsModel>();
            foreach (var company in Scope)
                all.AddRange((await monitor.For(company.Company).GetExecutionBucketsAsync(period, buckets, previousWindow, cancellationToken)).Where(x => Shows(x.IdApp, company)));

            return all
                .GroupBy(x => x.IdApp)
                .Select(group => new AppBucketsModel
                {
                    IdApp = group.Key,
                    Buckets = group.First().Buckets
                        .Select((bucket, index) => new ExecutionBucketModel
                        {
                            Start = bucket.Start,
                            End = bucket.End,
                            Executions = group.Sum(x => x.Buckets.ElementAtOrDefault(index)?.Executions ?? 0),
                            FailedExecutions = group.Sum(x => x.Buckets.ElementAtOrDefault(index)?.FailedExecutions ?? 0),
                            Errors = group.Sum(x => x.Buckets.ElementAtOrDefault(index)?.Errors ?? 0),
                            Records = group.Sum(x => x.Buckets.ElementAtOrDefault(index)?.Records ?? 0)
                        })
                        .ToList()
                })
                .ToList();
        }

        public async Task<IReadOnlyList<string>> QueuesAsync(CancellationToken cancellationToken = default)
        {
            var queues = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var company in Scope)
                queues.UnionWith(await monitor.For(company.Company).GetQueuesAsync(cancellationToken));
            return queues.ToList();
        }

        public async Task<List<MessageJobSummaryModel>> MessageJobsAsync(MessageQuery query, CancellationToken cancellationToken = default)
        {
            var jobs = new List<MessageJobSummaryModel>();
            foreach (var company in Scope)
                jobs.AddRange((await monitor.For(company.Company).GetMessageJobsAsync(query, cancellationToken)).Where(x => Shows(x.IdApp, company)));
            return jobs;
        }

        /// <summary>
        /// Aplicação da holding ligada na holding: desliga os jobs dela que estiverem ligados nas filiais (contratos de antes
        /// da regra), para a mesma carga não rodar duas vezes. Devolve quantas filiais tinham jobs ligados.
        /// </summary>
        public async Task<int> DeactivateInBranchesAsync(int idApp, CancellationToken cancellationToken = default)
        {
            var changed = 0;
            foreach (var company in loggedCompany.VisibleCompanies.Where(x => x.Company != loggedCompany.Holder))
            {
                var app = (await companies.GetAppsAsync(company.Company, cancellationToken)).FirstOrDefault(x => x.IdParent == idApp);
                if (app is not { ActiveJobs: > 0 })
                    continue;

                await monitor.For(company.Company).SetModuleActiveAsync(idApp, false);
                changed++;
            }

            return changed;
        }

        // os jobs das aplicações da holding (Hosts, DataWarehouse) que alguma filial ainda tenha não aparecem
        private bool Shows(int idApp, CompanyModel company) => CompanyApps.Shows(idApp, company.Company, loggedCompany.Holder);
    }
}
