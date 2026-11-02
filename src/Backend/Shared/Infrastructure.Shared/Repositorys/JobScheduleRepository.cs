using Dapper;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;
using Infrastructure.Shared.Abstractions;

namespace Infrastructure.Shared.Repositorys
{
    /// <summary>
    /// lê a agenda em auditing.Jobs pela conexão de auditoria, a mesma em que o LoggerService grava auditing.Logs. Uma linha
    /// por (job, empresa), só das empresas ativas em monitor.Company (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.4)
    /// </summary>
    public class JobScheduleRepository : IJobScheduleRepository
    {
        private readonly ISQLServerConnection _sqlServerConnection;

        public JobScheduleRepository(ISQLServerConnection sqlServerConnection) =>
            _sqlServerConnection = sqlServerConnection;

        public async Task<IEnumerable<JobSchedule>> GetIntegrationJobs()
        {
            const string sql = @"SELECT j.IdJob, j.Company, j.Title, j.IntervalTimeInMinutes, j.RunningDailyAt, j.Queue, j.IsActive
                                 FROM [auditing].[Jobs] j
                                 JOIN [monitor].[Company] c ON c.Company = j.Company AND c.IsActive = 1
                                 WHERE j.IdType = 2";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryAsync<JobSchedule>(sql);
        }

        public async Task<IEnumerable<JobSchedule>> GetIntegrationJobByCompany(int idJob)
        {
            const string sql = @"SELECT j.IdJob, j.Company, j.Title, j.IntervalTimeInMinutes, j.RunningDailyAt, j.Queue, j.IsActive
                                 FROM [auditing].[Jobs] j
                                 JOIN [monitor].[Company] c ON c.Company = j.Company AND c.IsActive = 1
                                 WHERE j.IdType = 2 AND j.IdJob = @idJob";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryAsync<JobSchedule>(sql, new { idJob });
        }

        public async Task<IEnumerable<JobHeartbeat>> GetActiveJobsHeartbeat(string company)
        {
            const string sql = @"SELECT IdParent, IdJob, Company, Title, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive,
                                        LastStartDate, LastEndDate, OpenSince
                                 FROM [auditing].[vw_JobsHeartbeat]
                                 WHERE IsActive = 1 AND Company = @company";

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            return await conn.QueryAsync<JobHeartbeat>(sql, new { company = new DbString { Value = company, IsAnsi = true, Length = 20 } });
        }
    }
}
