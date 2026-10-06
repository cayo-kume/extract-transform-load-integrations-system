using Domain.Shared.Extensions;
using System.Globalization;

namespace Domain.Jadlog.Entities
{
    public class Tracking
    {
        public DateTime lastupdateon { get; set; }
        public string codigo { get; set; }
        public string shipmentId { get; set; }
        public string dacte { get; set; }
        public DateTime? dtEmissao { get; set; }
        public string status { get; set; }
        public decimal valor { get; set; }
        public decimal peso { get; set; }
        public string? nomeRecebedor { get; set; }
        public string? docRecebedor { get; set; }
        public DateTime? dataRecebedor { get; set; }
        public DateTime? dataPrevisaoEntrega { get; set; }

        [SkipProperty]
        public List<Evento?> eventos { get; set; } = new List<Evento?>();

        [SkipProperty]
        public List<Volume?> volumes { get; set; } = new List<Volume?>();

        public Tracking() { }

        public Tracking(DTOs.Tracking tracking, string? dataPrevisaoEntrega, string shipmentId)
        {
            this.lastupdateon = DateTime.Now;
            this.codigo = tracking.codigo;
            this.shipmentId = shipmentId;
            this.dacte = tracking.dacte;
            this.dtEmissao = DateTime.Parse(tracking.dtEmissao, new CultureInfo("pt-BR"));
            this.status = tracking.status;
            this.valor = tracking.valor;
            this.peso = tracking.peso;
            this.dataPrevisaoEntrega = DateTime.Parse(dataPrevisaoEntrega, new CultureInfo("pt-BR"));

            var groupedTrackingEventos = tracking.eventos
                                .GroupBy(i => new { i.data, i.status, i.unidade })
                                .Select(g => g.First())
                                .ToList();

            if (tracking.recebedor != null)
            {
                this.nomeRecebedor = tracking.recebedor.nome;
                this.docRecebedor = tracking.recebedor.doc;
                this.dataRecebedor = tracking.recebedor.data;
            }

            foreach (var evento in groupedTrackingEventos)
            {
                this.eventos.Add(new Evento(evento, shipmentId));
            }

            foreach (var volume in tracking.volumes)
            {
                this.volumes.Add(new Volume(volume, shipmentId));
            }
        }
    }
}
