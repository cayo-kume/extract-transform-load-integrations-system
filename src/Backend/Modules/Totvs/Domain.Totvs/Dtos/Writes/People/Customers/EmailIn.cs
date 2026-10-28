namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// e-mail (EmailInDto)
    /// </summary>
    public class EmailIn
    {
        public int? typeCode { get; init; }
        public string? email { get; init; }
        public bool? isDefault { get; init; }
    }
}
