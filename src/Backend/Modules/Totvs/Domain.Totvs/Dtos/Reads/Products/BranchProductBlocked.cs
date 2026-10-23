namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// bloqueio do produto por empresa (BranchProductBlockedModel)
    /// </summary>
    public class BranchProductBlocked
    {
        public string? branchCode { get; init; }
        public string? isBlocked { get; init; }
    }
}
