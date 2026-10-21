using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.Status
{
    public class Root
    {
        public bool success { get; init; }
        public List<Status> data { get; init; }
    }
}
