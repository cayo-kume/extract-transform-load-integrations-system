namespace WebApp.Models
{
    /// <summary>
    /// Empresa do login, mostrada no topo. A visão é só dela: nenhuma tela mostra dados de outra empresa.
    /// Até a fase 6 do plano (docs/PLANO-WEBAPP.md) o valor é fixo aqui; depois, vem da sessão autenticada.
    /// </summary>
    public static class LoggedCompany
    {
        public const string Name = "NEWBLOOMERS";
        public const string DocCompany = "35899805838";

        public static string Label => $"{Name} - {DocCompany}";
    }
}
