using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class EcommerceOrderPaymentMethods
    {
        public string? doc_company { get; private set; }

        public Int64 reverse_id { get; private set; }
        public Int64 ecommerce_order_id { get; private set; }
        public Int64 ecommerce_order_payment_id { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }

        public string? payment_method { get; private set; }

        public EcommerceOrderPaymentMethods() { }

        public EcommerceOrderPaymentMethods(string doc_company, Int64 reverse_id, Int64 ecommerce_order_id, Int64 ecommerce_order_payment_id, string payment_method)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.reverse_id = reverse_id;
            this.ecommerce_order_id = ecommerce_order_id;
            this.ecommerce_order_payment_id = ecommerce_order_payment_id;
            this.payment_method = payment_method;
        }
    }
}
