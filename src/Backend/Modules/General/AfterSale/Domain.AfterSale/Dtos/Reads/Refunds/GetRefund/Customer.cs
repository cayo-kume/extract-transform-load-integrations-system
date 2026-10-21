using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class Customer
    {
        public string id { get; init; }
        public string first_name { get; init; }
        public string last_name { get; init; }
        public string email { get; init; }
        public string document { get; init; }
    }
}
