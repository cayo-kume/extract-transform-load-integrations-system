namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsCommandHandler
    {
        public string CreateGetCompanysQuery();
        public string CreateGetBranchCodesQuery(string doc_company);
        public string CreateGetBrandBranchCodesQuery(string doc_company, int baseBranchCode);
        public string CreateGetBranchCodeByCnpjQuery(string doc_company, string cnpj);
        public string CreateGetRecentRequestsQuery(string doc_company, string route, string field_key_value, DateTime since);
    }
}
