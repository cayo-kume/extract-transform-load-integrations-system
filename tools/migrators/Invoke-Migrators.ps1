#Requires -Version 7
<#
.SYNOPSIS
    Sobe as tabelas das aplicações escolhidas num banco SQL Server, rodando o migrator.csx (DbUp) de cada uma, por uma tela
    Windows Forms com o servidor, a porta, o banco, o usuário e a senha (docs/MIGRATOR-CSX.md).

.DESCRIPTION
    A tela tem o catálogo das aplicações (as pastas Scripts/SQLServer com migrator.csx) e roda os marcados na ordem do
    docs/MIGRATOR-CSX.md: Shared (sempre), as integrações, o DataWarehouse e o host de agenda. Cada
    migrator roda com `dotnet script migrator.csx -- "<connection string>"` a partir de uma unidade subst, que encurta o
    caminho e evita o erro de MAX_PATH do SQL Client. No primeiro erro, os seguintes não rodam: eles dependem dos anteriores.

    O host de agenda é um só por banco (README, "Um host de agenda por vez"): o Worker (Quartz.NET) roda o próprio
    migrator, com as tabelas do schema quartz; o WebApi (Hangfire) cria o schema HangFire sozinho ao subir; o WebJob não tem
    tabelas. No fim, o log diz o que configurar em cada host para ele ser o único a agendar.

    Num banco de teste, a opção de seeds roda tools/seeds/Import-TestSeeds.ps1 depois dos migrators, para os módulos
    marcados: a empresa, os parâmetros (credenciais) e os jobs dela, inativos se não pedir para ativar. Vale a trava daquele
    script: o nome do banco precisa ter "Test" ou "Teste" e não pode ser "Company".

    O banco é criado pelo próprio DbUp se não existir. Os migrators não inserem dados de cliente: a empresa e os parâmetros
    entram depois, pela tela do WebApp ou, em teste, por tools/seeds/Import-TestSeeds.ps1.

    Usuários de banco do WebApp (opcional): com a senha do etl_monitor e/ou do etl_query, o migrator do Shared cria esses
    usuários no fim (dbo.sp_Create_Monitor_User e dbo.sp_Grant_Query_User, do Script009; src/Frontend/WebApp/README.md). As
    senhas vão só para o processo do Shared, pelas variáveis ETL_MONITOR_PASSWORD e ETL_QUERY_PASSWORD; em branco, o usuário
    não é criado agora. Rodar de novo num banco já migrado cria quem faltar e não troca a senha de quem já existe.

    A senha não aparece no log: os migrators imprimem a connection string e o script troca a senha por ****. As senhas dos
    usuários do WebApp também são mascaradas.

    Pré-requisito: dotnet tool install -g dotnet-script

.PARAMETER Server
    IP ou nome do servidor; sem ele, localhost.

.PARAMETER Port
    Porta do SQL Server; sem ela, 1433.

.PARAMETER Database
    Banco onde as tabelas sobem; sem ele, NEWBLOOMERS.

.PARAMETER Credential
    Usuário e senha do SQL Server. Na tela, preenchem os campos; com -NoGui, sem ele o script pede com Get-Credential.

.PARAMETER Apps
    Com -NoGui, só estas aplicações (o Shared entra sempre); sem ele, as marcadas por padrão na tela.

.PARAMETER SchedulingHost
    Host que agenda as integrações: WebApi (Hangfire), WebJob (Azure) ou Worker (Quartz.NET). Obrigatório com -NoGui; na
    tela, deixa o host já escolhido.

.PARAMETER TestSeeds
    Com -NoGui, carrega a empresa de teste com tools/seeds/Import-TestSeeds.ps1 depois dos migrators. Na tela, deixa a
    opção marcada.

.PARAMETER Company
    CNPJ da empresa de teste (só dígitos); sem ele, o da holding NEWBLOOMERS (44065634000106).

.PARAMETER CompanyName
    Razão social da empresa de teste; sem ela, NEWBLOOMERS.

.PARAMETER ActivateJobs
    Ativa os jobs da empresa de teste. Desligado por padrão: os jobs nascem inativos.

.PARAMETER MonitorPassword
    Com -NoGui, a senha do usuário etl_monitor (painel e cadastro do WebApp); sem ela, o usuário não é criado agora.

.PARAMETER QueryPassword
    Com -NoGui, a senha do usuário etl_query (tela Executar Querys do WebApp); sem ela, o usuário não é criado agora.

.PARAMETER NoGui
    Roda sem a tela, no console, com os parâmetros acima.

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1 -Server 10.0.0.15 -Port 1433 -Database NEWBLOOMERS

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -Apps Totvs,DataWarehouse -SchedulingHost Worker

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -SchedulingHost WebApi -TestSeeds

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -SchedulingHost Worker -MonitorPassword (Read-Host -AsSecureString) -QueryPassword (Read-Host -AsSecureString)
#>
[CmdletBinding()]
param(
    [string]$Server = 'localhost',
    [ValidateRange(1, 65535)]
    [int]$Port = 1433,
    [string]$Database = 'NEWBLOOMERS',
    [pscredential]$Credential,
    [ValidateSet('Totvs', 'LinxMicrovix', 'LinxCommerce', 'Movidesk', 'AfterSale', 'DataWarehouse')]
    [string[]]$Apps,
    [ValidateSet('WebApi', 'WebJob', 'Worker')]
    [string]$SchedulingHost,
    [switch]$TestSeeds,
    [ValidatePattern('^[0-9]+$')]
    [string]$Company = '44065634000106',
    [string]$CompanyName = 'NEWBLOOMERS',
    [switch]$ActivateJobs,
    [securestring]$MonitorPassword,
    [securestring]$QueryPassword,
    [switch]$NoGui
)

