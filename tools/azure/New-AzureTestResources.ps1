#Requires -Version 7
<#
.SYNOPSIS
    Cria (ou apaga) os recursos da Azure dos testes de hospedagem C3, C6, C7 e A10 pelo az cli, na assinatura da
    NEWBLOOMERS, nos planos do orçamento (.docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md, seções 5 e 5.1).

.DESCRIPTION
    Tudo fica no grupo rg-etl-teste da assinatura da NEWBLOOMERS (f65c8654-2630-4bda-aba5-4c2a8eba4cf0), fixa no script
    (não é parâmetro). Cada comando az leva --subscription. A assinatura tem produção e o ambiente dev dos WebJobs, então
    o script só toca no grupo dele: o grupo nasce com a tag finalidade=etl-teste, a criação para se o grupo já existir
    sem essa tag, e o -Remove só apaga um grupo com ela.

    Por cenário:
      * C3, C6: um banco Azure SQL EtlTeste_<cenário> no free offer (General Purpose serverless, Gen5, até 2 vCores,
        mínimo 0,5, auto-pause, "pausar até o mês seguinte" ao acabar o limite gratuito), no servidor lógico do grupo,
        com regra de firewall para o IP público desta máquina.
      * C7: o banco EtlTeste_C7, o plano App Service Windows Basic B1, o App Service dos WebJobs (Always On, identidade
        gerenciada com o papel Website Contributor nele mesmo, para a recuperação) e o do painel (WebSockets), com as
        configurações e as connection strings; os IPs de saída dos App Services entram no firewall do banco.
      * A10: o banco EtlTeste_A10 e um App Service do WebApi com Hangfire (Always On) no mesmo plano B1.
    Rodar de novo só cria o que falta (servidor, plano e apps já existentes são reaproveitados), então os cenários
    podem ser criados um de cada vez. O alerta de orçamento (Budget) é criado no escopo do grupo
    (só os custos dele) se -AlertEmail for passado.

    O banco é sempre criado aqui, antes do migrator, já com a collation Latin1_General_100_CI_AS_SC_UTF8 (no Azure SQL ela
    não muda depois de criado, e o Script001 do Shared não a altera lá) e no free offer: o EnsureDatabase do DbUp criaria
    no tier padrão, que é pago. Se a Azure recusar o free offer (por exemplo, limite de bancos gratuitos na assinatura),
    o script para: nunca cai para um banco pago.

    Senhas não aparecem no console: o administrador do SQL vem de -SqlAdminCredential (ou Get-Credential), e as senhas do
    etl_monitor/etl_query, das variáveis ETL_MONITOR_PASSWORD e ETL_QUERY_PASSWORD (as mesmas que se
    passam ao Invoke-Migrators.ps1 em -MonitorPassword e -QueryPassword, que cria os dois usuários no banco).

    Pré-requisito: az login com acesso à assinatura da NEWBLOOMERS.

.PARAMETER Scenario
    Cenários a criar: C3, C6, C7, A10 (um ou mais).

.PARAMETER Phase
    Fase das configurações dos App Services: A (padrão, BaseUrl das APIs em http://127.0.0.1:9/, nenhuma chamada
    externa) ou B (APIs reais, BaseUrl do appsettings.json do host).

.PARAMETER Location
    Região; sem ela, brazilsouth (a do orçamento).

.PARAMETER SqlAdminCredential
    Login e senha do administrador do servidor lógico (criação do servidor e connection string dos hosts); sem ele, o
    script pede com Get-Credential.

.PARAMETER ClientIp
    IP público desta máquina para o firewall do banco; sem ele, descoberto em https://api.ipify.org.

.PARAMETER AlertEmail
    E-mail dos alertas do Budget (50%, 80% e 100% de -BudgetAmount). Sem ele, o Budget não é criado nem alterado.

.PARAMETER BudgetAmount
    Valor mensal do Budget, na moeda da assinatura; sem ele, 100.

