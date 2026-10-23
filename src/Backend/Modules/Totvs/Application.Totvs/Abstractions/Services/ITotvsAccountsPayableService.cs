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
    }
}
