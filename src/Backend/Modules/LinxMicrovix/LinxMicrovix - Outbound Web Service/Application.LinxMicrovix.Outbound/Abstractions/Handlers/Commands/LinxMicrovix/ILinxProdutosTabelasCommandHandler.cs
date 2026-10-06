namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosTabelasCommandHandler
    {
        string CreateGetRegistersExistsQuery();
        string CreateInsertRecordQuery(string tableName);
    }
}
