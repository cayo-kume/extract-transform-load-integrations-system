namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// campo adicional da pessoa física ou jurídica (API Person)
    /// </summary>
    public class AdditionalField
    {
        public string? code { get; init; }
        public string? type { get; init; }
        public string? name { get; init; }
        public string? value { get; init; }
    }
}
