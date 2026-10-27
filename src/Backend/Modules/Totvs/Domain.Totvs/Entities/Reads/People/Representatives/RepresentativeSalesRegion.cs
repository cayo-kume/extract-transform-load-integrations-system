using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Representatives
{
    /// <summary>
    /// região de venda do representante (RepresentativeSaleRegionModel), tabela totvs.TotvsRepresentativesSalesRegions. personCode é o código do representante (uma pessoa do PESFM010)
    /// </summary>
    public class RepresentativeSalesRegion
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int salesRegionCode { get; private set; }
        public string salesRegionName { get; private set; } = String.Empty;
        public int salesOrderClassificationTypeCode { get; private set; }
        public string salesOrderClassificationTypeName { get; private set; } = String.Empty;
        public string salesOrderClassificationCode { get; private set; } = String.Empty;
        public string salesOrderClassificationName { get; private set; } = String.Empty;

        public RepresentativeSalesRegion() { }

        public RepresentativeSalesRegion(string doc_company, DateTime integrated_or_updated_at, int personCode, Dtos.Reads.People.Representatives.RepresentativeSalesRegion dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            salesRegionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.salesRegionCode);
            salesRegionName = ConverterValidatorsExtensions.StringLengthValidation(dto.salesRegionName, 100);
            salesOrderClassificationTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.salesOrderClassificationTypeCode);
            salesOrderClassificationTypeName = ConverterValidatorsExtensions.StringLengthValidation(dto.salesOrderClassificationTypeName, 100);
            salesOrderClassificationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.salesOrderClassificationCode, 10);
            salesOrderClassificationName = ConverterValidatorsExtensions.StringLengthValidation(dto.salesOrderClassificationName, 100);
        }
    }
}
