using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class Product
    {
        public string id { get; init; }
        public string reverse_id { get; init; }
        public string motive_id { get; init; }

        public string ecommerce_order_product_id { get; init; }
        public string refund_id { get; init; }
        public string qty { get; init; }
        public string requested_qty { get; init; }
        public string received_qty { get; init; }
        public string product_received_comment { get; init; }
        public string comments { get; init; }
        public string reverse_action { get; init; }
        public string customer_retention_method_id { get; init; }
        public string protocol { get; init; }
        
        public string product_id { get; init; }
        public string is_included_by_admin { get; init; }
        public string is_bundle { get; init; }
        public string hash { get; init; }
        public string name { get; init; }
        public string sku { get; init; }
        public string reference { get; init; }
        public string price { get; init; }
        public string selling_price { get; init; }
        public string shipping_price { get; init; }
        public string weight { get; init; }
        public string order_id { get; init; }
        public string returned_invoice { get; init; }
        public string returned_invoice_url { get; init; }
        public string returned_invoice_url_xml { get; init; }
        public string invoice { get; init; }
        public string original_qty { get; init; }

        public string brand { get; init; }
        public string situation { get; init; }
        public string bundle_hash { get; init; }
        
        public Reason reason { get; init; }
        public SubReason subreason { get; init; }
        public List<Image> images { get; init; }
        public List<string> image_url { get; init; }
    }
}
