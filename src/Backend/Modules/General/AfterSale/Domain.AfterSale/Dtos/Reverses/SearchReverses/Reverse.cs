using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.SearchReverses
{
    public class Reverse
    {
        public string id { get; init; }
        public string reverse_type { get; init; }
        public string reverse_type_name { get; init; }
        public string created_at { get; init; }
        public string updated_at { get; init; }
        public string order_id { get; init; }
        public string ecommerce_order_id { get; init; }
        public string total_amount { get; init; }
        public Customer customer { get; init; }
        public Status status { get; init; }
        public Tracking tracking { get; init; }
        public string refunds_count { get; init; }
    }
}
