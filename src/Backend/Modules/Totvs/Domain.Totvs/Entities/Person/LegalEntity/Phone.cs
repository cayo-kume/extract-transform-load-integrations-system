using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Phone
    {
        public int legalEntityCode { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string number { get; private set; }
        public int branchLine { get; private set; }
        public bool isDefault { get; private set; }

        public Phone() { }

        public Phone(Domain.Totvs.Dtos.Person.LegalEntity.Phone phoneDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(phoneDto.Sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(phoneDto.typeCode);
            isDefault =ConverterValidatorsExtensions.ConvertToBooleanValidation( phoneDto.isDefault);
            branchLine = ConverterValidatorsExtensions.ConvertToInt32Validation(phoneDto.branchLine);

            typeName = phoneDto.typeName?.Length > 60 ? phoneDto.typeName.Substring(0, 60) : phoneDto.typeName;
            number = phoneDto.number?.Length > 20 ? phoneDto.number.Substring(0, 20) : phoneDto.number;
        }
    }
}
