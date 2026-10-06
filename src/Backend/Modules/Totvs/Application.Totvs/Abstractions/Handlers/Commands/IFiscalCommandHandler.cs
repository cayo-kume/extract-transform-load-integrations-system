namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface IFiscalCommandHandler
    {
        string CreateGetExistingInvoicesQuery(List<string?> registros);
    }
}
