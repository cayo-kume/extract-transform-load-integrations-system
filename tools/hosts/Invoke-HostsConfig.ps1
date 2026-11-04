#Requires -Version 7
<#
.SYNOPSIS
    Configura, num banco já migrado, a empresa, as credenciais das APIs dos módulos, quais jobs ficam ativos e o que os
    hosts de agenda e o WebApp precisam para rodar contra ele, por uma tela Windows Forms.

.DESCRIPTION
    É a segunda parte do tools/migrators/Invoke-Migrators.ps1: aquele sobe as tabelas; este cadastra e escolhe o que roda.

    1. Conecta no banco (servidor, porta, banco, usuário e senha) e lista as empresas de monitor.Company. Nova empresa
       cadastra uma (auditing.sp_Add_Company e auditing.sp_Add_Company_App dos módulos contratados, jobs inativos).
    2. Mostra as integrações da empresa em auditing.Jobs (IdType = 2, as que os hosts agendam; as procedures rodam dentro
       do job que as chama e ficam fora), agrupadas por aplicação (auditing.fn_JobCatalogTree), com uma caixa por job:
       marcada é IsActive = 1. Numa árvore: à esquerda do nome de cada aplicação, a seta abre e fecha e a caixa marca ou
       desmarca todos os jobs dela.
       Os módulos com API externa (Totvs, LinxMicrovix, LinxCommerce, Movidesk e AfterSale) só ativam jobs com a
       credencial de ao menos um CNPJ da empresa na tabela <Módulo>Parameters: marcar sem ela abre o diálogo da
       credencial, que entra no Salvar (INSERT ou UPDATE na tabela e o CNPJ ligado à empresa por
       auditing.sp_Add_Holding_Company).
    3. Escolhe o host de agenda (um só por banco): WebApi com o Hangfire, Worker com o Quartz.NET ou WebJob na Azure.
    4. Opcional: grava nos User Secrets do WebApi, do Worker, do WebJob e do WebApp o que depende desta máquina. São as
       connection strings deste banco, o host escolhido ligado e os outros desligados, usuário e senha do dashboard do
       Hangfire, as BaseUrl das APIs numa porta local fechada, o pepper do DataWarehouse e, no WebApp, Database:Enabled, a
       empresa logada, Admin:Enabled, a chave do assistente e os usuários etl_monitor e etl_query.

    Salvar grava numa transação as credenciais e os jobs alterados (só UPDATE de IsActive, na linha da empresa) e depois
    os User Secrets. Os valores das credenciais não passam pelo arquivo do sqlcmd nem pela linha de comando: o script tem
    $(ETL_CRED_n), e o sqlcmd lê os valores de variáveis de ambiente do próprio processo, apagadas em seguida.
    Jobs ativos com as APIs de verdade chamam as APIs de verdade quando um host subir: a tela pede confirmação.

    Os User Secrets são só da máquina de desenvolvimento: em produção os valores vêm de variável de ambiente. Gravar é
    direto no secrets.json de cada projeto (nenhum valor em linha de comando). Um projeto sem UserSecretsId ganha um no
    .csproj, gravado com a mesma codificação. O log mostra só os nomes das chaves, nunca os valores.

    A senha vai para o sqlcmd pela variável SQLCMDPASSWORD, nunca pela linha de comando.

.PARAMETER Server
    IP ou nome do servidor; sem ele, localhost.

.PARAMETER Port
    Porta do SQL Server; sem ela, 1433.

.PARAMETER Database
    Banco já migrado; sem ele, NEWBLOOMERS.

.PARAMETER SchedulingHost
    Host de agenda já escolhido na tela: WebApi, Worker ou WebJob.

.EXAMPLE
    ./tools/hosts/Invoke-HostsConfig.ps1

.EXAMPLE
    ./tools/hosts/Invoke-HostsConfig.ps1 -Database EtlTeste -SchedulingHost Worker
#>
[CmdletBinding()]
param(
    [string]$Server = 'localhost',
    [ValidateRange(1, 65535)]
    [int]$Port = 1433,
    [string]$Database = 'NEWBLOOMERS',
    [ValidateSet('WebApi', 'Worker', 'WebJob')]
    [string]$SchedulingHost
)

$ErrorActionPreference = 'Stop'

$root = Join-Path $PSScriptRoot '../..' | Resolve-Path | Select-Object -ExpandProperty Path

if ([Threading.Thread]::CurrentThread.ApartmentState -ne 'STA') {
    $arguments = @('-NoProfile', '-STA', '-File', $PSCommandPath, '-Server', $Server, '-Port', $Port, '-Database', $Database)
    if ($SchedulingHost) { $arguments += '-SchedulingHost', $SchedulingHost }
    & (Get-Process -Id $PID).Path @arguments
    return
}

# a saída do sqlcmd vem em UTF-8 (-f 65001): sem isto, os acentos dos títulos chegam trocados
[Console]::OutputEncoding = [Text.Encoding]::UTF8

# ---------------------------------------------------------------------------------------------------------------------
# hosts de agenda e projetos
# ---------------------------------------------------------------------------------------------------------------------

# um só agenda por banco (README, "Um host de agenda por vez"): o escolhido fica ligado e os outros desligados
$schedulingHosts = [ordered]@{
    WebApi = @{ Title = 'WebApi (Hangfire)'; Description = 'Hangfire:Enabled = true no WebApi; Quartz:Enabled = false no Worker. WebJobs parados no App Service.' }
    Worker = @{ Title = 'Worker (Quartz.NET)'; Description = 'Quartz:Enabled = true no Worker; Hangfire:Enabled = false no WebApi (as rotas continuam).' }
    WebJob = @{ Title = 'WebJob (Azure)'; Description = 'Hangfire e Quartz desligados; os WebJobs rodam no App Service (publish-webjob.ps1).' }
}

$projects = [ordered]@{
    WebApi = Join-Path $root 'src/Backend/Hosts/WebApi'
    Worker = Join-Path $root 'src/Backend/Hosts/Worker'
    WebJob = Join-Path $root 'src/Backend/Hosts/WebJob'
    WebApp = Join-Path $root 'src/Frontend/WebApp'
}

# APIs que os jobs dos hosts chamam; o Linx Commerce não tem BaseUrl (o endereço é de cada cliente, nos parâmetros)
$apiBaseUrls = 'AfterSale:BaseUrl', 'Movidesk:BaseUrl', 'LinxMicrovix:BaseUrl', 'Totvs:BaseUrl'

# a credencial da API de cada módulo, pelo IdApp (auditing.Apps): a tabela de parâmetros e as colunas que a tela pede. Os
# jobs de um módulo daqui só são ativados com a credencial de ao menos um CNPJ da empresa; DataWarehouse e Hosts não
# chamam API externa e ficam de fora. Chave em texto: no [ordered], um inteiro no índice é a posição, não a chave
$credentialModules = [ordered]@{
    '130' = @{ Name = 'Totvs'; Table = '[totvs].[TotvsParameters]'; Fields = @(
            @{ Column = 'client_id'; Required = $true }
            @{ Column = 'client_secret'; Required = $true; Secret = $true }
            @{ Column = 'username'; Required = $true }
            @{ Column = 'password'; Required = $true; Secret = $true }) }
    '80' = @{ Name = 'LinxMicrovix'; Table = '[linxmicrovix].[LinxMicrovixParameters]'; AnyOf = 'erp_chave', 'b2c_chave'; Fields = @(
            @{ Column = 'grupo' }
            @{ Column = 'erp_chave'; Guid = $true; Secret = $true }
            @{ Column = 'erp_user_name' }
            @{ Column = 'erp_password'; Secret = $true }
            @{ Column = 'b2c_chave'; Guid = $true; Secret = $true }
            @{ Column = 'b2c_user_name' }
            @{ Column = 'b2c_password'; Secret = $true }) }
    '70' = @{ Name = 'LinxCommerce'; Table = '[linxcommerce].[LinxCommerceParameters]'; Fields = @(
            @{ Column = 'base_url'; Required = $true }
            @{ Column = 'user_name'; Required = $true }
            @{ Column = 'api_key'; Required = $true; Secret = $true }) }
    '30' = @{ Name = 'Movidesk'; Table = '[movidesk].[MovideskParameters]'; Fields = @(
            @{ Column = 'token'; Required = $true; Guid = $true; Secret = $true }) }
    '50' = @{ Name = 'AfterSale'; Table = '[aftersale].[AfterSaleParameters]'; Fields = @(
            @{ Column = 'token'; Required = $true; Guid = $true; Secret = $true }) }
}

