using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class PageBodyContent
    {
        public string title { get; init; }
        public string description { get; init; }
        public string label_commentary { get; init; }
        public string description_commentary { get; init; }
        public string required_commentary { get; init; }
        public string image_upload_title { get; init; }
        public string image_upload_description { get; init; }
        public string checkbox { get; init; }
        public string required_invoice_number { get; init; }
        public string required_invoice_file { get; init; }
        public string deny_action { get; init; }
        public string observation { get; init; }
        public string enabled_observations { get; init; }
        public string request_pix_key_disabled { get; init; }
    }
}
