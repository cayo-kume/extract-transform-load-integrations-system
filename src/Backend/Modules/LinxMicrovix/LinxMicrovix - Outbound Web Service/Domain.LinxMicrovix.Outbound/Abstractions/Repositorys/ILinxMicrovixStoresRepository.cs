using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixStoresRepository
    {
        public Task<bool> BulkInsertIntoLinxGrupoLojas(List<LinxGrupoLojas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxLojas(List<LinxLojas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxLojasParametros(List<LinxLojasParametros> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxUsuarios(List<LinxUsuarios> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMetodos(List<LinxMetodos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxEntrantes(List<LinxEntrantes> data, Guid? parentExecutionGUID);
    }
}
