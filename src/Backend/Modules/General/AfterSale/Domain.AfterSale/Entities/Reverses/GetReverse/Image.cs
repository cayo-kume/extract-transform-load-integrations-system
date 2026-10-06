using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.GetReverse
{
    public class Image
    {
        public Int64 id { get; private set; }
        public Int64 reverse_id { get; private set; }
        public Int64 reverse_product_id { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }

        public string? product_id { get; private set; }
        public string? image_path { get; private set; }
        public string? image_url { get; private set; }

        public Image() { }

        public Image(Dtos.Reverses.GetReverse.Image imageDto, Int64 reverse_id, Int64 reverse_product_id)
        {
            this.reverse_id = reverse_id;
            this.reverse_product_id = reverse_product_id;
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(imageDto.id);
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            product_id = imageDto.product_id?.Length > 50 ? imageDto.product_id?.Substring(0, 50) : imageDto.product_id;
            image_path = imageDto.image_path?.Length > 300 ? imageDto.image_path?.Substring(0, 300) : imageDto.image_path;
            image_url = imageDto.image_url?.Length > 300 ? imageDto.image_url?.Substring(0, 300) : imageDto.image_url;
        }
    }
}
