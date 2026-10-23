namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// sócio da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Partner
    {
        public string? code { get; init; }
        public string? cpfCnpj { get; init; }
        public string? name { get; init; }
        public string? percentageParticipation { get; init; }
    }
}
