namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Ótica. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixOpticsService
    {
        /// <summary>
        /// os tipos de produtos opticos.
        /// </summary>
        public Task<bool?> LinxProdutosOpticosTipo();

        /// <summary>
        /// os tipos de aros
        /// </summary>
        public Task<bool?> LinxProdutosOpticosTipoAro();

        /// <summary>
        /// os formatos de aros
        /// </summary>
        public Task<bool?> LinxProdutosOpticosFormatoAro();

        /// <summary>
        /// os dados ópticos dos produtos da DAV
        /// </summary>
        public Task<bool?> LinxDadosOpticosDav();

        /// <summary>
        /// descrição de prismas óticos.
        /// </summary>
        public Task<bool?> LinxOticoPrismaDescricao();

        /// <summary>
        /// receitas óticas.
        /// </summary>
        public Task<bool?> LinxOticoReceitas();

        /// <summary>
        /// o cadastro da posição da OS do Ramo Optico
        /// </summary>
        public Task<bool?> LinxPosicaoOsRamoOptico();

        /// <summary>
        /// o histórico das posições das ordens de serviço do ramo optico
        /// </summary>
        public Task<bool?> LinxOrdensServicoPosicaoOsRamoOpticoHistorico();

        /// <summary>
        /// os componentes de fórmulas relacionados ao orçamento.
        /// </summary>
        public Task<bool?> LinxOrcamentoComponenteFormula();

        /// <summary>
        /// os componentes de fórmulas relacionados ao Dav.
        /// </summary>
        public Task<bool?> LinxComponenteFormulaDav();
    }
}
