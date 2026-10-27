namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// outra promoção do mesmo preço (InformationOtherPromotionsModel)
    /// </summary>
    public class OtherPromotion
    {
        public string? branchCode { get; init; }
        public string? code { get; init; }
        public string? description { get; init; }
        public string? startDate { get; init; }
        public string? endDate { get; init; }
        public string? price { get; init; }
    }
}
