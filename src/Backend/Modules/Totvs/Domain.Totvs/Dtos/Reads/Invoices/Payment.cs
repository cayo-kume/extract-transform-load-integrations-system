namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// pagamento da nota (documentNumber é int64 na API)
    /// </summary>
    public class Payment
    {
        public string? documentNumber { get; init; }
        public string? installment { get; init; }
        public string? expirationDate { get; init; }
        public string? paymentValue { get; init; }
        public string? documentTypeCode { get; init; }
        public string? documentType { get; init; }
        public string? bearerCode { get; init; }
        public string? bearerName { get; init; }

        public CardInformation? cardInformation { get; init; }
        public DigitalWalletInformation? digitalWalletInformation { get; init; }
    }
}
