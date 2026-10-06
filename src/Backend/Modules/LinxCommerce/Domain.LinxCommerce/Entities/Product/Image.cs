namespace Domain.LinxCommerce.Entities.Product
{
    public class Image
    {
        public DateTime lastupdateon { get; set; }
        public string? RelativePath { get; set; }
        public string? Extension { get; set; }
        public decimal? MaxWidth { get; set; }
        public decimal? MaxHeight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public Int32? MediaSizeType { get; set; }
        public string? AbsolutePath { get; set; }
        public Int32? MediaID { get; set; }

        public Image() { }

        public Image(Image image, Int32? mediaID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.RelativePath = image.RelativePath;
            this.Extension = image.Extension;
            this.MaxWidth = image.MaxWidth;
            this.MaxHeight = image.MaxHeight;
            this.Width = image.Width;
            this.Height = image.Height;
            this.MediaSizeType = image.MediaSizeType;
            this.AbsolutePath = image.AbsolutePath;
            this.MediaID = mediaID;
        }
    }
}
