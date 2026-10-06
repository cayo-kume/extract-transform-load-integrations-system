using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.History
{
    public class History
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 reverse_id { get; private set; }

        public string link { get; private set; }

        public History() { }

        public History(Dtos.Reverses.History.Root historyDto, Int64 reverse_id)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.reverse_id = reverse_id;
            link = historyDto.link;
        }
    }
}
