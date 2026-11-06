#Requires -Version 7
<#
.SYNOPSIS
    Carrega uma holding de teste completa num banco já migrado: a holding, as empresas dela, os parâmetros (credenciais) de
    cada módulo e os jobs de cada empresa, sem passar pela tela de cadastro (.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md).

.DESCRIPTION
    Os migrators (migrator.csx) criam só a estrutura e o catálogo do produto. Este script faz o que a tela de cadastro vai
    fazer, a partir dos scripts que ficam em Infrastructure.<Módulo>/Seeds/SQLServer:

    1. confere que o banco existe e que os migrators já rodaram (auditing.Catalog com jobs e auditing.Holders);
    2. cadastra a holding em auditing.Holders e a empresa que é a própria holding em auditing.Companies (Company = Holder),
       com os jobs dos hosts (IdParent 140, como o 14000);
    3. para cada módulo, roda o script de parâmetros de Seeds/SQLServer numa transação. Rodar de novo não duplica: a linha
       de um CNPJ que já tinha parâmetros no módulo é descartada (a regra é uma linha por CNPJ em cada *Parameters);
    4. cadastra cada CNPJ (doc_company) do script de parâmetros como empresa da holding em auditing.Companies; um CNPJ que
       já é de outra holding vira aviso;
    5. para cada módulo, cria os jobs dele (auditing.fn_CatalogTree) em cada empresa da holding com parâmetros no módulo,
       inativos e com a agenda sugerida de .docs/AGENDA-SUGERIDA-JOBS.csv. O DataWarehouse, sem parâmetros, fica na empresa
       que é a própria holding.

    Ativar jobs não é com este script: é no WebApp (tela Integrações), que só ativa os jobs de um módulo com
    a credencial dele cadastrada.

    No fim, avisa sobre CNPJs das tabelas *Parameters que não estão em auditing.Companies (nenhum serviço os lê).

    Os scripts de parâmetros têm credenciais reais. Os jobs ficam inativos e, antes de subir um host contra o banco de
    teste, aponte <Módulo>__BaseUrl para http://127.0.0.1:9/ para nenhum job chamar a API de verdade.

    A senha vai para o sqlcmd pela variável SQLCMDPASSWORD, nunca pela linha de comando, e não aparece na saída.

.PARAMETER Database
    Banco de teste, já migrado. O nome precisa ter "Test" ou "Teste" e não pode ser "Company"; fora disso, só com -Force.

.PARAMETER Server
    Instância do SQL Server; sem ele, a do container sql-server do Docker (localhost,1433).

.PARAMETER Credential
    Usuário e senha do SQL Server; sem ele, o script pede com Get-Credential.

.PARAMETER Holder
    CNPJ da holding (só dígitos); sem ele, a NEWBLOOMERS (44065634000106).

.PARAMETER HolderName
    Nome fantasia gravado em auditing.Holders (e na empresa que é a própria holding) quando ela ainda não existe; sem ele,
    NEWBLOOMERS.

.PARAMETER Modules
    Só estes módulos (Totvs, LinxMicrovix, LinxCommerce, Movidesk, AfterSale, DataWarehouse); sem ele, todos.

.PARAMETER Force
    Aceita um banco fora da regra de nome de -Database (por exemplo, a primeira carga de um ambiente novo).

.EXAMPLE
    ./tools/seeds/Import-TestSeeds.ps1 -Database EtlTeste

.EXAMPLE
    ./tools/seeds/Import-TestSeeds.ps1 -Database EtlTeste -Modules Totvs,DataWarehouse
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Database,
    [string]$Server = 'localhost,1433',
    [pscredential]$Credential,
    [ValidatePattern('^[0-9]+$')]
    [string]$Holder = '44065634000106',
    [string]$HolderName = 'NEWBLOOMERS',
    [ValidateSet('Totvs', 'LinxMicrovix', 'LinxCommerce', 'Movidesk', 'AfterSale', 'DataWarehouse')]
    [string[]]$Modules,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'JobAgenda.ps1')

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

