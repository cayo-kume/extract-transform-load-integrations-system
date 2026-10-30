namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Estoque, custos e preços do B2C. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CStocksAndPricesService
    {
        /// <summary>
        /// B2CConsultaProdutosCustos
        /// </summary>
        public Task<bool?> B2CConsultaProdutosCustos();

        /// <summary>
        /// B2CConsultaProdutosDepositos
        /// </summary>
        public Task<bool?> B2CConsultaProdutosDepositos();

        /// <summary>
        /// B2CConsultaProdutosDetalhes
        /// </summary>
        public Task<bool?> B2CConsultaProdutosDetalhes();

        /// <summary>
        /// B2CConsultaProdutosDetalhesDepositos
        /// </summary>
        public Task<bool?> B2CConsultaProdutosDetalhesDepositos();

        /// <summary>
        /// B2CConsultaProdutosPromocao
        /// </summary>
        public Task<bool?> B2CConsultaProdutosPromocao();

        /// <summary>
        /// B2CConsultaProdutosTabelas
        /// </summary>
        public Task<bool?> B2CConsultaProdutosTabelas();

        /// <summary>
        /// B2CConsultaProdutosTabelasPrecos
        /// </summary>
        public Task<bool?> B2CConsultaProdutosTabelasPrecos();
    }
}
