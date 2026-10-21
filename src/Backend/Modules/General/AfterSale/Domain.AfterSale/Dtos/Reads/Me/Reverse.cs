using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Reverse
    {
        public MailBodyContent invalid_courier_contract { get; init; }
        public MailBodyContent awaiting_with_freight_by_customer { get; init; }
        public MailBodyContent courier_collect_scheduling { get; init; }
        public MailBodyContent courier_collect_code_generated { get; init; }
        public MailBodyContent estafeta_post_code_generated { get; init; }
        public MailBodyContent manual_modal_change_correio_code_generated_collect { get; init; }
        public MailBodyContent manual_modal_change_correio_code_generated { get; init; }
        public MailBodyContent correio_code_generated { get; init; }
        public MailBodyContent send4_code_generated { get; init; }
        public MailBodyContent pending { get; init; }
        public MailBodyContent generic_courier_collect_requested_date_changed_code_generated { get; init; }
        public MailBodyContent locker_post_code_generated { get; init; }
        public MailBodyContent correio_code_expiring { get; init; }
        public MailBodyContent correio_collect_change_to_post { get; init; }
        public MailBodyContent correio_tracking_posted { get; init; }
        public MailBodyContent correio_tracking_delivered { get; init; }
        public MailBodyContent ecommerce_store_reverse { get; init; }
        public MailBodyContent products_received { get; init; }
        public MailBodyContent reverse_refused { get; init; }
        public MailBodyContent reverse_canceled { get; init; }
        public MailBodyContent reverse_reopened { get; init; }
        public MailBodyContent reverse_returned_invoice_solicitation { get; init; }
        public MailBodyContent reverse_returned_invoice_solicitation_partially_missing { get; init; }
        public MailBodyContent awaiting_without_process { get; init; }
        public MailBodyContent reverse_with_manual_processing { get; init; }
        public MailBodyContent generic_courier_collect_info { get; init; }
        public MailBodyContent return_point_code_generated { get; init; }
        public MailBodyContent return_point_code_expiring { get; init; }
        public MailBodyContent awaiting_collect_scheduling { get; init; }
        public MailBodyContent awaiting_collect_scheduling_reminder { get; init; }
    }
}
