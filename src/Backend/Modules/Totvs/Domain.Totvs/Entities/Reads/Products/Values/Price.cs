using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// preço de um produto numa loja, por tipo de preço (PRDFL003), tabela totvs.TotvsPrices
    /// </summary>
    public class Price
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int productCode { get; private set; }
        public int priceCode { get; private set; }
        public string priceName { get; private set; } = String.Empty;
        public decimal? price { get; private set; }
        public decimal? promotionalPrice { get; private set; }
        public int? promotionalInformationBranchCode { get; private set; }
        public int? promotionalInformationCode { get; private set; }
        public string promotionalInformationDescription { get; private set; } = String.Empty;
        public DateTime? promotionalInformationStartDate { get; private set; }
        public DateTime? promotionalInformationEndDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<PriceOtherPromotion> otherPromotions { get; private set; } = [];

        public Price() { }

        public Price(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Values.ProductPrice product, Dtos.Reads.Products.Values.ProductPriceItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(product.productCode);
            priceCode = (int)(NullableConverters.ToNullableDecimal(dto.priceCode) ?? 0);
            priceName = ConverterValidatorsExtensions.StringLengthValidation(dto.priceName, 60);
            price = NullableConverters.ToNullableDecimal(dto.price);
            promotionalPrice = NullableConverters.ToNullableDecimal(dto.promotionalPrice);
            promotionalInformationBranchCode = NullableConverters.ToNullableInt32(dto.promotionalInformation?.branchCode);
            promotionalInformationCode = NullableConverters.ToNullableInt32(dto.promotionalInformation?.code);
            promotionalInformationDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.promotionalInformation?.description, 40);
            promotionalInformationStartDate = NullableConverters.ToNullableDateTime(dto.promotionalInformation?.startDate);
            promotionalInformationEndDate = NullableConverters.ToNullableDateTime(dto.promotionalInformation?.endDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(product.maxChangeFilterDate);

            // repetidos violariam a PK
            otherPromotions.AddRange((dto.informationOtherPromotions ?? [])
                .Select(x => new PriceOtherPromotion(doc_company, integrated_or_updated_at, branchCode, productCode, priceCode, x))
                .Where(x => x.promotionCode != 0)
                .GroupBy(x => (x.promotionBranchCode, x.promotionCode))
                .Select(g => g.Last()));
        }
    }
}
