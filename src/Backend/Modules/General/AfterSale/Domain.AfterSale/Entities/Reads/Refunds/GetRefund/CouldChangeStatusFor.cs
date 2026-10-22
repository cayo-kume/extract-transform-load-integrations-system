using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class CouldChangeStatusFor
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public string? doc_company { get; private set; }

        public Int64 refund_id { get; private set; }
        public int id { get; private set; }

        public string? description { get; private set; }

        public CouldChangeStatusFor() { }

        public CouldChangeStatusFor(string doc_company, Int64 refund_id, int id, string could_change_status_for)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.refund_id = refund_id;
            this.id = id;
            description = could_change_status_for?.Length > 50 ? could_change_status_for?.Substring(0, 50) : could_change_status_for;
        }
    }
}
