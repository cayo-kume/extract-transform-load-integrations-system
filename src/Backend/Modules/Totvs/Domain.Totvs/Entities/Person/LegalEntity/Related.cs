using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Related
    {
        public int legalEntityCode { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; }
        public string name { get; private set; }

        public Related() { }

        public Related(Domain.Totvs.Dtos.Person.LegalEntity.Related relatedDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(relatedDto.code);
            
            cpfCnpj = relatedDto.cpfCnpj?.Length > 14 ? relatedDto.cpfCnpj.Substring(0, 14) : relatedDto.cpfCnpj;
            name = relatedDto.name?.Length > 250 ? relatedDto.name.Substring(0, 250) : relatedDto.name;
        }
    }
}
