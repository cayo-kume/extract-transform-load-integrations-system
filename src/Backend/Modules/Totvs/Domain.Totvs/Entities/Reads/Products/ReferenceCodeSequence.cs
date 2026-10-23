using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// sequência de código de referência do produto (code se repete: a chave é referenceId + parentCode), tabela totvs.TotvsProductsReferenceCodeSequences
    /// </summary>
    public class ReferenceCodeSequence
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int referenceId { get; private set; }
        public int parentCode { get; private set; }
        public string code { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;

        public ReferenceCodeSequence() { }

        public ReferenceCodeSequence(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.ReferenceCodeSequence dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            referenceId = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.referenceId);
            parentCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.parentCode);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 60);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 250);
        }
    }
}
