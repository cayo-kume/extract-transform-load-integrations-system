namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// informação do vendedor por empresa (SellerBranchInformationDto); só vêm as empresas do branchCodeList
    /// </summary>
    public class BranchInformation
    {
        public string? branchCode { get; init; }
        public string? auxiliaryCode { get; init; }
        public string? isInactive { get; init; }
        public string? sellerTypeCode { get; init; }
        public string? sellerTypeDescription { get; init; }
    }
}
