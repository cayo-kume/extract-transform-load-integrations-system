using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class Reverse
    {
        public string updated_at { get; init; }
        public string reverse_type { get; init; }
        public string id { get; init; }
        public string created_at { get; init; }
        public string refunds_count { get; init; }

        public Status status { get; init; }
        public EcommerceOrder ecommerceOrder { get; init; }
        public EcommerceOrderSeller ecommerce_order_seller { get; init; }
    }
}
