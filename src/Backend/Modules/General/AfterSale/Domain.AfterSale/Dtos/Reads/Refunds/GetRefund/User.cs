using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class User
    {
        public string id { get; init; }
        public string name { get; init; }
        public string first_name { get; init; }
        public string last_name { get; init; }
        public string email { get; init; }
        public string mobile_phone { get; init; }
        public string phone { get; init; }
        public string document { get; init; }
        public string wallet { get; init; }
    }
}
