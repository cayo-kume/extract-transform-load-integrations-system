using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class OrderPaymentMethod
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public string? doc_company { get; private set; }

        public Int64 refund_id { get; private set; }
        public int id { get; private set; }

        public string? payment_method { get; private set; }

        public OrderPaymentMethod() { }

        public OrderPaymentMethod(string doc_company, Int64 refund_id, int id, string payment_method)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.refund_id = refund_id;
            this.id = id;
            this.payment_method = payment_method?.Length > 50 ? payment_method?.Substring(0, 50) : payment_method;
        }
    }
}
