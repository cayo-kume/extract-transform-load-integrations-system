namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaClientesCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<string> registros);
        string CreateInsertRecordQuery(string tableName);
        string CreateIntegrityLockQuery();
    }
}
