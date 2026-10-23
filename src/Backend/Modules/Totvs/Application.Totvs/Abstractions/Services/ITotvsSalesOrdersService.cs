namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsSalesOrdersService
    {
        /// <summary>
        /// sincroniza os pedidos de venda de todas as lojas da holding pelo POST sales-order/v2/orders/search; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchSalesOrders();
    }
}
