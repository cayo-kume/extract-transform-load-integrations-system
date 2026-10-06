using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxClientesFornecRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxClientesFornec? record);
        public Task<bool> InsertRecord(string identifier, bool is_present_in_erp);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxClientesFornec> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<string?> registros);
    }
}
