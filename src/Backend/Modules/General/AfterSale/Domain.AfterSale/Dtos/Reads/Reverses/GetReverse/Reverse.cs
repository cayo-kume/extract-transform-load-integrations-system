using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.GetReverse
{
    public class Reverse
    {
        public string id { get; init; }
        public string service_type_change { get; init; }
        public string ecommerce_order_id { get; init; }
        public string courier_contract_id { get; init; }
        public string invoice { get; init; }
        public string courier_collect { get; init; }
        public string is_awaiting_resend_webhook { get; init; }
        public string is_erased { get; init; }
        public string freight_by_customer { get; init; }
        public string skip_process_step { get; init; }
        public string billing_item_id { get; init; }
        public string status_id { get; init; }
        public string ecommerce_id { get; init; }
        public string insurance_manual_needed { get; init; }
        public string created_at { get; init; }
        public string updated_at { get; init; }
        public string deleted_at { get; init; }
        public string origin_seller_id { get; init; }
        public string reverse_type { get; init; }
        public string courier_service_type { get; init; }
        public string store_id { get; init; }
        public string total_amount { get; init; }
        public string store_expire_date { get; init; }
        public string origin { get; init; }
        public string external_source { get; init; }
        public string type { get; init; }
        public string billing_date { get; init; }
        public string must_treat_refund { get; init; }
        public string customer_nfd_attached { get; init; }
        public string customer_nfd_xml_attached { get; init; }
        public string returned_invoice { get; init; }
        public string reverse_type_name { get; init; }
        public string is_generic_courier { get; init; }
        public string can_generate_voucher { get; init; }
        public string could_cancel { get; init; }
        public string can_send_correction_letter { get; init; }
        public string correction_letter_link { get; init; }
        public string customer_url { get; init; }
        public string timeline_url { get; init; }
        public string collect_scheduling_link { get; init; }
        public CourierData courier_data { get; init; }
        public DestinationData destination_data { get; init; }
        public CourierDataEncrypted courier_data_encrypted { get; init; }
        public Ecommerce ecommerce { get; init; }
        public EcommerceOrder ecommerce_order { get; init; }
        public Status status { get; init; }
        public Tracking tracking { get; init; }
        public EcommerceOrderSeller ecommerce_order_seller { get; init; }
        public Brand brand { get; init; }
        public RetryOptions retry_options { get; init; }
        public ReverseDestination reverse_destination { get; init; }
        public List<Product> products { get; init; }
        public List<StatusHistories> status_histories { get; init; }
    }
}
