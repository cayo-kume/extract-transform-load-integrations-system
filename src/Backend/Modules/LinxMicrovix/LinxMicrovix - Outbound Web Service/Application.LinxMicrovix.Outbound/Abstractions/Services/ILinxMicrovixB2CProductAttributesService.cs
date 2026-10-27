namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Atributos de produto do B2C. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CProductAttributesService
    {
        /// <summary>
        /// B2CConsultaClassificacao
        /// </summary>
        public Task<bool?> B2CConsultaClassificacao();

        /// <summary>
        /// B2CConsultaColecoes
        /// </summary>
        public Task<bool?> B2CConsultaColecoes();

        /// <summary>
        /// B2CConsultaEspessuras
        /// </summary>
        public Task<bool?> B2CConsultaEspessuras();

        /// <summary>
        /// B2CConsultaGrade1
        /// </summary>
        public Task<bool?> B2CConsultaGrade1();

        /// <summary>
        /// B2CConsultaGrade2
        /// </summary>
        public Task<bool?> B2CConsultaGrade2();

        /// <summary>
        /// B2CConsultaLinhas
        /// </summary>
        public Task<bool?> B2CConsultaLinhas();

        /// <summary>
        /// B2CConsultaMarcas
        /// </summary>
        public Task<bool?> B2CConsultaMarcas();

        /// <summary>
        /// B2CConsultaSetores
        /// </summary>
        public Task<bool?> B2CConsultaSetores();

        /// <summary>
        /// B2CConsultaUnidade
        /// </summary>
        public Task<bool?> B2CConsultaUnidade();

        /// <summary>
        /// B2CConsultaFornecedores
        /// </summary>
        public Task<bool?> B2CConsultaFornecedores();

        /// <summary>
        /// B2CConsultaFlags
        /// </summary>
        public Task<bool?> B2CConsultaFlags();

        /// <summary>
        /// B2CConsultaTags
        /// </summary>
        public Task<bool?> B2CConsultaTags();

        /// <summary>
        /// B2CConsultaPalavrasChavePesquisa
        /// </summary>
        public Task<bool?> B2CConsultaPalavrasChavePesquisa();

        /// <summary>
        /// B2CConsultaLegendasCadastrosAuxiliares
        /// </summary>
        public Task<bool?> B2CConsultaLegendasCadastrosAuxiliares();
    }
}
