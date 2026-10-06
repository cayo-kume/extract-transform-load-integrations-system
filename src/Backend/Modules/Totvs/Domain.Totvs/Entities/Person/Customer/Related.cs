using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Related
    {
        public int personCode { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; }
        public string name { get; private set; }

        public Related() { }

        public Related(Domain.Totvs.Dtos.Person.Customer.Related relatedDto, int personCode)
        {
            this.personCode = personCode;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(relatedDto.code);

            cpfCnpj = relatedDto.cpfCnpj?.Length > 14 ? relatedDto.cpfCnpj.Substring(0, 14) : relatedDto.cpfCnpj;
            name = relatedDto.name?.Length > 250 ? relatedDto.name.Substring(0, 250) : relatedDto.name;
        }
    }
}
