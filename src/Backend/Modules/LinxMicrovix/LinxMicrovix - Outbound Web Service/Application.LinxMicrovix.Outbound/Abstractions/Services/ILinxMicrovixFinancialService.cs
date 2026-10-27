namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Financeiro. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixFinancialService
    {
        /// <summary>
        /// as faturas a receber ou a pagar de uma loja
        /// </summary>
        public Task<bool?> LinxFaturas();

        /// <summary>
        /// as faturas de origem
        /// </summary>
        public Task<bool?> LinxFaturasOrigem();

        /// <summary>
        /// os dados dos portadores.
        /// </summary>
        public Task<bool?> LinxPortadores();

        /// <summary>
        /// informações das antecipações financeiras do portal
        /// </summary>
        public Task<bool?> LinxAntecipacoesFinanceiras();

        /// <summary>
        /// os dados da tabela antecipacoes_financeiras  e antecipacoes_financeiras_planos no portal.
        /// </summary>
        public Task<bool?> LinxAntecipacoesFinanceirasPlanos();

        /// <summary>
        /// os valores registrados na tabela desfazimentos_antecipacoes
        /// </summary>
        public Task<bool?> LinxDesfazimentosAntecipacoes();

        /// <summary>
        /// os valores registrados na tabela desfazimentos_antecipacoes_documento
        /// </summary>
        public Task<bool?> LinxDesfazimentosAntecipacoesDocumento();

        /// <summary>
        /// os valores registrados na tabela desfazimentos_antecipacoes_itens
        /// </summary>
        public Task<bool?> LinxDesfazimentosAntecipacoesItens();

        /// <summary>
        /// categoria financeira
        /// </summary>
        public Task<bool?> LinxCategoriasFinanceiras();

        /// <summary>
        /// as informações dos centros de custos cadastrados no portal
        /// </summary>
        public Task<bool?> LinxCentroCusto();

        /// <summary>
        /// as informações do plano de contas cadastrado no portal
        /// </summary>
        public Task<bool?> LinxPlanoContas();

        /// <summary>
        /// informações dos Lançamentos Contábeis por conta e empresa
        /// </summary>
        public Task<bool?> LinxLancContabil();

        /// <summary>
        /// os dados dos históricos contábeis.
        /// </summary>
        public Task<bool?> LinxHistoricosContabeis();

        /// <summary>
        /// os registros da tabela extrato_bancario_lancamentos.
        /// </summary>
        public Task<bool?> LinxExtratoBancarioLancamentos();

        /// <summary>
        /// os registros da tabela extrato_bancario_conciliacoes.
        /// </summary>
        public Task<bool?> LinxExtratoBancarioConciliacoes();
    }
}
