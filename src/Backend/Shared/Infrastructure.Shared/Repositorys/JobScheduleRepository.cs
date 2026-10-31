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

        public async Task<JobSchedule?> GetIntegrationJob(int idJob)
        {
            const string sql = @"SELECT IdJob, Title, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive
                                 FROM [auditing].[Jobs]
                                 WHERE IdType = 2 AND IdJob = @idJob";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryFirstOrDefaultAsync<JobSchedule>(sql, new { idJob });
        }

        public async Task<IEnumerable<JobHeartbeat>> GetActiveJobsHeartbeat()
        {
            const string sql = @"SELECT IdParent, IdJob, Title, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive,
                                        LastStartDate, LastEndDate, OpenSince
                                 FROM [auditing].[vw_JobsHeartbeat]
                                 WHERE IsActive = 1";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryAsync<JobHeartbeat>(sql);
        }
    }
}
