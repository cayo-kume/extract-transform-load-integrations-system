using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class CouldChangeStatusFor
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 refund_id { get; private set; }
        public int id { get; private set; }
        public string could_change_status_for { get; private set; }

        public CouldChangeStatusFor() { }

        public CouldChangeStatusFor(Int64 refund_id, int id, string could_change_status_for)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.refund_id = refund_id;
            this.id = id;
            this.could_change_status_for = could_change_status_for;
        }
    }
}
