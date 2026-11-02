#Requires -Version 7
<#
.SYNOPSIS
    Publica o host Worker (Quartz.NET) como serviço do Windows (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md, passo 6).

.DESCRIPTION
    Lê o perfil em publish-profiles.psd1 (na mesma pasta), faz o "dotnet publish" e, na máquina do perfil (a local ou uma VM,
    por PowerShell Remoting): para o serviço (a parada espera os jobs em andamento, até Quartz:Shutdown:TimeoutInMinutes),
    copia o build para a pasta do serviço sem apagar o appsettings.Production.json de lá, cria o serviço na primeira vez
    (início automático atrasado, reinício automático se o processo cair, a conta do perfil, a origem no Log de Eventos e o
    DOTNET_ENVIRONMENT), inicia e confere que ficou rodando. Precisa de administrador na máquina de destino.

.PARAMETER PublishProfile
    Perfil do arquivo de perfis (alias -Profile); sem ele, o DefaultProfile.

.PARAMETER ProfilesFile
    Arquivo de perfis; sem ele, o publish-profiles.psd1 desta pasta.

.PARAMETER DryRun
    Faz o build e o pacote e mostra o que faria na máquina, sem tocar nela.

.PARAMETER Uninstall
    Para e remove o serviço (a pasta fica, com o appsettings.Production.json).

.EXAMPLE
    ./publish-worker.ps1 -Profile dev -DryRun

.EXAMPLE
    ./publish-worker.ps1 -Profile prod
#>
[CmdletBinding()]
param(
    [Alias('Profile')]
    [string]$PublishProfile,
    [string]$ProfilesFile,
    [switch]$DryRun,
    [switch]$Uninstall
)

$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------------------------------------------------
# perfil
# ---------------------------------------------------------------------------------------------------------------------

$profilesPath = if ($ProfilesFile) { Resolve-Path $ProfilesFile } else { Join-Path $PSScriptRoot 'publish-profiles.psd1' }
$profiles = Import-PowerShellDataFile $profilesPath
$profileName = if ($PublishProfile) { $PublishProfile } else { $profiles.DefaultProfile }
$settings = $profiles.Profiles[$profileName]

if (-not $settings) {
    throw "Perfil '$profileName' não existe em $profilesPath. Perfis: $($profiles.Profiles.Keys -join ', ')."
}

foreach ($key in 'InstallPath', 'ServiceName', 'Environment') {
    if (-not $settings[$key]) { throw "Perfil '$profileName': $key está vazio em $profilesPath." }
}

$serviceName = $settings.ServiceName
$displayName = if ($settings.DisplayName) { $settings.DisplayName } else { $serviceName }
$account = if ($settings.ServiceAccount) { $settings.ServiceAccount } else { "NT SERVICE\$serviceName" }
$target = if ($settings.ComputerName) { "em $($settings.ComputerName)" } else { 'nesta máquina' }

Write-Host "Perfil '$profileName': serviço '$serviceName' $target ($($settings.InstallPath)), conta $account." -ForegroundColor Cyan

# ---------------------------------------------------------------------------------------------------------------------
# o que roda na máquina de destino (local ou pela sessão remota)
# ---------------------------------------------------------------------------------------------------------------------

$deploy = {
    param($ZipPath, $InstallPath, $ServiceName, $DisplayName, $Account, $Credential, $Environment, [bool]$Uninstall)

    $ErrorActionPreference = 'Stop'

    function Wait-Status($service, $status, [int]$seconds) {
        try { $service.WaitForStatus($status, [TimeSpan]::FromSeconds($seconds)) }
        catch { throw "O serviço '$($service.Name)' não chegou a '$status' em $seconds s (está '$((Get-Service $service.Name).Status)')." }
    }

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

    # para o serviço: o Worker para de disparar e espera os jobs em andamento (WaitForJobsToComplete, até
    # Quartz:Shutdown:TimeoutInMinutes); a espera aqui cobre esse tempo
    if ($service -and $service.Status -ne 'Stopped') {
        Write-Host "Parando '$ServiceName' (espera os jobs em andamento)..."
        Stop-Service -Name $ServiceName -NoWait
        Wait-Status $service 'Stopped' 900
    }

    if ($Uninstall) {
        if ($service) {
            & sc.exe delete $ServiceName | Out-Null
            Write-Host "Serviço '$ServiceName' removido; a pasta $InstallPath ficou."
        }
        else { Write-Host "Serviço '$ServiceName' não existe." }
        return
    }

    # copia o build: espelha a pasta, sem apagar nem sobrescrever o appsettings.Production.json da máquina
    $staging = Join-Path ([IO.Path]::GetTempPath()) "$ServiceName-staging"
    if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
    Expand-Archive -Path $ZipPath -DestinationPath $staging
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null

    & robocopy $staging $InstallPath /MIR /XF appsettings.Production.json /NFL /NDL /NJH /NJS /NP | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "robocopy falhou (código $LASTEXITCODE)." }

    Remove-Item $staging -Recurse -Force
    Remove-Item $ZipPath -Force

    if (-not $service) {
        Write-Host "Criando o serviço '$ServiceName'..."
        $binaryPath = '"' + (Join-Path $InstallPath 'Worker.exe') + '"'

        if ($Credential) {
            New-Service -Name $ServiceName -BinaryPathName $binaryPath -DisplayName $DisplayName -StartupType Automatic -Credential $Credential | Out-Null
        }
        else {
            New-Service -Name $ServiceName -BinaryPathName $binaryPath -DisplayName $DisplayName -StartupType Automatic | Out-Null
            # a conta virtual "NT SERVICE\<serviço>": sem senha, só com o que se der a ela
            & sc.exe config $ServiceName obj= $Account | Out-Null
        }

        & sc.exe description $ServiceName 'Agenda dos jobs de integração a partir de auditing.Jobs (Quartz.NET).' | Out-Null
        & sc.exe config $ServiceName start= delayed-auto | Out-Null
        # o processo caiu: reinicia depois de 1 minuto, até três vezes por dia
        & sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null

        # leitura e execução da pasta para a conta do serviço
        & icacls $InstallPath /grant "$($Account):(OI)(CI)RX" /T /Q | Out-Null

        # a origem no Log de Eventos (o host escreve Warning para cima); criar a origem precisa de administrador
        if (-not [Diagnostics.EventLog]::SourceExists($ServiceName)) {
            [Diagnostics.EventLog]::CreateEventSource($ServiceName, 'Application')
        }
    }

    # o ambiente do serviço: qual appsettings.<Environment>.json é lido e o nome do serviço no host (Service:Name)
    Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\$ServiceName" -Name Environment -Type MultiString `
        -Value @("DOTNET_ENVIRONMENT=$Environment", "Service__Name=$ServiceName")

    Write-Host "Iniciando '$ServiceName'..."
    Start-Service -Name $ServiceName
    $service = Get-Service -Name $ServiceName
    Wait-Status $service 'Running' 60

    # um host que não sobe (configuração inválida, banco fora) para logo depois de iniciar
    Start-Sleep -Seconds 15
    $status = (Get-Service -Name $ServiceName).Status
    if ($status -ne 'Running') { throw "O serviço '$ServiceName' parou depois de iniciar ($status): veja o Log de Eventos (origem $ServiceName)." }

    Write-Host "Serviço '$ServiceName' rodando." -ForegroundColor Green
}

