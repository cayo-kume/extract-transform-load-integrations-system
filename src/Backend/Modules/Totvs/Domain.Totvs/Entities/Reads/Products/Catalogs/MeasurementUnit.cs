using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// unidade de medida, a espécie (GET product/v2/measurement-unit, MeasurementUnitModel), tabela totvs.TotvsMeasurementUnits e filhas
    /// </summary>
    public class MeasurementUnit
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<MeasurementUnitVariation> variations { get; private set; } = [];

        public MeasurementUnit() { }

        public MeasurementUnit(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.MeasurementUnit dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK
            variations.AddRange((dto.additionalVariations ?? []).Where(x => !String.IsNullOrWhiteSpace(x.code)).Select(x => new MeasurementUnitVariation(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.code.ToUpperInvariant()).Select(g => g.Last()));
        }
    }
}
