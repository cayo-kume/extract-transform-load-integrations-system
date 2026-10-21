using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class Reverse
    {
        public string? doc_company { get; private set; }

        public Int64 id { get; private set; }
        public Int64 ecommerce_order_id { get; private set; }
        public Int64 billing_item_id { get; private set; }
        public Int64 ecommerce_address_id { get; private set; }

        public int service_type_change { get; private set; }
        public int courier_contract_id { get; private set; }
        public int invoice { get; private set; }
        public int skip_process_step { get; private set; }
        public int status_id { get; private set; }
        public int ecommerce_id { get; private set; }
        public int insurance_manual_needed { get; private set; }

        public bool courier_collect { get; private set; }
        public bool is_awaiting_resend_webhook { get; private set; }
        public bool is_erased { get; private set; }
        public bool courier_data_is_store_seller_contract { get; private set; }
        public bool freight_by_customer { get; private set; }
        public bool courier_data_encrypted_is_store_seller_contract { get; private set; }
        public bool ecommerce_is_active { get; private set; }
        public bool ecommerce_maintenance_mode_global { get; private set; }
        public bool ecommerce_is_test { get; private set; }
        public bool must_treat_refund { get; private set; }
        public bool is_generic_courier { get; private set; }
        public bool can_generate_voucher { get; private set; }
        public bool could_cancel { get; private set; }
        public bool can_send_correction_letter { get; private set; }
        public bool retry_options_can_retry { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime created_at { get; private set; }
        public DateTime updated_at { get; private set; }
        public DateTime deleted_at { get; private set; }
        public DateTime ecommerce_last_order_report_date { get; private set; }

        public decimal total_amount { get; private set; }

        public string? reverse_type { get; private set; }
        public string? courier_service_type { get; private set; }
        public string? store_id { get; private set; }
        public string? courier_data_posting_card { get; private set; }
        public string? store_expire_date { get; private set; }
        public string? origin { get; private set; }
        public string? external_source { get; private set; }
        public string? type { get; private set; }
        public string? origin_seller_id { get; private set; }
        public string? courier_data_encrypted_posting_card { get; private set; }
        public Guid ecommerce_uuid { get; private set; }
        public string? ecommerce_company_name { get; private set; }
        public string? ecommerce_trade_name { get; private set; }
        public string? ecommerce_display_name { get; private set; }
        public string? ecommerce_url { get; private set; }
        public string? ecommerce_phone { get; private set; }
        public string? ecommerce_email { get; private set; }
        public string? ecommerce_document { get; private set; }
        public DateTime billing_date { get; private set; }
        public string? customer_nfd_attached { get; private set; }
        public string? customer_nfd_xml_attached { get; private set; }
        public string? returned_invoice { get; private set; }
        public string? reverse_type_name { get; private set; }
        public string? correction_letter_link { get; private set; }
        public string? customer_url { get; private set; }
        public string? timeline_url { get; private set; }
        public string? collect_scheduling_link { get; private set; }
        public string? status_service_code { get; private set; }
        public string? status_name { get; private set; }
        public string? status_description { get; private set; }
        public string? ecommerce_order_seller_invoice_number { get; private set; }
        public string? ecommerce_order_seller_invoice_url { get; private set; }
        public string? brand_name { get; private set; }

        [SkipProperty]
        public DestinationData destination_data { get; private set; }

        [SkipProperty]
        public EcommerceOrder ecommerce_order { get; private set; }

        [SkipProperty]
        public Tracking tracking { get; private set; }

        [SkipProperty]
        public Customer customer { get; private set; }

        [SkipProperty]
        public ReverseDestination reverse_destination { get; private set; }

        [SkipProperty]
        public List<Product> products { get; private set; } = [];

        [SkipProperty]
        public List<StatusHistories> status_histories { get; private set; } = [];

        public Reverse() { }

        public Reverse(string doc_company, Dtos.Reads.Reverses.GetReverse.Reverse reverseDto, Dtos.Reads.Reverses.GetReverse.Customer customerDto, List<Dtos.Reads.Reverses.GetReverse.TrackingHistory> trackingHistoryDto)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.id);
            ecommerce_order_id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.ecommerce_order_id);
            billing_item_id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.billing_item_id);
            ecommerce_address_id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.ecommerce.address_id);

            service_type_change = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.service_type_change);
            courier_contract_id = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.courier_contract_id);
            invoice = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.invoice);
            skip_process_step = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.skip_process_step);
            status_id = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.status_id);
            ecommerce_id = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.ecommerce_id);
            insurance_manual_needed = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.insurance_manual_needed);

            courier_collect = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.courier_collect);
            is_awaiting_resend_webhook = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.is_awaiting_resend_webhook);
            is_erased = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.is_erased);
            courier_data_is_store_seller_contract = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.courier_data.is_store_seller_contract);
            freight_by_customer = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.freight_by_customer);
            courier_data_encrypted_is_store_seller_contract = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.courier_data_encrypted.is_store_seller_contract);
            ecommerce_is_active = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.ecommerce.is_active);
            ecommerce_maintenance_mode_global = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.ecommerce.maintenance_mode_global);
            ecommerce_is_test = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.ecommerce.is_test);
            must_treat_refund = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.must_treat_refund);
            is_generic_courier = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.is_generic_courier);
            can_generate_voucher = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.can_generate_voucher);
            could_cancel = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.could_cancel);
            can_send_correction_letter = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.can_send_correction_letter);
            retry_options_can_retry = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDto.retry_options.can_retry);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.created_at);
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.updated_at);
            deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.deleted_at);
            ecommerce_last_order_report_date = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.ecommerce.last_order_report_date);

            total_amount = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.total_amount);

            reverse_type = reverseDto.reverse_type?.Length > 50 ? reverseDto.reverse_type?.Substring(0, 50) : reverseDto.reverse_type;
            courier_service_type = reverseDto.courier_service_type?.Length > 50 ? reverseDto.courier_service_type?.Substring(0, 50) : reverseDto.courier_service_type;
            store_id = reverseDto.store_id?.Length > 50 ? reverseDto.store_id?.Substring(0, 50) : reverseDto.store_id;
            courier_data_posting_card = reverseDto.courier_data?.posting_card?.Length > 50 ? reverseDto.courier_data?.posting_card?.Substring(0, 50) : reverseDto.courier_data?.posting_card;
            store_expire_date = reverseDto.store_expire_date?.Length > 50 ? reverseDto.store_expire_date?.Substring(0, 50) : reverseDto.store_expire_date;
            origin = reverseDto.origin?.Length > 50 ? reverseDto.origin?.Substring(0, 50) : reverseDto.origin;
            external_source = reverseDto.external_source?.Length > 50 ? reverseDto.external_source?.Substring(0, 50) : reverseDto.external_source;
            type = reverseDto.type?.Length > 50 ? reverseDto.type?.Substring(0, 50) : reverseDto.type;
            origin_seller_id = reverseDto.origin_seller_id?.Length > 50 ? reverseDto.origin_seller_id?.Substring(0, 50) : reverseDto.origin_seller_id;
            courier_data_encrypted_posting_card = reverseDto.courier_data_encrypted?.posting_card?.Length > 50 ? reverseDto.courier_data_encrypted?.posting_card?.Substring(0, 50) : reverseDto.courier_data_encrypted?.posting_card;
            ecommerce_uuid = ConverterValidatorsExtensions.ConvertToGuidValidation(reverseDto.ecommerce?.uuid);
            ecommerce_company_name = reverseDto.ecommerce?.company_name?.Length > 250 ? reverseDto.ecommerce?.company_name?.Substring(0, 250) : reverseDto.ecommerce?.company_name;
            ecommerce_trade_name = reverseDto.ecommerce?.trade_name?.Length > 250 ? reverseDto.ecommerce?.trade_name?.Substring(0, 250) : reverseDto.ecommerce?.trade_name;
            ecommerce_display_name = reverseDto.ecommerce?.display_name?.Length > 250 ? reverseDto.ecommerce?.display_name?.Substring(0, 250) : reverseDto.ecommerce?.display_name;
            ecommerce_url = reverseDto.ecommerce?.url?.Length > 300 ? reverseDto.ecommerce?.url?.Substring(0, 300) : reverseDto.ecommerce?.url;
            ecommerce_phone = reverseDto.ecommerce?.phone?.Length > 20 ? reverseDto.ecommerce?.phone?.Substring(0, 20) : reverseDto.ecommerce?.phone;
            ecommerce_email = reverseDto.ecommerce?.email?.Length > 50 ? reverseDto.ecommerce?.email?.Substring(0, 50) : reverseDto.ecommerce?.email;
            ecommerce_document = reverseDto.ecommerce?.document?.Length > 14 ? reverseDto.ecommerce?.document?.Substring(0, 14) : reverseDto.ecommerce?.document;
            billing_date = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.billing_date);
            customer_nfd_attached = reverseDto.customer_nfd_attached?.Length > 50 ? reverseDto.customer_nfd_attached?.Substring(0, 50) : reverseDto.customer_nfd_attached;
            returned_invoice = reverseDto.returned_invoice?.Length > 50 ? reverseDto.returned_invoice?.Substring(0, 50) : reverseDto.returned_invoice;
            reverse_type_name = reverseDto.reverse_type_name?.Length > 50 ? reverseDto.reverse_type_name?.Substring(0, 50) : reverseDto.reverse_type_name;
            correction_letter_link = reverseDto.correction_letter_link?.Length > 300 ? reverseDto.correction_letter_link?.Substring(0, 300) : reverseDto.correction_letter_link;
            customer_url = reverseDto.customer_url?.Length > 300 ? reverseDto.customer_url?.Substring(0, 300) : reverseDto.customer_url;
            timeline_url = reverseDto.timeline_url?.Length > 300 ? reverseDto.timeline_url?.Substring(0, 300) : reverseDto.timeline_url;
            collect_scheduling_link = reverseDto.collect_scheduling_link?.Length > 300 ? reverseDto.collect_scheduling_link?.Substring(0, 300) : reverseDto.collect_scheduling_link;
            status_service_code = reverseDto.status?.service_code?.Length > 50 ? reverseDto.status?.service_code?.Substring(0, 50) : reverseDto.status?.service_code;
            status_name = reverseDto.status?.name?.Length > 50 ? reverseDto.status?.name?.Substring(0, 50) : reverseDto.status?.name;
            status_description = reverseDto.status?.description?.Length > 300 ? reverseDto.status?.description?.Substring(0, 300) : reverseDto.status?.description;
            ecommerce_order_seller_invoice_number = reverseDto.ecommerce_order_seller?.invoice_number?.Length > 20 ? reverseDto.ecommerce_order_seller?.invoice_number?.Substring(0, 20) : reverseDto.ecommerce_order_seller?.invoice_number;
            ecommerce_order_seller_invoice_url = reverseDto.ecommerce_order_seller?.invoice_url?.Length > 300 ? reverseDto.ecommerce_order_seller?.invoice_url?.Substring(0, 300) : reverseDto.ecommerce_order_seller?.invoice_url;
            brand_name = reverseDto.brand?.name?.Length > 50 ? reverseDto.brand?.name?.Substring(0, 50) : reverseDto.brand?.name;

            customer_nfd_xml_attached = reverseDto.customer_nfd_xml_attached;

            destination_data = reverseDto.destination_data != null ? new DestinationData(doc_company, reverseDto.destination_data, id) : new DestinationData();
            ecommerce_order = reverseDto.ecommerce_order != null ? new EcommerceOrder(doc_company, reverseDto.ecommerce_order, id) : new EcommerceOrder();
            tracking = reverseDto.tracking != null ? new Tracking(doc_company, reverseDto.tracking, trackingHistoryDto) : new Tracking();
            customer = customerDto != null ? new Customer(doc_company, customerDto, id) : new Customer();
            reverse_destination = reverseDto.reverse_destination != null ? new ReverseDestination(doc_company, reverseDto.reverse_destination, id) : new ReverseDestination();

            reverseDto.products?.ForEach(x =>
            {
                products.Add(new Product(doc_company, x));
            });

            reverseDto.status_histories?.ForEach(x =>
            {
                status_histories.Add(new StatusHistories(doc_company, x));
            });
        }
    }
}
