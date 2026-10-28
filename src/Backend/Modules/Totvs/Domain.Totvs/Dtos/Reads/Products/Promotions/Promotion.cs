namespace Domain.Totvs.Dtos.Reads.Products.Promotions
{
    /// <summary>
    /// item de promoção: uma linha por promoção e produto (GET product/v2/promotion, ProductPromotionModel). status vem como letra
    /// (A aberta, B bloqueada, E encerrada), embora o filtro Status aceite só o nome do enum (Open, Blocked, Closed; testado)
    /// </summary>
    public class Promotion
    {
        public string? branchCode { get; init; }
        public string? promotionCode { get; init; }
        public string? promotionDescription { get; init; }
        public string? startDate { get; init; }
        public string? endDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? productCode { get; init; }
        public string? previousValue { get; init; }
        public string? promotionValue { get; init; }
        public string? status { get; init; }
    }
}
