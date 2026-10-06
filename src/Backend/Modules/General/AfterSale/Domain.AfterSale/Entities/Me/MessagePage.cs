using Domain.Shared.Extensions;

namespace Domain.AfterSale.Entities.Me
{
    public class MessagePage
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public int ecommerce_id { get; private set; }

        public string? introduction_title { get; private set; }
        public string? introduction_description { get; private set; }

        public string? select_source_title { get; private set; }
        public string? select_source_description { get; private set; }

        public string? order_title { get; private set; }
        public string? order_description { get; private set; }

        public string? products_title { get; private set; }
        public string? products_description { get; private set; }
        public string? products_label_commentary { get; private set; }
        public string? products_description_commentary { get; private set; }
        public string? products_image_upload_title { get; private set; }
        public string? products_image_upload_description { get; private set; }
        public bool products_required_commentary { get; private set; }

        public string? insurance_checklist_title { get; private set; }
        public string? insurance_checklist_description { get; private set; }

        public string? cancellation_insurance_title { get; private set; }
        public string? cancellation_insurance_description { get; private set; }
        public string? cancellation_insurance_checkbox { get; private set; }

        public string? deadline_for_insurance_cancellation_expired_title { get; private set; }
        public string? deadline_for_insurance_cancellation_expired_description { get; private set; }
        public string? deadline_for_insurance_cancellation_expired_checkbox { get; private set; }

        public string? insurance_loading_failure_title { get; private set; }
        public string? insurance_loading_failure_description { get; private set; }

        public string? shipping_title { get; private set; }
        public string? shipping_description { get; private set; }

        public string? resume_title { get; private set; }
        public string? resume_description { get; private set; }

        public string? finish_title { get; private set; }
        public string? finish_description { get; private set; }
        public string? finish_label_commentary { get; private set; }
        public string? finish_description_commentary { get; private set; }
        public string? finish_thank_title { get; private set; }
        public string? finish_thank_description { get; private set; }

        public string? customer_title { get; private set; }
        public string? customer_description { get; private set; }
        public string? customer_products_title { get; private set; }
        public string? customer_products_description { get; private set; }

        public string? seller_title { get; private set; }
        public string? seller_description { get; private set; }
        public string? email_by_customer_title { get; private set; }
        public string? email_by_customer_description { get; private set; }
        public bool seller_required_invoice_number { get; private set; }
        public bool seller_required_invoice_file { get; private set; }

        public string? retention_title { get; private set; }
        public string? retention_description { get; private set; }
        public string? retention_deny_action { get; private set; }
        public string? retention_observation { get; private set; }
        public bool retention_enabled_observations { get; private set; }

        public string? retention_voucher_title { get; private set; }
        public string? retention_voucher_description { get; private set; }
        public string? retention_voucher_deny_action { get; private set; }
        public string? retention_voucher_observation { get; private set; }
        public bool retention_voucher_enabled_observations { get; private set; }

        public string? retention_freight_title { get; private set; }
        public string? retention_freight_description { get; private set; }
        public string? retention_freight_deny_action { get; private set; }
        public string? retention_freight_observation { get; private set; }
        public bool retention_freight_enabled_observations { get; private set; }

        public string? bank_account_details_title { get; private set; }
        public string? bank_account_details_description { get; private set; }
        public string? bank_account_details_request_pix_key_disabled { get; private set; }

        public string? existing_request_post_title { get; private set; }
        public string? existing_request_post_description { get; private set; }
        public string? existing_request_collect_title { get; private set; }
        public string? existing_request_collect_description { get; private set; }
        public string? existing_request_title { get; private set; }
        public string? existing_request_description { get; private set; }

        public string? request_follow_up_title { get; private set; }
        public string? request_follow_up_description { get; private set; }

        public string? refund_nps_title { get; private set; }
        public string? refund_nps_description_commentary { get; private set; }
        public string? refund_nps_label_commentary { get; private set; }
        public bool refund_nps_required_commentary { get; private set; }

        public string? refund_nps_confirmation_title { get; private set; }
        public string? refund_nps_confirmation_description { get; private set; }

        public MessagePage() { }

