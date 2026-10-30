namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Campos adicionais de produto do B2C. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CProductFieldsService
    {
        /// <summary>
        /// B2CConsultaProdutosCamposAdicionaisDetalhes
        /// </summary>
        public Task<bool?> B2CConsultaProdutosCamposAdicionaisDetalhes();

        /// <summary>
        /// B2CConsultaProdutosCamposAdicionaisNomes
        /// </summary>
        public Task<bool?> B2CConsultaProdutosCamposAdicionaisNomes();

        /// <summary>
        /// B2CConsultaProdutosCamposAdicionaisValores
        /// </summary>
        public Task<bool?> B2CConsultaProdutosCamposAdicionaisValores();

        /// <summary>
        /// B2CConsultaB2CProdutosCamposAdicionais
        /// </summary>
        public Task<bool?> B2CConsultaB2CProdutosCamposAdicionais();

        /// <summary>
        /// B2CConsultaB2CProdutosCamposAdicionaisDados
        /// </summary>
        public Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisDados();

        /// <summary>
        /// B2CConsultaB2CProdutosCamposAdicionaisMedia
        /// </summary>
        public Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisMedia();

        /// <summary>
        /// B2CConsultaB2CProdutosCamposAdicionaisMediaDados
        /// </summary>
        public Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisMediaDados();

        /// <summary>
        /// B2CConsultaB2CProdutosCamposAdicionaisOpcoes
        /// </summary>
        public Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisOpcoes();
    }
}
