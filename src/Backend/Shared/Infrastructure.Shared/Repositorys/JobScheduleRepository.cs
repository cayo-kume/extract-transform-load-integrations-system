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

        public async Task<int> CloseInterruptedExecutions(int idJob, string company)
        {
            // fim = agora (a execução seguinte começa), em horário de Brasília como o resto de auditing.Logs
            const string sql = """
                DECLARE @now DATETIME = CAST(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time' AS DATETIME);
                DECLARE @closed TABLE (IdJob INT, IdType INT, Company VARCHAR(20), Execution UNIQUEIDENTIFIER, StartDate DATETIME);

                UPDATE [auditing].[Logs] SET EndDate = @now
                OUTPUT inserted.IdJob, inserted.IdType, inserted.Company, inserted.Execution, inserted.StartDate INTO @closed
                WHERE IdJob = @idJob AND IdType = 2 AND Company = @company AND EndDate IS NULL;

                INSERT INTO [auditing].[Messages] (IdJob, IdType, Company, Execution, [Message], [Level])
                SELECT IdJob, IdType, Company, Execution,
                    CONCAT('Execução interrompida: o processo parou antes do fim (queda, reinício ou deploy). Iniciada em ',
                        FORMAT(StartDate, 'dd/MM/yyyy HH:mm:ss'), ' e fechada pela execução seguinte.'),
                    3
                FROM @closed;

                SELECT COUNT(*) FROM @closed;
                """;

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();
            using var transaction = conn.BeginTransaction();

            var closed = await conn.ExecuteScalarAsync<int>(sql, new { idJob, company = new DbString { Value = company, IsAnsi = true, Length = 20 } }, transaction);

            transaction.Commit();

            return closed;
        }

        public async Task<IReadOnlyList<SchedulingHostType>> HeartbeatSchedulingHost(Guid instanceId, string hostType, string instanceName, TimeSpan expireAfter)
        {
            const string sql = """
                DELETE FROM [auditing].[SchedulingHosts] WHERE LastHeartbeat < DATEADD(SECOND, -@expireSeconds, SYSUTCDATETIME());

                UPDATE [auditing].[SchedulingHosts] SET LastHeartbeat = SYSUTCDATETIME() WHERE InstanceId = @instanceId;

                IF @@ROWCOUNT = 0
                    INSERT INTO [auditing].[SchedulingHosts] (InstanceId, HostType, InstanceName, StartedAt, LastHeartbeat)
                    VALUES (@instanceId, @hostType, @instanceName, SYSUTCDATETIME(), SYSUTCDATETIME());

                SELECT HostType, COUNT(*) AS Instances, MIN(StartedAt) AS StartedAt, MAX(LastHeartbeat) AS LastHeartbeat
                FROM [auditing].[SchedulingHosts]
                GROUP BY HostType
                ORDER BY MIN(StartedAt), HostType;
                """;

            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            var types = await conn.QueryAsync<SchedulingHostType>(sql, new
            {
                instanceId,
                hostType = new DbString { Value = hostType, IsAnsi = true, Length = 20 },
                instanceName = instanceName.Length > 200 ? instanceName[..200] : instanceName,
                expireSeconds = (int)expireAfter.TotalSeconds
            });

            return types.ToList();
        }

        public async Task RemoveSchedulingHost(Guid instanceId)
        {
            using var conn = _sqlServerConnection.GetIDbAuditingConnection();

            await conn.ExecuteAsync("DELETE FROM [auditing].[SchedulingHosts] WHERE InstanceId = @instanceId", new { instanceId });
        }
    }
}
