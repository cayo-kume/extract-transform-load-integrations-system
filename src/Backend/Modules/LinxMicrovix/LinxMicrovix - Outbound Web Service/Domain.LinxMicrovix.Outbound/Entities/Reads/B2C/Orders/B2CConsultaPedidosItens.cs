using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaPedidosItens, tabela linxmicrovix.B2CConsultaPedidosItens. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_pedido_item
    /// </summary>
    public class B2CConsultaPedidosItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_pedido_item { get; private set; }
        public int? id_pedido { get; private set; }
        public long? codigoproduto { get; private set; }
        public int? quantidade { get; private set; }
        public decimal? vl_unitario { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaPedidosItens() { }

        public B2CConsultaPedidosItens(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_pedido_item = MicrovixConverters.ToInt32(dto.id_pedido_item);
            id_pedido = MicrovixConverters.ToNullableInt32(dto.id_pedido);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            quantidade = MicrovixConverters.ToNullableInt32(dto.quantidade);
            vl_unitario = MicrovixConverters.ToNullableDecimal(dto.vl_unitario);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
