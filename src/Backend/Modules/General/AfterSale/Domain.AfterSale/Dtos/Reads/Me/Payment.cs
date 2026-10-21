using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Payment
    {
        public CompensationPaymentCancellation cancellation { get; set; }
        public PaymentOccurrence occurrence { get; set; }
    }
}
