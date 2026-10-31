#Requires -Version 7
<#
.SYNOPSIS
    Publica os jobs do host WebJob na Azure: um WebJob contínuo por job, chamado "<Title> - V<versão>"
    (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md, passo 6).

.DESCRIPTION
    Lê o perfil em publish-profiles.psd1 (na mesma pasta), faz um "dotnet publish" só e, para cada IdJob do perfil, monta um
    zip com o build, o run.cmd ("--job <IdJob>") e o settings.job em App_Data/jobs/continuous/<nome>, e publica com
    "az webapp deploy --type zip --clean false": o WebJob é substituído no lugar, sem apagar e recriar. O código-fonte não é
    tocado. Com uma versão nova, o nome muda: depois de publicar, o script apaga o WebJob da versão anterior do mesmo job.

.PARAMETER PublishProfile
    Perfil do arquivo de perfis (alias -Profile); sem ele, o DefaultProfile.

.PARAMETER ProfilesFile
    Arquivo de perfis; sem ele, o publish-profiles.psd1 desta pasta.

.PARAMETER Jobs
    Publica só estes IdJob do perfil (ex.: -Jobs 8000,3000).

.PARAMETER ListJobs
    Lista os jobs do perfil (IdJob, título, versão, nome do WebJob) e os WebJobs que existem no App Service, e não publica.

.PARAMETER RemoveMissing
    Depois de publicar, apaga do App Service os WebJobs deste host (nome "<Title> - V<versão>" de um job dos mapas) cujo job
    saiu do perfil. Desligado por padrão.

.PARAMETER DryRun
    Faz o build e monta os zips, sem chamar a Azure; mostra onde ficaram os zips.

.EXAMPLE
    ./publish-webjob.ps1 -Profile dev -ListJobs

.EXAMPLE
    ./publish-webjob.ps1 -Profile prod -Jobs 8006
