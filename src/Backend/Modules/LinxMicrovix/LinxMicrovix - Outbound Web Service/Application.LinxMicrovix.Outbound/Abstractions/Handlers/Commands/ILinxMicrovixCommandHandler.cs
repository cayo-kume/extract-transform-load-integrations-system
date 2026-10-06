namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands
{
    public interface ILinxMicrovixCommandHandler
    {
        string CreateGetParametersQuery(string parametersInterval, string parametersTableName, string jobName);
        string CreateGetB2CCompanysQuery();
        string CreateGetMicrovixCompanysQuery();
        string CreateGetMicrovixGroupCompanysQuery();
        string CreateGetLast7DaysMaxTimestampQuery(string schema, string tableName);
        string CreateGetLastMaxTimestampQuery(string schema, string tableName, string columnCompany, string companyValue);
        string CreateGetLastMaxTimestampByCnpjAndIdentificadorQuery(string schema, string tableName, string columnCompany, string companyValue, string columnIdentificador, string columnIdentificadorValue);
        string CreateGetLast7DaysMinTimestampQuery(string schema, string tableName, string columnDate);
        string CreateGetLast7DaysMinTimestampWithCompanyQuery(string schema, string tableName, string columnDate, string columnCompany, string companyValue);
    }
}
