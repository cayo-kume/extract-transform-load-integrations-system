using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// classificação (code nulo vira vazio, faz parte da chave) da pessoa, tabelas totvs.TotvsPeopleClassifications (física) e totvs.TotvsLegalEntitiesClassifications (jurídica)
    /// </summary>
    public class Classification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int typeCode { get; private set; }
        public string code { get; private set; } = String.Empty;
        public string typeName { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;

        public Classification() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Classification(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Classification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 40);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
        }
    }
}
