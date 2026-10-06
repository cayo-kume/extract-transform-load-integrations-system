using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Email
    {
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string email { get; private set; }
        public bool isDefault { get; private set; }

        public Email() { }

        public Email(Domain.Totvs.Dtos.Person.Customer.Email emailDto, int personCode)
        {
            this.personCode = personCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(emailDto.sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(emailDto.typeCode);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(emailDto.isDefault);
            
            typeName = emailDto.typeName?.Length > 60 ? emailDto.typeName.Substring(0, 60) : emailDto.typeName;
            email = emailDto.typeName?.Length > 60 ? emailDto.email.Substring(0, 60) : emailDto.typeName;
        }
    }
}