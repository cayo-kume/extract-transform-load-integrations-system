namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaEmpresasCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<string> registros);
    }
}
