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
        /// sincroniza os produtos similares pelo GET product/v2/similar, pela janela desde a última lida (a data de alteração da
        /// resposta não serve), dividida quando o volume é grande; só grava os produtos novos ou cujo hash mudou
        /// </summary>
        public Task<bool?> GetSimilar();

        /// <summary>
        /// sincroniza os lotes de todas as lojas pelo POST product/v2/batch/search, lidos inteiros; só grava os novos ou cujo hash mudou
        /// </summary>
        public Task<bool?> PostSearchBatch();

        /// <summary>
        /// confere, uma vez por dia, os produtos de cada marca (POST product/v2/product-codes/search) com os gravados das referências e
        /// recupera as referências dos que faltam
        /// </summary>
        public Task<bool?> PostSearchProductCodes();

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

        /// <summary>
        /// resolve um código (produto, pack, barras, RFID, item de lote) para o produto (GET product/v2/products/{code}), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetProducts(string user_name, string doc_company, string code, int? branchCode);

        /// <summary>
        /// devolve o kardex de um produto numa loja, até 180 dias (GET product/v2/kardex-movement), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetKardexMovement(string user_name, string doc_company, int productCode, int branchCode, DateTime startDate, DateTime endDate, int balanceType);

        /// <summary>
        /// devolve a grade de frequência de um produto (GET product/v2/grid-frequency), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetGridFrequency(string user_name, string doc_company, int productCode, int page, int pageSize);

        /// <summary>
        /// devolve as conversões de unidade de um produto ou grupo (GET product/v2/measurement-conversions), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetMeasurementConversions(string user_name, string doc_company, int? productCode, int? groupSequence, bool? onlyDefault);

        /// <summary>
        /// devolve a composição por produto (GET product/v2/composition-product), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetCompositionProduct(string user_name, string doc_company, List<int>? productCodeList, List<int>? groupCodeList, List<string>? referenceCodeList, int page, int pageSize);

        /// <summary>
        /// devolve a composição por grupo (GET product/v2/composition-group-product), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetCompositionGroupProduct(string user_name, string doc_company, List<int>? productCodeList, List<int>? groupCodeList, List<string>? referenceCodeList, int page, int pageSize);
    }
}
