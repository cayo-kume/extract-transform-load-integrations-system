using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class ShippingAddress
    {
        public string id { get; init; }
        public string zip_code { get; init; }
        public string address { get; init; }
        public string complement { get; init; }
        public string number { get; init; }
        public string number_encrypted { get; init; }
        public string neighborhood { get; init; }
        public string city { get; init; }
        public string state { get; init; }
        public string country { get; init; }
        public string lat { get; init; }
        public string _long { get; init; }
        public string is_restrict { get; init; }
        public string updated_at { get; init; }
        public string deleted_at { get; init; }
        public string is_erased { get; init; }
    }
}
