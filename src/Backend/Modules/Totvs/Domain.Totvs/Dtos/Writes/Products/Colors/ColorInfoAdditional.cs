namespace Domain.Totvs.Dtos.Writes.Products.Colors
{
    /// <summary>
    /// dados da cor numa loja (UpsertColorInfoAdditionalModel); a loja precisa ser da empresa em totvs.TotvsBranches
    /// </summary>
    public class ColorInfoAdditional
    {
        public int? branchCode { get; init; }

        public string? auxiliaryCode { get; init; }     // até 10
        public string? auxiliaryName { get; init; }     // até 80
        public string? pantoneCode { get; init; }       // até 40

        public bool? isFinishedProduct { get; init; }
        public bool? isRawMaterial { get; init; }
        public bool? isBulkMaterial { get; init; }
    }
}