# módulos que uma empresa nova pode contratar (auditing.sp_Add_Company_App cria os jobs deles, inativos)
$companyModules = [ordered]@{ Totvs = 130; LinxMicrovix = 80; LinxCommerce = 70; Movidesk = 30; AfterSale = 50; DataWarehouse = 90 }

# ---------------------------------------------------------------------------------------------------------------------
# banco
# ---------------------------------------------------------------------------------------------------------------------

# roda um script no banco e devolve as linhas, colunas separadas por |; -b faz o sqlcmd sair com erro no primeiro erro de SQL.
# $variables vira variável de ambiente só durante a chamada: o script usa $(NOME) e o sqlcmd troca pelo valor, então
# valores de credencial não passam pelo arquivo temporário nem pela linha de comando
function Invoke-Sql($fields, [string]$query, [string]$database = $fields.Database, [hashtable]$variables = @{}) {
    $file = New-TemporaryFile

    try {
        foreach ($name in $variables.Keys) { Set-Item -LiteralPath "Env:$name" -Value $variables[$name] }

        Set-Content -LiteralPath $file -Value "SET NOCOUNT ON;`n$query" -Encoding utf8

        $arguments = @('-S', "$($fields.Server),$($fields.Port)", '-d', $database, '-U', $fields.User, '-b', '-I', '-f', '65001',
            '-h', '-1', '-W', '-s', '|', '-l', '15', '-i', $file.FullName)
        if ($fields.Trust) { $arguments += '-C' }

        $env:SQLCMDPASSWORD = $fields.Password

        try {
            $output = & sqlcmd @arguments 2>&1
        }
        finally {
            Remove-Item Env:SQLCMDPASSWORD -ErrorAction SilentlyContinue
        }

        if ($LASTEXITCODE -ne 0) { throw (($output | Where-Object { "$_".Trim() }) -join ' ') }

        @($output | ForEach-Object { "$_" } | Where-Object { $_.Trim() })
    }
    finally {
        Remove-Item -LiteralPath $file -ErrorAction SilentlyContinue
        foreach ($name in $variables.Keys) { Remove-Item -LiteralPath "Env:$name" -ErrorAction SilentlyContinue }
    }
}

function Get-Companies($fields) {
    Invoke-Sql $fields "SELECT Company, REPLACE([Name], '|', '/'), CAST(IsActive AS int) FROM [monitor].[Company] ORDER BY [Name];" |
        ForEach-Object {
            $columns = $_ -split '\|', 3
            [pscustomobject]@{ Company = $columns[0]; Name = $columns[1]; IsActive = $columns[2] -eq '1' }
        }
}

# as integrações da empresa (IdType = 2, as únicas que os hosts agendam; as procedures rodam dentro do job que as chama)
# com a aplicação de cada uma, pela árvore do módulo
function Get-Jobs($fields, [string]$company) {
    if ($company -notmatch '^\d+$') { throw "CNPJ inválido: $company" }

    Invoke-Sql $fields @"
WITH JobApps AS (
    SELECT t.IdJob, t.IdType, MIN(a.IdParent) AS IdApp
    FROM [auditing].[Apps] a
    CROSS APPLY [auditing].[fn_JobCatalogTree](a.IdParent) t
    GROUP BY t.IdJob, t.IdType
)
SELECT
    COALESCE(ja.IdApp, j.IdParent, 0),
    REPLACE(COALESCE(a.Title, CONCAT(N'Aplicação ', COALESCE(ja.IdApp, j.IdParent))), '|', '/'),
    j.IdJob,
    j.IdType,
    REPLACE(COALESCE(ty.JobType, ''), '|', '/'),
    CAST(j.IsActive AS int),
    j.IntervalTimeInMinutes,
    COALESCE(CONVERT(varchar(5), j.RunningDailyAt, 108), ''),
    j.Queue,
    REPLACE(COALESCE(j.Title, ''), '|', '/')
FROM
    [auditing].[Jobs] j
    JOIN [auditing].[Types] ty ON ty.IdType = j.IdType
    LEFT JOIN JobApps ja ON ja.IdJob = j.IdJob AND ja.IdType = j.IdType
    LEFT JOIN [auditing].[Apps] a ON a.IdParent = COALESCE(ja.IdApp, j.IdParent)
WHERE
    j.Company = '$company'
    AND j.IdType = 2
ORDER BY
    2, j.IdJob;
"@ | ForEach-Object {
        $columns = $_ -split '\|', 10
        [pscustomobject]@{
            IdApp = [int]$columns[0]; AppTitle = $columns[1]; IdJob = [int]$columns[2]; IdType = [int]$columns[3]
            JobType = $columns[4]; IsActive = $columns[5] -eq '1'; Interval = [int]$columns[6]; DailyAt = $columns[7]
            Queue = $columns[8]; Title = $columns[9]; Key = "$($columns[2])|$($columns[3])"
        }
    }
}

# os CNPJs com credencial em cada módulo, da própria empresa ou das marcas dela (monitor.HoldingCompanies), e as tabelas
# de parâmetros que existem no banco (sem a tabela, o migrator do módulo não rodou e a credencial não tem onde entrar)
function Get-Credentials($fields, [string]$company) {
    if ($company -notmatch '^\d+$') { throw "CNPJ inválido: $company" }

    $query = ($credentialModules.Keys | ForEach-Object {
            $table = $credentialModules[$_].Table
            @"
IF OBJECT_ID('$table') IS NOT NULL
BEGIN
    SELECT 'TABLE|$_';
    SELECT CONCAT('DOC|$_|', doc_company) FROM $table
    WHERE doc_company = '$company' OR doc_company IN (SELECT Company FROM [monitor].[HoldingCompanies] WHERE Holding = '$company');
END
"@
        }) -join "`n"

    $credentials = @{ Tables = [Collections.Generic.HashSet[string]]::new(); Docs = @{} }

    foreach ($line in (Invoke-Sql $fields $query)) {
        $columns = $line -split '\|'
        if ($columns[0] -eq 'TABLE') { $null = $credentials.Tables.Add($columns[1]) }
        elseif ($columns[0] -eq 'DOC' -and $columns[2]) { $credentials.Docs[$columns[1]] = @($credentials.Docs[$columns[1]] | Where-Object { $_ }) + $columns[2] }
    }

    $credentials
}