# um script grande (a agenda sugerida passa do limite da linha de comando) vai por arquivo temporário
function Invoke-SqlScript {
    param([string]$Script)

    $file = New-TemporaryFile
    try {
        Set-Content -LiteralPath $file -Encoding utf8 -Value $Script
        Invoke-Sql -InputFile $file
    }
    finally {
        Remove-Item -LiteralPath $file -ErrorAction SilentlyContinue
    }
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
SELECT CONCAT('CATALOG=', CASE WHEN OBJECT_ID('auditing.Catalog') IS NULL THEN -1 ELSE (SELECT COUNT(*) FROM auditing.Catalog) END);
SELECT CONCAT('HOLDERS=', CASE WHEN OBJECT_ID('auditing.Holders') IS NULL THEN 0 ELSE 1 END);
"@
if ((Get-Marker $check 'CATALOG') -le 0 -or (Get-Marker $check 'HOLDERS') -ne 1) {
    throw "O banco '$Database' não tem auditing.Catalog com jobs ou auditing.Holders: rode os migrators antes (.docs/MIGRATOR-CSX.md)."
}

# ---------------------------------------------------------------------------------------------------------------------
# 2. holding, a empresa que é a própria holding e os jobs dos hosts
# ---------------------------------------------------------------------------------------------------------------------

$safeName = $HolderName.Replace("'", "''")
$null = Invoke-Sql -Query @"
IF NOT EXISTS (SELECT 1 FROM auditing.Holders WHERE Document = '$Holder')
    INSERT INTO auditing.Holders (Document, FantasyName) VALUES ('$Holder', '$safeName');
IF NOT EXISTS (SELECT 1 FROM auditing.Companies WHERE Company = '$Holder')
    INSERT INTO auditing.Companies (Company, Holder, FantasyName) VALUES ('$Holder', '$Holder', '$safeName');
"@
$hostJobs = Get-Marker (Invoke-SqlScript (Get-AddJobsSql -Holder $Holder -Company $Holder -IdParent 140)) 'CREATED'
Write-Host "Holding $Holder cadastrada (ou já existente), com $hostJobs job(s) novo(s) dos hosts."

# ---------------------------------------------------------------------------------------------------------------------
# 3 a 5. parâmetros, empresas da holding e jobs de cada módulo
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

    # cada CNPJ do script vira empresa da holding; um CNPJ que já é de outra holding vira aviso e fica sem jobs
    foreach ($doc in $docs) {
        if ($doc -notmatch '^[0-9]+$') {
            $warnings.Add("doc_company '$doc' não é só dígitos: fora de auditing.Companies")
            continue
        }

        $other = Invoke-Sql -Query @"
IF NOT EXISTS (SELECT 1 FROM auditing.Companies WHERE Company = '$doc')
    INSERT INTO auditing.Companies (Company, Holder) VALUES ('$doc', '$Holder');
SELECT CONCAT('OTHER=', Holder) FROM auditing.Companies WHERE Company = '$doc' AND Holder <> '$Holder';
"@
        Get-Markers $other 'OTHER' | ForEach-Object { $warnings.Add("CNPJ $doc é da holding $_ em auditing.Companies: fica sem jobs desta") }
    }

    # as empresas da holding com parâmetros no módulo (as de cargas anteriores também); o DW fica na própria holding
    $companies = if ($module.Table) {
        Get-Markers (Invoke-Sql -Query "SELECT DISTINCT CONCAT('COMPANY=', c.Company) FROM $($module.Table) p JOIN auditing.Companies c ON c.Company = p.doc_company WHERE c.Holder = '$Holder';") 'COMPANY'
    }
    else {
        @($Holder)
    }

    $created = 0
    foreach ($company in $companies) {
        $created += Get-Marker (Invoke-SqlScript (Get-AddJobsSql -Holder $Holder -Company $company -IdParent $module.IdParent)) 'CREATED'
    }

    $state = Invoke-Sql -Query @"
SELECT CONCAT('TOTAL=', COUNT(*)) FROM auditing.Jobs j JOIN auditing.fn_CatalogTree($($module.IdParent)) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType WHERE j.Holder = '$Holder';
SELECT CONCAT('ACTIVE=', COUNT(*)) FROM auditing.Jobs j JOIN auditing.fn_CatalogTree($($module.IdParent)) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType WHERE j.Holder = '$Holder' AND IsActive = 1;
"@

    [pscustomobject]@{
        Modulo     = $name
        Parametros = if ($module.Table) { if ($null -ne $inserted) { "$inserted inserida(s)" } else { 'pulado' } } else { '-' }
        Empresas   = $companies -join ', '
        JobsNovos  = $created
        Jobs       = Get-Marker $state 'TOTAL'
        Ativos     = Get-Marker $state 'ACTIVE'
        Avisos     = $warnings
    }
}

$summary | Select-Object Modulo, Parametros, Empresas, JobsNovos, Jobs, Ativos | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

$summary | ForEach-Object { $m = $_.Modulo; $_.Avisos | ForEach-Object { Write-Warning "${m}: $_" } }

# CNPJs de parâmetros sem empresa, em todos os módulos com tabela no banco: nenhum serviço os lê
$orphanQuery = ($catalog.Values | Where-Object Table | ForEach-Object {
        "IF OBJECT_ID('$($_.Table)') IS NOT NULL SELECT DISTINCT CONCAT('ORPHAN=$($_.Table) ', p.doc_company) FROM $($_.Table) p WHERE NOT EXISTS (SELECT 1 FROM auditing.Companies c WHERE c.Company = p.doc_company);"
    }) -join "`n"
Get-Markers (Invoke-Sql -Query $orphanQuery) 'ORPHAN' | ForEach-Object {
    Write-Warning "CNPJ de parâmetro sem empresa em auditing.Companies (nenhum serviço lê): $_"
}

Write-Host 'Os scripts de parâmetros têm credenciais reais: antes de subir um host contra este banco, aponte' -ForegroundColor Yellow
Write-Host '<Módulo>__BaseUrl para http://127.0.0.1:9/ (AfterSale, Movidesk, LinxMicrovix e Totvs).' -ForegroundColor Yellow
