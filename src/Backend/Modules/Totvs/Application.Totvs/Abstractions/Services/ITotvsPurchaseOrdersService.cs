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

        /// <summary>
        /// inclui um pedido de compra (POST purchase-order/v2/orders); a resposta traz loja e número. Recusa (422) um supplierOrderCode do mesmo
        /// fornecedor já gravado (DP2) e, por 10 minutos, o mesmo corpo (409, T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrders(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.InsertPurchaseOrder body);

        /// <summary>
        /// troca a quantidade pedida dos itens (POST purchase-order/v2/quantity-items). Com o pedido no banco: atendido ou cancelado, produto fora
        /// do pedido e quantidade abaixo da recebida voltam 422 sem chamar a Totvs (DP7). Quantidade 0 deixa o item com 0, sem cancelar
        /// (testado): o cancelamento é pelo PostPurchaseOrdersCancel
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrdersQuantityItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.RequestedQuantity body);

        /// <summary>
        /// cria ou troca o vínculo de um item de lote com um pedido de compra (POST purchase-order/v2/purchase-order/batch-link); a nota de
        /// entrada do pedido é obrigatória (testado)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrderBatchLink(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.BatchLink body);

        /// <summary>
        /// cancela um pedido de compra (POST purchase-order/v2/orders/cancel), irreversível pela API. Com o pedido no banco, o fornecedor esperado
        /// precisa bater (DP3) e cancelado, atendido, parcialmente recebido ou distribuído voltam 422 sem chamar a Totvs
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrdersCancel(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.CancelPurchaseOrder body);
    }
}
