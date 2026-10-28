namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// contagem ligada (CountsRequestModel), com a sugestão de faturamento aos pares
    /// </summary>
    public class OrderCount
    {
        public int? branchCountCode { get; init; }
        public int? countCode { get; init; }
        public int? branchSuggestionCode { get; init; }
        public int? suggestionCode { get; init; }
    }
}
