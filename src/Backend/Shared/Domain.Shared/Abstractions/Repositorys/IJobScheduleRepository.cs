using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IJobScheduleRepository
    {
        /// <summary>
        /// agenda dos jobs de integração recorrentes (auditing.Jobs com IdType = 2, fora os sob demanda), ativos ou não, uma
        /// linha por (job, holding, empresa)
        /// </summary>
        public Task<IEnumerable<JobSchedule>> GetIntegrationJobs();

        /// <summary>
        /// agenda de um job de integração em cada empresa; vazia quando ele não está em auditing.Jobs (o WebJob relê a
        /// cada ciclo)
        /// </summary>
        public Task<IEnumerable<JobSchedule>> GetIntegrationJobByCompany(int idJob);

        /// <summary>
        /// heartbeat dos jobs de integração ativos das empresas da holding (auditing.vw_JobsHeartbeat)
        /// </summary>
        public Task<IEnumerable<JobHeartbeat>> GetActiveJobsHeartbeat(string holder);

        /// <summary>
        /// fecha as execuções do job e da empresa que ficaram abertas em auditing.Logs (EndDate nulo) e grava nelas uma mensagem de
        /// erro de execução interrompida. Só quem segura a trava da empresa chama (o ScheduledJobRunner, antes de executar): com ela,
        /// nenhuma execução do job e da empresa está viva. Devolve quantas foram fechadas
        /// </summary>
        public Task<int> CloseInterruptedExecutions(int idJob, string company);

        /// <summary>
        /// heartbeat de uma instância de host de agenda em auditing.SchedulingHosts: apaga as linhas sem heartbeat há mais de
        /// expireAfter, grava ou atualiza a da instância e devolve os tipos de host no ar, do que está no ar há mais tempo para o
        /// mais recente
        /// </summary>
        public Task<IReadOnlyList<SchedulingHostType>> HeartbeatSchedulingHost(Guid instanceId, string hostType, string instanceName, TimeSpan expireAfter);

        /// <summary>
        /// tira a instância de auditing.SchedulingHosts (parada do host), para outro tipo de host assumir sem esperar o heartbeat expirar
        /// </summary>
        public Task RemoveSchedulingHost(Guid instanceId);
    }
}
