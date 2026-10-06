using Domain.Core.Extensions;

namespace Domain.Stone.Entities
{
    public class Item
    {
        public Guid orderId { get; set; }
        public string code { get; set; }
        public decimal depth { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public decimal quantity { get; set; }
        public decimal unitPrice { get; set; }
        public string invoiceKey { get; set; }
        public string description { get; set; }
        public string invoiceNumber { get; set; }
        public decimal invoiceTotalValue { get; set; }

        public Item() { }

        public Item(Dtos.Item item, string orderId)
        {
            this.orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(orderId);
            this.code = ConverterValidatorsExtensions.StringLengthValidation(item.code, 50);
            this.depth = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.depth);
            this.width = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.width);
            this.height = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.height);
            this.weight = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.weight);
            this.quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.quantity);
            this.unitPrice = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.unitPrice);
            this.invoiceKey = ConverterValidatorsExtensions.StringLengthValidation(item.invoiceKey, 44);
            this.description = ConverterValidatorsExtensions.StringLengthValidation(item.description, 255);
            this.invoiceNumber = ConverterValidatorsExtensions.StringLengthValidation(item.invoiceNumber, 50);
            this.invoiceTotalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(item.invoiceTotalValue);
        }
    }
}
