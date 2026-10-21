using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Root
    {
        public bool success { get; init; }
        public Ecommerce data { get; init; }
    }
}
