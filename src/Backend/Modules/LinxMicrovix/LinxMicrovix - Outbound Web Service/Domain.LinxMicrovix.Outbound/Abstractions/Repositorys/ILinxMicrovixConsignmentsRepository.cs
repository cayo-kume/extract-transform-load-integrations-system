using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixConsignmentsRepository
    {
        public Task<bool> BulkInsertIntoLinxMovimentoRemessas(List<LinxMovimentoRemessas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoRemessasItens(List<LinxMovimentoRemessasItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoRemessasAcertos(List<LinxMovimentoRemessasAcertos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoRemessasAcertosItens(List<LinxMovimentoRemessasAcertosItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRemessasIdentificadores(List<LinxRemessasIdentificadores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRemessasOperacoes(List<LinxRemessasOperacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRemessasRetornoTipos(List<LinxRemessasRetornoTipos> data, Guid? parentExecutionGUID);
    }
}
