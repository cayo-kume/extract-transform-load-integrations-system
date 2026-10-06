namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaPedidosItens
    {
        public string? id_pedido_item { get; private set; }
        public string? id_pedido { get; private set; }
        public string? codigoproduto { get; private set; }
        public string? quantidade { get; private set; }
        public string? vl_unitario { get; private set; }
        public string? timestamp { get; private set; }
        public string? portal { get; private set; }

        public B2CConsultaPedidosItens() { }

        public B2CConsultaPedidosItens(
            string? id_pedido_item,
            string? id_pedido,
            string? codigoproduto,
            string? quantidade,
            string? vl_unitario,
            string? timestamp,
            string? portal
        )
        {
            this.id_pedido_item = id_pedido_item;
            this.id_pedido = id_pedido;
            this.codigoproduto = codigoproduto;
            this.quantidade = quantidade;
            this.vl_unitario = vl_unitario;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}