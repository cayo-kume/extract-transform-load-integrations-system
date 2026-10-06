namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ISalesOrderCommandHandler
    {
        string CreateGetExistingSalesOrdersQuery(List<string?> registros);
    }
}
