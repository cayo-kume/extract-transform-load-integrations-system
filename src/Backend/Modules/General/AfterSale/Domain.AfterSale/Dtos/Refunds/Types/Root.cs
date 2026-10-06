using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Refunds.Types
{
    public class Root
    {
        public bool success { get; init; }
        public List<Type> data { get; init; }
    }
}
