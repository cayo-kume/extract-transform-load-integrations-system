using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxProdutosTabelasRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosTabelas? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosTabelas> records);
        public Task<IEnumerable<string?>> GetRegistersExists();
    }
}