.PARAMETER Remove
    Apaga o Budget e o grupo rg-etl-teste inteiro (bancos, servidor, plano e apps), com confirmação, e confere que o
    grupo não existe mais (etapa 7 da seção 6). Só apaga um grupo com a tag finalidade=etl-teste.

.PARAMETER Force
    Com -Remove, não pede confirmação.

.PARAMETER DryRun
    Mostra os comandos az, sem chamar a Azure.

.EXAMPLE
    ./tools/azure/New-AzureTestResources.ps1 -Scenario C3 -AlertEmail fulano@exemplo.com -DryRun

.EXAMPLE
    ./tools/azure/New-AzureTestResources.ps1 -Scenario C7 -Phase B

.EXAMPLE
    ./tools/azure/New-AzureTestResources.ps1 -Remove
#>
[CmdletBinding(DefaultParameterSetName = 'Create')]
param(
    [Parameter(Mandatory, ParameterSetName = 'Create')]
    [ValidateSet('C3', 'C6', 'C7', 'A10')]
    [string[]]$Scenario,

    [Parameter(ParameterSetName = 'Create')]
    [ValidateSet('A', 'B')]
    [string]$Phase = 'A',

    [Parameter(ParameterSetName = 'Create')]
    [string]$Location = 'brazilsouth',

    [Parameter(ParameterSetName = 'Create')]
    [pscredential]$SqlAdminCredential,

    [Parameter(ParameterSetName = 'Create')]
    [string]$ClientIp,

    [Parameter(ParameterSetName = 'Create')]
    [string]$AlertEmail,

    [Parameter(ParameterSetName = 'Create')]
    [ValidateRange(1, 100000)]
    [int]$BudgetAmount = 100,

    [Parameter(Mandatory, ParameterSetName = 'Remove')]
    [switch]$Remove,

    [Parameter(ParameterSetName = 'Remove')]
    [switch]$Force,

    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------------------------------------------------
# nomes (seção 5): a assinatura é fixa (a da NEWBLOOMERS); servidor e apps precisam de nome único na Azure, daí o sufixo da assinatura
# ---------------------------------------------------------------------------------------------------------------------

$subscription = 'f65c8654-2630-4bda-aba5-4c2a8eba4cf0'
$resourceGroup = 'rg-etl-teste'
$tag = 'finalidade=etl-teste'
$suffix = $subscription.Substring(0, 6)

$sqlServer = "sql-etl-teste-$suffix"
$plan = 'plan-etl-teste'
$appWebJobs = "app-etl-teste-webjobs-$suffix"
$appPanel = "app-etl-teste-painel-$suffix"
$appWebApi = "app-etl-teste-webapi-$suffix"
$budgetName = 'budget-etl-teste'

# segredos trocados por **** no que o script mostra
$secrets = [Collections.Generic.List[string]]::new()

# sem bloco param: os argumentos do az ("-o", "--name"...) passam direto em $args
function Invoke-Az {
    $Arguments = [string[]]$args

    $shown = "az $($Arguments -join ' ') --subscription $subscription"
    foreach ($secret in $secrets) { if ($secret) { $shown = $shown.Replace($secret, '****') } }
    Write-Host "  $shown" -ForegroundColor DarkGray

    if ($DryRun) { return $null }

    $output = & az @Arguments --subscription $subscription --only-show-errors 2>&1
    if ($LASTEXITCODE -ne 0) {
        $message = ($output | Out-String)
        foreach ($secret in $secrets) { if ($secret) { $message = $message.Replace($secret, '****') } }
        throw "az falhou ($LASTEXITCODE): $message"
    }
    return $output
}

# leitura: em DryRun não chama a Azure e responde "não existe"
function Test-Az {
    $Arguments = [string[]]$args

    if ($DryRun) { return $false }
    & az @Arguments --subscription $subscription --only-show-errors -o none 2>$null
    return $LASTEXITCODE -eq 0
}

# ---------------------------------------------------------------------------------------------------------------------
# trava contra engano: o login precisa enxergar a assinatura
# ---------------------------------------------------------------------------------------------------------------------

if (-not $DryRun) {
    $account = & az account show --subscription $subscription --query '{name:name, id:id}' -o json 2>$null | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0 -or $account.id -ne $subscription) {
        throw "O az login não enxerga a assinatura da NEWBLOOMERS $subscription. Rode 'az login' com a conta dela."
    }
    Write-Host "Assinatura: $($account.name) ($subscription)." -ForegroundColor Cyan
}

