namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosTabelasPrecosCommandHandler
    {
        string CreateGetProductsTablesIdsQuery();
        string CreateGetRegistersExistsQuery(List<long?> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
