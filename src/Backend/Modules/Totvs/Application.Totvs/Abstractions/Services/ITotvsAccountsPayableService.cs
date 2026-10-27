namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsAccountsPayableService
    {
        /// <summary>
        /// sincroniza as duplicatas (parcelas) a pagar de todas as lojas pelo POST accounts-payable/v2/duplicates/search; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchDuplicates();

        /// <summary>
        /// sincroniza os fechamentos de comissão pelo POST accounts-payable/v2/comissions-paid/search, uma chamada por loja (a rota não
        /// tem filtro de alteração: a janela é pela data do fechamento e é relida a cada execução); só grava os que mudaram (hash)
        /// </summary>
        public Task<bool?> PostSearchComissionsPaid();

        /// <summary>
        /// troca o estágio de uma parcela pelo POST accounts-payable/v2/duplicates/installments/stage. O corpo chega validado pelo
        /// controller; a chamada é registrada em totvs.TotvsRequests. Sem nova tentativa em 5xx (escrita)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostDuplicatesInstallmentsStage(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage body);
    }
}
