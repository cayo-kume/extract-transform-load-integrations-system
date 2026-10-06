namespace Domain.AfterSale.Entities.Me
{
    public class StringListValue
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public int ecommerce_id { get; private set; }
        public string? value { get; private set; }

        public StringListValue() { }

        public StringListValue(int id, int ecommerce_id, string value)
        {
            this.integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.id = id;
            this.ecommerce_id = ecommerce_id;
            this.value = value?.Length > 300 ? value?.Substring(0, 300) : value;
        }
    }
}
