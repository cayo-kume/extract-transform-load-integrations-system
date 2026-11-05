/****** Heartbeat dos jobs de integração (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md, passo 7) ******/
-- Por job de integração recorrente (IdType = 2, IsOnDemand = 0) e empresa (Holder + Company): a agenda (auditing.Jobs) e
-- a última execução terminada e a execução aberta mais antiga (auditing.Logs) daquela empresa. Só dados: a regra de "off"
-- (ex.: sem execução terminada há mais que 1 × o intervalo) fica em quem lê, como a recuperação dos WebJobs e o painel do
-- WebApp. Os jobs sob demanda ficam de fora: não têm intervalo e nunca ficam "off". As datas de auditing.Logs estão no
-- horário de Brasília.
-- O índice IX_Logs_Heartbeat (Script001) cobre as duas buscas em auditing.Logs.
CREATE OR ALTER VIEW [auditing].[vw_JobsHeartbeat]
AS
SELECT
    c.IdParent,
    j.IdJob,
    j.Holder,
    j.Company,
    c.Title,
    j.IntervalTimeInMinutes,
    j.RunningDailyAt,
    j.Queue,
    j.IsActive,
    finished.LastStartDate,
    finished.LastEndDate,
    opened.OpenSince
FROM
    [auditing].[Jobs] j
    JOIN [auditing].[Catalog] c ON c.IdJob = j.IdJob AND c.IdType = j.IdType
    OUTER APPLY (
        SELECT
            MAX(l.StartDate) AS LastStartDate,
            MAX(l.EndDate) AS LastEndDate
        FROM
            [auditing].[Logs] l
        WHERE
            l.IdJob = j.IdJob
            AND l.IdType = j.IdType
            AND l.Holder = j.Holder
            AND l.Company = j.Company
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
            AND l.Holder = j.Holder
            AND l.Company = j.Company
            AND l.EndDate IS NULL
    ) opened
WHERE
    j.IdType = 2
    AND c.IsOnDemand = 0;
GO
