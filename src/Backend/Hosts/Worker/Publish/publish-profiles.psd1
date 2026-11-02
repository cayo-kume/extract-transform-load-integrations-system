# Perfis de publicação do host Worker (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md §6.1), lidos pelo publish-worker.ps1.
# Só nomes de máquina, pasta, serviço e conta, nenhum segredo: a connection string fica na máquina, no
# appsettings.Production.json da pasta do serviço (que o script nunca sobrescreve) ou em variável de ambiente do serviço.
# Este arquivo não entra no build nem na pasta publicada.
@{
    # perfil usado quando o script roda sem -Profile
    DefaultProfile = 'dev'

    Profiles = @{
        dev = @{
            # vazio: a máquina local; preenchido: a VM, por PowerShell Remoting (Enter-PSSession precisa funcionar nela)
            ComputerName   = ''

            # pasta do serviço na máquina de destino
            InstallPath    = 'C:\Services\EtlWorker'

            # nome do serviço (o mesmo Service:Name do host, que vira a origem no Log de Eventos) e o nome de exibição
            ServiceName    = 'EtlWorker'
            DisplayName    = 'ETL Integrations - Worker (Quartz)'

            # vazio: a conta virtual "NT SERVICE\<ServiceName>", sem senha; preenchido: uma conta de domínio (a senha é pedida
            # pelo Get-Credential na primeira instalação)
            ServiceAccount = ''

            # DOTNET_ENVIRONMENT do serviço: decide qual appsettings.<Environment>.json da pasta é lido
            Environment    = 'Production'

            # $true: publica para win-x64 com o runtime junto (a máquina não precisa do .NET 10); $false: precisa do runtime
            SelfContained  = $true
        }

        prod = @{
            ComputerName   = ''
            InstallPath    = 'C:\Services\EtlWorker'
            ServiceName    = 'EtlWorker'
            DisplayName    = 'ETL Integrations - Worker (Quartz)'
            ServiceAccount = ''
            Environment    = 'Production'
            SelfContained  = $true
        }
    }
}
