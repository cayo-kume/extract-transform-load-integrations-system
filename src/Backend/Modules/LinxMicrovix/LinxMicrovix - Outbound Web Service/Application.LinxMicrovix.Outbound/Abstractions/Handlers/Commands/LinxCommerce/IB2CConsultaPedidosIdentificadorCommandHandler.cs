using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaPedidosIdentificadorCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<B2CConsultaPedidosIdentificador> registros);
        string CreateInsertRecordQuery(string tableName);
        string CreateIntegrityLockQuery();
    }
}
