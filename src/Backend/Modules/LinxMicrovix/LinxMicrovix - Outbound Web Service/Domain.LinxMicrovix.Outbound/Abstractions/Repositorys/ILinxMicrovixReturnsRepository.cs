using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixReturnsRepository
    {
        public Task<bool> BulkInsertIntoLinxMovimentoTrocas(List<LinxMovimentoTrocas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoTrocafone(List<LinxMovimentoTrocafone> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoDevolucoesItens(List<LinxMovimentoDevolucoesItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoOrigemDevolucoes(List<LinxMovimentoOrigemDevolucoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMotivoDevolucao(List<LinxMotivoDevolucao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxTrocaUnificadaResumoVendas(List<LinxTrocaUnificadaResumoVendas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxTrocaUnificadaResumoVendasItens(List<LinxTrocaUnificadaResumoVendasItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxTrocaUnificadaResumoBaixa(List<LinxTrocaUnificadaResumoBaixa> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxTradeinParceiro(List<LinxTradeinParceiro> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDevolucaoRemanejoFabrica(List<LinxDevolucaoRemanejoFabrica> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDevolucaoRemanejoFabricaItem(List<LinxDevolucaoRemanejoFabricaItem> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDevolucaoRemanejoFabricaStatus(List<LinxDevolucaoRemanejoFabricaStatus> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDevolucaoRemanejoFabricaTipo(List<LinxDevolucaoRemanejoFabricaTipo> data, Guid? parentExecutionGUID);
    }
}
