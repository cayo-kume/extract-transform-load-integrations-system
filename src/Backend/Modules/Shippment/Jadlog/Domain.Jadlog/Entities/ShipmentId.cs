namespace Domain.Jadlog.Entities
{
    public class ShipmentId
    {
        public string shipment_id { get; set; }
        public string doc_company { get; set; }
        public string product { get; set; }
        public List<Evento> eventos { get; set; } = new List<Evento>();
    }
}
