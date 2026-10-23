using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// pessoa relacionada da pessoa, tabelas totvs.TotvsPeopleRelateds (física) e totvs.TotvsLegalEntitiesRelateds (jurídica)
    /// </summary>
    public class Related
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;

        public Related() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Related(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Related dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
        }
    }
}
