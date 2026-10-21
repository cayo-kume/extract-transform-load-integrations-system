using Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds;
using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class Refund
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 id { get; private set; }
        public Int64 customer_id { get; private set; }
        public Int64 reverse_id { get; private set; }
        public Int64 reverse_ecommerce_id { get; private set; }

        public decimal bonus_amount { get; private set; }
        public decimal bonus_amount_percent { get; private set; }
        public decimal requested_shipping_amount { get; private set; }
        public decimal added_taxes_amount { get; private set; }
        public decimal requested_raw_amount { get; private set; }
        public decimal requested_amount { get; private set; }
        public decimal requested_total_amount { get; private set; }
        public decimal received_amount { get; private set; }
        public decimal received_raw_amount { get; private set; }
        public decimal total_amount { get; private set; }
        public decimal insurance_amount { get; private set; }

        public int status_id { get; private set; }
        public int reverse_status_id { get; private set; }
        public int reverse_refunds_count { get; private set; }
        public int comments_count { get; private set; }

        public bool free_shipping { get; private set; }
        public bool should_ask_voucher_code { get; private set; }
        public bool can_edit_wire_transfer { get; private set; }
        public bool can_edit_email { get; private set; }
        public bool has_wire_transfer_account { get; private set; }
        public bool enable_payment { get; private set; }

        public DateTime created_at { get; private set; }
        public DateTime updated_at { get; private set; }

        [SkipProperty]
        public List<Product> products { get; private set; } = [];

        [SkipProperty]
        public List<StatusHistories> status_histories { get; private set; } = [];

        [SkipProperty]
        public List<TotalAmountHistories> total_amount_histories { get; private set; } = [];

        [SkipProperty]
        public List<OrderPaymentMethod> order_payment_methods { get; private set; } = [];
        
        [SkipProperty]
        public List<CouldChangeStatusFor> could_change_status_for { get; private set; } = [];

        public string? type { get; private set; }
        public string? action { get; private set; }
        public string? shipping_method { get; private set; }
        public string? customer_first_name { get; private set; }
        public string? customer_last_name { get; private set; }
        public string? customer_email { get; private set; }
        public string? customer_document { get; private set; }
        public string? status_name { get; private set; }
        public string? status_description { get; private set; }
        public string? seller_info { get; private set; }
        public string? cashback_account { get; private set; }
        public string? voucher_redemption_code { get; private set; }
        public string? voucher_giftcard_id { get; private set; }
        public string? voucher_expiring_date { get; private set; }
        public string? reverse_updated_at { get; private set; }
        public string? reverse_reverse_type { get; private set; }
        public string? reverse_status_service_code { get; private set; }
        public string? reverse_status_name { get; private set; }
        public string? reverse_status_description { get; private set; }
        public string? reverse_created_at { get; private set; }
        public string? reverse_ecommerce_order_id { get; private set; }
        public string? reverse_ecommerce_order_seller_invoice_number { get; private set; }
        public string? reverse_ecommerce_order_seller_invoice_url { get; private set; }
        public string? product_price_type_type { get; private set; }
        public string? product_price_type_label { get; private set; }
        public string? refunded_shipping_type { get; private set; }
        public string? customer_retention_method_id { get; private set; }
        public string? external_order_url { get; private set; }

        public Refund() { }

        public Refund(Dtos.Reads.Refunds.GetRefund.Refund refundDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.id);
            customer_id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.customer.id);
            reverse_id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.reverse.id);
            reverse_ecommerce_id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.reverse.ecommerceOrder.id);

            bonus_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.bonus_amount);
            bonus_amount_percent = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.bonus_amount_percent);
            requested_shipping_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_shipping_amount);
            added_taxes_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.added_taxes_amount);
            requested_raw_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_raw_amount);
            requested_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_amount);
            requested_total_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_total_amount);
            received_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.received_amount);
            received_raw_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.received_raw_amount);
            total_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.total_amount);
            insurance_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.insurance_amount);

            status_id = ConverterValidatorsExtensions.ConvertToInt32Validation(refundDto.status.id);
            reverse_status_id = ConverterValidatorsExtensions.ConvertToInt32Validation(refundDto.reverse.status.id);
            reverse_refunds_count = ConverterValidatorsExtensions.ConvertToInt32Validation(refundDto.reverse.refunds_count);
            comments_count = ConverterValidatorsExtensions.ConvertToInt32Validation(refundDto.comments_count);

            free_shipping = ConverterValidatorsExtensions.ConvertToBooleanValidation(refundDto.free_shipping);
            should_ask_voucher_code = ConverterValidatorsExtensions.ConvertToBooleanValidation(refundDto.should_ask_voucher_code);
            can_edit_wire_transfer = ConverterValidatorsExtensions.ConvertToBooleanValidation(refundDto.can_edit_wire_transfer);
            can_edit_email = ConverterValidatorsExtensions.ConvertToBooleanValidation(refundDto.can_edit_email);
            has_wire_transfer_account = ConverterValidatorsExtensions.ConvertToBooleanValidation(refundDto.has_wire_transfer_account);
            enable_payment = ConverterValidatorsExtensions.ConvertToBooleanValidation(refundDto.enable_payment);

            created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(refundDto.created_at);
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(refundDto.updated_at);
            
            refundDto.products?.ForEach(x => {
                products.Add(new Product(x));
            });

            refundDto.status_histories?.ForEach(x => {
                status_histories.Add(new StatusHistories(x, id));
            });

            refundDto.total_amount_histories?.ForEach(x => {
                total_amount_histories.Add(new TotalAmountHistories(x, id));
            });

            for (int i = 0; i < refundDto.order_payment_methods?.Count(); i++)
            {
                order_payment_methods.Add(new OrderPaymentMethod(id, i++, refundDto.order_payment_methods[i]));
            }

            for (int i = 0; i < refundDto.could_change_status_for?.Count(); i++)
            {
                could_change_status_for.Add(new CouldChangeStatusFor(id, i++, refundDto.could_change_status_for[i]));
            }

            type = refundDto.type?.Length > 50 ? refundDto.type?.Substring(0, 50) : refundDto.type;
            action = refundDto.action?.Length > 50 ? refundDto.action?.Substring(0, 50) : refundDto.action;
            shipping_method = refundDto.shipping_method?.Length > 50 ? refundDto.shipping_method?.Substring(0, 50) : refundDto.shipping_method;
            customer_first_name = refundDto.customer?.first_name?.Length > 50 ? refundDto.customer?.first_name?.Substring(0, 50) : refundDto.customer?.first_name;
            customer_last_name = refundDto.customer?.last_name?.Length > 100 ? refundDto.customer?.last_name?.Substring(0, 100) : refundDto.customer?.last_name;
            customer_email = refundDto.customer?.email?.Length > 50 ? refundDto.customer?.email?.Substring(0, 50) : refundDto.customer?.email;
            customer_document = refundDto.customer?.document?.Length > 50 ? refundDto.customer?.document?.Substring(0, 50) : refundDto.customer?.document;
            status_name = refundDto.status?.name?.Length > 50 ? refundDto.status?.name?.Substring(0, 50) : refundDto.status?.name;
            status_description = refundDto.status?.description?.Length > 50 ? refundDto.status?.description?.Substring(0, 50) : refundDto.status?.description;
            seller_info = refundDto.seller_info?.Length > 50 ? refundDto.seller_info?.Substring(0, 50) : refundDto.seller_info;
            cashback_account = refundDto.cashback_account?.Length > 50 ? refundDto.cashback_account?.Substring(0, 50) : refundDto.cashback_account;
            voucher_redemption_code = refundDto.voucher?.redemption_code?.Length > 50 ? refundDto.voucher?.redemption_code?.Substring(0, 50) : refundDto.voucher?.redemption_code;
            voucher_giftcard_id = refundDto.voucher?.giftcard_id?.Length > 50 ? refundDto.voucher?.giftcard_id?.Substring(0, 50) : refundDto.voucher?.giftcard_id;
            voucher_expiring_date = refundDto.voucher?.expiring_date?.Length > 50 ? refundDto.voucher?.expiring_date?.Substring(0, 50) : refundDto.voucher?.expiring_date;
            reverse_updated_at = refundDto.reverse?.updated_at?.Length > 50 ? refundDto.reverse?.updated_at?.Substring(0, 50) : refundDto.reverse?.updated_at;
            reverse_reverse_type = refundDto.reverse?.reverse_type?.Length > 50 ? refundDto.reverse?.reverse_type?.Substring(0, 50) : refundDto.reverse?.reverse_type;
            reverse_status_service_code = refundDto.reverse?.status?.service_code?.Length > 50 ? refundDto.reverse?.status?.service_code?.Substring(0, 50) : refundDto.reverse?.status?.service_code;
            reverse_status_name = refundDto.reverse?.status?.name?.Length > 50 ? refundDto.reverse?.status?.name?.Substring(0, 50) : refundDto.reverse?.status?.name;
            reverse_status_description = refundDto.reverse?.status?.description?.Length > 50 ? refundDto.reverse?.status?.description?.Substring(0, 50) : refundDto.reverse?.status?.description;
            reverse_created_at = refundDto.reverse?.created_at?.Length > 50 ? refundDto.reverse?.created_at?.Substring(0, 50) : refundDto.reverse?.created_at;
            reverse_ecommerce_order_id = refundDto.reverse?.ecommerceOrder.id?.Length > 50 ? refundDto.reverse?.ecommerceOrder.id?.Substring(0, 50) : refundDto.reverse?.ecommerceOrder.id;
            reverse_ecommerce_order_seller_invoice_number = refundDto.reverse?.ecommerce_order_seller?.invoice_number?.Length > 50 ? refundDto.reverse?.ecommerce_order_seller?.invoice_number?.Substring(0, 50) : refundDto.reverse?.ecommerce_order_seller?.invoice_number;
            reverse_ecommerce_order_seller_invoice_url = refundDto.reverse?.ecommerce_order_seller?.invoice_url?.Length > 50 ? refundDto.reverse?.ecommerce_order_seller?.invoice_url?.Substring(0, 50) : refundDto.reverse?.ecommerce_order_seller?.invoice_url;
            product_price_type_type = refundDto.product_price_type?.type?.Length > 50 ? refundDto.product_price_type?.type?.Substring(0, 50) : refundDto.product_price_type?.type;
            product_price_type_label = refundDto.product_price_type?.label?.Length > 50 ? refundDto.product_price_type?.label?.Substring(0, 50) : refundDto.product_price_type?.label;
            refunded_shipping_type = refundDto.refunded_shipping_type?.Length > 50 ? refundDto.refunded_shipping_type?.Substring(0, 50) : refundDto.refunded_shipping_type;
            customer_retention_method_id = refundDto.customer_retention_method_id?.Length > 50 ? refundDto.customer_retention_method_id?.Substring(0, 50) : refundDto.customer_retention_method_id;
            external_order_url = refundDto.external_order_url?.Length > 50 ? refundDto.external_order_url?.Substring(0, 50) : refundDto.external_order_url;
        }
    }
}
