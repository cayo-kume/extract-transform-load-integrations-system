using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Address
    {
        public string deleted_at { get; init; }
        public string updated_at { get; init; }
        public string @long { get; init; }
        public string lat { get; init; }
        public string country { get; init; }
        public string state { get; init; }
        public string city { get; init; }
        public string number { get; init; }
        public string neighborhood { get; init; }
        public string complement { get; init; }
        public string address { get; init; }
        public string zip_code { get; init; }
        public string id { get; init; }
    }
}
