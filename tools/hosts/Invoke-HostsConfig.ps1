#Requires -Version 7
<#
.SYNOPSIS
    Configura, num banco já migrado, quais jobs de uma empresa ficam ativos e o que os hosts de agenda e o WebApp precisam
    para rodar contra ele, por uma tela Windows Forms.

.DESCRIPTION
    É a segunda parte do tools/migrators/Invoke-Migrators.ps1: aquele sobe as tabelas; este escolhe o que roda.

    1. Conecta no banco (servidor, porta, banco, usuário e senha) e lista as empresas de monitor.Company.
    2. Mostra os jobs da empresa em auditing.Jobs, agrupados por aplicação (a árvore de auditing.fn_JobCatalogTree, com as
       procedures debaixo da integração que as chama), com uma caixa por job: marcada é IsActive = 1.
    3. Escolhe o host de agenda (um só por banco): WebApi com o Hangfire, Worker com o Quartz.NET ou WebJob na Azure.
    4. Opcional: grava nos User Secrets do WebApi, do Worker, do WebJob e do WebApp o que depende desta máquina. São as
       connection strings deste banco, o host escolhido ligado e os outros desligados, usuário e senha do dashboard do
       Hangfire, as BaseUrl das APIs numa porta local fechada, o pepper do DataWarehouse e, no WebApp, Database:Enabled, a
       empresa logada, Admin:Enabled, a chave do assistente e os usuários etl_monitor e etl_query.

    Salvar grava os jobs alterados numa transação (só UPDATE de IsActive, na linha da empresa) e depois os User Secrets.
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

# ---------------------------------------------------------------------------------------------------------------------
# banco
# ---------------------------------------------------------------------------------------------------------------------

# roda um script no banco e devolve as linhas, colunas separadas por |; -b faz o sqlcmd sair com erro no primeiro erro de SQL
function Invoke-Sql($fields, [string]$query, [string]$database = $fields.Database) {
    $file = New-TemporaryFile

    try {
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
    }
}

function Get-Companies($fields) {
    Invoke-Sql $fields "SELECT Company, REPLACE([Name], '|', '/'), CAST(IsActive AS int) FROM [monitor].[Company] ORDER BY [Name];" |
        ForEach-Object {
            $columns = $_ -split '\|', 3
            [pscustomobject]@{ Company = $columns[0]; Name = $columns[1]; IsActive = $columns[2] -eq '1' }
        }
}

# os jobs da empresa com a aplicação de cada um: a integração está na árvore do módulo, e a procedure na do job que a chama
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
ORDER BY
    2, j.IdType DESC, j.IdJob;
"@ | ForEach-Object {
        $columns = $_ -split '\|', 10
        [pscustomobject]@{
            IdApp = [int]$columns[0]; AppTitle = $columns[1]; IdJob = [int]$columns[2]; IdType = [int]$columns[3]
            JobType = $columns[4]; IsActive = $columns[5] -eq '1'; Interval = [int]$columns[6]; DailyAt = $columns[7]
            Queue = $columns[8]; Title = $columns[9]; Key = "$($columns[2])|$($columns[3])"
        }
    }
}

# grava só os jobs que mudaram, numa transação, na linha da empresa; devolve as linhas alteradas
function Save-Jobs($fields, [string]$company, $changes) {
    if ($company -notmatch '^\d+$') { throw "CNPJ inválido: $company" }

    $values = ($changes | ForEach-Object { "($($_.IdJob), $($_.IdType), $([int]$_.IsActive))" }) -join ",`n    "

    Invoke-Sql $fields @"
SET XACT_ABORT ON;
BEGIN TRANSACTION;

UPDATE j
SET IsActive = v.IsActive
FROM
    [auditing].[Jobs] j
    JOIN (VALUES
    $values
    ) v (IdJob, IdType, IsActive) ON v.IdJob = j.IdJob AND v.IdType = j.IdType
WHERE
    j.Company = '$company';

SELECT @@ROWCOUNT;
COMMIT TRANSACTION;
"@ | Select-Object -First 1
}

