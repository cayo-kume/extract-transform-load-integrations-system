using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class EcommerceOrderSeller
    {
        public string? invoice_number { get; set; }
        public string? invoice_url { get; set; }
    }
}
