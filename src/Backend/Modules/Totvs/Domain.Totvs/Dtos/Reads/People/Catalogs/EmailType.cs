namespace Domain.Totvs.Dtos.Reads.People.Catalogs
{
    /// <summary>
    /// tipo de e-mail (GET person/v2/email-types, EmailTypeModel)
    /// </summary>
    public class EmailType
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? isDefault { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
