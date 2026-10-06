using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Person
{
    public class Address
    {
        public string personId { get; private set; }
        public string addressType { get; private set; }
        public string country { get; private set; }
        public string postalCode { get; private set; }
        public string state { get; private set; }
        public string district { get; private set; }
        public string street { get; private set; }
        public string number { get; private set; }
        public string complement { get; private set; }
        public string reference { get; private set; }
        public bool isDefault { get; private set; }
        public int countryId { get; private set; }

        public Address(string personId, Dtos.Person.Address addressDto)
        {
            this.personId = personId;
            addressType = addressDto.addressType;
            country = addressDto.country;
            postalCode = addressDto.postalCode;
            state = addressDto.state;
            district = addressDto.district;
            street = addressDto.street;
            number = addressDto.number;
            complement = addressDto.complement;
            reference = addressDto.reference;

            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(addressDto.isDefault);

            countryId = ConverterValidatorsExtensions.ConvertToInt32Validation(addressDto.countryId);
        }
    }
}
