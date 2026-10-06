using Domain.Core.Extensions;

namespace Domain.Stone.Entities
{
    public class Sender
    {
        public string name { get; set; }
        public string email { get; set; }
        public string document { get; set; } = "0";
        public string phoneNumber { get; set; }
        public Guid logisticAccountId { get; set; }

        public Sender() { }

        public Sender(Dtos.Sender sender)
        {
            this.name = ConverterValidatorsExtensions.StringLengthValidation(sender.name, 60);
            this.email = ConverterValidatorsExtensions.StringLengthValidation(sender.email, 50);
            this.document = ConverterValidatorsExtensions.StringLengthValidation(sender.document, 14);
            this.phoneNumber = ConverterValidatorsExtensions.StringLengthValidation(sender.phoneNumber, 20);
            this.logisticAccountId = ConverterValidatorsExtensions.ConvertToGuidValidation(sender.logisticAccountId);
        }
    }
}
