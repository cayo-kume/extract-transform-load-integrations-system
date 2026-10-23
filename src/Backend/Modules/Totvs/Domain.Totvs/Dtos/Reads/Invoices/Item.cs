namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// item da nota (sequence nulo é rejeitado na validação e vira 0)
    /// </summary>
    public class Item
    {
        public string? sequence { get; init; }
        public string? code { get; init; }
        public string? name { get; init; }
        public string? ncm { get; init; }
        public string? cfop { get; init; }
        public string? measureUnit { get; init; }
        public string? quantity { get; init; }
        public string? grossValue { get; init; }
        public string? discountValue { get; init; }
        public string? netValue { get; init; }
        public string? unitGrossValue { get; init; }
        public string? unitDiscountValue { get; init; }
        public string? unitNetValue { get; init; }
        public string? additionalValue { get; init; }
        public string? freightValue { get; init; }
        public string? insuranceValue { get; init; }
        public string? cest { get; init; }
        public string? additionalItemInformation { get; init; }

        public RateDifferential? rateDifferential { get; init; }
        public List<ItemProduct>? products { get; init; }
        public List<ItemTax>? taxes { get; init; }
        public List<ItemBarcode>? barCodes { get; init; }
    }
}
