using Domain.Shared.Extensions;

namespace Domain.AfterSale.Entities.Me
{
    public class MessageMail
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public int ecommerce_id { get; private set; }

        public string? greeting_content { get; private set; }
        public bool greeting_advanced_email_enabled { get; private set; }

        public string? footer_content { get; private set; }
        public bool footer_advanced_email_enabled { get; private set; }

        public string? reverse_invalid_courier_contract_content { get; private set; }
        public bool reverse_invalid_courier_contract_enabled { get; private set; }
        public bool reverse_invalid_courier_contract_advanced_email_enabled { get; private set; }
        public bool reverse_invalid_courier_contract_from_customer_origin_enabled { get; private set; }
        public bool reverse_invalid_courier_contract_from_agent_origin_enabled { get; private set; }

        public string? reverse_awaiting_with_freight_by_customer_subject { get; private set; }
        public string? reverse_awaiting_with_freight_by_customer_content { get; private set; }
        public bool reverse_awaiting_with_freight_by_customer_enabled { get; private set; }
        public bool reverse_awaiting_with_freight_by_customer_advanced_email_enabled { get; private set; }
        public bool reverse_awaiting_with_freight_by_customer_from_customer_origin_enabled { get; private set; }
        public bool reverse_awaiting_with_freight_by_customer_from_agent_origin_enabled { get; private set; }

        public string? reverse_pending_subject { get; private set; }
        public string? reverse_pending_content { get; private set; }
        public bool reverse_pending_enabled { get; private set; }
        public bool reverse_pending_advanced_email_enabled { get; private set; }
        public bool reverse_pending_from_customer_origin_enabled { get; private set; }
        public bool reverse_pending_from_agent_origin_enabled { get; private set; }

