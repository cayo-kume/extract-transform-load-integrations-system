namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderInvoice do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderInvoice
    {
        public string? OrderInvoiceID { get; init; }
        public string? Code { get; init; }
        public string? Url { get; init; }
        public string? FulfillmentID { get; init; }
        public string? IsIssued { get; init; }
        public string? Series { get; init; }
        public string? Number { get; init; }
        public string? CFOP { get; init; }
        public string? XML { get; init; }
        public string? InvoicePdf { get; init; }
        public string? Observation { get; init; }
        public string? Operation { get; init; }
        public string? ProcessedAt { get; init; }
        public string? UpdatedAt { get; init; }
        public string? IssuedAt { get; init; }
        public string? CreatedAt { get; init; }
    }
}