$ErrorActionPreference = 'Stop'

$backend = Join-Path $PSScriptRoot '../../src/Backend' | Resolve-Path | Select-Object -ExpandProperty Path

# ---------------------------------------------------------------------------------------------------------------------
# catálogo: a ordem de execução é a da lista (docs/MIGRATOR-CSX.md, "Ordem entre os módulos"); a pasta é a partir de
# src/Backend e aponta para Scripts/SQLServer, a que tem o migrator.csx. Os módulos fora do host (Dootax, FlashCourier...)
# ficam de fora: não fazem parte do sistema novo e o FlashCourier grava os mesmos ids (90 e 9000) do DataWarehouse
# ---------------------------------------------------------------------------------------------------------------------

$catalog = @(
    @{ Name = 'Shared'; Group = 'Base'; Default = $true; Required = $true; Folder = 'Shared/Infrastructure.Shared'
        Description = 'Schemas auditing, untreated e monitor, auditoria, a sp_Create_Merge_Procedure e as procedures dos usuários do WebApp (obrigatório)' }
    @{ Name = 'Totvs'; Group = 'Integrações'; Default = $true; Folder = 'Modules/Totvs/Infrastructure.Totvs'
        Description = 'Totvs Moda' }
    @{ Name = 'LinxMicrovix'; Group = 'Integrações'; Default = $true; Folder = 'Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/Infrastructure.LinxMicrovix.Outbound'
        Description = 'Linx Microvix (Outbound Web Service): ERP e B2C' }
    @{ Name = 'LinxCommerce'; Group = 'Integrações'; Default = $true; Folder = 'Modules/LinxCommerce/Infrastructure.LinxCommerce'
        Description = 'Linx Commerce' }
    @{ Name = 'Movidesk'; Group = 'Integrações'; Default = $true; Folder = 'Modules/General/Movidesk/Infrastructure.Movidesk'
        Description = 'Movidesk: tickets, pessoas e serviços' }
    @{ Name = 'AfterSale'; Group = 'Integrações'; Default = $true; Folder = 'Modules/General/AfterSale/Infrastructure.AfterSale'
        Description = 'AfterSale: trocas e devoluções' }
    @{ Name = 'DataWarehouse'; Group = 'Análise'; Default = $true; Folder = 'Modules/DataWarehouse/Infrastructure.DataWarehouse'
        Description = 'Roda depois das integrações: lê as tabelas delas' }
)

# hosts de agenda: um só agenda no banco. Só o Worker tem migrator (as tabelas do Quartz, por último, depois do Shared);
# Steps é o que acertar para ele ser o único (src/Backend/Hosts/<host>/README.md)
$schedulingHosts = [ordered]@{
    WebApi = @{
        Title = 'WebApi (Hangfire)'
        Description = 'Hangfire junto das rotas do WebApi. O schema HangFire é criado pelo próprio Hangfire quando o WebApi sobe.'
        Steps = @(
            'WebApi: Hangfire:Enabled = true (o schema HangFire é criado na primeira subida)'
            'Worker: serviço EtlWorker parado ou Quartz:Enabled = false; WebJobs parados no App Service'
        )
    }
    WebJob = @{
        Title = 'WebJob (Azure)'
        Description = 'Um WebJob contínuo por job no App Service. Não tem tabelas: a agenda é a auditing.Jobs.'
        Steps = @(
            'WebApi: Hangfire:Enabled = false (as rotas continuam para disparo manual)'
            'Worker: serviço EtlWorker parado ou Quartz:Enabled = false'
            'Publicar os WebJobs com src/Backend/Hosts/WebJob/Publish/publish-webjob.ps1'
        )
    }
    Worker = @{
        Title = 'Worker (Quartz.NET)'
        Description = 'Serviço do Windows EtlWorker com o Quartz.NET em cluster. Sobe as tabelas do Quartz (schema quartz).'
        App = @{ Name = 'Worker'; Folder = 'Hosts/Worker' }
        Steps = @(
            'WebApi: Hangfire:Enabled = false (as rotas continuam para disparo manual); WebJobs parados no App Service'
            'Publicar o serviço com src/Backend/Hosts/Worker/Publish/publish-worker.ps1'
        )
    }
}
foreach ($key in $schedulingHosts.Keys) { $schedulingHosts[$key].Key = $key }

foreach ($app in @($catalog) + @($schedulingHosts.Values | Where-Object App | ForEach-Object App)) {
    $app.Path = Join-Path $backend $app.Folder 'Scripts/SQLServer'
    $app.Scripts = @(Get-ChildItem -LiteralPath $app.Path -Filter '*.sql' -Recurse -File -ErrorAction SilentlyContinue).Count
}

# ---------------------------------------------------------------------------------------------------------------------
# execução: roda num runspace separado para a tela não travar; a saída vai para a fila e a tela (ou o console) a esvazia
# ---------------------------------------------------------------------------------------------------------------------

