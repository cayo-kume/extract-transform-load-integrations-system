using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.SearchReverses
{
    public class Tracking
    {
        public string authorization_code { get; init; }
        public string shipping_amount { get; init; }
        public string delivery_deadline { get; init; }
        public string tracking_code { get; init; }
    }
}
