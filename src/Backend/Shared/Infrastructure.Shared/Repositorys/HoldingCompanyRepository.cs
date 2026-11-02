using Dapper;
using Domain.Shared.Abstractions.Repositorys;
using Infrastructure.Shared.Abstractions;

namespace Infrastructure.Shared.Repositorys
{
    /// <summary>
    /// lê monitor.HoldingCompanies pela conexão das integrações, a mesma das tabelas *Parameters
    /// </summary>
    public class HoldingCompanyRepository : IHoldingCompanyRepository
    {
        private readonly ISQLServerConnection _sqlServerConnection;

        public HoldingCompanyRepository(ISQLServerConnection sqlServerConnection) =>
            _sqlServerConnection = sqlServerConnection;

        public async Task<string?> GetHolding(string docCompany)
        {
            const string sql = @"SELECT Holding FROM [monitor].[HoldingCompanies] WHERE Company = @docCompany";

            using var conn = _sqlServerConnection.GetIDbConnection();

            return await conn.QueryFirstOrDefaultAsync<string?>(sql, new { docCompany = new DbString { Value = docCompany, IsAnsi = true, Length = 20 } });
        }
    }
}