$worker = {
    param($apps, [string]$connectionString, [string[]]$secrets, $queue, $state, $schedulingHost, $seeds, $userPasswords)

    function Write-Line([string]$text, [string]$kind = 'out') {
        foreach ($secret in $secrets) {
            if ($secret) { $text = $text.Replace($secret, '****') }
        }

        $queue.Enqueue([pscustomobject]@{ Text = $text; Kind = $kind })
    }

    $state.Successful = $false
    $index = 0

    foreach ($app in $apps) {
        $index++

        if ($state.Cancel) {
            Write-Line 'Cancelado.' 'warn'
            return
        }

        Write-Line '' 'out'
        Write-Line "[$index/$($apps.Count)] $($app.Name) ($($app.Scripts) scripts)" 'step'

        # unidade livre para o subst, de Z: para trás
        $used = @([IO.DriveInfo]::GetDrives() | ForEach-Object { $_.Name.Substring(0, 1) })
        $letter = [char[]]'ZYXWVUTSRQPONM' | Where-Object { "$_" -notin $used } | Select-Object -First 1

        if (-not $letter) {
            Write-Line 'Nenhuma letra de unidade livre para o subst.' 'error'
            return
        }

        $drive = "${letter}:"
        $null = & subst.exe $drive $app.Path 2>&1

        if ($LASTEXITCODE -ne 0) {
            Write-Line "Não foi possível mapear $drive para $($app.Path)." 'error'
            return
        }

        try {
            $info = [Diagnostics.ProcessStartInfo]::new('dotnet')
            foreach ($argument in 'script', 'migrator.csx', '--', $connectionString) { $info.ArgumentList.Add($argument) }
            $info.WorkingDirectory = "$drive\"
            $info.UseShellExecute = $false
            $info.CreateNoWindow = $true
            $info.RedirectStandardOutput = $true
            $info.RedirectStandardError = $true
            $info.StandardOutputEncoding = [Text.Encoding]::UTF8
            $info.StandardErrorEncoding = [Text.Encoding]::UTF8

            # senhas dos usuários do WebApp: só o migrator do Shared as recebe (Script009); os outros nem as veem, mesmo que
            # estejam no ambiente de quem abriu a tela
            foreach ($variable in 'ETL_MONITOR_PASSWORD', 'ETL_QUERY_PASSWORD') {
                $value = $app.Name -eq 'Shared' ? $userPasswords[$variable] : $null
                if ($value) { $info.Environment[$variable] = $value } else { $null = $info.Environment.Remove($variable) }
            }

            $process = [Diagnostics.Process]::Start($info)
            $state.Process = $process
            $errors = $process.StandardError.ReadToEndAsync()

            while ($null -ne ($line = $process.StandardOutput.ReadLine())) {
                Write-Line $line ($line -eq 'Sucesso!' ? 'ok' : 'out')
            }

            $process.WaitForExit()
            $state.Process = $null

            # com sucesso, o stderr só tem avisos do dotnet-script (script sem cache)
            foreach ($line in ($errors.Result -split "`r?`n" | Where-Object { $_ })) {
                Write-Line $line ($process.ExitCode -eq 0 ? 'warn' : 'error')
            }

            if ($state.Cancel) {
                Write-Line 'Cancelado.' 'warn'
                return
            }

            if ($process.ExitCode -ne 0) {
                Write-Line "$($app.Name) falhou (código $($process.ExitCode)); as aplicações seguintes não rodaram." 'error'
                return
            }
        }
        catch {
            Write-Line "$($app.Name): $($_.Exception.Message)" 'error'
            return
        }
        finally {
            $null = & subst.exe $drive /D 2>&1
        }
    }

    Write-Line '' 'out'
    Write-Line "Migrators concluídos: $($apps.Count) aplicações aplicadas." 'ok'

    # a empresa de teste, no mesmo runspace: Write-Host chega como InformationRecord (amarelo vira aviso) e Write-Warning
    # como WarningRecord
    if ($seeds) {
        if ($state.Cancel) {
            Write-Line 'Cancelado.' 'warn'
            return
        }

        Write-Line '' 'out'
        Write-Line "Empresa de teste $($seeds.Parameters.Company): tools/seeds/Import-TestSeeds.ps1 ($($seeds.Parameters.Modules -join ', '))" 'step'

        try {
            $parameters = $seeds.Parameters.Clone()
            $parameters.Credential = [pscredential]::new($seeds.User, (ConvertTo-SecureString $seeds.Password -AsPlainText -Force))

            & $seeds.Path @parameters *>&1 | ForEach-Object {
                $kind = 'out'

                if ($_ -is [Management.Automation.ErrorRecord]) { $kind = 'error' }
                elseif ($_ -is [Management.Automation.WarningRecord]) { $kind = 'warn' }
                elseif ($_ -is [Management.Automation.InformationRecord] -and $_.MessageData.ForegroundColor -eq 'Yellow') { $kind = 'warn' }

                $text = $_ -is [Management.Automation.WarningRecord] ? "AVISO: $($_.Message)" : "$_"

                foreach ($line in ($text -split "`r?`n")) {
                    if ($line.Trim()) { Write-Line $line.TrimEnd() $kind }
                }
            }
        }
        catch {
            Write-Line "Import-TestSeeds: $($_.Exception.Message)" 'error'
            return
        }
    }

    $state.Successful = $true
    Write-Line '' 'out'
    Write-Line 'Concluído.' 'ok'
    Write-Line '' 'out'
    Write-Line "Host de agenda: $($schedulingHost.Title). Para ele ser o único a agendar:" 'step'

    foreach ($step in $schedulingHost.Steps) { Write-Line "  - $step" 'out' }
    Write-Line '' 'out'
    $builder = [Data.Common.DbConnectionStringBuilder]::new()
    $builder.set_ConnectionString($connectionString)  # com "=", o PowerShell grava uma chave "ConnectionString" no dicionário
    Write-Line "Jobs ativos e User Secrets dos projetos: ./tools/hosts/Invoke-HostsConfig.ps1 -Database $($builder['Database']) -SchedulingHost $($schedulingHost.Key)" 'step'
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

# a senha como aparece na connection string (entre aspas quando tem ; ou aspas) também é mascarada
function Get-Secrets([string]$password) {
    if (-not $password) { return @() }

    $builder = [Data.Common.DbConnectionStringBuilder]::new()
    $builder['Password'] = $password
    @($password, ($builder.ConnectionString -replace '^password=', '')) | Select-Object -Unique
}

# as senhas dos usuários do WebApp, pelo nome da variável que o migrator.csx do Shared lê; em branco fica de fora
function New-UserPasswords([string]$monitorPassword, [string]$queryPassword) {
    $passwords = @{}
    if ($monitorPassword) { $passwords['ETL_MONITOR_PASSWORD'] = $monitorPassword }
    if ($queryPassword) { $passwords['ETL_QUERY_PASSWORD'] = $queryPassword }
    $passwords
}

# o que o log e a confirmação dizem sobre os usuários do WebApp
function Get-UserPasswordsText($userPasswords) {
    $users = @(
        if ($userPasswords['ETL_MONITOR_PASSWORD']) { 'etl_monitor' }
        if ($userPasswords['ETL_QUERY_PASSWORD']) { 'etl_query' }
    )

    $users ? "Usuários do WebApp: $($users -join ' e ') (criados pelo Shared)" : 'Usuários do WebApp: nenhum (sem senha)'
}

function Start-Migration($apps, [string]$connectionString, [string[]]$secrets, $schedulingHost, $seeds, $userPasswords) {
    $queue = [Collections.Concurrent.ConcurrentQueue[object]]::new()
    $state = [hashtable]::Synchronized(@{ Cancel = $false; Process = $null; Successful = $false })

    $powershell = [powershell]::Create()
    $null = $powershell.AddScript($worker).AddArgument($apps).AddArgument($connectionString).AddArgument($secrets).
        AddArgument($queue).AddArgument($state).AddArgument($schedulingHost).AddArgument($seeds).AddArgument($userPasswords)

    [pscustomobject]@{ PowerShell = $powershell; Handle = $powershell.BeginInvoke(); Queue = $queue; State = $state }
}

function Test-DotnetScript {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { return $false }

    $null = & dotnet script --version 2>&1
    $LASTEXITCODE -eq 0
}

# os seeds: só os módulos marcados que o Import-TestSeeds conhece (o Shared e o Worker não têm parâmetros nem jobs de
# empresa); a trava do nome do banco é a dele, conferida antes para os migrators não rodarem à toa
$seedsScript = Join-Path $PSScriptRoot '../seeds/Import-TestSeeds.ps1' | Resolve-Path | Select-Object -ExpandProperty Path
$seedsModules = 'Totvs', 'LinxMicrovix', 'LinxCommerce', 'Movidesk', 'AfterSale', 'DataWarehouse'

function Get-SeedsProblem([string]$database, [string]$company, $apps) {
    if ($database -eq 'Company' -or $database -notmatch 'Test') {
        return "O banco '$database' não parece de teste: para carregar a empresa de teste, o nome precisa ter 'Test' ou 'Teste' e não pode ser 'Company'."
    }

    if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) { return 'O Import-TestSeeds precisa do sqlcmd no PATH.' }

    if ($company -notmatch '^[0-9]+$') { return 'O CNPJ da empresa de teste precisa ter só dígitos.' }

    if (-not ($apps | Where-Object Name -in $seedsModules)) { return 'Marque ao menos um módulo para carregar a empresa de teste.' }
}

