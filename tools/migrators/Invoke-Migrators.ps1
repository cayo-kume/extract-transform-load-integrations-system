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

    O banco é criado pelo próprio DbUp se não existir. Os migrators só criam a estrutura e o catálogo do produto: as
    holdings, as empresas, as credenciais dos módulos e os jobs entram depois, pelo WebApp (tela inicial, "Cadastrar holding
    e empresas", e Cadastro › Módulos e Credenciais; docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md) ou, num banco de
    teste, pelo tools/seeds/Import-TestSeeds.ps1.

    Usuários de banco do WebApp (opcional): com a senha do etl_monitor e/ou do etl_query, o migrator do Shared cria esses
    usuários no fim (dbo.sp_Create_Monitor_User e dbo.sp_Grant_Query_User, do Script009; src/Frontend/WebApp/README.md). As
    senhas vão só para o processo do Shared, pelas variáveis ETL_MONITOR_PASSWORD e ETL_QUERY_PASSWORD; em branco, o usuário
    não é criado agora. Rodar de novo cria quem faltar e troca a senha de quem já existe para a informada.

    Connection strings dos projetos: no fim, com os migrators concluídos, o script grava a seção ConnectionStrings do
    appsettings.json do WebApp, do WebApi, do WebJob e do Worker com este servidor e este banco (o arquivo é editado no
    lugar, com os comentários). WebApi, WebJob e Worker: Connection com o usuário e a senha da tela. WebApp: Connection com
    o etl_monitor e QueryConnection com o etl_query, com as senhas da seção Usuários de banco do WebApp (em branco, o
    usuário e a senha da tela). As mesmas chaves saem dos User Secrets dos quatro projetos, que passariam por cima do
    appsettings. A senha fica no arquivo e vai para o git, de propósito: é a configuração de teste. -SkipAppSettings (ou a
    caixa da tela desmarcada) não grava nada.

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

.PARAMETER Services
    Com -NoGui, só estes services, no formato Aplicação/service (ex.: Totvs/People, LinxMicrovix/Erp/Customers); a
    aplicação sem nenhum na lista sobe todos os services dela. Parameters sobe sempre.

.PARAMETER SchedulingHost
    Host que agenda as integrações: WebApi (Hangfire), WebJob (Azure) ou Worker (Quartz.NET). Obrigatório com -NoGui; na
    tela, deixa o host já escolhido.

.PARAMETER MonitorPassword
    Com -NoGui, a senha do usuário etl_monitor (painel e cadastro do WebApp); sem ela, o usuário não é criado agora.

.PARAMETER QueryPassword
    Com -NoGui, a senha do usuário etl_query (tela Executar Querys do WebApp); sem ela, o usuário não é criado agora.

.PARAMETER HangfireUser
    Com -NoGui, o usuário do dashboard do Hangfire (Hangfire:Dashboard:UserName no appsettings.json do WebApi); sem ele, o
    usuário e a senha que estão lá ficam como estão.

.PARAMETER HangfirePassword
    Com -NoGui, a senha do dashboard do Hangfire (Hangfire:Dashboard:Password), junto com -HangfireUser.

.PARAMETER SkipAppSettings
    Não grava as connection strings nos appsettings.json do WebApp, do WebApi, do WebJob e do Worker (por exemplo, migrando
    um banco da Azure sem trocar o banco de teste da máquina).

.PARAMETER NoGui
    Roda sem a tela, no console, com os parâmetros acima.

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1 -Server 10.0.0.15 -Port 1433 -Database NEWBLOOMERS

.EXAMPLE
    ./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -Apps Totvs,DataWarehouse -SchedulingHost Worker

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
    [string[]]$Services,
    [ValidateSet('WebApi', 'WebJob', 'Worker')]
    [string]$SchedulingHost,
    [securestring]$MonitorPassword,
    [securestring]$QueryPassword,
    [string]$HangfireUser,
    [securestring]$HangfirePassword,
    [switch]$SkipAppSettings,
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

    # services: as pastas com scripts abaixo de Scripts/SQLServer (ex.: People, Erp/Customers), menos Parameters, que sobe
    # sempre, e a raiz. A tela e -Services escolhem quais sobem; o migrator.csx do módulo filtra pela ETL_MIGRATOR_SERVICES
    $app.Services = @(Get-ChildItem -LiteralPath $app.Path -Filter '*.sql' -Recurse -File -ErrorAction SilentlyContinue |
        ForEach-Object { [IO.Path]::GetRelativePath($app.Path, $_.DirectoryName).Replace('\', '/') } |
        Where-Object { $_ -ne '.' -and $_ -ne 'Parameters' } |
        Group-Object | Sort-Object Name |
        ForEach-Object { [pscustomobject]@{ Name = $_.Name; Scripts = $_.Count } })
}

# ---------------------------------------------------------------------------------------------------------------------
# execução: roda num runspace separado para a tela não travar; a saída vai para a fila e a tela (ou o console) a esvazia
# ---------------------------------------------------------------------------------------------------------------------

$worker = {
    param($apps, [string]$connectionString, [string[]]$secrets, $queue, $state, $schedulingHost, $userPasswords, $appSettings)

    function Write-Line([string]$text, [string]$kind = 'out') {
        foreach ($secret in $secrets) {
            if ($secret) { $text = $text.Replace($secret, '****') }
        }

        $queue.Enqueue([pscustomobject]@{ Text = $text; Kind = $kind })
    }

    # o appsettings.json editado no texto do arquivo, para os comentários, a ordem, o BOM e as quebras de linha ficarem
    # como estão: a seção ConnectionStrings trocada inteira (sem ela, entra no começo do arquivo) e cada valor de Settings
    # ("Hangfire:Dashboard:UserName") trocado no lugar
    function Update-AppSettings([string]$path, $connectionStrings, $settings) {
        $bytes = [IO.File]::ReadAllBytes($path)
        $bom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        $text = [Text.UTF8Encoding]::new($false).GetString($bytes, ($bom ? 3 : 0), $bytes.Length - ($bom ? 3 : 0))
        $newline = $text.Contains("`r`n") ? "`r`n" : "`n"

        $lines = @('  "ConnectionStrings": {', '    // gravada pelo tools/migrators/Invoke-Migrators.ps1 (servidor, banco, usuário e senha da tela)')
        $keys = @($connectionStrings.Keys)
        for ($i = 0; $i -lt $keys.Count; $i++) {
            $escaped = $connectionStrings[$keys[$i]].Replace('\', '\\').Replace('"', '\"')
            $lines += "    `"$($keys[$i])`": `"$escaped`"$($i -lt $keys.Count - 1 ? ',' : '')"
        }
        $lines += '  },'
        $section = ($lines -join $newline) + $newline

        $pattern = [regex]::new('(?ms)^  "ConnectionStrings"\s*:\s*\{.*?^  \},?\r?\n')
        if ($pattern.IsMatch($text)) { $text = $pattern.Replace($text, { $section }, 1) }
        else { $text = ([regex]::new('^\{\r?\n')).Replace($text, { "{$newline$section$newline" }, 1) }

        foreach ($key in @($settings.Keys)) { $text = Set-JsonValue $text $key $settings[$key] }

        [IO.File]::WriteAllText($path, $text, [Text.UTF8Encoding]::new($bom))
    }

    # um valor simples (texto, número, true/false) pelo caminho com ":", achado em ordem no texto: cada nome é o primeiro
    # depois do anterior, o que basta para o appsettings, em que cada caminho tem nomes únicos dentro da seção dele
    function Set-JsonValue([string]$text, [string]$path, $value) {
        $position = 0
        foreach ($name in $path.Split(':')) {
            $match = [regex]::new('"' + [regex]::Escape($name) + '"\s*:\s*').Match($text, $position)
            if (-not $match.Success) { throw "$path não está no appsettings.json." }
            $position = $match.Index + $match.Length
        }

        $literal = [regex]::new('"(?:[^"\\]|\\.)*"|true|false|null|-?\d+(?:\.\d+)?').Match($text, $position)
        if (-not $literal.Success -or $literal.Index -ne $position) { throw "$path não é um valor simples no appsettings.json." }

        $json = $value -is [bool] ? ($value ? 'true' : 'false') : '"' + ([string]$value).Replace('\', '\\').Replace('"', '\"') + '"'
        $text.Substring(0, $position) + $json + $text.Substring($literal.Index + $literal.Length)
    }

    # as chaves que o appsettings passou a ter, tiradas dos User Secrets do projeto (planas ou aninhadas, pelo prefixo):
    # elas passariam por cima do appsettings
    function Remove-SecretKeys([string]$path, [string[]]$prefixes) {
        if (-not $path -or -not (Test-Path -LiteralPath $path)) { return @() }

        $secretsJson = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json -AsHashtable
        if (-not $secretsJson) { return @() }

        $removed = @($secretsJson.Keys | Where-Object { $key = $_; $prefixes | Where-Object { $key -eq $_ -or $key -like "$($_):*" } })
        if ($removed.Count -eq 0) { return @() }

        foreach ($key in $removed) { $secretsJson.Remove($key) }
        Set-Content -LiteralPath $path -Value ($secretsJson | ConvertTo-Json -Depth 20) -Encoding utf8NoBOM
        $removed
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
        Write-Line "[$index/$($apps.Count)] $($app.Name) ($($app.Scripts) scripts$($null -ne $app.SelectedServices ? ", services: $(@($app.SelectedServices).Count) de $($app.Services.Count)" : ''))" 'step'

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

            # services escolhidos (migrator.csx dos módulos): sem escolha, todos; com ela, as pastas separadas por ";" (o ";" no
            # fim garante a variável mesmo sem nenhum service: aí só Parameters)
            if ($null -ne $app.SelectedServices) { $info.Environment['ETL_MIGRATOR_SERVICES'] = (@($app.SelectedServices) -join ';') + ';' }
            else { $null = $info.Environment.Remove('ETL_MIGRATOR_SERVICES') }

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

    # as connection strings deste banco nos appsettings.json dos projetos (só com tudo migrado): o log mostra só as chaves
    if ($appSettings) {
        Write-Line '' 'out'

        foreach ($project in $appSettings) {
            try {
                Update-AppSettings $project.Path $project.Values $project.Settings
                $written = @(@($project.Values.Keys | ForEach-Object { "ConnectionStrings:$_" }) + @($project.Settings.Keys))
                Write-Line "$($project.Name): appsettings.json com $($written -join ', ')." 'ok'

                $removed = Remove-SecretKeys $project.SecretsPath (@('ConnectionStrings') + @($project.Settings.Keys))
                if ($removed.Count -gt 0) {
                    Write-Line "$($project.Name): $($removed -join ', ') saíram dos User Secrets (passariam por cima do appsettings)." 'warn'
                }
            }
            catch {
                Write-Line "$($project.Name): não foi possível gravar o appsettings.json: $($_.Exception.Message)" 'error'
            }
        }
    }

    $state.Successful = $true
    Write-Line '' 'out'
    Write-Line 'Concluído.' 'ok'
    Write-Line '' 'out'
    Write-Line "Host de agenda: $($schedulingHost.Title). Para ele ser o único a agendar:" 'step'

    foreach ($step in $schedulingHost.Steps) { Write-Line "  - $step" 'out' }
    Write-Line '' 'out'
    Write-Line 'Holdings, empresas, credenciais e jobs: pelo WebApp (tela inicial, "Cadastrar holding e empresas") ou, num banco de teste, ./tools/seeds/Import-TestSeeds.ps1.' 'step'
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

# os appsettings.json que recebem a configuração deste banco (Update-AppSettings, no fim da migração) e os User Secrets de
# cada projeto, de onde as mesmas chaves saem. WebApi, WebJob e Worker: o usuário da tela (auditing fica no mesmo banco).
# WebApp: etl_monitor e etl_query com as senhas da seção Usuários de banco do WebApp; sem a senha, o usuário da tela.
# WebApi, da seção Hangfire: Hangfire:Enabled ligado só com o WebApi como host de agenda e, com o usuário preenchido, o
# usuário e a senha do dashboard
function New-AppSettingsTargets([string]$server, [int]$port, [string]$database, [string]$user, [string]$password, [bool]$trust, $userPasswords,
    [string]$schedulingHostKey, [string]$hangfireUser, [string]$hangfirePassword) {
    $root = Join-Path $PSScriptRoot '../..' | Resolve-Path | Select-Object -ExpandProperty Path
    $main = New-ConnectionString $server $port $database $user $password $trust

    $monitor = $userPasswords['ETL_MONITOR_PASSWORD'] ? (New-ConnectionString $server $port $database 'etl_monitor' $userPasswords['ETL_MONITOR_PASSWORD'] $trust) : $main
    $query = $userPasswords['ETL_QUERY_PASSWORD'] ? (New-ConnectionString $server $port $database 'etl_query' $userPasswords['ETL_QUERY_PASSWORD'] $trust) : $main

    $hangfire = [ordered]@{ 'Hangfire:Enabled' = $schedulingHostKey -eq 'WebApi' }
    if ($hangfireUser) {
        $hangfire['Hangfire:Dashboard:UserName'] = $hangfireUser
        $hangfire['Hangfire:Dashboard:Password'] = $hangfirePassword
    }

    $projects = [ordered]@{
        WebApp = @{ Folder = 'src/Frontend/WebApp'; Values = [ordered]@{ Connection = $monitor; QueryConnection = $query } }
        WebApi = @{ Folder = 'src/Backend/Hosts/WebApi'; Values = [ordered]@{ Connection = $main }; Settings = $hangfire }
        WebJob = @{ Folder = 'src/Backend/Hosts/WebJob'; Values = [ordered]@{ Connection = $main } }
        Worker = @{ Folder = 'src/Backend/Hosts/Worker'; Values = [ordered]@{ Connection = $main } }
    }

    foreach ($name in $projects.Keys) {
        $folder = Join-Path $root $projects[$name].Folder
        $project = Get-ChildItem -LiteralPath $folder -Filter '*.csproj' | Select-Object -First 1
        $secretsId = $project ? ((([xml](Get-Content -LiteralPath $project.FullName -Raw)).Project.PropertyGroup.UserSecretsId | Where-Object { $_ }) | Select-Object -First 1) : $null

        [pscustomobject]@{
            Name = $name
            Path = Join-Path $folder 'appsettings.json'
            Values = $projects[$name].Values
            Settings = $projects[$name].Settings ?? [ordered]@{}
            SecretsPath = $secretsId ? (Join-Path $env:APPDATA "Microsoft/UserSecrets/$secretsId/secrets.json") : $null
        }
    }
}

function Start-Migration($apps, [string]$connectionString, [string[]]$secrets, $schedulingHost, $userPasswords, $appSettings) {
    $queue = [Collections.Concurrent.ConcurrentQueue[object]]::new()
    $state = [hashtable]::Synchronized(@{ Cancel = $false; Process = $null; Successful = $false })

    $powershell = [powershell]::Create()
    $null = $powershell.AddScript($worker).AddArgument($apps).AddArgument($connectionString).AddArgument($secrets).
        AddArgument($queue).AddArgument($state).AddArgument($schedulingHost).AddArgument($userPasswords).AddArgument($appSettings)

    [pscustomobject]@{ PowerShell = $powershell; Handle = $powershell.BeginInvoke(); Queue = $queue; State = $state }
}

function Test-DotnetScript {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { return $false }

    $null = & dotnet script --version 2>&1
    $LASTEXITCODE -eq 0
}

$missingTool = 'O dotnet-script não está instalado. Instale com: dotnet tool install -g dotnet-script'

# ---------------------------------------------------------------------------------------------------------------------
# sem tela
# ---------------------------------------------------------------------------------------------------------------------

if ($NoGui) {
    if (-not $SchedulingHost) { throw 'Informe o host de agenda com -SchedulingHost (WebApi, WebJob ou Worker).' }

    if (-not (Test-DotnetScript)) { throw $missingTool }

    $schedulingHostInfo = $schedulingHosts[$SchedulingHost]
    $selected = @($catalog | Where-Object { $_.Required -or ($Apps ? $_.Name -in $Apps : $_.Default) } | ForEach-Object { $_.Clone() }) +
        @($schedulingHostInfo.App | Where-Object { $_ })

    # -Services: "Aplicação/service" (ex.: Totvs/People, LinxMicrovix/Erp/Customers); a aplicação sem nenhum na lista sobe
    # todos os services dela
    foreach ($service in $Services) {
        $name, $folder = $service.Replace('\', '/').Split('/', 2)
        $app = $selected | Where-Object { $_.Name -eq $name } | Select-Object -First 1

        if (-not $app) { throw "-Services $service`: a aplicação $name não está entre as que vão subir." }
        if ($folder -notin $app.Services.Name) { throw "-Services $service`: $name não tem o service $folder ($(($app.Services.Name) -join ', '))." }

        $app.SelectedServices = @($app.SelectedServices) + $folder | Where-Object { $_ }
    }

    if (-not $Credential) { $Credential = Get-Credential -Message "SQL Server $Server,$Port" }
    $password = $Credential.GetNetworkCredential().Password
    $connectionString = New-ConnectionString $Server $Port $Database $Credential.UserName $password $true

    $userPasswords = New-UserPasswords ($MonitorPassword ? [Net.NetworkCredential]::new('', $MonitorPassword).Password : '') `
        ($QueryPassword ? [Net.NetworkCredential]::new('', $QueryPassword).Password : '')

    Write-Host "Servidor $Server,$Port, banco ${Database}, host de agenda $($schedulingHostInfo.Title): $(($selected | ForEach-Object { $null -ne $_.SelectedServices ? "$($_.Name) ($(@($_.SelectedServices) -join ', '))" : $_.Name }) -join ', ')"
    Write-Host (Get-UserPasswordsText $userPasswords)

    $appSettings = $SkipAppSettings ? $null : @(New-AppSettingsTargets $Server $Port $Database $Credential.UserName $password $true $userPasswords `
        $SchedulingHost $HangfireUser ($HangfirePassword ? [Net.NetworkCredential]::new('', $HangfirePassword).Password : ''))
    $secrets = @(Get-Secrets $password) + @($userPasswords.Values | ForEach-Object { Get-Secrets $_ }) +
        @(Get-Secrets ($HangfirePassword ? [Net.NetworkCredential]::new('', $HangfirePassword).Password : ''))
    $run = Start-Migration $selected $connectionString $secrets $schedulingHostInfo $userPasswords $appSettings
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
    & (Get-Process -Id $PID).Path @arguments
    return
}

Add-Type -AssemblyName System.Windows.Forms, System.Drawing
[Windows.Forms.Application]::EnableVisualStyles()

$font = [Drawing.Font]::new('Segoe UI', 9)

$form = [Windows.Forms.Form]@{
    Text = 'Migrators: subir tabelas no SQL Server'
    ClientSize = [Drawing.Size]::new(860, 880)
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
    Text = 'Em branco, o usuário não é criado agora. Quem já existe passa a ter a senha informada (a mesma que vai para o appsettings).'
    Location = [Drawing.Point]::new(526, 22); Size = [Drawing.Size]::new(296, 54); ForeColor = [Drawing.SystemColors]::GrayText
}

if ($MonitorPassword) { $monitorBox.Text = [Net.NetworkCredential]::new('', $MonitorPassword).Password }
if ($QueryPassword) { $queryBox.Text = [Net.NetworkCredential]::new('', $QueryPassword).Password }

$usersGroup.Controls.AddRange(@($monitorLabel, $monitorBox, $queryLabel, $queryBox, $usersHint))

# dashboard do Hangfire: vai para o appsettings.json do WebApi (Hangfire:Dashboard), com a caixa dos appsettings marcada;
# Hangfire:Enabled segue o host de agenda escolhido

$hangfireGroup = [Windows.Forms.GroupBox]@{
    Text = 'Dashboard do Hangfire (WebApi)'; Location = [Drawing.Point]::new(12, 326); Size = [Drawing.Size]::new(836, 64)
    Anchor = 'Top, Left, Right'
}

$hangfireUserLabel = [Windows.Forms.Label]@{ Text = 'Usuário'; Location = [Drawing.Point]::new(14, 28); AutoSize = $true }
$hangfireUserBox = [Windows.Forms.TextBox]@{ Location = [Drawing.Point]::new(70, 25); Width = 184 }
$hangfirePasswordLabel = [Windows.Forms.Label]@{ Text = 'Senha'; Location = [Drawing.Point]::new(270, 28); AutoSize = $true }
$hangfirePasswordBox = [Windows.Forms.TextBox]@{ UseSystemPasswordChar = $true; Location = [Drawing.Point]::new(320, 25); Width = 190 }
$hangfireHint = [Windows.Forms.Label]@{
    Text = 'Obrigatório com o WebApi como host de agenda. Em branco com outro host, o appsettings fica como está.'
    Location = [Drawing.Point]::new(526, 18); Size = [Drawing.Size]::new(296, 40); ForeColor = [Drawing.SystemColors]::GrayText
}

if ($HangfireUser) { $hangfireUserBox.Text = $HangfireUser }
if ($HangfirePassword) { $hangfirePasswordBox.Text = [Net.NetworkCredential]::new('', $HangfirePassword).Password }

$hangfireGroup.Controls.AddRange(@($hangfireUserLabel, $hangfireUserBox, $hangfirePasswordLabel, $hangfirePasswordBox, $hangfireHint))

# connection strings deste banco nos appsettings.json do WebApp, do WebApi, do WebJob e do Worker, no fim da migração
$appSettingsBox = [Windows.Forms.CheckBox]@{
    Text = 'Gravar as connection strings nos appsettings'; Checked = -not $SkipAppSettings
    Location = [Drawing.Point]::new(262, 679); AutoSize = $true
}

# catálogo

$catalogLabel = [Windows.Forms.Label]@{
    Text = 'Catálogo das aplicações e dos services (rodam na ordem da lista; o Shared é obrigatório; Parameters sobe sempre)'
    Location = [Drawing.Point]::new(12, 400); AutoSize = $true
}

# uma linha por aplicação e, abaixo, uma por service (as pastas de Scripts/SQLServer): marcar ou desmarcar a aplicação
# marca ou desmarca os services dela, e a aplicação fica marcada enquanto algum service estiver
$list = [Windows.Forms.TreeView]@{
    CheckBoxes = $true; ShowLines = $true; HideSelection = $false
    Location = [Drawing.Point]::new(12, 420); Size = [Drawing.Size]::new(836, 246); Anchor = 'Top, Left, Right'
}

foreach ($app in $catalog) {
    $node = [Windows.Forms.TreeNode]::new("[$($app.Group)] $($app.Name) ($($app.Scripts) scripts): $($app.Description)")
    $node.Tag = $app
    $node.Checked = [bool]$app.Default

    if ($app.Required) { $node.ForeColor = [Drawing.SystemColors]::GrayText }

    foreach ($service in $app.Services) {
        $child = [Windows.Forms.TreeNode]::new("$($service.Name) ($($service.Scripts) scripts)")
        $child.Tag = $service.Name
        $child.Checked = $node.Checked
        $null = $node.Nodes.Add($child)
    }

    $null = $list.Nodes.Add($node)
}

$list.Add_BeforeCheck({
    param($sender, $e)

    if ($e.Node.Tag -is [hashtable] -and $e.Node.Tag.Required -and $e.Node.Checked) { $e.Cancel = $true }
})

# só o clique (ou o teclado) propaga: as mudanças feitas aqui chegam com Action = Unknown e param
$list.Add_AfterCheck({
    param($sender, $e)

    if ($e.Action -eq 'Unknown') { return }

    if ($e.Node.Parent) {
        $parent = $e.Node.Parent
        $any = @($parent.Nodes | Where-Object Checked).Count -gt 0
        if ($parent.Checked -ne $any) { $parent.Checked = $any }
    }
    else {
        foreach ($child in $e.Node.Nodes) { $child.Checked = $e.Node.Checked }
    }
})

# as aplicações marcadas, cada uma com os services escolhidos (nulo: todos)
function Get-SelectedApps {
    @($list.Nodes | Where-Object Checked | ForEach-Object {
            $app = $_.Tag.Clone()
            $chosen = @($_.Nodes | Where-Object Checked | ForEach-Object Tag)
            if ($app.Services.Count -gt 0 -and $chosen.Count -lt $app.Services.Count) { $app.SelectedServices = $chosen }
            $app
        })
}

$checkAllButton = [Windows.Forms.Button]@{ Text = 'Marcar todas'; Location = [Drawing.Point]::new(12, 674); Size = [Drawing.Size]::new(110, 28) }
$checkNoneButton = [Windows.Forms.Button]@{ Text = 'Desmarcar todas'; Location = [Drawing.Point]::new(128, 674); Size = [Drawing.Size]::new(120, 28) }
$runButton = [Windows.Forms.Button]@{
    Text = 'Subir tabelas'; Location = [Drawing.Point]::new(616, 674); Size = [Drawing.Size]::new(120, 28); Anchor = 'Top, Right'
}
$cancelButton = [Windows.Forms.Button]@{
    Text = 'Cancelar'; Location = [Drawing.Point]::new(742, 674); Size = [Drawing.Size]::new(106, 28); Anchor = 'Top, Right'; Enabled = $false
}

# log

$log = [Windows.Forms.RichTextBox]@{
    Location = [Drawing.Point]::new(12, 712); Size = [Drawing.Size]::new(836, 134); Anchor = 'Top, Bottom, Left, Right'
    ReadOnly = $true; BackColor = [Drawing.Color]::FromArgb(30, 30, 30); ForeColor = [Drawing.Color]::Gainsboro
    Font = [Drawing.Font]::new('Consolas', 9); WordWrap = $false
}

$status = [Windows.Forms.Label]@{
    Text = 'Pronto.'; Location = [Drawing.Point]::new(12, 854); AutoSize = $true; Anchor = 'Bottom, Left'
}

$form.Controls.AddRange(@($connection, $hostGroup, $usersGroup, $hangfireGroup, $appSettingsBox, $catalogLabel, $list, $checkAllButton, $checkNoneButton, $runButton, $cancelButton, $log, $status))

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
    foreach ($control in $connection, $hostGroup, $usersGroup, $hangfireGroup, $appSettingsBox, $list, $checkAllButton, $checkNoneButton, $runButton) { $control.Enabled = -not $running }
    $cancelButton.Enabled = $running
}

$checkAllButton.Add_Click({ foreach ($node in $list.Nodes) { $node.Checked = $true; foreach ($child in $node.Nodes) { $child.Checked = $true } } })
$checkNoneButton.Add_Click({
    foreach ($node in $list.Nodes) {
        # o obrigatório (Shared) continua marcado
        if (-not $node.Tag.Required) { $node.Checked = $false }
        foreach ($child in $node.Nodes) { $child.Checked = $false }
    }
})

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

    if ($appSettingsBox.Checked -and $hostRadio.Tag -eq 'WebApi' -and -not ($hangfireUserBox.Text.Trim() -and $hangfirePasswordBox.Text)) {
        $null = [Windows.Forms.MessageBox]::Show('Com o WebApi como host de agenda, preencha o usuário e a senha do dashboard do Hangfire.', $form.Text, 'OK', 'Warning')
        return
    }

    $schedulingHost = $schedulingHosts[$hostRadio.Tag]
    $selected = @(Get-SelectedApps) + @($schedulingHost.App | Where-Object { $_ })
    $names = ($selected | ForEach-Object { $null -ne $_.SelectedServices ? "$($_.Name) ($(@($_.SelectedServices).Count) de $($_.Services.Count) services)" : $_.Name }) -join ', '
    $userPasswords = New-UserPasswords $monitorBox.Text $queryBox.Text
    $usersText = Get-UserPasswordsText $userPasswords

    $answer = [Windows.Forms.MessageBox]::Show(
        ("Subir as tabelas no banco $($fields.Database) de $($fields.Server),$($fields.Port)?`n`n$names`n`n" +
            "Host de agenda: $($schedulingHost.Title)`n`n$usersText`n`n" +
            ($appSettingsBox.Checked ? "As connection strings deste banco (e, no WebApi, a seção Hangfire) vão para os appsettings.json do WebApp, do WebApi, do WebJob e do Worker (e saem dos User Secrets deles).`n`n" : '') +
            "O banco é criado se não existir."),
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

    $appSettings = $appSettingsBox.Checked ? @(New-AppSettingsTargets $fields.Server $fields.Port $fields.Database $fields.User $fields.Password $fields.Trust $userPasswords `
        $hostRadio.Tag $hangfireUserBox.Text.Trim() $hangfirePasswordBox.Text) : $null
    $secrets = @(Get-Secrets $fields.Password) + @($userPasswords.Values | ForEach-Object { Get-Secrets $_ }) + @(Get-Secrets $hangfirePasswordBox.Text)
    $script:run = Start-Migration $selected $connectionString $secrets $schedulingHost $userPasswords $appSettings
    $timer.Start()
})

$cancelButton.Add_Click({
    if (-not $script:run) { return }

    $script:run.State.Cancel = $true
    $process = $script:run.State.Process

    # entre um migrator e outro não há processo: o runspace para
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
