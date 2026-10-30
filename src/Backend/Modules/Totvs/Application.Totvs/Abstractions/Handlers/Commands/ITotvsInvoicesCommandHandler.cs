namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsInvoicesCommandHandler
    {
        public string CreateGetInvoicesLastChangeQuery(string doc_company);
        public string CreateGetInvoicesByKeysQuery(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys);

        public string CreateGetInvoicesDisableLastChangeQuery(string doc_company);
        public string CreateGetInvoicesDisableQuery(string doc_company);
        public string CreateGetCostCentersHashesQuery(string doc_company);
    }
}
