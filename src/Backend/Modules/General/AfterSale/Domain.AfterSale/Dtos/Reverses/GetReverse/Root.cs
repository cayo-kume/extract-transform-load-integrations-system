using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class Root
    {
        public bool success { get; init; }
        public string? error { get; init; }
        public string? error_code { get; init; }
        public string? message { get; init; }
        public string? message_translated { get; init; }
        public Data data { get; init; }
    }

    public class Data
    {
        public string ecommerce_order { get; init; }
        public Reverse reverse { get; init; }
        public Customer customer { get; init; }
        public List<TrackingHistory> tracking_history { get; init; }
    }
}
