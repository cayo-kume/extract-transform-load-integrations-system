namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// e-mail da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Email
    {
        public string? sequence { get; init; }
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? email { get; init; }
        public string? isDefault { get; init; }
    }
}
