namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Pedidos de venda. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixOrdersService
    {
        /// <summary>
        /// informações de pedidos de venda de acordo com a loja pesquisada
        /// </summary>
        public Task<bool?> LinxPedidosVenda();

        /// <summary>
        /// os Planos de Pagamentos realizados nos pedidos de vendas
        /// </summary>
        public Task<bool?> LinxPlanosPedidoVenda();

        /// <summary>
        /// os tipos de locais cadastrados no checklist de entrega do pedido de venda.
        /// </summary>
        public Task<bool?> LinxPedidosVendaChecklistEntregaLocal();

        /// <summary>
        /// os tipos de armazenamentos cadastrados no checklist de entrega do pedido de venda.
        /// </summary>
        public Task<bool?> LinxPedidosVendaChecklistEntregaArmazenamento();

        /// <summary>
        /// os tipos de dificuldades cadastradas no checklist de entrega do pedido de venda.
        /// </summary>
        public Task<bool?> LinxPedidosVendaChecklistEntregaDificuldade();

        /// <summary>
        /// os dados das posições cadastradas para os orçamentos.
        /// </summary>
        public Task<bool?> LinxOrcamentoPosicao();

        /// <summary>
        /// os pedidos de B2C
        /// </summary>
        public Task<bool?> LinxB2CPedidos();

        /// <summary>
        /// os itens dos pedidos de B2C
        /// </summary>
        public Task<bool?> LinxB2CPedidosItens();

        /// <summary>
        /// o status dos pedidos de B2C
        /// </summary>
        public Task<bool?> LinxB2CPedidosStatus();

        /// <summary>
        /// o cadastro dos status de B2C
        /// </summary>
        public Task<bool?> LinxB2CStatus();

        /// <summary>
        /// sob demanda: busca LinxPedidosVenda por cod_pedido, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxPedidosVendaByCodPedido(string user_name, string doc_company, string cnpj_emp, string cod_pedido);
    }
}
