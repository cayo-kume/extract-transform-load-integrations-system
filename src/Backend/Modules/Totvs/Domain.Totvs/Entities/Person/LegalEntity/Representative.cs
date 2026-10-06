using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Representative
    {
        public int legalEntityCode { get; private set; }
        public int representativeCode { get; private set; }
        public string cpfCnpj { get; private set; }
        public string classificationCode { get; private set; }
        public int classificationTypeCode { get; private set; }

        public Representative() { }

        public Representative(Domain.Totvs.Dtos.Person.LegalEntity.Representative representativeDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            representativeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(representativeDto.representativeCode);
            classificationTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(representativeDto.classificationTypeCode);

            cpfCnpj = representativeDto.classificationCode?.Length > 14 ? representativeDto.classificationCode.Substring(0, 14) : representativeDto.classificationCode;
            classificationCode = representativeDto.classificationCode?.Length > 60 ? representativeDto.classificationCode.Substring(0, 60) : representativeDto.classificationCode;
        }
    }
}
