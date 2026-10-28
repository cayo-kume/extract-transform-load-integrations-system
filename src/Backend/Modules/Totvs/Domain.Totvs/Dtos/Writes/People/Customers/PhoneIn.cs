namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// telefone (PhoneInDto)
    /// </summary>
    public class PhoneIn
    {
        public int? typeCode { get; init; }
        public string? number { get; init; }
        public int? branchLine { get; init; }
        public bool? isDefault { get; init; }
    }
}
