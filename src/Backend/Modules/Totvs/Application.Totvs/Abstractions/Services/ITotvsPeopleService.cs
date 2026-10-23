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
    }
}
