using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Localization
    {
        public string language { get; init; }
        public string timezone { get; init; }
        public string currency { get; init; }
    }
}
