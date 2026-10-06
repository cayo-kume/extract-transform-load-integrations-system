using Domain.Shared.Extensions;

namespace Domain.Totvs.Dtos.Person.Branch
{
    public class Branch
    {
        public string code { get; init; }
        public string description { get; init; }
        public string cnpj { get; init; }
        public string personCode { get; init; }
        public string personName { get; init; }
        public string fantasyName { get; init; }
        public string branchGroupName { get; init; }
        public string lastchange { get; init; }
        public string maxChangeFilterDate { get; init; }
        
        public List<Address> addresses { get; init; }
    }
}
