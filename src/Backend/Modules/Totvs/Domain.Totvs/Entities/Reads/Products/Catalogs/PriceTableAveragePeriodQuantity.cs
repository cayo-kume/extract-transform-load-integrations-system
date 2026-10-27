using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// prazo médio por quantidade da tabela de preço, na posição em que a API o devolve (AveragePeriodQuantityModel, sem chave), tabela totvs.TotvsPriceTablesAveragePeriodQuantities
    /// </summary>
    public class PriceTableAveragePeriodQuantity
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int priceTableCode { get; private set; }
        public int sequence { get; private set; }
        public int? averagePeriod { get; private set; }
        public decimal? variationQuantity { get; private set; }
        public decimal? variationPercentage { get; private set; }

        public PriceTableAveragePeriodQuantity() { }

        public PriceTableAveragePeriodQuantity(string doc_company, DateTime integrated_or_updated_at, int priceTableCode, int sequence, Dtos.Reads.Products.Catalogs.PriceTableAveragePeriodQuantity dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.priceTableCode = priceTableCode;

            this.sequence = sequence;
            averagePeriod = NullableConverters.ToNullableInt32(dto.averagePeriod);
            variationQuantity = NullableConverters.ToNullableDecimal(dto.variationQuantity);
            variationPercentage = NullableConverters.ToNullableDecimal(dto.variationPercentage);
        }
    }
}