function New-Seeds([string]$server, [int]$port, [string]$database, [string]$user, [string]$password, [string]$company,
    [string]$companyName, [bool]$activateJobs, $apps) {
    @{
        Path = $seedsScript; User = $user; Password = $password
        Parameters = @{
            Database = $database; Server = "$server,$port"; Company = $company; CompanyName = $companyName
            Modules = @($apps | Where-Object Name -in $seedsModules | ForEach-Object Name); ActivateJobs = $activateJobs
        }
    }
}

$missingTool = 'O dotnet-script não está instalado. Instale com: dotnet tool install -g dotnet-script'

# ---------------------------------------------------------------------------------------------------------------------
# sem tela
# ---------------------------------------------------------------------------------------------------------------------

if ($NoGui) {
    if (-not $SchedulingHost) { throw 'Informe o host de agenda com -SchedulingHost (WebApi, WebJob ou Worker).' }

    if (-not (Test-DotnetScript)) { throw $missingTool }

    $schedulingHostInfo = $schedulingHosts[$SchedulingHost]
    $selected = @($catalog | Where-Object { $_.Required -or ($Apps ? $_.Name -in $Apps : $_.Default) }) +
        @($schedulingHostInfo.App | Where-Object { $_ })

    if ($TestSeeds -and ($problem = Get-SeedsProblem $Database $Company $selected)) { throw $problem }

    if (-not $Credential) { $Credential = Get-Credential -Message "SQL Server $Server,$Port" }
    $password = $Credential.GetNetworkCredential().Password
    $connectionString = New-ConnectionString $Server $Port $Database $Credential.UserName $password $true

    $userPasswords = New-UserPasswords ($MonitorPassword ? [Net.NetworkCredential]::new('', $MonitorPassword).Password : '') `
        ($QueryPassword ? [Net.NetworkCredential]::new('', $QueryPassword).Password : '')

    Write-Host "Servidor $Server,$Port, banco ${Database}, host de agenda $($schedulingHostInfo.Title): $(($selected | ForEach-Object Name) -join ', ')"
    Write-Host (Get-UserPasswordsText $userPasswords)

    $seeds = $TestSeeds ? (New-Seeds $Server $Port $Database $Credential.UserName $password $Company $CompanyName $ActivateJobs $selected) : $null
    $secrets = @(Get-Secrets $password) + @($userPasswords.Values | ForEach-Object { Get-Secrets $_ })
    $run = Start-Migration $selected $connectionString $secrets $schedulingHostInfo $seeds $userPasswords
    $colors = @{ out = 'Gray'; step = 'Cyan'; ok = 'Green'; error = 'Red'; warn = 'Yellow' }

    while (-not $run.Handle.IsCompleted -or -not $run.Queue.IsEmpty) {
        $item = $null

        while ($run.Queue.TryDequeue([ref]$item)) { Write-Host $item.Text -ForegroundColor $colors[$item.Kind] }

        Start-Sleep -Milliseconds 100
    }

    $run.PowerShell.EndInvoke($run.Handle)
    $run.PowerShell.Dispose()

    if (-not $run.State.Successful) { exit 1 }

    return
}

# ---------------------------------------------------------------------------------------------------------------------
# tela
# ---------------------------------------------------------------------------------------------------------------------

if ([Threading.Thread]::CurrentThread.ApartmentState -ne 'STA') {
    $arguments = @('-NoProfile', '-STA', '-File', $PSCommandPath, '-Server', $Server, '-Port', $Port, '-Database', $Database)
    if ($SchedulingHost) { $arguments += '-SchedulingHost', $SchedulingHost }
    if ($TestSeeds) { $arguments += '-TestSeeds' }
    if ($ActivateJobs) { $arguments += '-ActivateJobs' }
    $arguments += '-Company', $Company, '-CompanyName', $CompanyName
    & (Get-Process -Id $PID).Path @arguments
    return
}

Add-Type -AssemblyName System.Windows.Forms, System.Drawing
[Windows.Forms.Application]::EnableVisualStyles()

$font = [Drawing.Font]::new('Segoe UI', 9)

$form = [Windows.Forms.Form]@{
    Text = 'Migrators: subir tabelas no SQL Server'
    ClientSize = [Drawing.Size]::new(860, 972)
    MinimumSize = [Drawing.Size]::new(760, 852)
    StartPosition = 'CenterScreen'
    Font = $font
}

# conexão

$connection = [Windows.Forms.GroupBox]@{
    Text = 'Banco de dados'; Location = [Drawing.Point]::new(12, 10); Size = [Drawing.Size]::new(836, 112); Anchor = 'Top, Left, Right'
}

function Add-Field([string]$label, [int]$x, [int]$y, [int]$width, $control) {
    $connection.Controls.Add([Windows.Forms.Label]@{ Text = $label; Location = [Drawing.Point]::new($x, $y); AutoSize = $true })
    $control.Location = [Drawing.Point]::new($x, $y + 18)
    $control.Width = $width
    $connection.Controls.Add($control)
    $control
}

$serverBox = Add-Field 'Servidor (IP)' 14 22 200 ([Windows.Forms.TextBox]@{ Text = $Server })
# o Value só depois do Maximum: as chaves do @{} não têm ordem garantida
$portBox = Add-Field 'Porta' 226 22 80 ([Windows.Forms.NumericUpDown]@{ Minimum = 1; Maximum = 65535 })
$portBox.Value = $Port
$databaseBox = Add-Field 'Banco' 318 22 170 ([Windows.Forms.TextBox]@{ Text = $Database })
$userBox = Add-Field 'Usuário' 500 22 150 ([Windows.Forms.TextBox]@{ Text = $Credential ? $Credential.UserName : 'sa' })
$passwordBox = Add-Field 'Senha' 662 22 160 ([Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true })

if ($Credential) { $passwordBox.Text = $Credential.GetNetworkCredential().Password }

$trustBox = [Windows.Forms.CheckBox]@{
    Text = 'Confiar no certificado do servidor (TrustServerCertificate)'; Checked = $true
    Location = [Drawing.Point]::new(14, 78); AutoSize = $true
}
$testButton = [Windows.Forms.Button]@{ Text = 'Testar conexão'; Location = [Drawing.Point]::new(702, 74); Size = [Drawing.Size]::new(120, 28) }
$connection.Controls.AddRange(@($trustBox, $testButton))

# host de agenda: um só por banco; o radio guarda a chave de $schedulingHosts na Tag

$hostGroup = [Windows.Forms.GroupBox]@{
    Text = 'Host de agenda das integrações (um só por banco)'; Location = [Drawing.Point]::new(12, 130)
    Size = [Drawing.Size]::new(836, 96); Anchor = 'Top, Left, Right'
}

$hostDescription = [Windows.Forms.Label]@{
    Text = 'Escolha o host que vai agendar os jobs.'; Location = [Drawing.Point]::new(14, 56)
    Size = [Drawing.Size]::new(808, 32); ForeColor = [Drawing.SystemColors]::GrayText
}
$hostGroup.Controls.Add($hostDescription)

$hostRadios = @()
$x = 14

# sem AutoCheck: o WinForms marca o rádio que recebe o foco (o primeiro, ao voltar de um MessageBox), e o host trocaria
# sem o usuário clicar; aqui só o clique (ou a barra de espaço) marca
foreach ($key in $schedulingHosts.Keys) {
    $radio = [Windows.Forms.RadioButton]@{
        Text = $schedulingHosts[$key].Title; Tag = $key; Location = [Drawing.Point]::new($x, 26); AutoSize = $true; AutoCheck = $false
    }
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

# usuários de banco do WebApp: as senhas vão só para o migrator do Shared, que cria etl_monitor e etl_query (Script009)

$usersGroup = [Windows.Forms.GroupBox]@{
    Text = 'Usuários de banco do WebApp (opcional)'; Location = [Drawing.Point]::new(12, 232); Size = [Drawing.Size]::new(836, 84)
    Anchor = 'Top, Left, Right'
}

$monitorLabel = [Windows.Forms.Label]@{ Text = 'Senha do etl_monitor (painel e cadastro)'; Location = [Drawing.Point]::new(14, 22); AutoSize = $true }
$monitorBox = [Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true; Location = [Drawing.Point]::new(14, 40); Width = 240 }
$queryLabel = [Windows.Forms.Label]@{ Text = 'Senha do etl_query (Executar Querys)'; Location = [Drawing.Point]::new(270, 22); AutoSize = $true }
$queryBox = [Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true; Location = [Drawing.Point]::new(270, 40); Width = 240 }
$usersHint = [Windows.Forms.Label]@{
    Text = 'Em branco, o usuário não é criado agora. Num banco já migrado, cria quem faltar; não troca a senha de quem já existe.'
    Location = [Drawing.Point]::new(526, 22); Size = [Drawing.Size]::new(296, 54); ForeColor = [Drawing.SystemColors]::GrayText
}

if ($MonitorPassword) { $monitorBox.Text = [Net.NetworkCredential]::new('', $MonitorPassword).Password }
if ($QueryPassword) { $queryBox.Text = [Net.NetworkCredential]::new('', $QueryPassword).Password }

$usersGroup.Controls.AddRange(@($monitorLabel, $monitorBox, $queryLabel, $queryBox, $usersHint))

# banco de teste: a empresa, as credenciais e os jobs pelo tools/seeds/Import-TestSeeds.ps1, depois dos migrators

$testGroup = [Windows.Forms.GroupBox]@{
    Text = 'Banco de teste'; Location = [Drawing.Point]::new(12, 324); Size = [Drawing.Size]::new(836, 84); Anchor = 'Top, Left, Right'
}

$seedsBox = [Windows.Forms.CheckBox]@{
    Text = 'Carregar a empresa de teste com as credenciais (tools/seeds/Import-TestSeeds.ps1), depois dos migrators'
    Location = [Drawing.Point]::new(14, 22); AutoSize = $true; Checked = [bool]$TestSeeds
}
$companyLabel = [Windows.Forms.Label]@{ Text = 'CNPJ'; Location = [Drawing.Point]::new(30, 53); AutoSize = $true }
$companyBox = [Windows.Forms.TextBox]@{ Text = $Company; Location = [Drawing.Point]::new(70, 50); Width = 130 }
$companyNameLabel = [Windows.Forms.Label]@{ Text = 'Razão social'; Location = [Drawing.Point]::new(216, 53); AutoSize = $true }
$companyNameBox = [Windows.Forms.TextBox]@{ Text = $CompanyName; Location = [Drawing.Point]::new(296, 50); Width = 200 }
$activateBox = [Windows.Forms.CheckBox]@{
    Text = 'Ativar os jobs (chamam as APIs reais)'; Location = [Drawing.Point]::new(516, 52); AutoSize = $true; Checked = [bool]$ActivateJobs
}
$seedsFields = @($companyLabel, $companyBox, $companyNameLabel, $companyNameBox, $activateBox)
$testGroup.Controls.AddRange(@($seedsBox) + $seedsFields)

function Update-SeedsFields { foreach ($control in $seedsFields) { $control.Enabled = $seedsBox.Checked } }
$seedsBox.Add_CheckedChanged({ Update-SeedsFields })
Update-SeedsFields

# catálogo

$catalogLabel = [Windows.Forms.Label]@{
    Text = 'Catálogo das aplicações (rodam na ordem da lista; o Shared é obrigatório)'
    Location = [Drawing.Point]::new(12, 418); AutoSize = $true
}

$list = [Windows.Forms.ListView]@{
    View = 'Details'; CheckBoxes = $true; FullRowSelect = $true; ShowGroups = $true; HeaderStyle = 'Nonclickable'
    Location = [Drawing.Point]::new(12, 438); Size = [Drawing.Size]::new(836, 246); Anchor = 'Top, Left, Right'
}
$null = $list.Columns.Add('Aplicação', 150)
$null = $list.Columns.Add('Scripts', 60, 'Right')
$null = $list.Columns.Add('Descrição', 600)

$groups = @{}

foreach ($app in $catalog) {
    if (-not $groups.ContainsKey($app.Group)) {
        $groups[$app.Group] = [Windows.Forms.ListViewGroup]::new($app.Group)
        $null = $list.Groups.Add($groups[$app.Group])
    }

    $item = [Windows.Forms.ListViewItem]::new([string[]]@($app.Name, $app.Scripts, $app.Description), $groups[$app.Group])
    $item.Tag = $app
    $item.Checked = [bool]$app.Default

    if ($app.Required) { $item.ForeColor = [Drawing.SystemColors]::GrayText }

    $null = $list.Items.Add($item)
}

$list.Add_ItemCheck({
    param($sender, $e)

    if ($sender.Items[$e.Index].Tag.Required) { $e.NewValue = 'Checked' }
})

$checkAllButton = [Windows.Forms.Button]@{ Text = 'Marcar todas'; Location = [Drawing.Point]::new(12, 692); Size = [Drawing.Size]::new(110, 28) }
$checkNoneButton = [Windows.Forms.Button]@{ Text = 'Desmarcar todas'; Location = [Drawing.Point]::new(128, 692); Size = [Drawing.Size]::new(120, 28) }
$runButton = [Windows.Forms.Button]@{
    Text = 'Subir tabelas'; Location = [Drawing.Point]::new(616, 692); Size = [Drawing.Size]::new(120, 28); Anchor = 'Top, Right'
}
$cancelButton = [Windows.Forms.Button]@{
    Text = 'Cancelar'; Location = [Drawing.Point]::new(742, 692); Size = [Drawing.Size]::new(106, 28); Anchor = 'Top, Right'; Enabled = $false
}

# log

$log = [Windows.Forms.RichTextBox]@{
    Location = [Drawing.Point]::new(12, 730); Size = [Drawing.Size]::new(836, 208); Anchor = 'Top, Bottom, Left, Right'
    ReadOnly = $true; BackColor = [Drawing.Color]::FromArgb(30, 30, 30); ForeColor = [Drawing.Color]::Gainsboro
    Font = [Drawing.Font]::new('Consolas', 9); WordWrap = $false
}

$status = [Windows.Forms.Label]@{
    Text = 'Pronto.'; Location = [Drawing.Point]::new(12, 946); AutoSize = $true; Anchor = 'Bottom, Left'
}

$form.Controls.AddRange(@($connection, $hostGroup, $usersGroup, $testGroup, $catalogLabel, $list, $checkAllButton, $checkNoneButton, $runButton, $cancelButton, $log, $status))

$logColors = @{
    out = [Drawing.Color]::Gainsboro; step = [Drawing.Color]::DeepSkyBlue; ok = [Drawing.Color]::LightGreen
    error = [Drawing.Color]::Tomato; warn = [Drawing.Color]::Gold
}

function Write-Log([string]$text, [string]$kind = 'out') {
    $log.SelectionStart = $log.TextLength
    $log.SelectionLength = 0
    $log.SelectionColor = $logColors[$kind]
    $log.AppendText("$text`n")
    $log.ScrollToCaret()
}

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

