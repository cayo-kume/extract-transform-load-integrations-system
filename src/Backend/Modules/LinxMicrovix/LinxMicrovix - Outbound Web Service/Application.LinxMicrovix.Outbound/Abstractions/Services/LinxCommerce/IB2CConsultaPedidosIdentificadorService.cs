using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce
{
    public interface IB2CConsultaPedidosIdentificadorService
    {
        public List<B2CConsultaPedidosIdentificador?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records);
        public Task<bool> GetRecords(LinxAPIParam jobParameter);
        public Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp);
        public Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter);
    }
}
