using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Person
{
    public class Email
    {
        public string personId { get; private set; }
        public string emailType { get; private set; }
        public string email { get; private set; }
        public bool isDefault { get; private set; }

        public Email() { }

        public Email(string personId, Dtos.Person.Email emailDto)
        {
            this.personId = personId;

            emailType = emailDto.emailType;
            email = emailDto.email;
            
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(emailDto.isDefault);
        }
    }
}
