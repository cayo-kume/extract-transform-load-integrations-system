using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class Refund
    {
        public string id { get; init; }
        public string type { get; init; }
        public string action { get; init; }
        public string requested_shipping_amount { get; init; }
        public string shipping_method { get; init; }
        public string added_taxes_amount { get; init; }
        public string requested_raw_amount { get; init; }
        public string requested_amount { get; init; }
        public string requested_total_amount { get; init; }
        public string received_amount { get; init; }
        public string received_raw_amount { get; init; }
        public string total_amount { get; init; }
        public string free_shipping { get; init; }
        public string created_at { get; init; }
        public string updated_at { get; init; }
        public string seller_info { get; init; }
        public string refunded_shipping_type { get; init; }
        public string can_edit_wire_transfer { get; init; }
        public string can_edit_email { get; init; }
        public string has_wire_transfer_account { get; init; }
        public string customer_retention_method_id { get; init; }
        public string external_order_url { get; init; }
        public string cashback_account { get; init; }
        public string should_ask_voucher_code { get; init; }
        public string comments_count { get; init; }
        public string enable_payment { get; init; }
        public string insurance_amount { get; init; }
        public string bonus_amount { get; init; }
        public string bonus_amount_percent { get; init; }

        public Customer customer { get; init; }
        public Status status { get; init; }
        public List<StatusHistories> status_histories { get; init; }
        public List<TotalAmountHistories> total_amount_histories { get; init; }
        public Voucher voucher { get; init; }
        public Reverse reverse { get; init; }
        public List<Product> products { get; init; }
        public ProductPriceType product_price_type { get; init; }

        public List<string> order_payment_methods { get; init; }
        public List<string> could_change_status_for { get; init; }
    }
}
