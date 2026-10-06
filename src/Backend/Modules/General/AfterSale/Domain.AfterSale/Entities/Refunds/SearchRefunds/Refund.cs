using Domain.Shared.Extensions;
using Domain.AfterSale.Dtos.Refunds.SearchRefunds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Refunds.SearchRefunds
{
    public class Refund
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 id { get; private set; }
        public Int64 reverse_id { get; private set; }
        public Int64 ecommerce_order_id { get; private set; }
        public Int64 customer_id { get; private set; }
        
        public int status_id { get; private set; }
        
        public decimal requested_amount { get; private set; }
        public decimal requested_total_amount { get; private set; }
        public decimal requested_shipping_amount { get; private set; }
        public decimal? bonus_amount { get; private set; }
        public decimal? bonus_amount_percent { get; private set; }
        public decimal? total_amount { get; private set; }

        public DateTime updated_at { get; private set; }
        public DateTime created_at { get; private set; }
        public DateTime last_status_history_date { get; private set; }
        
        public string? action { get; private set; }
        public string? reverse_partner_store { get; private set; }
        public string? refund_attachment { get; private set; }
        public string? order_id { get; private set; }
        public string? type { get; private set; }

        public string? status_name { get; private set; }
        public string? status_description { get; private set; }

        public string? customer_first_name { get; private set; }
        public string? customer_last_name { get; private set; }

        public string? product_price_type { get; private set; }
        public string? product_price_type_label { get; private set; }
        public string? refunded_shipping_type { get; private set; }

        public Refund() { }

        public Refund(Dtos.Refunds.SearchRefunds.Refund refundDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.id);
            reverse_id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.reverse_id);
            ecommerce_order_id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.ecommerce_order_id);
            customer_id = ConverterValidatorsExtensions.ConvertToInt64Validation(refundDto.customer?.id);

            status_id = ConverterValidatorsExtensions.ConvertToInt32Validation(refundDto.status?.id);

            requested_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_amount);
            requested_total_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_total_amount);
            requested_shipping_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_shipping_amount);
            bonus_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.bonus_amount);
            bonus_amount_percent = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.bonus_amount_percent);
            total_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.total_amount);

            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(refundDto.updated_at);
            created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(refundDto.created_at);
            last_status_history_date = ConverterValidatorsExtensions.ConvertToDateTimeValidation(refundDto.last_status_history_date);

            action = refundDto.action?.Length > 50 ? refundDto.action?.Substring(0, 50) : refundDto.action;
            reverse_partner_store = refundDto.reverse?.partner_store?.Length > 50 ? refundDto.reverse?.partner_store?.Substring(0, 50) : refundDto.reverse?.partner_store;
            refund_attachment = refundDto.refund_attachment?.Length > 50 ? refundDto.refund_attachment?.Substring(0, 50) : refundDto.refund_attachment;
            order_id = refundDto.order_id?.Length > 50 ? refundDto.order_id?.Substring(0, 50) : refundDto.order_id;
            type = refundDto.type?.Length > 50 ? refundDto.type?.Substring(0, 50) : refundDto.type;
            status_name = refundDto.status?.name?.Length > 50 ? refundDto.status?.name?.Substring(0, 50) : refundDto.status?.name;
            status_description = refundDto.status?.description?.Length > 50 ? refundDto.status?.description?.Substring(0, 50) : refundDto.status?.description;
            customer_first_name = refundDto.customer?.first_name?.Length > 50 ? refundDto.customer?.first_name?.Substring(0, 50) : refundDto.customer?.first_name;
            customer_last_name = refundDto.customer?.last_name?.Length > 100 ? refundDto.customer?.last_name?.Substring(0, 100) : refundDto.customer?.last_name;
            product_price_type = refundDto.product_price_type?.type?.Length > 50 ? refundDto.product_price_type?.type?.Substring(0, 50) : refundDto.product_price_type?.type;
            product_price_type_label = refundDto.product_price_type?.label?.Length > 50 ? refundDto.product_price_type?.label?.Substring(0, 50) : refundDto.product_price_type?.label;
            refunded_shipping_type = refundDto.refunded_shipping_type?.Length > 50 ? refundDto.refunded_shipping_type?.Substring(0, 50) : refundDto.refunded_shipping_type;
        }
    }
}
