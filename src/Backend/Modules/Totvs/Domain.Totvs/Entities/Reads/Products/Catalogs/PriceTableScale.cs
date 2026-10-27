using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// escala de tabela de preço (GET product/v2/price-table-scales, PriceTableScaleModel), tabela totvs.TotvsPriceTableScales e filhas
    /// </summary>
    public class PriceTableScale
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int scaleCode { get; private set; }
        public string scaleDescription { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<PriceTableScaleItem> items { get; private set; } = [];

        public PriceTableScale() { }

        public PriceTableScale(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.PriceTableScale dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            scaleCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.scaleCode);
            scaleDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.scaleDescription, 60);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK
            items.AddRange((dto.scales ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.quantity) != 0).Select(x => new PriceTableScaleItem(doc_company, integrated_or_updated_at, scaleCode, x)).GroupBy(x => x.quantity).Select(g => g.Last()));
        }
    }
}
