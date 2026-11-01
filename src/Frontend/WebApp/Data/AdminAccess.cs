namespace WebApp.Data
{
    /// <summary>
    /// Quem pode usar as telas de administrador: Executar Querys (escreve no banco) e Sob Demanda (escreve nos sistemas de
    /// origem). Até a autenticação (fase 6 do plano), as duas só abrem com a chave Admin:Enabled ligada e o ambiente
    /// Development; depois, isto dá lugar ao grupo de administradores do Entra ID.
    /// </summary>
    public class AdminAccess(IConfiguration configuration, IWebHostEnvironment environment)
    {
        public bool IsAllowed =>
            environment.IsDevelopment() && configuration.GetValue<bool>("Admin:Enabled");
    }
}
