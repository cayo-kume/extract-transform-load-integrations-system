using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IJobScheduleRepository
    {
        /// <summary>
        /// agenda dos jobs de integração (auditing.Jobs com IdType = 2), ativos ou não
        /// </summary>
        public Task<IEnumerable<JobSchedule>> GetIntegrationJobs();

        /// <summary>
        /// agenda de um job de integração; null quando ele não está em auditing.Jobs (o WebJob relê a cada ciclo)
        /// </summary>
        public Task<JobSchedule?> GetIntegrationJob(int idJob);

        /// <summary>
        /// heartbeat dos jobs de integração ativos (auditing.vw_JobsHeartbeat)
        /// </summary>
        public Task<IEnumerable<JobHeartbeat>> GetActiveJobsHeartbeat();
    }
}
