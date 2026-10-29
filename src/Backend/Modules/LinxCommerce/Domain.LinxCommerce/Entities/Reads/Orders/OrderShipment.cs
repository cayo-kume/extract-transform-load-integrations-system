using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// remessa do pedido, tabela linxcommerce.LinxCommerceOrdersShipments
    /// </summary>
    public class OrderShipment
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public Guid OrderShipmentID { get; private set; }
        public int? DeliveryMethodID { get; private set; }
        public string ShipmentNumber { get; private set; } = String.Empty;
        public string ShipmentStatus { get; private set; } = String.Empty;
        public int? AssignUserId { get; private set; }
        public string AssignUserName { get; private set; } = String.Empty;
        public int? DockID { get; private set; }

        public OrderShipment() { }

        public OrderShipment(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderShipment dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderShipmentID = ConverterValidatorsExtensions.ConvertToGuidValidation(dto.OrderShipmentID);
            DeliveryMethodID = NullableConverters.ToNullableInt32(dto.DeliveryMethodID);
            ShipmentNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.ShipmentNumber, 50);
            ShipmentStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.ShipmentStatus, 50);
            AssignUserId = NullableConverters.ToNullableInt32(dto.AssignUserId);
            AssignUserName = ConverterValidatorsExtensions.StringLengthValidation(dto.AssignUserName, 250);
            DockID = NullableConverters.ToNullableInt32(dto.DockID);
        }
    }
}
