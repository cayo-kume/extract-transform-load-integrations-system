namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Tabelas fiscais. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixTaxesService
    {
        /// <summary>
        /// as naturezas de operação.
        /// </summary>
        public Task<bool?> LinxNaturezaOperacao();

        /// <summary>
        /// o cadastro da CFOP Fiscal
        /// </summary>
        public Task<bool?> LinxCfopFiscal();

        /// <summary>
        /// o cadastro de cst icms fiscal
        /// </summary>
        public Task<bool?> LinxCstIcmsFiscal();

        /// <summary>
        /// o conteúdo da tabela cst_ipi_fiscal
        /// </summary>
        public Task<bool?> LinxCstIpiFiscal();

        /// <summary>
        /// o conteúdo da tabela cst_pis_fiscal
        /// </summary>
        public Task<bool?> LinxCstPisFiscal();

        /// <summary>
        /// o conteúdo da tabela cst_cofins_fiscal
        /// </summary>
        public Task<bool?> LinxCstCofinsFiscal();

        /// <summary>
        /// o cadastro de csosn fiscal
        /// </summary>
        public Task<bool?> LinxCsosnFiscal();

        /// <summary>
        /// o cadastro da classe fiscal.
        /// </summary>
        public Task<bool?> LinxClasseFiscal();

        /// <summary>
        /// o conteúdo da tabela classificacao_cest_produto
        /// </summary>
        public Task<bool?> LinxCest();

        /// <summary>
        /// o conteúdo da tabela classificacao_ncm_produto.
        /// </summary>
        public Task<bool?> LinxNcm();

        /// <summary>
        /// o conteúdo da tabela enquadramento_ipi
        /// </summary>
        public Task<bool?> LinxEnquadramentoIpi();

        /// <summary>
        /// o conteúdo da tabela motivos_desoneracao_icms
        /// </summary>
        public Task<bool?> LinxMotivosDesoneracaoIcms();

        /// <summary>
        /// dados do Sped tipo item.
        /// </summary>
        public Task<bool?> LinxSpedTipoItem();

        /// <summary>
        /// o conteúdo da tabela sped_tipo_base_credito.
        /// </summary>
        public Task<bool?> LinxSpedTipoBaseCredito();

        /// <summary>
        /// retornas as rotinas origem cadastradas.
        /// </summary>
        public Task<bool?> LinxRotinaOrigem();

        /// <summary>
        /// dados do IBGE sobre municípios.
        /// </summary>
        public Task<bool?> LinxNfeIbgeMunicipio();

        /// <summary>
        /// sob demanda: busca LinxNaturezaOperacao por cod_natureza_operacao, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxNaturezaOperacaoByCodNaturezaOperacao(string user_name, string doc_company, string cod_natureza_operacao);
    }
}
