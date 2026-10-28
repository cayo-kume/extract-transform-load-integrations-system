namespace Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// sugestão de faturamento (GET sales-order/v2/billing-suggestions, BillingSuggestionModel); statusSuggestion vem como número (1 em andamento, 4 encerrada, 6 cancelada)
    /// </summary>
    public class BillingSuggestion
    {
        public string? branchCode { get; init; }
        public string? suggestionCode { get; init; }
        public string? statusSuggestion { get; init; }
        public string? suggestionDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<BillingSuggestionOrder>? orders { get; init; }
    }
}
