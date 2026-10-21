using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Contact
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public int id { get; private set; }
        public string contactType { get; private set; }
        public string contact { get; private set; }
        public bool isDefault { get; private set; }

        public Contact() { }

        public Contact(string doc_company, DateTime integrated_or_updated_at, string personId, int id, Dtos.Reads.People.Contact dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.id = id;

            contactType = ConverterValidatorsExtensions.StringLengthValidation(dto.contactType, 128);
            contact = ConverterValidatorsExtensions.StringLengthValidation(dto.contact, 128);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDefault);
        }
    }
}
