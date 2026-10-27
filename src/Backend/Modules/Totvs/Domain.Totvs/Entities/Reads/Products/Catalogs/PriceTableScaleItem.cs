using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// item da escala de tabela de preço (PriceTableScaleItemModel), tabela totvs.TotvsPriceTableScalesItems
    /// </summary>
    public class PriceTableScaleItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int scaleCode { get; private set; }
        public int quantity { get; private set; }
        public decimal? variationValue { get; private set; }
        public decimal? variationPercentage { get; private set; }

        public PriceTableScaleItem() { }

        public PriceTableScaleItem(string doc_company, DateTime integrated_or_updated_at, int scaleCode, Dtos.Reads.Products.Catalogs.PriceTableScaleItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.scaleCode = scaleCode;

            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.quantity);
            variationValue = NullableConverters.ToNullableDecimal(dto.variationValue);
            variationPercentage = NullableConverters.ToNullableDecimal(dto.variationPercentage);
        }
    }
}
