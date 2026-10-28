using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.PriceTables
{
    /// <summary>
    /// produto numa tabela de preço, tabela totvs.TotvsPriceTablesItems, com os preços por loja e escala
    /// </summary>
    public class PriceTableItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int priceTableCode { get; private set; }
        public int productCode { get; private set; }
        public int? referenceId { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<PriceTableItemPrice> prices { get; private set; } = [];

        public PriceTableItem() { }

        public PriceTableItem(string doc_company, DateTime integrated_or_updated_at, int priceTableCode, Dtos.Reads.Products.PriceTables.PriceTableProduct dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.priceTableCode = priceTableCode;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            referenceId = NullableConverters.ToNullableInt32(dto.referenceId);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK
            prices.AddRange((dto.prices ?? [])
                .Select(x => new PriceTableItemPrice(doc_company, integrated_or_updated_at, priceTableCode, productCode, x))
                .Where(x => x.branchCode != 0)
                .GroupBy(x => (x.branchCode, x.scaleCode))
                .Select(g => g.Last()));
        }
    }
}
