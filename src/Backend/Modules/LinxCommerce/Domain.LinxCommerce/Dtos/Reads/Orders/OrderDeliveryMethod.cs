namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderDeliveryMethod do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderDeliveryMethod
    {
        public string? OrderDeliveryMethodID { get; init; }
        public string? LogisticOptionId { get; init; }
        public string? LogisticOptionName { get; init; }
        public string? LogisticContractId { get; init; }
        public string? LogisticContractName { get; init; }
        public string? DeliveryMethodID { get; init; }
        public string? DeliveryGroupID { get; init; }
        public string? Amount { get; init; }
        public string? ETA { get; init; }
        public string? ETADays { get; init; }
        public string? IntegrationID { get; init; }
        public string? ScheduleShiftID { get; init; }
        public string? ScheduleDisplayName { get; init; }
        public string? ScheduleTax { get; init; }
        public string? ScheduleDate { get; init; }
        public string? DeliveryMethodAlias { get; init; }
        public string? PointOfSaleID { get; init; }
        public string? PointOfSaleIntegrationID { get; init; }
        public string? PointOfSaleName { get; init; }
        public string? DeliveryMethodType { get; init; }
        public string? DeliveryLogisticType { get; init; }
        public string? ExternalID { get; init; }
        public string? WarehouseID { get; init; }
        public string? WarehouseIntegrationID { get; init; }
        public string? DockID { get; init; }
        public string? CarrierName { get; init; }
        public string? DeliveryEstimatedDate { get; init; }
    }
}
