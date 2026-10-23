using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// contato da pessoa, tabelas totvs.TotvsPeopleContacts (física) e totvs.TotvsLegalEntitiesContacts (jurídica)
    /// </summary>
    public class Contact
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string name { get; private set; } = String.Empty;
        public int typeCode { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public string function { get; private set; } = String.Empty;
        public string phoneNumber { get; private set; } = String.Empty;
        public string cellNumber { get; private set; } = String.Empty;
        public string email { get; private set; } = String.Empty;
        public DateTime? birthDate { get; private set; }
        public bool? isDefault { get; private set; }

        public Contact() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Contact(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Contact dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 20);
            function = ConverterValidatorsExtensions.StringLengthValidation(dto.function, 20);
            phoneNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.phoneNumber, 20);
            cellNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.cellNumber, 20);
            email = ConverterValidatorsExtensions.StringLengthValidation(dto.email, 60);
            birthDate = NullableConverters.ToNullableDateTime(dto.birthDate);
            isDefault = NullableConverters.ToNullableBoolean(dto.isDefault);
        }
    }
}
