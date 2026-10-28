namespace Domain.Totvs.Dtos.Writes.Products.Values
{
    /// <summary>
    /// produto e valor de promoção (UpdatePromotionItemCommand e PromotionItemInsertModel)
    /// </summary>
    public class PromotionItem
    {
        public int? productCode { get; init; }
        public decimal? promotionValue { get; init; }
    }
}
