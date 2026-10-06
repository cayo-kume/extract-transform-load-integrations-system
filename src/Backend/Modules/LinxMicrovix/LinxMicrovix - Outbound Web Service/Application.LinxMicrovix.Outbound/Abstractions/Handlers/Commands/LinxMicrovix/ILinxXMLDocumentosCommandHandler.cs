namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxXMLDocumentosCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<string> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
