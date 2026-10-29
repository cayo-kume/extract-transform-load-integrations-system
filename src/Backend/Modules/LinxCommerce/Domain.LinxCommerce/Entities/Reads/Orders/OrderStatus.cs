using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// status de pedido (POST Sales/API.svc/web/SearchOrderStatus), tabela linxcommerce.LinxCommerceOrderStatus
    /// </summary>
    public class OrderStatus
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int OrderStatusID { get; private set; }
        public string Status { get; private set; } = String.Empty;
        public string Color { get; private set; } = String.Empty;
        public bool? IsDefault { get; private set; }
        public string IntegrationID { get; private set; } = String.Empty;

        public OrderStatus() { }

        public OrderStatus(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Orders.OrderStatus dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            OrderStatusID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.OrderStatusID);
            Status = ConverterValidatorsExtensions.StringLengthValidation(dto.Status, 100);
            Color = ConverterValidatorsExtensions.StringLengthValidation(dto.Color, 50);
            IsDefault = NullableConverters.ToNullableBoolean(dto.IsDefault);
            IntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.IntegrationID, 100);
        }
    }
}
