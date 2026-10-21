using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class TotalAmountHistories
    {
        public string id { get; init; }
        public string total_amount { get; init; }
        public string date { get; init; }
        public User user { get; init; }
        public Type type { get; init; }
    }
}
