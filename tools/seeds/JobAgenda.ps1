# agenda sugerida dos jobs (docs/AGENDA-SUGERIDA-JOBS.csv), a mesma que a tela do WebApp sugere ao contratar um módulo
# (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md). Usada pelo Import-TestSeeds.ps1 (o WebApp embute o mesmo .csv, Data/JobAgenda.cs):
# . (Join-Path $PSScriptRoot '../seeds/JobAgenda.ps1')

$script:JobAgendaPath = Join-Path $PSScriptRoot '../../docs/AGENDA-SUGERIDA-JOBS.csv' | Resolve-Path

# SQL que cria os jobs de um módulo (auditing.fn_CatalogTree, com as procedures) para a empresa, inativos e com a agenda
# sugerida; rodar de novo não duplica. Job sem linha no .csv: sob demanda com intervalo 0, recorrente de hora em hora.
# Termina com CREATED=<jobs criados>. Holding e empresa só dígitos (quem chama valida)
function Get-AddJobsSql {
    param(
        [Parameter(Mandatory)][string]$Holder,
        [Parameter(Mandatory)][string]$Company,
        [Parameter(Mandatory)][int]$IdParent
    )

    $values = Import-Csv -LiteralPath $script:JobAgendaPath -Delimiter ';' | ForEach-Object {
        $dailyAt = if ($_.RunningDailyAt) { "'$($_.RunningDailyAt)'" } else { 'NULL' }
        "($($_.IdJob), $($_.IdType), $($_.IntervalTimeInMinutes), $dailyAt, N'$($_.Queue)')"
    }

    @"
SET NOCOUNT ON;
DECLARE @agenda TABLE (IdJob INT, IdType INT, IntervalTimeInMinutes INT, RunningDailyAt TIME(0), Queue NVARCHAR(50), PRIMARY KEY (IdJob, IdType));
INSERT INTO @agenda (IdJob, IdType, IntervalTimeInMinutes, RunningDailyAt, Queue) VALUES
$($values -join ",`n");

INSERT INTO [auditing].[Jobs] (IdJob, IdType, Holder, Company, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive)
SELECT
    t.IdJob,
    t.IdType,
    '$Holder',
    '$Company',
    COALESCE(a.IntervalTimeInMinutes, CASE c.IsOnDemand WHEN 1 THEN 0 ELSE 60 END),
    a.RunningDailyAt,
    COALESCE(a.Queue, N'default'),
    0
FROM
    [auditing].[fn_CatalogTree]($IdParent) t
    JOIN [auditing].[Catalog] c ON c.IdJob = t.IdJob AND c.IdType = t.IdType
    LEFT JOIN @agenda a ON a.IdJob = t.IdJob AND a.IdType = t.IdType
WHERE
    NOT EXISTS (
        SELECT 1 FROM [auditing].[Jobs] j
        WHERE j.IdJob = t.IdJob AND j.IdType = t.IdType AND j.Holder = '$Holder' AND j.Company = '$Company'
    );

SELECT CONCAT('CREATED=', @@ROWCOUNT);
"@
}
