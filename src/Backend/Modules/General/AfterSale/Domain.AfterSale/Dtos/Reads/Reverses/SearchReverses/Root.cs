namespace Domain.AfterSale.Dtos.Reads.Reverses.SearchReverses
{
    public class Root
    {
        public int current_page { get; init; }
        public int from { get; init; }
        public int last_page { get; init; }
        public int to { get; init; }
        public int total { get; init; }
        public int per_page { get; init; }

        public bool success { get; init; }
        public string error { get; init; }
        public string error_code { get; init; }
        public string message { get; init; }
        public string message_translated { get; init; }

        public string first_page_url { get; init; }
        public string last_page_url { get; init; }
        public string next_page_url { get; init; }
        public string path { get; init; }
        public string prev_page_url { get; init; }

        public List<Reverse> data { get; init; }
        public List<Link> links { get; init; }
    }
}
