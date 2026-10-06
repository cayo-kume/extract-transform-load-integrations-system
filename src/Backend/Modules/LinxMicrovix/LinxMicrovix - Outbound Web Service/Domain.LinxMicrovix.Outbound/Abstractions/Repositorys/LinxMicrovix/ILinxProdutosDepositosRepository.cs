using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxProdutosDepositosRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosDepositos? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosDepositos> records);
        public Task<IEnumerable<string?>> GetRegistersExists();
    }
}
