using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// LinxPedidosCompraNumeroFornecedor, tabela linxmicrovix.LinxPedidosCompraNumeroFornecedor. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_pedido_fornecedor
    /// </summary>
    public class LinxPedidosCompraNumeroFornecedor
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_pedido_fornecedor { get; private set; }
        public int? numero_ped_erp { get; private set; }
        public string? numero_ped_fornecedor { get; private set; }
        public long timestamp { get; private set; }

        public LinxPedidosCompraNumeroFornecedor() { }

        public LinxPedidosCompraNumeroFornecedor(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.PurchaseOrders.LinxPedidosCompraNumeroFornecedor dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_pedido_fornecedor = MicrovixConverters.ToInt32(dto.id_pedido_fornecedor);
            numero_ped_erp = MicrovixConverters.ToNullableInt32(dto.numero_ped_erp);
            numero_ped_fornecedor = MicrovixConverters.ToNullableString(dto.numero_ped_fornecedor, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
