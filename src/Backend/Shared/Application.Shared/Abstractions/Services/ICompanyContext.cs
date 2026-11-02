namespace Application.Shared.Abstractions.Services
{
    /// <summary>
    /// empresa do painel (CNPJ da holding, monitor.Company) da execução em curso. Quem inicia a execução (o agendador ou a
    /// rota) informa a empresa uma vez no escopo; o LoggerService grava o log com ela e as procedures de sincronização a
    /// recebem em @Company (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.3)
    /// </summary>
    public interface ICompanyContext
    {
        /// <summary>
        /// nula enquanto ninguém informou a empresa
        /// </summary>
        public string? Company { get; }

        /// <summary>
        /// informa a empresa da execução; só dígitos. Trocar de empresa no meio do escopo não é permitido
        /// </summary>
        /// <param name="company"></param>
        public void SetCompany(string company);
    }
}
