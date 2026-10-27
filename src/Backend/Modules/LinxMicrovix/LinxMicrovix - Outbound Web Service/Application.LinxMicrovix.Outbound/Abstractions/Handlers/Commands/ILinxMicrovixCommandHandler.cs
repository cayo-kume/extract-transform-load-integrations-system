using Domain.LinxMicrovix.Outbound.Enums;

namespace Application.LinxMicrovix.Outbound.Abstractions.Handlers.Commands
{
    public interface ILinxMicrovixCommandHandler
    {
        public string CreateGetCompanysQuery();
        public string CreateGetStoresQuery(string doc_company, EnumApi api);
        public string CreateGetLastTimestampQuery(string doc_company, string tableName, string? cnpj_emp);
        public string CreateGetLastTimestampQuery(string doc_company, string tableName, string? cnpj_emp, string filterColumn, string filterValue);
        public string CreateGetSourceValuesQuery(string doc_company, string sourceTable, string sourceColumn, string? cnpj_emp, string? changedSinceTable);
    }
}
