namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// SKU da referência (InsertProductModel): cor e tamanho; sem productCode, a Totvs gera. A classificação 6000 (marca) é obrigatória
    /// </summary>
    public class InsertProduct
    {
        public int? productCode { get; init; }
        public string? productName { get; init; }
        public string? colorCode { get; init; }
        public string? sizeName { get; init; }
        public string? ncmCode { get; init; }
        public string? measuredUnit { get; init; }
        public TaxStatusCodeType? cstCode { get; init; }
        public decimal? weight { get; init; }
        public string? prefixEanGtin { get; init; }
        public string? cestCode { get; init; }
        public int? conservationInstructionCode { get; init; }
        public List<Classifications.ProductClassificationItem>? classifications { get; init; }
        public List<ProductAdditionalField>? additionalFields { get; init; }
        public List<ProductSupplier>? suppliers { get; init; }
        public List<ProductManufacturer>? manufacturers { get; init; }
        public List<BarCodes.BarCodeInsert>? barCodes { get; init; }
        public List<ProductLocation>? locations { get; init; }
        public List<ProductValueInsert>? values { get; init; }
        public List<ProductImageInsert>? images { get; init; }
    }
}
