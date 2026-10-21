using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.GetReverse
{
    public class StatusHistories
    {
        public string id { get; init; }
        public string reverse_id { get; init; }
        public string status_id { get; init; }
        public string user_id { get; init; }
        public string customer_id { get; init; }
        public string date { get; init; }
        public string comments { get; init; }
        public string billing_message { get; init; }
        public Status status { get; init; }
        public User user { get; init; }
    }
}
