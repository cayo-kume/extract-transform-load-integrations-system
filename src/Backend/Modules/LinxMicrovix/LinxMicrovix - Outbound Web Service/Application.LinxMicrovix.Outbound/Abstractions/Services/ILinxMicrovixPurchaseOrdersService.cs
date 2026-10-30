namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Pedidos de compra. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixPurchaseOrdersService
    {
        /// <summary>
        /// informações de pedidos de compra de acordo com a loja pesquisada
        /// </summary>
        public Task<bool?> LinxPedidosCompra();

        /// <summary>
        /// os status de pedidos de compra.
        /// </summary>
        public Task<bool?> LinxPedidosCompraStatus();

        /// <summary>
        /// o numero do fornecedor do pedido de compra.
        /// </summary>
        public Task<bool?> LinxPedidosCompraNumeroFornecedor();
    }
}
