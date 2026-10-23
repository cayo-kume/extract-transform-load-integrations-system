namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// produto do item da nota
    /// </summary>
    public class ItemProduct
    {
        public string? productCode { get; init; }
        public string? productName { get; init; }
        public string? dealerCode { get; init; }
        public string? quantity { get; init; }
        public string? unitGrossValue { get; init; }
        public string? unitDiscountValue { get; init; }
        public string? unitNetValue { get; init; }
        public string? grossValue { get; init; }
        public string? discountValue { get; init; }
        public string? netValue { get; init; }

        public List<ItemProductReferencedInvoice>? referencedInvoices { get; init; }
    }
}
