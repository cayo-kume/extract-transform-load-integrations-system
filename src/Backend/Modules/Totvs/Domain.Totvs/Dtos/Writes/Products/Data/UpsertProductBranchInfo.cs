namespace Domain.Totvs.Dtos.Writes.Products.Data
{
    /// <summary>
    /// corpo do PUT product/v2/branch-info/{branchCode} (UpsertProductBranchInfoCommand): indicadores e tipo SPED dos produtos do filtro
    /// numa empresa
    /// </summary>
    public class UpsertProductBranchInfo
    {
        public ProductDataFilter? filter { get; init; }
        public bool? isOwnProduction { get; init; }
        public bool? isInactive { get; init; }
        public bool? isFinishedProduct { get; init; }
        public bool? isRawMaterial { get; init; }
        public bool? isBulkMaterial { get; init; }
        public SpedItemType? spedItemType { get; init; }
    }
}