# ---------------------------------------------------------------------------------------------------------------------
# grupo dos testes: só o que tem a tag finalidade=etl-teste (a assinatura tem produção)
# ---------------------------------------------------------------------------------------------------------------------

$budgetUrl = "https://management.azure.com/subscriptions/$subscription/resourceGroups/$resourceGroup/providers/Microsoft.Consumption/budgets/$($budgetName)?api-version=2023-05-01"

# $null: o grupo não existe; $true/$false: existe, com ou sem a tag dos testes
function Get-TestGroupTagged {
    if ($DryRun -or -not (Test-Az group show --name $resourceGroup)) { return $null }
    $value = & az group show --name $resourceGroup --subscription $subscription --query 'tags.finalidade' -o tsv 2>$null
    return $value -eq 'etl-teste'
}

# ---------------------------------------------------------------------------------------------------------------------
# remoção: o Budget e o grupo inteiro, com conferência no fim (seção 6, etapa 7)
# ---------------------------------------------------------------------------------------------------------------------

if ($Remove) {
    $tagged = Get-TestGroupTagged

    if (-not $DryRun -and $null -eq $tagged) {
        Write-Host "O grupo $resourceGroup não existe: nada a apagar." -ForegroundColor Green
        return
    }
    if (-not $DryRun -and -not $tagged) {
        throw "O grupo $resourceGroup existe, mas sem a tag ${tag}: não foi criado por este script e não será apagado."
    }

    if (-not $Force -and -not $DryRun -and
        -not $PSCmdlet.ShouldContinue("Apagar o grupo $resourceGroup e tudo dentro dele na assinatura $subscription?", 'Remover recursos de teste')) {
        Write-Host 'Nada foi apagado.' -ForegroundColor Yellow
        return
    }

    Write-Host "Apagando o Budget $budgetName..."
    # sem Budget (não houve -AlertEmail) ou sem acesso ao Cost Management: avisa e segue, porque o grupo tem de sair
    try { Invoke-Az rest --method delete --url $budgetUrl -o none | Out-Null }
    catch { Write-Warning "Budget $budgetName não foi apagado (não existe ou falta acesso ao Cost Management); o grupo será apagado assim mesmo." }

    Write-Host "Apagando $resourceGroup (leva alguns minutos)..." -ForegroundColor Yellow
    Invoke-Az group delete --name $resourceGroup --yes | Out-Null

    # a conferência: o grupo não pode existir mais
    if (-not $DryRun) {
        $exists = & az group exists --name $resourceGroup --subscription $subscription
        if ($exists -ne 'false') { throw "O grupo $resourceGroup ainda existe depois do delete: conferir no portal e apagar de novo." }
    }
    Write-Host "Grupo $resourceGroup apagado e conferido (az group exists = false)." -ForegroundColor Green
    return
}

# ---------------------------------------------------------------------------------------------------------------------
# criação
# ---------------------------------------------------------------------------------------------------------------------

$needsApps = $Scenario | Where-Object { $_ -in 'C7', 'A10' }

if (-not $SqlAdminCredential) {
    $SqlAdminCredential = Get-Credential -Message "Administrador do servidor $sqlServer (criação e connection string dos hosts)"
}
$sqlUser = $SqlAdminCredential.UserName
$sqlPassword = $SqlAdminCredential.GetNetworkCredential().Password
$secrets.Add($sqlPassword)

