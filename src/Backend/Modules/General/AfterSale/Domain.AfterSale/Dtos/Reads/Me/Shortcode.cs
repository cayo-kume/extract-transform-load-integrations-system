using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Shortcode
    {
        public string app_name { get; set; }
        public string ecommerce_name { get; set; }
        public string ecommerce_phone { get; set; }
        public string ecommerce_url { get; set; }
        public string ecommerce_logo_url { get; set; }
        public string ecommerce_logo_email_url { get; set; }
        public string ecommerce_display_name { get; set; }
    }
}
