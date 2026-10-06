using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix
{
    public interface ILinxProdutosTabelasPrecosService
    {
        public List<LinxProdutosTabelasPrecos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records);
        public Task<bool> GetRecords(LinxAPIParam jobParameter);
        public Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? identificador2, string? cnpj_emp);
    }
}
