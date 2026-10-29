using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// propriedade do pedido, tabela linxcommerce.LinxCommerceOrdersProperties
    /// </summary>
    public class OrderProperty
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public long OrderPropertyID { get; private set; }
        public decimal? Amount { get; private set; }
        public string Message { get; private set; } = String.Empty;
        public string Reference { get; private set; } = String.Empty;
        public string Source { get; private set; } = String.Empty;
        public string Type { get; private set; } = String.Empty;
        public string XmlExtended { get; private set; } = String.Empty;

        public OrderProperty() { }

        public OrderProperty(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderProperty dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderPropertyID = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.OrderPropertyID);
            Amount = NullableConverters.ToNullableDecimal(dto.Amount);
            Message = ConverterValidatorsExtensions.StringLengthValidation(dto.Message, 400);
            Reference = ConverterValidatorsExtensions.StringLengthValidation(dto.Reference, 250);
            Source = ConverterValidatorsExtensions.StringLengthValidation(dto.Source, 100);
            Type = ConverterValidatorsExtensions.StringLengthValidation(dto.Type, 100);
            XmlExtended = dto.XmlExtended ?? String.Empty;
        }
    }
}
