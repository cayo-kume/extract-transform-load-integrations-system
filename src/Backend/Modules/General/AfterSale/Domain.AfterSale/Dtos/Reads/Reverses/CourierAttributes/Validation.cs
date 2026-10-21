using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes
{
    public class Validation
    {
        public string required { get; init; }
        public string mime { get; init; }
        public string mask { get; init; }
    }
}
