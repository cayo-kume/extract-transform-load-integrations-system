namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderPackage
    {
        public DateTime? lastupdateon { get; set; }
        public Guid? OrderPackageID { get; set; }
        public Guid? OrderShipmentID { get; set; }
        public Int32? DeliveryMethodID { get; set; }
        public string? PackageNumber { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingNumberUrl { get; set; }
        public DateTimeOffset? ShippedDate { get; set; }
        public string? ShippedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public string? PackageType { get; set; }
        public string? Source { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Weight { get; set; }

        public OrderPackage() { }

        public OrderPackage(OrderPackage orderPackage)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderPackageID = orderPackage.OrderPackageID;
            this.OrderShipmentID = orderPackage.OrderShipmentID;
            this.DeliveryMethodID = orderPackage.DeliveryMethodID;
            this.PackageNumber = orderPackage.PackageNumber;
            this.TrackingNumber = orderPackage.TrackingNumber;
            this.TrackingNumberUrl = orderPackage.TrackingNumberUrl;
            this.ShippedDate = orderPackage.ShippedDate;
            this.ShippedBy = orderPackage.ShippedBy;
            this.IsDeleted = orderPackage.IsDeleted;
            this.PackageType = orderPackage.PackageType;
            this.Source = orderPackage.Source;
            this.InsuranceAmount = orderPackage.InsuranceAmount;
            this.Height = orderPackage.Height;
            this.Width = orderPackage.Width;
            this.Length = orderPackage.Length;
            this.Weight = orderPackage.Weight;
        }
    }
}
