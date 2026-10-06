using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeWebSiteSettings
    {
        public int? SalesRepresentativeID { get; set; }
        public string? WebSiteFilter { get; set; }
        public List<int>? WebSiteGroups { get; set; }
        public List<int>? WebSites { get; set; }

        [SkipProperty]
        public string? WebSiteGroup { get; set; }

        [SkipProperty]
        public string? WebSite { get; set; }

        public SalesRepresentativeWebSiteSettings() { }

        public SalesRepresentativeWebSiteSettings(SalesRepresentativeWebSiteSettings salesRepresentativeWebSiteSettings, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.WebSiteFilter = salesRepresentativeWebSiteSettings.WebSiteFilter;
            this.WebSiteGroup = salesRepresentativeWebSiteSettings.WebSiteGroups.Count() > 0 ? string.Join(", ", salesRepresentativeWebSiteSettings.WebSiteGroups) : null;
            this.WebSite = salesRepresentativeWebSiteSettings.WebSites.Count() > 0 ? string.Join(", ", salesRepresentativeWebSiteSettings.WebSites) : null;
        }
    }
}
