namespace Domain.LinxCommerce.Entities.Sku
{
    public class ParentRelation
    {
        public DateTime lastupdateon { get; set; }
        public Int32? ParentID { get; set; }
        public string? ParentSKU { get; set; }
        public string? ParentIntegrationID { get; set; }
        public Int32? ProductID { get; set; }

        public ParentRelation() { }

        public ParentRelation(ParentRelation parentRelation, Int32? productID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.ParentID = parentRelation.ParentID;
            this.ParentSKU = parentRelation.ParentSKU;
            this.ParentIntegrationID = parentRelation.ParentIntegrationID;
            this.ProductID = productID;
        }
    }
}
