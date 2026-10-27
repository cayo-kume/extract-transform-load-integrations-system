namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// representantes e recepções de venda da API Person (fase PE-2, docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1)
    /// </summary>
    public interface ITotvsRepresentativesService
    {
        /// <summary>
        /// sincroniza os representantes pelo POST person/v2/representatives/search, com clientes, tabelas de preço, regiões de venda
        /// e parâmetros; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchRepresentatives();

        /// <summary>
        /// busca um representante por código (representativeCodeList) e grava sempre, sem consultar o cache; a chamada é registrada em
        /// totvs.TotvsRequests. true gravado, false não existe na Totvs, null empresa não cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetRepresentativeByCode(string user_name, string doc_company, int code);

        /// <summary>
        /// sincroniza as recepções de venda pelo GET person/v2/reception-sale/search (janela pela data do movimento, hash do JSON);
        /// só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> GetSearchReceptionSale();
    }
}
