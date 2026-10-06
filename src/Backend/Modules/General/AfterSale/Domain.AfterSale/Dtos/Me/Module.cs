using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class Module
    {
        public List<string> whitelabel { get; init; }
        public List<string> security { get; init; }
        public List<string> order { get; init; }
        public List<string> logistic { get; init; }
        public List<string> insurance { get; init; }
        public List<string> fiscal { get; init; }
        public List<string> retention { get; init; }
        public List<string> omnichannel { get; init; }
        public List<string> money { get; init; }
        public List<string> loyalty { get; init; }
        public List<string> care { get; init; }
        public List<string> beta { get; init; }
        public List<string> app { get; init; }
    }
}
