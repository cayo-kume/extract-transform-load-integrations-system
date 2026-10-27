using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// classificação de produto e o seu tipo (GET product/v2/classifications, ClassificationDateModel), tabela totvs.TotvsProductClassifications
    /// </summary>
    public class ProductClassification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int typeCode { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public string typeNameAux { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string nameAux { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        public ProductClassification() { }

        public ProductClassification(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.ProductClassification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 40);
            typeNameAux = ConverterValidatorsExtensions.StringLengthValidation(dto.typeNameAux, 100);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            nameAux = ConverterValidatorsExtensions.StringLengthValidation(dto.nameAux, 100);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
