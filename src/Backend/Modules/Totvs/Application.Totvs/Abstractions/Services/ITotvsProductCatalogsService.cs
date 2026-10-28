namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// catálogos da API Product (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.1, PR12)
    /// </summary>
    public interface ITotvsProductCatalogsService
    {
        /// <summary>
        /// sincroniza o catálogo de tipos de classificação de produto pelo GET product/v2/classificationType (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetClassificationType();

        /// <summary>
        /// sincroniza o catálogo de classificações de produto pelo GET product/v2/classifications (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetClassifications();

        /// <summary>
        /// sincroniza o catálogo de categorias de produto pelo GET product/v2/category (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetCategory();

        /// <summary>
        /// sincroniza o catálogo de grades pelo GET product/v2/grid (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetGrid();

        /// <summary>
        /// sincroniza o catálogo de unidades de medida pelo GET product/v2/measurement-unit (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetMeasurementUnit();

        /// <summary>
        /// sincroniza o catálogo de cores pelo POST product/v2/colors/search (lido inteiro, só grava as cores novas ou cujo hash mudou)
        /// </summary>
        public Task<bool?> PostSearchColors();

        /// <summary>
        /// sincroniza o catálogo de tipos de campo adicional pelo GET product/v2/additional-fields-types (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetAdditionalFieldsTypes();

        /// <summary>
        /// sincroniza o catálogo de configurações de agrupador de produto pelo GET product/v2/product-grouper-configuration (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetProductGrouperConfiguration();

        /// <summary>
        /// sincroniza o catálogo de itens de instrução pelo GET product/v2/instruction-items (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetInstructionItems();

        /// <summary>
        /// sincroniza o catálogo de composições pelo POST product/v2/compositions (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> PostCompositions();

        /// <summary>
        /// sincroniza o catálogo de cabeçalhos de tabela de preço pelo GET product/v2/price-tables-headers (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetPriceTablesHeaders();

        /// <summary>
        /// sincroniza o catálogo de escalas de tabela de preço pelo GET product/v2/price-table-scales (janela de alteração, como as filiais; só grava os novos ou alterados)
        /// </summary>
        public Task<bool?> GetPriceTableScales();

        /// <summary>
        /// inclui ou altera uma cor (POST product/v2/colors): um code que já existe é alterado, e o nome novo vale em todas as referências.
        /// As lojas dos dados adicionais precisam ser da empresa (422 sem chamar a API)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostColors(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Colors.UpsertColor body);

        /// <summary>
        /// inclui uma grade de tamanhos (POST product/v2/grids); a resposta traz o gridCode. Só inclui: gridCode ou nome que já existem voltam
        /// 400 AlreadyExist (testado), e os tamanhos de uma grade existente só mudam no ERP. Não idempotente: o mesmo corpo em 10 minutos
        /// volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostGrids(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Grids.InsertGrid body);

        /// <summary>
        /// inclui ou altera um tipo de classificação de produto e as classificações dele (POST product/v2/classifications). O tipo 6000 (marca)
        /// é recusado no controller (PR10)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostClassifications(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Classifications.UpsertTypeClassification body);
    }
}
