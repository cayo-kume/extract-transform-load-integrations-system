using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.CourierAttributes
{
    public class CourierAttributes
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 reverse_id { get; private set; }
        
        public string? fill_form_from_file { get; private set; }

        [SkipProperty]
        public List<Field> fields { get; private set; } = [];

        public CourierAttributes() { }

        public CourierAttributes(Dtos.Reverses.CourierAttributes.CourierAttribute courierAttributeDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            reverse_id = ConverterValidatorsExtensions.ConvertToInt64Validation(courierAttributeDto.reverse.id);

            fill_form_from_file = courierAttributeDto.fill_form_from_file?.Length > 50 ? courierAttributeDto.fill_form_from_file?.Substring(0, 50) : courierAttributeDto.fill_form_from_file;

            courierAttributeDto.fields?.ForEach(x =>
            {
                fields.Add(new Field(x, reverse_id));
            });
        }
    }
}
