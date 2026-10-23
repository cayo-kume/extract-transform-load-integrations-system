using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// campo adicional da pessoa, tabelas totvs.TotvsPeopleAdditionalFields (física) e totvs.TotvsLegalEntitiesAdditionalFields (jurídica)
    /// </summary>
    public class AdditionalField
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int code { get; private set; }
        public string type { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string value { get; private set; } = String.Empty;

        public AdditionalField() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public AdditionalField(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.AdditionalField dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 20);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            value = ConverterValidatorsExtensions.StringLengthValidation(dto.value, 60);
        }
    }
}
