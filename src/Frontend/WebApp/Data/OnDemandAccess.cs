namespace WebApp.Data
{
    /// <summary>
    /// Quem pode usar a tela Sob Demanda: as operações escrevem nos sistemas de origem (Movidesk, Totvs, AfterSale). Até a
    /// autenticação (fase 6 do plano), a tela só abre com a chave OnDemand:Enabled ligada e o ambiente Development, como
    /// Executar Querys; depois, isto dá lugar a um grupo do Entra ID.
    /// </summary>
    public class OnDemandAccess(IConfiguration configuration, IWebHostEnvironment environment)
    {
        public bool IsAllowed =>
            environment.IsDevelopment() && configuration.GetValue<bool>("OnDemand:Enabled");
    }
}
