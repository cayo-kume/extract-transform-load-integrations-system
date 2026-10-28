namespace Domain.Totvs.Dtos.Writes.Products.Values
{
    /// <summary>
    /// corpo do POST product/v2/promotion-values/update e /promotion-items (UpdatePromotionValueCommand e InsertPromotionItemCommand):
    /// valores de promoção dos produtos numa campanha da loja; mudam o preço de venda dentro do período
    /// </summary>
    public class PromotionItems
    {
        public int? branchCode { get; init; }
        public int? promotionCode { get; init; }
        public List<PromotionItem>? items { get; init; }
    }
}
