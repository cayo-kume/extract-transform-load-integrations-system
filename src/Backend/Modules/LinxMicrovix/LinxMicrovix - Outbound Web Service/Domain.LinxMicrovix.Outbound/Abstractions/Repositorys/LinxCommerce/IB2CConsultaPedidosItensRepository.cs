using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;

namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce
{
    public interface IB2CConsultaPedidosItensRepository
    {
        public Task<bool> InsertRecord(LinxAPIParam jobParameter, B2CConsultaPedidosItens? record);
        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaPedidosItens> records);
        public Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<Int64?> registros);
    }
}
