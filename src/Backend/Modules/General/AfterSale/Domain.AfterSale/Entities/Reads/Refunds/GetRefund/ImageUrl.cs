using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class ImageUrl
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public string? doc_company { get; private set; }

        public Int64 refund_id { get; private set; }
        public Int64 product_id { get; private set; }
        public Int64 id { get; private set; }

        public string? url { get; private set; }

        public ImageUrl() { }

        /// <param name="product_id">id do produto do reembolso dono da imagem</param>
        public ImageUrl(string doc_company, Int64 refund_id, Int64 product_id, Int64 id, string url)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.refund_id = refund_id;
            this.product_id = product_id;
            this.id = id;
            this.url = url?.Length > 300 ? url?.Substring(0, 300) : url;
        }
    }
}
