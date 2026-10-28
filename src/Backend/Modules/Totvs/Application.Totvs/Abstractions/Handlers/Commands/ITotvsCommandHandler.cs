namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsCommandHandler
    {
        public string CreateGetCompanysQuery();
        public string CreateGetBranchCodesQuery(string doc_company);
        public string CreateGetBrandBranchCodesQuery(string doc_company, int baseBranchCode);
        public string CreateGetBranchCodeByCnpjQuery(string doc_company, string cnpj);
        public string CreateGetRecentRequestsQuery(string doc_company, string route, string field_key_value, DateTime since);

        public string CreateGetBranchesLastChangeQuery(string doc_company);
        public string CreateGetBranchesByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetCustomersLastChangeQuery(string doc_company);
        public string CreateGetCustomersByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetLegalEntitiesLastChangeQuery(string doc_company);
        public string CreateGetLegalEntitiesByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetSellersLastChangeQuery(string doc_company);
        public string CreateGetSellersByCodesQuery(string doc_company, List<int> codes);
        public string CreateGetOperationalAreasHashesQuery(string doc_company);

        public string CreateGetPurchaseOrdersLastChangeQuery(string doc_company);
        public string CreateGetPurchaseOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys);

        public string CreateGetSalesOrdersLastChangeQuery(string doc_company);
        public string CreateGetSalesOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys);

        public string CreateGetDuplicatesLastChangeQuery(string doc_company);
        public string CreateGetDuplicatesByKeysQuery(string doc_company, List<(int branchCode, int supplierCode, long duplicateCode, int installmentCode)> keys);

        public string CreateGetInvoicesLastChangeQuery(string doc_company);
        public string CreateGetInvoicesByKeysQuery(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys);

        public string CreateGetPersonClassificationsLastChangeQuery(string doc_company);
        public string CreateGetPersonClassificationsQuery(string doc_company);
        public string CreateGetEmailTypesLastChangeQuery(string doc_company);
        public string CreateGetEmailTypesQuery(string doc_company);
        public string CreateGetPhoneTypesLastChangeQuery(string doc_company);
        public string CreateGetPhoneTypesQuery(string doc_company);
        public string CreateGetContactTypesLastChangeQuery(string doc_company);
        public string CreateGetContactTypesQuery(string doc_company);

        public string CreateGetRepresentativesLastChangeQuery(string doc_company);
        public string CreateGetRepresentativesByCodesQuery(string doc_company, List<int> codes);
        public string CreateGetReceptionSalesLastMovementDateQuery(string doc_company);
        public string CreateGetReceptionSalesHashesQuery(string doc_company, DateTime startMovementDate, DateTime endMovementDate);

        public string CreateGetSkusLastChangeQuery(string doc_company, int branchInfoCode);
        public string CreateGetSkusByIdsQuery(string doc_company, int branchInfoCode, List<int> referenceIds);

        public string CreateGetClassificationTypesLastChangeQuery(string doc_company);
        public string CreateGetClassificationTypesQuery(string doc_company);
        public string CreateGetProductClassificationsLastChangeQuery(string doc_company);
        public string CreateGetProductClassificationsQuery(string doc_company);
        public string CreateGetCategoriesLastChangeQuery(string doc_company);
        public string CreateGetCategoriesQuery(string doc_company);
        public string CreateGetGridsLastChangeQuery(string doc_company);
        public string CreateGetGridsQuery(string doc_company);
        public string CreateGetMeasurementUnitsLastChangeQuery(string doc_company);
        public string CreateGetMeasurementUnitsQuery(string doc_company);
        public string CreateGetColorsHashesQuery(string doc_company);
        public string CreateGetAdditionalFieldsTypesLastChangeQuery(string doc_company);
        public string CreateGetAdditionalFieldsTypesQuery(string doc_company);
        public string CreateGetProductGrouperConfigurationsLastChangeQuery(string doc_company);
        public string CreateGetProductGrouperConfigurationsQuery(string doc_company);
        public string CreateGetInstructionItemsLastChangeQuery(string doc_company);
        public string CreateGetInstructionItemsQuery(string doc_company);
        public string CreateGetCompositionsLastChangeQuery(string doc_company);
        public string CreateGetCompositionsQuery(string doc_company);
        public string CreateGetPriceTablesLastChangeQuery(string doc_company);
        public string CreateGetPriceTablesQuery(string doc_company);
        public string CreateGetPriceTableScalesLastChangeQuery(string doc_company);
        public string CreateGetPriceTableScalesQuery(string doc_company);

        public string CreateGetPricesStateQuery(string doc_company, int branchCode);
        public string CreateGetPricesChangesQuery(string doc_company, int branchCode, List<int> productCodes);
        public string CreateGetCostsStateQuery(string doc_company, int branchCode);
        public string CreateGetCostsChangesQuery(string doc_company, int branchCode, List<int> productCodes);
        public string CreateGetBalancesStateQuery(string doc_company, int branchCode);
        public string CreateGetBalancesChangesQuery(string doc_company, int branchCode, List<int> productCodes);

        public string CreateGetActivePriceTableCodesQuery(string doc_company);
        public string CreateGetPriceTableItemsStateQuery(string doc_company, int priceTableCode);
        public string CreateGetPriceTableItemsChangesQuery(string doc_company, int priceTableCode, List<int> productCodes);
        public string CreateGetPromotionsHashesQuery(string doc_company, int branchCode);

        public string CreateGetSimilarHashesQuery(string doc_company);
        public string CreateGetSimilarLastWindowEndQuery(string doc_company);
        public string CreateGetBatchesHashesQuery(string doc_company, int branchCode);
        public string CreateGetSkusProductCodesQuery(string doc_company, int branchInfoCode);

        public string CreateGetSaleDiscountTypesLastChangeQuery(string doc_company);
        public string CreateGetSaleDiscountTypesQuery(string doc_company);
        public string CreateGetSaleClassificationsLastChangeQuery(string doc_company);
        public string CreateGetSaleClassificationsQuery(string doc_company);
        public string CreateGetSaleOrderBatchesLastChangeQuery(string doc_company);
        public string CreateGetSaleOrderBatchesQuery(string doc_company);
        public string CreateGetBillingSuggestionsLastChangeQuery(string doc_company);
        public string CreateGetInvoicesDisableLastChangeQuery(string doc_company);
        public string CreateGetInvoicesDisableQuery(string doc_company);
        public string CreateGetCostCentersHashesQuery(string doc_company);
        public string CreateGetBillingSuggestionsByKeysQuery(string doc_company, List<(int branchCode, int suggestionCode)> keys);
        public string CreateGetPurchaseOrderStatusQuery(string doc_company, int branchCode, int orderCode);
    }
}
