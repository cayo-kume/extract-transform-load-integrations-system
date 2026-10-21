using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.Actions
{
    public class Root
    {
        public bool success { get; init; }
        public List<Action> data { get; init; }
    }
}
