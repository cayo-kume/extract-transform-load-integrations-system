namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsPurchaseOrdersService
    {
        /// <summary>
        /// sincroniza os pedidos de compra de todas as lojas da holding pelo POST purchase-order/v2/search; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchPurchaseOrders();

        /// <summary>
        /// busca um pedido de compra por loja + número (POST purchase-order/v2/search com branchCodeList + orderCodeList) e grava; a chamada
        /// fica em totvs.TotvsRequests. true gravado; false não encontrado na Totvs; null empresa não cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetPurchaseOrderByKey(string user_name, string doc_company, int branchCode, int orderCode);

        /// <summary>
        /// põe um pedido de compra em andamento ou bloqueado (POST purchase-order/v2/orders/change-status). Com o pedido no banco, só
        /// em andamento → bloqueado e bloqueado → em andamento chegam à Totvs (DP7)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrdersChangeStatus(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.UpdateOrderStatus body);
    }
}
