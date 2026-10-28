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

        /// <summary>
        /// devolve o XML de uma NF-e/NFC-e pela chave de acesso (GET fiscal/v2/xml-contents/{accessKey}), sem gravar; a chamada fica em
        /// totvs.TotvsRequests só com um resumo
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetXmlContents(string user_name, string doc_company, string accessKey);

        /// <summary>
        /// devolve o PDF do DANFE de uma NF-e pela chave de acesso: lê o XML (xml-contents) e o envia ao POST fiscal/v2/danfe-search
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostSearchDanfe(string user_name, string doc_company, string accessKey, int? nfeDocumentType);

        /// <summary>
        /// devolve o PDF da impressão da transação de uma nota (POST fiscal/v2/transaction/print), com o modelo do TRAFP004 informado
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostTransactionPrint(string user_name, string doc_company, int branchCode, int invoiceSequence, DateTime invoiceDate, int transactionPrintTemplate, int transactionPrintType);
    }
}
