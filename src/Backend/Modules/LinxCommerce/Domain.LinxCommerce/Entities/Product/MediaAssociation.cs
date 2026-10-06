namespace Domain.LinxCommerce.Entities.Product
{
    public class MediaAssociation
    {
        public DateTime lastupdateon { get; set; }
        public Int32? ID { get; set; }
        public Int32? ProductID { get; set; }
        public Int32? MediaID { get; set; }
        public string? Path { get; set; }
        public Int32? Order { get; set; }

        public MediaAssociation() { }

        public MediaAssociation(MediaAssociation mediaAssociation)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.ID = mediaAssociation.ID;
            this.ProductID = mediaAssociation.ProductID;
            this.MediaID = mediaAssociation.MediaID;
            this.Path = mediaAssociation.Path;
            this.Order = mediaAssociation.Order;
        }
    }
}
