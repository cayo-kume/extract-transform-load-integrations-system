using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.GetReverse
{
    public class EcommerceOrderPaymentMethods
    {
        public Int64 reverse_id { get; private set; }
        public Int64 ecommerce_order_id { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }

        public string? payment_method { get; private set; }

        public EcommerceOrderPaymentMethods() { }

        public EcommerceOrderPaymentMethods(Int64 reverse_id, Int64 ecommerce_order_id, string payment_method)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.reverse_id = reverse_id;
            this.ecommerce_order_id = ecommerce_order_id;
            this.payment_method = payment_method;
        }
    }
}
