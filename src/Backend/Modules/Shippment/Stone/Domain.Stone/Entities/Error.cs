using Domain.Core.Extensions;

namespace Domain.Stone.Entities
{
    public class Error
    {
        public string orderNumber { get; set; }
        public string message { get; set; }
        public string error { get; set; }

        public Error() { }

        public Error(string orderNumber, string message, string error)
        {
            this.orderNumber = orderNumber;
            this.message = ConverterValidatorsExtensions.StringLengthValidation(message, 300);
            this.error = ConverterValidatorsExtensions.StringLengthValidation(error, 300);
        }
    }
}
