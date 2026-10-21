using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.GetRefund
{
    public class Voucher
    {
        public string redemption_code { get; init; }
        public string giftcard_id { get; init; }
        public string expiring_date { get; init; }
    }
}
