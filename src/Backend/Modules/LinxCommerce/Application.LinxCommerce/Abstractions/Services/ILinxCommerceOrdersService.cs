namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ILinxCommerceOrdersService
    {
        /// <summary>
        /// sincroniza os pedidos alterados desde a última leitura (POST Sales/SearchOrders pela ModifiedDate e GetOrder de cada um);
        /// só grava os novos ou alterados
        /// </summary>
        public Task<bool?> SearchOrders();

        /// <summary>
        /// sincroniza os status de pedido (POST Sales/SearchOrderStatus, lidos inteiros)
        /// </summary>
        public Task<bool?> SearchOrderStatus();

        /// <summary>
        /// busca um pedido pelo OrderID (POST Sales/GetOrder) e grava sempre, sem consultar o cache; a chamada é registrada em
        /// linxcommerce.LinxCommerceRequests. true gravado, false não existe no Linx Commerce, null empresa não cadastrada ou
        /// resposta inválida
        /// </summary>
        public Task<bool?> GetOrder(string user_name, string doc_company, Guid orderId);

        /// <summary>
        /// busca um pedido pelo número (POST Sales/GetOrderByNumber); mesmo contrato do GetOrder
        /// </summary>
        public Task<bool?> GetOrderByNumber(string user_name, string doc_company, string orderNumber);
    }
}
