using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Catalogs
{
    /// <summary>
    /// classificação de pessoa e o seu tipo (GET person/v2/classifications, ClassificationTypeRelationshipModel), tabela totvs.TotvsPersonClassifications
    /// </summary>
    public class PersonClassification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int typeCode { get; private set; }
        public string typeDescription { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        public PersonClassification() { }

        public PersonClassification(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Catalogs.PersonClassification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.typeDescription, 40);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
