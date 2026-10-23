namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// pessoa relacionada da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Related
    {
        public string? code { get; init; }
        public string? cpfCnpj { get; init; }
        public string? name { get; init; }
    }
}
