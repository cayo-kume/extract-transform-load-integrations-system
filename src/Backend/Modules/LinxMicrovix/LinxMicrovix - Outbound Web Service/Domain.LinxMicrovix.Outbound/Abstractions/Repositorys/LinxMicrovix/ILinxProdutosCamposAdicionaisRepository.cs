using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxProdutosCamposAdicionaisRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosCamposAdicionais? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosCamposAdicionais> records);
        public Task<IEnumerable<string>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosCamposAdicionais> registros);
    }
}
