using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class Customer
    {
        public string? doc_company { get; private set; }

        public Int64 reverse_id { get; private set; }
        public Int64 id { get; private set; }
        public Int64 address_id { get; private set; }
        public Int64 shipping_address_id { get; private set; }

        public bool is_restrict { get; private set; }
        public bool is_erased { get; private set; }
        public bool address_is_restrict { get; private set; }
        public bool address_is_erased { get; private set; }
        public bool shipping_address_is_restrict { get; private set; }
        public bool shipping_address_is_erased { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime address_updated_at { get; private set; }
        public DateTime address_deleted_at { get; private set; }
        public DateTime shipping_address_updated_at { get; private set; }
        public DateTime shipping_address_deleted_at { get; private set; }

        public string? first_name { get; private set; }
        public string? last_name { get; private set; }
        public string? email { get; private set; }
        public string? contact_email { get; private set; }
        public string? document { get; private set; }
        public string? phone { get; private set; }
        public string? contact_phone { get; private set; }
        public string? first_name_encrypted { get; private set; }
        public string? last_name_encrypted { get; private set; }
        public string? document_encrypted { get; private set; }
        public string? email_encrypted { get; private set; }
        public string? phone_encrypted { get; private set; }
        public string? contact_phone_encrypted { get; private set; }
        public string? contact_email_encrypted { get; private set; }
        public string? order_email { get; private set; }
        public string? order_phone { get; private set; }
        public string? address_zip_code { get; private set; }
        public string? address_address { get; private set; }
        public string? address_complement { get; private set; }
        public string? address_number { get; private set; }
        public string? address_number_encrypted { get; private set; }
        public string? address_neighborhood { get; private set; }
        public string? address_city { get; private set; }
        public string? address_state { get; private set; }
        public string? address_country { get; private set; }
        public string? address_lat { get; private set; }
        public string? address_long { get; private set; }
        public string? shipping_address_zip_code { get; private set; }
        public string? shipping_address_address { get; private set; }
        public string? shipping_address_complement { get; private set; }
        public string? shipping_address_number { get; private set; }
        public string? shipping_address_number_encrypted { get; private set; }
        public string? shipping_address_neighborhood { get; private set; }
        public string? shipping_address_city { get; private set; }
        public string? shipping_address_state { get; private set; }
        public string? shipping_address_country { get; private set; }
        public string? shipping_address_lat { get; private set; }
        public string? shipping_address_long { get; private set; }

        public Customer() { }

        public Customer(string doc_company, Dtos.Reads.Reverses.GetReverse.Customer customerDto, Int64 reverse_id)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.reverse_id = reverse_id;
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(customerDto.id);
            address_id = ConverterValidatorsExtensions.ConvertToInt64Validation(customerDto.address_id);
            shipping_address_id = ConverterValidatorsExtensions.ConvertToInt64Validation(customerDto.shipping_address_id);

            is_restrict = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.is_restrict);
            is_erased = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.is_erased);
            address_is_restrict = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.address.is_restrict);
            address_is_erased = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.address.is_erased);
            shipping_address_is_restrict = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.shipping_address.is_restrict);
            shipping_address_is_erased = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.shipping_address.is_erased);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            address_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(customerDto.address?.updated_at);
            address_deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(customerDto.address?.deleted_at);
            shipping_address_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(customerDto.shipping_address?.updated_at);
            shipping_address_deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(customerDto.shipping_address?.deleted_at);

            first_name = customerDto.first_name?.Length > 50 ? customerDto.first_name?.Substring(0, 50) : customerDto.first_name;
            last_name = customerDto.last_name?.Length > 250 ? customerDto.last_name?.Substring(0, 250) : customerDto.last_name;
            email = customerDto.email?.Length > 50 ? customerDto.email?.Substring(0, 50) : customerDto.email;
            contact_email = customerDto.contact_email?.Length > 50 ? customerDto.contact_email?.Substring(0, 50) : customerDto.contact_email;
            document = customerDto.document?.Length > 14 ? customerDto.document?.Substring(0, 14) : customerDto.document;
            phone = customerDto.phone?.Length > 20 ? customerDto.phone?.Substring(0, 20) : customerDto.phone;
            contact_phone = customerDto.contact_phone?.Length > 20 ? customerDto.contact_phone?.Substring(0, 20) : customerDto.contact_phone;
            first_name_encrypted = customerDto.first_name_encrypted?.Length > 50 ? customerDto.first_name_encrypted?.Substring(0, 50) : customerDto.first_name_encrypted;
            last_name_encrypted = customerDto.last_name_encrypted?.Length > 250 ? customerDto.last_name_encrypted?.Substring(0, 250) : customerDto.last_name_encrypted;
            document_encrypted = customerDto.document_encrypted?.Length > 14 ? customerDto.document_encrypted?.Substring(0, 14) : customerDto.document_encrypted;
            email_encrypted = customerDto.email_encrypted?.Length > 50 ? customerDto.email_encrypted?.Substring(0, 50) : customerDto.email_encrypted;
            phone_encrypted = customerDto.phone_encrypted?.Length > 50 ? customerDto.phone_encrypted?.Substring(0, 50) : customerDto.phone_encrypted;
            contact_phone_encrypted = customerDto.contact_phone_encrypted?.Length > 20 ? customerDto.contact_phone_encrypted?.Substring(0, 20) : customerDto.contact_phone_encrypted;
            contact_email_encrypted = customerDto.contact_email_encrypted?.Length > 50 ? customerDto.contact_email_encrypted?.Substring(0, 50) : customerDto.contact_email_encrypted;
            order_email = customerDto.order_email?.Length > 50 ? customerDto.order_email?.Substring(0, 50) : customerDto.order_email;
            order_phone = customerDto.order_phone?.Length > 20 ? customerDto.order_phone?.Substring(0, 20) : customerDto.order_phone;
            address_zip_code = customerDto.address?.zip_code?.Length > 9 ? customerDto.address?.zip_code?.Substring(0, 9) : customerDto.address?.zip_code;
            address_address = customerDto.address?.address?.Length > 250 ? customerDto.address?.address?.Substring(0, 250) : customerDto.address?.address;
            address_complement = customerDto.address?.complement?.Length > 50 ? customerDto.address?.complement?.Substring(0, 50) : customerDto.address?.complement;
            address_number = customerDto.address?.number?.Length > 20 ? customerDto.address?.number?.Substring(0, 20) : customerDto.address?.number;
            address_number_encrypted = customerDto.address?.number_encrypted?.Length > 20 ? customerDto.address?.number_encrypted?.Substring(0, 20) : customerDto.address?.number_encrypted;
            address_neighborhood = customerDto.address?.neighborhood?.Length > 50 ? customerDto.address?.neighborhood?.Substring(0, 50) : customerDto.address?.neighborhood;
            address_city = customerDto.address?.city?.Length > 40 ? customerDto.address?.city?.Substring(0, 40) : customerDto.address?.city;
            address_state = customerDto.address?.state?.Length > 20 ? customerDto.address?.state?.Substring(0, 20) : customerDto.address?.state;
            address_country = customerDto.address?.country?.Length > 80 ? customerDto.address?.country?.Substring(0, 80) : customerDto.address?.country;
            address_lat = customerDto.address?.lat?.Length > 50 ? customerDto.address?.lat?.Substring(0, 50) : customerDto.address?.lat;
            address_long = customerDto.address?._long?.Length > 50 ? customerDto.address?._long?.Substring(0, 50) : customerDto.address?._long;
            shipping_address_zip_code = customerDto.shipping_address?.zip_code?.Length > 9 ? customerDto.shipping_address?.zip_code?.Substring(0, 9) : customerDto.shipping_address?.zip_code;
            shipping_address_address = customerDto.shipping_address?.address?.Length > 250 ? customerDto.shipping_address?.address?.Substring(0, 250) : customerDto.shipping_address?.address;
            shipping_address_complement = customerDto.shipping_address?.complement?.Length > 50 ? customerDto.shipping_address?.complement?.Substring(0, 50) : customerDto.shipping_address?.complement;
            shipping_address_number = customerDto.shipping_address?.number?.Length > 20 ? customerDto.shipping_address?.number?.Substring(0, 20) : customerDto.shipping_address?.number;
            shipping_address_number_encrypted = customerDto.shipping_address?.number_encrypted?.Length > 20 ? customerDto.shipping_address?.number_encrypted?.Substring(0, 20) : customerDto.shipping_address?.number_encrypted;
            shipping_address_neighborhood = customerDto.shipping_address?.neighborhood?.Length > 50 ? customerDto.shipping_address?.neighborhood?.Substring(0, 50) : customerDto.shipping_address?.neighborhood;
            shipping_address_city = customerDto.shipping_address?.city?.Length > 40 ? customerDto.shipping_address?.city?.Substring(0, 40) : customerDto.shipping_address?.city;
            shipping_address_state = customerDto.shipping_address?.state?.Length > 20 ? customerDto.shipping_address?.state?.Substring(0, 20) : customerDto.shipping_address?.state;
            shipping_address_country = customerDto.shipping_address?.country?.Length > 80 ? customerDto.shipping_address?.country?.Substring(0, 80) : customerDto.shipping_address?.country;
            shipping_address_lat = customerDto.shipping_address?.lat?.Length > 50 ? customerDto.shipping_address?.lat?.Substring(0, 50) : customerDto.shipping_address?.lat;
            shipping_address_long = customerDto.shipping_address?._long?.Length > 50 ? customerDto.shipping_address?._long?.Substring(0, 50) : customerDto.shipping_address?._long;
        }
    }
}