# grava numa transação só: cada credencial nova ou trocada (INSERT ou UPDATE na tabela do módulo, o CNPJ ligado à
# empresa e os jobs do módulo criados se faltarem, inativos) e depois os jobs alterados. Os valores das credenciais vão
# como $(ETL_CRED_n) no script e chegam ao sqlcmd por variável de ambiente; devolve as linhas de auditing.Jobs alteradas
function Save-Changes($fields, [string]$company, $credentials, $changes) {
    if ($company -notmatch '^\d+$') { throw "CNPJ inválido: $company" }

    $sql = [Text.StringBuilder]::new()
    $variables = @{}
    $null = $sql.AppendLine('SET XACT_ABORT ON;').AppendLine('BEGIN TRANSACTION;')

    foreach ($credential in @($credentials)) {
        $doc = $credential.Doc
        if ($doc -notmatch '^\d+$') { throw "CNPJ inválido: $doc" }

        $table = $credentialModules["$($credential.IdApp)"].Table
        $columns = @()
        $values = @()

        foreach ($column in $credential.Values.Keys) {
            $name = "ETL_CRED_$($variables.Count + 1)"
            $variables[$name] = $credential.Values[$column].Replace("'", "''")
            $columns += "[$column]"
            $values += "N'`$($name)'"
        }

        if ($columns) {
            $sets = for ($i = 0; $i -lt $columns.Count; $i++) { "$($columns[$i]) = $($values[$i])" }
            $null = $sql.AppendLine("IF EXISTS (SELECT 1 FROM $table WHERE doc_company = '$doc')").
                AppendLine("    UPDATE $table SET $($sets -join ', '), integrated_or_updated_at = GETDATE() WHERE doc_company = '$doc';").
                AppendLine('ELSE').
                AppendLine("    INSERT INTO $table (integrated_or_updated_at, doc_company, $($columns -join ', ')) VALUES (GETDATE(), '$doc', $($values -join ', '));")
        }

        $null = $sql.AppendLine("EXEC [auditing].[sp_Add_Holding_Company] @company = '$company', @docCompany = '$doc';").
            AppendLine("EXEC [auditing].[sp_Add_Company_App] @company = '$company', @idParent = $($credential.IdApp);")
    }

    if ($changes) {
        $rows = ($changes | ForEach-Object { "($($_.IdJob), $($_.IdType), $([int]$_.IsActive))" }) -join ",`n    "
        $null = $sql.AppendLine(@"
UPDATE j
SET IsActive = v.IsActive
FROM
    [auditing].[Jobs] j
    JOIN (VALUES
    $rows
    ) v (IdJob, IdType, IsActive) ON v.IdJob = j.IdJob AND v.IdType = j.IdType
WHERE
    j.Company = '$company';
SELECT CONCAT('JOBS=', @@ROWCOUNT);
"@)
    }

    $null = $sql.AppendLine('COMMIT TRANSACTION;')
    $output = Invoke-Sql $fields $sql.ToString() -variables $variables
    $jobs = @($output | Where-Object { $_ -like 'JOBS=*' }) | Select-Object -Last 1
    $jobs ? [int]$jobs.Substring(5) : 0
}

# cadastra a empresa (auditing.sp_Add_Company, que já cria os jobs dos hosts) e os módulos contratados, numa transação
function Add-Company($fields, [string]$company, [string]$name, [int[]]$idApps) {
    if ($company -notmatch '^\d+$') { throw "CNPJ inválido: $company" }

    $apps = ($idApps | ForEach-Object { "EXEC [auditing].[sp_Add_Company_App] @company = '$company', @idParent = $_;" }) -join "`n"
    $null = Invoke-Sql $fields @"
SET XACT_ABORT ON;
BEGIN TRANSACTION;
EXEC [auditing].[sp_Add_Company] @company = '$company', @name = N'`$(ETL_COMPANY_NAME)';
$apps
COMMIT TRANSACTION;
"@ -variables @{ ETL_COMPANY_NAME = $name.Replace("'", "''") }
}

function New-ConnectionString([string]$server, [int]$port, [string]$database, [string]$user, [string]$password, [bool]$trust) {
    $builder = [Data.Common.DbConnectionStringBuilder]::new()
    $builder['Server'] = "$server,$port"
    $builder['Database'] = $database
    $builder['User Id'] = $user
    $builder['Password'] = $password

    if ($trust) { $builder['TrustServerCertificate'] = 'True' }

    $builder.ConnectionString
}

# o que vai para os User Secrets de cada projeto. Set grava, Remove tira (vale o appsettings.json); campo em branco do
# formulário (pepper, chave do assistente) deixa a chave como está
function New-ProjectSettings($fields, [string]$schedulingHostKey, [string]$company, [bool]$apisOff, [string]$dashboardUser,
    [string]$dashboardPassword, [string]$pepper, [bool]$adminEnabled, [string]$assistantApiKey, [string]$monitorPassword,
    [string]$queryPassword) {
    $connection = New-ConnectionString $fields.Server $fields.Port $fields.Database $fields.User $fields.Password $fields.Trust

    function New-HostSettings([hashtable]$extra) {
        $set = @{ 'ConnectionStrings:Connection' = $connection }
        $remove = @()

        foreach ($key in $apiBaseUrls) {
            if ($apisOff) { $set[$key] = 'http://127.0.0.1:9/' } else { $remove += $key }
        }

        if ($pepper) { $set['DataWarehouse:Pepper'] = $pepper }
        foreach ($key in $extra.Keys) { $set[$key] = $extra[$key] }

        @{ Set = $set; Remove = $remove }
    }

    $webApi = New-HostSettings @{ 'Hangfire:Enabled' = ($schedulingHostKey -eq 'WebApi' ? 'true' : 'false') }
    if ($schedulingHostKey -eq 'WebApi') {
        $webApi.Set['Hangfire:Dashboard:UserName'] = $dashboardUser
        $webApi.Set['Hangfire:Dashboard:Password'] = $dashboardPassword
    }

    # WebApp: etl_monitor e etl_query quando a senha deles veio no formulário; sem ela, o usuário desta conexão
    $webApp = @{
        Set = @{
            'Database:Enabled' = 'true'
            'LoggedCompany:DocCompany' = $company
            'Admin:Enabled' = ($adminEnabled ? 'true' : 'false')
            'ConnectionStrings:Connection' = $monitorPassword ?
                (New-ConnectionString $fields.Server $fields.Port $fields.Database 'etl_monitor' $monitorPassword $fields.Trust) : $connection
            'ConnectionStrings:QueryConnection' = $queryPassword ?
                (New-ConnectionString $fields.Server $fields.Port $fields.Database 'etl_query' $queryPassword $fields.Trust) : $connection
        }
        Remove = @()
    }
    if ($assistantApiKey) { $webApp.Set['Assistant:ApiKey'] = $assistantApiKey }

    @(
        @{ Name = 'WebApi'; Folder = $projects.WebApi } + $webApi
        @{ Name = 'Worker'; Folder = $projects.Worker } + (New-HostSettings @{ 'Quartz:Enabled' = ($schedulingHostKey -eq 'Worker' ? 'true' : 'false') })
        @{ Name = 'WebJob'; Folder = $projects.WebJob } + (New-HostSettings @{})
        @{ Name = 'WebApp'; Folder = $projects.WebApp } + $webApp
    )
}

# grava direto no secrets.json do projeto (nada de valor em linha de comando); devolve as linhas do log, só com os nomes
function Set-ProjectSecrets($project) {
    $csproj = Get-ChildItem -LiteralPath $project.Folder -Filter '*.csproj' -File | Select-Object -First 1
    if (-not $csproj) { throw "nenhum .csproj em $($project.Folder)" }

    $pattern = '<UserSecretsId>([^<]+)</UserSecretsId>'
    $id = [regex]::Match((Get-Content -Raw -LiteralPath $csproj.FullName), $pattern).Groups[1].Value

    # Sem UserSecretsId, a linha entra no primeiro PropertyGroup mantendo a codificação e as quebras de linha do .csproj
    # (o dotnet user-secrets init grava com BOM e CRLF e suja o diff).
    if (-not $id) {
        $bytes = [IO.File]::ReadAllBytes($csproj.FullName)
        $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        $text = [Text.UTF8Encoding]::new($false).GetString($bytes, ($hasBom ? 3 : 0), $bytes.Length - ($hasBom ? 3 : 0))
        $newLine = $text.Contains("`r`n") ? "`r`n" : "`n"
        $id = [guid]::NewGuid().ToString()
        $match = [regex]::Match($text, '(?m)^([ \t]*)</PropertyGroup>')
        if (-not $match.Success) { throw "nenhum PropertyGroup em $($csproj.Name)" }

        $text = $text.Insert($match.Index, "$($match.Groups[1].Value)  <UserSecretsId>$id</UserSecretsId>$newLine")
        [IO.File]::WriteAllText($csproj.FullName, $text, [Text.UTF8Encoding]::new($hasBom))
        "$($project.Name): UserSecretsId criado no $($csproj.Name)"
    }

    $file = Join-Path $env:APPDATA "Microsoft\UserSecrets\$id\secrets.json"
    $values = (Test-Path -LiteralPath $file) ? (Get-Content -Raw -LiteralPath $file | ConvertFrom-Json -AsHashtable) : $null
    if (-not $values) { $values = @{} }

    foreach ($key in $project.Set.Keys) { $values[$key] = $project.Set[$key] }
    $removed = @($project.Remove | Where-Object { $values.ContainsKey($_) })
    foreach ($key in $removed) { $values.Remove($key) }

    $null = New-Item -ItemType Directory -Force -Path (Split-Path $file)
    $values | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $file -Encoding utf8

    "$($project.Name): $(@($project.Set.Keys | Sort-Object) -join ', ')"
    if ($removed) { "$($project.Name), removidas: $($removed -join ', ')" }
}

# ---------------------------------------------------------------------------------------------------------------------
# tela
# ---------------------------------------------------------------------------------------------------------------------

Add-Type -AssemblyName System.Windows.Forms, System.Drawing
[Windows.Forms.Application]::EnableVisualStyles()

$form = [Windows.Forms.Form]@{
    Text = 'Hosts e monitor: jobs ativos e configuração'
    ClientSize = [Drawing.Size]::new(860, 968)
    MinimumSize = [Drawing.Size]::new(760, 860)
    StartPosition = 'CenterScreen'
    Font = [Drawing.Font]::new('Segoe UI', 9)
}

function New-Label([string]$text, [int]$x, [int]$y) { [Windows.Forms.Label]@{ Text = $text; Location = [Drawing.Point]::new($x, $y); AutoSize = $true } }

# banco

$connection = [Windows.Forms.GroupBox]@{
    Text = 'Banco de dados (já migrado)'; Location = [Drawing.Point]::new(12, 10); Size = [Drawing.Size]::new(836, 112)
    Anchor = 'Top, Left, Right'
}

function Add-Field($group, [string]$label, [int]$x, [int]$y, [int]$width, $control) {
    $group.Controls.Add((New-Label $label $x $y))
    $control.Location = [Drawing.Point]::new($x, $y + 18)
    $control.Width = $width
    $group.Controls.Add($control)
    $control
}

$serverBox = Add-Field $connection 'Servidor (IP)' 14 22 200 ([Windows.Forms.TextBox]@{ Text = $Server })
$portBox = Add-Field $connection 'Porta' 226 22 80 ([Windows.Forms.NumericUpDown]@{ Minimum = 1; Maximum = 65535 })
$portBox.Value = $Port
$databaseBox = Add-Field $connection 'Banco' 318 22 170 ([Windows.Forms.TextBox]@{ Text = $Database })
$userBox = Add-Field $connection 'Usuário' 500 22 150 ([Windows.Forms.TextBox]@{ Text = 'sa' })
$passwordBox = Add-Field $connection 'Senha' 662 22 160 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })
$trustBox = [Windows.Forms.CheckBox]@{
    Text = 'Confiar no certificado do servidor (TrustServerCertificate)'; Checked = $true; Location = [Drawing.Point]::new(14, 78); AutoSize = $true
}
$loadButton = [Windows.Forms.Button]@{ Text = 'Carregar empresas'; Location = [Drawing.Point]::new(682, 74); Size = [Drawing.Size]::new(140, 28) }
$connection.Controls.AddRange(@($trustBox, $loadButton))

