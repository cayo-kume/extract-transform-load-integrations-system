namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosCommandHandler
    {
        string CreateGetProductsSetorIdsQuery();
        string CreateGetRegistersExistsQuery(List<long?> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
