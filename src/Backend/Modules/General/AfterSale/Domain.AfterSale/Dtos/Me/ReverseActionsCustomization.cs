using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class ReverseActionsCustomization
    {
        public OnlineOfflineBool online { get; init; }
        public OnlineOfflineBool offline { get; init; }
    }
}