# ---------------------------------------------------------------------------------------------------------------------
# build e pacote
# ---------------------------------------------------------------------------------------------------------------------

$zipPath = $null

if (-not $Uninstall) {
    $project = Resolve-Path (Join-Path $PSScriptRoot '..\Worker.csproj')
    $workDir = Join-Path ([IO.Path]::GetTempPath()) "worker-publish-$profileName"
    $publishDir = Join-Path $workDir 'publish'
    $zipPath = Join-Path $workDir "$serviceName.zip"

    if (Test-Path $workDir) { Remove-Item $workDir -Recurse -Force }
    New-Item -ItemType Directory -Path $publishDir -Force | Out-Null

    Write-Host 'Build (dotnet publish)...' -ForegroundColor Gray

    $publishArgs = @('publish', $project, '-c', 'Release', '-o', $publishDir, '--nologo', '-v', 'q')

    if ($settings.SelfContained) {
        $publishArgs += @('-r', 'win-x64', '--self-contained', 'true')
    }

    $buildOutput = & dotnet @publishArgs 2>&1
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish falhou:`n$($buildOutput -join "`n")" }

    # o pacote nunca leva connection string nem appsettings de ambiente: a configuração da máquina fica na máquina
    Get-ChildItem $publishDir -Filter 'appsettings.*.json' | Remove-Item -Force

    if ((Get-Content (Join-Path $publishDir 'appsettings.json') -Raw) -match '"ConnectionStrings"') {
        throw 'O appsettings.json do build tem ConnectionStrings: a connection string fica na máquina, não no pacote.'
    }

    Compress-Archive -Path (Join-Path $publishDir '*') -DestinationPath $zipPath
    Write-Host ("Pacote: {0} ({1:N1} MB)." -f $zipPath, ((Get-Item $zipPath).Length / 1MB))
}

if ($DryRun) {
    $action = if ($Uninstall) { 'parar e remover o serviço' } else { 'parar o serviço, copiar o pacote para a pasta (sem o appsettings.Production.json de lá), criar o serviço se não existir, iniciar e conferir' }
    Write-Host "DryRun: $target, faria: $action. Nada foi alterado." -ForegroundColor Yellow
    return
}

# ---------------------------------------------------------------------------------------------------------------------
# na máquina de destino
# ---------------------------------------------------------------------------------------------------------------------

# conta de domínio: a senha só é pedida na primeira instalação
$credential = $null

if ($settings.ServiceAccount -and -not $Uninstall) {
    $exists = if ($settings.ComputerName) {
        Invoke-Command -ComputerName $settings.ComputerName -ScriptBlock { param($n) [bool](Get-Service -Name $n -ErrorAction SilentlyContinue) } -ArgumentList $serviceName
    }
    else { [bool](Get-Service -Name $serviceName -ErrorAction SilentlyContinue) }

    if (-not $exists) { $credential = Get-Credential -UserName $settings.ServiceAccount -Message "Senha da conta do serviço '$serviceName'" }
}

if ($settings.ComputerName) {
    $session = New-PSSession -ComputerName $settings.ComputerName

    try {
        $remoteZip = $null

        if ($zipPath) {
            $remoteTemp = Invoke-Command -Session $session -ScriptBlock { [IO.Path]::GetTempPath() }
            $remoteZip = Join-Path $remoteTemp (Split-Path $zipPath -Leaf)
            Copy-Item -Path $zipPath -Destination $remoteZip -ToSession $session -Force
        }

        Invoke-Command -Session $session -ScriptBlock $deploy -ArgumentList $remoteZip, $settings.InstallPath, $serviceName, $displayName, $account, $credential, $settings.Environment, [bool]$Uninstall
    }
    finally {
        Remove-PSSession $session
    }
}
else {
    $principal = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'Instalar ou atualizar o serviço precisa de um PowerShell como administrador.'
    }

    & $deploy $zipPath $settings.InstallPath $serviceName $displayName $account $credential $settings.Environment ([bool]$Uninstall)
}
