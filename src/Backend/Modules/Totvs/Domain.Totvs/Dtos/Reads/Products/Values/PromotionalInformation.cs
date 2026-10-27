namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// promoção vigente do preço (PromotionalInformationModel), gravada como colunas do preço (decisão 26)
    /// </summary>
    public class PromotionalInformation
    {
        public string? branchCode { get; init; }
        public string? code { get; init; }
        public string? description { get; init; }
        public string? startDate { get; init; }
        public string? endDate { get; init; }
    }
}
