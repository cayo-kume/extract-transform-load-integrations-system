using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// rede social da pessoa, tabelas totvs.TotvsPeopleSocialNetworks (física) e totvs.TotvsLegalEntitiesSocialNetworks (jurídica)
    /// </summary>
    public class SocialNetwork
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public string address { get; private set; } = String.Empty;

        public SocialNetwork() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public SocialNetwork(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.SocialNetwork dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 20);
            address = ConverterValidatorsExtensions.StringLengthValidation(dto.address, 20);
        }
    }
}
