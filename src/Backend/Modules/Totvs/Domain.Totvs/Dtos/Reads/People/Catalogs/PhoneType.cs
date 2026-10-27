namespace Domain.Totvs.Dtos.Reads.People.Catalogs
{
    /// <summary>
    /// tipo de telefone (GET person/v2/phone-types, PhoneTypeModel)
    /// </summary>
    public class PhoneType
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? isDefault { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
