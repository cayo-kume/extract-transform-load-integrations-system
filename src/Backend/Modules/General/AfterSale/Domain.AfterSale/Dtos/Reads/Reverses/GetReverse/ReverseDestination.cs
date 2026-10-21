using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.GetReverse
{
    public class ReverseDestination
    {
        public string is_omnichannel { get; init; }
        public string ie_number { get; init; }
        public string display_name { get; init; }
        public string cnpj { get; init; }
        public Address address { get; init; }
    }
}
