using Dapper;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;
using Infrastructure.Shared.Abstractions;

namespace Infrastructure.Shared.Repositorys
{
    /// <summary>
    /// lê a agenda em auditing.Jobs pela conexão de auditoria, a mesma em que o LoggerService grava auditing.Logs
    /// </summary>
    public class JobScheduleRepository : IJobScheduleRepository
    {
        private readonly ISQLServerConnection _sqlServerConnection;

        public JobScheduleRepository(ISQLServerConnection sqlServerConnection) =>
            _sqlServerConnection = sqlServerConnection;

        public async Task<IEnumerable<JobSchedule>> GetIntegrationJobs()
        {
            const string sql = @"SELECT IdJob, Title, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive
                                 FROM [auditing].[Jobs]
                                 WHERE IdType = 2";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryAsync<JobSchedule>(sql);
        }
    }
}
