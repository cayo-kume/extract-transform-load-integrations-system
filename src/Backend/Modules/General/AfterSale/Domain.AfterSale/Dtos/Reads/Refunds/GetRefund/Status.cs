using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class Status
    {
        public string id { get; init; }
        public string service_code { get; init; }
        public string name { get; init; }
        public string description { get; init; }
    }
}
