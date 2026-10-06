namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface IProductCommandHandler
    {
        string CreateGetExistingSkusQuery(List<string?> registros);
    }
}
