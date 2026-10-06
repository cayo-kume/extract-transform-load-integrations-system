namespace Domain.LinxCommerce.Entities.Product
{
    public class MetadataValue
    {
        public DateTime lastupdateon { get; set; }
        public int? PropertyMetadataID { get; set; }
        public int? ProductID { get; set; }
        public string? PropertyName { get; set; }
        public string? DisplayName { get; set; }
        public string? FormattedValue { get; set; }
        public string? PropertyGroup { get; set; }


        public MetadataValue() { }

        public MetadataValue(MetadataValue metadataValue, int? productID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            ProductID = productID;
            PropertyMetadataID = metadataValue.PropertyMetadataID;
            PropertyName = metadataValue.PropertyName;
            PropertyGroup = metadataValue.PropertyGroup;
            FormattedValue = metadataValue.FormattedValue;
            DisplayName = metadataValue.DisplayName;
        }
    }
}
