using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class Tracking
    {
        public string id { get; init; }
        public string reverse_id { get; init; }
        public string courier_contract_id { get; init; }
        public string authorization_code { get; init; }
        public string tracking_code { get; init; }
        public string shipping_amount { get; init; }
        public string package_amount { get; init; }
        public string courier_name { get; init; }
        public string tracking_url { get; init; }
        public string expire_date { get; init; }
        public string collect_date { get; init; }
        public string requested_collect_date { get; init; }
        public string status { get; init; }
        public string message { get; init; }
        public string status_updated_at { get; init; }
        public string is_change_collect_to_post { get; init; }
        public string type { get; init; }
        public string qr_code { get; init; }
        public string service_type { get; init; }
        public string cte { get; init; }
        public string delivery_deadline { get; init; }
        public string updated_at { get; init; }
        public string deleted_at { get; init; }
    }
}
