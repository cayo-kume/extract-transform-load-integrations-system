using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixPosSalesRepository
    {
        public Task<bool> BulkInsertIntoLinxVendasPosTmp(List<LinxVendasPosTmp> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxVendasPosProdutosTmp(List<LinxVendasPosProdutosTmp> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxVendasPosAgrupamentoTmp(List<LinxVendasPosAgrupamentoTmp> data, Guid? parentExecutionGUID);
    }
}
