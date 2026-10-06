using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix
{
    public interface ILinxPedidosCompraService
    {
        public List<LinxPedidosCompra?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records);
        public Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter);
        public Task<bool> GetRecords(LinxAPIParam jobParameter);
    }
}
