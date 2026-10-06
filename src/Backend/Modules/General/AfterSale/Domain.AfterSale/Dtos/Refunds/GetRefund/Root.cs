using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Refunds.GetRefund
{

    public class Root
    {
        public bool success { get; init; }
        public Refund data { get; init; }
    }
}
