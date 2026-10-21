using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.GetReverse
{
    public class DestinationData
    {
        public string name { get; init; }
        public string type { get; init; }
        public string type_id { get; init; }
        public string updated_at { get; init; }
        public string seller_info { get; init; }
        public string return_to_seller { get; init; }
        public string was_manually_changed { get; init; }
        public string label { get; init; }
        public Address address { get; init; }
    }
}