        public MessagePage(Dtos.Me.Page pageDto, int id, int ecommerce_id)
        {
            this.integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.id = id;
            this.ecommerce_id = ecommerce_id;

            products_required_commentary = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.products?.required_commentary);
            seller_required_invoice_number = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.seller?.required_invoice_number);
            seller_required_invoice_file = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.seller?.required_invoice_file);
            retention_enabled_observations = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.retention?.enabled_observations);
            retention_voucher_enabled_observations = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.retention_voucher?.enabled_observations);
            retention_freight_enabled_observations = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.retention_freight?.enabled_observations);
            refund_nps_required_commentary = ConverterValidatorsExtensions.ConvertToBooleanValidation(pageDto.refund_nps?.required_commentary);

            introduction_title = pageDto.introduction?.title?.Length > 800 ? pageDto.introduction?.title?.Substring(0, 800) : pageDto.introduction?.title;
            introduction_description = pageDto.introduction?.description?.Length > 800 ? pageDto.introduction?.description?.Substring(0, 800) : pageDto.introduction?.description;
            select_source_title = pageDto.select_source?.title?.Length > 800 ? pageDto.select_source?.title?.Substring(0, 800) : pageDto.select_source?.title;
            select_source_description = pageDto.select_source?.description?.Length > 800 ? pageDto.select_source?.description?.Substring(0, 800) : pageDto.select_source?.description;
            order_title = pageDto.order?.title?.Length > 800 ? pageDto.order?.title?.Substring(0, 800) : pageDto.order?.title;
            order_description = pageDto.order?.description?.Length > 800 ? pageDto.order?.description?.Substring(0, 800) : pageDto.order?.description;
            products_title = pageDto.products?.title?.Length > 800 ? pageDto.products?.title?.Substring(0, 800) : pageDto.products?.title;
            products_description = pageDto.products?.description?.Length > 800 ? pageDto.products?.description?.Substring(0, 800) : pageDto.products?.description;
            products_label_commentary = pageDto.products?.label_commentary?.Length > 800 ? pageDto.products?.label_commentary?.Substring(0, 800) : pageDto.products?.label_commentary;
            products_description_commentary = pageDto.products?.description_commentary?.Length > 800 ? pageDto.products?.description_commentary?.Substring(0, 800) : pageDto.products?.description_commentary;
            products_image_upload_title = pageDto.products?.image_upload_title?.Length > 800 ? pageDto.products?.image_upload_title?.Substring(0, 800) : pageDto.products?.image_upload_title;
            products_image_upload_description = pageDto.products?.image_upload_description?.Length > 800 ? pageDto.products?.image_upload_description?.Substring(0, 800) : pageDto.products?.image_upload_description;
            insurance_checklist_title = pageDto.insurance_checklist?.title?.Length > 800 ? pageDto.insurance_checklist?.title?.Substring(0, 800) : pageDto.insurance_checklist?.title;
            insurance_checklist_description = pageDto.insurance_checklist?.description?.Length > 800 ? pageDto.insurance_checklist?.description?.Substring(0, 800) : pageDto.insurance_checklist?.description;
            cancellation_insurance_title = pageDto.cancellation_insurance?.title?.Length > 800 ? pageDto.cancellation_insurance?.title?.Substring(0, 800) : pageDto.cancellation_insurance?.title;
            cancellation_insurance_description = pageDto.cancellation_insurance?.description?.Length > 800 ? pageDto.cancellation_insurance?.description?.Substring(0, 800) : pageDto.cancellation_insurance?.description;
            cancellation_insurance_checkbox = pageDto.cancellation_insurance?.checkbox?.Length > 800 ? pageDto.cancellation_insurance?.checkbox?.Substring(0, 800) : pageDto.cancellation_insurance?.checkbox;
            deadline_for_insurance_cancellation_expired_title = pageDto.deadline_for_insurance_cancellation_expired?.title?.Length > 800 ? pageDto.deadline_for_insurance_cancellation_expired?.title?.Substring(0, 800) : pageDto.deadline_for_insurance_cancellation_expired?.title;
            deadline_for_insurance_cancellation_expired_description = pageDto.deadline_for_insurance_cancellation_expired?.description?.Length > 800 ? pageDto.deadline_for_insurance_cancellation_expired?.description?.Substring(0, 800) : pageDto.deadline_for_insurance_cancellation_expired?.description;
            deadline_for_insurance_cancellation_expired_checkbox = pageDto.deadline_for_insurance_cancellation_expired?.checkbox?.Length > 800 ? pageDto.deadline_for_insurance_cancellation_expired?.checkbox?.Substring(0, 800) : pageDto.deadline_for_insurance_cancellation_expired?.checkbox;
            insurance_loading_failure_title = pageDto.insurance_loading_failure?.title?.Length > 800 ? pageDto.insurance_loading_failure?.title?.Substring(0, 800) : pageDto.insurance_loading_failure?.title;
            insurance_loading_failure_description = pageDto.insurance_loading_failure?.description?.Length > 800 ? pageDto.insurance_loading_failure?.description?.Substring(0, 800) : pageDto.insurance_loading_failure?.description;
            shipping_title = pageDto.shipping?.title?.Length > 800 ? pageDto.shipping?.title?.Substring(0, 800) : pageDto.shipping?.title;
            shipping_description = pageDto.shipping?.description?.Length > 800 ? pageDto.shipping?.description?.Substring(0, 800) : pageDto.shipping?.description;
            resume_title = pageDto.resume?.title?.Length > 800 ? pageDto.resume?.title?.Substring(0, 800) : pageDto.resume?.title;
            resume_description = pageDto.resume?.description?.Length > 800 ? pageDto.resume?.description?.Substring(0, 800) : pageDto.resume?.description;
            finish_title = pageDto.finish?.title?.Length > 800 ? pageDto.finish?.title?.Substring(0, 800) : pageDto.finish?.title;
            finish_description = pageDto.finish?.description?.Length > 800 ? pageDto.finish?.description?.Substring(0, 800) : pageDto.finish?.description;
            finish_label_commentary = pageDto.finish?.label_commentary?.Length > 800 ? pageDto.finish?.label_commentary?.Substring(0, 800) : pageDto.finish?.label_commentary;
            finish_description_commentary = pageDto.finish?.description_commentary?.Length > 800 ? pageDto.finish?.description_commentary?.Substring(0, 800) : pageDto.finish?.description_commentary;
            finish_thank_title = pageDto.finish_thank?.title?.Length > 800 ? pageDto.finish_thank?.title?.Substring(0, 800) : pageDto.finish_thank?.title;
            finish_thank_description = pageDto.finish_thank?.description?.Length > 800 ? pageDto.finish_thank?.description?.Substring(0, 800) : pageDto.finish_thank?.description;
            customer_title = pageDto.customer?.title?.Length > 800 ? pageDto.customer?.title?.Substring(0, 800) : pageDto.customer?.title;
            customer_description = pageDto.customer?.description?.Length > 800 ? pageDto.customer?.description?.Substring(0, 800) : pageDto.customer?.description;
            customer_products_title = pageDto.customer_products?.title?.Length > 800 ? pageDto.customer_products?.title?.Substring(0, 800) : pageDto.customer_products?.title;
            customer_products_description = pageDto.customer_products?.description?.Length > 800 ? pageDto.customer_products?.description?.Substring(0, 800) : pageDto.customer_products?.description;
            seller_title = pageDto.seller?.title?.Length > 800 ? pageDto.seller?.title?.Substring(0, 800) : pageDto.seller?.title;
            seller_description = pageDto.seller?.description?.Length > 800 ? pageDto.seller?.description?.Substring(0, 800) : pageDto.seller?.description;
            email_by_customer_title = pageDto.email_by_customer?.title?.Length > 800 ? pageDto.email_by_customer?.title?.Substring(0, 800) : pageDto.email_by_customer?.title;
            email_by_customer_description = pageDto.email_by_customer?.description?.Length > 800 ? pageDto.email_by_customer?.description?.Substring(0, 800) : pageDto.email_by_customer?.description;
            retention_title = pageDto.retention?.title?.Length > 800 ? pageDto.retention?.title?.Substring(0, 800) : pageDto.retention?.title;
            retention_description = pageDto.retention?.description?.Length > 800 ? pageDto.retention?.description?.Substring(0, 800) : pageDto.retention?.description;
            retention_deny_action = pageDto.retention?.deny_action?.Length > 800 ? pageDto.retention?.deny_action?.Substring(0, 800) : pageDto.retention?.deny_action;
            retention_observation = pageDto.retention?.observation?.Length > 800 ? pageDto.retention?.observation?.Substring(0, 800) : pageDto.retention?.observation;
            retention_voucher_title = pageDto.retention_voucher?.title?.Length > 800 ? pageDto.retention_voucher?.title?.Substring(0, 800) : pageDto.retention_voucher?.title;
            retention_voucher_description = pageDto.retention_voucher?.description?.Length > 800 ? pageDto.retention_voucher?.description?.Substring(0, 800) : pageDto.retention_voucher?.description;
            retention_voucher_deny_action = pageDto.retention_voucher?.deny_action?.Length > 800 ? pageDto.retention_voucher?.deny_action?.Substring(0, 800) : pageDto.retention_voucher?.deny_action;
            retention_voucher_observation = pageDto.retention_voucher?.observation?.Length > 800 ? pageDto.retention_voucher?.observation?.Substring(0, 800) : pageDto.retention_voucher?.observation;
            retention_freight_title = pageDto.retention_freight?.title?.Length > 800 ? pageDto.retention_freight?.title?.Substring(0, 800) : pageDto.retention_freight?.title;
            retention_freight_description = pageDto.retention_freight?.description?.Length > 800 ? pageDto.retention_freight?.description?.Substring(0, 800) : pageDto.retention_freight?.description;
            retention_freight_deny_action = pageDto.retention_freight?.deny_action?.Length > 800 ? pageDto.retention_freight?.deny_action?.Substring(0, 800) : pageDto.retention_freight?.deny_action;
            retention_freight_observation = pageDto.retention_freight?.observation?.Length > 800 ? pageDto.retention_freight?.observation?.Substring(0, 800) : pageDto.retention_freight?.observation;
            bank_account_details_title = pageDto.bank_account_details?.title?.Length > 800 ? pageDto.bank_account_details?.title?.Substring(0, 800) : pageDto.bank_account_details?.title;
            bank_account_details_description = pageDto.bank_account_details?.description?.Length > 800 ? pageDto.bank_account_details?.description?.Substring(0, 800) : pageDto.bank_account_details?.description;
            bank_account_details_request_pix_key_disabled = pageDto.bank_account_details?.request_pix_key_disabled?.Length > 800 ? pageDto.bank_account_details?.request_pix_key_disabled?.Substring(0, 800) : pageDto.bank_account_details?.request_pix_key_disabled;
            existing_request_post_title = pageDto.existing_request_post?.title?.Length > 800 ? pageDto.existing_request_post?.title?.Substring(0, 800) : pageDto.existing_request_post?.title;
            existing_request_post_description = pageDto.existing_request_post?.description?.Length > 800 ? pageDto.existing_request_post?.description?.Substring(0, 800) : pageDto.existing_request_post?.description;
            existing_request_collect_title = pageDto.existing_request_collect?.title?.Length > 800 ? pageDto.existing_request_collect?.title?.Substring(0, 800) : pageDto.existing_request_collect?.title;
            existing_request_collect_description = pageDto.existing_request_collect?.description?.Length > 800 ? pageDto.existing_request_collect?.description?.Substring(0, 800) : pageDto.existing_request_collect?.description;
            existing_request_title = pageDto.existing_request?.title?.Length > 800 ? pageDto.existing_request?.title?.Substring(0, 800) : pageDto.existing_request?.title;
            existing_request_description = pageDto.existing_request?.description?.Length > 800 ? pageDto.existing_request?.description?.Substring(0, 800) : pageDto.existing_request?.description;
            request_follow_up_title = pageDto.request_follow_up?.title?.Length > 800 ? pageDto.request_follow_up?.title?.Substring(0, 800) : pageDto.request_follow_up?.title;
            request_follow_up_description = pageDto.request_follow_up?.description?.Length > 800 ? pageDto.request_follow_up?.description?.Substring(0, 800) : pageDto.request_follow_up?.description;
            refund_nps_title = pageDto.refund_nps?.title?.Length > 800 ? pageDto.refund_nps?.title?.Substring(0, 800) : pageDto.refund_nps?.title;
            refund_nps_description_commentary = pageDto.refund_nps?.description_commentary?.Length > 800 ? pageDto.refund_nps?.description_commentary?.Substring(0, 800) : pageDto.refund_nps?.description_commentary;
            refund_nps_label_commentary = pageDto.refund_nps?.label_commentary?.Length > 800 ? pageDto.refund_nps?.label_commentary?.Substring(0, 800) : pageDto.refund_nps?.label_commentary;
            refund_nps_confirmation_title = pageDto.refund_nps_confirmation?.title?.Length > 800 ? pageDto.refund_nps_confirmation?.title?.Substring(0, 800) : pageDto.refund_nps_confirmation?.title;
            refund_nps_confirmation_description = pageDto.refund_nps_confirmation?.description?.Length > 800 ? pageDto.refund_nps_confirmation?.description?.Substring(0, 800) : pageDto.refund_nps_confirmation?.description;
        }
    }
}
