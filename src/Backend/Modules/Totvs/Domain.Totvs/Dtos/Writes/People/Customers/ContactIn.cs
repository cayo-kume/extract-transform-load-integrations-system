namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// contato da pessoa jurídica (ContactInDto): sequence só na alteração
    /// </summary>
    public class ContactIn
    {
        public int? sequence { get; init; }
        public int? typeCode { get; init; }
        public string? name { get; init; }
        public string? function { get; init; }
        public string? phoneNumber { get; init; }
        public string? cellNumber { get; init; }
        public string? email { get; init; }
        public DateTime? birthDate { get; init; }
        public bool? isDefault { get; init; }
    }
}
