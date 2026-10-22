using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{

    public class Root
    {
        public bool success { get; init; }
        public string? error { get; init; }
        public string? error_code { get; init; }
        public string? message { get; init; }
        public string? message_translated { get; init; }
        public Refund data { get; init; }
    }
}
