using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Email
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public int id { get; private set; }
        public string emailType { get; private set; }
        public string email { get; private set; }
        public bool isDefault { get; private set; }

        public Email() { }

        public Email(string doc_company, DateTime integrated_or_updated_at, string personId, int id, Dtos.Reads.People.Email dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.id = id;

            emailType = ConverterValidatorsExtensions.StringLengthValidation(dto.emailType, 128);
            email = ConverterValidatorsExtensions.StringLengthValidation(dto.email, 128);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDefault);
        }
    }
}
