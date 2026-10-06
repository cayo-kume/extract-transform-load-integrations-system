using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxCfopFiscalRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxCfopFiscal? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxCfopFiscal> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<Int32> registros);
    }
}
