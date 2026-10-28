namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// preços e custos dos produtos (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, PR12) e itens das tabelas de preço e das promoções
    /// (§5.2); as escritas de valor (§5.7) entram aqui nas próximas fases
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

        /// <summary>
        /// sincroniza os itens das tabelas de preço vigentes (de TotvsPriceTables), com os preços por loja e escala, pelo POST
        /// product/v2/price-tables/search (carga inicial por produto, depois janela de alteração)
        /// </summary>
        public Task<bool?> PostSearchPriceTables();

        /// <summary>
        /// sincroniza os itens de promoção de todas as lojas pelo GET product/v2/promotion, lidos inteiros (o filtro de data da rota dá
        /// erro na Totvs); só grava os itens novos ou cujo hash mudou
        /// </summary>
        public Task<bool?> GetPromotion();
    }
}
