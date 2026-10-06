using Domain.Shared.Extensions;
using System.Runtime.CompilerServices;

namespace Domain.Totvs.Entities.Person.Branch
{
    public class Address
    {
        public int branchCode { get; private set; }
        public int sequenceCode { get; private set; }
        public int addressTypeCode { get; private set; }
        public string addressType { get; private set; }
        public string publicPlace { get; private set; }
        public string address { get; private set; }
        public string addressNumber { get; private set; }
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

        public Address(Dtos.Person.Branch.Address addressDto, int branchCode)
        {
            this.branchCode = branchCode;

            sequenceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.sequenceCode);
            addressTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.addressTypeCode);
            addressType = addressDto.addressType;
            publicPlace = addressDto.publicPlace;
            address = addressDto.address;
            addressNumber = addressDto.addressNumber;
            complement = addressDto.complement;
            neighborhood = addressDto.neighborhood;
            ibgeCityCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.ibgeCityCode);
            cityName = addressDto.cityName;
            stateAbbreviation = addressDto.stateAbbreviation;
            cep = addressDto.cep;
            bcbCountryCode = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.bcbCountryCode);
            countryName = addressDto.countryName;
            postOfficeBox = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.postOfficeBox);
            reference = addressDto.reference;
        }
    }
}
