namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ILinxCommerceCatalogService
    {
        /// <summary>
        /// sincroniza os produtos alterados desde a última leitura (POST Catalog/SearchProduct pela ModifiedDate e GetProduct de
        /// cada um); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> SearchProduct();

        /// <summary>
        /// sincroniza os SKUs alterados desde a última leitura (POST Catalog/SearchSKU pela ModifiedDate e GetSKU de cada um);
        /// só grava os novos ou alterados
        /// </summary>
        public Task<bool?> SearchSKU();

        /// <summary>
        /// busca um produto pelo ProductID (POST Catalog/GetProduct) e grava sempre, sem consultar o cache; a chamada é registrada
        /// em linxcommerce.LinxCommerceRequests. true gravado, false não existe no Linx Commerce, null empresa não cadastrada ou
        /// resposta inválida
        /// </summary>
        public Task<bool?> GetProduct(string user_name, string doc_company, int productId);

        /// <summary>
        /// busca um SKU pelo ProductID (POST Catalog/GetSKU); mesmo contrato do GetProduct
        /// </summary>
        public Task<bool?> GetSKU(string user_name, string doc_company, int productId);
    }
}
