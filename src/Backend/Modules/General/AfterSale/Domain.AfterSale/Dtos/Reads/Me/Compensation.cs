using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Compensation
    {
        public Payment payment { get; set; }
        public WireTransfer wire_transfer { get; set; }
    }
}
