namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsInvoicesService
    {
        /// <summary>
        /// sincroniza as notas fiscais de todas as lojas da holding pelo POST fiscal/v2/invoices/search; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchInvoices();

        /// <summary>
        /// busca uma nota fiscal por loja + sequência (POST fiscal/v2/invoices/search com branchCodeList + invoiceSequenceList) e grava
        /// sempre, sem consultar o cache; a chamada é registrada em totvs.TotvsRequests. true gravada, false não existe na Totvs,
        /// null empresa não cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetInvoiceByKey(string user_name, string doc_company, int branchCode, int invoiceSequence);
    }
}
