namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsAccountsPayableCommandHandler
    {
        public string CreateGetDuplicatesLastChangeQuery(string doc_company);
        public string CreateGetDuplicatesByKeysQuery(string doc_company, List<(int branchCode, int supplierCode, long duplicateCode, int installmentCode)> keys);
    }
}
