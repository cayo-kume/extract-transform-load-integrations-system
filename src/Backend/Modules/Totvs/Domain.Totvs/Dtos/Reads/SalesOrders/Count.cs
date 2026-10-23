namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// contagem (conferência) do pedido (countBranchCode é o branchCode da contagem)
    /// </summary>
    public class Count
    {
        public string? branchCode { get; init; }
        public string? code { get; init; }
        public string? description { get; init; }
        public string? statusCount { get; init; }
        public string? packagingTypeCode { get; init; }
        public string? packagingTypeDescription { get; init; }
        public string? separator { get; init; }
        public string? separatorName { get; init; }
    }
}
