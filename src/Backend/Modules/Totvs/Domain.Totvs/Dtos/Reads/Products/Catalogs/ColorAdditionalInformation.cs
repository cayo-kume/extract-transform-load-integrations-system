namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// informação adicional da cor por empresa (AdditionalColorModel)
    /// </summary>
    public class ColorAdditionalInformation
    {
        public string? branchCode { get; init; }
        public string? groupName { get; init; }
        public string? auxiliaryCode { get; init; }
        public string? auxiliaryName { get; init; }
        public string? pantoneCode { get; init; }
        public string? colorType { get; init; }
        public string? colorTypeName { get; init; }
        public string? isActive { get; init; }
        public string? isFinishedProduct { get; init; }
        public string? isRawMaterial { get; init; }
        public string? isBulkMaterial { get; init; }
    }
}
