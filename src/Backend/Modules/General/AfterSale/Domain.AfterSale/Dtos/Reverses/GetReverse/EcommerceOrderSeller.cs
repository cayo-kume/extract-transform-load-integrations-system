using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.GetReverse
{
    public class EcommerceOrderSeller
    {
        public string invoice_number { get; init; }
        public string invoice_url { get; init; }
    }
}
