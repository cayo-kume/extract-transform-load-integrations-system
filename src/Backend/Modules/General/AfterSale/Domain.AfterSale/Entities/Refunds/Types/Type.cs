using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Domain.AfterSale.Entities.Refunds.Types
{
    public class Type
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public int id { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Type() { }

        public Type(int id, Dtos.Refunds.Types.Type typeDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id = id;
            name = typeDto.name?.Length > 50 ? typeDto.name?.Substring(0, 50) : typeDto.name;
            description = typeDto.description?.Length > 250 ? typeDto.description?.Substring(0, 250) : typeDto.description;
        }
    }
}
