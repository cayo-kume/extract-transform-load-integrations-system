namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaPedidosIdentificador
    {
        public string? portal { get; private set; }
        public string? empresa { get; private set; }
        public string? identificador { get; private set; }
        public string? id_venda { get; private set; }
        public string? order_id { get; private set; }
        public string? id_cliente { get; private set; }
        public string? valor_frete { get; private set; }
        public string? data_origem { get; private set; }
        public string? timestamp { get; private set; }

        public B2CConsultaPedidosIdentificador() { }

        public B2CConsultaPedidosIdentificador(
            string? portal,
            string? empresa,
            string? identificador,
            string? id_venda,
            string? order_id,
            string? id_cliente,
            string? valor_frete,
            string? data_origem,
            string? timestamp
        )
        {
            this.portal = portal;
            this.empresa = empresa;
            this.identificador = identificador;
            this.id_venda = id_venda;
            this.order_id = order_id;
            this.id_cliente = id_cliente;
            this.valor_frete = valor_frete;
            this.data_origem = data_origem;
            this.timestamp = timestamp;
        }
    }
}