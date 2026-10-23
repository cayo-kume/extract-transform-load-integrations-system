namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// contato da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Contact
    {
        public string? sequence { get; init; }
        public string? name { get; init; }
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? function { get; init; }
        public string? phoneNumber { get; init; }
        public string? cellNumber { get; init; }
        public string? email { get; init; }
        public string? birthDate { get; init; }
        public string? isDefault { get; init; }
    }
}
