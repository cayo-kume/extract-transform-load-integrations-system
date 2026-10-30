namespace Application.LinxCommerce.Abstractions.Handlers.Commands
{
    public interface ILinxCommerceCatalogCommandHandler
    {
        public string CreateGetProductsLastModifiedDateQuery(string doc_company);
        public string CreateGetProductsByIdsQuery(string doc_company, List<int> ids);

        public string CreateGetSkusLastModifiedDateQuery(string doc_company);
        public string CreateGetSkusByIdsQuery(string doc_company, List<int> ids);
    }
}
