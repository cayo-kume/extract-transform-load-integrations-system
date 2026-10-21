using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Page
    {
        public PageBodyContent introduction { get; init; }
        public PageBodyContent select_source { get; init; }
        public PageBodyContent order { get; init; }
        public PageBodyContent insurance_checklist { get; init; }
        public PageBodyContent insurance_loading_failure { get; init; }
        public PageBodyContent shipping { get; init; }
        public PageBodyContent resume { get; init; }
        public PageBodyContent deadline_for_insurance_cancellation_expired { get; init; }
        public PageBodyContent finish { get; init; }
        public PageBodyContent finish_thank { get; init; }
        public PageBodyContent customer { get; init; }
        public PageBodyContent customer_products { get; init; }
        public PageBodyContent email_by_customer { get; init; }
        public PageBodyContent retention_voucher { get; init; }
        public PageBodyContent retention_freight { get; init; }
        public PageBodyContent existing_request_post { get; init; }
        public PageBodyContent existing_request_collect { get; init; }
        public PageBodyContent existing_request { get; init; }
        public PageBodyContent request_follow_up { get; init; }
        public PageBodyContent refund_nps { get; init; }
        public PageBodyContent refund_nps_confirmation { get; init; }
        public PageBodyContent products { get; init; }
        public PageBodyContent cancellation_insurance { get; init; }
        public PageBodyContent seller { get; init; }
        public PageBodyContent retention { get; init; }
        public PageBodyContent bank_account_details { get; init; }
    }
}
