namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface IPurchaseOrderCommandHandler
    {
        string CreateGetExistingPurchaseOrdersQuery(List<string?> registros);
    }
}
