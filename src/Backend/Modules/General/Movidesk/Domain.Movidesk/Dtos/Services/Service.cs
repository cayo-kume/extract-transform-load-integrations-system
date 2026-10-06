namespace Domain.Movidesk.Dtos.Services
{
    public class Service
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string parentServiceId { get; set; }
        public string serviceForTicketType { get; set; }
        public string isVisible { get; set; }
        public string allowSelection { get; set; }
        public string allowFinishTicket { get; set; }
        public string isActive { get; set; }
        public string automationMacro { get; set; }
        public string defaultCategory { get; set; }
        public string defaultUrgency { get; set; }
        public string allowAllCategories { get; set; }
        public string[] categories { get; set; }
    }
}
