using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IJobScheduleRepository
    {
        /// <summary>
        /// agenda dos jobs de integração (auditing.Jobs com IdType = 2), ativos ou não
        /// </summary>
        public Task<IEnumerable<JobSchedule>> GetIntegrationJobs();
    }
}