#>
[CmdletBinding()]
param(
    [Alias('Profile')]
    [string]$PublishProfile,
    [string]$ProfilesFile,
    [int[]]$Jobs,
    [switch]$ListJobs,
    [switch]$RemoveMissing,
    [switch]$DryRun
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

foreach ($key in 'ResourceGroup', 'AppServiceName') {
    if (-not $settings[$key] -and -not $DryRun) {
        throw "Perfil '$profileName': $key está vazio em $profilesPath."
    }
}

$nameFormat = if ($settings.NameFormat) { $settings.NameFormat } else { 'Spaces' }

if ($nameFormat -notin 'Spaces', 'Safe') {
    throw "Perfil '$profileName': NameFormat '$nameFormat' inválido (use 'Spaces' ou 'Safe')."
}

$azArgs = @('--resource-group', $settings.ResourceGroup, '--name', $settings.AppServiceName)

if ($settings.SubscriptionId) {
    $azArgs += @('--subscription', $settings.SubscriptionId)
}

# ---------------------------------------------------------------------------------------------------------------------
# nome do WebJob: "<Title> - V<versão>" ou, no formato reserva, "<Title>-V<versão com _>"
# ---------------------------------------------------------------------------------------------------------------------

function Get-WebJobName([string]$title, [string]$version) {
    if ($nameFormat -eq 'Safe') { return "$title-V$($version -replace '\.', '_')" }
    return "$title - V$version"
}

function Test-WebJobOfTitle([string]$webJobName, [string]$title) {
    $escaped = [regex]::Escape($title)
    if ($nameFormat -eq 'Safe') { return $webJobName -match "^$escaped-V\d+_\d+_\d+$" }
    return $webJobName -match "^$escaped - V\d+\.\d+\.\d+$"
}

function Invoke-Az([string[]]$arguments) {
    $output = & az @arguments 2>&1
    if ($LASTEXITCODE -ne 0) { throw "az $($arguments[0..2] -join ' ') falhou: $output" }
    return $output
}

function Get-ExistingWebJobs {
    # a Azure devolve "<app service>/<webjob>"
    $names = Invoke-Az (@('webapp', 'webjob', 'continuous', 'list') + $azArgs + @('--query', '[].name', '-o', 'tsv'))
    return @($names | Where-Object { $_ } | ForEach-Object { ($_ -split '/', 2)[-1].Trim() })
}

function Remove-WebJob([string]$webJobName) {
    Invoke-Az (@('webapp', 'webjob', 'continuous', 'remove') + $azArgs + @('--webjob-name', $webJobName)) | Out-Null
}

# ---------------------------------------------------------------------------------------------------------------------
# build: um "dotnet publish" só, para todos os WebJobs
# ---------------------------------------------------------------------------------------------------------------------

$project = Resolve-Path (Join-Path $PSScriptRoot '..\WebJob.csproj')
$workDir = Join-Path ([IO.Path]::GetTempPath()) "webjob-publish-$profileName"
$publishDir = Join-Path $workDir 'publish'
$zipsDir = Join-Path $workDir 'zips'

if (Test-Path $workDir) { Remove-Item $workDir -Recurse -Force }
New-Item -ItemType Directory -Path $publishDir, $zipsDir -Force | Out-Null

Write-Host "Perfil '$profileName': $($settings.AppServiceName) ($($settings.ResourceGroup)), nomes '$nameFormat'." -ForegroundColor Cyan
Write-Host "Build (dotnet publish)..." -ForegroundColor Gray

$publishArgs = @('publish', $project, '-c', 'Release', '-o', $publishDir, '--nologo', '-v', 'q')

if ($settings.SelfContained) {
    $publishArgs += @('-r', 'win-x64', '--self-contained', 'true')
}

$buildOutput = & dotnet @publishArgs 2>&1
if ($LASTEXITCODE -ne 0) { throw "dotnet publish falhou:`n$($buildOutput -join "`n")" }

# título e versão de cada job, do próprio build (sem banco)
$catalog = @{}
$listed = & dotnet (Join-Path $publishDir 'WebJob.dll') --list-jobs 2>&1
if ($LASTEXITCODE -ne 0) { throw "WebJob.dll --list-jobs falhou:`n$($listed -join "`n")" }

foreach ($line in $listed) {
    $parts = "$line" -split "`t"
    if ($parts.Count -eq 3) {
        $catalog[[int]$parts[0]] = [pscustomobject]@{ IdJob = [int]$parts[0]; Title = $parts[1]; Version = $parts[2] }
    }
}

$profileJobs = @($settings.Jobs)
$targets = if ($Jobs) { @($Jobs) } else { $profileJobs }

foreach ($id in $targets) {
    if (-not $catalog.ContainsKey([int]$id)) { throw "IdJob $id não está nos mapas de jobs recorrentes deste build." }
    if ($Jobs -and $id -notin $profileJobs) { throw "IdJob $id não está no perfil '$profileName'." }
}

# ---------------------------------------------------------------------------------------------------------------------
# -ListJobs
# ---------------------------------------------------------------------------------------------------------------------

if (-not $DryRun) {
    $null = az account show -o none 2>&1
    if ($LASTEXITCODE -ne 0) { throw 'Sem login na Azure CLI: rode "az login".' }
}

$existing = if ($DryRun) { @() } else { Get-ExistingWebJobs }

if ($ListJobs) {
    $profileJobs | ForEach-Object {
        $job = $catalog[[int]$_]
        $name = Get-WebJobName $job.Title $job.Version
        [pscustomobject]@{
            IdJob   = $job.IdJob
            Title   = $job.Title
            Versao  = $job.Version
            WebJob  = $name
            NaAzure = if ($DryRun) { '(sem consulta)' } elseif ($name -in $existing) { 'sim' } else { 'não' }
        }
    } | Format-Table -AutoSize

    if (-not $DryRun) {
        $others = $existing | Where-Object { $name = $_; -not ($profileJobs | Where-Object { $name -eq (Get-WebJobName $catalog[[int]$_].Title $catalog[[int]$_].Version) }) }
        if ($others) {
            Write-Host 'WebJobs no App Service fora do perfil (ou de outra versão):' -ForegroundColor Yellow
            $others | ForEach-Object { Write-Host "  $_" }
        }
    }

    return
}

# ---------------------------------------------------------------------------------------------------------------------
# publicação: um zip por WebJob
# ---------------------------------------------------------------------------------------------------------------------

$runCommand = if ($settings.SelfContained) { 'WebJob.exe' } else { 'dotnet WebJob.dll' }
$results = @()

foreach ($id in $targets) {
    $job = $catalog[[int]$id]
    $name = Get-WebJobName $job.Title $job.Version

    try {
        $stagingDir = Join-Path $workDir "staging\$id"
        $webJobDir = Join-Path $stagingDir "App_Data\jobs\continuous\$name"
        New-Item -ItemType Directory -Path $webJobDir -Force | Out-Null
        Copy-Item -Path (Join-Path $publishDir '*') -Destination $webJobDir -Recurse -Force

        # o job deste WebJob; o settings.job (is_singleton) já vem do build
        Set-Content -Path (Join-Path $webJobDir 'run.cmd') -Value "@echo off`r`n$runCommand --job $id" -Encoding ascii -NoNewline

        $zip = Join-Path $zipsDir "$id.zip"
        Compress-Archive -Path (Join-Path $stagingDir '*') -DestinationPath $zip -Force

        if ($DryRun) {
            $results += [pscustomobject]@{ IdJob = $id; WebJob = $name; Resultado = "zip: $zip"; Removidos = '' }
            continue
        }

        Write-Host "Publicando '$name'..." -ForegroundColor Gray
        Invoke-Az (@('webapp', 'deploy') + $azArgs + @('--src-path', $zip, '--type', 'zip', '--clean', 'false', '-o', 'none')) | Out-Null

        # versão nova = WebJob novo: a anterior do mesmo job sai depois de a nova estar publicada
        $old = @($existing | Where-Object { $_ -ne $name -and (Test-WebJobOfTitle $_ $job.Title) })
        $old | ForEach-Object { Remove-WebJob $_ }

        $results += [pscustomobject]@{ IdJob = $id; WebJob = $name; Resultado = 'publicado'; Removidos = ($old -join ', ') }
    }
    catch {
        $results += [pscustomobject]@{ IdJob = $id; WebJob = $name; Resultado = "falhou: $($_.Exception.Message)"; Removidos = '' }
    }
}

# ---------------------------------------------------------------------------------------------------------------------
# -RemoveMissing: WebJobs deste host (nome de um job dos mapas) cujo job saiu do perfil
# ---------------------------------------------------------------------------------------------------------------------

if ($RemoveMissing -and -not $DryRun) {
    $kept = $profileJobs | ForEach-Object { $catalog[[int]$_].Title }
    $removable = $existing | Where-Object {
        $webJob = $_
        $ofTitle = $catalog.Values | Where-Object { Test-WebJobOfTitle $webJob $_.Title } | Select-Object -First 1
        $ofTitle -and $ofTitle.Title -notin $kept
    }

    foreach ($webJob in $removable) {
        Remove-WebJob $webJob
        $results += [pscustomobject]@{ IdJob = ''; WebJob = $webJob; Resultado = 'removido (fora do perfil)'; Removidos = '' }
    }
}

$results | Format-Table -AutoSize -Wrap

if ($DryRun) { Write-Host "Simulação: nada foi enviado à Azure. Zips em $zipsDir." -ForegroundColor Yellow }

if ($results | Where-Object { $_.Resultado -like 'falhou*' }) { exit 1 }
