using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Product
{
    public class Midia
    {
        public DateTime lastupdateon { get; set; }
        public Int32? MediaID { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Index { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? Type { get; set; }
        public Int32? ParentMediaID { get; set; }
        public string? OriginalFileName { get; set; }

        [SkipProperty]
        public List<MediaAssociation> MediaAssociations { get; set; } = new List<MediaAssociation>();

        [SkipProperty]
        public Image Image { get; set; }

        [SkipProperty]
        public Video Video { get; set; }

        public Midia() { }

        public Midia(Midia midia)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.MediaID = midia.MediaID;
            this.IsDeleted = midia.IsDeleted;
            this.Index = midia.Index;
            this.CreatedDate = midia.CreatedDate;
            this.Type = midia.Type;
            this.ParentMediaID = midia.ParentMediaID;
            this.OriginalFileName = midia.OriginalFileName;

            this.Video = new Video(midia.Video, midia.MediaID);
            this.Image = new Image(midia.Image, midia.MediaID);
        }
    }
}
