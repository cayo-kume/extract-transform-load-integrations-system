using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Stone.Entities
{
    public class Zpl
    {
        public DateTime lastupdateon { get; set; }
        public string documento { get; set; }

        [SkipProperty]
        public string referencekey { get; set; }

        public string zpl { get; set; }

        [NotMapped]
        [SkipProperty]
        public Error error { get; set; } = new Error();

        public void SetZpl(string zpl)
        {
            this.lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.zpl = zpl;
        }

        public void SetError(string message, string error)
        {
            this.error = String.IsNullOrEmpty(message) && String.IsNullOrEmpty(error) ? new Error() : new Error(documento, message, error);
        }
    }
}
