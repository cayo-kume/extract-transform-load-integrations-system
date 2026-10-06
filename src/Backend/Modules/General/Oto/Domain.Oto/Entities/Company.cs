using Domain.Core.Entities.Base;

namespace Domain.Oto.Models
{
    public class Company : CompanyBase
    {
        public string segment_company { get; set; }
        public string country_company { get; set; }
        public int status_company { get; set; }
    }
}
