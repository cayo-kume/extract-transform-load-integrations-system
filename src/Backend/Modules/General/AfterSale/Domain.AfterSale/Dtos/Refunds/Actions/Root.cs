using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Refunds.Actions
{
    public class Rootobject
    {
        public bool success { get; init; }
        public List<Action> data { get; init; }
    }
}
