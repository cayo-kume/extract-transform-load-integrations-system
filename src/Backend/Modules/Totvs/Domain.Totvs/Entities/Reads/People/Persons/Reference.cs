using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// referência comercial/pessoal da pessoa, tabelas totvs.TotvsPeopleReferences (física) e totvs.TotvsLegalEntitiesReferences (jurídica)
    /// </summary>
    public class Reference
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string type { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public string phoneNumber { get; private set; } = String.Empty;
        public string responsiblePersonName { get; private set; } = String.Empty;
        public bool? isInactive { get; private set; }

        public Reference() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Reference(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Reference dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 20);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 20);
            phoneNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.phoneNumber, 20);
            responsiblePersonName = ConverterValidatorsExtensions.StringLengthValidation(dto.responsiblePersonName, 60);
            isInactive = NullableConverters.ToNullableBoolean(dto.isInactive);
        }
    }
}
