using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.PriceTables
{
    /// <summary>
    /// preço do produto na tabela por loja e escala (scaleCode 0 sem escala), tabela totvs.TotvsPriceTablesItemsPrices
    /// </summary>
    public class PriceTableItemPrice
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int priceTableCode { get; private set; }
        public int productCode { get; private set; }
        public int branchCode { get; private set; }
        public int scaleCode { get; private set; }
        public decimal? originalPrice { get; private set; }
        public decimal? price { get; private set; }

        public PriceTableItemPrice() { }

        public PriceTableItemPrice(string doc_company, DateTime integrated_or_updated_at, int priceTableCode, int productCode, Dtos.Reads.Products.PriceTables.PriceTableProductPrice dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.priceTableCode = priceTableCode;
            this.productCode = productCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            scaleCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.scaleCode);
            originalPrice = NullableConverters.ToNullableDecimal(dto.originalPrice);
            price = NullableConverters.ToNullableDecimal(dto.price);
        }
    }
}
