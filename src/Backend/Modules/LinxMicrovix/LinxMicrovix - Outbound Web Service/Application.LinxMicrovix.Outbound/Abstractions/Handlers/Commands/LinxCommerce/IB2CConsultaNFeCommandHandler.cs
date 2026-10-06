namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce
{
    public interface IB2CConsultaNFeCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<int?> registros);
        string CreateInsertRecordQuery(string tableName);
        string CreateIntegrityLockQuery();
    }
}
