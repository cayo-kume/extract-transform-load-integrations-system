namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaPedidosItensCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<long?> registros);
        string CreateInsertRecordQuery(string tableName);
        string CreateIntegrityLockQuery();
    }
}
