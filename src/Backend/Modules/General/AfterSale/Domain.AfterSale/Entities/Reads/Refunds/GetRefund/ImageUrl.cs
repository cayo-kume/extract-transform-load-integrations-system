using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class ImageUrl
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 refund_id { get; private set; }
        public Int64 product_id { get; private set; }
        public Int64 id { get; private set; }
        public string url { get; private set; }

        public ImageUrl() { }

        public ImageUrl(Int64 refund_id, Int64 product_id, Int64 id, string url)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id = id;
            this.refund_id = refund_id;
            this.product_id = product_id;
            this.url = url;
        }
    }
}
