namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// referência comercial/pessoal da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Reference
    {
        public string? sequence { get; init; }
        public string? type { get; init; }
        public string? description { get; init; }
        public string? phoneNumber { get; init; }
        public string? responsiblePersonName { get; init; }
        public string? isInactive { get; init; }
    }
}
