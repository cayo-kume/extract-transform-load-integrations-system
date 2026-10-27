using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixServiceOrdersRepository
    {
        public Task<bool> BulkInsertIntoLinxOrdensServico(List<LinxOrdensServico> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOrdensServicoProdutos(List<LinxOrdensServicoProdutos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxServicos(List<LinxServicos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxServicosDetalhes(List<LinxServicosDetalhes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxServicoClientes(List<LinxServicoClientes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxServicoAgendaComandas(List<LinxServicoAgendaComandas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxServicoAgendaEventos(List<LinxServicoAgendaEventos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOrdemServicoExternaDav(List<LinxOrdemServicoExternaDav> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoOrdemServicoExterna(List<LinxMovimentoOrdemServicoExterna> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxValeOrdemServicoExterna(List<LinxValeOrdemServicoExterna> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPagamentoParcialDav(List<LinxPagamentoParcialDav> data, Guid? parentExecutionGUID);
    }
}
