namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Imagens de produto do B2C. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CProductImagesService
    {
        /// <summary>
        /// B2CConsultaProdutosImagens
        /// </summary>
        public Task<bool?> B2CConsultaProdutosImagens();

        /// <summary>
        /// B2CConsultaProdutosImagensURL
        /// </summary>
        public Task<bool?> B2CConsultaProdutosImagensURL();

        /// <summary>
        /// B2CConsultaImagens
        /// </summary>
        public Task<bool?> B2CConsultaImagens();

        /// <summary>
        /// B2CConsultaImagensHD
        /// </summary>
        public Task<bool?> B2CConsultaImagensHD();
    }
}
