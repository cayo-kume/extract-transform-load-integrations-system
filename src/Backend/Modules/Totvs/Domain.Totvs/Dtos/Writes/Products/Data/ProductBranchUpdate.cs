namespace Domain.Totvs.Dtos.Writes.Products.Data
{
    /// <summary>
    /// corpo do PUT product/v2/products/{code}/{branchCode} (ProductBranchUpdateRequestModel): dados fiscais e indicadores do produto
    /// numa empresa. NCM e CST entram no cálculo de impostos da nota; isInactive tira o produto de venda na loja
    /// </summary>
    public class ProductBranchUpdate
    {
        public string? ncmCode { get; init; }
        public TaxStatusCodeType? cstCode { get; init; }
        public decimal? weight { get; init; }
        public bool? isOwnProduction { get; init; }
        public bool? isInactive { get; init; }
        public bool? isFinishedProduct { get; init; }
        public bool? isRawMaterial { get; init; }
        public bool? isBulkMaterial { get; init; }
    }
}
