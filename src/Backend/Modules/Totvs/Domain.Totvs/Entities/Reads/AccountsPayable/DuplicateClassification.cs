using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.AccountsPayable
{
    /// <summary>
    /// classificação da duplicata (só vem com expand=classifications; classificationCode nulo vira vazio, faz parte da chave), tabela totvs.TotvsDuplicatesClassifications
    /// </summary>
    public class DuplicateClassification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int supplierCode { get; private set; }
        public long duplicateCode { get; private set; }
        public int installmentCode { get; private set; }
        public int classificationType { get; private set; }
        public string classificationCode { get; private set; } = String.Empty;
        public string classificationTypeDescription { get; private set; } = String.Empty;
        public string classificationDescription { get; private set; } = String.Empty;

        public DuplicateClassification() { }

        public DuplicateClassification(string doc_company, DateTime integrated_or_updated_at, int branchCode, int supplierCode, long duplicateCode, int installmentCode, Dtos.Reads.AccountsPayable.DuplicateClassification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.supplierCode = supplierCode;
            this.duplicateCode = duplicateCode;
            this.installmentCode = installmentCode;

            classificationType = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.classificationType);
            classificationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.classificationCode, 10);
            classificationTypeDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.classificationTypeDescription, 60);
            classificationDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.classificationDescription, 60);
        }
    }
}
