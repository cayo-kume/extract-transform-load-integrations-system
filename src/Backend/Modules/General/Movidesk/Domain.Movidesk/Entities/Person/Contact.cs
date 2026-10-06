using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Person
{
    public class Contact
    {
        public string personId { get; private set; }
        public string contactType { get; private set; }
        public string contact { get; private set; }
        public bool isDefault { get; private set; }

        public Contact() { }

        public Contact(string personId, Dtos.Person.Contact contactDto)
        {
            this.personId = personId;

            contactType = contactDto.contactType;
            contact = contactDto.contact;

            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(contactDto.isDefault);
        }
    }
}
