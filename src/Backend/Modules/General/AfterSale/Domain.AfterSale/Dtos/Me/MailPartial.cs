using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class MailPartial
    {
        public MailBodyContent partially_received_warning { get; init; }
        public MailBodyContent new_products_include_info { get; init; }
    }
}
