using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce
{
    public interface IB2CConsultaNFeRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, B2CConsultaNFe? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaNFe> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros);
    }
}
