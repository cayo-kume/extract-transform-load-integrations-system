using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class MailBodyContent
    {
        public string subject { get; init; }
        public string content { get; init; }
        public string enabled { get; init; }
        public string advancedEmailEnabled { get; init; }
        public string fromCustomerOriginEnabled { get; init; }
        public string fromAgentOriginEnabled { get; init; }
    }
}
