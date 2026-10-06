using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeShippingRegion
    {
        public int? SalesRepresentativeID { get; set; }
        public string? SelectedMode { get; set; }
        public int? ShippingRegionID { get; set; }
        public List<int>? PointOfSalesList { get; set; }

        [SkipProperty]
        public string? PointOfSales { get; set; }

        public SalesRepresentativeShippingRegion() { }

        public SalesRepresentativeShippingRegion(SalesRepresentativeShippingRegion salesRepresentativeShippingRegion, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.SelectedMode = salesRepresentativeShippingRegion.SelectedMode;
            this.ShippingRegionID = salesRepresentativeShippingRegion.ShippingRegionID;
            this.PointOfSales = salesRepresentativeShippingRegion.PointOfSalesList.Count() > 0 ? string.Join(", ", salesRepresentativeShippingRegion.PointOfSalesList) : null;
        }
    }
}
