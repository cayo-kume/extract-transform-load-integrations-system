using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxNaturezaOperacaoRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxNaturezaOperacao? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxNaturezaOperacao> records);
        public Task<IEnumerable<string?>> GetRegistersExists();
    }
}
