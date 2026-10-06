namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxClientesFornecCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<string> registros);
        string CreateInsertRecordQuery(string tableName);
        string CreateInsertRecordQuery(string identifier, bool is_present_in_erp);
        string CreateIntegrityLockQuery();
    }
}
