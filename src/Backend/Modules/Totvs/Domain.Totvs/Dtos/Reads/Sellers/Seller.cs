namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// vendedor do POST seller/v2/search (SellerDto); a rota não tem expand: as filhas vêm sempre
    /// </summary>
    public class Seller
    {
        public string? sellerCode { get; init; }
        public string? sellerName { get; init; }
        public string? personCode { get; init; }
        public string? personName { get; init; }
        public string? maxChangeFilterDate { get; init; }

        public List<BranchInformation>? branchInformations { get; init; }
        public List<Customer>? customers { get; init; }
    }
}
