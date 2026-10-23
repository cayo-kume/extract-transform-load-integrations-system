namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsCommandHandler
    {
        public string CreateGetCompanysQuery();
        public string CreateGetBranchCodesQuery(string doc_company);
        public string CreateGetBrandBranchCodesQuery(string doc_company, int baseBranchCode);

        public string CreateGetBranchesLastChangeQuery(string doc_company);
        public string CreateGetBranchesByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetCustomersLastChangeQuery(string doc_company);
        public string CreateGetCustomersByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetLegalEntitiesLastChangeQuery(string doc_company);
        public string CreateGetLegalEntitiesByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetSellersLastChangeQuery(string doc_company);
        public string CreateGetSellersByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetPurchaseOrdersLastChangeQuery(string doc_company);
        public string CreateGetPurchaseOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys);

        public string CreateGetSalesOrdersLastChangeQuery(string doc_company);
        public string CreateGetSalesOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys);

        public string CreateGetDuplicatesLastChangeQuery(string doc_company);
        public string CreateGetDuplicatesByKeysQuery(string doc_company, List<(int branchCode, int supplierCode, long duplicateCode, int installmentCode)> keys);

        public string CreateGetInvoicesLastChangeQuery(string doc_company);
        public string CreateGetInvoicesByKeysQuery(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys);

        public string CreateGetSkusLastChangeQuery(string doc_company, int branchInfoCode);
        public string CreateGetSkusByIdsQuery(string doc_company, int branchInfoCode, List<int> referenceIds);
    }
}
