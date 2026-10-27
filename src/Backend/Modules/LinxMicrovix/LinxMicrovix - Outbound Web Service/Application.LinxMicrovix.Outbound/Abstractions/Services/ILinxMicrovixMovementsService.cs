namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Movimento. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixMovementsService
    {
        /// <summary>
        /// as movimentações de entradas e saídas de acordo com a loja e data pesquisada
        /// </summary>
        public Task<bool?> LinxMovimento();

        /// <summary>
        /// dados da movimento principal.
        /// </summary>
        public Task<bool?> LinxMovimentoPrincipal();

        /// <summary>
        /// as informações adicionais da venda
        /// </summary>
        public Task<bool?> LinxMovimentoExtensao();

        /// <summary>
        /// os campos adicionais da movimentação
        /// </summary>
        public Task<bool?> LinxMovimentoCamposAdicionais();

        /// <summary>
        /// o conteúdo da tabela movimento_observacoes
        /// </summary>
        public Task<bool?> LinxMovimentoObservacoes();

        /// <summary>
        /// as observações do cupom fiscal.
        /// </summary>
        public Task<bool?> LinxMovimentoObservacaoCF();

        /// <summary>
        /// o cliente indicador da venda.
        /// </summary>
        public Task<bool?> LinxMovimentoIndicacoes();

        /// <summary>
        /// os Código de Seriais realizadas nas movimentações
        /// </summary>
        public Task<bool?> LinxMovimentoSerial();

        /// <summary>
        /// os registros da tabela serial_venda
        /// </summary>
        public Task<bool?> LinxSerialVenda();

        /// <summary>
        /// movimento da venda conjugada
        /// </summary>
        public Task<bool?> LinxMovimentoVendaConjugada();

        /// <summary>
        /// as perguntas realizadas no final da venda cadastradas no portal.
        /// </summary>
        public Task<bool?> LinxPerguntaVenda();

        /// <summary>
        /// o cadastro das perguntas realizadas no final da venda no portal.
        /// </summary>
        public Task<bool?> LinxRespostaVenda();

        /// <summary>
        /// sob demanda: busca LinxMovimento por documento, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxMovimentoByDocumento(string user_name, string doc_company, string cnpj_emp, string documento);
    }
}
