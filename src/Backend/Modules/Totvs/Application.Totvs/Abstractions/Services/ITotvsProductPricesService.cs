namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// preços e custos dos produtos (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, PR12); as tabelas de preço, as promoções e as
    /// escritas de valor (§5.2 e §5.7) entram aqui nas próximas partes
    /// </summary>
    public interface ITotvsProductPricesService
    {
        /// <summary>
        /// sincroniza os preços de todos os tipos, por loja de cada marca, pelo POST product/v2/prices/search (carga inicial por produto,
        /// depois janela de alteração); só grava os produtos novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchPrices();

        /// <summary>
        /// sincroniza os custos de todos os tipos, por loja de cada marca, pelo POST product/v2/costs/search (carga inicial por produto,
        /// depois janela de alteração); só grava os produtos novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchCosts();
    }
}
