namespace Application.LinxCommerce.Abstractions.Handlers.Commands
{
    public interface ILinxCommerceCommandHandler
    {
        public string CreateGetCompanysQuery();

        public string CreateGetOrdersLastModifiedDateQuery(string doc_company);
        public string CreateGetOrdersByIdsQuery(string doc_company, List<Guid> ids);

        public string CreateGetCustomersLastBatchTimeQuery(string doc_company);

        public string CreateGetProductsLastModifiedDateQuery(string doc_company);
        public string CreateGetProductsByIdsQuery(string doc_company, List<int> ids);

        public string CreateGetSkusLastModifiedDateQuery(string doc_company);
        public string CreateGetSkusByIdsQuery(string doc_company, List<int> ids);
    }
}
