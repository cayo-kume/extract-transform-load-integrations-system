#Requires -Version 7
<#
.SYNOPSIS
    Carrega uma empresa de teste completa num banco já migrado: a empresa, os parâmetros (credenciais) de cada módulo e os
    jobs dela, sem passar pela tela de cadastro (docs/PLANO-TELA-CADASTRO-EMPRESA.md, seção 3 e fase 4.2).

.DESCRIPTION
    Os migrators (migrator.csx) criam só a estrutura e o catálogo do produto. Este script faz o que a tela de cadastro vai
    fazer, a partir dos scripts que ficam em Infrastructure.<Módulo>/Seeds/SQLServer:

    1. confere que o banco existe e que os migrators já rodaram (auditing.JobCatalog com jobs e monitor.Company);
    2. cadastra a empresa com auditing.sp_Add_Company (que já cria os jobs dos hosts dela, como o 14000);
    3. para cada módulo, roda o script de parâmetros de Seeds/SQLServer numa transação. Rodar de novo não duplica: a linha
       de um CNPJ que já tinha parâmetros no módulo é descartada (a regra é uma linha por CNPJ em cada *Parameters);
    4. liga cada CNPJ (doc_company) do script de parâmetros à empresa em monitor.HoldingCompanies, com
       auditing.sp_Add_Holding_Company: os parâmetros não mudam, a holding só entra se tiver parâmetros com o CNPJ dela;
    5. para cada módulo, chama auditing.sp_Add_Company_App, que copia os jobs do catálogo para a empresa, inativos;
    6. com -ActivateJobs, ativa os jobs da empresa nos módulos pedidos (os dos hosts continuam como estão).

    No fim, avisa sobre CNPJs das tabelas *Parameters que não estão em monitor.HoldingCompanies (nenhum serviço os lê).

    Os scripts de parâmetros têm credenciais reais. Os jobs nascem inativos e, antes de subir um host contra o banco de
    teste, aponte <Módulo>__BaseUrl para http://127.0.0.1:9/ para nenhum job chamar a API de verdade.

    A senha vai para o sqlcmd pela variável SQLCMDPASSWORD, nunca pela linha de comando, e não aparece na saída.

.PARAMETER Database
    Banco de teste, já migrado. O nome precisa ter "Test" ou "Teste" e não pode ser "Company"; fora disso, só com -Force.

.PARAMETER Server
    Instância do SQL Server; sem ele, a do container sql-server do Docker (localhost,1433).

.PARAMETER Credential
    Usuário e senha do SQL Server; sem ele, o script pede com Get-Credential.

.PARAMETER Company
    CNPJ da empresa (só dígitos); sem ele, a holding NEWBLOOMERS (44065634000106).

.PARAMETER CompanyName
    Razão social gravada em monitor.Company quando a empresa ainda não existe; sem ele, NEWBLOOMERS.

.PARAMETER Modules
    Só estes módulos (Totvs, LinxMicrovix, LinxCommerce, Movidesk, AfterSale, DataWarehouse); sem ele, todos.

.PARAMETER ActivateJobs
    Ativa os jobs da empresa nos módulos pedidos. Desligado por padrão: os jobs nascem inativos.

.PARAMETER Force
    Aceita um banco fora da regra de nome de -Database (por exemplo, a primeira carga de um ambiente novo).

.EXAMPLE
    ./tools/seeds/Import-TestSeeds.ps1 -Database EtlTeste

.EXAMPLE
    ./tools/seeds/Import-TestSeeds.ps1 -Database EtlTeste -Modules Totvs,DataWarehouse -ActivateJobs
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Database,
    [string]$Server = 'localhost,1433',
    [pscredential]$Credential,
    [ValidatePattern('^[0-9]+$')]
    [string]$Company = '44065634000106',
    [string]$CompanyName = 'NEWBLOOMERS',
    [ValidateSet('Totvs', 'LinxMicrovix', 'LinxCommerce', 'Movidesk', 'AfterSale', 'DataWarehouse')]
    [string[]]$Modules,
    [switch]$ActivateJobs,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------------------------------------------------
