using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// observação (sequence anulável na API: nulo vira a posição) da pessoa, tabelas totvs.TotvsPeopleObservations (física) e totvs.TotvsLegalEntitiesObservations (jurídica)
    /// </summary>
    public class Observation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; } = String.Empty;
        public bool? isMaintenance { get; private set; }

        public Observation() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Observation(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Observation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = NullableConverters.ToNullableInt32(dto.sequence) ?? position;
            observation = ConverterValidatorsExtensions.StringLengthValidation(dto.observation, 80);
            isMaintenance = NullableConverters.ToNullableBoolean(dto.isMaintenance);
        }
    }
}
