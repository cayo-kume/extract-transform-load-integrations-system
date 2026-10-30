namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsProductPricesCommandHandler
    {
        public string CreateGetPricesStateQuery(string doc_company, int branchCode);
        public string CreateGetPricesChangesQuery(string doc_company, int branchCode, List<int> productCodes);
        public string CreateGetCostsStateQuery(string doc_company, int branchCode);
        public string CreateGetCostsChangesQuery(string doc_company, int branchCode, List<int> productCodes);

        public string CreateGetActivePriceTableCodesQuery(string doc_company);
        public string CreateGetPriceTableItemsStateQuery(string doc_company, int priceTableCode);
        public string CreateGetPriceTableItemsChangesQuery(string doc_company, int priceTableCode, List<int> productCodes);
        public string CreateGetPromotionsHashesQuery(string doc_company, int branchCode);

        public string CreateGetCurrentProductValuesQuery(string doc_company, List<int> productCodes);
        public string CreateGetPromotionItemsQuery(string doc_company, int branchCode, int promotionCode);
    }
}
