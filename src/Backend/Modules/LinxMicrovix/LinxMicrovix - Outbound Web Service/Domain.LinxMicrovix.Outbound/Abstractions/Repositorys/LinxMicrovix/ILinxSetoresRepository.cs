using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxSetoresRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxSetores? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxSetores> records);
        public Task<IEnumerable<string>> GetRegistersExists();
    }
}
