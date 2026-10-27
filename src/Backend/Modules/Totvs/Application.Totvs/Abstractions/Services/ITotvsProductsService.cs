namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsProductsService
    {
        /// <summary>
        /// sincroniza os saldos de todos os tipos, por loja de cada marca, pelo POST product/v2/balances/search (carga inicial por produto,
        /// depois janela de alteração), com os pendentes e os locais; só grava os produtos novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchBalances();

        /// <summary>
        /// sincroniza as referências e produtos pelo POST product/v2/references/search, uma chamada por marca (matrizes 1 e 101
        /// como empresa base, filtradas pela classificação 6000 da marca); só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchReferences();

        /// <summary>
        /// busca a referência de um produto por código (POST product/v2/references/search com productCodeList, tentando as matrizes
        /// das marcas) e grava em TotvsSkus e filhas sempre, sem consultar o cache; as chamadas são registradas em totvs.TotvsRequests.
        /// true gravada, false não existe na Totvs, null empresa não cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetProductByCode(string user_name, string doc_company, int code);
    }
}
