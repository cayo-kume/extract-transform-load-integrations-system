namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsInvoicesService
    {
        /// <summary>
        /// sincroniza as notas fiscais de todas as lojas da holding pelo POST fiscal/v2/invoices/search; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchInvoices();
    }
}
