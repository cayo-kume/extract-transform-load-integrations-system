namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// representante da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Representative
    {
        public string? representativeCode { get; init; }
        public string? cpfCnpj { get; init; }
        public string? classificationCode { get; init; }
        public string? classificationTypeCode { get; init; }
    }
}
