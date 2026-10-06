using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxClientesEnderecosEntregaRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxClientesEnderecosEntrega? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxClientesEnderecosEntrega> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros);
    }
}
