using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class TrackingHistory
    {
        public string id { get; init; }
        public string tracking_id { get; init; }
        public string status { get; init; }
        public string message { get; init; }
        public string status_updated_at { get; init; }
        public string created_at { get; init; }
        public string updated_at { get; init; }
        public string deleted_at { get; init; }
    }
}
