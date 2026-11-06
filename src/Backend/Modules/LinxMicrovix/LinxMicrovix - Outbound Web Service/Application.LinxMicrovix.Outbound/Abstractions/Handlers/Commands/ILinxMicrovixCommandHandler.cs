using Domain.LinxMicrovix.Outbound.Enums;

namespace Application.LinxMicrovix.Outbound.Abstractions.Handlers.Commands
{
    public interface ILinxMicrovixCommandHandler
    {
        public string CreateGetCompanysQuery(string company);
        public string CreateGetStoresQuery(string doc_company, EnumApi api);
        public string CreateGetB2CKeyStoresQuery(string doc_company);
        public string CreateGetB2CPendingImageIdsQuery(string doc_company);
        public string CreateGetLastTimestampQuery(string doc_company, string tableName, string? cnpj_emp);
        public string CreateGetLastTimestampQuery(string doc_company, string tableName, string? cnpj_emp, string filterColumn, string filterValue);
        public string CreateGetSourceValuesQuery(string doc_company, string sourceTable, string sourceColumn, string? cnpj_emp, string? changedSinceTable);
    }
}
