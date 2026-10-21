using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.Types
{
    /// <summary>
    /// the API does not return an id for the refund types, so the name is the key (PK doc_company, name)
    /// </summary>
    public class Type
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public string? doc_company { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Type() { }

        public Type(Dtos.Reads.Refunds.Types.Type typeDto, string doc_company)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
            name = typeDto.name?.Length > 50 ? typeDto.name?.Substring(0, 50) : typeDto.name;
            description = typeDto.description?.Length > 250 ? typeDto.description?.Substring(0, 250) : typeDto.description;
        }
    }
}
