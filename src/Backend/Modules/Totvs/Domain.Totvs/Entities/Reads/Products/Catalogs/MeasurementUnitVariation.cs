using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// variação da unidade de medida (AdditionalMeasurementUnitVariationModel), tabela totvs.TotvsMeasurementUnitsVariations
    /// </summary>
    public class MeasurementUnitVariation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string measurementUnitCode { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public bool? isTribXml { get; private set; }

        public MeasurementUnitVariation() { }

        public MeasurementUnitVariation(string doc_company, DateTime integrated_or_updated_at, string measurementUnitCode, Dtos.Reads.Products.Catalogs.MeasurementUnitVariation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.measurementUnitCode = measurementUnitCode;

            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 40);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            isTribXml = NullableConverters.ToNullableBoolean(dto.isTribXml);
        }
    }
}
