namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxUsuariosCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<int?> registros);
    }
}
