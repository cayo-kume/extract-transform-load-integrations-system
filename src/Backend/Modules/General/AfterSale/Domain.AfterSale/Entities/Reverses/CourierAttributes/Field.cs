using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.CourierAttributes
{
    public class Field
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public bool required { get; private set; }

        public string? id { get; private set; }
        public string? name { get; private set; }
        public string? type { get; private set; }
        public string? mime { get; private set; }

        public Int64 reverse_id { get; private set; }

        public Field() { }

        public Field(Dtos.Reverses.CourierAttributes.Field fieldDto, Int64 reverse_id)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.reverse_id = reverse_id;

            id = fieldDto.id?.Length > 50 ? fieldDto.id?.Substring(0, 50) : fieldDto.id;
            name = fieldDto.name?.Length > 250 ? fieldDto.name?.Substring(0, 250) : fieldDto.name;
            type = fieldDto.type?.Length > 50 ? fieldDto.type?.Substring(0, 50) : fieldDto.type;
            mime = fieldDto.validation?.mime?.Length > 50 ? fieldDto.validation?.mime?.Substring(0, 50) : fieldDto.validation?.mime;

            required = ConverterValidatorsExtensions.ConvertToBooleanValidation(fieldDto.validation.required);
        }
    }
}
