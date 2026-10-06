using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Address
    {
        public int personCode { get; private set; }
        public int sequenceCode { get; private set; }
        public int addressTypeCode { get; private set; }
        public string addressType { get; private set; }
        public string publicPlace { get; private set; }
        public string address { get; private set; }
        public int addressNumber { get; private set; }
        public string complement { get; private set; }
        public string neighborhood { get; private set; }
        public int ibgeCityCode { get; private set; }
        public string cityName { get; private set; }
        public string stateAbbreviation { get; private set; }
        public string cep { get; private set; }
        public int bcbCountryCode { get; private set; }
        public string countryName { get; private set; }
        public int postOfficeBox { get; private set; }
        public string reference { get; private set; }

        public Address() { }

        public Address(Domain.Totvs.Dtos.Person.Customer.Address addressDto, int personCode)
        {
            this.personCode = personCode;
            sequenceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.sequenceCode);
            addressTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.addressTypeCode);
            addressNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.addressNumber);
            ibgeCityCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.ibgeCityCode);
            bcbCountryCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.bcbCountryCode);
            postOfficeBox = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.postOfficeBox);
            
            addressType = addressDto.addressType?.Length > 60 ? addressDto.addressType.Substring(0, 60) : addressDto.addressType;
            publicPlace = addressDto.publicPlace?.Length > 1 ? addressDto.publicPlace.Substring(0, 1) : addressDto.publicPlace;
            address = addressDto.address?.Length > 250 ? addressDto.address.Substring(0, 250) : addressDto.address;
            complement = addressDto.complement?.Length > 60 ? addressDto.complement.Substring(0, 60) : addressDto.complement;
            neighborhood = addressDto.neighborhood?.Length > 60 ? addressDto.neighborhood.Substring(0, 60) : addressDto.neighborhood;
            cityName = addressDto.cityName?.Length > 40 ? addressDto.cityName.Substring(0, 40) : addressDto.cityName;
            stateAbbreviation = addressDto.stateAbbreviation?.Length > 20 ? addressDto.stateAbbreviation.Substring(0, 20) : addressDto.stateAbbreviation;
            cep = addressDto.cep?.Length > 9 ? addressDto.cep.Substring(0, 9) : addressDto.cep;
            countryName = addressDto.countryName?.Length > 80 ? addressDto.countryName.Substring(0, 80) : addressDto.countryName;
            reference = addressDto.reference?.Length > 60 ? addressDto.reference.Substring(0, 60) : addressDto.reference;
        }
    }
}
