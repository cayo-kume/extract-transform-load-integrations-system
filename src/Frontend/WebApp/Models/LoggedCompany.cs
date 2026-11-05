namespace WebApp.Models
{
    /// <summary>
    /// Holding e empresa do painel, mostradas no topo (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md). A visão é só da
    /// empresa escolhida: nenhuma tela mostra dados de outra. Uma por aba (circuito): o seletor do topo troca a empresa e
    /// guarda a escolha no navegador; o MainLayout lê a escolha antes de mostrar a primeira tela. Até a fase 6 do plano
    /// (docs/PLANO-WEBAPP.md), a primeira escolha vem da configuração (LoggedCompany:DocCompany e LoggedCompany:Name).
    /// O CNPJ da empresa é o Company de auditing.Jobs, e o da holding, o Holder.
    /// </summary>
    public class LoggedCompany(IConfiguration configuration)
    {
        private const string DefaultDocument = "44065634000106";
        private const string DefaultName = "NEWBLOOMERS";

        /// <summary>
        /// CNPJ da holding (auditing.Holders).
        /// </summary>
        public string Holder { get; private set; } = Configured(configuration, "LoggedCompany:DocCompany", DefaultDocument);

        public string HolderName { get; private set; } = Configured(configuration, "LoggedCompany:Name", DefaultName);

        /// <summary>
        /// CNPJ da empresa (auditing.Companies): o doc_company dos parâmetros e o Company dos jobs.
        /// </summary>
        public string DocCompany { get; private set; } = Configured(configuration, "LoggedCompany:DocCompany", DefaultDocument);

        /// <summary>
        /// Nome fantasia da empresa.
        /// </summary>
        public string Name { get; private set; } = Configured(configuration, "LoggedCompany:Name", DefaultName);

        public string Label => $"{Name} - {DocCompany}";

        /// <summary>
        /// Troca a holding e a empresa da aba. Nome vazio vira o CNPJ.
        /// </summary>
        public void Set(string holder, string? holderName, string company, string? companyName)
        {
            Holder = holder;
            HolderName = string.IsNullOrWhiteSpace(holderName) ? holder : holderName;
            DocCompany = company;
            Name = string.IsNullOrWhiteSpace(companyName) ? company : companyName;
        }

        private static string Configured(IConfiguration configuration, string key, string fallback) =>
            configuration[key] is { Length: > 0 } value ? value : fallback;
    }
}
