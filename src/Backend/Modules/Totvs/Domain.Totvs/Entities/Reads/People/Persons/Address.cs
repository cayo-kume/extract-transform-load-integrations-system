using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// endereço da pessoa, tabelas totvs.TotvsPeopleAddresses (física) e totvs.TotvsLegalEntitiesAddresses (jurídica)
    /// </summary>
    public class Address
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int addressTypeCode { get; private set; }
        public int sequenceCode { get; private set; }
        public string addressType { get; private set; } = String.Empty;
        public string publicPlace { get; private set; } = String.Empty;
        public string address { get; private set; } = String.Empty;
        public int? addressNumber { get; private set; }
        public string complement { get; private set; } = String.Empty;
        public string neighborhood { get; private set; } = String.Empty;
        public int? ibgeCityCode { get; private set; }
        public string cityName { get; private set; } = String.Empty;
        public string stateAbbreviation { get; private set; } = String.Empty;
        public string cep { get; private set; } = String.Empty;
        public int? bcbCountryCode { get; private set; }
        public string countryName { get; private set; } = String.Empty;
        public int? postOfficeBox { get; private set; }
        public string reference { get; private set; } = String.Empty;

        public Address() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Address(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Address dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            addressTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.addressTypeCode);
            sequenceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequenceCode);
            addressType = ConverterValidatorsExtensions.StringLengthValidation(dto.addressType, 20);
            publicPlace = ConverterValidatorsExtensions.StringLengthValidation(dto.publicPlace, 10);
            address = ConverterValidatorsExtensions.StringLengthValidation(dto.address, 60);
            addressNumber = NullableConverters.ToNullableInt32(dto.addressNumber);
            complement = ConverterValidatorsExtensions.StringLengthValidation(dto.complement, 65);
            neighborhood = ConverterValidatorsExtensions.StringLengthValidation(dto.neighborhood, 60);
            ibgeCityCode = NullableConverters.ToNullableInt32(dto.ibgeCityCode);
            cityName = ConverterValidatorsExtensions.StringLengthValidation(dto.cityName, 60);
            stateAbbreviation = ConverterValidatorsExtensions.StringLengthValidation(dto.stateAbbreviation, 2);
            cep = ConverterValidatorsExtensions.StringLengthValidation(dto.cep, 8);
            bcbCountryCode = NullableConverters.ToNullableInt32(dto.bcbCountryCode);
            countryName = ConverterValidatorsExtensions.StringLengthValidation(dto.countryName, 60);
            postOfficeBox = NullableConverters.ToNullableInt32(dto.postOfficeBox);
            reference = ConverterValidatorsExtensions.StringLengthValidation(dto.reference, 60);
        }
    }
}