# host de agenda

$hostGroup = [Windows.Forms.GroupBox]@{
    Text = 'Host de agenda (um só por banco)'; Location = [Drawing.Point]::new(12, 130); Size = [Drawing.Size]::new(836, 70); Anchor = 'Top, Left, Right'
}
$hostDescription = [Windows.Forms.Label]@{
    Text = 'Escolha o host que vai agendar os jobs.'; Location = [Drawing.Point]::new(14, 46); AutoSize = $true; ForeColor = [Drawing.SystemColors]::GrayText
}
$hostGroup.Controls.Add($hostDescription)
$hostRadios = @()
$x = 14

# sem AutoCheck, como no Invoke-Migrators: o WinForms marcaria o rádio que recebe o foco
foreach ($key in $schedulingHosts.Keys) {
    $radio = [Windows.Forms.RadioButton]@{ Text = $schedulingHosts[$key].Title; Tag = $key; Location = [Drawing.Point]::new($x, 22); AutoSize = $true; AutoCheck = $false }
    $radio.Add_Click({
        param($sender)
        foreach ($other in $hostRadios) { $other.Checked = $other -eq $sender }
    })
    $radio.Add_CheckedChanged({
        param($sender)
        if ($sender.Checked) {
            $hostDescription.Text = $schedulingHosts[$sender.Tag].Description
            $hostDescription.ForeColor = [Drawing.SystemColors]::ControlText
        }
    })
    $hostGroup.Controls.Add($radio)
    $hostRadios += $radio
    $x += 200
}

if ($SchedulingHost) { ($hostRadios | Where-Object Tag -eq $SchedulingHost).Checked = $true }

# configuração dos projetos

$configGroup = [Windows.Forms.GroupBox]@{
    Text = 'Configuração dos projetos (User Secrets desta máquina)'; Location = [Drawing.Point]::new(12, 208)
    Size = [Drawing.Size]::new(836, 172); Anchor = 'Top, Left, Right'
}
$configBox = [Windows.Forms.CheckBox]@{
    Text = 'Gravar nos User Secrets do WebApi, Worker, WebJob e WebApp: este banco, a empresa e o host de agenda ligado'
    Location = [Drawing.Point]::new(14, 22); AutoSize = $true; Checked = $true
}
$apisOffBox = [Windows.Forms.CheckBox]@{
    Text = 'APIs externas desligadas (BaseUrl em http://127.0.0.1:9/)'; Location = [Drawing.Point]::new(30, 46); AutoSize = $true; Checked = $true
}
$adminBox = [Windows.Forms.CheckBox]@{
    Text = 'Telas de administrador no WebApp (Admin:Enabled)'; Location = [Drawing.Point]::new(430, 46); AutoSize = $true
}
$configGroup.Controls.AddRange(@($configBox, $apisOffBox, $adminBox))

$dashboardUserBox = Add-Field $configGroup 'Usuário do Hangfire (WebApi)' 30 70 150 ([Windows.Forms.TextBox]@{})
$dashboardPasswordBox = Add-Field $configGroup 'Senha do Hangfire' 192 70 150 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })
$pepperBox = Add-Field $configGroup 'Pepper do DataWarehouse' 354 70 200 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })
$assistantBox = Add-Field $configGroup 'Chave da API Anthropic (assistente)' 566 70 256 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })
$monitorBox = Add-Field $configGroup 'Senha do etl_monitor (WebApp)' 30 118 200 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })
$queryBox = Add-Field $configGroup 'Senha do etl_query (WebApp)' 242 118 200 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })
$configGroup.Controls.Add([Windows.Forms.Label]@{
    Text = 'Em branco: o WebApp usa o usuário desta conexão; pepper e chave em branco ficam como estão.'
    Location = [Drawing.Point]::new(454, 120); Size = [Drawing.Size]::new(368, 40); ForeColor = [Drawing.SystemColors]::GrayText
})

$configFields = @($configGroup.Controls | Where-Object { $_ -ne $configBox })
function Update-ConfigFields { foreach ($control in $configFields) { $control.Enabled = $configBox.Checked } }
$configBox.Add_CheckedChanged({ Update-ConfigFields })

# empresa e jobs

$jobsGroup = [Windows.Forms.GroupBox]@{
    Text = 'Empresa e jobs (marcado = ativo)'; Location = [Drawing.Point]::new(12, 388); Size = [Drawing.Size]::new(836, 396)
    Anchor = 'Top, Bottom, Left, Right'
}
$companyBox = Add-Field $jobsGroup 'Empresa (monitor.Company)' 14 18 300 ([Windows.Forms.ComboBox]@{ DropDownStyle = 'DropDownList' })
$newCompanyButton = [Windows.Forms.Button]@{ Text = 'Nova empresa…'; Location = [Drawing.Point]::new(320, 35); Size = [Drawing.Size]::new(110, 25) }
$filterBox = Add-Field $jobsGroup 'Filtro (IdJob, título ou aplicação)' 442 18 250 ([Windows.Forms.TextBox]@{})
$changesLabel = [Windows.Forms.Label]@{
    Text = ''; Location = [Drawing.Point]::new(484, 362); AutoSize = $true; ForeColor = [Drawing.Color]::DarkOrange; Anchor = 'Bottom, Left'
}

# árvore: cada aplicação com a seta e a caixa à esquerda do nome (a caixa marca ou desmarca os jobs dela) e, dentro, os
# jobs, com as colunas desenhadas no DrawNode; Agenda e Fila ficam a uma distância fixa da borda direita
$tree = [Windows.Forms.TreeView]@{
    CheckBoxes = $true; DrawMode = 'OwnerDrawText'; ShowLines = $false; HideSelection = $false; ItemHeight = 20
    Location = [Drawing.Point]::new(14, 90); Size = [Drawing.Size]::new(808, 258); Anchor = 'Top, Bottom, Left, Right'
}
$scheduleColumn = 250
$queueColumn = 120

