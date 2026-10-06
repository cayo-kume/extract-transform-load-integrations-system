using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Refunds.SearchRefunds
{
    public class Refund
    {
        public string id { get; init; }
        public string action { get; init; }
        public string bonus_amount_percent { get; init; }
        public string order_id { get; init; }
        public string requested_amount { get; init; }
        public string requested_total_amount { get; init; }
        public string reverse_id { get; init; }
        public string total_amount { get; init; }
        public string type { get; init; }
        public string updated_at { get; init; }
        public string created_at { get; init; }
        public string ecommerce_order_id { get; init; }
        public string bonus_amount { get; init; }
        public string requested_shipping_amount { get; init; }
        public string refund_attachment { get; init; }
        public string shipping_method { get; init; }
        public string requested_raw_amount { get; init; }
        public string refunded_shipping_type { get; init; }
        public string last_status_history_date { get; init; }

        public Customer customer { get; init; }
        public Status status { get; init; }
        public Reverse reverse { get; init; }
        public ProductPriceType product_price_type { get; init; }
    }
}
