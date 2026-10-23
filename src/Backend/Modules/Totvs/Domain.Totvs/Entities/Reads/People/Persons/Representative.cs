using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// representante da pessoa, tabelas totvs.TotvsPeopleRepresentatives (física) e totvs.TotvsLegalEntitiesRepresentatives (jurídica)
    /// </summary>
    public class Representative
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int representativeCode { get; private set; }
        public string cpfCnpj { get; private set; } = String.Empty;
        public string classificationCode { get; private set; } = String.Empty;
        public int? classificationTypeCode { get; private set; }

        public Representative() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Representative(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Representative dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            representativeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.representativeCode);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
            classificationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.classificationCode, 10);
            classificationTypeCode = NullableConverters.ToNullableInt32(dto.classificationTypeCode);
        }
    }
}
