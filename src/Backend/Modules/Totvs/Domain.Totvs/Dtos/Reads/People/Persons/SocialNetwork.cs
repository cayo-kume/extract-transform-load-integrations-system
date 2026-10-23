namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// rede social da pessoa física ou jurídica (API Person)
    /// </summary>
    public class SocialNetwork
    {
        public string? sequence { get; init; }
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? address { get; init; }
    }
}
