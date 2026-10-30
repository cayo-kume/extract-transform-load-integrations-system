namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsSalesOrdersCommandHandler
    {
        public string CreateGetSalesOrdersLastChangeQuery(string doc_company);
        public string CreateGetSalesOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys);

        public string CreateGetSaleDiscountTypesLastChangeQuery(string doc_company);
        public string CreateGetSaleDiscountTypesQuery(string doc_company);
        public string CreateGetSaleClassificationsLastChangeQuery(string doc_company);
        public string CreateGetSaleClassificationsQuery(string doc_company);
        public string CreateGetSaleOrderBatchesLastChangeQuery(string doc_company);
        public string CreateGetSaleOrderBatchesQuery(string doc_company);
        public string CreateGetBillingSuggestionsLastChangeQuery(string doc_company);
        public string CreateGetBillingSuggestionsByKeysQuery(string doc_company, List<(int branchCode, int suggestionCode)> keys);
        public string CreateGetSaleOrderStatusQuery(string doc_company, int branchCode, int orderCode);
        public string CreateGetSaleOrderProductsQuery(string doc_company, int branchCode, int orderCode);
    }
}