function New-ColumnLabel([string]$text, [int]$x, [string]$anchor) {
    [Windows.Forms.Label]@{ Text = $text; Location = [Drawing.Point]::new($x, 72); AutoSize = $true; Anchor = $anchor; ForeColor = [Drawing.SystemColors]::GrayText; BackColor = [Drawing.Color]::Transparent }
}
$idColumnLabel = New-ColumnLabel 'IdJob' 74 'Top, Left'
$titleColumnLabel = New-ColumnLabel 'Título' 134 'Top, Left'
$scheduleColumnLabel = New-ColumnLabel 'Agenda' ($tree.Right - $scheduleColumn) 'Top, Left'
$queueColumnLabel = New-ColumnLabel 'Fila' ($tree.Right - $queueColumn) 'Top, Left'

# os títulos acompanham as colunas desenhadas: x é onde começa o texto do job (o da aplicação + um recuo), e Agenda e
# Fila contam da largura útil, que muda com a barra de rolagem
function Set-ColumnLabels([int]$x) {
    $positions = @(
        @{ Label = $idColumnLabel; Left = $tree.Left + $x + 2 }
        @{ Label = $titleColumnLabel; Left = $tree.Left + $x + 62 }
        @{ Label = $scheduleColumnLabel; Left = $tree.Left + $tree.ClientSize.Width - $scheduleColumn + 2 }
        @{ Label = $queueColumnLabel; Left = $tree.Left + $tree.ClientSize.Width - $queueColumn + 2 }
    )
    foreach ($position in $positions) {
        if ($position.Label.Left -ne $position.Left) { $position.Label.Left = $position.Left }
    }
}

function New-Button([string]$text, [int]$x, [int]$width) {
    [Windows.Forms.Button]@{ Text = $text; Location = [Drawing.Point]::new($x, 356); Size = [Drawing.Size]::new($width, 28); Anchor = 'Bottom, Left' }
}
$checkAllButton = New-Button 'Ativar os da lista' 14 130
$uncheckAllButton = New-Button 'Desativar os da lista' 150 140
$credentialsButton = New-Button 'Credenciais do módulo…' 296 170
$jobsGroup.Controls.AddRange(@($newCompanyButton, $changesLabel, $idColumnLabel, $titleColumnLabel, $scheduleColumnLabel, $queueColumnLabel, $tree,
    $checkAllButton, $uncheckAllButton, $credentialsButton))

# salvar, log e status

$saveButton = [Windows.Forms.Button]@{
    Text = 'Salvar'; Location = [Drawing.Point]::new(616, 792); Size = [Drawing.Size]::new(120, 28); Anchor = 'Bottom, Right'
}
$closeButton = [Windows.Forms.Button]@{
    Text = 'Fechar'; Location = [Drawing.Point]::new(742, 792); Size = [Drawing.Size]::new(106, 28); Anchor = 'Bottom, Right'
}
$log = [Windows.Forms.RichTextBox]@{
    Location = [Drawing.Point]::new(12, 828); Size = [Drawing.Size]::new(836, 110); Anchor = 'Bottom, Left, Right'
    ReadOnly = $true; BackColor = [Drawing.Color]::FromArgb(30, 30, 30); ForeColor = [Drawing.Color]::Gainsboro
    Font = [Drawing.Font]::new('Consolas', 9); WordWrap = $false
}
$status = [Windows.Forms.Label]@{ Text = 'Preencha a senha e carregue as empresas.'; Location = [Drawing.Point]::new(12, 944); AutoSize = $true; Anchor = 'Bottom, Left' }

$form.Controls.AddRange(@($connection, $hostGroup, $configGroup, $jobsGroup, $saveButton, $closeButton, $log, $status))

$logColors = @{ out = [Drawing.Color]::Gainsboro; step = [Drawing.Color]::DeepSkyBlue; ok = [Drawing.Color]::LightGreen; error = [Drawing.Color]::Tomato; warn = [Drawing.Color]::Gold }

function Write-Log([string]$text, [string]$kind = 'out') {
    $log.SelectionStart = $log.TextLength
    $log.SelectionLength = 0
    $log.SelectionColor = $logColors[$kind]
    $log.AppendText("$text`n")
    $log.ScrollToCaret()
}

# ---------------------------------------------------------------------------------------------------------------------
# estado: os jobs como vieram do banco e a marcação atual, pela chave IdJob|IdType (o filtro reconstrói a lista)
# ---------------------------------------------------------------------------------------------------------------------

$script:jobs = @()
$script:companies = @()
$script:checked = @{}
$script:loadedFields = $null
$script:rebuilding = $false
$script:syncing = $false
$script:collapsed = @{}
$script:filtered = $false
# credenciais: as do banco (Get-Credentials) e as digitadas e ainda não salvas, uma por módulo e CNPJ
$script:credentials = @{ Tables = [Collections.Generic.HashSet[string]]::new(); Docs = @{} }
$script:pending = [Collections.Generic.List[object]]::new()

function Get-Fields {
    $missing = @()
    if (-not $serverBox.Text.Trim()) { $missing += 'servidor' }
    if (-not $databaseBox.Text.Trim()) { $missing += 'banco' }
    if (-not $userBox.Text.Trim()) { $missing += 'usuário' }
    if (-not $passwordBox.Text) { $missing += 'senha' }

    if ($missing) {
        $null = [Windows.Forms.MessageBox]::Show("Preencha: $($missing -join ', ').", $form.Text, 'OK', 'Warning')
        return $null
    }

    @{
        Server = $serverBox.Text.Trim(); Port = [int]$portBox.Value; Database = $databaseBox.Text.Trim()
        User = $userBox.Text.Trim(); Password = $passwordBox.Text; Trust = $trustBox.Checked
    }
}

function Get-Changes { @($script:jobs | Where-Object { $script:checked[$_.Key] -ne $_.IsActive }) }

function Test-Unsaved { [bool](Get-Changes) -or $script:pending.Count -gt 0 }
# os CNPJs com credencial de um módulo: os do banco e os digitados ainda não salvos
function Get-CredentialDocs([string]$idApp) {
    @(@($script:credentials.Docs[$idApp]) + @($script:pending | Where-Object IdApp -eq $idApp | ForEach-Object Doc) |
        Where-Object { $_ } | Select-Object -Unique)
}

# módulo sem API externa (DataWarehouse, Hosts) não precisa de credencial
function Test-HasCredential([string]$idApp) {
    -not $credentialModules.Contains($idApp) -or [bool](Get-CredentialDocs $idApp)
}

# o que o nome da aplicação mostra sobre a credencial
function Get-CredentialText([string]$idApp) {
    if (-not $credentialModules.Contains($idApp)) { return '' }

    $docs = Get-CredentialDocs $idApp
    if (-not $docs) { return '  ·  sem chave da API' }

    $toSave = @($script:pending | Where-Object IdApp -eq $idApp).Count
    "  ·  chave da API: $($docs.Count) CNPJ$($docs.Count -eq 1 ? '' : 's')$($toSave ? ' (a gravar)' : '')"
}

# o resumo das credenciais a gravar, sem valores: módulo e CNPJ
function Get-PendingText {
    ($script:pending | Group-Object { $credentialModules[$_.IdApp].Name } | ForEach-Object {
            "$($_.Name) ($(($_.Group | ForEach-Object Doc) -join ', '))"
        }) -join '; '
}