# módulos: IdParent em auditing.Apps, tabela de parâmetros e pasta da infraestrutura (a partir de src/Backend/Modules)
# ---------------------------------------------------------------------------------------------------------------------

$catalog = [ordered]@{
    Totvs         = @{ IdParent = 130; Table = 'totvs.TotvsParameters'; Folder = 'Totvs/Infrastructure.Totvs' }
    LinxMicrovix  = @{ IdParent = 80; Table = 'linxmicrovix.LinxMicrovixParameters'; Folder = 'LinxMicrovix/LinxMicrovix - Outbound Web Service/Infrastructure.LinxMicrovix.Outbound' }
    LinxCommerce  = @{ IdParent = 70; Table = 'linxcommerce.LinxCommerceParameters'; Folder = 'LinxCommerce/Infrastructure.LinxCommerce' }
    Movidesk      = @{ IdParent = 30; Table = 'movidesk.MovideskParameters'; Folder = 'General/Movidesk/Infrastructure.Movidesk' }
    AfterSale     = @{ IdParent = 50; Table = 'aftersale.AfterSaleParameters'; Folder = 'General/AfterSale/Infrastructure.AfterSale' }
    # só jobs: o DW lê as tabelas dos módulos e não tem credenciais
    DataWarehouse = @{ IdParent = 90; Table = $null; Folder = 'DataWarehouse/Infrastructure.DataWarehouse' }
}

$selected = if ($Modules) { $catalog.Keys | Where-Object { $_ -in $Modules } } else { $catalog.Keys }
$modulesRoot = Join-Path $PSScriptRoot '../../src/Backend/Modules' | Resolve-Path

# ---------------------------------------------------------------------------------------------------------------------
# trava do banco
# ---------------------------------------------------------------------------------------------------------------------

if (-not $Force -and ($Database -eq 'Company' -or $Database -notmatch 'Test')) {
    throw "O banco '$Database' não parece de teste (o nome precisa ter 'Test' ou 'Teste' e não pode ser 'Company'). Use -Force se for mesmo para ele."
}

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw 'sqlcmd não encontrado no PATH.'
}

if (-not $Credential) {
    $Credential = Get-Credential -Message "Usuário do SQL Server em $Server"
}

# ---------------------------------------------------------------------------------------------------------------------
# sqlcmd
# ---------------------------------------------------------------------------------------------------------------------

