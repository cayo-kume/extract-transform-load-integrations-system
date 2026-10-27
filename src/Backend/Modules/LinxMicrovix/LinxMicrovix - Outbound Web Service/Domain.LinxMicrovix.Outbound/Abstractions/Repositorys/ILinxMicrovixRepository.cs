using Domain.LinxMicrovix.Outbound.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.Enums;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
        public Task<IEnumerable<string>> GetStores(string doc_company, EnumApi api);
        public Task<long> GetLastTimestamp(string doc_company, string tableName, string? cnpj_emp);
        public Task<long> GetLastTimestamp(string doc_company, string tableName, string? cnpj_emp, string filterColumn, string filterValue);
        public Task<IEnumerable<string>> GetSourceValues(string doc_company, string sourceTable, string sourceColumn, string? cnpj_emp, string? changedSinceTable);
        public bool InsertIntoLinxMicrovixRequests(Domain.LinxMicrovix.Outbound.Entities.Writes.Requests.Request request);
    }
}
