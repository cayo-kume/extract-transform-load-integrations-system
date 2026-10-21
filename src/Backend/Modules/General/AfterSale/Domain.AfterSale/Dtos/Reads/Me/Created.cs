using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Created
    {
        public MailBodyContent customer { get; set; }
        public MailBodyContent agent { get; set; }
        public MailBodyContent customer_by_agent { get; set; }
    }
}
