namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Ordens de serviço. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixServiceOrdersService
    {
        /// <summary>
        /// as ordens de serviço
        /// </summary>
        public Task<bool?> LinxOrdensServico();

        /// <summary>
        /// os produtos das ordens de serviço
        /// </summary>
        public Task<bool?> LinxOrdensServicoProdutos();

        /// <summary>
        /// o cadastro de serviços de acordo com a loja pesquisada ou dados da estrutura mercadológica
        /// </summary>
        public Task<bool?> LinxServicos();

        /// <summary>
        /// informações dos serviços de acordo com a loja pesquisada
        /// </summary>
        public Task<bool?> LinxServicosDetalhes();

        /// <summary>
        /// os dados dos clientes do serviço.
        /// </summary>
        public Task<bool?> LinxServicoClientes();

        /// <summary>
        /// a agenda das comandas dos serviços.
        /// </summary>
        public Task<bool?> LinxServicoAgendaComandas();

        /// <summary>
        /// a agenda dos eventos dos serviços.
        /// </summary>
        public Task<bool?> LinxServicoAgendaEventos();

        /// <summary>
        /// as ordens de serviço externas das DAVs
        /// </summary>
        public Task<bool?> LinxOrdemServicoExternaDav();

        /// <summary>
        /// a ordem de serviço externa da movimentação.
        /// </summary>
        public Task<bool?> LinxMovimentoOrdemServicoExterna();

        /// <summary>
        /// os dados de vale ordem de serviço externa.
        /// </summary>
        public Task<bool?> LinxValeOrdemServicoExterna();

        /// <summary>
        /// os pagamentos parciais da DAVs
        /// </summary>
        public Task<bool?> LinxPagamentoParcialDav();
    }
}
