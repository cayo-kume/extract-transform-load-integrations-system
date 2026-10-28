namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// conta bancária da pessoa física (BankAccountInDto): conta, dígito e chave PIX vão mascarados para totvs.TotvsRequests (T6)
    /// </summary>
    public class BankAccountIn
    {
        public string? holderName { get; init; }
        public string? additionalData { get; init; }
        public int? bankNumber { get; init; }
        public int? branchNumber { get; init; }
        public string? accountNumber { get; init; }
        public string? personType { get; init; }
        public string? accountDigit { get; init; }
        public string? accountType { get; init; }
        public string? purposeType { get; init; }
        public int? compensationType { get; init; }
        public string? checkType { get; init; }
        public string? pixKeyType { get; init; }
        public string? pixKey { get; init; }
    }
}
