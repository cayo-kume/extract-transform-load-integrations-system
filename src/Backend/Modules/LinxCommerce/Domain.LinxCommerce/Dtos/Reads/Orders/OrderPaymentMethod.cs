namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderPaymentMethod do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderPaymentMethod
    {
        public string? OrderPaymentMethodID { get; init; }
        public string? PaymentNumber { get; init; }
        public string? PaymentMethodID { get; init; }
        public string? TransactionID { get; init; }
        public string? ReconciliationNumber { get; init; }
        public string? Status { get; init; }
        public string? IntegrationID { get; init; }
        public string? Amount { get; init; }
        public string? AmountNoInterest { get; init; }
        public string? InterestValue { get; init; }
        public string? PaidAmount { get; init; }
        public string? RefundAmount { get; init; }
        public string? Installments { get; init; }
        public string? InstallmentAmount { get; init; }
        public string? TaxAmount { get; init; }
        public string? PaymentDate { get; init; }
        public string? CaptureDate { get; init; }
        public string? AcquiredDate { get; init; }
        public string? PaymentCancelledDate { get; init; }

        public OrderPaymentInfo? PaymentInfo { get; init; }
    }
}
