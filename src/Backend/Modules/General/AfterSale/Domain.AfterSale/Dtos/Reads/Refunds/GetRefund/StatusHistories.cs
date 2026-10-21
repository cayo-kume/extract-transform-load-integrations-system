using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class StatusHistories
    {
        public string date { get; init; }
        public string comments { get; init; }
        public Status status { get; init; }
        public User user { get; init; }
    }
}
