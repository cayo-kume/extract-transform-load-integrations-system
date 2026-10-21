using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes
{
    public class Root
    {
        public bool success { get; init; }
        public CourierAttribute data { get; init; }
    }
}
