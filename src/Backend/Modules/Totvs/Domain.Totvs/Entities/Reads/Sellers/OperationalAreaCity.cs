using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// cidade da região de atuação, tabela totvs.TotvsOperationalAreasCities
    /// </summary>
    public class OperationalAreaCity
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int operationalAreaCode { get; private set; }
        public int cityCode { get; private set; }
        public string cityName { get; private set; } = String.Empty;
        public int? stateCode { get; private set; }
        public string stateAbbreviation { get; private set; } = String.Empty;
        public string stateName { get; private set; } = String.Empty;

        public OperationalAreaCity() { }

        public OperationalAreaCity(string doc_company, DateTime integrated_or_updated_at, int operationalAreaCode, Dtos.Reads.Sellers.OperationalAreaCity dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.operationalAreaCode = operationalAreaCode;

            cityCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.cityCode);
            cityName = ConverterValidatorsExtensions.StringLengthValidation(dto.cityName, 100);
            stateCode = NullableConverters.ToNullableInt32(dto.stateCode);
            stateAbbreviation = ConverterValidatorsExtensions.StringLengthValidation(dto.stateAbbreviation, 2);
            stateName = ConverterValidatorsExtensions.StringLengthValidation(dto.stateName, 60);
        }
    }
}
