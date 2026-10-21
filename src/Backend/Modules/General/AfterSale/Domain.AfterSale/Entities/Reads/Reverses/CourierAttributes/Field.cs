using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes
{
    public class Field
    {
        public string? doc_company { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }

        public bool required { get; private set; }

        public string? id { get; private set; }
        public string? name { get; private set; }
        public string? type { get; private set; }
        public string? mime { get; private set; }
        public string? mask { get; private set; }

        public Int64 reverse_id { get; private set; }

        public Field() { }

        public Field(string doc_company, Dtos.Reads.Reverses.CourierAttributes.Field fieldDto, Int64 reverse_id)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.reverse_id = reverse_id;

            id = fieldDto.id?.Length > 50 ? fieldDto.id?.Substring(0, 50) : fieldDto.id;
            name = fieldDto.name?.Length > 250 ? fieldDto.name?.Substring(0, 250) : fieldDto.name;
            type = fieldDto.type?.Length > 50 ? fieldDto.type?.Substring(0, 50) : fieldDto.type;
            mime = fieldDto.validation?.mime?.Length > 50 ? fieldDto.validation?.mime?.Substring(0, 50) : fieldDto.validation?.mime;
            mask = fieldDto.validation?.mask?.Length > 250 ? fieldDto.validation?.mask?.Substring(0, 250) : fieldDto.validation?.mask;

            required = ConverterValidatorsExtensions.ConvertToBooleanValidation(fieldDto.validation?.required);
        }
    }
}
