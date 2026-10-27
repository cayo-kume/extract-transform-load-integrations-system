using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// LinxPedidosCompraStatus, tabela linxmicrovix.LinxPedidosCompraStatus. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_compras_pedido_status (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPedidosCompraStatus
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_compras_pedido_status { get; private set; }
        public string? descricao { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxPedidosCompraStatus() { }

        public LinxPedidosCompraStatus(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.PurchaseOrders.LinxPedidosCompraStatus dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_compras_pedido_status = MicrovixConverters.ToInt32(dto.id_compras_pedido_status);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
