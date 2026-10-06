namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosCodBarCommandHandler
    {
        string CreateGetProductsSetorIdsQuery();
        string CreateGetRegistersExistsQuery(List<long?> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
