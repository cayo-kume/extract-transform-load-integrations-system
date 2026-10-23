namespace Domain.Totvs.Dtos.Reads.People.Branches
{
    /// <summary>
    /// empresa (loja) do GET person/v2/branchesList (BranchListModel)
    /// </summary>
    public class Branch
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? cnpj { get; init; }
        public string? personCode { get; init; }
        public string? personName { get; init; }
        public string? fantasyName { get; init; }
        public string? branchGroupName { get; init; }
        public string? lastchange { get; init; }
        public string? maxChangeFilterDate { get; init; }

        public List<Address>? addresses { get; init; }
    }
}
