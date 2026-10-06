using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxPedidosCompraRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxPedidosCompra? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxPedidosCompra> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros);
    }
}