# roda um script (texto ou arquivo) e devolve as linhas de saída; -b faz o sqlcmd sair com erro no primeiro erro de SQL
function Invoke-Sql {
    param(
        [string]$Query,
        [string]$InputFile
    )

    $arguments = @('-S', $Server, '-d', $Database, '-U', $Credential.UserName, '-C', '-b', '-I', '-f', '65001', '-h', '-1', '-W', '-s', '|')
    $arguments += if ($InputFile) { @('-i', $InputFile) } else { @('-Q', "SET NOCOUNT ON; $Query") }

    $env:SQLCMDPASSWORD = $Credential.GetNetworkCredential().Password
    try {
        $output = & sqlcmd @arguments 2>&1
        $exitCode = $LASTEXITCODE
    }
    finally {
        Remove-Item Env:SQLCMDPASSWORD -ErrorAction SilentlyContinue
    }

    if ($exitCode -ne 0) {
        throw "sqlcmd falhou (código $exitCode):`n$($output -join "`n")"
    }

    # o sqlcmd devolve linhas vazias entre os resultados
    @($output | ForEach-Object { "$_".Trim() } | Where-Object { $_ })
}

# a última linha "CHAVE=valor" da saída
function Get-Marker {
    param([string[]]$Lines, [string]$Name)

    $line = $Lines | Where-Object { $_ -like "$Name=*" } | Select-Object -Last 1
    if ($null -eq $line) { throw "Saída sem $Name=:`n$($Lines -join "`n")" }
    [int]($line.Substring($Name.Length + 1))
}

# todas as linhas "CHAVE=valor" da saída, só os valores
function Get-Markers {
    param([string[]]$Lines, [string]$Name)

    @($Lines | Where-Object { $_ -like "$Name=*" } | ForEach-Object { $_.Substring($Name.Length + 1) })
}

# ---------------------------------------------------------------------------------------------------------------------
# 1. o banco foi migrado?
# ---------------------------------------------------------------------------------------------------------------------

$check = Invoke-Sql -Query @"
SELECT CONCAT('CATALOG=', CASE WHEN OBJECT_ID('auditing.JobCatalog') IS NULL THEN -1 ELSE (SELECT COUNT(*) FROM auditing.JobCatalog) END);
SELECT CONCAT('COMPANY=', CASE WHEN OBJECT_ID('monitor.Company') IS NULL THEN 0 ELSE 1 END);
"@
if ((Get-Marker $check 'CATALOG') -le 0 -or (Get-Marker $check 'COMPANY') -ne 1) {
    throw "O banco '$Database' não tem auditing.JobCatalog com jobs ou monitor.Company: rode os migrators antes (docs/MIGRATOR-CSX.md)."
}

# ---------------------------------------------------------------------------------------------------------------------
# 2. empresa
# ---------------------------------------------------------------------------------------------------------------------

$safeName = $CompanyName.Replace("'", "''")
$null = Invoke-Sql -Query "EXEC auditing.sp_Add_Company @company = '$Company', @name = N'$safeName';"
Write-Host "Empresa $Company cadastrada (ou já existente), com os jobs dos hosts."

# ---------------------------------------------------------------------------------------------------------------------
# 3 a 6. parâmetros, CNPJs da holding e jobs de cada módulo
# ---------------------------------------------------------------------------------------------------------------------

$summary = foreach ($name in $selected) {
    $module = $catalog[$name]
    $warnings = [System.Collections.Generic.List[string]]::new()
    $inserted = $null
    $docs = @()

    if ($module.Table) {
        $seedsDir = Join-Path $modulesRoot $module.Folder 'Seeds/SQLServer'
        $seed = Get-ChildItem -LiteralPath $seedsDir -Filter '*_Insert_Values_Into_*_Parameters_Table.sql' -ErrorAction SilentlyContinue | Select-Object -First 1

        if (-not $seed) {
            $oldDir = Join-Path $modulesRoot $module.Folder 'Scripts/SQLServer/Parameters'
            if (Get-ChildItem -LiteralPath $oldDir -Filter '*_Insert_Values_Into_*_Parameters_Table.sql' -ErrorAction SilentlyContinue) {
                $warnings.Add('o script de parâmetros está no caminho antigo (Scripts/SQLServer/Parameters, que o DbUp executa): mova para Seeds/SQLServer')
            }
            else {
                $warnings.Add('sem script de parâmetros em Seeds/SQLServer (fora do git: copie o .sql.example)')
            }
        }
        else {
            # o script do módulo entra pelo :r, numa transação; os CNPJs das linhas dele saem como DOC=, e depois as
            # linhas novas de CNPJs que já tinham parâmetros no módulo são descartadas: o que sobra é o que foi inserido
            $wrapper = New-TemporaryFile
            try {
                Set-Content -LiteralPath $wrapper -Encoding utf8 -Value @"
SET NOCOUNT ON;
SET XACT_ABORT ON;
IF OBJECT_ID('tempdb..#before') IS NOT NULL DROP TABLE #before;
SELECT ISNULL(MAX(id), 0) AS max_id INTO #before FROM $($module.Table);
BEGIN TRANSACTION;
GO
:r "$($seed.FullName)"
GO
SELECT DISTINCT CONCAT('DOC=', doc_company) FROM $($module.Table) WHERE id > (SELECT max_id FROM #before);
DELETE t
FROM $($module.Table) t
WHERE t.id > (SELECT max_id FROM #before)
  AND EXISTS (SELECT 1 FROM $($module.Table) o WHERE o.doc_company = t.doc_company AND o.id <= (SELECT max_id FROM #before));
SELECT CONCAT('INSERTED=', COUNT(*)) FROM $($module.Table) WHERE id > (SELECT max_id FROM #before);
COMMIT TRANSACTION;
GO
"@
                $output = Invoke-Sql -InputFile $wrapper
                $inserted = Get-Marker $output 'INSERTED'
                $docs = Get-Markers $output 'DOC'
            }
            finally {
                Remove-Item -LiteralPath $wrapper -ErrorAction SilentlyContinue
            }
        }
    }

    # um CNPJ de outra holding (ou que é outra empresa do painel) é recusado pela procedure e vira aviso
    foreach ($doc in $docs) {
        if ($doc -notmatch '^[0-9]+$') {
            $warnings.Add("doc_company '$doc' não é só dígitos: fora de monitor.HoldingCompanies")
            continue
        }
        try {
            $null = Invoke-Sql -Query "EXEC auditing.sp_Add_Holding_Company @company = '$Company', @docCompany = '$doc';"
        }
        catch {
            $warnings.Add("CNPJ $doc fora da holding: $(($_.Exception.Message -split "`n" | Where-Object { $_ -match 'holding|painel' } | Select-Object -First 1))")
        }
    }

    $created = Get-Marker (Invoke-Sql -Query @"
DECLARE @r TABLE (JobsCreated INT);
INSERT INTO @r EXEC auditing.sp_Add_Company_App @company = '$Company', @idParent = $($module.IdParent);
SELECT CONCAT('CREATED=', JobsCreated) FROM @r;
"@) 'CREATED'

    if ($ActivateJobs) {
        $null = Invoke-Sql -Query "UPDATE j SET IsActive = 1 FROM auditing.Jobs j JOIN auditing.fn_JobCatalogTree($($module.IdParent)) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType WHERE j.Company = '$Company';"
    }

    $state = Invoke-Sql -Query @"
SELECT CONCAT('TOTAL=', COUNT(*)) FROM auditing.Jobs j JOIN auditing.fn_JobCatalogTree($($module.IdParent)) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType WHERE j.Company = '$Company';
SELECT CONCAT('ACTIVE=', COUNT(*)) FROM auditing.Jobs j JOIN auditing.fn_JobCatalogTree($($module.IdParent)) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType WHERE j.Company = '$Company' AND IsActive = 1;
"@

    [pscustomobject]@{
        Modulo     = $name
        Parametros = if ($module.Table) { if ($null -ne $inserted) { "$inserted inserida(s)" } else { 'pulado' } } else { '-' }
        CNPJs      = $docs -join ', '
        JobsNovos  = $created
        Jobs       = Get-Marker $state 'TOTAL'
        Ativos     = Get-Marker $state 'ACTIVE'
        Avisos     = $warnings
    }
}

$summary | Select-Object Modulo, Parametros, CNPJs, JobsNovos, Jobs, Ativos | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

$summary | ForEach-Object { $m = $_.Modulo; $_.Avisos | ForEach-Object { Write-Warning "${m}: $_" } }

# CNPJs de parâmetros sem holding, em todos os módulos com tabela no banco: nenhum serviço os lê
$orphanQuery = ($catalog.Values | Where-Object Table | ForEach-Object {
        "IF OBJECT_ID('$($_.Table)') IS NOT NULL SELECT DISTINCT CONCAT('ORPHAN=$($_.Table) ', p.doc_company) FROM $($_.Table) p WHERE NOT EXISTS (SELECT 1 FROM monitor.HoldingCompanies hc WHERE hc.Company = p.doc_company);"
    }) -join "`n"
Get-Markers (Invoke-Sql -Query $orphanQuery) 'ORPHAN' | ForEach-Object {
    Write-Warning "CNPJ de parâmetro sem holding em monitor.HoldingCompanies (nenhum serviço lê): $_"
}

Write-Host 'Os scripts de parâmetros têm credenciais reais: antes de subir um host contra este banco, aponte' -ForegroundColor Yellow
Write-Host '<Módulo>__BaseUrl para http://127.0.0.1:9/ (AfterSale, Movidesk, LinxMicrovix e Totvs).' -ForegroundColor Yellow
