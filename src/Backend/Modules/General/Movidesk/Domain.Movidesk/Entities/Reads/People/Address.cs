using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Address
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public int id { get; private set; }
        public string addressType { get; private set; }
        public string country { get; private set; }
        public int? countryId { get; private set; }
        public string postalCode { get; private set; }
        public string state { get; private set; }
        public string city { get; private set; }
        public string district { get; private set; }
        public string street { get; private set; }
        public string number { get; private set; }
        public string complement { get; private set; }
        public string reference { get; private set; }
        public bool isDefault { get; private set; }

        public Address() { }

        public Address(string doc_company, DateTime integrated_or_updated_at, string personId, int id, Dtos.Reads.People.Address dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.id = id;

            addressType = ConverterValidatorsExtensions.StringLengthValidation(dto.addressType, 128);
            country = ConverterValidatorsExtensions.StringLengthValidation(dto.country, 128);
            countryId = NullableConverters.ToNullableInt32(dto.countryId);   // não documentado, vem na resposta real
            postalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.postalCode, 32);
            state = ConverterValidatorsExtensions.StringLengthValidation(dto.state, 128);
            city = ConverterValidatorsExtensions.StringLengthValidation(dto.city, 128);
            district = ConverterValidatorsExtensions.StringLengthValidation(dto.district, 128);
            street = ConverterValidatorsExtensions.StringLengthValidation(dto.street, 128);
            number = ConverterValidatorsExtensions.StringLengthValidation(dto.number, 32);
            complement = ConverterValidatorsExtensions.StringLengthValidation(dto.complement, 128);
            reference = ConverterValidatorsExtensions.StringLengthValidation(dto.reference, 128);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDefault);
        }
    }
}
