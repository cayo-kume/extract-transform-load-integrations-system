using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// tipo de campo adicional de produto (GET product/v2/additional-fields-types; o type vem como número, 1 a 3, e não pelo nome do enum, testado), tabela totvs.TotvsAdditionalFieldsTypes
    /// </summary>
    public class AdditionalFieldType
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string title { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;
        public int? size { get; private set; }
        public int? @decimal { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public AdditionalFieldType() { }

        public AdditionalFieldType(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.AdditionalFieldType dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            title = ConverterValidatorsExtensions.StringLengthValidation(dto.title, 30);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 30);
            size = NullableConverters.ToNullableInt32(dto.size);
            @decimal = NullableConverters.ToNullableInt32(dto.@decimal);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
