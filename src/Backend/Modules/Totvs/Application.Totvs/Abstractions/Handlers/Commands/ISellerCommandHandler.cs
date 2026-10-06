namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ISellerCommandHandler
    {
        string CreateGetExistingSellersQuery(List<string?> registros);
    }
}
