using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxXMLDocumentosRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxXMLDocumentos? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxXMLDocumentos> records);
        public Task<IEnumerable<string>> GetRegistersExists(LinxAPIParam jobParameter, List<string?> registros);

    }
}
