using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes
{
    public class CourierAttributes
    {
        public string? doc_company { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 reverse_id { get; private set; }
        
        public string? fill_form_from_file { get; private set; }

        [SkipProperty]
        public List<Field> fields { get; private set; } = [];

        public CourierAttributes() { }

        /// <param name="reverse_id">id da reversa consultada; a API pode devolver reverse.id = 0 quando não há campos</param>
        public CourierAttributes(string doc_company, Int64 reverse_id, Dtos.Reads.Reverses.CourierAttributes.CourierAttribute courierAttributeDto)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.reverse_id = reverse_id;

            //gravado como JSON (nvarchar(max))
            fill_form_from_file = courierAttributeDto.fill_form_from_file is null || courierAttributeDto.fill_form_from_file.Type == Newtonsoft.Json.Linq.JTokenType.Null
                ? null
                : courierAttributeDto.fill_form_from_file.ToString(Newtonsoft.Json.Formatting.None);

            //id do campo faz parte da PK de AfterSaleCourierAttributesFields
            courierAttributeDto.fields?
                .Where(x => !string.IsNullOrEmpty(x?.id))
                .ToList()
                .ForEach(x =>
                {
                    fields.Add(new Field(doc_company, x, reverse_id));
                });
        }
    }
}
