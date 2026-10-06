using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.SearchReverses
{
    public class Reverse
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 id { get; private set; }
        public Int64 ecommerce_order_id { get; private set; }
        public Int64 customer_id { get; private set; }

        public DateTime created_at { get; private set; }
        public DateTime updated_at { get; private set; }
        public DateTime tracking_delivery_deadline { get; private set; }

        public decimal total_amount { get; private set; }

        public int status_id { get; private set; }
        public int refunds_count { get; private set; }

        public string cnpj_emp { get; private set; }
        public string? reverse_type { get; private set; }
        public string? reverse_type_name { get; private set; }
        public string? order_id { get; private set; }
        public string? customer_first_name { get; private set; }
        public string? customer_last_name { get; private set; }
        public string? status_name { get; private set; }
        public string? status_description { get; private set; }
        public string? tracking_authorization_code { get; private set; }
        public string? tracking_shipping_amount { get; private set; }
        public string? tracking_code { get; private set; }

        public Reverse() { }

        public Reverse(string cnpj_emp, Dtos.Reverses.SearchReverses.Reverse reverseDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.cnpj_emp = cnpj_emp;
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.id);
            ecommerce_order_id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.ecommerce_order_id);
            customer_id = ConverterValidatorsExtensions.ConvertToInt64Validation(reverseDto.customer.id);

            created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.created_at);
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.updated_at);
            tracking_delivery_deadline = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reverseDto.tracking.delivery_deadline);

            total_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(reverseDto.total_amount);

            status_id = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.status.id);
            refunds_count = ConverterValidatorsExtensions.ConvertToInt32Validation(reverseDto.refunds_count);

            reverse_type = reverseDto.reverse_type?.Length > 50 ? reverseDto.reverse_type?.Substring(0, 50) : reverseDto.reverse_type;
            reverse_type_name = reverseDto.reverse_type_name?.Length > 50 ? reverseDto.reverse_type_name?.Substring(0, 50) : reverseDto.reverse_type_name;
            order_id = reverseDto.order_id?.Length > 50 ? reverseDto.order_id?.Substring(0, 50) : reverseDto.order_id;
            customer_first_name = reverseDto.customer?.first_name?.Length > 50 ? reverseDto.customer?.first_name?.Substring(0, 50) : reverseDto.customer?.first_name;
            customer_last_name = reverseDto.customer?.last_name?.Length > 100 ? reverseDto.customer?.last_name?.Substring(0, 100) : reverseDto.customer?.last_name;
            status_name = reverseDto.status?.name?.Length > 50 ? reverseDto.status?.name?.Substring(0, 50) : reverseDto.status?.name;
            status_description = reverseDto.status?.description?.Length > 50 ? reverseDto.status?.description?.Substring(0, 50) : reverseDto.status?.description;
            tracking_authorization_code = reverseDto.tracking?.authorization_code?.Length > 50 ? reverseDto.tracking?.authorization_code?.Substring(0, 50) : reverseDto.tracking?.authorization_code;
            tracking_shipping_amount = reverseDto.tracking?.shipping_amount?.Length > 50 ? reverseDto.tracking?.shipping_amount?.Substring(0, 50) : reverseDto.tracking?.shipping_amount;
            tracking_code = reverseDto.tracking?.tracking_code?.Length > 50 ? reverseDto.tracking?.tracking_code?.Substring(0, 50) : reverseDto.tracking?.tracking_code;
        }
    }
}
