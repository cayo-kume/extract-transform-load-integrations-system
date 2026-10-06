using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class EcommerceOrder
    {
        public string id { get; init; }
        public string order_id { get; init; }
        public string invoice_number { get; init; }
        public string uuid { get; init; }
        public string shipping_value { get; init; }
        public string ordered_at { get; init; }
        public string invoiced_at { get; init; }
        public string delivered_at { get; init; }
        public string value { get; init; }
        public string ecommerce_id { get; init; }
        public string customer_id { get; init; }
        public string is_online { get; init; }
        public string is_marketplace { get; init; }
        public string marketplace_name { get; init; }
        public string updated_at { get; init; }
        public string deleted_at { get; init; }
        public string is_erased { get; init; }
        public string is_restrict { get; init; }
        public List<string> payment_methods { get; init; }
    }
}
