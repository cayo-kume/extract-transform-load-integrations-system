using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// forma de entrega do pedido, tabela linxcommerce.LinxCommerceOrdersDeliveryMethods
    /// </summary>
    public class OrderDeliveryMethod
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public int OrderDeliveryMethodID { get; private set; }
        public int? DeliveryMethodID { get; private set; }
        public int? DeliveryGroupID { get; private set; }
        public string DeliveryMethodAlias { get; private set; } = String.Empty;
        public string DeliveryMethodType { get; private set; } = String.Empty;
        public string DeliveryLogisticType { get; private set; } = String.Empty;
        public string LogisticOptionId { get; private set; } = String.Empty;
        public string LogisticOptionName { get; private set; } = String.Empty;
        public string LogisticContractId { get; private set; } = String.Empty;
        public string LogisticContractName { get; private set; } = String.Empty;
        public decimal? Amount { get; private set; }
        public string ETA { get; private set; } = String.Empty;
        public int? ETADays { get; private set; }
        public long? IntegrationID { get; private set; }
        public long? ScheduleShiftID { get; private set; }
        public string ScheduleDisplayName { get; private set; } = String.Empty;
        public decimal? ScheduleTax { get; private set; }
        public DateTimeOffset? ScheduleDate { get; private set; }
        public int? PointOfSaleID { get; private set; }
        public string PointOfSaleIntegrationID { get; private set; } = String.Empty;
        public string PointOfSaleName { get; private set; } = String.Empty;
        public string ExternalID { get; private set; } = String.Empty;
        public int? WarehouseID { get; private set; }
        public string WarehouseIntegrationID { get; private set; } = String.Empty;
        public int? DockID { get; private set; }
        public string CarrierName { get; private set; } = String.Empty;
        public DateTimeOffset? DeliveryEstimatedDate { get; private set; }

        public OrderDeliveryMethod() { }

        public OrderDeliveryMethod(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderDeliveryMethod dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderDeliveryMethodID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.OrderDeliveryMethodID);
            DeliveryMethodID = NullableConverters.ToNullableInt32(dto.DeliveryMethodID);
            DeliveryGroupID = NullableConverters.ToNullableInt32(dto.DeliveryGroupID);
            DeliveryMethodAlias = ConverterValidatorsExtensions.StringLengthValidation(dto.DeliveryMethodAlias, 100);
            DeliveryMethodType = ConverterValidatorsExtensions.StringLengthValidation(dto.DeliveryMethodType, 50);
            DeliveryLogisticType = ConverterValidatorsExtensions.StringLengthValidation(dto.DeliveryLogisticType, 50);
            LogisticOptionId = ConverterValidatorsExtensions.StringLengthValidation(dto.LogisticOptionId, 100);
            LogisticOptionName = ConverterValidatorsExtensions.StringLengthValidation(dto.LogisticOptionName, 250);
            LogisticContractId = ConverterValidatorsExtensions.StringLengthValidation(dto.LogisticContractId, 100);
            LogisticContractName = ConverterValidatorsExtensions.StringLengthValidation(dto.LogisticContractName, 250);
            Amount = NullableConverters.ToNullableDecimal(dto.Amount);
            ETA = ConverterValidatorsExtensions.StringLengthValidation(dto.ETA, 50);
            ETADays = NullableConverters.ToNullableInt32(dto.ETADays);
            IntegrationID = LinxCommerceConverters.ToNullableInt64(dto.IntegrationID);
            ScheduleShiftID = LinxCommerceConverters.ToNullableInt64(dto.ScheduleShiftID);
            ScheduleDisplayName = ConverterValidatorsExtensions.StringLengthValidation(dto.ScheduleDisplayName, 250);
            ScheduleTax = NullableConverters.ToNullableDecimal(dto.ScheduleTax);
            ScheduleDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ScheduleDate);
            PointOfSaleID = NullableConverters.ToNullableInt32(dto.PointOfSaleID);
            PointOfSaleIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.PointOfSaleIntegrationID, 100);
            PointOfSaleName = ConverterValidatorsExtensions.StringLengthValidation(dto.PointOfSaleName, 250);
            ExternalID = ConverterValidatorsExtensions.StringLengthValidation(dto.ExternalID, 100);
            WarehouseID = NullableConverters.ToNullableInt32(dto.WarehouseID);
            WarehouseIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.WarehouseIntegrationID, 100);
            DockID = NullableConverters.ToNullableInt32(dto.DockID);
            CarrierName = ConverterValidatorsExtensions.StringLengthValidation(dto.CarrierName, 250);
            DeliveryEstimatedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.DeliveryEstimatedDate);
        }
    }
}
