using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// telefone ("Sequence" com S maiúsculo na API) da pessoa, tabelas totvs.TotvsPeoplePhones (física) e totvs.TotvsLegalEntitiesPhones (jurídica)
    /// </summary>
    public class Phone
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public string number { get; private set; } = String.Empty;
        public int? branchLine { get; private set; }
        public bool? isDefault { get; private set; }

        public Phone() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Phone(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Phone dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.Sequence);
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 20);
            number = ConverterValidatorsExtensions.StringLengthValidation(dto.number, 20);
            branchLine = NullableConverters.ToNullableInt32(dto.branchLine);
            isDefault = NullableConverters.ToNullableBoolean(dto.isDefault);
        }
    }
}