# diálogo da credencial de um módulo: o CNPJ (da empresa, de uma marca já cadastrada ou novo) e as colunas da tabela de
# parâmetros. Num CNPJ que já tem credencial, campo em branco fica como está. Não grava: entra em $script:pending, que vai
# para o banco no Salvar, na mesma transação dos jobs. Devolve $true se a credencial entrou
function Show-CredentialDialog([string]$idApp) {
    $module = $credentialModules[$idApp]

    if (-not $script:credentials.Tables.Contains($idApp)) {
        $null = [Windows.Forms.MessageBox]::Show("A tabela $($module.Table) não existe neste banco: rode o migrator do $($module.Name) antes.", $form.Text, 'OK', 'Warning')
        return $false
    }

    $company = $script:companies[$companyBox.SelectedIndex].Company
    $existing = @($script:credentials.Docs[$idApp] | Where-Object { $_ })
    $rows = [Math]::Ceiling($module.Fields.Count / 2)

    $dialog = [Windows.Forms.Form]@{
        Text = "Credenciais do $($module.Name)"; FormBorderStyle = 'FixedDialog'; MaximizeBox = $false; MinimizeBox = $false
        StartPosition = 'CenterParent'; Font = $form.Font; ClientSize = [Drawing.Size]::new(500, 170 + 48 * $rows)
    }
    $dialog.Controls.Add([Windows.Forms.Label]@{
            Text = "Os jobs do $($module.Name) só são ativados com a credencial da API. Ela entra em $($module.Table -replace '[\[\]]', '') ao Salvar, com o CNPJ ligado à empresa. Num CNPJ que já tem credencial, campo em branco fica como está."
            Location = [Drawing.Point]::new(14, 12); Size = [Drawing.Size]::new(472, 48)
        })

    $docBox = Add-Field $dialog 'CNPJ (doc_company, só dígitos)' 14 64 230 ([Windows.Forms.ComboBox]@{ DropDownStyle = 'DropDown' })
    foreach ($doc in (@($company) + $existing | Select-Object -Unique)) { $null = $docBox.Items.Add($doc) }
    $docBox.Text = $existing ? $existing[0] : $company

    $boxes = [ordered]@{}
    for ($i = 0; $i -lt $module.Fields.Count; $i++) {
        $field = $module.Fields[$i]
        $label = "$($field.Column)$($field.Required ? ' *' : '')"
        $boxes[$field.Column] = Add-Field $dialog $label (14 + 242 * ($i % 2)) (112 + 48 * [Math]::Floor($i / 2)) 230 `
            ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = [bool]$field.Secret })
    }

    $hint = $module.AnyOf ? "Preencha ao menos um de: $($module.AnyOf -join ', '). " : ''
    $dialog.Controls.Add([Windows.Forms.Label]@{
            Text = "$hint* obrigatório num CNPJ novo."; Location = [Drawing.Point]::new(14, 120 + 48 * $rows); AutoSize = $true
            ForeColor = [Drawing.SystemColors]::GrayText
        })
    $okButton = [Windows.Forms.Button]@{
        Text = 'Gravar credencial'; DialogResult = 'OK'; Location = [Drawing.Point]::new(260, 132 + 48 * $rows); Size = [Drawing.Size]::new(130, 28)
    }
    $cancelButton = [Windows.Forms.Button]@{
        Text = 'Cancelar'; DialogResult = 'Cancel'; Location = [Drawing.Point]::new(396, 132 + 48 * $rows); Size = [Drawing.Size]::new(90, 28)
    }
    $dialog.Controls.AddRange(@($okButton, $cancelButton))
    $dialog.CancelButton = $cancelButton

    try {
        # o diálogo reabre com o que foi digitado até a credencial passar na conferência
        while ($true) {
            if ($dialog.ShowDialog($form) -ne 'OK') { return $false }

            $doc = $docBox.Text.Trim()
            $isNew = $doc -notin $existing
            $values = [ordered]@{}
            foreach ($column in $boxes.Keys) { if ($boxes[$column].Text.Trim()) { $values[$column] = $boxes[$column].Text.Trim() } }

            $problems = @(
                if ($doc -notmatch '^\d+$') { 'o CNPJ precisa ter só dígitos' }
                foreach ($field in $module.Fields) {
                    if ($isNew -and $field.Required -and -not $values.Contains($field.Column)) { "$($field.Column) é obrigatório num CNPJ novo" }
                    if ($field.Guid -and $values.Contains($field.Column) -and -not [guid]::TryParse($values[$field.Column], [ref][guid]::Empty)) {
                        "$($field.Column) precisa ser um GUID (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)"
                    }
                }
                if ($isNew -and $module.AnyOf -and -not ($module.AnyOf | Where-Object { $values.Contains($_) })) { "preencha ao menos um de: $($module.AnyOf -join ', ')" }
                if (-not $isNew -and -not $values.Count) { 'nenhum campo preenchido para trocar' }
            )

            if (-not $problems) { break }
            $null = [Windows.Forms.MessageBox]::Show("Confira: $($problems -join '; ').", $dialog.Text, 'OK', 'Warning')
        }

        $null = $script:pending.RemoveAll([Predicate[object]] { param($item) $item.IdApp -eq $idApp -and $item.Doc -eq $doc })
        $script:pending.Add([pscustomobject]@{ IdApp = $idApp; Doc = $doc; Values = $values })
        Write-Log "Credencial do $($module.Name) para o CNPJ $doc pronta para gravar no Salvar." 'step'
        $true
    }
    finally {
        $dialog.Dispose()
    }
}

# diálogo da empresa nova: CNPJ da holding, razão social e os módulos contratados (jobs criados inativos). Grava na hora
# e recarrega as empresas com a nova escolhida
function Show-CompanyDialog {
    $dialog = [Windows.Forms.Form]@{
        Text = 'Nova empresa'; FormBorderStyle = 'FixedDialog'; MaximizeBox = $false; MinimizeBox = $false
        StartPosition = 'CenterParent'; Font = $form.Font; ClientSize = [Drawing.Size]::new(440, 330)
    }
    $cnpjBox = Add-Field $dialog 'CNPJ da holding (só dígitos)' 14 12 190 ([Windows.Forms.TextBox]@{})
    $nameBox = Add-Field $dialog 'Razão social' 216 12 210 ([Windows.Forms.TextBox]@{})
    $modulesBox = Add-Field $dialog 'Módulos contratados (os jobs são criados inativos)' 14 62 412 ([Windows.Forms.CheckedListBox]@{ CheckOnClick = $true; Height = 130 })
    foreach ($name in $companyModules.Keys) { $null = $modulesBox.Items.Add($name, $true) }
    $dialog.Controls.Add([Windows.Forms.Label]@{
            Text = 'Grava com auditing.sp_Add_Company e auditing.sp_Add_Company_App. As credenciais entram depois, ao ativar os jobs de cada módulo.'
            Location = [Drawing.Point]::new(14, 230); Size = [Drawing.Size]::new(412, 36); ForeColor = [Drawing.SystemColors]::GrayText
        })
    $okButton = [Windows.Forms.Button]@{ Text = 'Cadastrar'; DialogResult = 'OK'; Location = [Drawing.Point]::new(240, 288); Size = [Drawing.Size]::new(90, 28) }
    $cancelButton = [Windows.Forms.Button]@{ Text = 'Cancelar'; DialogResult = 'Cancel'; Location = [Drawing.Point]::new(336, 288); Size = [Drawing.Size]::new(90, 28) }
    $dialog.Controls.AddRange(@($okButton, $cancelButton))
    $dialog.CancelButton = $cancelButton

    try {
        while ($true) {
            if ($dialog.ShowDialog($form) -ne 'OK') { return }

            $company = $cnpjBox.Text.Trim()
            $name = $nameBox.Text.Trim()
            $problems = @(
                if ($company -notmatch '^\d+$') { 'o CNPJ precisa ter só dígitos' }
                if (-not $name) { 'preencha a razão social' }
                if ($company -in $script:companies.Company) { "a empresa $company já existe" }
            )

            if (-not $problems) { break }
            $null = [Windows.Forms.MessageBox]::Show("Confira: $($problems -join '; ').", $dialog.Text, 'OK', 'Warning')
        }

        $idApps = @($modulesBox.CheckedItems | ForEach-Object { $companyModules[$_] })
        $form.Cursor = 'WaitCursor'
        Add-Company $script:loadedFields $company $name $idApps
        Write-Log "Empresa $name - $company cadastrada, com os jobs inativos de: $(@($modulesBox.CheckedItems) -join ', ')." 'ok'
        Import-Companies $company
    }
    catch {
        Write-Log "Falha ao cadastrar a empresa: $($_.Exception.Message)" 'error'
    }
    finally {
        $form.Cursor = 'Default'
        $dialog.Dispose()
    }
}

function Format-Schedule($job) {
    if ($job.DailyAt) { return "diário às $($job.DailyAt)" }
    if ($job.Interval -eq 0) { return 'sob demanda' }
    if ($job.Interval % 60 -eq 0) { return "a cada $($job.Interval / 60) h" }
    "a cada $($job.Interval) min"
}

# nó da aplicação: o nome com a contagem da aplicação inteira (não só do filtro) e a caixa marcada se algum job dela na
# lista estiver marcado; desmarcar a caixa desmarca todos, marcar marca todos
function Update-AppNode($node) {
    $appJobs = @($script:jobs | Where-Object IdApp -eq $node.Tag)
    $active = @($appJobs | Where-Object { $script:checked[$_.Key] }).Count
    $node.Text = "$($appJobs[0].AppTitle) ($active de $($appJobs.Count) ativos)$(Get-CredentialText "$($node.Tag)")"

    $syncing = $script:syncing
    $script:syncing = $true
    $node.Checked = [bool]@($node.Nodes | Where-Object Checked)
    $script:syncing = $syncing
}

function Update-ChangesLabel {
    $changes = Get-Changes
    $on = @($changes | Where-Object { $script:checked[$_.Key] }).Count
    $parts = @(
        if ($changes) { "$on a ativar, $($changes.Count - $on) a desativar" }
        if ($script:pending.Count) { "$($script:pending.Count) credencia$($script:pending.Count -eq 1 ? 'l' : 'is')" }
    )
    $changesLabel.Text = $parts ? "Alterações: $($parts -join ', ')" : ''
}

function Update-JobList {
    $filter = $filterBox.Text.Trim()
    $script:syncing = $true
    $tree.BeginUpdate()

    try {
        # guarda o que estava aberto ou fechado; com filtro, as aplicações abrem para mostrar o que achou
        if (-not $script:filtered) {
            foreach ($node in $tree.Nodes) { $script:collapsed[$node.Tag] = -not $node.IsExpanded }
        }
        $script:filtered = [bool]$filter

        $tree.Nodes.Clear()
        $apps = [ordered]@{}

        foreach ($job in $script:jobs) {
            if ($filter -and -not ("$($job.IdJob)".Contains($filter) -or $job.Title -like "*$filter*" -or $job.AppTitle -like "*$filter*")) { continue }

            # chave em texto: no [ordered], um inteiro no índice é a posição, não a chave
            $app = "$($job.IdApp)"
            if (-not $apps.Contains($app)) {
                $apps[$app] = [Windows.Forms.TreeNode]@{ Tag = $job.IdApp }
                $null = $tree.Nodes.Add($apps[$app])
            }

            $null = $apps[$app].Nodes.Add([Windows.Forms.TreeNode]@{ Text = $job.Title; Tag = $job; Checked = [bool]$script:checked[$job.Key] })
        }

        foreach ($node in $apps.Values) {
            Update-AppNode $node
            if ($filter -or $script:collapsed[$node.Tag] -eq $false) { $node.Expand() }
        }
    }
    finally {
        $tree.EndUpdate()
        $script:syncing = $false
    }

    Update-ChangesLabel
}

# as colunas do job; a aplicação usa o desenho padrão
$tree.Add_DrawNode({
    param($sender, $e)

    if ($e.Bounds.IsEmpty -or -not $e.Node.IsVisible) { return }

    if ($e.Node.Level -eq 0) {
        Set-ColumnLabels ($e.Bounds.X + $tree.Indent)
        $e.DrawDefault = $true
        return
    }

    $job = $e.Node.Tag
    $x = $e.Bounds.X
    $width = $tree.ClientSize.Width
    $height = $e.Bounds.Height
    $flags = [Windows.Forms.TextFormatFlags]'VerticalCenter, SingleLine, EndEllipsis, NoPrefix'

    Set-ColumnLabels $x
    $e.Graphics.FillRectangle([Drawing.SystemBrushes]::Window, $x, $e.Bounds.Y, $width - $x, $height)
    $cells = @(
        @{ Text = "$($job.IdJob)"; X = $x; Width = 56 }
        @{ Text = $job.Title; X = $x + 60; Width = $width - $scheduleColumn - $x - 68 }
        @{ Text = Format-Schedule $job; X = $width - $scheduleColumn; Width = $scheduleColumn - $queueColumn - 8 }
        @{ Text = $job.Queue; X = $width - $queueColumn; Width = $queueColumn - 4 }
    )
    foreach ($cell in $cells) {
        $bounds = [Drawing.Rectangle]::new($cell.X, $e.Bounds.Y, [Math]::Max($cell.Width, 0), $height)
        [Windows.Forms.TextRenderer]::DrawText($e.Graphics, $cell.Text, $tree.Font, $bounds, [Drawing.SystemColors]::WindowText, $flags)
    }
})

# marcar um job (ou a aplicação) de um módulo sem credencial pede a credencial antes; sem ela, a caixa não marca.
# Desmarcar sempre pode
$tree.Add_BeforeCheck({
    param($sender, $e)

    if ($script:syncing -or $e.Node.Checked) { return }

    $idApp = $e.Node.Level -eq 0 ? "$($e.Node.Tag)" : "$($e.Node.Tag.IdApp)"
    if (Test-HasCredential $idApp) { return }

    if (-not (Show-CredentialDialog $idApp)) { $e.Cancel = $true }
})

$tree.Add_AfterCheck({
    param($sender, $e)

    if ($script:syncing) { return }

    $node = $e.Node
    $script:syncing = $true

    try {
        if ($node.Level -eq 0) {
            foreach ($child in $node.Nodes) {
                $child.Checked = $node.Checked
                $script:checked[$child.Tag.Key] = $node.Checked
            }
            Update-AppNode $node
        }
        else {
            $script:checked[$node.Tag.Key] = $node.Checked
            Update-AppNode $node.Parent
        }
    }
    finally {
        $script:syncing = $false
    }

    Update-ChangesLabel
})

# marca ou desmarca todos os jobs que estão na lista (com filtro, só os que aparecem)
function Set-AllJobs([bool]$value) {
    $script:syncing = $true
    $tree.BeginUpdate()

    $skipped = @()

    try {
        foreach ($node in $tree.Nodes) {
            if ($value -and -not (Test-HasCredential "$($node.Tag)")) {
                $skipped += $node.Text -replace ' \(.*$', ''
                continue
            }

            foreach ($child in $node.Nodes) {
                $child.Checked = $value
                $script:checked[$child.Tag.Key] = $value
            }
            Update-AppNode $node
        }
    }
    finally {
        $tree.EndUpdate()
        $script:syncing = $false
    }

    if ($skipped) { Write-Log "Sem a chave da API, ficaram desativados: $($skipped -join ', '). Marque a aplicação ou use Credenciais do módulo." 'warn' }
    Update-ChangesLabel
}

$credentialsButton.Add_Click({
    $node = $tree.SelectedNode

    if (-not $node) {
        $null = [Windows.Forms.MessageBox]::Show('Selecione a aplicação na lista.', $form.Text, 'OK', 'Information')
        return
    }

    if ($node.Level -gt 0) { $node = $node.Parent }

    if (-not $credentialModules.Contains("$($node.Tag)")) {
        $appName = $node.Text -replace ' \(.*$', ''
        $null = [Windows.Forms.MessageBox]::Show("$appName não chama API externa: não tem credencial.", $form.Text, 'OK', 'Information')
        return
    }

    if (Show-CredentialDialog "$($node.Tag)") {
        Update-AppNode $node
        Update-ChangesLabel
    }
})

$newCompanyButton.Add_Click({
    if (-not $script:loadedFields) {
        $null = [Windows.Forms.MessageBox]::Show('Carregue as empresas antes (conecta no banco).', $form.Text, 'OK', 'Information')
        return
    }

    if ((Test-Unsaved) -and [Windows.Forms.MessageBox]::Show('Há jobs ou credenciais alterados e não salvos. Cadastrar outra empresa descarta as alterações. Continuar?', $form.Text, 'YesNo', 'Warning') -ne 'Yes') {
        return
    }

    Show-CompanyDialog
})

$checkAllButton.Add_Click({ Set-AllJobs $true })
$uncheckAllButton.Add_Click({ Set-AllJobs $false })
$filterBox.Add_TextChanged({ Update-JobList })

function Import-Jobs {
    $company = $script:companies[$companyBox.SelectedIndex].Company
    $form.Cursor = 'WaitCursor'

    try {
        $script:jobs = @(Get-Jobs $script:loadedFields $company)
        $script:checked = @{}
        foreach ($job in $script:jobs) { $script:checked[$job.Key] = $job.IsActive }
        $script:credentials = Get-Credentials $script:loadedFields $company
        $script:pending.Clear()

        $active = @($script:jobs | Where-Object IsActive).Count
        Write-Log "Empresa $company`: $($script:jobs.Count) jobs, $active ativos." 'step'
        $status.Text = "$($script:jobs.Count) jobs de $($script:companies[$companyBox.SelectedIndex].Name)."
        Update-JobList
    }
    catch {
        Write-Log "Jobs: $($_.Exception.Message)" 'error'
    }
    finally {
        $form.Cursor = 'Default'
    }
}

