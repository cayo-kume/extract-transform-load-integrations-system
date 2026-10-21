using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class Image
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 refund_id { get; private set; }
        public int id { get; private set; }
        public string? product_id { get; private set; }
        public string? image_path { get; private set; }
        public string? image_url { get; private set; }

        public Image() { }

        public Image(Dtos.Reads.Refunds.GetRefund.Image imageDto, Int64 refund_id)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.refund_id = refund_id;

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(imageDto.id);
            product_id = imageDto.product_id?.Length > 50 ? imageDto.product_id?.Substring(0, 50) : imageDto.product_id;
            image_path = imageDto.image_path?.Length > 50 ? imageDto.image_path?.Substring(0, 50) : imageDto.image_path;
            image_url = imageDto.image_url?.Length > 50 ? imageDto.image_url?.Substring(0, 50) : imageDto.image_url;
        }
    }
}
