namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Pagamentos e caixa. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixPaymentsService
    {
        /// <summary>
        /// informações dos cartões de acordo com a loja pesquisada
        /// </summary>
        public Task<bool?> LinxMovimentoCartoes();

        /// <summary>
        /// os Planos de Pagamentos realizados nas movimentações
        /// </summary>
        public Task<bool?> LinxMovimentoPlanos();

        /// <summary>
        /// o cadastro dos Planos de Pagamentos do portal
        /// </summary>
        public Task<bool?> LinxPlanos();

        /// <summary>
        /// LinxPlanosParcelas
        /// </summary>
        public Task<bool?> LinxPlanosParcelas();

        /// <summary>
        /// o cadastro de bandeiras de cartões vinculadas aos Planos de Pagamento do Portal
        /// </summary>
        public Task<bool?> LinxPlanosBandeiras();

        /// <summary>
        /// os registro de giftcard
        /// </summary>
        public Task<bool?> LinxMovimentoGiftCard();

        /// <summary>
        /// vale Compras importado via API.
        /// </summary>
        public Task<bool?> LinxValesComprasEnviadosAPI();

        /// <summary>
        /// as informações das Sangrias ou Suprimentos feitos no caixa
        /// </summary>
        public Task<bool?> LinxSangriaSuprimentos();

        /// <summary>
        /// LinxFechamentoCaixa
        /// </summary>
        public Task<bool?> LinxFechamentoCaixa();

        /// <summary>
        /// sob demanda: busca LinxMovimentoCartoes por identificador, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxMovimentoCartoesByIdentificador(string user_name, string doc_company, string cnpj_emp, string identificador);

        /// <summary>
        /// sob demanda: busca LinxMovimentoPlanos por identificador, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxMovimentoPlanosByIdentificador(string user_name, string doc_company, string cnpj_emp, string identificador);

        /// <summary>
        /// sob demanda: busca LinxPlanos por plano, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxPlanosByPlano(string user_name, string doc_company, string plano);
    }
}
