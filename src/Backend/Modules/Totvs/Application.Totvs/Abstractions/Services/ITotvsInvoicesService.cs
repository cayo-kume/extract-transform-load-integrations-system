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

        /// <summary>
        /// sincroniza as faixas de numeração inutilizadas de todas as lojas pelo GET fiscal/v2/invoices/disable (janela de alteração);
        /// só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> GetInvoicesDisable();

        /// <summary>
        /// sincroniza os centros de custo, com as classificações, pelo GET fiscal/v2/cost-center, lidos inteiros; só grava os novos ou
        /// cujo hash mudou
        /// </summary>
        public Task<bool?> GetCostCenter();
    }
}
