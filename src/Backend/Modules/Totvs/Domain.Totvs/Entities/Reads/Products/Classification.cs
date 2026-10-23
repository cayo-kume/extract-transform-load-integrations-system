using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// classificação do produto (um código por tipo), tabela totvs.TotvsProductsClassifications
    /// </summary>
    public class Classification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public string typeNameAux { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string nameAux { get; private set; } = String.Empty;

        public Classification() { }

        public Classification(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.Classification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 60);
            typeNameAux = ConverterValidatorsExtensions.StringLengthValidation(dto.typeNameAux, 60);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 60);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            nameAux = ConverterValidatorsExtensions.StringLengthValidation(dto.nameAux, 60);
        }
    }
}
