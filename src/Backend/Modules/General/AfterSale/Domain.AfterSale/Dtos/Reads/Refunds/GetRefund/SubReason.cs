using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class SubReason
    {
        public string id { get; init; }
        public string reason_id { get; init; }
        public string description { get; init; }
        public string created_at { get; init; }
        public string updated_at { get; init; }
        public string deleted_at { get; init; }
    }
}
