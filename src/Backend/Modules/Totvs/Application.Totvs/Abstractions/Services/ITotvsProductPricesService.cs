namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// preços e custos dos produtos (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, PR12) e itens das tabelas de preço e das promoções
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

        /// <summary>
        /// inclui preço ou custo dos produtos (POST product/v2/values/create), com os códigos e lojas permitidos e a trava de variação (PR7); código
        /// que o produto já tem na loja volta 400 AlreadyExist (testado): a alteração é pelo PostUpdateValues
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostCreateValues(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.ProductValues body, bool confirmVariation);

        /// <summary>
        /// altera preço ou custo dos produtos (POST product/v2/values/update), com os códigos e lojas permitidos e a trava de variação (PR7); sem
        /// valor gravado para conferir, recusa salvo confirmVariation
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostUpdateValues(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.ProductValues body, bool confirmVariation);

        /// <summary>
        /// altera valores de promoção dos produtos numa campanha (POST product/v2/promotion-values/update); com a promoção no banco, só aberta e
        /// com o valor abaixo do anterior
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostUpdatePromotionValues(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.PromotionItems body);

        /// <summary>
        /// inclui produtos numa campanha (POST product/v2/promotion-items); com a promoção no banco, só aberta; o mesmo corpo em 10 minutos volta 409
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostPromotionItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.PromotionItems body);
    }
}
