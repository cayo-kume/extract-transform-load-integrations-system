using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce
{
    public interface IB2CConsultaNFeSituacaoRepository
    {
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaNFeSituacao> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros);
    }
}
