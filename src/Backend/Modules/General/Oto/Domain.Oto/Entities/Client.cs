using Domain.Core.Entities.Base;

namespace Domain.Oto.Models
{
    public class Client : ClientBase
    {
        public string? branch { get; set; }
        public string? data_source { get; set; }
        public string? store { get; set; }
        public string? country { get; set; }
        public string? gender { get; set; }
        public string? type { get; set; }
        public DateTime date { get; set; }
        public DateTime created_date { get; set; }
        public DateTime birthday_date { get; set; }
        public decimal? cashback_value { get; set; }
    }
}
