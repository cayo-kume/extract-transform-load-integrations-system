using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Refunds.GetRefund
{
    public class OrderPaymentMethod
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 refund_id { get; private set; }
        public int id { get; private set; }
        public string payment_method { get; private set; }

        public OrderPaymentMethod() { }

        public OrderPaymentMethod(Int64 refund_id, int id, string payment_method)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.refund_id = refund_id;
            this.id = id;
            this.payment_method = payment_method;
        }
    }
}
