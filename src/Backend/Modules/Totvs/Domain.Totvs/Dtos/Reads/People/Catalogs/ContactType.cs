namespace Domain.Totvs.Dtos.Reads.People.Catalogs
{
    /// <summary>
    /// tipo de contato (GET person/v2/contact-types; o swagger declara o PhoneTypeModel, mas a resposta real traz typeName)
    /// </summary>
    public class ContactType
    {
        public string? code { get; init; }
        public string? typeName { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
