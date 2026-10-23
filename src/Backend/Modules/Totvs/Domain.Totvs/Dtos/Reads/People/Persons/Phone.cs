namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// telefone ("Sequence" com S maiúsculo na API) da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Phone
    {
        public string? Sequence { get; init; }
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? number { get; init; }
        public string? branchLine { get; init; }
        public string? isDefault { get; init; }
    }
}
