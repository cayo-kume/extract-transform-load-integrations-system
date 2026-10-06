namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxPedidosVendaCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<int?> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
