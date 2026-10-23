namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsPurchaseOrdersService
    {
        /// <summary>
        /// sincroniza os pedidos de compra de todas as lojas da holding pelo POST purchase-order/v2/search; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchPurchaseOrders();
    }
}
