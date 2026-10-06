using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class Mail
    {
        public MailBodyContent greeting { get; init; }
        public MailBodyContent footer { get; init; }
        public Reverse reverse { get; init; }
        public Refund refund { get; init; }
        public Retention retention { get; init; }
        public MailPartial partials { get; init; }
        public Occurrence occurrence { get; init; }
        public MailCancellation cancellation { get; init; }
        public Solicitation solicitation { get; init; }
        public Compensation compensation { get; init; }
    }
}
