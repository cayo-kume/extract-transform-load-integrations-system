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
    }
}
