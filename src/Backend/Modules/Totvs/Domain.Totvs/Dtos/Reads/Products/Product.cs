namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// produto (SKU) da cor (ProductModel); code é único no cadastro
    /// </summary>
    public class Product
    {
        public string? code { get; init; }
        public string? sku { get; init; }
        public string? name { get; init; }
        public string? ncm { get; init; }
        public string? ipi { get; init; }
        public string? cst { get; init; }
        public string? cest { get; init; }
        public string? prefixEanGtin { get; init; }
        public string? measuredUnit { get; init; }
        public string? size { get; init; }
        public string? weight { get; init; }
        public string? isActive { get; init; }
        public string? insertDate { get; init; }
        public string? isFinishedProduct { get; init; }
        public string? isRawMaterial { get; init; }
        public string? isBulkMaterial { get; init; }
        public string? isOwnProduction { get; init; }
        public string? isKit { get; init; }
        public string? integrationProductCode { get; init; }
        public string? minimumStockAmount { get; init; }
        public string? maximumStockAmount { get; init; }
        public string? idealStockAmount { get; init; }
        public string? SalesStartDate { get; init; }
        public string? SalesEndDate { get; init; }
        public string? BillingForecastDate { get; init; }
        public string? isBlocked { get; init; }
        public string? balanceValidationType { get; init; }

        public List<Barcode>? barCodes { get; init; }
        public List<Classification>? classifications { get; init; }
        public List<AdditionalField>? additionalFields { get; init; }
        public List<Supplier>? suppliers { get; init; }
        public List<Manufacturer>? manufacturers { get; init; }
        public List<ReferenceCodeSequence>? referenceCodeSequences { get; init; }
        public List<WebData>? webData { get; init; }
        public List<WebDetail>? webDetail { get; init; }
        public List<BranchProductBlocked>? branchesProductBlocked { get; init; }
        public List<ConservationInstruction>? conservationInstructions { get; init; }
    }
}
