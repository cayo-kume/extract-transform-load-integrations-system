namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Produtos do B2C. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CProductsService
    {
        /// <summary>
        /// B2CConsultaProdutos
        /// </summary>
        public Task<bool?> B2CConsultaProdutos();

        /// <summary>
        /// B2CConsultaProdutosAssociados
        /// </summary>
        public Task<bool?> B2CConsultaProdutosAssociados();

        /// <summary>
        /// B2CConsultaProdutosCampanhas
        /// </summary>
        public Task<bool?> B2CConsultaProdutosCampanhas();

        /// <summary>
        /// B2CConsultaProdutosCodebar
        /// </summary>
        public Task<bool?> B2CConsultaProdutosCodebar();

        /// <summary>
        /// B2CConsultaProdutosDimensoes
        /// </summary>
        public Task<bool?> B2CConsultaProdutosDimensoes();

        /// <summary>
        /// B2CConsultaProdutosInformacoes
        /// </summary>
        public Task<bool?> B2CConsultaProdutosInformacoes();

        /// <summary>
        /// B2CConsultaProdutosStatus
        /// </summary>
        public Task<bool?> B2CConsultaProdutosStatus();

        /// <summary>
        /// B2CConsultaProdutosFlags
        /// </summary>
        public Task<bool?> B2CConsultaProdutosFlags();

        /// <summary>
        /// B2CConsultaProdutosTags
        /// </summary>
        public Task<bool?> B2CConsultaProdutosTags();

        /// <summary>
        /// B2CConsultaProdutosPalavrasChavePesquisa
        /// </summary>
        public Task<bool?> B2CConsultaProdutosPalavrasChavePesquisa();
    }
}
