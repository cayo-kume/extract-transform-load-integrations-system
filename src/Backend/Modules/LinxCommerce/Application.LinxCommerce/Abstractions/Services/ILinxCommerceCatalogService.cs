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
    }
}
