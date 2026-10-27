namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Documentos fiscais. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixFiscalDocumentsService
    {
        /// <summary>
        /// o XML dos documentos.
        /// </summary>
        public Task<bool?> LinxXMLDocumentos();

        /// <summary>
        /// os dados eventos da NFe.
        /// </summary>
        public Task<bool?> LinxNFeEvento();

        /// <summary>
        /// nota Fiscal de Serviços Eletrônica
        /// </summary>
        public Task<bool?> LinxNfse();

        /// <summary>
        /// dados de NFes de entrada.
        /// </summary>
        public Task<bool?> LinxEntradaNFe();

        /// <summary>
        /// dados de itens de NFes de entrada.
        /// </summary>
        public Task<bool?> LinxEntradaNFeProdutos();

        /// <summary>
        /// dados de conhecimento de fretes.
        /// </summary>
        public Task<bool?> LinxConhecimentoFretes();

        /// <summary>
        /// tipos de frete.
        /// </summary>
        public Task<bool?> LinxFreteTipos();

        /// <summary>
        /// as ReduçõesZ de acordo com a loja pesquisada, somente para clientes que ainda utilizam a impressora fiscal
        /// </summary>
        public Task<bool?> LinxReducoesZ();

        /// <summary>
        /// as ECFs cadastradas para a empresa.
        /// </summary>
        public Task<bool?> LinxECF();

        /// <summary>
        /// as formas de pagamento da ECF.
        /// </summary>
        public Task<bool?> LinxECFFormasPagamento();

        /// <summary>
        /// cadastro de estações.
        /// </summary>
        public Task<bool?> LinxNfceEstacao();

        /// <summary>
        /// as séries cadastradas no portal.
        /// </summary>
        public Task<bool?> LinxSeries();

        /// <summary>
        /// sob demanda: busca LinxXMLDocumentos por documento, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxXMLDocumentosByDocumento(string user_name, string doc_company, string cnpj_emp, string documento);
    }
}
