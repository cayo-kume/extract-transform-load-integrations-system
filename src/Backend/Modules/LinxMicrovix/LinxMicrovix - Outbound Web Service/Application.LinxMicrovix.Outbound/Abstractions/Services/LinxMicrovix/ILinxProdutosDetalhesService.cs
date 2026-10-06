using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix
{
    public interface ILinxProdutosDetalhesService
    {
        public List<LinxProdutosDetalhes?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records);
        public Task<bool> GetRecords(LinxAPIParam jobParameter);
        public Task<bool> GetZeroStockRecords(LinxAPIParam jobParameter);
        public Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp);
    }
}
