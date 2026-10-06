namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaStatusCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<int?> registros);
    }
}
