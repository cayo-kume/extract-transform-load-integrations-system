using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Classification
    {
        public int personCode { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string code { get; private set; }
        public string name { get; private set; }

        public Classification() { }

        public Classification(Domain.Totvs.Dtos.Person.Customer.Classification classificationDto, int personCode)
        {
            this.personCode = personCode;
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(classificationDto.typeCode);
            
            typeName = classificationDto.typeName?.Length > 60 ? classificationDto.typeName.Substring(0, 60) : classificationDto.typeName;
            code =  classificationDto.code?.Length > 60 ? classificationDto.code.Substring(0, 60) : classificationDto.code;
            name =  classificationDto.name?.Length > 60 ? classificationDto.name.Substring(0, 60) : classificationDto.name;
        }
    }
}