# ---------------------------------------------------------------------------------------------------------------------
# User Secrets
# ---------------------------------------------------------------------------------------------------------------------

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
$companyBox = Add-Field $jobsGroup 'Empresa (monitor.Company)' 14 18 360 ([Windows.Forms.ComboBox]@{ DropDownStyle = 'DropDownList' })
$filterBox = Add-Field $jobsGroup 'Filtro (IdJob, título ou aplicação)' 386 18 250 ([Windows.Forms.TextBox]@{})
$changesLabel = [Windows.Forms.Label]@{ Text = ''; Location = [Drawing.Point]::new(648, 40); AutoSize = $true; ForeColor = [Drawing.Color]::DarkOrange }

$list = [Windows.Forms.ListView]@{
    View = 'Details'; CheckBoxes = $true; FullRowSelect = $true; ShowGroups = $true; HeaderStyle = 'Nonclickable'
    Location = [Drawing.Point]::new(14, 70); Size = [Drawing.Size]::new(808, 278); Anchor = 'Top, Bottom, Left, Right'
}
$null = $list.Columns.Add('IdJob', 70, 'Right')
$null = $list.Columns.Add('Tipo', 90)
$null = $list.Columns.Add('Título', 360)
$null = $list.Columns.Add('Agenda', 130)
$null = $list.Columns.Add('Fila', 120)

function New-Button([string]$text, [int]$x, [int]$width) {
    [Windows.Forms.Button]@{ Text = $text; Location = [Drawing.Point]::new($x, 356); Size = [Drawing.Size]::new($width, 28); Anchor = 'Bottom, Left' }
}
$checkAppButton = New-Button 'Ativar a aplicação' 14 140
$uncheckAppButton = New-Button 'Desativar a aplicação' 160 150
$checkAllButton = New-Button 'Ativar os da lista' 316 130
$uncheckAllButton = New-Button 'Desativar os da lista' 452 140
$jobsGroup.Controls.AddRange(@($changesLabel, $list, $checkAppButton, $uncheckAppButton, $checkAllButton, $uncheckAllButton))

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

function Format-Schedule($job) {
    if ($job.DailyAt) { return "diário às $($job.DailyAt)" }
    if ($job.Interval -eq 0) { return 'sob demanda' }
    if ($job.Interval % 60 -eq 0) { return "a cada $($job.Interval / 60) h" }
    "a cada $($job.Interval) min"
}

# cabeçalho do grupo com a contagem da aplicação inteira, não só do filtro
function Get-GroupHeader([int]$idApp) {
    $appJobs = @($script:jobs | Where-Object IdApp -eq $idApp)
    $active = @($appJobs | Where-Object { $script:checked[$_.Key] }).Count
    "$($appJobs[0].AppTitle) ($active de $($appJobs.Count) ativos)"
}

function Update-ChangesLabel {
    $changes = Get-Changes
    $on = @($changes | Where-Object { $script:checked[$_.Key] }).Count
    $changesLabel.Text = $changes ? "Alterações: $on a ativar, $($changes.Count - $on) a desativar" : ''
}

function Update-JobList {
    $filter = $filterBox.Text.Trim()
    $script:rebuilding = $true
    $list.BeginUpdate()

    try {
        $list.Items.Clear()
        $list.Groups.Clear()
        $groups = @{}

        foreach ($job in $script:jobs) {
            if ($filter -and -not ("$($job.IdJob)".Contains($filter) -or $job.Title -like "*$filter*" -or $job.AppTitle -like "*$filter*")) { continue }

            if (-not $groups.ContainsKey($job.IdApp)) {
                $groups[$job.IdApp] = [Windows.Forms.ListViewGroup]::new((Get-GroupHeader $job.IdApp))
                $groups[$job.IdApp].Tag = $job.IdApp
                $null = $list.Groups.Add($groups[$job.IdApp])
            }

            $item = [Windows.Forms.ListViewItem]::new([string[]]@("$($job.IdJob)", $job.JobType, $job.Title, (Format-Schedule $job), $job.Queue), $groups[$job.IdApp])
            $item.Tag = $job
            $item.Checked = [bool]$script:checked[$job.Key]
            $null = $list.Items.Add($item)
        }
    }
    finally {
        $list.EndUpdate()
        $script:rebuilding = $false
    }

    Update-ChangesLabel
}

