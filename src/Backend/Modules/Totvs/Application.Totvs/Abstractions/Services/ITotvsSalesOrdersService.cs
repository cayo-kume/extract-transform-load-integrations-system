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

        /// <summary>
        /// devolve as notas de um pedido de venda (GET sales-order/v2/invoices), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetInvoices(string user_name, string doc_company, int branchCode, int orderCode, List<string>? invoiceStatus);

        /// <summary>
        /// devolve os itens pendentes de um pedido de venda, com os preços líquidos (GET sales-order/v2/pending-items), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetPendingItems(string user_name, string doc_company, int branchCode, int orderCode);

        /// <summary>
        /// devolve os itens de lote ligados a um pedido de venda (GET sales-order/v2/changed-batch-items), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetChangedBatchItems(string user_name, string doc_company, int branchCode, int orderCode, int? batchCode, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// altera os dados adicionais de um pedido de venda (POST sales-order/v2/additional-order): rastreio, estágio do e-commerce, entrega
        /// antecipada, tipo de experiência
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostAdditionalOrder(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderAdditional body);

        /// <summary>
        /// altera transportadora, redespacho, frete e endereço de entrega de um pedido de venda (POST sales-order/v2/shipping-order). A Totvs
        /// recusa em pedido bloqueado e em pedido do e-commerce
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostShippingOrder(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderShipping body);

        /// <summary>
        /// inclui linhas de observação num pedido de venda (POST sales-order/v2/observations-order). Não idempotente: o mesmo corpo em
        /// 10 minutos volta 409 (T5). A Totvs recusa em pedido do e-commerce
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostObservationsOrder(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderObservations body);

        /// <summary>
        /// altera medida de rolo e previsão de faturamento dos itens de um pedido de venda (POST sales-order/v2/additional-items). A Totvs
        /// recusa em pedido do e-commerce
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostAdditionalItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdditional body);

        /// <summary>
        /// muda a situação do pedido de venda (POST sales-order/v2/orders/change-status), pelo nome; BillingReleased libera para faturar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostOrdersChangeStatus(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderStatusChange body);

        /// <summary>
        /// altera a capa do pedido de venda (POST sales-order/v2/header). Com o pedido no banco, operação e tabela de preço só mudam em
        /// andamento ou bloqueado (SO7)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostHeader(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderHeader body);

        /// <summary>
        /// aumenta a quantidade dos itens do pedido de venda (POST sales-order/v2/quantity-items); com o item no banco, a nova precisa ser
        /// maior que a atual (SO7)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostQuantityItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsQuantity body);

        /// <summary>
        /// inclui itens no pedido de venda (POST sales-order/v2/items); o mesmo corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdd body);

        /// <summary>
        /// remove um item do pedido de venda (DELETE sales-order/v2/items, com os campos na query)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> DeleteItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemDelete parameters);

        /// <summary>
        /// liga contagens ao pedido de venda (POST sales-order/v2/relationship-counts); o mesmo corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostRelationshipCounts(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderCounts body);

        /// <summary>
        /// liga itens de lote aos itens do pedido de venda (POST sales-order/v2/relationship-batch-items); o mesmo corpo em 10 minutos volta 409
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostRelationshipBatchItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItems body);

        /// <summary>
        /// remove o vínculo de um item de lote com o item do pedido (DELETE sales-order/v2/batch-items, com os campos na query)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> DeleteBatchItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemDelete parameters);

        /// <summary>
        /// o ERP escolhe e grava os itens de lote do pedido pelo nuance de cor (POST sales-order/v2/suggest-batch-items-nuance); o mesmo
        /// pedido em 10 minutos volta 409
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostSuggestBatchItemsNuance(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemsNuance body);

        /// <summary>
        /// inclui um pedido de venda B2C (POST sales-order/v2/b2c-orders); a resposta traz loja, número e orderId. O orderId é conferido antes
        /// pelo orders/search (SO5): existente volta 422; sem como conferir, não inclui. O mesmo corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostB2COrders(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders.B2COrder body);

        /// <summary>
        /// cancela o pedido de venda inteiro (POST sales-order/v2/orders/cancel), sem volta pela API; com o pedido no banco, cancelado ou atendido
        /// volta 422 (SO7)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostOrdersCancel(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderCancel body);

        /// <summary>
        /// altera o preço dos itens do pedido de venda (POST sales-order/v2/price-items)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPriceItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsPrice body);

        /// <summary>
        /// cancela quantidades dos itens do pedido de venda (POST sales-order/v2/cancel-items); com o item no banco, até a pendente (SO7). O mesmo
        /// corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostCancelItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsCancel body);

        /// <summary>
        /// liga o pedido de venda a uma transação do TOTVS Moda PDV (POST sales-order/v2/pdv-transactions); o mesmo corpo em 10 minutos volta 409
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPdvTransactions(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderPdvTransaction body);
    }
}
