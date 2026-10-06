using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class Customer
    {
        public string id { get; init; }
        public string first_name { get; init; }
        public string last_name { get; init; }
        public string email { get; init; }
        public string contact_email { get; init; }
        public string document { get; init; }
        public string phone { get; init; }
        public string contact_phone { get; init; }
        public string address_id { get; init; }
        public string shipping_address_id { get; init; }
        public string is_restrict { get; init; }
        public string is_erased { get; init; }
        public string first_name_encrypted { get; init; }
        public string last_name_encrypted { get; init; }
        public string document_encrypted { get; init; }
        public string email_encrypted { get; init; }
        public string phone_encrypted { get; init; }
        public string contact_phone_encrypted { get; init; }
        public string order_email { get; init; }
        public string order_phone { get; init; }
        public Address address { get; init; }
        public ShippingAddress shipping_address { get; init; }
    }
}
