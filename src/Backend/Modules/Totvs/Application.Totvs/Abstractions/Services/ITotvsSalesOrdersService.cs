namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsSalesOrdersService
    {
        /// <summary>
        /// sincroniza os pedidos de venda de todas as lojas da holding pelo POST sales-order/v2/orders/search; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchSalesOrders();

        /// <summary>
        /// busca um pedido de venda por loja + número (POST sales-order/v2/orders/search com branchCodeList + orderCodeList) e grava
        /// sempre, sem consultar o cache; a chamada é registrada em totvs.TotvsRequests. true gravado, false não existe na Totvs,
        /// null empresa não cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetSalesOrderByKey(string user_name, string doc_company, int branchCode, int orderCode);

        /// <summary>
        /// sincroniza o catálogo de tipos de desconto pelo GET sales-order/v2/discount-type (janela de alteração, como as filiais); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetDiscountType();

        /// <summary>
        /// sincroniza o catálogo de classificações de pedido de venda pelo GET sales-order/v2/classifications (janela de alteração); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetClassifications();

        /// <summary>
        /// sincroniza os lotes de pedidos de venda (PEDFM018) pelo GET sales-order/v2/batch-items (janela de alteração); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetBatchItems();

        /// <summary>
        /// sincroniza as sugestões de faturamento, com pedidos e itens, pelo GET sales-order/v2/billing-suggestions (janela de alteração); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetBillingSuggestions();
    }
}