function Set-Running([bool]$running) {
    foreach ($control in $connection, $hostGroup, $usersGroup, $testGroup, $list, $checkAllButton, $checkNoneButton, $runButton) { $control.Enabled = -not $running }
    $cancelButton.Enabled = $running
}

$checkAllButton.Add_Click({ foreach ($item in $list.Items) { $item.Checked = $true } })
$checkNoneButton.Add_Click({ foreach ($item in $list.Items) { $item.Checked = $false } })

# a porta aberta no servidor e, com o sqlcmd no PATH, o login (no master, sem depender do banco já existir)
$testButton.Add_Click({
    $fields = Get-Fields
    if (-not $fields) { return }

    $form.Cursor = 'WaitCursor'

    try {
        $client = [Net.Sockets.TcpClient]::new()

        try {
            $reached = $client.ConnectAsync($fields.Server, $fields.Port).Wait(5000) -and $client.Connected
        }
        catch {
            $reached = $false
        }
        finally {
            $client.Dispose()
        }

        if (-not $reached) {
            Write-Log "Sem resposta em $($fields.Server),$($fields.Port)." 'error'
            return
        }

        if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
            Write-Log "Porta $($fields.Port) aberta em $($fields.Server) (sem sqlcmd no PATH, o login não foi testado)." 'warn'
            return
        }

        $arguments = @('-S', "$($fields.Server),$($fields.Port)", '-U', $fields.User, '-d', 'master', '-l', '10', '-b', '-h', '-1',
            '-Q', 'SET NOCOUNT ON; SELECT @@VERSION')
        if ($fields.Trust) { $arguments += '-C' }

        $env:SQLCMDPASSWORD = $fields.Password

        try {
            $output = & sqlcmd @arguments 2>&1
        }
        finally {
            Remove-Item Env:SQLCMDPASSWORD -ErrorAction SilentlyContinue
        }

        if ($LASTEXITCODE -eq 0) {
            Write-Log "Conectado em $($fields.Server),$($fields.Port) como $($fields.User): $((@($output) | Select-Object -First 1).ToString().Trim())" 'ok'
        }
        else {
            Write-Log "Falha no login em $($fields.Server),$($fields.Port): $(($output | Where-Object { "$_".Trim() }) -join ' ')" 'error'
        }
    }
    finally {
        $form.Cursor = 'Default'
    }
})

