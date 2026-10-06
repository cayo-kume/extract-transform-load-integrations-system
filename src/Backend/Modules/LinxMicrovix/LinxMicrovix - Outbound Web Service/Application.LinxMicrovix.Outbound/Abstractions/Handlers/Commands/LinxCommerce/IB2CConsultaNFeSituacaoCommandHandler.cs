namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaNFeSituacaoCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<int?> registros);
    }
}