$list.Add_ItemChecked({
    param($sender, $e)

    if ($script:rebuilding) { return }

    $job = $e.Item.Tag
    $script:checked[$job.Key] = $e.Item.Checked
    $e.Item.Group.Header = Get-GroupHeader $job.IdApp
    Update-ChangesLabel
})

function Set-Items($items, [bool]$value) {
    foreach ($item in @($items)) { $item.Checked = $value }
}

function Get-SelectedGroupItems {
    $group = @($list.SelectedItems)[0].Group
    if (-not $group) {
        $null = [Windows.Forms.MessageBox]::Show('Selecione um job da aplicação na lista.', $form.Text, 'OK', 'Information')
        return @()
    }

    @($list.Items | Where-Object { $_.Group -eq $group })
}

$checkAppButton.Add_Click({ Set-Items (Get-SelectedGroupItems) $true })
$uncheckAppButton.Add_Click({ Set-Items (Get-SelectedGroupItems) $false })
$checkAllButton.Add_Click({ Set-Items $list.Items $true })
$uncheckAllButton.Add_Click({ Set-Items $list.Items $false })
$filterBox.Add_TextChanged({ Update-JobList })

function Import-Jobs {
    $company = $script:companies[$companyBox.SelectedIndex].Company
    $form.Cursor = 'WaitCursor'

    try {
        $script:jobs = @(Get-Jobs $script:loadedFields $company)
        $script:checked = @{}
        foreach ($job in $script:jobs) { $script:checked[$job.Key] = $job.IsActive }

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

    if (Get-Changes) {
        $answer = [Windows.Forms.MessageBox]::Show('Há jobs alterados e não salvos. Trocar de empresa descarta as alterações. Continuar?', $form.Text, 'YesNo', 'Warning')
        if ($answer -ne 'Yes') { return }
    }

    Import-Jobs
})

$loadButton.Add_Click({
    $fields = Get-Fields
    if (-not $fields) { return }

    $form.Cursor = 'WaitCursor'

    try {
        $companies = @(Get-Companies $fields)
        $script:loadedFields = $fields

        # a lista mostra o texto; a empresa de cada linha fica em $script:companies, no mesmo índice
        $script:rebuilding = $true
        $script:companies = $companies
        $companyBox.Items.Clear()
        foreach ($company in $companies) {
            $null = $companyBox.Items.Add("$($company.Name) - $($company.Company)$($company.IsActive ? '' : ' (inativa)')")
        }
        $script:rebuilding = $false

        Write-Log "Conectado em $($fields.Server),$($fields.Port), banco $($fields.Database): $($companies.Count) empresa(s)." 'ok'

        if ($companies) { $companyBox.SelectedIndex = 0 }
        else { Write-Log 'Nenhuma empresa em monitor.Company: cadastre com o Import-TestSeeds.ps1 ou pela tela do WebApp.' 'warn' }
    }
    catch {
        $script:rebuilding = $false
        Write-Log "Falha ao carregar: $($_.Exception.Message)" 'error'
    }
    finally {
        $form.Cursor = 'Default'
    }
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
    $on = @($changes | Where-Object { $script:checked[$_.Key] }).Count
    $activeTotal = @($script:jobs | Where-Object { $script:checked[$_.Key] }).Count

    if (-not $changes -and -not $configBox.Checked) {
        $null = [Windows.Forms.MessageBox]::Show('Nada a salvar: nenhum job alterado e a configuração desmarcada.', $form.Text, 'OK', 'Information')
        return
    }

    $text = "Banco $($fields.Database) de $($fields.Server),$($fields.Port), empresa $($company.Name) - $($company.Company).`n`n"
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
        if ($changes) {
            $updated = Save-Jobs $fields $company.Company $changes
            Write-Log "Jobs gravados em auditing.Jobs: $updated linha(s) ($on ativados, $($changes.Count - $on) desativados)." 'ok'
            foreach ($job in $script:jobs) { $job.IsActive = $script:checked[$job.Key] }
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

    if (-not (Get-Changes)) { return }

    $answer = [Windows.Forms.MessageBox]::Show('Há jobs alterados e não salvos. Fechar mesmo assim?', $form.Text, 'YesNo', 'Warning')
    if ($answer -ne 'Yes') { $e.Cancel = $true }
})

Update-ConfigFields
$null = $form.ShowDialog()
$form.Dispose()
