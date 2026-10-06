namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxLojasCommandHandler
    {
        string CreateGetRegistersExistsQuery();
        string CreateInsertRecordQuery(string tableName);
    }
}