$companyBox.Add_SelectedIndexChanged({
    if ($script:rebuilding -or $companyBox.SelectedIndex -lt 0) { return }

    if (Test-Unsaved) {
        $answer = [Windows.Forms.MessageBox]::Show('Há jobs ou credenciais alterados e não salvos. Trocar de empresa descarta as alterações. Continuar?', $form.Text, 'YesNo', 'Warning')
        if ($answer -ne 'Yes') { return }
    }

    Import-Jobs
})

# lê as empresas e escolhe a pedida (ou a primeira); com o banco sem empresa, oferece cadastrar uma
function Import-Companies([string]$select) {
    $form.Cursor = 'WaitCursor'

    try {
        $companies = @(Get-Companies $script:loadedFields)
        $fields = $script:loadedFields

        # a lista mostra o texto; a empresa de cada linha fica em $script:companies, no mesmo índice
        $script:rebuilding = $true
        $script:companies = $companies
        $companyBox.Items.Clear()
        foreach ($company in $companies) {
            $null = $companyBox.Items.Add("$($company.Name) - $($company.Company)$($company.IsActive ? '' : ' (inativa)')")
        }
        $script:rebuilding = $false

        Write-Log "Conectado em $($fields.Server),$($fields.Port), banco $($fields.Database): $($companies.Count) empresa(s)." 'ok'

        if ($companies) {
            $index = [Array]::IndexOf(@($companies.Company), $select)
            $companyBox.SelectedIndex = [Math]::Max($index, 0)
        }
        else {
            $script:jobs = @()
            $script:pending.Clear()
            Update-JobList
            Write-Log 'Nenhuma empresa em monitor.Company: cadastre em Nova empresa.' 'warn'
            $answer = [Windows.Forms.MessageBox]::Show('O banco não tem empresa. Cadastrar uma agora?', $form.Text, 'YesNo', 'Question')
            if ($answer -eq 'Yes') { Show-CompanyDialog }
        }
    }
    catch {
        $script:rebuilding = $false
        Write-Log "Falha ao carregar: $($_.Exception.Message)" 'error'
    }
    finally {
        $form.Cursor = 'Default'
    }
}

