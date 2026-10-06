using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxGrupoLojasRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxGrupoLojas? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxGrupoLojas> records);
        public Task<IEnumerable<string?>> GetRegistersExists();
    }
}
