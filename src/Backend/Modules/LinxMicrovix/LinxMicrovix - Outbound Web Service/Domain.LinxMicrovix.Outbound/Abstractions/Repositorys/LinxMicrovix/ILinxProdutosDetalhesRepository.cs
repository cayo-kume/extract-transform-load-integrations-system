using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxProdutosDetalhesRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosDetalhes? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosDetalhes> records);
        public Task<IEnumerable<LinxProdutosDetalhes>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosDetalhes> registros);
        public Task<IEnumerable<LinxProdutosDetalhes>> GetRegistersExists(LinxAPIParam jobParameter);
    }
}
