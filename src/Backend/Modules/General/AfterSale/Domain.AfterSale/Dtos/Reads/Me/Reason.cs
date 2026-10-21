using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Me
{
    public class Reason
    {
        public string deleted_at { get; init; }
        public string updated_at { get; init; }
        public string created_at { get; init; }
        public string ord { get; init; }
        public string show_product_grid { get; init; }
        public string upload_image { get; init; }
        public string should_approve { get; init; }
        public string action { get; init; }
        public string description { get; init; }
        public string ecommerce_id { get; init; }
        public string id { get; init; }

        public List<SubReason> subreasons { get; init; }
    }
}
