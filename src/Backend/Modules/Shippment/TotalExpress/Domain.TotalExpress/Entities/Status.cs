using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.TotalExpress.Entities
{
    public class Status
    {
        public string? pedido { get; set; }
        public string? id_cliente { get; set; }
        public string? awb { get; set; }
        public string? nfiscal { get; set; }
        public string? nfiscalserie { get; set; }
        public string? cod_barra { get; set; }
        public string? rota { get; set; }
        public DateTime? prev_entrega { get; set; }
        public DateTime? prev_entrega_atualizada { get; set; }

        [NotMapped]
        [SkipProperty]
        /// <summary>
        /// 
        ///</summary>
        public List<statusDeEncomenda> statusDeEncomenda { get; set; } = new List<statusDeEncomenda>();

        [NotMapped]
        [SkipProperty]
        /// <summary>
        /// 
        ///</summary>
        public detalhes detalhes { get; set; }

        [NotMapped]
        [SkipProperty]
        /// <summary>
        /// 
        ///</summary>
        public Dictionary<string?, string> Responses { get; set; } = new Dictionary<string?, string>();

        public Status() { }

        public Status(Status status, string json)
        {
            pedido = status.pedido;
            id_cliente = status.id_cliente;
            awb = status.awb;
            nfiscal = status.nfiscal;
            nfiscalserie = status.nfiscalserie;
            cod_barra = status.cod_barra;
            rota = status.rota;
            this.Responses.Add(status.pedido, json);

            if (status.detalhes is not null)
            {
                prev_entrega = status.detalhes.dataPrev != null ? status.detalhes.dataPrev.PrevEntrega : null;
                prev_entrega_atualizada = status.detalhes.dataPrev != null ? status.detalhes.dataPrev.PrevEntregaAtualizada : null;

                foreach (var statusEncomenda in status.detalhes.statusDeEncomenda)
                {
                    statusDeEncomenda.Add(new statusDeEncomenda(statusEncomenda, status.awb));
                }
            }
        }
    }

    public class detalhes
    {
        public statusDeEncomenda[] statusDeEncomenda { get; set; }
        public dataPrev dataPrev { get; set; }
    }

    public class dataPrev
    {
        public DateTime? PrevEntrega { get; set; }
        public DateTime? PrevEntregaAtualizada { get; set; }
    }

    public class statusDeEncomenda
    {
        public string? awb { get; set; }
        public string? statusid { get; set; }
        public string? status { get; set; }
        public string? data { get; set; }

        public statusDeEncomenda() { }

        public statusDeEncomenda(statusDeEncomenda statusEncomenda, string awb)
        {
            this.awb = awb;
            statusid = statusEncomenda.statusid;
            status = statusEncomenda.status;
            data = statusEncomenda.data;
        }
    }
}
