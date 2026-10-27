using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Catalogs
{
    /// <summary>
    /// tipo de contato (GET person/v2/contact-types; o swagger declara o PhoneTypeModel, mas a resposta real traz typeName), tabela totvs.TotvsContactTypes
    /// </summary>
    public class ContactType
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        public ContactType() { }

        public ContactType(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Catalogs.ContactType dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 60);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
