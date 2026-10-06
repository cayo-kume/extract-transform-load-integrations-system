using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxRotinaOrigemRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxRotinaOrigem? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxRotinaOrigem> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<Int32?> registros);
    }
}
