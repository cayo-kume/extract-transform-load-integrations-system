using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxProdutosCodBarRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosCodBar? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosCodBar> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<Int64?> registros);
        public Task<IEnumerable<string?>> GetProductsSetorIds(LinxAPIParam jobParameter);
    }
}
