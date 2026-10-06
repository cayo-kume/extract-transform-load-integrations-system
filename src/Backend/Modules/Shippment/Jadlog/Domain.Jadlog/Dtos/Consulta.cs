namespace Domain.Jadlog.DTOs
{
    public class Consulta
    {
        public string shipmentId { get; set; }
        public Tracking tracking { get; set; }
        public string previsaoEntrega { get; set; }
    }
}
