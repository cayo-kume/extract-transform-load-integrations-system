namespace Domain.LinxCommerce.Entities.Sku
{
    public class MetadataValue
    {
        public DateTime lastupdateon { get; set; }
        public Int32? ProductID { get; set; }
        public Int32? PropertyMetadataID { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyGroup { get; set; }
        public Int32? InputType { get; set; }
        public string? Value { get; set; }
        public string? SerializedValue { get; set; }
        public string? SerializedBlobValue { get; set; }
        public string? IntegrationID { get; set; }
        public string? FormattedValue { get; set; }
        public string? DisplayName { get; set; }

        public MetadataValue() { }

        public MetadataValue(MetadataValue metadataValue, Int32? productID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.ProductID = productID;
            this.PropertyMetadataID = metadataValue.PropertyMetadataID;
            this.PropertyName = metadataValue.PropertyName;
            this.PropertyGroup = metadataValue.PropertyGroup;
            this.InputType = metadataValue.InputType;
            this.Value = metadataValue.Value;
            this.SerializedValue = metadataValue.SerializedValue;
            this.SerializedBlobValue = metadataValue.SerializedBlobValue;
            this.IntegrationID = metadataValue.IntegrationID;
            this.FormattedValue = metadataValue.FormattedValue;
            this.DisplayName = metadataValue.DisplayName;
        }
    }
}