$monitorPassword = $env:ETL_MONITOR_PASSWORD
$queryPassword = $env:ETL_QUERY_PASSWORD
$secrets.Add($monitorPassword)
$secrets.Add($queryPassword)

if (-not $ClientIp) {
    $ClientIp = if ($DryRun) { '0.0.0.0' } else { (Invoke-RestMethod -Uri 'https://api.ipify.org' -TimeoutSec 15).Trim() }
}

Write-Host "Cenários $($Scenario -join ', '), fase $Phase, região $Location, grupo $resourceGroup." -ForegroundColor Cyan

# grupo: um grupo com o mesmo nome e sem a tag não é dos testes, e o script não mexe nele
$tagged = Get-TestGroupTagged
if ($tagged -eq $false) {
    throw "O grupo $resourceGroup já existe sem a tag ${tag}: não é dos testes. Nada foi criado."
}
if ($null -eq $tagged) {
    Write-Host "Grupo $resourceGroup (tag $tag)..."
    Invoke-Az group create --name $resourceGroup --location $Location --tags $tag -o none | Out-Null
}

# Budget no escopo do grupo: só os custos dos testes; só avisa (não desliga nada); o -Remove apaga
if ($AlertEmail) {
    Write-Host "Budget $budgetName ($BudgetAmount por mês, alertas em 50%, 80% e 100% para $AlertEmail)..."
    $start = (Get-Date -Day 1).ToString('yyyy-MM-01T00:00:00Z')
    $notifications = @{}
    foreach ($threshold in 50, 80, 100) {
        $notifications["Actual_GreaterThan_$threshold"] = @{
            enabled       = $true
            operator      = 'GreaterThan'
            threshold     = $threshold
            thresholdType = 'Actual'
            contactEmails = @($AlertEmail)
        }
    }
    $body = @{
        properties = @{
            category      = 'Cost'
            amount        = $BudgetAmount
            timeGrain     = 'Monthly'
            timePeriod    = @{ startDate = $start; endDate = '2027-12-31T00:00:00Z' }
            notifications = $notifications
        }
    } | ConvertTo-Json -Depth 6 -Compress

    $bodyFile = Join-Path ([IO.Path]::GetTempPath()) 'etl-teste-budget.json'
    Set-Content -Path $bodyFile -Value $body -Encoding utf8
    try {
        Invoke-Az rest --method put `
            --url $budgetUrl `
            --body "@$bodyFile" -o none | Out-Null
    }
    finally { Remove-Item $bodyFile -ErrorAction SilentlyContinue }
}
else {
    Write-Warning 'Sem -AlertEmail: o Budget não foi criado nem alterado (seção 5, item 3).'
}

