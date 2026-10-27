namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Trocas e devoluções. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixReturnsService
    {
        /// <summary>
        /// os valores e números das trocas realizadas nas vendas
        /// </summary>
        public Task<bool?> LinxMovimentoTrocas();

        /// <summary>
        /// os valores e números dos vouchers Trocafone realizadas nas vendas
        /// </summary>
        public Task<bool?> LinxMovimentoTrocafone();

        /// <summary>
        /// os dados da tabela movimento_devolucoes_itens cadastrados no portal.
        /// </summary>
        public Task<bool?> LinxMovimentoDevolucoesItens();

        /// <summary>
        /// os documentos de origem relacionados as devoluções
        /// </summary>
        public Task<bool?> LinxMovimentoOrigemDevolucoes();

        /// <summary>
        /// os motivos de devoluções cadastrados no ERP.
        /// </summary>
        public Task<bool?> LinxMotivoDevolucao();

        /// <summary>
        /// as vendas do troca unificada.
        /// </summary>
        public Task<bool?> LinxTrocaUnificadaResumoVendas();

        /// <summary>
        /// LinxTrocaUnificadaResumoVendasItens
        /// </summary>
        public Task<bool?> LinxTrocaUnificadaResumoVendasItens();

        /// <summary>
        /// os dados do resumo baixa do troca unificada
        /// </summary>
        public Task<bool?> LinxTrocaUnificadaResumoBaixa();

        /// <summary>
        /// os registros cadastrados na tabela tradein_parceiro do portal
        /// </summary>
        public Task<bool?> LinxTradeinParceiro();

        /// <summary>
        /// as devoluções e remanejos de fábrica.
        /// </summary>
        public Task<bool?> LinxDevolucaoRemanejoFabrica();

        /// <summary>
        /// os itens das devoluções e remanejos de fábrica.
        /// </summary>
        public Task<bool?> LinxDevolucaoRemanejoFabricaItem();

        /// <summary>
        /// o cadastro de status da rotina devoluções e remanejos de fábrica.
        /// </summary>
        public Task<bool?> LinxDevolucaoRemanejoFabricaStatus();

        /// <summary>
        /// o cadastro de tipos da rotina devoluções e remanejos de fábrica.
        /// </summary>
        public Task<bool?> LinxDevolucaoRemanejoFabricaTipo();

        /// <summary>
        /// sob demanda: busca LinxMovimentoTrocas por doc_origem, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxMovimentoTrocasByDocOrigem(string user_name, string doc_company, string cnpj_emp, string doc_origem);
    }
}
