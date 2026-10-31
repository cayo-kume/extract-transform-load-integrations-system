/****** App e jobs dos próprios hosts de agenda (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md, passo 7) ******/
-- Range 14000 - 14499: jobs dos hosts (Domain.Shared.Enums.EnumJob). O 14000 só roda no host WebJob: no Hangfire do
-- WebApi ele fica sem método no mapa e o SchedulerRefresh avisa.
INSERT INTO [auditing].[Apps] ([IdParent], [Title], [Description])
VALUES
(140, 'Hosts', 'Jobs dos próprios hosts de agenda (WebApi com o Hangfire e WebJob)');

INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Integrações*/
(140, 14000, 'StuckJobsRecovery', 'Recuperação dos WebJobs travados: lê o heartbeat (auditing.vw_JobsHeartbeat) e reinicia na Azure só os WebJobs dos jobs com o heartbeat off', 15, NULL, 'default', 2);
