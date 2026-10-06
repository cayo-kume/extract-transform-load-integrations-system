using Domain.Core.Extensions;

namespace Domain.Stone.Entities
{
    public class Address
    {
        public Guid pickupPointId { get; set; }
        public string customerDocument { get; set; }
        public bool partialMatch { get; set; }
        public string referenceKey { get; set; }
        public string chave { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string zipCode { get; set; }
        public decimal latitude { get; set; }
        public Guid accountId { get; set; }
        public decimal longitude { get; set; }
        public string complement { get; set; }
        public string countryState { get; set; }
        public string locationType { get; set; }
        public string neighborhood { get; set; }
        public string addressNumber { get; set; }
        public Guid orderId { get; set; }

        public Address() { }

        public Address(Dtos.Deliveryaddress deliveryAddress, string customerDocument, string orderId)
        {
            this.chave = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.key, 800);
            this.city = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.city, 40);
            this.address = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.address, 250);
            this.country = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.country, 60);
            this.zipCode = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.zipCode, 9);
            this.latitude = ConverterValidatorsExtensions.ConvertToDecimalValidation(deliveryAddress.latitude);
            this.accountId = ConverterValidatorsExtensions.ConvertToGuidValidation(deliveryAddress.accountId);
            this.longitude = ConverterValidatorsExtensions.ConvertToDecimalValidation(deliveryAddress.longitude);
            this.complement = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.reference + deliveryAddress.complement, 300);
            this.countryState = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.countryState, 2);
            this.locationType = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.locationType, 60);
            this.neighborhood = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.neighborhood, 60);
            this.addressNumber = ConverterValidatorsExtensions.StringLengthValidation(deliveryAddress.addressNumber, 20);
            this.customerDocument = ConverterValidatorsExtensions.StringLengthValidation(customerDocument, 14);
            this.orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(orderId);
        }

        public Address(Dtos.Pickupaddress pickupAddress, string orderId)
        {
            this.pickupPointId = ConverterValidatorsExtensions.ConvertToGuidValidation(pickupAddress.id);
            this.chave = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.key, 800);
            this.city = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.city, 40);
            this.address = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.address, 250);
            this.country = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.country, 60);
            this.zipCode = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.zipCode, 9);
            this.latitude = ConverterValidatorsExtensions.ConvertToDecimalValidation(pickupAddress.latitude);
            this.accountId = ConverterValidatorsExtensions.ConvertToGuidValidation(pickupAddress.accountId);
            this.longitude = ConverterValidatorsExtensions.ConvertToDecimalValidation(pickupAddress.longitude);
            this.complement = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.complement, 300);
            this.countryState = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.countryState, 2);
            this.locationType = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.locationType, 60);
            this.customerDocument = "0";
            this.neighborhood = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.neighborhood, 60);
            this.partialMatch = ConverterValidatorsExtensions.ConvertToBooleanValidation(pickupAddress.partialMatch);
            this.referenceKey = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.referenceKey, 300);
            this.addressNumber = ConverterValidatorsExtensions.StringLengthValidation(pickupAddress.addressNumber, 20);
            this.orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(orderId);
        }
    }
}
