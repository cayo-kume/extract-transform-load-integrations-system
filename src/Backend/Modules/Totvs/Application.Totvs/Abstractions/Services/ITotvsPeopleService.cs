namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// pessoas da Totvs (API Person): filiais, pessoas físicas e jurídicas
    /// </summary>
    public interface ITotvsPeopleService
    {
        /// <summary>
        /// sincroniza as empresas (lojas) da holding pelo GET person/v2/branchesList, com endereços; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> GetBranchesList();

        /// <summary>
        /// sincroniza as pessoas físicas pelo POST person/v2/individuals/search, com as estatísticas de todas as lojas; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchCustomers();

        /// <summary>
        /// sincroniza as pessoas jurídicas pelo POST person/v2/legal-entities/search, com as estatísticas de todas as lojas; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchLegalEntity();

        /// <summary>
        /// busca uma pessoa física por código (POST person/v2/individuals/search com personCodeList) e grava sempre, sem consultar
        /// o cache; a chamada é registrada em totvs.TotvsRequests. true gravada, false não existe na Totvs, null empresa não
        /// cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetCustomerByCode(string user_name, string doc_company, int code);

        /// <summary>
        /// busca uma pessoa jurídica por código (POST person/v2/legal-entities/search com personCodeList); mesmo contrato do GetCustomerByCode
        /// </summary>
        public Task<bool?> GetLegalEntityByCode(string user_name, string doc_company, int code);
    }
}
