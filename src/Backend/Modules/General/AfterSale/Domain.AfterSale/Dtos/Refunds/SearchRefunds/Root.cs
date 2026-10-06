using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Refunds.SearchRefunds
{
    public class Root
    {
        public int current_page { get; init; }
        public string first_page_url { get; init; }
        public int from { get; init; }
        public int last_page { get; init; }
        public string last_page_url { get; init; }
        public string next_page_url { get; init; }
        public string path { get; init; }
        public int per_page { get; init; }
        public string prev_page_url { get; init; }
        public int to { get; init; }
        public int total { get; init; }
        public bool success { get; init; }
        public List<Refund> data { get; init; }
        public List<Link> links { get; init; }
    }
}
