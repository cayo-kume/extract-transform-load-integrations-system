using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class Refund
    {
        public MailBodyContent approved { get; init; }
        public MailBodyContent paid_refund { get; init; }
        public MailBodyContent refund_nps { get; init; }
        public MailBodyContent paid_wire_transfer { get; init; }
        public MailBodyContent paid_product { get; init; }
        public MailBodyContent paid_voucher { get; init; }
        public MailBodyContent paid_credit { get; init; }
        public MailBodyContent ask_wire_transfer_data { get; init; }
        public MailBodyContent paid_hard_retention { get; init; }
        public MailBodyContent ask_wire_transfer_data_update { get; init; }
        public MailBodyContent review { get; init; }
    }
}
