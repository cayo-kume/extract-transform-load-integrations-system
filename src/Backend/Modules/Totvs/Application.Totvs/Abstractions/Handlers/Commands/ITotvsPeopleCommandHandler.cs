namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsPeopleCommandHandler
    {
        public string CreateGetBranchesLastChangeQuery(string doc_company);
        public string CreateGetBranchesByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetCustomersLastChangeQuery(string doc_company);
        public string CreateGetCustomersByCodesQuery(string doc_company, List<int> codes);

        public string CreateGetLegalEntitiesLastChangeQuery(string doc_company);
        public string CreateGetLegalEntitiesByCodesQuery(string doc_company, List<int> codes);
    }
}
