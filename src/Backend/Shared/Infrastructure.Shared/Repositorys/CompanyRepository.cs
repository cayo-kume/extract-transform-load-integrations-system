using Dapper;
using Domain.Shared.Abstractions.Repositorys;
using Infrastructure.Shared.Abstractions;

namespace Infrastructure.Shared.Repositorys
{
    /// <summary>
    /// lê auditing.Companies pela conexão de auditoria, a mesma de auditing.Jobs
    /// </summary>
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ISQLServerConnection _sqlServerConnection;

        public CompanyRepository(ISQLServerConnection sqlServerConnection) =>
            _sqlServerConnection = sqlServerConnection;

        public async Task<string?> GetHolder(string company)
        {
            const string sql = @"SELECT Holder FROM [auditing].[Companies] WHERE Company = @company";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryFirstOrDefaultAsync<string?>(sql, new { company = new DbString { Value = company, IsAnsi = true, Length = 20 } });
        }
    }
}
