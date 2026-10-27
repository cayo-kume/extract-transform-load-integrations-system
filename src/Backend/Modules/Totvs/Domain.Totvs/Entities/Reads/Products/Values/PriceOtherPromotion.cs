using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// outra promoção do preço (informationOtherPromotions), tabela totvs.TotvsPricesOtherPromotions
    /// </summary>
    public class PriceOtherPromotion
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int productCode { get; private set; }
        public int priceCode { get; private set; }
        public int promotionBranchCode { get; private set; }
        public int promotionCode { get; private set; }
        public string description { get; private set; } = String.Empty;
        public DateTime? startDate { get; private set; }
        public DateTime? endDate { get; private set; }
        public decimal? price { get; private set; }

        public PriceOtherPromotion() { }

        public PriceOtherPromotion(string doc_company, DateTime integrated_or_updated_at, int branchCode, int productCode, int priceCode, Dtos.Reads.Products.Values.OtherPromotion dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            this.branchCode = branchCode;
            this.productCode = productCode;
            this.priceCode = priceCode;
            promotionBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            promotionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            startDate = NullableConverters.ToNullableDateTime(dto.startDate);
            endDate = NullableConverters.ToNullableDateTime(dto.endDate);
            price = NullableConverters.ToNullableDecimal(dto.price);
        }
    }
}
