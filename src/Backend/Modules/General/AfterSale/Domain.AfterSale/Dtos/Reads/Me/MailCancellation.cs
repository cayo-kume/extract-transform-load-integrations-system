using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class MailCancellation
    {
        public MailBodyContent failed { get; set; }
        public MailBodyContent fully { get; set; }
        public MailBodyContent partially { get; set; }
        public MailBodyContent refused { get; set; } 
        public Created created { get; set; }
    }
}
