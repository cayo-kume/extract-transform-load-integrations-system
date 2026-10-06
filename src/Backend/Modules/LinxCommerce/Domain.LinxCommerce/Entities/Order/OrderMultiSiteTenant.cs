namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderMultiSiteTenant
    {
        public string? BrandId { get; set; }
        public string? BrandType { get; set; }
        public string? CompanyId { get; set; }
        public string? DeviceType { get; set; }

        public OrderMultiSiteTenant() { }

        public OrderMultiSiteTenant(OrderMultiSiteTenant orderMultiSiteTenant)
        {
            BrandId = orderMultiSiteTenant.BrandId;
            BrandType = orderMultiSiteTenant.BrandType;
            CompanyId = orderMultiSiteTenant.CompanyId;
            DeviceType = orderMultiSiteTenant.DeviceType;
        }
    }
}
