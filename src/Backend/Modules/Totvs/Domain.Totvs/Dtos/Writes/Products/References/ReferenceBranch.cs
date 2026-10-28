namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// dados da referência por empresa (BranchModel); a loja precisa ser da empresa
    /// </summary>
    public class ReferenceBranch
    {
        public int? branchCode { get; init; }
        public bool? isOwnProduction { get; init; }
        public bool? isInactive { get; init; }
        public bool? isFinishedProduct { get; init; }
        public bool? isRawMaterial { get; init; }
        public bool? isBulkMaterial { get; init; }
        public bool? isFractionalQuantity { get; init; }
        public SpedItemType? spedItemType { get; init; }
        public InspectionType? inspectionType { get; init; }
        public BatchType? batchType { get; init; }
    }
}
