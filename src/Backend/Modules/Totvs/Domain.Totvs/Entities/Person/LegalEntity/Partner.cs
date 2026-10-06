using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Partner
    {
        public int legalEntityCode { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; }
        public string name { get; private set; }
        public int percentageParticipation { get; private set; }

        public Partner() { }

        public Partner(Domain.Totvs.Dtos.Person.LegalEntity.Partner partnerDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(partnerDto.code);
            percentageParticipation = ConverterValidatorsExtensions.ConvertToInt32Validation(partnerDto.percentageParticipation);
            
            cpfCnpj = partnerDto.cpfCnpj?.Length > 14 ? partnerDto.cpfCnpj.Substring(0, 14) : partnerDto.cpfCnpj;
            name = partnerDto.name?.Length > 250 ? partnerDto.name.Substring(0, 250) : partnerDto.name;
        }
    }
}