        public string? reverse_send4_code_generated_subject { get; private set; }
        public string? reverse_send4_code_generated_content { get; private set; }
        public bool reverse_send4_code_generated_enabled { get; private set; }
        public bool reverse_send4_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_send4_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_send4_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_correio_code_generated_subject { get; private set; }
        public string? reverse_correio_code_generated_content { get; private set; }
        public bool reverse_correio_code_generated_enabled { get; private set; }
        public bool reverse_correio_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_correio_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_correio_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_manual_modal_change_correio_code_generated_subject { get; private set; }
        public string? reverse_manual_modal_change_correio_code_generated_content { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_enabled { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_manual_modal_change_correio_code_generated_collect_subject { get; private set; }
        public string? reverse_manual_modal_change_correio_code_generated_collect_content { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_collect_enabled { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_collect_advanced_email_enabled { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_collect_from_customer_origin_enabled { get; private set; }
        public bool reverse_manual_modal_change_correio_code_generated_collect_from_agent_origin_enabled { get; private set; }

        public string? reverse_estafeta_post_code_generated_subject { get; private set; }
        public string? reverse_estafeta_post_code_generated_content { get; private set; }
        public bool reverse_estafeta_post_code_generated_enabled { get; private set; }
        public bool reverse_estafeta_post_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_estafeta_post_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_estafeta_post_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_courier_collect_code_generated_subject { get; private set; }
        public string? reverse_courier_collect_code_generated_content { get; private set; }
        public bool reverse_courier_collect_code_generated_enabled { get; private set; }
        public bool reverse_courier_collect_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_courier_collect_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_courier_collect_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_courier_collect_scheduling_subject { get; private set; }
        public string? reverse_courier_collect_scheduling_content { get; private set; }
        public bool reverse_courier_collect_scheduling_enabled { get; private set; }
        public bool reverse_courier_collect_scheduling_advanced_email_enabled { get; private set; }
        public bool reverse_courier_collect_scheduling_from_customer_origin_enabled { get; private set; }
        public bool reverse_courier_collect_scheduling_from_agent_origin_enabled { get; private set; }

        public string? reverse_generic_courier_collect_requested_date_changed_code_generated_subject { get; private set; }
        public string? reverse_generic_courier_collect_requested_date_changed_code_generated_content { get; private set; }
        public bool reverse_generic_courier_collect_requested_date_changed_code_generated_enabled { get; private set; }
        public bool reverse_generic_courier_collect_requested_date_changed_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_generic_courier_collect_requested_date_changed_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_generic_courier_collect_requested_date_changed_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_locker_post_code_generated_subject { get; private set; }
        public string? reverse_locker_post_code_generated_content { get; private set; }
        public bool reverse_locker_post_code_generated_enabled { get; private set; }
        public bool reverse_locker_post_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_locker_post_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_locker_post_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_correio_code_expiring_subject { get; private set; }
        public string? reverse_correio_code_expiring_content { get; private set; }
        public bool reverse_correio_code_expiring_enabled { get; private set; }
        public bool reverse_correio_code_expiring_advanced_email_enabled { get; private set; }
        public bool reverse_correio_code_expiring_from_customer_origin_enabled { get; private set; }
        public bool reverse_correio_code_expiring_from_agent_origin_enabled { get; private set; }

        public string? reverse_correio_collect_change_to_post_subject { get; private set; }
        public string? reverse_correio_collect_change_to_post_content { get; private set; }
        public bool reverse_correio_collect_change_to_post_enabled { get; private set; }
        public bool reverse_correio_collect_change_to_post_advanced_email_enabled { get; private set; }
        public bool reverse_correio_collect_change_to_post_from_customer_origin_enabled { get; private set; }
        public bool reverse_correio_collect_change_to_post_from_agent_origin_enabled { get; private set; }

        public string? reverse_correio_tracking_posted_subject { get; private set; }
        public string? reverse_correio_tracking_posted_content { get; private set; }
        public bool reverse_correio_tracking_posted_enabled { get; private set; }
        public bool reverse_correio_tracking_posted_advanced_email_enabled { get; private set; }
        public bool reverse_correio_tracking_posted_from_customer_origin_enabled { get; private set; }
        public bool reverse_correio_tracking_posted_from_agent_origin_enabled { get; private set; }

        public string? reverse_correio_tracking_delivered_subject { get; private set; }
        public string? reverse_correio_tracking_delivered_content { get; private set; }
        public bool reverse_correio_tracking_delivered_enabled { get; private set; }
        public bool reverse_correio_tracking_delivered_advanced_email_enabled { get; private set; }
        public bool reverse_correio_tracking_delivered_from_customer_origin_enabled { get; private set; }
        public bool reverse_correio_tracking_delivered_from_agent_origin_enabled { get; private set; }

        public string? reverse_ecommerce_store_reverse_subject { get; private set; }
        public string? reverse_ecommerce_store_reverse_content { get; private set; }
        public bool reverse_ecommerce_store_reverse_enabled { get; private set; }
        public bool reverse_ecommerce_store_reverse_advanced_email_enabled { get; private set; }
        public bool reverse_ecommerce_store_reverse_from_customer_origin_enabled { get; private set; }
        public bool reverse_ecommerce_store_reverse_from_agent_origin_enabled { get; private set; }

        public string? reverse_products_received_subject { get; private set; }
        public string? reverse_products_received_content { get; private set; }
        public bool reverse_products_received_enabled { get; private set; }
        public bool reverse_products_received_advanced_email_enabled { get; private set; }
        public bool reverse_products_received_from_customer_origin_enabled { get; private set; }
        public bool reverse_products_received_from_agent_origin_enabled { get; private set; }

        public string? reverse_reverse_refused_subject { get; private set; }
        public string? reverse_reverse_refused_content { get; private set; }
        public bool reverse_reverse_refused_enabled { get; private set; }
        public bool reverse_reverse_refused_advanced_email_enabled { get; private set; }
        public bool reverse_reverse_refused_from_customer_origin_enabled { get; private set; }
        public bool reverse_reverse_refused_from_agent_origin_enabled { get; private set; }

        public string? reverse_reverse_canceled_subject { get; private set; }
        public string? reverse_reverse_canceled_content { get; private set; }
        public bool reverse_reverse_canceled_enabled { get; private set; }
        public bool reverse_reverse_canceled_advanced_email_enabled { get; private set; }
        public bool reverse_reverse_canceled_from_customer_origin_enabled { get; private set; }
        public bool reverse_reverse_canceled_from_agent_origin_enabled { get; private set; }

        public string? reverse_reverse_reopened_subject { get; private set; }
        public string? reverse_reverse_reopened_content { get; private set; }
        public bool reverse_reverse_reopened_enabled { get; private set; }
        public bool reverse_reverse_reopened_advanced_email_enabled { get; private set; }
        public bool reverse_reverse_reopened_from_customer_origin_enabled { get; private set; }
        public bool reverse_reverse_reopened_from_agent_origin_enabled { get; private set; }

        public string? reverse_reverse_returned_invoice_solicitation_subject { get; private set; }
        public string? reverse_reverse_returned_invoice_solicitation_content { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_enabled { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_advanced_email_enabled { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_from_customer_origin_enabled { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_from_agent_origin_enabled { get; private set; }

        public string? reverse_reverse_returned_invoice_solicitation_partially_missing_subject { get; private set; }
        public string? reverse_reverse_returned_invoice_solicitation_partially_missing_content { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_partially_missing_enabled { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_partially_missing_advanced_email_enabled { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_partially_missing_from_customer_origin_enabled { get; private set; }
        public bool reverse_reverse_returned_invoice_solicitation_partially_missing_from_agent_origin_enabled { get; private set; }

        public string? reverse_awaiting_without_process_subject { get; private set; }
        public string? reverse_awaiting_without_process_content { get; private set; }
        public bool reverse_awaiting_without_process_enabled { get; private set; }
        public bool reverse_awaiting_without_process_advanced_email_enabled { get; private set; }
        public bool reverse_awaiting_without_process_from_customer_origin_enabled { get; private set; }
        public bool reverse_awaiting_without_process_from_agent_origin_enabled { get; private set; }

        public string? reverse_reverse_with_manual_processing_subject { get; private set; }
        public string? reverse_reverse_with_manual_processing_content { get; private set; }
        public bool reverse_reverse_with_manual_processing_enabled { get; private set; }
        public bool reverse_reverse_with_manual_processing_advanced_email_enabled { get; private set; }
        public bool reverse_reverse_with_manual_processing_from_customer_origin_enabled { get; private set; }
        public bool reverse_reverse_with_manual_processing_from_agent_origin_enabled { get; private set; }

        public string? reverse_generic_courier_collect_info_subject { get; private set; }
        public string? reverse_generic_courier_collect_info_content { get; private set; }
        public bool reverse_generic_courier_collect_info_enabled { get; private set; }
        public bool reverse_generic_courier_collect_info_advanced_email_enabled { get; private set; }
        public bool reverse_generic_courier_collect_info_from_customer_origin_enabled { get; private set; }
        public bool reverse_generic_courier_collect_info_from_agent_origin_enabled { get; private set; }

        public string? reverse_return_point_code_generated_subject { get; private set; }
        public string? reverse_return_point_code_generated_content { get; private set; }
        public bool reverse_return_point_code_generated_enabled { get; private set; }
        public bool reverse_return_point_code_generated_advanced_email_enabled { get; private set; }
        public bool reverse_return_point_code_generated_from_customer_origin_enabled { get; private set; }
        public bool reverse_return_point_code_generated_from_agent_origin_enabled { get; private set; }

        public string? reverse_return_point_code_expiring_subject { get; private set; }
        public string? reverse_return_point_code_expiring_content { get; private set; }
        public bool reverse_return_point_code_expiring_enabled { get; private set; }
        public bool reverse_return_point_code_expiring_advanced_email_enabled { get; private set; }
        public bool reverse_return_point_code_expiring_from_customer_origin_enabled { get; private set; }
        public bool reverse_return_point_code_expiring_from_agent_origin_enabled { get; private set; }

        public string? reverse_awaiting_collect_scheduling_subject { get; private set; }
        public string? reverse_awaiting_collect_scheduling_content { get; private set; }
        public bool reverse_awaiting_collect_scheduling_enabled { get; private set; }
        public bool reverse_awaiting_collect_scheduling_advanced_email_enabled { get; private set; }
        public bool reverse_awaiting_collect_scheduling_from_customer_origin_enabled { get; private set; }
        public bool reverse_awaiting_collect_scheduling_from_agent_origin_enabled { get; private set; }

        public string? reverse_awaiting_collect_scheduling_reminder_subject { get; private set; }
        public string? reverse_awaiting_collect_scheduling_reminder_content { get; private set; }
        public bool reverse_awaiting_collect_scheduling_reminder_enabled { get; private set; }
        public bool reverse_awaiting_collect_scheduling_reminder_advanced_email_enabled { get; private set; }
        public bool reverse_awaiting_collect_scheduling_reminder_from_customer_origin_enabled { get; private set; }
        public bool reverse_awaiting_collect_scheduling_reminder_from_agent_origin_enabled { get; private set; }

        public string? refund_approved_subject { get; private set; }
        public string? refund_approved_content { get; private set; }
        public bool refund_approved_enabled { get; private set; }
        public bool refund_approved_advanced_email_enabled { get; private set; }
        public bool refund_approved_from_customer_origin_enabled { get; private set; }
        public bool refund_approved_from_agent_origin_enabled { get; private set; }

        public string? refund_paid_refund_subject { get; private set; }
        public string? refund_paid_refund_content { get; private set; }
        public bool refund_paid_refund_enabled { get; private set; }
        public bool refund_paid_refund_advanced_email_enabled { get; private set; }
        public bool refund_paid_refund_from_customer_origin_enabled { get; private set; }
        public bool refund_paid_refund_from_agent_origin_enabled { get; private set; }

        public string? refund_refund_nps_subject { get; private set; }
        public string? refund_refund_nps_content { get; private set; }
        public bool refund_refund_nps_enabled { get; private set; }
        public bool refund_refund_nps_advanced_email_enabled { get; private set; }
        public bool refund_refund_nps_from_customer_origin_enabled { get; private set; }
        public bool refund_refund_nps_from_agent_origin_enabled { get; private set; }

        public string? refund_paid_wire_transfer_subject { get; private set; }
        public string? refund_paid_wire_transfer_content { get; private set; }
        public bool refund_paid_wire_transfer_enabled { get; private set; }
        public bool refund_paid_wire_transfer_advanced_email_enabled { get; private set; }
        public bool refund_paid_wire_transfer_from_customer_origin_enabled { get; private set; }
        public bool refund_paid_wire_transfer_from_agent_origin_enabled { get; private set; }

        public string? refund_paid_product_subject { get; private set; }
        public string? refund_paid_product_content { get; private set; }
        public bool refund_paid_product_enabled { get; private set; }
        public bool refund_paid_product_advanced_email_enabled { get; private set; }
        public bool refund_paid_product_from_customer_origin_enabled { get; private set; }
        public bool refund_paid_product_from_agent_origin_enabled { get; private set; }

        public string? refund_paid_voucher_subject { get; private set; }
        public string? refund_paid_voucher_content { get; private set; }
        public bool refund_paid_voucher_enabled { get; private set; }
        public bool refund_paid_voucher_advanced_email_enabled { get; private set; }
        public bool refund_paid_voucher_from_customer_origin_enabled { get; private set; }
        public bool refund_paid_voucher_from_agent_origin_enabled { get; private set; }

        public string? refund_paid_credit_subject { get; private set; }
        public string? refund_paid_credit_content { get; private set; }
        public bool refund_paid_credit_enabled { get; private set; }
        public bool refund_paid_credit_advanced_email_enabled { get; private set; }
        public bool refund_paid_credit_from_customer_origin_enabled { get; private set; }
        public bool refund_paid_credit_from_agent_origin_enabled { get; private set; }

        public string? refund_ask_wire_transfer_data_subject { get; private set; }
        public string? refund_ask_wire_transfer_data_content { get; private set; }
        public bool refund_ask_wire_transfer_data_enabled { get; private set; }
        public bool refund_ask_wire_transfer_data_advanced_email_enabled { get; private set; }
        public bool refund_ask_wire_transfer_data_from_customer_origin_enabled { get; private set; }
        public bool refund_ask_wire_transfer_data_from_agent_origin_enabled { get; private set; }

        public string? refund_paid_hard_retention_subject { get; private set; }
        public string? refund_paid_hard_retention_content { get; private set; }
        public bool refund_paid_hard_retention_enabled { get; private set; }
        public bool refund_paid_hard_retention_advanced_email_enabled { get; private set; }
        public bool refund_paid_hard_retention_from_customer_origin_enabled { get; private set; }
        public bool refund_paid_hard_retention_from_agent_origin_enabled { get; private set; }

        public string? refund_ask_wire_transfer_data_update_subject { get; private set; }
        public string? refund_ask_wire_transfer_data_update_content { get; private set; }
        public bool refund_ask_wire_transfer_data_update_enabled { get; private set; }
        public bool refund_ask_wire_transfer_data_update_advanced_email_enabled { get; private set; }
        public bool refund_ask_wire_transfer_data_update_from_customer_origin_enabled { get; private set; }
        public bool refund_ask_wire_transfer_data_update_from_agent_origin_enabled { get; private set; }

        public string? refund_review_subject { get; private set; }
        public string? refund_review_content { get; private set; }
        public bool refund_review_enabled { get; private set; }
        public bool refund_review_advanced_email_enabled { get; private set; }
        public bool refund_review_from_customer_origin_enabled { get; private set; }
        public bool refund_review_from_agent_origin_enabled { get; private set; }

        public string? retention_disabled_subject { get; private set; }
        public string? retention_disabled_content { get; private set; }
        public bool retention_disabled_enabled { get; private set; }
        public bool retention_disabled_advanced_email_enabled { get; private set; }
        public bool retention_disabled_from_customer_origin_enabled { get; private set; }
        public bool retention_disabled_from_agent_origin_enabled { get; private set; }

        public string? partials_partially_received_warning_content { get; private set; }
        public bool partials_partially_received_warning_advanced_email_enabled { get; private set; }

        public string? partials_new_products_include_info_content { get; private set; }
        public bool partials_new_products_include_info_advanced_email_enabled { get; private set; }

        public string? occurrence_awaiting_customer_subject { get; private set; }
        public string? occurrence_awaiting_customer_content { get; private set; }
        public bool occurrence_awaiting_customer_enabled { get; private set; }
        public bool occurrence_awaiting_customer_advanced_email_enabled { get; private set; }
        public bool occurrence_awaiting_customer_from_customer_origin_enabled { get; private set; }
        public bool occurrence_awaiting_customer_from_agent_origin_enabled { get; private set; }

        public string? occurrence_awaiting_customer_remind_deadline_subject { get; private set; }
        public string? occurrence_awaiting_customer_remind_deadline_content { get; private set; }
        public bool occurrence_awaiting_customer_remind_deadline_enabled { get; private set; }
        public bool occurrence_awaiting_customer_remind_deadline_advanced_email_enabled { get; private set; }
        public bool occurrence_awaiting_customer_remind_deadline_from_customer_origin_enabled { get; private set; }
        public bool occurrence_awaiting_customer_remind_deadline_from_agent_origin_enabled { get; private set; }

        public string? occurrence_created_subject { get; private set; }
        public string? occurrence_created_content { get; private set; }
        public bool occurrence_created_enabled { get; private set; }
        public bool occurrence_created_advanced_email_enabled { get; private set; }
        public bool occurrence_created_from_customer_origin_enabled { get; private set; }
        public bool occurrence_created_from_agent_origin_enabled { get; private set; }

        public string? cancellation_failed_subject { get; private set; }
        public string? cancellation_failed_content { get; private set; }
        public bool cancellation_failed_enabled { get; private set; }
        public bool cancellation_failed_advanced_email_enabled { get; private set; }
        public bool cancellation_failed_from_customer_origin_enabled { get; private set; }
        public bool cancellation_failed_from_agent_origin_enabled { get; private set; }

        public string? cancellation_fully_accepted_subject { get; private set; }
        public string? cancellation_fully_accepted_content { get; private set; }
        public bool cancellation_fully_accepted_enabled { get; private set; }
        public bool cancellation_fully_accepted_advanced_email_enabled { get; private set; }
        public bool cancellation_fully_accepted_from_customer_origin_enabled { get; private set; }
        public bool cancellation_fully_accepted_from_agent_origin_enabled { get; private set; }

        public string? cancellation_partially_accepted_subject { get; private set; }
        public string? cancellation_partially_accepted_content { get; private set; }
        public bool cancellation_partially_accepted_enabled { get; private set; }
        public bool cancellation_partially_accepted_advanced_email_enabled { get; private set; }
        public bool cancellation_partially_accepted_from_customer_origin_enabled { get; private set; }
        public bool cancellation_partially_accepted_from_agent_origin_enabled { get; private set; }

        public string? cancellation_refused_subject { get; private set; }
        public string? cancellation_refused_content { get; private set; }
        public bool cancellation_refused_enabled { get; private set; }
        public bool cancellation_refused_advanced_email_enabled { get; private set; }
        public bool cancellation_refused_from_customer_origin_enabled { get; private set; }
        public bool cancellation_refused_from_agent_origin_enabled { get; private set; }

        public string? cancellation_created_customer_subject { get; private set; }
        public string? cancellation_created_customer_content { get; private set; }
        public bool cancellation_created_customer_enabled { get; private set; }
        public bool cancellation_created_customer_advanced_email_enabled { get; private set; }
        public bool cancellation_created_customer_from_customer_origin_enabled { get; private set; }
        public bool cancellation_created_customer_from_agent_origin_enabled { get; private set; }

        public string? cancellation_created_agent_subject { get; private set; }
        public string? cancellation_created_agent_content { get; private set; }
        public bool cancellation_created_agent_enabled { get; private set; }
        public bool cancellation_created_agent_advanced_email_enabled { get; private set; }
        public bool cancellation_created_agent_from_customer_origin_enabled { get; private set; }
        public bool cancellation_created_agent_from_agent_origin_enabled { get; private set; }

        public string? cancellation_created_customer_by_agent_subject { get; private set; }
        public string? cancellation_created_customer_by_agent_content { get; private set; }
        public bool cancellation_created_customer_by_agent_enabled { get; private set; }
        public bool cancellation_created_customer_by_agent_advanced_email_enabled { get; private set; }
        public bool cancellation_created_customer_by_agent_from_customer_origin_enabled { get; private set; }
        public bool cancellation_created_customer_by_agent_from_agent_origin_enabled { get; private set; }

        public string? solicitation_partials_requested_products_ref_id { get; private set; }
        public string? solicitation_partials_requested_products_sku { get; private set; }
        public string? solicitation_partials_requested_products_unity { get; private set; }
        public string? solicitation_partials_requested_products_unity_plural { get; private set; }

        public string? compensation_payment_cancellation_gift_card_subject { get; private set; }
        public string? compensation_payment_cancellation_gift_card_content { get; private set; }
        public bool compensation_payment_cancellation_gift_card_enabled { get; private set; }
        public bool compensation_payment_cancellation_gift_card_advanced_email_enabled { get; private set; }
        public bool compensation_payment_cancellation_gift_card_from_customer_origin_enabled { get; private set; }
        public bool compensation_payment_cancellation_gift_card_from_agent_origin_enabled { get; private set; }

        public string? compensation_payment_cancellation_refund_subject { get; private set; }
        public string? compensation_payment_cancellation_refund_content { get; private set; }
        public bool compensation_payment_cancellation_refund_enabled { get; private set; }
        public bool compensation_payment_cancellation_refund_advanced_email_enabled { get; private set; }
        public bool compensation_payment_cancellation_refund_from_customer_origin_enabled { get; private set; }
        public bool compensation_payment_cancellation_refund_from_agent_origin_enabled { get; private set; }

        public string? compensation_payment_cancellation_wire_transfer_subject { get; private set; }
        public string? compensation_payment_cancellation_wire_transfer_content { get; private set; }
        public bool compensation_payment_cancellation_wire_transfer_enabled { get; private set; }
        public bool compensation_payment_cancellation_wire_transfer_advanced_email_enabled { get; private set; }
        public bool compensation_payment_cancellation_wire_transfer_from_customer_origin_enabled { get; private set; }
        public bool compensation_payment_cancellation_wire_transfer_from_agent_origin_enabled { get; private set; }

        public string? compensation_payment_occurrence_gift_card_subject { get; private set; }
        public string? compensation_payment_occurrence_gift_card_content { get; private set; }
        public bool compensation_payment_occurrence_gift_card_enabled { get; private set; }
        public bool compensation_payment_occurrence_gift_card_advanced_email_enabled { get; private set; }
        public bool compensation_payment_occurrence_gift_card_from_customer_origin_enabled { get; private set; }
        public bool compensation_payment_occurrence_gift_card_from_agent_origin_enabled { get; private set; }

        public string? compensation_payment_occurrence_refund_subject { get; private set; }
        public string? compensation_payment_occurrence_refund_content { get; private set; }
        public bool compensation_payment_occurrence_refund_enabled { get; private set; }
        public bool compensation_payment_occurrence_refund_advanced_email_enabled { get; private set; }
        public bool compensation_payment_occurrence_refund_from_customer_origin_enabled { get; private set; }
        public bool compensation_payment_occurrence_refund_from_agent_origin_enabled { get; private set; }

        public string? compensation_payment_occurrence_wire_transfer_subject { get; private set; }
        public string? compensation_payment_occurrence_wire_transfer_content { get; private set; }
        public bool compensation_payment_occurrence_wire_transfer_enabled { get; private set; }
        public bool compensation_payment_occurrence_wire_transfer_advanced_email_enabled { get; private set; }
        public bool compensation_payment_occurrence_wire_transfer_from_customer_origin_enabled { get; private set; }
        public bool compensation_payment_occurrence_wire_transfer_from_agent_origin_enabled { get; private set; }

        public string? compensation_wire_transfer_ask_transfer_bank_account_info_subject { get; private set; }
        public string? compensation_wire_transfer_ask_transfer_bank_account_info_content { get; private set; }
        public bool compensation_wire_transfer_ask_transfer_bank_account_info_enabled { get; private set; }
        public bool compensation_wire_transfer_ask_transfer_bank_account_info_advanced_email_enabled { get; private set; }
        public bool compensation_wire_transfer_ask_transfer_bank_account_info_from_customer_origin_enabled { get; private set; }
        public bool compensation_wire_transfer_ask_transfer_bank_account_info_from_agent_origin_enabled { get; private set; }

        public MessageMail() { }

        public MessageMail(Dtos.Me.Mail mailDto, int id, int ecommerce_id)
        {
            this.integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.id = id;
            this.ecommerce_id = ecommerce_id;

            greeting_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.greeting?.advancedEmailEnabled);
            footer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.footer?.advancedEmailEnabled);
            reverse_invalid_courier_contract_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.invalid_courier_contract?.enabled);
            reverse_invalid_courier_contract_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.invalid_courier_contract?.advancedEmailEnabled);
            reverse_invalid_courier_contract_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.invalid_courier_contract?.fromCustomerOriginEnabled);
            reverse_invalid_courier_contract_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.invalid_courier_contract?.fromAgentOriginEnabled);
            reverse_awaiting_with_freight_by_customer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_with_freight_by_customer?.enabled);
            reverse_awaiting_with_freight_by_customer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_with_freight_by_customer?.advancedEmailEnabled);
            reverse_awaiting_with_freight_by_customer_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_with_freight_by_customer?.fromCustomerOriginEnabled);
            reverse_awaiting_with_freight_by_customer_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_with_freight_by_customer?.fromAgentOriginEnabled);
            reverse_pending_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.pending?.enabled);
            reverse_pending_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.pending?.advancedEmailEnabled);
            reverse_pending_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.pending?.fromCustomerOriginEnabled);
            reverse_pending_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.pending?.fromAgentOriginEnabled);
            reverse_send4_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.send4_code_generated?.enabled);
            reverse_send4_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.send4_code_generated?.advancedEmailEnabled);
            reverse_send4_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.send4_code_generated?.fromCustomerOriginEnabled);
            reverse_send4_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.send4_code_generated?.fromAgentOriginEnabled);
            reverse_correio_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_generated?.enabled);
            reverse_correio_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_generated?.advancedEmailEnabled);
            reverse_correio_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_generated?.fromCustomerOriginEnabled);
            reverse_correio_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_generated?.fromAgentOriginEnabled);
            reverse_manual_modal_change_correio_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated?.enabled);
            reverse_manual_modal_change_correio_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated?.advancedEmailEnabled);
            reverse_manual_modal_change_correio_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated?.fromCustomerOriginEnabled);
            reverse_manual_modal_change_correio_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated?.fromAgentOriginEnabled);
            reverse_manual_modal_change_correio_code_generated_collect_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.enabled);
            reverse_manual_modal_change_correio_code_generated_collect_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.advancedEmailEnabled);
            reverse_manual_modal_change_correio_code_generated_collect_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.fromCustomerOriginEnabled);
            reverse_manual_modal_change_correio_code_generated_collect_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.fromAgentOriginEnabled);
            reverse_estafeta_post_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.estafeta_post_code_generated?.enabled);
            reverse_estafeta_post_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.estafeta_post_code_generated?.advancedEmailEnabled);
            reverse_estafeta_post_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.estafeta_post_code_generated?.fromCustomerOriginEnabled);
            reverse_estafeta_post_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.estafeta_post_code_generated?.fromAgentOriginEnabled);
            reverse_courier_collect_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_code_generated?.enabled);
            reverse_courier_collect_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_code_generated?.advancedEmailEnabled);
            reverse_courier_collect_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_code_generated?.fromCustomerOriginEnabled);
            reverse_courier_collect_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_code_generated?.fromAgentOriginEnabled);
            reverse_courier_collect_scheduling_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_scheduling?.enabled);
            reverse_courier_collect_scheduling_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_scheduling?.advancedEmailEnabled);
            reverse_courier_collect_scheduling_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_scheduling?.fromCustomerOriginEnabled);
            reverse_courier_collect_scheduling_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.courier_collect_scheduling?.fromAgentOriginEnabled);
            reverse_generic_courier_collect_requested_date_changed_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.enabled);
            reverse_generic_courier_collect_requested_date_changed_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.advancedEmailEnabled);
            reverse_generic_courier_collect_requested_date_changed_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.fromCustomerOriginEnabled);
            reverse_generic_courier_collect_requested_date_changed_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.fromAgentOriginEnabled);
            reverse_locker_post_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.locker_post_code_generated?.enabled);
            reverse_locker_post_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.locker_post_code_generated?.advancedEmailEnabled);
            reverse_locker_post_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.locker_post_code_generated?.fromCustomerOriginEnabled);
            reverse_locker_post_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.locker_post_code_generated?.fromAgentOriginEnabled);
            reverse_correio_code_expiring_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_expiring?.enabled);
            reverse_correio_code_expiring_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_expiring?.advancedEmailEnabled);
            reverse_correio_code_expiring_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_expiring?.fromCustomerOriginEnabled);
            reverse_correio_code_expiring_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_code_expiring?.fromAgentOriginEnabled);
            reverse_correio_collect_change_to_post_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_collect_change_to_post?.enabled);
            reverse_correio_collect_change_to_post_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_collect_change_to_post?.advancedEmailEnabled);
            reverse_correio_collect_change_to_post_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_collect_change_to_post?.fromCustomerOriginEnabled);
            reverse_correio_collect_change_to_post_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_collect_change_to_post?.fromAgentOriginEnabled);
            reverse_correio_tracking_posted_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_posted?.enabled);
            reverse_correio_tracking_posted_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_posted?.advancedEmailEnabled);
            reverse_correio_tracking_posted_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_posted?.fromCustomerOriginEnabled);
            reverse_correio_tracking_posted_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_posted?.fromAgentOriginEnabled);
            reverse_correio_tracking_delivered_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_delivered?.enabled);
            reverse_correio_tracking_delivered_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_delivered?.advancedEmailEnabled);
            reverse_correio_tracking_delivered_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_delivered?.fromCustomerOriginEnabled);
            reverse_correio_tracking_delivered_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.correio_tracking_delivered?.fromAgentOriginEnabled);
            reverse_ecommerce_store_reverse_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.ecommerce_store_reverse?.enabled);
            reverse_ecommerce_store_reverse_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.ecommerce_store_reverse?.advancedEmailEnabled);
            reverse_ecommerce_store_reverse_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.ecommerce_store_reverse?.fromCustomerOriginEnabled);
            reverse_ecommerce_store_reverse_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.ecommerce_store_reverse?.fromAgentOriginEnabled);
            reverse_products_received_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.products_received?.enabled);
            reverse_products_received_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.products_received?.advancedEmailEnabled);
            reverse_products_received_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.products_received?.fromCustomerOriginEnabled);
            reverse_products_received_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.products_received?.fromAgentOriginEnabled);
            reverse_reverse_refused_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_refused?.enabled);
            reverse_reverse_refused_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_refused?.advancedEmailEnabled);
            reverse_reverse_refused_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_refused?.fromCustomerOriginEnabled);
            reverse_reverse_refused_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_refused?.fromAgentOriginEnabled);
            reverse_reverse_canceled_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_canceled?.enabled);
            reverse_reverse_canceled_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_canceled?.advancedEmailEnabled);
            reverse_reverse_canceled_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_canceled?.fromCustomerOriginEnabled);
            reverse_reverse_canceled_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_canceled?.fromAgentOriginEnabled);
            reverse_reverse_reopened_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_reopened?.enabled);
            reverse_reverse_reopened_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_reopened?.advancedEmailEnabled);
            reverse_reverse_reopened_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_reopened?.fromCustomerOriginEnabled);
            reverse_reverse_reopened_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_reopened?.fromAgentOriginEnabled);
            reverse_reverse_returned_invoice_solicitation_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation?.enabled);
            reverse_reverse_returned_invoice_solicitation_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation?.advancedEmailEnabled);
            reverse_reverse_returned_invoice_solicitation_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation?.fromCustomerOriginEnabled);
            reverse_reverse_returned_invoice_solicitation_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation?.fromAgentOriginEnabled);
            reverse_reverse_returned_invoice_solicitation_partially_missing_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.enabled);
            reverse_reverse_returned_invoice_solicitation_partially_missing_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.advancedEmailEnabled);
            reverse_reverse_returned_invoice_solicitation_partially_missing_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.fromCustomerOriginEnabled);
            reverse_reverse_returned_invoice_solicitation_partially_missing_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.fromAgentOriginEnabled);
            reverse_awaiting_without_process_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_without_process?.enabled);
            reverse_awaiting_without_process_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_without_process?.advancedEmailEnabled);
            reverse_awaiting_without_process_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_without_process?.fromCustomerOriginEnabled);
            reverse_awaiting_without_process_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_without_process?.fromAgentOriginEnabled);
            reverse_reverse_with_manual_processing_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_with_manual_processing?.enabled);
            reverse_reverse_with_manual_processing_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_with_manual_processing?.advancedEmailEnabled);
            reverse_reverse_with_manual_processing_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_with_manual_processing?.fromCustomerOriginEnabled);
            reverse_reverse_with_manual_processing_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.reverse_with_manual_processing?.fromAgentOriginEnabled);
            reverse_generic_courier_collect_info_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_info?.enabled);
            reverse_generic_courier_collect_info_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_info?.advancedEmailEnabled);
            reverse_generic_courier_collect_info_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_info?.fromCustomerOriginEnabled);
            reverse_generic_courier_collect_info_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.generic_courier_collect_info?.fromAgentOriginEnabled);
            reverse_return_point_code_generated_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_generated?.enabled);
            reverse_return_point_code_generated_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_generated?.advancedEmailEnabled);
            reverse_return_point_code_generated_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_generated?.fromCustomerOriginEnabled);
            reverse_return_point_code_generated_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_generated?.fromAgentOriginEnabled);
            reverse_return_point_code_expiring_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_expiring?.enabled);
            reverse_return_point_code_expiring_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_expiring?.advancedEmailEnabled);
            reverse_return_point_code_expiring_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_expiring?.fromCustomerOriginEnabled);
            reverse_return_point_code_expiring_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.return_point_code_expiring?.fromAgentOriginEnabled);
            reverse_awaiting_collect_scheduling_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling?.enabled);
            reverse_awaiting_collect_scheduling_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling?.advancedEmailEnabled);
            reverse_awaiting_collect_scheduling_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling?.fromCustomerOriginEnabled);
            reverse_awaiting_collect_scheduling_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling?.fromAgentOriginEnabled);
            reverse_awaiting_collect_scheduling_reminder_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling_reminder?.enabled);
            reverse_awaiting_collect_scheduling_reminder_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling_reminder?.advancedEmailEnabled);
            reverse_awaiting_collect_scheduling_reminder_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling_reminder?.fromCustomerOriginEnabled);
            reverse_awaiting_collect_scheduling_reminder_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.reverse?.awaiting_collect_scheduling_reminder?.fromAgentOriginEnabled);
            refund_approved_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.approved?.enabled);
            refund_approved_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.approved?.advancedEmailEnabled);
            refund_approved_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.approved?.fromCustomerOriginEnabled);
            refund_approved_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.approved?.fromAgentOriginEnabled);
            refund_paid_refund_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_refund?.enabled);
            refund_paid_refund_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_refund?.advancedEmailEnabled);
            refund_paid_refund_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_refund?.fromCustomerOriginEnabled);
            refund_paid_refund_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_refund?.fromAgentOriginEnabled);
            refund_refund_nps_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.refund_nps?.enabled);
            refund_refund_nps_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.refund_nps?.advancedEmailEnabled);
            refund_refund_nps_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.refund_nps?.fromCustomerOriginEnabled);
            refund_refund_nps_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.refund_nps?.fromAgentOriginEnabled);
            refund_paid_wire_transfer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_wire_transfer?.enabled);
            refund_paid_wire_transfer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_wire_transfer?.advancedEmailEnabled);
            refund_paid_wire_transfer_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_wire_transfer?.fromCustomerOriginEnabled);
            refund_paid_wire_transfer_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_wire_transfer?.fromAgentOriginEnabled);
            refund_paid_product_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_product?.enabled);
            refund_paid_product_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_product?.advancedEmailEnabled);
            refund_paid_product_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_product?.fromCustomerOriginEnabled);
            refund_paid_product_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_product?.fromAgentOriginEnabled);
            refund_paid_voucher_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_voucher?.enabled);
            refund_paid_voucher_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_voucher?.advancedEmailEnabled);
            refund_paid_voucher_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_voucher?.fromCustomerOriginEnabled);
            refund_paid_voucher_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_voucher?.fromAgentOriginEnabled);
            refund_paid_credit_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_credit?.enabled);
            refund_paid_credit_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_credit?.advancedEmailEnabled);
            refund_paid_credit_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_credit?.fromCustomerOriginEnabled);
            refund_paid_credit_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_credit?.fromAgentOriginEnabled);
            refund_ask_wire_transfer_data_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data?.enabled);
            refund_ask_wire_transfer_data_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data?.advancedEmailEnabled);
            refund_ask_wire_transfer_data_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data?.fromCustomerOriginEnabled);
            refund_ask_wire_transfer_data_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data?.fromAgentOriginEnabled);
            refund_paid_hard_retention_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_hard_retention?.enabled);
            refund_paid_hard_retention_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_hard_retention?.advancedEmailEnabled);
            refund_paid_hard_retention_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_hard_retention?.fromCustomerOriginEnabled);
            refund_paid_hard_retention_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.paid_hard_retention?.fromAgentOriginEnabled);
            refund_ask_wire_transfer_data_update_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data_update?.enabled);
            refund_ask_wire_transfer_data_update_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data_update?.advancedEmailEnabled);
            refund_ask_wire_transfer_data_update_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data_update?.fromCustomerOriginEnabled);
            refund_ask_wire_transfer_data_update_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.ask_wire_transfer_data_update?.fromAgentOriginEnabled);
            refund_review_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.review?.enabled);
            refund_review_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.review?.advancedEmailEnabled);
            refund_review_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.review?.fromCustomerOriginEnabled);
            refund_review_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.refund?.review?.fromAgentOriginEnabled);
            retention_disabled_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.retention?.retention_disabled?.enabled);
            retention_disabled_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.retention?.retention_disabled?.advancedEmailEnabled);
            retention_disabled_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.retention?.retention_disabled?.fromCustomerOriginEnabled);
            retention_disabled_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.retention?.retention_disabled?.fromAgentOriginEnabled);
            partials_partially_received_warning_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.partials?.partially_received_warning?.advancedEmailEnabled);
            partials_new_products_include_info_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.partials?.new_products_include_info?.advancedEmailEnabled);
            occurrence_awaiting_customer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer?.enabled);
            occurrence_awaiting_customer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer?.advancedEmailEnabled);
            occurrence_awaiting_customer_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer?.fromCustomerOriginEnabled);
            occurrence_awaiting_customer_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer?.fromAgentOriginEnabled);
            occurrence_awaiting_customer_remind_deadline_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer_remind_deadline?.enabled);
            occurrence_awaiting_customer_remind_deadline_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer_remind_deadline?.advancedEmailEnabled);
            occurrence_awaiting_customer_remind_deadline_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer_remind_deadline?.fromCustomerOriginEnabled);
            occurrence_awaiting_customer_remind_deadline_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.awaiting_customer_remind_deadline?.fromAgentOriginEnabled);
            occurrence_created_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.created?.enabled);
            occurrence_created_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.created?.advancedEmailEnabled);
            occurrence_created_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.created?.fromCustomerOriginEnabled);
            occurrence_created_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.occurrence?.created?.fromAgentOriginEnabled);
            cancellation_failed_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.failed?.enabled);
            cancellation_failed_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.failed?.advancedEmailEnabled);
            cancellation_failed_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.failed?.fromCustomerOriginEnabled);
            cancellation_failed_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.failed?.fromAgentOriginEnabled);
            cancellation_fully_accepted_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.fully?.enabled);
            cancellation_fully_accepted_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.fully?.advancedEmailEnabled);
            cancellation_fully_accepted_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.fully?.fromCustomerOriginEnabled);
            cancellation_fully_accepted_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.fully?.fromAgentOriginEnabled);
            cancellation_partially_accepted_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.partially?.enabled);
            cancellation_partially_accepted_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.partially?.advancedEmailEnabled);
            cancellation_partially_accepted_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.partially?.fromCustomerOriginEnabled);
            cancellation_partially_accepted_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.partially?.fromAgentOriginEnabled);
            cancellation_refused_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.refused?.enabled);
            cancellation_refused_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.refused?.advancedEmailEnabled);
            cancellation_refused_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.refused?.fromCustomerOriginEnabled);
            cancellation_refused_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.refused?.fromAgentOriginEnabled);
            cancellation_created_customer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer?.enabled);
            cancellation_created_customer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer?.advancedEmailEnabled);
            cancellation_created_customer_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer?.fromCustomerOriginEnabled);
            cancellation_created_customer_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer?.fromAgentOriginEnabled);
            cancellation_created_agent_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.agent?.enabled);
            cancellation_created_agent_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.agent?.advancedEmailEnabled);
            cancellation_created_agent_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.agent?.fromCustomerOriginEnabled);
            cancellation_created_agent_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.agent?.fromAgentOriginEnabled);
            cancellation_created_customer_by_agent_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer_by_agent?.enabled);
            cancellation_created_customer_by_agent_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer_by_agent?.advancedEmailEnabled);
            cancellation_created_customer_by_agent_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer_by_agent?.fromCustomerOriginEnabled);
            cancellation_created_customer_by_agent_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.cancellation?.created?.customer_by_agent?.fromAgentOriginEnabled);
            compensation_payment_cancellation_gift_card_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.gift_card?.enabled);
            compensation_payment_cancellation_gift_card_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.gift_card?.advancedEmailEnabled);
            compensation_payment_cancellation_gift_card_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.gift_card?.fromCustomerOriginEnabled);
            compensation_payment_cancellation_gift_card_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.gift_card?.fromAgentOriginEnabled);
            compensation_payment_cancellation_refund_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.refund?.enabled);
            compensation_payment_cancellation_refund_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.refund?.advancedEmailEnabled);
            compensation_payment_cancellation_refund_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.refund?.fromCustomerOriginEnabled);
            compensation_payment_cancellation_refund_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.refund?.fromAgentOriginEnabled);
            compensation_payment_cancellation_wire_transfer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.wire_transfer?.enabled);
            compensation_payment_cancellation_wire_transfer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.wire_transfer?.advancedEmailEnabled);
            compensation_payment_cancellation_wire_transfer_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.wire_transfer?.fromCustomerOriginEnabled);
            compensation_payment_cancellation_wire_transfer_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.cancellation?.wire_transfer?.fromAgentOriginEnabled);
            compensation_payment_occurrence_gift_card_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.gift_card?.enabled);
            compensation_payment_occurrence_gift_card_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.gift_card?.advancedEmailEnabled);
            compensation_payment_occurrence_gift_card_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.gift_card?.fromCustomerOriginEnabled);
            compensation_payment_occurrence_gift_card_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.gift_card?.fromAgentOriginEnabled);
            compensation_payment_occurrence_refund_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.refund?.enabled);
            compensation_payment_occurrence_refund_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.refund?.advancedEmailEnabled);
            compensation_payment_occurrence_refund_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.refund?.fromCustomerOriginEnabled);
            compensation_payment_occurrence_refund_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.refund?.fromAgentOriginEnabled);
            compensation_payment_occurrence_wire_transfer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.wire_transfer?.enabled);
            compensation_payment_occurrence_wire_transfer_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.wire_transfer?.advancedEmailEnabled);
            compensation_payment_occurrence_wire_transfer_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.wire_transfer?.fromCustomerOriginEnabled);
            compensation_payment_occurrence_wire_transfer_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.payment?.occurrence?.wire_transfer?.fromAgentOriginEnabled);
            compensation_wire_transfer_ask_transfer_bank_account_info_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.enabled);
            compensation_wire_transfer_ask_transfer_bank_account_info_advanced_email_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.advancedEmailEnabled);
            compensation_wire_transfer_ask_transfer_bank_account_info_from_customer_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.fromCustomerOriginEnabled);
            compensation_wire_transfer_ask_transfer_bank_account_info_from_agent_origin_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.fromAgentOriginEnabled);

            greeting_content = mailDto.greeting?.content?.Length > 800 ? mailDto.greeting?.content?.Substring(0, 800) : mailDto.greeting?.content;
            footer_content = mailDto.footer?.content?.Length > 800 ? mailDto.footer?.content?.Substring(0, 800) : mailDto.footer?.content;
            reverse_invalid_courier_contract_content = mailDto.reverse?.invalid_courier_contract?.content?.Length > 800 ? mailDto.reverse?.invalid_courier_contract?.content?.Substring(0, 800) : mailDto.reverse?.invalid_courier_contract?.content;
            reverse_awaiting_with_freight_by_customer_subject = mailDto.reverse?.awaiting_with_freight_by_customer?.subject?.Length > 800 ? mailDto.reverse?.awaiting_with_freight_by_customer?.subject?.Substring(0, 800) : mailDto.reverse?.awaiting_with_freight_by_customer?.subject;
            reverse_awaiting_with_freight_by_customer_content = mailDto.reverse?.awaiting_with_freight_by_customer?.content?.Length > 800 ? mailDto.reverse?.awaiting_with_freight_by_customer?.content?.Substring(0, 800) : mailDto.reverse?.awaiting_with_freight_by_customer?.content;
            reverse_pending_subject = mailDto.reverse?.pending?.subject?.Length > 800 ? mailDto.reverse?.pending?.subject?.Substring(0, 800) : mailDto.reverse?.pending?.subject;
            reverse_pending_content = mailDto.reverse?.pending?.content?.Length > 800 ? mailDto.reverse?.pending?.content?.Substring(0, 800) : mailDto.reverse?.pending?.content;
            reverse_send4_code_generated_subject = mailDto.reverse?.send4_code_generated?.subject?.Length > 800 ? mailDto.reverse?.send4_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.send4_code_generated?.subject;
            reverse_send4_code_generated_content = mailDto.reverse?.send4_code_generated?.content?.Length > 800 ? mailDto.reverse?.send4_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.send4_code_generated?.content;
            reverse_correio_code_generated_subject = mailDto.reverse?.correio_code_generated?.subject?.Length > 800 ? mailDto.reverse?.correio_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.correio_code_generated?.subject;
            reverse_correio_code_generated_content = mailDto.reverse?.correio_code_generated?.content?.Length > 800 ? mailDto.reverse?.correio_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.correio_code_generated?.content;
            reverse_manual_modal_change_correio_code_generated_subject = mailDto.reverse?.manual_modal_change_correio_code_generated?.subject?.Length > 800 ? mailDto.reverse?.manual_modal_change_correio_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.manual_modal_change_correio_code_generated?.subject;
            reverse_manual_modal_change_correio_code_generated_content = mailDto.reverse?.manual_modal_change_correio_code_generated?.content?.Length > 800 ? mailDto.reverse?.manual_modal_change_correio_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.manual_modal_change_correio_code_generated?.content;
            reverse_manual_modal_change_correio_code_generated_collect_subject = mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.subject?.Length > 800 ? mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.subject?.Substring(0, 800) : mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.subject;
            reverse_manual_modal_change_correio_code_generated_collect_content = mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.content?.Length > 800 ? mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.content?.Substring(0, 800) : mailDto.reverse?.manual_modal_change_correio_code_generated_collect?.content;
            reverse_estafeta_post_code_generated_subject = mailDto.reverse?.estafeta_post_code_generated?.subject?.Length > 800 ? mailDto.reverse?.estafeta_post_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.estafeta_post_code_generated?.subject;
            reverse_estafeta_post_code_generated_content = mailDto.reverse?.estafeta_post_code_generated?.content?.Length > 800 ? mailDto.reverse?.estafeta_post_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.estafeta_post_code_generated?.content;
            reverse_courier_collect_code_generated_subject = mailDto.reverse?.courier_collect_code_generated?.subject?.Length > 800 ? mailDto.reverse?.courier_collect_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.courier_collect_code_generated?.subject;
            reverse_courier_collect_code_generated_content = mailDto.reverse?.courier_collect_code_generated?.content?.Length > 800 ? mailDto.reverse?.courier_collect_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.courier_collect_code_generated?.content;
            reverse_courier_collect_scheduling_subject = mailDto.reverse?.courier_collect_scheduling?.subject?.Length > 800 ? mailDto.reverse?.courier_collect_scheduling?.subject?.Substring(0, 800) : mailDto.reverse?.courier_collect_scheduling?.subject;
            reverse_courier_collect_scheduling_content = mailDto.reverse?.courier_collect_scheduling?.content?.Length > 800 ? mailDto.reverse?.courier_collect_scheduling?.content?.Substring(0, 800) : mailDto.reverse?.courier_collect_scheduling?.content;
            reverse_generic_courier_collect_requested_date_changed_code_generated_subject = mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.subject?.Length > 800 ? mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.subject;
            reverse_generic_courier_collect_requested_date_changed_code_generated_content = mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.content?.Length > 800 ? mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.generic_courier_collect_requested_date_changed_code_generated?.content;
            reverse_locker_post_code_generated_subject = mailDto.reverse?.locker_post_code_generated?.subject?.Length > 800 ? mailDto.reverse?.locker_post_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.locker_post_code_generated?.subject;
            reverse_locker_post_code_generated_content = mailDto.reverse?.locker_post_code_generated?.content?.Length > 800 ? mailDto.reverse?.locker_post_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.locker_post_code_generated?.content;
            reverse_correio_code_expiring_subject = mailDto.reverse?.correio_code_expiring?.subject?.Length > 800 ? mailDto.reverse?.correio_code_expiring?.subject?.Substring(0, 800) : mailDto.reverse?.correio_code_expiring?.subject;
            reverse_correio_code_expiring_content = mailDto.reverse?.correio_code_expiring?.content?.Length > 800 ? mailDto.reverse?.correio_code_expiring?.content?.Substring(0, 800) : mailDto.reverse?.correio_code_expiring?.content;
            reverse_correio_collect_change_to_post_subject = mailDto.reverse?.correio_collect_change_to_post?.subject?.Length > 800 ? mailDto.reverse?.correio_collect_change_to_post?.subject?.Substring(0, 800) : mailDto.reverse?.correio_collect_change_to_post?.subject;
            reverse_correio_collect_change_to_post_content = mailDto.reverse?.correio_collect_change_to_post?.content?.Length > 800 ? mailDto.reverse?.correio_collect_change_to_post?.content?.Substring(0, 800) : mailDto.reverse?.correio_collect_change_to_post?.content;
            reverse_correio_tracking_posted_subject = mailDto.reverse?.correio_tracking_posted?.subject?.Length > 800 ? mailDto.reverse?.correio_tracking_posted?.subject?.Substring(0, 800) : mailDto.reverse?.correio_tracking_posted?.subject;
            reverse_correio_tracking_posted_content = mailDto.reverse?.correio_tracking_posted?.content?.Length > 800 ? mailDto.reverse?.correio_tracking_posted?.content?.Substring(0, 800) : mailDto.reverse?.correio_tracking_posted?.content;
            reverse_correio_tracking_delivered_subject = mailDto.reverse?.correio_tracking_delivered?.subject?.Length > 800 ? mailDto.reverse?.correio_tracking_delivered?.subject?.Substring(0, 800) : mailDto.reverse?.correio_tracking_delivered?.subject;
            reverse_correio_tracking_delivered_content = mailDto.reverse?.correio_tracking_delivered?.content?.Length > 800 ? mailDto.reverse?.correio_tracking_delivered?.content?.Substring(0, 800) : mailDto.reverse?.correio_tracking_delivered?.content;
            reverse_ecommerce_store_reverse_subject = mailDto.reverse?.ecommerce_store_reverse?.subject?.Length > 800 ? mailDto.reverse?.ecommerce_store_reverse?.subject?.Substring(0, 800) : mailDto.reverse?.ecommerce_store_reverse?.subject;
            reverse_ecommerce_store_reverse_content = mailDto.reverse?.ecommerce_store_reverse?.content?.Length > 800 ? mailDto.reverse?.ecommerce_store_reverse?.content?.Substring(0, 800) : mailDto.reverse?.ecommerce_store_reverse?.content;
            reverse_products_received_subject = mailDto.reverse?.products_received?.subject?.Length > 800 ? mailDto.reverse?.products_received?.subject?.Substring(0, 800) : mailDto.reverse?.products_received?.subject;
            reverse_products_received_content = mailDto.reverse?.products_received?.content?.Length > 800 ? mailDto.reverse?.products_received?.content?.Substring(0, 800) : mailDto.reverse?.products_received?.content;
            reverse_reverse_refused_subject = mailDto.reverse?.reverse_refused?.subject?.Length > 800 ? mailDto.reverse?.reverse_refused?.subject?.Substring(0, 800) : mailDto.reverse?.reverse_refused?.subject;
            reverse_reverse_refused_content = mailDto.reverse?.reverse_refused?.content?.Length > 800 ? mailDto.reverse?.reverse_refused?.content?.Substring(0, 800) : mailDto.reverse?.reverse_refused?.content;
            reverse_reverse_canceled_subject = mailDto.reverse?.reverse_canceled?.subject?.Length > 800 ? mailDto.reverse?.reverse_canceled?.subject?.Substring(0, 800) : mailDto.reverse?.reverse_canceled?.subject;
            reverse_reverse_canceled_content = mailDto.reverse?.reverse_canceled?.content?.Length > 800 ? mailDto.reverse?.reverse_canceled?.content?.Substring(0, 800) : mailDto.reverse?.reverse_canceled?.content;
            reverse_reverse_reopened_subject = mailDto.reverse?.reverse_reopened?.subject?.Length > 800 ? mailDto.reverse?.reverse_reopened?.subject?.Substring(0, 800) : mailDto.reverse?.reverse_reopened?.subject;
            reverse_reverse_reopened_content = mailDto.reverse?.reverse_reopened?.content?.Length > 800 ? mailDto.reverse?.reverse_reopened?.content?.Substring(0, 800) : mailDto.reverse?.reverse_reopened?.content;
            reverse_reverse_returned_invoice_solicitation_subject = mailDto.reverse?.reverse_returned_invoice_solicitation?.subject?.Length > 800 ? mailDto.reverse?.reverse_returned_invoice_solicitation?.subject?.Substring(0, 800) : mailDto.reverse?.reverse_returned_invoice_solicitation?.subject;
            reverse_reverse_returned_invoice_solicitation_content = mailDto.reverse?.reverse_returned_invoice_solicitation?.content?.Length > 800 ? mailDto.reverse?.reverse_returned_invoice_solicitation?.content?.Substring(0, 800) : mailDto.reverse?.reverse_returned_invoice_solicitation?.content;
            reverse_reverse_returned_invoice_solicitation_partially_missing_subject = mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.subject?.Length > 800 ? mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.subject?.Substring(0, 800) : mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.subject;
            reverse_reverse_returned_invoice_solicitation_partially_missing_content = mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.content?.Length > 800 ? mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.content?.Substring(0, 800) : mailDto.reverse?.reverse_returned_invoice_solicitation_partially_missing?.content;
            reverse_awaiting_without_process_subject = mailDto.reverse?.awaiting_without_process?.subject?.Length > 800 ? mailDto.reverse?.awaiting_without_process?.subject?.Substring(0, 800) : mailDto.reverse?.awaiting_without_process?.subject;
            reverse_awaiting_without_process_content = mailDto.reverse?.awaiting_without_process?.content?.Length > 800 ? mailDto.reverse?.awaiting_without_process?.content?.Substring(0, 800) : mailDto.reverse?.awaiting_without_process?.content;
            reverse_reverse_with_manual_processing_subject = mailDto.reverse?.reverse_with_manual_processing?.subject?.Length > 800 ? mailDto.reverse?.reverse_with_manual_processing?.subject?.Substring(0, 800) : mailDto.reverse?.reverse_with_manual_processing?.subject;
            reverse_reverse_with_manual_processing_content = mailDto.reverse?.reverse_with_manual_processing?.content?.Length > 800 ? mailDto.reverse?.reverse_with_manual_processing?.content?.Substring(0, 800) : mailDto.reverse?.reverse_with_manual_processing?.content;
            reverse_generic_courier_collect_info_subject = mailDto.reverse?.generic_courier_collect_info?.subject?.Length > 800 ? mailDto.reverse?.generic_courier_collect_info?.subject?.Substring(0, 800) : mailDto.reverse?.generic_courier_collect_info?.subject;
            reverse_generic_courier_collect_info_content = mailDto.reverse?.generic_courier_collect_info?.content?.Length > 800 ? mailDto.reverse?.generic_courier_collect_info?.content?.Substring(0, 800) : mailDto.reverse?.generic_courier_collect_info?.content;
            reverse_return_point_code_generated_subject = mailDto.reverse?.return_point_code_generated?.subject?.Length > 800 ? mailDto.reverse?.return_point_code_generated?.subject?.Substring(0, 800) : mailDto.reverse?.return_point_code_generated?.subject;
            reverse_return_point_code_generated_content = mailDto.reverse?.return_point_code_generated?.content?.Length > 800 ? mailDto.reverse?.return_point_code_generated?.content?.Substring(0, 800) : mailDto.reverse?.return_point_code_generated?.content;
            reverse_return_point_code_expiring_subject = mailDto.reverse?.return_point_code_expiring?.subject?.Length > 800 ? mailDto.reverse?.return_point_code_expiring?.subject?.Substring(0, 800) : mailDto.reverse?.return_point_code_expiring?.subject;
            reverse_return_point_code_expiring_content = mailDto.reverse?.return_point_code_expiring?.content?.Length > 800 ? mailDto.reverse?.return_point_code_expiring?.content?.Substring(0, 800) : mailDto.reverse?.return_point_code_expiring?.content;
            reverse_awaiting_collect_scheduling_subject = mailDto.reverse?.awaiting_collect_scheduling?.subject?.Length > 800 ? mailDto.reverse?.awaiting_collect_scheduling?.subject?.Substring(0, 800) : mailDto.reverse?.awaiting_collect_scheduling?.subject;
            reverse_awaiting_collect_scheduling_content = mailDto.reverse?.awaiting_collect_scheduling?.content?.Length > 800 ? mailDto.reverse?.awaiting_collect_scheduling?.content?.Substring(0, 800) : mailDto.reverse?.awaiting_collect_scheduling?.content;
            reverse_awaiting_collect_scheduling_reminder_subject = mailDto.reverse?.awaiting_collect_scheduling_reminder?.subject?.Length > 800 ? mailDto.reverse?.awaiting_collect_scheduling_reminder?.subject?.Substring(0, 800) : mailDto.reverse?.awaiting_collect_scheduling_reminder?.subject;
            reverse_awaiting_collect_scheduling_reminder_content = mailDto.reverse?.awaiting_collect_scheduling_reminder?.content?.Length > 800 ? mailDto.reverse?.awaiting_collect_scheduling_reminder?.content?.Substring(0, 800) : mailDto.reverse?.awaiting_collect_scheduling_reminder?.content;
            refund_approved_subject = mailDto.refund?.approved?.subject?.Length > 800 ? mailDto.refund?.approved?.subject?.Substring(0, 800) : mailDto.refund?.approved?.subject;
            refund_approved_content = mailDto.refund?.approved?.content?.Length > 800 ? mailDto.refund?.approved?.content?.Substring(0, 800) : mailDto.refund?.approved?.content;
            refund_paid_refund_subject = mailDto.refund?.paid_refund?.subject?.Length > 800 ? mailDto.refund?.paid_refund?.subject?.Substring(0, 800) : mailDto.refund?.paid_refund?.subject;
            refund_paid_refund_content = mailDto.refund?.paid_refund?.content?.Length > 800 ? mailDto.refund?.paid_refund?.content?.Substring(0, 800) : mailDto.refund?.paid_refund?.content;
            refund_refund_nps_subject = mailDto.refund?.refund_nps?.subject?.Length > 800 ? mailDto.refund?.refund_nps?.subject?.Substring(0, 800) : mailDto.refund?.refund_nps?.subject;
            refund_refund_nps_content = mailDto.refund?.refund_nps?.content?.Length > 800 ? mailDto.refund?.refund_nps?.content?.Substring(0, 800) : mailDto.refund?.refund_nps?.content;
            refund_paid_wire_transfer_subject = mailDto.refund?.paid_wire_transfer?.subject?.Length > 800 ? mailDto.refund?.paid_wire_transfer?.subject?.Substring(0, 800) : mailDto.refund?.paid_wire_transfer?.subject;
            refund_paid_wire_transfer_content = mailDto.refund?.paid_wire_transfer?.content?.Length > 800 ? mailDto.refund?.paid_wire_transfer?.content?.Substring(0, 800) : mailDto.refund?.paid_wire_transfer?.content;
            refund_paid_product_subject = mailDto.refund?.paid_product?.subject?.Length > 800 ? mailDto.refund?.paid_product?.subject?.Substring(0, 800) : mailDto.refund?.paid_product?.subject;
            refund_paid_product_content = mailDto.refund?.paid_product?.content?.Length > 800 ? mailDto.refund?.paid_product?.content?.Substring(0, 800) : mailDto.refund?.paid_product?.content;
            refund_paid_voucher_subject = mailDto.refund?.paid_voucher?.subject?.Length > 800 ? mailDto.refund?.paid_voucher?.subject?.Substring(0, 800) : mailDto.refund?.paid_voucher?.subject;
            refund_paid_voucher_content = mailDto.refund?.paid_voucher?.content?.Length > 800 ? mailDto.refund?.paid_voucher?.content?.Substring(0, 800) : mailDto.refund?.paid_voucher?.content;
            refund_paid_credit_subject = mailDto.refund?.paid_credit?.subject?.Length > 800 ? mailDto.refund?.paid_credit?.subject?.Substring(0, 800) : mailDto.refund?.paid_credit?.subject;
            refund_paid_credit_content = mailDto.refund?.paid_credit?.content?.Length > 800 ? mailDto.refund?.paid_credit?.content?.Substring(0, 800) : mailDto.refund?.paid_credit?.content;
            refund_ask_wire_transfer_data_subject = mailDto.refund?.ask_wire_transfer_data?.subject?.Length > 800 ? mailDto.refund?.ask_wire_transfer_data?.subject?.Substring(0, 800) : mailDto.refund?.ask_wire_transfer_data?.subject;
            refund_ask_wire_transfer_data_content = mailDto.refund?.ask_wire_transfer_data?.content?.Length > 800 ? mailDto.refund?.ask_wire_transfer_data?.content?.Substring(0, 800) : mailDto.refund?.ask_wire_transfer_data?.content;
            refund_paid_hard_retention_subject = mailDto.refund?.paid_hard_retention?.subject?.Length > 800 ? mailDto.refund?.paid_hard_retention?.subject?.Substring(0, 800) : mailDto.refund?.paid_hard_retention?.subject;
            refund_paid_hard_retention_content = mailDto.refund?.paid_hard_retention?.content?.Length > 800 ? mailDto.refund?.paid_hard_retention?.content?.Substring(0, 800) : mailDto.refund?.paid_hard_retention?.content;
            refund_ask_wire_transfer_data_update_subject = mailDto.refund?.ask_wire_transfer_data_update?.subject?.Length > 800 ? mailDto.refund?.ask_wire_transfer_data_update?.subject?.Substring(0, 800) : mailDto.refund?.ask_wire_transfer_data_update?.subject;
            refund_ask_wire_transfer_data_update_content = mailDto.refund?.ask_wire_transfer_data_update?.content?.Length > 800 ? mailDto.refund?.ask_wire_transfer_data_update?.content?.Substring(0, 800) : mailDto.refund?.ask_wire_transfer_data_update?.content;
            refund_review_subject = mailDto.refund?.review?.subject?.Length > 800 ? mailDto.refund?.review?.subject?.Substring(0, 800) : mailDto.refund?.review?.subject;
            refund_review_content = mailDto.refund?.review?.content?.Length > 800 ? mailDto.refund?.review?.content?.Substring(0, 800) : mailDto.refund?.review?.content;
            retention_disabled_subject = mailDto.retention?.retention_disabled?.subject?.Length > 800 ? mailDto.retention?.retention_disabled?.subject?.Substring(0, 800) : mailDto.retention?.retention_disabled?.subject;
            retention_disabled_content = mailDto.retention?.retention_disabled?.content?.Length > 800 ? mailDto.retention?.retention_disabled?.content?.Substring(0, 800) : mailDto.retention?.retention_disabled?.content;
            partials_partially_received_warning_content = mailDto.partials?.partially_received_warning?.content?.Length > 800 ? mailDto.partials?.partially_received_warning?.content?.Substring(0, 800) : mailDto.partials?.partially_received_warning?.content;
            partials_new_products_include_info_content = mailDto.partials?.new_products_include_info?.content?.Length > 800 ? mailDto.partials?.new_products_include_info?.content?.Substring(0, 800) : mailDto.partials?.new_products_include_info?.content;
            occurrence_awaiting_customer_subject = mailDto.occurrence?.awaiting_customer?.subject?.Length > 800 ? mailDto.occurrence?.awaiting_customer?.subject?.Substring(0, 800) : mailDto.occurrence?.awaiting_customer?.subject;
            occurrence_awaiting_customer_content = mailDto.occurrence?.awaiting_customer?.content?.Length > 800 ? mailDto.occurrence?.awaiting_customer?.content?.Substring(0, 800) : mailDto.occurrence?.awaiting_customer?.content;
            occurrence_awaiting_customer_remind_deadline_subject = mailDto.occurrence?.awaiting_customer_remind_deadline?.subject?.Length > 800 ? mailDto.occurrence?.awaiting_customer_remind_deadline?.subject?.Substring(0, 800) : mailDto.occurrence?.awaiting_customer_remind_deadline?.subject;
            occurrence_awaiting_customer_remind_deadline_content = mailDto.occurrence?.awaiting_customer_remind_deadline?.content?.Length > 800 ? mailDto.occurrence?.awaiting_customer_remind_deadline?.content?.Substring(0, 800) : mailDto.occurrence?.awaiting_customer_remind_deadline?.content;
            occurrence_created_subject = mailDto.occurrence?.created?.subject?.Length > 800 ? mailDto.occurrence?.created?.subject?.Substring(0, 800) : mailDto.occurrence?.created?.subject;
            occurrence_created_content = mailDto.occurrence?.created?.content?.Length > 800 ? mailDto.occurrence?.created?.content?.Substring(0, 800) : mailDto.occurrence?.created?.content;
            cancellation_failed_subject = mailDto.cancellation?.failed?.subject?.Length > 800 ? mailDto.cancellation?.failed?.subject?.Substring(0, 800) : mailDto.cancellation?.failed?.subject;
            cancellation_failed_content = mailDto.cancellation?.failed?.content?.Length > 800 ? mailDto.cancellation?.failed?.content?.Substring(0, 800) : mailDto.cancellation?.failed?.content;
            cancellation_fully_accepted_subject = mailDto.cancellation?.fully?.subject?.Length > 800 ? mailDto.cancellation?.fully?.subject?.Substring(0, 800) : mailDto.cancellation?.fully?.subject;
            cancellation_fully_accepted_content = mailDto.cancellation?.fully?.content?.Length > 800 ? mailDto.cancellation?.fully?.content?.Substring(0, 800) : mailDto.cancellation?.fully?.content;
            cancellation_partially_accepted_subject = mailDto.cancellation?.partially?.subject?.Length > 800 ? mailDto.cancellation?.partially?.subject?.Substring(0, 800) : mailDto.cancellation?.partially?.subject;
            cancellation_partially_accepted_content = mailDto.cancellation?.partially?.content?.Length > 800 ? mailDto.cancellation?.partially?.content?.Substring(0, 800) : mailDto.cancellation?.partially?.content;
            cancellation_refused_subject = mailDto.cancellation?.refused?.subject?.Length > 800 ? mailDto.cancellation?.refused?.subject?.Substring(0, 800) : mailDto.cancellation?.refused?.subject;
            cancellation_refused_content = mailDto.cancellation?.refused?.content?.Length > 800 ? mailDto.cancellation?.refused?.content?.Substring(0, 800) : mailDto.cancellation?.refused?.content;
            cancellation_created_customer_subject = mailDto.cancellation?.created?.customer?.subject?.Length > 800 ? mailDto.cancellation?.created?.customer?.subject?.Substring(0, 800) : mailDto.cancellation?.created?.customer?.subject;
            cancellation_created_customer_content = mailDto.cancellation?.created?.customer?.content?.Length > 800 ? mailDto.cancellation?.created?.customer?.content?.Substring(0, 800) : mailDto.cancellation?.created?.customer?.content;
            cancellation_created_agent_subject = mailDto.cancellation?.created?.agent?.subject?.Length > 800 ? mailDto.cancellation?.created?.agent?.subject?.Substring(0, 800) : mailDto.cancellation?.created?.agent?.subject;
            cancellation_created_agent_content = mailDto.cancellation?.created?.agent?.content?.Length > 800 ? mailDto.cancellation?.created?.agent?.content?.Substring(0, 800) : mailDto.cancellation?.created?.agent?.content;
            cancellation_created_customer_by_agent_subject = mailDto.cancellation?.created?.customer_by_agent?.subject?.Length > 800 ? mailDto.cancellation?.created?.customer_by_agent?.subject?.Substring(0, 800) : mailDto.cancellation?.created?.customer_by_agent?.subject;
            cancellation_created_customer_by_agent_content = mailDto.cancellation?.created?.customer_by_agent?.content?.Length > 800 ? mailDto.cancellation?.created?.customer_by_agent?.content?.Substring(0, 800) : mailDto.cancellation?.created?.customer_by_agent?.content;
            solicitation_partials_requested_products_ref_id = mailDto.solicitation?.partials?.requested_products?.ref_id?.Length > 800 ? mailDto.solicitation?.partials?.requested_products?.ref_id?.Substring(0, 800) : mailDto.solicitation?.partials?.requested_products?.ref_id;
            solicitation_partials_requested_products_sku = mailDto.solicitation?.partials?.requested_products?.sku?.Length > 800 ? mailDto.solicitation?.partials?.requested_products?.sku?.Substring(0, 800) : mailDto.solicitation?.partials?.requested_products?.sku;
            solicitation_partials_requested_products_unity = mailDto.solicitation?.partials?.requested_products?.unity?.Length > 800 ? mailDto.solicitation?.partials?.requested_products?.unity?.Substring(0, 800) : mailDto.solicitation?.partials?.requested_products?.unity;
            solicitation_partials_requested_products_unity_plural = mailDto.solicitation?.partials?.requested_products?.unity_plural?.Length > 800 ? mailDto.solicitation?.partials?.requested_products?.unity_plural?.Substring(0, 800) : mailDto.solicitation?.partials?.requested_products?.unity_plural;
            compensation_payment_cancellation_gift_card_subject = mailDto.compensation?.payment?.cancellation?.gift_card?.subject?.Length > 800 ? mailDto.compensation?.payment?.cancellation?.gift_card?.subject?.Substring(0, 800) : mailDto.compensation?.payment?.cancellation?.gift_card?.subject;
            compensation_payment_cancellation_gift_card_content = mailDto.compensation?.payment?.cancellation?.gift_card?.content?.Length > 800 ? mailDto.compensation?.payment?.cancellation?.gift_card?.content?.Substring(0, 800) : mailDto.compensation?.payment?.cancellation?.gift_card?.content;
            compensation_payment_cancellation_refund_subject = mailDto.compensation?.payment?.cancellation?.refund?.subject?.Length > 800 ? mailDto.compensation?.payment?.cancellation?.refund?.subject?.Substring(0, 800) : mailDto.compensation?.payment?.cancellation?.refund?.subject;
            compensation_payment_cancellation_refund_content = mailDto.compensation?.payment?.cancellation?.refund?.content?.Length > 800 ? mailDto.compensation?.payment?.cancellation?.refund?.content?.Substring(0, 800) : mailDto.compensation?.payment?.cancellation?.refund?.content;
            compensation_payment_cancellation_wire_transfer_subject = mailDto.compensation?.payment?.cancellation?.wire_transfer?.subject?.Length > 800 ? mailDto.compensation?.payment?.cancellation?.wire_transfer?.subject?.Substring(0, 800) : mailDto.compensation?.payment?.cancellation?.wire_transfer?.subject;
            compensation_payment_cancellation_wire_transfer_content = mailDto.compensation?.payment?.cancellation?.wire_transfer?.content?.Length > 800 ? mailDto.compensation?.payment?.cancellation?.wire_transfer?.content?.Substring(0, 800) : mailDto.compensation?.payment?.cancellation?.wire_transfer?.content;
            compensation_payment_occurrence_gift_card_subject = mailDto.compensation?.payment?.occurrence?.gift_card?.subject?.Length > 800 ? mailDto.compensation?.payment?.occurrence?.gift_card?.subject?.Substring(0, 800) : mailDto.compensation?.payment?.occurrence?.gift_card?.subject;
            compensation_payment_occurrence_gift_card_content = mailDto.compensation?.payment?.occurrence?.gift_card?.content?.Length > 800 ? mailDto.compensation?.payment?.occurrence?.gift_card?.content?.Substring(0, 800) : mailDto.compensation?.payment?.occurrence?.gift_card?.content;
            compensation_payment_occurrence_refund_subject = mailDto.compensation?.payment?.occurrence?.refund?.subject?.Length > 800 ? mailDto.compensation?.payment?.occurrence?.refund?.subject?.Substring(0, 800) : mailDto.compensation?.payment?.occurrence?.refund?.subject;
            compensation_payment_occurrence_refund_content = mailDto.compensation?.payment?.occurrence?.refund?.content?.Length > 800 ? mailDto.compensation?.payment?.occurrence?.refund?.content?.Substring(0, 800) : mailDto.compensation?.payment?.occurrence?.refund?.content;
            compensation_payment_occurrence_wire_transfer_subject = mailDto.compensation?.payment?.occurrence?.wire_transfer?.subject?.Length > 800 ? mailDto.compensation?.payment?.occurrence?.wire_transfer?.subject?.Substring(0, 800) : mailDto.compensation?.payment?.occurrence?.wire_transfer?.subject;
            compensation_payment_occurrence_wire_transfer_content = mailDto.compensation?.payment?.occurrence?.wire_transfer?.content?.Length > 800 ? mailDto.compensation?.payment?.occurrence?.wire_transfer?.content?.Substring(0, 800) : mailDto.compensation?.payment?.occurrence?.wire_transfer?.content;
            compensation_wire_transfer_ask_transfer_bank_account_info_subject = mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.subject?.Length > 800 ? mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.subject?.Substring(0, 800) : mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.subject;
            compensation_wire_transfer_ask_transfer_bank_account_info_content = mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.content?.Length > 800 ? mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.content?.Substring(0, 800) : mailDto.compensation?.wire_transfer?.ask_transfer_bank_account_info?.content;
        }
    }
}