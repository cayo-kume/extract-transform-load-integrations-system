namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// preço de um produto numa loja (ProductPriceItemModel)
    /// </summary>
    public class ProductPriceItem
    {
        public string? branchCode { get; init; }
        public string? priceCode { get; init; }
        public string? priceName { get; init; }
        public string? price { get; init; }
        public string? promotionalPrice { get; init; }
        public PromotionalInformation? promotionalInformation { get; init; }
        public List<OtherPromotion>? informationOtherPromotions { get; init; }
    }
}
