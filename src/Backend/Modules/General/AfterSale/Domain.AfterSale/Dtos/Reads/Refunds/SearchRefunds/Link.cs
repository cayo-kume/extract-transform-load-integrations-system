using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds
{
    public class Link
    {
        public string label { get; init; }
        public string url { get; init; }
        public string active { get; init; }
    }
}
