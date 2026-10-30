namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsProductsCommandHandler
    {
        public string CreateGetSkusLastChangeQuery(string doc_company, int branchInfoCode);
        public string CreateGetSkusByIdsQuery(string doc_company, int branchInfoCode, List<int> referenceIds);

        public string CreateGetBalancesStateQuery(string doc_company, int branchCode);
        public string CreateGetBalancesChangesQuery(string doc_company, int branchCode, List<int> productCodes);

        public string CreateGetSimilarHashesQuery(string doc_company);
        public string CreateGetSimilarLastWindowEndQuery(string doc_company);
        public string CreateGetBatchesHashesQuery(string doc_company, int branchCode);
        public string CreateGetSkusProductCodesQuery(string doc_company, int branchInfoCode);
    }
}
