namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderPaymentInfo do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderPaymentInfo
    {
        public string? Identifier { get; init; }
        public string? Alias { get; init; }
        public string? PaymentDate { get; init; }
        public string? ExpirationDate { get; init; }
        public string? Month { get; init; }
        public string? Year { get; init; }
        public string? TransactionNumber { get; init; }
        public string? AuthorizationCode { get; init; }
        public string? ReceiptCode { get; init; }
        public string? ReconciliationNumber { get; init; }
        public string? ConfirmationNumber { get; init; }
        public string? PaymentType { get; init; }
        public string? Provider { get; init; }
        public string? ProviderDocumentNumber { get; init; }
    }
}
