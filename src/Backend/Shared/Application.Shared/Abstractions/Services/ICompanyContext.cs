namespace Application.Shared.Abstractions.Services
{
    /// <summary>
    /// empresa (auditing.Companies) e holding (auditing.Holders) da execução em curso. Quem inicia a execução (o agendador ou a
    /// rota) informa as duas uma vez no escopo; o LoggerService grava o log com elas, os módulos leem os parâmetros da empresa
    /// (doc_company) e as procedures de sincronização recebem a empresa em @Company
    /// (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)
    /// </summary>
    public interface ICompanyContext
    {
        /// <summary>
        /// holding da empresa; nula enquanto ninguém informou a empresa
        /// </summary>
        public string? Holder { get; }

        /// <summary>
        /// empresa da execução (CNPJ, o doc_company dos parâmetros); nula enquanto ninguém informou
        /// </summary>
        public string? Company { get; }

        /// <summary>
        /// informa a holding e a empresa da execução; só dígitos. Trocar de empresa no meio do escopo não é permitido
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="company"></param>
        public void SetCompany(string holder, string company);
    }
}
