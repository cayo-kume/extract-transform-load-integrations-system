namespace WebApp.Data
{
    /// <summary>
    /// Quem pode usar a tela Executar Querys. Até a autenticação (fase 6 do plano), a tela só abre com a chave
    /// QueryRunner:Enabled ligada e o ambiente Development; depois, isto dá lugar a um grupo do Entra ID.
    /// </summary>
    public class QueryRunnerAccess(IConfiguration configuration, IWebHostEnvironment environment)
    {
        public bool IsAllowed =>
            environment.IsDevelopment() && configuration.GetValue<bool>("QueryRunner:Enabled");
    }
}
