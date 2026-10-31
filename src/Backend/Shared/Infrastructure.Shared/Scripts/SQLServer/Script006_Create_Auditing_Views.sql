/****** Heartbeat dos jobs de integração (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md, passo 7) ******/
-- Por job de integração (IdType = 2): a agenda (auditing.Jobs) e a última execução terminada e a execução aberta mais
-- antiga (auditing.Logs). Só dados: a regra de "off" (ex.: sem execução terminada há mais que 1 × o intervalo) fica em
-- quem lê, como a recuperação dos WebJobs e o painel do WebApp. As datas de auditing.Logs estão no horário de Brasília.
-- O índice IX_Logs_Heartbeat (Script001) cobre as duas buscas em auditing.Logs.
CREATE OR ALTER VIEW [auditing].[vw_JobsHeartbeat]
AS
SELECT
    j.IdParent,
    j.IdJob,
    j.Title,
    j.IntervalTimeInMinutes,
    j.RunningDailyAt,
    j.Queue,
    j.IsActive,
    finished.LastStartDate,
    finished.LastEndDate,
    opened.OpenSince
FROM
    [auditing].[Jobs] j
    OUTER APPLY (
        SELECT
            MAX(l.StartDate) AS LastStartDate,
            MAX(l.EndDate) AS LastEndDate
        FROM
            [auditing].[Logs] l
        WHERE
            l.IdJob = j.IdJob
            AND l.IdType = j.IdType
            AND l.EndDate IS NOT NULL
    ) finished
    OUTER APPLY (
        SELECT
            MIN(l.StartDate) AS OpenSince
        FROM
            [auditing].[Logs] l
        WHERE
            l.IdJob = j.IdJob
            AND l.IdType = j.IdType
            AND l.EndDate IS NULL
    ) opened
WHERE
    j.IdType = 2;
GO
