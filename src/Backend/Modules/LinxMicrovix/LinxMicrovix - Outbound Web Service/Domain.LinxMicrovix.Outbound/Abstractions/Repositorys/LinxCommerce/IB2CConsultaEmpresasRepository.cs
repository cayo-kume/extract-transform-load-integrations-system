using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce
{
    public interface IB2CConsultaEmpresasRepository
    {
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaEmpresas> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<string?> registros);
    }
}
