using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Reference
    {
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string type { get; private set; }
        public string description { get; private set; }
        public string phoneNumber { get; private set; }
        public string responsiblePersonName { get; private set; }
        public bool isInactive { get; private set; }

        public Reference() { }

        public Reference(Domain.Totvs.Dtos.Person.Customer.Reference referenceDto, int personCode)
        {
            this.personCode = personCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(referenceDto.sequence);
            isInactive = ConverterValidatorsExtensions.ConvertToBooleanValidation(referenceDto.isInactive);

            type = referenceDto.type?.Length > 60 ? referenceDto.type.Substring(0, 60) : referenceDto.type;
            description = referenceDto.description?.Length > 250 ? referenceDto.description.Substring(0, 250) : referenceDto.description;
            phoneNumber = referenceDto.phoneNumber?.Length > 20 ? referenceDto.phoneNumber.Substring(0, 20) : referenceDto.phoneNumber;
            responsiblePersonName = referenceDto.responsiblePersonName?.Length > 250 ? referenceDto.responsiblePersonName.Substring(0, 250) : referenceDto.responsiblePersonName;
        }
    }
}
