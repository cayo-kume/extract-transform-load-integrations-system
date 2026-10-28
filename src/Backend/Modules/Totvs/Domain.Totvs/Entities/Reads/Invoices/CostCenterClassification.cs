using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// classificação do centro de custo, tabela totvs.TotvsCostCentersClassifications
    /// </summary>
    public class CostCenterClassification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int costCenterCode { get; private set; }
        public int typeCode { get; private set; }
        public string code { get; private set; } = String.Empty;
        public string typeDescription { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;

        public CostCenterClassification() { }

        public CostCenterClassification(string doc_company, DateTime integrated_or_updated_at, int costCenterCode, Dtos.Reads.Invoices.CostCenterClassification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.costCenterCode = costCenterCode;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            typeDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.typeDescription, 40);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
        }
    }
}
