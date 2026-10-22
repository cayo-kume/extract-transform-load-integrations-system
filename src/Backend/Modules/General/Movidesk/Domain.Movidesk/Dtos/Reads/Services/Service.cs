namespace Domain.Movidesk.Dtos.Reads.Services
{
    public class Service
    {
        public string id { get; init; }
        public string name { get; init; }
        public string description { get; init; }
        public string parentServiceId { get; init; }
        public string serviceForTicketType { get; init; }
        public string isVisible { get; init; }
        public string allowSelection { get; init; }
        public string allowFinishTicket { get; init; }
        public string isActive { get; init; }
        public string automationMacro { get; init; }
        public string defaultCategory { get; init; }
        public string defaultUrgency { get; init; }
        public string allowAllCategories { get; init; }

        public List<string> categories { get; init; }
    }
}
