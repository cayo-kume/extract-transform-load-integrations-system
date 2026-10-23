using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// familiar (sem chave na API: a posição é a chave) da pessoa, tabelas totvs.TotvsPeopleFamiliars (física) e totvs.TotvsLegalEntitiesFamiliars (jurídica)
    /// </summary>
    public class Familiar
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string name { get; private set; } = String.Empty;
        public DateTime? birthDate { get; private set; }
        public string gender { get; private set; } = String.Empty;
        public string kinshipDescription { get; private set; } = String.Empty;

        public Familiar() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Familiar(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Familiar dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = position;
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            birthDate = NullableConverters.ToNullableDateTime(dto.birthDate);
            gender = ConverterValidatorsExtensions.StringLengthValidation(dto.gender, 20);
            kinshipDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.kinshipDescription, 60);
        }
    }
}
