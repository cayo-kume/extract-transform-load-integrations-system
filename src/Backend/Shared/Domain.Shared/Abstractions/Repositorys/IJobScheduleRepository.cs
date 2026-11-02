using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IJobScheduleRepository
    {
        /// <summary>
        /// agenda dos jobs de integração (auditing.Jobs com IdType = 2), ativos ou não, uma linha por (job, empresa), só das
        /// empresas ativas em monitor.Company
        /// </summary>
        public Task<IEnumerable<JobSchedule>> GetIntegrationJobs();

        /// <summary>
        /// agenda de um job de integração em cada empresa ativa; vazia quando ele não está em auditing.Jobs (o WebJob relê a
        /// cada ciclo)
        /// </summary>
        public Task<IEnumerable<JobSchedule>> GetIntegrationJobByCompany(int idJob);

        /// <summary>
        /// heartbeat dos jobs de integração ativos da empresa (auditing.vw_JobsHeartbeat)
        /// </summary>
        public Task<IEnumerable<JobHeartbeat>> GetActiveJobsHeartbeat(string company);
    }
}
