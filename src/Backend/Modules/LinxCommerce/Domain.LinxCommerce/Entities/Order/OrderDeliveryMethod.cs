namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderDeliveryMethod
    {
        public DateTime lastupdateon { get; set; }
        public string? LogisticOptionId { get; set; }
        public string? LogisticOptionName { get; set; }
        public string? LogisticContractId { get; set; }
        public string? LogisticContractName { get; set; }
        public Int32? OrderDeliveryMethodID { get; set; }
        public Guid OrderID { get; set; }
        public Int32? DeliveryMethodID { get; set; }
        public Int32? DeliveryGroupID { get; set; }
        public decimal? Amount { get; set; }
        public string? ETA { get; set; }
        public Int32? ETADays { get; set; }
        public Int64? IntegrationID { get; set; }
        public Int64? ScheduleShiftID { get; set; }
        public string? ScheduleDisplayName { get; set; }
        public decimal? ScheduleTax { get; set; }
        //public DateTime? ScheduleStartTime { get; set; }
        //public DateTime? ScheduleEndTime { get; set; }
        public DateTimeOffset? ScheduleDate { get; set; }
        public string? DeliveryMethodAlias { get; set; }
        public Int32? PointOfSaleID { get; set; }
        public string? PointOfSaleIntegrationID { get; set; }
        public string? PointOfSaleName { get; set; }
        public string? DeliveryMethodType { get; set; }
        public string? DeliveryLogisticType { get; set; }
        public string? ExternalID { get; set; }
        public Int32? WarehouseID { get; set; }
        public string? WarehouseIntegrationID { get; set; }
        public Int32? DockID { get; set; }
        public string? CarrierName { get; set; }
        public DateTimeOffset? DeliveryEstimatedDate { get; set; }

        public OrderDeliveryMethod() { }

        public OrderDeliveryMethod(OrderDeliveryMethod orderDeliveryMethod)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.LogisticOptionId = orderDeliveryMethod.LogisticOptionId;
            this.LogisticOptionName = orderDeliveryMethod.LogisticOptionName;
            this.LogisticContractId = orderDeliveryMethod.LogisticContractId;
            this.LogisticContractName = orderDeliveryMethod.LogisticContractName;
            this.OrderDeliveryMethodID = orderDeliveryMethod.OrderDeliveryMethodID;
            this.OrderID = orderDeliveryMethod.OrderID;
            this.DeliveryMethodID = orderDeliveryMethod.DeliveryMethodID;
            this.DeliveryGroupID = orderDeliveryMethod.DeliveryGroupID;
            this.Amount = orderDeliveryMethod.Amount;
            this.ETA = orderDeliveryMethod.ETA;
            this.ETADays = orderDeliveryMethod.ETADays;
            this.IntegrationID = orderDeliveryMethod.IntegrationID;
            this.ScheduleShiftID = orderDeliveryMethod.ScheduleShiftID;
            this.ScheduleDisplayName = orderDeliveryMethod.ScheduleDisplayName;
            this.ScheduleTax = orderDeliveryMethod.ScheduleTax;
            this.ScheduleDate = orderDeliveryMethod.ScheduleDate;
            this.DeliveryMethodAlias = orderDeliveryMethod.DeliveryMethodAlias;
            this.PointOfSaleID = orderDeliveryMethod.PointOfSaleID;
            this.PointOfSaleIntegrationID = orderDeliveryMethod.PointOfSaleIntegrationID;
            this.PointOfSaleName = orderDeliveryMethod.PointOfSaleName;
            this.DeliveryMethodType = orderDeliveryMethod.DeliveryMethodType;
            this.DeliveryLogisticType = orderDeliveryMethod.DeliveryLogisticType;
            this.ExternalID = orderDeliveryMethod.ExternalID;
            this.WarehouseID = orderDeliveryMethod.WarehouseID;
            this.WarehouseIntegrationID = orderDeliveryMethod.WarehouseIntegrationID;
            this.DockID = orderDeliveryMethod.DockID;
            this.CarrierName = orderDeliveryMethod.CarrierName;
            this.DeliveryEstimatedDate = orderDeliveryMethod.DeliveryEstimatedDate;
        }
    }
}
