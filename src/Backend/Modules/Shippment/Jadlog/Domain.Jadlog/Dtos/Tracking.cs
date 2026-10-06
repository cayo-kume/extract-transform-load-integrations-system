namespace Domain.Jadlog.DTOs
{
    public class Tracking
    {
        public string codigo { get; set; }
        public string shipmentId { get; set; }
        public string dacte { get; set; }
        public string dtEmissao { get; set; }
        public string status { get; set; }
        public decimal valor { get; set; }
        public decimal peso { get; set; }
        public Recebedor recebedor { get; set; }
        public List<Evento> eventos { get; set; }
        public List<Volume> volumes { get; set; }
    }
}