$script:run = $null

$timer = [Windows.Forms.Timer]@{ Interval = 150 }
$timer.Add_Tick({
    if (-not $script:run) { return }

    $item = $null
    while ($script:run.Queue.TryDequeue([ref]$item)) { Write-Log $item.Text $item.Kind }

    if (-not $script:run.Handle.IsCompleted) { return }

    while ($script:run.Queue.TryDequeue([ref]$item)) { Write-Log $item.Text $item.Kind }

    try {
        $script:run.PowerShell.EndInvoke($script:run.Handle)
    }
    catch {
        Write-Log $_.Exception.Message 'error'
    }

    $status.Text = $script:run.State.Successful ? 'Concluído.' : 'Parou com erro ou cancelado: confira o log.'
    $script:run.PowerShell.Dispose()
    $script:run = $null
    $timer.Stop()
    Set-Running $false
})

$runButton.Add_Click({
    $fields = Get-Fields
    if (-not $fields) { return }

    $hostRadio = $hostRadios | Where-Object Checked

    if (-not $hostRadio) {
        $null = [Windows.Forms.MessageBox]::Show('Escolha o host de agenda das integrações.', $form.Text, 'OK', 'Warning')
        return
    }

    $schedulingHost = $schedulingHosts[$hostRadio.Tag]
    $selected = @($list.Items | Where-Object Checked | ForEach-Object Tag) + @($schedulingHost.App | Where-Object { $_ })
    $names = ($selected | ForEach-Object Name) -join ', '
    $company = $companyBox.Text.Trim()
    $seedsText = ''

    if ($seedsBox.Checked) {
        $problem = Get-SeedsProblem $fields.Database $company $selected

        if ($problem) {
            $null = [Windows.Forms.MessageBox]::Show($problem, $form.Text, 'OK', 'Warning')
            return
        }

        $seedsText = "`n`nDepois, a empresa de teste $company com as credenciais (Import-TestSeeds), jobs " +
            ($activateBox.Checked ? 'ATIVOS.' : 'inativos.')
    }

    $userPasswords = New-UserPasswords $monitorBox.Text $queryBox.Text
    $usersText = Get-UserPasswordsText $userPasswords

    $answer = [Windows.Forms.MessageBox]::Show(
        ("Subir as tabelas no banco $($fields.Database) de $($fields.Server),$($fields.Port)?`n`n$names`n`n" +
            "Host de agenda: $($schedulingHost.Title)`n`n$usersText$seedsText`n`nO banco é criado se não existir."),
        $form.Text, 'YesNo', 'Question')

    if ($answer -ne 'Yes') { return }

    $form.Cursor = 'WaitCursor'
    $hasTool = Test-DotnetScript
    $form.Cursor = 'Default'

    if (-not $hasTool) {
        $null = [Windows.Forms.MessageBox]::Show($missingTool, $form.Text, 'OK', 'Error')
        return
    }

    $connectionString = New-ConnectionString $fields.Server $fields.Port $fields.Database $fields.User $fields.Password $fields.Trust

    $log.Clear()
    Write-Log "Servidor $($fields.Server),$($fields.Port), banco $($fields.Database), host de agenda $($schedulingHost.Title): $names" 'step'
    Write-Log $usersText 'step'
    $status.Text = 'Rodando...'
    Set-Running $true

    $seeds = $seedsBox.Checked ?
        (New-Seeds $fields.Server $fields.Port $fields.Database $fields.User $fields.Password $company $companyNameBox.Text.Trim() $activateBox.Checked $selected) :
        $null

    $secrets = @(Get-Secrets $fields.Password) + @($userPasswords.Values | ForEach-Object { Get-Secrets $_ })
    $script:run = Start-Migration $selected $connectionString $secrets $schedulingHost $seeds $userPasswords
    $timer.Start()
})

$cancelButton.Add_Click({
    if (-not $script:run) { return }

    $script:run.State.Cancel = $true
    $process = $script:run.State.Process

    # sem migrator rodando, o que roda é o Import-TestSeeds, no próprio runspace
    if ($process -and -not $process.HasExited) { $process.Kill($true) }
    else { $null = $script:run.PowerShell.BeginStop($null, $null) }

    $status.Text = 'Cancelando...'
})

$form.Add_FormClosing({
    param($sender, $e)

    if (-not $script:run) { return }

    $answer = [Windows.Forms.MessageBox]::Show('Os migrators ainda estão rodando. Cancelar e fechar?', $form.Text, 'YesNo', 'Warning')

    if ($answer -ne 'Yes') {
        $e.Cancel = $true
        return
    }

    $script:run.State.Cancel = $true
    $process = $script:run.State.Process
    if ($process -and -not $process.HasExited) { $process.Kill($true) }
})

$null = $form.ShowDialog()
$timer.Dispose()
$form.Dispose()
