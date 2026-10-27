using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// prazo médio da tabela de preço (AveragePeriodModel), tabela totvs.TotvsPriceTablesAveragePeriods
    /// </summary>
    public class PriceTableAveragePeriod
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int priceTableCode { get; private set; }
        public int quantity { get; private set; }
        public decimal? variationValue { get; private set; }
        public decimal? variationPercentage { get; private set; }

        public PriceTableAveragePeriod() { }

        public PriceTableAveragePeriod(string doc_company, DateTime integrated_or_updated_at, int priceTableCode, Dtos.Reads.Products.Catalogs.PriceTableAveragePeriod dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.priceTableCode = priceTableCode;

            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.quantity);
            variationValue = NullableConverters.ToNullableDecimal(dto.variationValue);
            variationPercentage = NullableConverters.ToNullableDecimal(dto.variationPercentage);
        }
    }
}
