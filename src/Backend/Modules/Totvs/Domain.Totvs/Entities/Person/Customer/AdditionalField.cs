using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class AdditionalField
    {
        public int personCode { get; private set; }
        public int code { get; private set; }
        public string type { get; private set; }
        public string name { get; private set; }
        public string value { get; private set; }

        public AdditionalField() { }

        public AdditionalField(Domain.Totvs.Dtos.Person.Customer.Additionalfield additionalfieldDto, int personCode)
        {
            this.personCode = personCode;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(additionalfieldDto.code);

            type = additionalfieldDto.type?.Length > 60 ? additionalfieldDto.type.Substring(0, 60) : additionalfieldDto.type;
            name = additionalfieldDto.name?.Length > 60 ? additionalfieldDto.name.Substring(0, 60) : additionalfieldDto.name;
            value = additionalfieldDto.value?.Length > 250 ? additionalfieldDto.value.Substring(0, 250) : additionalfieldDto.value;
        }
    }
}
