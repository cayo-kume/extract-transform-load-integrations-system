namespace WebApp.Models
{
    /// <summary>
    /// Empresa do login, mostrada no topo. A visão é só dela: nenhuma tela mostra dados de outra empresa.
    /// Até a fase 6 do plano (docs/PLANO-WEBAPP.md) vem da configuração (LoggedCompany:DocCompany e LoggedCompany:Name,
    /// lidas uma vez no Program.cs); depois, da sessão autenticada. O CNPJ é o Company de auditing.Jobs e monitor.Company.
    /// </summary>
    public static class LoggedCompany
    {
        public static string Name { get; private set; } = "NEWBLOOMERS";

        public static string DocCompany { get; private set; } = "44065634000106";

        public static string Label => $"{Name} - {DocCompany}";

        public static void Configure(IConfiguration configuration)
        {
            Name = configuration["LoggedCompany:Name"] is { Length: > 0 } name ? name : Name;
            DocCompany = configuration["LoggedCompany:DocCompany"] is { Length: > 0 } doc ? doc : DocCompany;
        }
    }
}
