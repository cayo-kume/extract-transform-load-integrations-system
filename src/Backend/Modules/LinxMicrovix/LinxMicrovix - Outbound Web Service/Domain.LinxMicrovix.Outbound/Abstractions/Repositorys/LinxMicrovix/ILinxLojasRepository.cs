using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxLojasRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxLojas? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxLojas> records);
        public Task<IEnumerable<string>> GetRegistersExists();
    }
}
