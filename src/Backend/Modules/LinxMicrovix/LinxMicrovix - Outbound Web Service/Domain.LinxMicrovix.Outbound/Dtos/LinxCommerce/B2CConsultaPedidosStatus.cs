namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaPedidosStatus
    {
        public string? id { get; private set; }
        public string? id_status { get; private set; }
        public string? id_pedido { get; private set; }
        public string? data_hora { get; private set; }
        public string? anotacao { get; private set; }
        public string? timestamp { get; private set; }
        public string? portal { get; private set; }

        public B2CConsultaPedidosStatus() { }

        public B2CConsultaPedidosStatus(
            string? id,
            string? id_status,
            string? id_pedido,
            string? data_hora,
            string? anotacao,
            string? timestamp,
            string? portal
        )
        {
            this.id = id;
            this.id_status = id_status;
            this.id_pedido = id_pedido;
            this.data_hora = data_hora;
            this.anotacao = anotacao;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}