$loadButton.Add_Click({
    $fields = Get-Fields
    if (-not $fields) { return }

    $script:loadedFields = $fields
    Import-Companies
})

$saveButton.Add_Click({
    if (-not $script:loadedFields -or $companyBox.SelectedIndex -lt 0) {
        $null = [Windows.Forms.MessageBox]::Show('Carregue as empresas e escolha uma antes de salvar.', $form.Text, 'OK', 'Warning')
        return
    }

    $hostRadio = $hostRadios | Where-Object Checked
    if ($configBox.Checked -and -not $hostRadio) {
        $null = [Windows.Forms.MessageBox]::Show('Escolha o host de agenda.', $form.Text, 'OK', 'Warning')
        return
    }

    if ($configBox.Checked -and $hostRadio.Tag -eq 'WebApi' -and -not ($dashboardUserBox.Text.Trim() -and $dashboardPasswordBox.Text)) {
        $null = [Windows.Forms.MessageBox]::Show('Com o host WebApi, preencha o usuário e a senha do Hangfire.', $form.Text, 'OK', 'Warning')
        return
    }

    $fields = $script:loadedFields
    $company = $script:companies[$companyBox.SelectedIndex]
    $changes = Get-Changes
    $pending = @($script:pending)
    $on = @($changes | Where-Object { $script:checked[$_.Key] }).Count
    $activeTotal = @($script:jobs | Where-Object { $script:checked[$_.Key] }).Count

    if (-not $changes -and -not $pending -and -not $configBox.Checked) {
        $null = [Windows.Forms.MessageBox]::Show('Nada a salvar: nenhum job ou credencial alterado e a configuração desmarcada.', $form.Text, 'OK', 'Information')
        return
    }

    $text = "Banco $($fields.Database) de $($fields.Server),$($fields.Port), empresa $($company.Name) - $($company.Company).`n`n"
    if ($pending) { $text += "Credenciais: $(Get-PendingText).`n" }
    $text += $changes ? "Jobs: $on a ativar e $($changes.Count - $on) a desativar ($activeTotal ativos depois)." : 'Jobs: nenhuma alteração.'

    if ($configBox.Checked) {
        $text += "`n`nUser Secrets do WebApi, Worker, WebJob e WebApp: este banco e esta empresa, host $($schedulingHosts[$hostRadio.Tag].Title)."
        $text += $apisOffBox.Checked ? ' APIs externas desligadas.' : ''
    }

    # com as APIs de verdade, os jobs ativos chamam Totvs, Movidesk e as outras assim que um host subir
    if ($activeTotal -and -not ($configBox.Checked -and $apisOffBox.Checked)) {
        $text += "`n`nATENÇÃO: $activeTotal jobs ativos com as APIs de verdade. Quando um host subir, eles chamam as APIs externas."
    }

    $answer = [Windows.Forms.MessageBox]::Show("$text`n`nSalvar?", $form.Text, 'YesNo', 'Question')
    if ($answer -ne 'Yes') { return }

    $form.Cursor = 'WaitCursor'

    try {
        if ($changes -or $pending) {
            $updated = Save-Changes $fields $company.Company $pending $changes
            if ($pending) { Write-Log "Credenciais gravadas (só módulo e CNPJ): $(Get-PendingText)." 'ok' }
            if ($changes) { Write-Log "Jobs gravados em auditing.Jobs: $updated linha(s) ($on ativados, $($changes.Count - $on) desativados)." 'ok' }
            foreach ($job in $script:jobs) { $job.IsActive = $script:checked[$job.Key] }
            $script:pending.Clear()
            $script:credentials = Get-Credentials $fields $company.Company
            Update-JobList
        }

        if ($configBox.Checked) {
            $settings = New-ProjectSettings $fields $hostRadio.Tag $company.Company $apisOffBox.Checked $dashboardUserBox.Text.Trim() `
                $dashboardPasswordBox.Text $pepperBox.Text $adminBox.Checked $assistantBox.Text.Trim() $monitorBox.Text $queryBox.Text

            Write-Log 'User Secrets (só os nomes das chaves):' 'step'
            foreach ($project in $settings) {
                foreach ($line in (Set-ProjectSecrets $project)) { Write-Log "  $line" ($line -like '*init*' ? 'warn' : 'out') }
            }
        }

        Write-Log 'Salvo. Reinicie os hosts e o WebApp para lerem a configuração nova.' 'ok'
        $status.Text = 'Salvo.'
    }
    catch {
        Write-Log "Falha ao salvar: $($_.Exception.Message)" 'error'
        $status.Text = 'Falha ao salvar: confira o log.'
    }
    finally {
        $form.Cursor = 'Default'
    }
})

$closeButton.Add_Click({ $form.Close() })

$form.Add_FormClosing({
    param($sender, $e)

    if (-not (Test-Unsaved)) { return }

    $answer = [Windows.Forms.MessageBox]::Show('Há jobs ou credenciais alterados e não salvos. Fechar mesmo assim?', $form.Text, 'YesNo', 'Warning')
    if ($answer -ne 'Yes') { $e.Cancel = $true }
})

Update-ConfigFields
$null = $form.ShowDialog()
$form.Dispose()
