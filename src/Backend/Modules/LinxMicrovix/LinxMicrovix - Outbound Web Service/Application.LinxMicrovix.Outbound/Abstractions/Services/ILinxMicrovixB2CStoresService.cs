namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Empresas, vendedores, transportadores e planos do B2C. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CStoresService
    {
        /// <summary>
        /// B2CConsultaEmpresas
        /// </summary>
        public Task<bool?> B2CConsultaEmpresas();

        /// <summary>
        /// B2CConsultaCNPJsChave
        /// </summary>
        public Task<bool?> B2CConsultaCNPJsChave();

        /// <summary>
        /// B2CConsultaVendedores
        /// </summary>
        public Task<bool?> B2CConsultaVendedores();

        /// <summary>
        /// B2CConsultaTransportadores
        /// </summary>
        public Task<bool?> B2CConsultaTransportadores();

        /// <summary>
        /// B2CConsultaFormasPagamento
        /// </summary>
        public Task<bool?> B2CConsultaFormasPagamento();

        /// <summary>
        /// B2CConsultaPlanos
        /// </summary>
        public Task<bool?> B2CConsultaPlanos();

        /// <summary>
        /// B2CConsultaPlanosParcelas
        /// </summary>
        public Task<bool?> B2CConsultaPlanosParcelas();

        /// <summary>
        /// B2CConsultaTiposCobrancaFrete
        /// </summary>
        public Task<bool?> B2CConsultaTiposCobrancaFrete();

        /// <summary>
        /// B2CConsultaTipoEncomenda
        /// </summary>
        public Task<bool?> B2CConsultaTipoEncomenda();
    }
}
