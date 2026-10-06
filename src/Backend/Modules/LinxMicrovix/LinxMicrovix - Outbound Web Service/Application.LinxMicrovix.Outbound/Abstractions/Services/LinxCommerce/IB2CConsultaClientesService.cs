using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce
{
    public interface IB2CConsultaClientesService
    {
        public List<B2CConsultaClientes?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records);
        public Task<bool> GetRecords(LinxAPIParam jobParameter);
        public Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador);
        public Task<bool> IntegrityLockB2CConsultaClientesRegisters(LinxAPIParam jobParameter); //Refatorar Aqui (trocar nome para public Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter);)
    }
}
