using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.Transportations
{
    public class Transportation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        //public string? doc_company { get; private set; }
        public string transportation { get; private set; }

        public Transportation() { }

        public Transportation(string transportation/*, string doc_company*/)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            //this.doc_company = doc_company.name?.Length > 14 ? doc_company.name?.Substring(0, 14) : doc_company.name;
            this.transportation = transportation;
        }
    }
}
