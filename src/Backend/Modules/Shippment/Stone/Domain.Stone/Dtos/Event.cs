namespace Domain.Stone.Dtos
{
    public class Event
    {
        public string id { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
        public string modifiedBy { get; set; }
        public string trackingCode { get; set; }
        public string description { get; set; }

        public Carrier carrier { get; set; }
    }
}
