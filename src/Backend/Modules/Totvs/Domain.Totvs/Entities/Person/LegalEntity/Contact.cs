using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Contact
    {
        public int legalEntityCode { get; private set; }
        public int sequence { get; private set; }
        public string name { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string function { get; private set; }
        public string phoneNumber { get; private set; }
        public string cellNumber { get; private set; }
        public string email { get; private set; }
        public DateTime birthDate { get; private set; }
        public bool isDefault { get; private set; }

        public Contact() { }

        public Contact(Domain.Totvs.Dtos.Person.LegalEntity.Contact contactDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(contactDto.sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(contactDto.typeCode);
            birthDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(contactDto.birthDate);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(contactDto.isDefault);
            
            name = contactDto.name?.Length > 60 ? contactDto.name.Substring(0, 250) : contactDto.name ;
            typeName = contactDto.typeName?.Length > 60 ? contactDto.typeName.Substring(0, 60) : contactDto.typeName ;
            function = contactDto.function?.Length > 60 ? contactDto.function.Substring(0, 60) : contactDto.function ;
            phoneNumber = contactDto.phoneNumber?.Length > 60 ? contactDto.phoneNumber.Substring(0, 20) : contactDto.phoneNumber ;
            cellNumber = contactDto.cellNumber?.Length > 60 ? contactDto.cellNumber.Substring(0, 20) : contactDto.cellNumber ;
            email = contactDto.email?.Length > 60 ? contactDto.email.Substring(0, 60) : contactDto.email ;
        }
    }
}
