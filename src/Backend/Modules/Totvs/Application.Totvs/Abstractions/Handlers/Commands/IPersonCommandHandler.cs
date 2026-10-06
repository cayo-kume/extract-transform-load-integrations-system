namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface IPersonCommandHandler
    {
        string CreateGetExistingLegalEntitiesQuery(List<string?> registros);
        string CreateGetExistingCustomersQuery(List<string?> registros);
        string CreateGetExistingBranchesQuery(List<string?> registros);
    }
}
