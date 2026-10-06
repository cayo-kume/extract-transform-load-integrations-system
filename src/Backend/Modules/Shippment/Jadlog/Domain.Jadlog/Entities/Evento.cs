using System.Globalization;

namespace Domain.Jadlog.Entities
{
    public class Evento
    {
        public DateTime lastupdateon { get; set; }
        public DateTime data { get; set; }
        public string status { get; set; }
        public string unidade { get; set; }
        public string shipmentId { get; set; }

        public Evento() { }

        public Evento(DTOs.Evento evento, string shipmentId)
        {
            this.lastupdateon = DateTime.Now;
            this.data = DateTime.Parse(evento.data, new CultureInfo("pt-BR"));
            this.status = evento.status;

            if (evento.unidade is null)
                this.unidade = String.Empty;
            else
                this.unidade = evento.unidade;

            this.shipmentId = shipmentId;
        }
    }
}
