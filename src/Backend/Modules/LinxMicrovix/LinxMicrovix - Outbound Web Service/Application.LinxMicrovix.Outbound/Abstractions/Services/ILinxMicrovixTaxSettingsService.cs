namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Configurações tributárias. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixTaxSettingsService
    {
        /// <summary>
        /// o cadastro de configurações tributárias da empresa
        /// </summary>
        public Task<bool?> LinxConfiguracoesTributarias();

        /// <summary>
        /// o detalhamento das configurações tributárias da empresa
        /// </summary>
        public Task<bool?> LinxConfiguracoesTributariasDetalhes();

        /// <summary>
        /// o detalhamento das configurações tributarias de forma simplificada.
        /// </summary>
        public Task<bool?> LinxConfiguracoesTributariasDetalhesSimplificado();

        /// <summary>
        /// o conteúdo da tabela config_tributaria_empresa.
        /// </summary>
        public Task<bool?> LinxConfiguracoesTributariasEmpresas();
    }
}
