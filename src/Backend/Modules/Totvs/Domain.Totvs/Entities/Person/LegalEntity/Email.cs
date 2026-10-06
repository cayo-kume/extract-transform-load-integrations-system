using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Email
    {
        public int legalEntityCode { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string email { get; private set; }
        public bool isDefault { get; private set; }

        public Email() { }

        public Email(Domain.Totvs.Dtos.Person.LegalEntity.Email emailDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(emailDto.sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(emailDto.typeCode);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(emailDto.isDefault);

            typeName = emailDto.typeName?.Length > 60 ? emailDto.typeName.Substring(0, 60) : emailDto.typeName;
            email = emailDto.typeName?.Length > 60 ? emailDto.email.Substring(0, 60) : emailDto.typeName;
        }
    }
}