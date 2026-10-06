namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosPromocoesCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<long?> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
