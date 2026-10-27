using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// tipo de classificação de produto (GET product/v2/classificationType, ClassificationTypeResultModel), tabela totvs.TotvsClassificationTypes
    /// </summary>
    public class ClassificationType
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string auxiliaryDescription { get; private set; } = String.Empty;
        public bool? isGroup { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public ClassificationType() { }

        public ClassificationType(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.ClassificationType dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            auxiliaryDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryDescription, 100);
            isGroup = NullableConverters.ToNullableBoolean(dto.isGroup);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
