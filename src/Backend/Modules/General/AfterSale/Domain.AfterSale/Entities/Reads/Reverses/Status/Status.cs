using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.Status
{
    public class Status
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public string? doc_company { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Status() { }

        public Status(Dtos.Reads.Reverses.Status.Status statusDto, string doc_company)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(statusDto.id);

            name = statusDto.name?.Length > 50 ? statusDto.name?.Substring(0, 50) : statusDto.name;
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
            description = statusDto.description?.Length > 250 ? statusDto.description?.Substring(0, 250) : statusDto.description;
        }
    }
}