# servidor lógico e firewall desta máquina
if (-not (Test-Az sql server show --resource-group $resourceGroup --name $sqlServer)) {
    Write-Host "Servidor $sqlServer..."
    Invoke-Az sql server create --resource-group $resourceGroup --name $sqlServer --location $Location `
        --admin-user $sqlUser --admin-password $sqlPassword --minimal-tls-version 1.2 -o none | Out-Null
}

Invoke-Az sql server firewall-rule create --resource-group $resourceGroup --server $sqlServer --name maquina-teste `
    --start-ip-address $ClientIp --end-ip-address $ClientIp -o none | Out-Null

# bancos no free offer: um por cenário, cada um com o próprio limite gratuito
foreach ($name in $Scenario) {
    $database = "EtlTeste_$name"

    if (Test-Az sql db show --resource-group $resourceGroup --server $sqlServer --name $database) {
        Write-Host "Banco $database já existe."
        continue
    }

    Write-Host "Banco $database (free offer, serverless)..."
    try {
        Invoke-Az sql db create --resource-group $resourceGroup --server $sqlServer --name $database `
            --edition GeneralPurpose --family Gen5 --capacity 2 --compute-model Serverless --min-capacity 0.5 `
            --auto-pause-delay 60 --backup-storage-redundancy Local --collation Latin1_General_100_CI_AS_SC_UTF8 `
            --use-free-limit true --free-limit-exhaustion-behavior AutoPause -o none | Out-Null
    }
    catch {
        throw "A Azure recusou o banco $database no free offer; nenhum banco pago foi criado. $($_.Exception.Message)"
    }
}

# ---------------------------------------------------------------------------------------------------------------------
# App Services (C7, A10): plano B1 Windows e os apps
# ---------------------------------------------------------------------------------------------------------------------

function Get-ConnectionString($database, $user, $password) {
    "Server=tcp:$sqlServer.database.windows.net,1433;Initial Catalog=$database;User ID=$user;Password=$password;Encrypt=True;Connection Timeout=60;"
}

# fase A: nenhuma chamada externa; fase B: apaga essas chaves e vale o BaseUrl do appsettings.json do host
$baseUrlKeys = 'Totvs', 'LinxMicrovix', 'LinxCommerce', 'Movidesk', 'AfterSale' | ForEach-Object { "$($_)__BaseUrl" }
$baseUrls = if ($Phase -eq 'A') { $baseUrlKeys | ForEach-Object { "$_=http://127.0.0.1:9/" } } else { @() }

function New-App($app, [string[]]$appSettings, [hashtable]$connectionStrings, [switch]$WebSockets) {
    if (-not (Test-Az webapp show --resource-group $resourceGroup --name $app)) {
        Write-Host "App Service $app..."
        Invoke-Az webapp create --resource-group $resourceGroup --plan $plan --name $app --runtime 'dotnet:10' -o none | Out-Null
    }

    $config = @('webapp', 'config', 'set', '--resource-group', $resourceGroup, '--name', $app, '--always-on', 'true', '--use-32bit-worker-process', 'false')
    if ($WebSockets) { $config += @('--web-sockets-enabled', 'true') }
    Invoke-Az @config -o none | Out-Null

    Invoke-Az webapp config appsettings set --resource-group $resourceGroup --name $app --settings @appSettings -o none | Out-Null

    if ($Phase -eq 'B') {
        Invoke-Az webapp config appsettings delete --resource-group $resourceGroup --name $app --setting-names @baseUrlKeys -o none | Out-Null
    }

    foreach ($key in $connectionStrings.Keys) {
        Invoke-Az webapp config connection-string set --resource-group $resourceGroup --name $app `
            --connection-string-type SQLAzure --settings "$key=$($connectionStrings[$key])" -o none | Out-Null
    }

    # os IPs de saída do app no firewall do banco
    if (-not $DryRun) {
        $ips = (Invoke-Az webapp show --resource-group $resourceGroup --name $app --query possibleOutboundIpAddresses -o tsv) -split ',' |
            Where-Object { $_ } | Sort-Object -Unique
        foreach ($ip in $ips) {
            Invoke-Az sql server firewall-rule create --resource-group $resourceGroup --server $sqlServer `
                --name "app-$($ip.Replace('.', '-'))" --start-ip-address $ip --end-ip-address $ip -o none | Out-Null
        }
    }
}

if ($needsApps) {
    if (-not (Test-Az appservice plan show --resource-group $resourceGroup --name $plan)) {
        Write-Host "Plano $plan (Windows Basic B1, cobrado por hora enquanto existir)..."
        Invoke-Az appservice plan create --resource-group $resourceGroup --name $plan --location $Location --sku B1 -o none | Out-Null
    }
}

if ('C7' -in $Scenario) {
    $database = 'EtlTeste_C7'

    $settings = @(
        'InitialLoad__MaxMonths=3'
        'Recovery__DryRun=true'
        "Recovery__TenantId=$(if ($DryRun) { '<tenant>' } else { (Invoke-Az account show --query tenantId -o tsv) })"
        "Recovery__SubscriptionId=$subscription"
        "Recovery__ResourceGroup=$resourceGroup"
        "Recovery__AppServiceName=$appWebJobs"
    ) + $baseUrls

    New-App $appWebJobs $settings @{ Connection = (Get-ConnectionString $database $sqlUser $sqlPassword) }

    # a recuperação lista, para e inicia os WebJobs pela identidade gerenciada do próprio App Service
    Write-Host "Identidade gerenciada de $appWebJobs (Website Contributor nele mesmo)..."
    $scope = if ($DryRun) { "<id de $appWebJobs>" } else { Invoke-Az webapp show --resource-group $resourceGroup --name $appWebJobs --query id -o tsv }
    $principal = Invoke-Az webapp identity assign --resource-group $resourceGroup --name $appWebJobs --query principalId -o tsv

    # dar o papel pede Microsoft.Authorization/roleAssignments/write (Owner ou User Access Administrator); sem ele, os
    # recursos seguem e só o reinício real da recuperação fica sem teste
    try {
        Invoke-Az role assignment create --assignee-object-id $principal --assignee-principal-type ServicePrincipal `
            --role 'Website Contributor' --scope $scope -o none | Out-Null
    }
    catch {
        if ($_.Exception.Message -notmatch 'AuthorizationFailed') { throw }
        Write-Warning "Sem permissão para dar o papel Website Contributor à identidade de ${appWebJobs}: a recuperação (Recovery) não consegue listar nem reiniciar os WebJobs. Peça a um Owner da assinatura."
    }

    # o painel lê com etl_monitor/etl_query (usuários contidos que o migrator cria com as mesmas variáveis)
    $panelConnections = @{}
    if ($monitorPassword) { $panelConnections.Connection = Get-ConnectionString $database 'etl_monitor' $monitorPassword }
    if ($queryPassword) { $panelConnections.QueryConnection = Get-ConnectionString $database 'etl_query' $queryPassword }
    if (-not $monitorPassword) { Write-Warning "Sem ETL_MONITOR_PASSWORD: o painel $appPanel fica sem connection string até rodar de novo com a variável." }

    New-App $appPanel @('Database__Enabled=true') $panelConnections -WebSockets
}

if ('A10' -in $Scenario) {
    $settings = @('Hangfire__Enabled=true', 'InitialLoad__MaxMonths=3') + $baseUrls
    New-App $appWebApi $settings @{ Connection = (Get-ConnectionString 'EtlTeste_A10' $sqlUser $sqlPassword) }
}

# ---------------------------------------------------------------------------------------------------------------------
# resumo: o que roda depois (migrator e publicação)
# ---------------------------------------------------------------------------------------------------------------------

Write-Host ''
Write-Host "Pronto. Servidor: $sqlServer.database.windows.net (porta 1433)." -ForegroundColor Green
foreach ($name in $Scenario) {
    $host_ = if ($name -in 'C3', 'A10') { 'WebApi' } elseif ($name -eq 'C6') { 'Worker' } else { 'WebJob' }
    Write-Host "  $($name): ./tools/migrators/Invoke-Migrators.ps1 -NoGui -Server $sqlServer.database.windows.net -Database EtlTeste_$name -SchedulingHost $host_"
    Write-Host "  $($name): ./tools/seeds/Import-TestSeeds.ps1 -Server $sqlServer.database.windows.net -Database EtlTeste_$name (holding, empresas, credenciais e jobs; ativar os jobs pelo WebApp)"
}
if ('C7' -in $Scenario) {
    Write-Host "  WebJobs: publish-webjob.ps1 -Profile teste (ResourceGroup $resourceGroup, AppServiceName $appWebJobs, SubscriptionId $subscription)."
    Write-Host "  Painel: https://$appPanel.azurewebsites.net (dotnet publish do WebApp + az webapp deploy)."
}
if ($needsApps) {
    Write-Host "  Lembrete: o plano $plan custa enquanto existir. No fim: ./tools/azure/New-AzureTestResources.ps1 -Remove" -ForegroundColor Yellow
}
