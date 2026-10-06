using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class RequestedProduct
    {
        public string ref_id { get; set; }
        public string sku { get; set; }
        public string unity { get; set; }
        public string unity_plural { get; set; }
    }
}
