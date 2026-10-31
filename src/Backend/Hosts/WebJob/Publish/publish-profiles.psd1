# Perfis de publicação do host WebJob (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §6.1), lidos pelo publish-webjob.ps1.
# Só nomes de recursos da Azure, nenhum segredo: a autenticação é a do "az login" de quem publica (ou a identidade do
# pipeline). Este arquivo não entra no build nem no zip dos WebJobs.
@{
    # perfil usado quando o script roda sem -Profile
    DefaultProfile = 'dev'

    Profiles = @{
        dev = @{
            # vazio: a assinatura padrão do "az login"
            SubscriptionId = ''
            ResourceGroup  = 'rg-webjobs'
            AppServiceName = 'app-dev-webjobs-2'

            # 'Spaces' = "<Title> - V<versão>" (ex.: "LinxClientesFornec - V1.0.0"); 'Safe' = "<Title>-V<versão com _>"
            # (ex.: "LinxClientesFornec-V1_0_0"), o formato reserva caso a Azure não aceite espaço e ponto no nome (§6.3)
            NameFormat     = 'Spaces'

            # $false: o App Service precisa ter o runtime do .NET 10 (run.cmd chama "dotnet WebJob.dll");
            # $true: publica com o runtime junto (win-x64), zip maior por WebJob (run.cmd chama "WebJob.exe")
            SelfContained  = $false

            # cada IdJob vira um WebJob chamado "<Title> - V<versão>" (o título e a versão vêm do build: WebJob.dll --list-jobs).
            # Ex.: Jobs = @(8000, 3000, 8006)
            Jobs           = @()
        }

        prod = @{
            SubscriptionId = ''
            ResourceGroup  = ''
            AppServiceName = ''
            NameFormat     = 'Spaces'
            SelfContained  = $false
            Jobs           = @()
        }
    }
}
