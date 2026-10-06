using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxPedidosVendaRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxPedidosVenda? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxPedidosVenda> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros);
    }
}
