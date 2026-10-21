using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes
{
    public class Field
    {
        public string id { get; init; }
        public string name { get; init; }
        public Validation validation { get; init; }
        public string type { get; init; }
    }
}
