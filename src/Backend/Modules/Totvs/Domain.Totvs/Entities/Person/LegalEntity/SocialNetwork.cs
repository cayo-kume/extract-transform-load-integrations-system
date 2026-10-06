using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class SocialNetwork
    {
        public int legalEntityCode { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string address { get; private set; }

        public SocialNetwork() { }

        public SocialNetwork(Domain.Totvs.Dtos.Person.LegalEntity.Socialnetwork socialnetworkDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(socialnetworkDto.sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(socialnetworkDto.typeCode);
            
            typeName = socialnetworkDto.typeName?.Length > 60 ? socialnetworkDto.typeName.Substring(0, 60) : socialnetworkDto.typeName;
            address = socialnetworkDto.address?.Length > 250 ? socialnetworkDto.address.Substring(0, 250) : socialnetworkDto.address;
        }
    }
}
