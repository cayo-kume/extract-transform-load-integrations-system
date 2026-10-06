using System.Globalization;

namespace Domain.Jadlog.Entities
{
    public class Consulta
    {
        public string shipmentId { get; set; }
        public Tracking tracking { get; set; }
        public DateTime? previsaoEntrega { get; set; }
        public Dictionary<string?, string?> Responses { get; set; } = new Dictionary<string?, string?>();

        public Consulta() { }

        public Consulta(DTOs.Consulta consulta, string? response)
        {
            try
            {
                this.shipmentId = consulta.shipmentId;
                this.tracking = new Tracking(consulta.tracking, consulta.previsaoEntrega, consulta.shipmentId);
                this.previsaoEntrega = DateTime.Parse(consulta.previsaoEntrega, new CultureInfo("pt-BR"));
                this.Responses.Add(consulta.shipmentId, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
