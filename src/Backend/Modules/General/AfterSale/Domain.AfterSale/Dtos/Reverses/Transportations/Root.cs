using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.Transportations
{
    public class Root
    {
        public bool success { get; init; }
        public List<string> data { get; init; }
    }
}
