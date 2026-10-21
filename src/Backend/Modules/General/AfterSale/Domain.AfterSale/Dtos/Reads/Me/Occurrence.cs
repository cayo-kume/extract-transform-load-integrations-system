using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Occurrence
    {
        public MailBodyContent awaiting_customer { get; set; }
        public MailBodyContent awaiting_customer_remind_deadline { get; set; }
        public MailBodyContent created { get; set; }
    }
}
