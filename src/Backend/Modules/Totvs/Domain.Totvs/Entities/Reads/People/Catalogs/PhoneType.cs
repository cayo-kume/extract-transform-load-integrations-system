using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Catalogs
{
    /// <summary>
    /// tipo de telefone (GET person/v2/phone-types, PhoneTypeModel), tabela totvs.TotvsPhoneTypes
    /// </summary>
    public class PhoneType
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public bool? isDefault { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public PhoneType() { }

        public PhoneType(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Catalogs.PhoneType dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            isDefault = NullableConverters.ToNullableBoolean(dto.isDefault);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
