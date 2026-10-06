using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxPlanosRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxPlanos? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxPlanos> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros);
    }
}
