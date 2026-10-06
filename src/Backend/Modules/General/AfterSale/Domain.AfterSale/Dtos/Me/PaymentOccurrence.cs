using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class PaymentOccurrence
    {
        public MailBodyContent gift_card { get; set; }
        public MailBodyContent refund { get; set; }
        public MailBodyContent wire_transfer { get; set; }
    }
}
