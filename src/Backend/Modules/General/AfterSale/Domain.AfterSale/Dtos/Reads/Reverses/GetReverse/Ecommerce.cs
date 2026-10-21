using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.GetReverse
{
    public class Ecommerce
    {
        public string id { get; init; }
        public string uuid { get; init; }
        public string company_name { get; init; }
        public string trade_name { get; init; }
        public string display_name { get; init; }
        public string url { get; init; }
        public string is_active { get; init; }
        public string phone { get; init; }
        public string email { get; init; }
        public string document { get; init; }
        public string address_id { get; init; }
        public string maintenance_mode_global { get; init; }
        public string last_order_report_date { get; init; }
        public string is_test { get; init; }
    }
}
