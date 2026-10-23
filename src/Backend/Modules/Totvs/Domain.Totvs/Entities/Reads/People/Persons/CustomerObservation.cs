using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// observação de cliente (só as 10 últimas por padrão; sequence anulável: nulo vira a posição) da pessoa, tabelas totvs.TotvsPeopleCustomerObservations (física) e totvs.TotvsLegalEntitiesCustomerObservations (jurídica)
    /// </summary>
    public class CustomerObservation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; } = String.Empty;
        public bool? isMaintenance { get; private set; }
        public DateTime? lastChangeDate { get; private set; }

        public CustomerObservation() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public CustomerObservation(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.CustomerObservation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = NullableConverters.ToNullableInt32(dto.sequence) ?? position;
            observation = ConverterValidatorsExtensions.StringLengthValidation(dto.observation, 80);
            isMaintenance = NullableConverters.ToNullableBoolean(dto.isMaintenance);
            lastChangeDate = NullableConverters.ToNullableDateTime(dto.LastChangeDate);
        }
    }
}
