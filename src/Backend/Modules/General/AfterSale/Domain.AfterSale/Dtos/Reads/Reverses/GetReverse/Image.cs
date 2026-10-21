using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.GetReverse
{
    public class Image
    {
        public string id { get; init; }
        public string product_id { get; init; }
        public string image_path { get; init; }
        public string image_url { get; init; }
    }
}
