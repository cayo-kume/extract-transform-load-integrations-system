using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce
{
    public interface IB2CConsultaPedidosItensService
    {
        public List<B2CConsultaPedidosItens?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records);
        public Task<bool> GetRecords(LinxAPIParam jobParameter);
        public Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador);
        public Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter);
    }
}
