namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Remessas. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixConsignmentsService
    {
        /// <summary>
        /// os dados da tabela remessas no portal.
        /// </summary>
        public Task<bool?> LinxMovimentoRemessas();

        /// <summary>
        /// os dados da tabela remessas_itens no portal.
        /// </summary>
        public Task<bool?> LinxMovimentoRemessasItens();

        /// <summary>
        /// os dados da tabela remessas_acertos no portal.
        /// </summary>
        public Task<bool?> LinxMovimentoRemessasAcertos();

        /// <summary>
        /// os dados da tabela remessas_acertos_itens no portal.
        /// </summary>
        public Task<bool?> LinxMovimentoRemessasAcertosItens();

        /// <summary>
        /// recebe o identificador da venda e retorna a relação com os identificadores das notas dos itens da remessa.
        /// </summary>
        public Task<bool?> LinxRemessasIdentificadores();

        /// <summary>
        /// os dados da tabela remessa_operacoes no portal.
        /// </summary>
        public Task<bool?> LinxRemessasOperacoes();

        /// <summary>
        /// os dados da tabela remessa_retorno_tipos no portal.
        /// </summary>
        public Task<bool?> LinxRemessasRetornoTipos();
    }
}
