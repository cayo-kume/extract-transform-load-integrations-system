using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.Banks
{
    public class Root
    {
        public bool success { get; init; }
        public List<Bank> data { get; init; }
    }
}
