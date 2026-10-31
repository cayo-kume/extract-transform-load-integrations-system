# Plano do host WebJob (antigo host AzureWebJob)

O host `src/Backend/Hosts/WebJob` (até o passo 1, `src/Backend/Hosts/AzureWebJob`) é hoje só o modelo
`dotnet new worker`: um `BackgroundService` que escreve no log a cada segundo, sem módulos, sem configuração e sem o
Azure WebJobs SDK. Este plano leva o projeto, passo a passo, a ser
o host que roda os jobs como **WebJobs na Azure**, no lugar do WebJob do sistema antigo
(`newbloomers/newbloomers-recurring-jobs/src/WebJob`). Ele segue o modelo do
[PLANO-HOST-WEBAPI.md](../../WebApi/docs/PLANO-HOST-WEBAPI.md) e reaproveita o que o host WebApi já construiu: a agenda
em `auditing.Jobs`, os mapas `IdJob → método`, a conversão em cron e a trava `IJobLock` do Shared. Cada passo é um
commit, e depois de cada um o host tem que compilar sem erros e com os mesmos avisos. Os testes de banco rodam num banco
de teste próprio na instância SQL Server do Docker (container `sql-server`), apagado no fim, nunca no LocalDB.

Situação em 30/10/2026:

| Item | Estado |
|---|---|
| Projeto | `AzureWebJob.csproj` (`Microsoft.NET.Sdk.Worker`, .NET 10), só com `Microsoft.Extensions.Hosting` |
| Código | `Program.cs` e `Worker.cs` do modelo; namespace `AzureWebJob` |
| Módulos | nenhum referenciado nem registrado |
| Configuração | `appsettings.json` e `appsettings.Development.json` só com `Logging`; `UserSecretsId` já existe no `.csproj` |
| Referências | `.slnx`, `README.md` da raiz (árvore e "worker alternativo (não usado)"), `PLANO-HOST-WEBAPI.md` e a proposta do Linx Microvix Outbound |

### O que o WebJob do sistema antigo faz (referência, só leitura)

| Item | No sistema antigo |
|---|---|
| Base | Azure WebJobs SDK 3.0 (`ConfigureWebJobs`, `AddAzureStorageCoreServices`, `AddTimers`), .NET 8 |
| Jobs | 17 classes em `RecurringJobs/` (por módulo e `Procedures/`), cerca de 130 métodos com `[TimerTrigger("<cron com segundos>")]`, a maioria a cada 3 minutos |
| Agenda | fixa no atributo de cada método; nada vem do banco |
| Publicação | um WebJob por nome na Azure, com um timer só: o `publish-webjob.ps1` comenta todos os métodos menos um (marcadores `//[JOB:Nome]`), faz um `dotnet publish` por método, apaga e recria o WebJob e restaura o código. Recursos da Azure passados na linha de comando |
| Erros | cada método roda dentro do `WebJobExceptionHandlingMiddleware` |
| Recuperação | `TotvsRecovery` → `AzureService.RecoveryTotvsJobs()`, a cada 5 min: lê a última execução dos jobs da Totvs em `auditing.Logs` e reinicia pela API da Azure os WebJobs parados há mais que o limite (com cooldown e respeitando parada manual) |
| Jobs de procedures | `Procedures/` (General, LinxMicrovix, Operacoes, Shippment): só executam procedures do banco (`P_Volo_Integracao_*`, `P_Canal_Movimentacao_*`, `P_Snapshot_Estoque`, `P_Server_Maintenance`...) |
| Fora deste repositório | Frete Rápido, o módulo Azure, as escritas recorrentes (ex.: `MovideskSendDuplicatesTotvs`) e as procedures acima |

---

## Fases de execução

| Fase | O que entrega | Passos | Depende de | Pronta quando |
|---|---|---|---|---|
| 1 — Preparação | o host renomeado e o que os dois hosts dividem num projeto compartilhado, com o executor que grava o log das execuções que falham (corrige também o Hangfire do WebApi) | 1 e 2 | — | o WebApi agenda os mesmos 327 jobs, com os mesmos nomes e versões, e um job que lança exceção grava o log com o erro |
| 2 — Host no local | o WebJob rodando um job por processo, com a agenda de `auditing.Jobs` e a trava | 3, 4 e 5 | fase 1 | `WebJob.exe --job <IdJob>` roda o job no banco de teste, relê a tabela a cada ciclo e pula quando a trava está ocupada |
| 3 — Publicação | o script e os perfis, um WebJob por nome na Azure com a versão no nome | 6 | fase 2 | um perfil de teste publicado; confirmado se a Azure aceita espaço e ponto no nome |
| 4 — Recuperação | `RecoverStuckJobs()` com o heartbeat e a identidade gerenciada | 7 | fase 3 (precisa dos WebJobs na Azure) | um WebJob parado é reiniciado só quando o heartbeat dele fica off |
| 5 — Documentação e o que entra depois | o README do host e os módulos e jobs que ainda faltam | 8 e 9 | o README ao fim de cada fase; os módulos conforme forem migrados | — |

A fase 1 vem antes do host porque o WebJob usa os mapas, o `JobCron` e o executor que hoje estão no WebApi; mudar de
lugar primeiro deixa o WebApi conferido antes de o WebJob depender disso.

---

# Fase 1 — Preparação

## Passo 1 — Renomear o projeto para WebJob

O projeto deixa de se chamar AzureWebJob: o nome segue o do WebApi (o que ele é, não onde roda).

1. Pasta `src/Backend/Hosts/AzureWebJob` → `src/Backend/Hosts/WebJob` (com `git mv`, para manter o histórico).
2. `AzureWebJob.csproj` → `WebJob.csproj`, e o caminho do projeto no `etl-integrations-system.slnx` (na ordem alfabética
   da pasta `Hosts`: Quartz, WebApi, WebJob).
3. Namespace `AzureWebJob` → `WebJob` em `Program.cs` e `Worker.cs` (o `Worker.cs` do modelo sai no passo 3; aqui só
   muda o nome).
4. Perfil `AzureWebJob` → `WebJob` no `Properties/launchSettings.json`. O `UserSecretsId`
   (`dotnet-AzureWebJob-...`) fica como está: trocar o id faria os segredos locais já gravados sumirem.
5. Atualizar as referências: a árvore e o texto "Os hosts `Quartz` e `AzureWebJob` não são usados" no `README.md` da
   raiz, as menções no `PLANO-HOST-WEBAPI.md` (§4.7) e na `PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md`. Registros de
   resultado já feitos ficam como estão, como no passo 1 do WebApi.
6. Fechar o Visual Studio antes (a pasta `.vs` fica presa) e apagar `bin/` e `obj/` da pasta antiga.

Conferência: build do `WebJob.csproj`, o host sobe e escreve no log como antes, e `grep` sem sobras de
`Hosts/AzureWebJob`, `AzureWebJob.csproj` e `namespace AzureWebJob`.

**Feito em 30/10/2026.** `git mv` da pasta (histórico preservado), `WebJob.csproj`, o `.slnx` (Quartz, WebApi, WebJob), o
namespace `WebJob` e o perfil `WebJob`; `bin/` e `obj/` antigos apagados; o `UserSecretsId` ficou como estava. Build sem
erros e sem avisos, e o host sobe e escreve no log como `WebJob.Worker`. Referências atualizadas no `README.md` da raiz
(o WebJob aparece como em construção), no `PLANO-HOST-WEBAPI.md` (§4.7) e na proposta do Linx Microvix Outbound; a
tabela "Situação em 30/10/2026" deste plano ficou como estava, por ser o retrato de antes.

## Passo 2 — Projeto compartilhado e executor de jobs

### 2.1 Projeto compartilhado

1. Os mapas `IdJob → método` (`Scheduling/RecurringJobs/`), o `JobCron` e o executor (2.2) saem do WebApi para
   um projeto compartilhado pelos hosts (ex.: `src/Backend/Hosts/Hosts.Scheduling`, uma class library). Não podem ir para
   o Shared: os mapas referenciam os services dos módulos, e os módulos referenciam o Shared.
2. O WebApi passa a referenciar esse projeto; o comportamento dele não muda (mesmos 327 jobs, mesma conferência).

### 2.2 Executor de jobs: o `WebJobExceptionHandlingMiddleware` é necessário?

O que ele faz: roda o job num `try/catch` e, se uma exceção escapa do service, chama o `IExceptionHandler` do Shared, que
grava a mensagem de erro (com o `SqlCommand` e a pilha) **no log da execução** (`ILoggerService`), fecha o log
(`SetLogEndDate`) e grava em `auditing.Logs` e `auditing.Messages` (`CommitAllChanges`).

**A função é necessária.** Os services abrem o log no começo (`AddLog`) e só gravam no fim. Vários métodos recorrentes não
têm `try/finally` em volta do corpo inteiro (ex.: `MovideskTicketsService.GetTickets`): se o banco ou a API derruba a
execução no meio, só quem está por fora grava o log com o erro. Sem isso, a execução que falhou não aparece em
`auditing.Logs` e o heartbeat (passo 7) e o painel do WebApp a veriam como "sem execução", não como "erro".

**A implementação atual não serve.** Ela foi feita a partir da versão de API: cria um `HttpContext` falso
(`DefaultHttpContext`) só para chamar o `HandleAsync`, que escreve uma resposta HTTP em JSON que ninguém lê. E recebe o
`IServiceProvider` de fora: se ele não for o mesmo escopo do service que rodou, o `ILoggerService` que grava o erro não é
o que abriu o log da execução (o erro iria para um log sem job). A conferir no protótipo.

Proposta:

1. Um executor de job no projeto compartilhado (2.1) (`ScheduledJobRunner`), que cria um escopo por execução,
   resolve o service e o `ILoggerService` **do mesmo escopo**, chama o método do mapa e, numa exceção, grava a mensagem,
   fecha e grava o log, como o `ExceptionHandler`, sem `HttpContext`.
2. O `WebJobExceptionHandlingMiddleware` do Shared sai (ninguém mais usa) ou fica só para o sistema antigo até ele sair.
3. **Vale também para o WebApi:** hoje, no Hangfire, uma exceção que escapa do service leva o job para `Failed` no
   dashboard, mas o log aberto da execução não é gravado em `auditing.Logs` (nas rotas HTTP, o
   `APIExceptionHandlingMiddleware` cobre). Os mapas do WebApi passam a chamar o mesmo executor.

Conferência: um job que lança exceção no meio grava uma linha em `auditing.Logs` com `EndDate` e a mensagem de erro em
`auditing.Messages` (`IsError`), nos dois hosts.

# Fase 2 — Host rodando no local

## Passo 3 — Base do host e módulos

### 3.1 Base

A agenda vem de `auditing.Jobs` (passo 4), e não de um `[TimerTrigger("<cron>")]` fixo numa classe. **Decisão
(30/10/2026): worker do .NET com agenda própria**, sem o Azure WebJobs SDK:

* roda como WebJob contínuo, do mesmo jeito que o do sistema antigo;
* não precisa da conta de storage da Azure (`AzureWebJobsStorage`), que o SDK usa para os locks e o histórico dos timers;
* a agenda é lida do banco, como no Hangfire do WebApi.

O SDK com um `TimerSchedule` próprio (`[TimerTrigger(typeof(...))]`) lendo a tabela foi descartado: manteria a conta de
storage e uma classe por job.

### 3.2 Módulos

1. Sai o `Worker.cs` do modelo.
2. Registro dos módulos igual ao do WebApi (`AddScopedSharedServices` e os `AddScoped<Módulo>Services` de Totvs,
   Movidesk, AfterSale, Linx Commerce, Linx Microvix e DW), com as referências no `.csproj` e as seções de configuração
   de cada módulo no `appsettings.json`.
3. Connection strings e credenciais por variável de ambiente (na Azure, as configurações do App Service) ou
   user-secrets no local, nunca no `appsettings.json` do git.

## Passo 4 — Agenda a partir de `auditing.Jobs`

O intervalo de cada job vem de `auditing.Jobs`, igual ao Hangfire: `IsActive`, `IntervalTimeInMinutes`,
`RunningDailyAt` e `Queue` (PLANO-HOST-WEBAPI.md §4.1). Nada de `[TimerTrigger]` com cron no código.

### 4.1 Um job por processo

Cada WebJob na Azure roda **um job só** (passo 6): o processo sobe com o job que vai rodar (`WebJob.exe --job 8000`).
Ele:

1. lê a linha do job em `auditing.Jobs` (`IdType = 2`) e calcula o próximo horário pelo `JobCron` (intervalo ou
   `RunningDailyAt`), no fuso configurado (`America/Sao_Paulo`, como no WebApi);
2. espera até o horário, roda o método do mapa pelo executor (2.2) com a trava (passo 5) e volta ao item 1;
3. relê a linha a cada ciclo: mudar `IntervalTimeInMinutes`, `RunningDailyAt` ou `IsActive` na tabela vale no próximo
   ciclo, sem republicar;
4. com `IsActive = 0`, não roda: fica esperando e relê a tabela de tempos em tempos (ex.: a cada 5 minutos);
5. job sem método no mapa ou com intervalo que não vira cron: o processo escreve o aviso no log e não roda.

### 4.2 Jobs de procedures

Os jobs que só executam procedures no banco ficam no projeto, **comentados**, até as procedures existirem neste
repositório:

1. Um segundo tipo de entrada no mapa, para procedure (ex.: `ProcedureJobDefinition.For(idJob, "[general].[p_Volo_Integracao_Pedidos]")`),
   executada pelo repositório do Shared (`ICommandRepository`), com o mesmo executor, a mesma trava e o mesmo log.
2. Um arquivo de mapa para eles (ex.: `ProceduresRecurringJobs.cs`), com as procedures do sistema antigo (`General`,
   `LinxMicrovix`, `Operacoes` e `Shippment`) comentadas, cada uma com o intervalo que tinha lá, como referência.
3. Ao descomentar uma, ela ganha uma linha em `auditing.Jobs` (com `RunningDailyAt` e `Queue`) e o `IdJob`
   correspondente no mapa. O caso de `P_Volo_Integracao_Pedidos`, que roda a procedure uma vez por `EXECUTION` de
   `GENERAL.IT4_WMS_DOCUMENTO_CARGA`, vira um método próprio, não uma entrada simples.
4. É o mesmo formato dos jobs do DW: um método que só chama procedures, na ordem, com o log de cada uma.

### 4.3 Divisão com o WebApi

**Decisão (30/10/2026): um host de agenda por vez.** O WebApi (com o Hangfire) e o WebJob ficam na mesma solution para a
equipe escolher como vai orquestrar os jobs, mas **um nunca está agendando ao mesmo tempo que o outro** no mesmo banco:
ou o Hangfire do WebApi (`Hangfire:Enabled = true`) ou os WebJobs publicados.

1. A coluna `Queue` não divide jobs entre os hosts: continua sendo só a fila do Hangfire (tudo em `default`).
2. A trava do passo 5 continua valendo entre os dois: se, numa troca de host, os dois ficarem ligados por um tempo, um
   job ainda nunca roda junto.
3. As rotas do WebApi continuam disponíveis para disparo manual com qualquer um dos dois agendando (fora da trava, como
   decidido no PLANO-HOST-WEBAPI.md §4.8).
4. O README dos dois hosts diz isso, com o que desligar de um lado antes de ligar o outro.

## Passo 5 — Concorrência

1. A trava é o `IJobLock` do Shared (`sp_getapplock` no banco das integrações), a mesma do WebApi e com as mesmas chaves
   (`job:<IdJob>` e `group:datawarehouse`). Um job nunca roda junto, nem entre um WebJob e o WebApi.
2. Ocupada: a execução é pulada, com o registro no log e o aviso depois de `MaxConsecutiveSkips` pulos seguidos, como no
   WebApi.
3. `settings.job` de cada WebJob com `"is_singleton": true`: se o App Service tiver mais de uma instância, o WebJob roda
   numa só.

# Fase 3 — Publicação

## Passo 6 — Publicação na Azure

A regra continua: **um WebJob por nome na Azure**, cada um com um job só.

### 6.1 Perfis de publicação num arquivo do script

Os recursos da Azure de cada ambiente ficam num arquivo de dados do próprio script, e não mais na linha de comando nem
no `appsettings.json`. O `appsettings.json` vai dentro do zip de cada WebJob e é configuração do job em execução; os
perfis só interessam a quem publica e não precisam ir para a Azure.

Os dois arquivos ficam juntos numa pasta do host:

```
WebJob/
└── Publish/
    ├── publish-webjob.ps1        o script
    └── publish-profiles.psd1     os perfis
```

O `publish-profiles.psd1` é um arquivo de dados do PowerShell, lido pelo script com `Import-PowerShellDataFile`, sem
pacote nenhum; ao contrário do JSON, aceita comentários:

```powershell
@{
    DefaultProfile = 'dev'

    Profiles = @{
        dev = @{
            SubscriptionId = '<id da assinatura>'
            ResourceGroup  = 'rg-webjobs'
            AppServiceName = 'app-dev-webjobs-2'
            WebJobType     = 'continuous'

            # cada IdJob vira um WebJob chamado "<Title> - V<versão>" (6.3)
            Jobs = @(8000, 3000, 8006)
        }

        prod = @{
            # ...
        }
    }
}
```

* Só nomes de recursos, nenhum segredo: a autenticação na Azure é a do `az login` de quem publica (ou a identidade do
  pipeline). Por isso o arquivo pode ficar no git.
* `Jobs` lista os `IdJob` do perfil: cada um vira um WebJob com o nome do 6.3.
* O arquivo fica fora do build: o `.csproj` não o copia para a saída, e ele não entra no zip dos WebJobs.

### 6.2 Script de publicação

O `publish-webjob.ps1` lê o `publish-profiles.psd1` da própria pasta e publica no perfil pedido (`-Profile prod`) ou no
`DefaultProfile`. Ele substitui o `publish-webjob.ps1` e o `publishing.md` do sistema antigo.

Forma proposta, melhor que a do sistema antigo:

| No sistema antigo | Proposta |
|---|---|
| um `dotnet publish` por método | **um `dotnet publish` só** |
| comenta e descomenta métodos no código-fonte antes de cada build e restaura no fim (um erro no meio deixa o código comentado) | **o código não é tocado**: o mesmo build serve para todos os WebJobs |
| o timer ativo é escolhido pelo que ficou descomentado | o job de cada WebJob é escolhido por um `run.cmd` na pasta do WebJob (`WebJob.exe --job 8000`) |
| apaga o WebJob e cria de novo | **substitui no lugar**, pela API do Kudu (`PUT /api/continuouswebjobs/<nome>` com o zip), sem janela sem WebJob |
| recursos na linha de comando | recursos no perfil do `publish-profiles.psd1` (6.1) |

Para cada `Jobs` do perfil: o script monta o zip (o build + `run.cmd` + `settings.job`), publica com o nome do WebJob e,
no fim, mostra o resumo (publicados, falhas). Opções:

* publicar só alguns jobs do perfil (`-Jobs 8000,3000`);
* `-ListJobs`: lista os WebJobs do perfil e os que existem no App Service;
* `-RemoveMissing`: apaga do App Service os WebJobs que saíram do perfil (desligado por padrão).

O script é em **PowerShell 7** (`pwsh`, roda em Windows e Linux), como o do sistema antigo; o `.psd1` é o formato de
dados nativo dele.

### 6.3 Nome do WebJob com a versão

Cada método de job recorrente tem a versão em `[JobVersion]` (PLANO-HOST-WEBAPI.md §4.10, já feito para o Hangfire), e
o nome do WebJob leva a versão: **`<Title> - V<versão>`**, ex.: `LinxClientesFornec - V1.0.0`. Assim, a lista de
WebJobs na Azure mostra que versão de cada job roda em produção, para comparar com a do código local.

1. `Title` é o nome do job no `EnumJob` do módulo (o mesmo de `auditing.Jobs`), e a versão vem do atributo; os dois saem
   do mapa compartilhado, sem consultar o banco. O script pergunta ao build (`WebJob.exe --list-jobs`, que imprime
   `IdJob`, `Title` e versão de cada job do mapa) e monta os nomes.
2. **Versão nova = WebJob novo:** como o nome muda, o script publica o WebJob com o nome novo e, depois de publicado,
   apaga o WebJob da versão anterior do mesmo job (mesmo `Title`, outra versão). Sem isso, as duas versões rodariam (a
   trava evita só que rodem juntas).
3. A recuperação (passo 7) acha o WebJob de cada job pelo mesmo nome, montado do mapa.
4. **A conferir no primeiro deploy:** o nome do WebJob vira pasta (`App_Data/jobs/continuous/<nome>`) e trecho de URL na
   API do Kudu, e não está confirmado se a Azure aceita espaço e ponto nele. Se não aceitar, o formato reserva é
   `LinxClientesFornec-V1_0_0`, montado no mesmo lugar (o script), sem mudar o resto.

### 6.4 Alternativa avaliada

Uma cópia só dos binários no App Service, com cada WebJob tendo só o `run.cmd` apontando para ela, deixaria cada WebJob
com poucos bytes. Mas atualizar os binários com os WebJobs rodando esbarra em arquivos presos, e todos os jobs mudariam
de versão juntos sem reinício. Fica o zip completo por WebJob.

# Fase 4 — Recuperação

## Passo 7 — Recuperação de jobs travados

A recuperação passa a valer para **todos os jobs**, não só para os da Totvs, e fica num arquivo próprio no host (ex.:
`Recovery/StuckJobsRecovery.cs`). O método muda de nome: `RecoveryTotvsJobs()` → **`RecoverStuckJobs()`**.

### 7.1 Só quando o heartbeat está off

No sistema antigo, a cada 5 minutos o `RecoveryTotvsJobs` lê os logs e chama a API da Azure. Agora, a API da Azure (a
parte cara: listar os WebJobs, reiniciar) só é chamada **quando o heartbeat de algum job está off**.

1. **Heartbeat** (decisão de 30/10/2026): off = **job ativo sem execução terminada há mais que 1 × o intervalo**
   (`IntervalTimeInMinutes`; num diário, sem execução terminada desde o último horário de `RunningDailyAt`). Uma view no
   Shared (ex.: `auditing.vw_JobsHeartbeat`) devolve, por job ativo, a última execução terminada e se há execução aberta;
   a regra de 1 × fica em quem lê. O painel do WebApp usa hoje 2 × na proposta (PLANO-WEBAPP.md §3.4, pergunta 1 em
   aberto); com a view, os dois podem usar o mesmo número.
   **Execuções longas:** os services de integração gravam o log só no fim da execução, então uma execução longa e
   saudável não aparece como aberta (ex.: `TotvsBalances`, que no sistema antigo levava horas rodando a cada 3 minutos).
   Pela regra de 1 ×, ela ficaria off e seria reiniciada no meio. Por isso (decisão de 30/10/2026), como no sistema
   antigo, a seção `Recovery` tem exceções por job, só para a recuperação (a agenda não muda): um limite próprio (`Recovery:Thresholds`, por
   `IdJob`, em minutos) e os jobs que a recuperação ignora (`Recovery:IgnoredJobs`). E, depois de um restart, o job tem o
   limite inteiro para terminar a primeira execução antes de ser reiniciado de novo.
2. **Checagem:** uma consulta só, nessa view, sobre os jobs ativos, **a cada 15 minutos** (decisão de 30/10/2026; a
   linha do `WJ-Recovery` em `auditing.Jobs` com `IntervalTimeInMinutes = 15`, que vira `*/15 * * * *`). Um job de 3
   minutos travado é percebido em até 15 minutos. Alguém precisa olhar o heartbeat; o que muda é que o
   olhar custa uma consulta indexada em `auditing.Logs`, e não chamadas à API da Azure.
3. **Recuperação:** só com algum heartbeat off, o `RecoverStuckJobs()` lista os WebJobs do App Service e reinicia os dos
   jobs parados, com as proteções do sistema antigo: cooldown entre reinícios, limite inteiro depois de um restart (o job
   só grava log no fim), WebJob parado à mão não é retomado e estado transitório espera o próximo ciclo.
4. Cada recuperação grava o seu próprio log em `auditing` (job novo em `auditing.Jobs`), com o que foi reiniciado e por
   quê.

### 7.2 Onde roda

Como qualquer job deste host: um WebJob com o nome próprio (ex.: `WJ-Recovery`), a checagem com o intervalo vindo da
sua linha em `auditing.Jobs`, e a configuração da Azure numa seção `Recovery` (assinatura, resource group, App Service,
limites e cooldown).

### 7.3 Autenticação na API da Azure

**Como é hoje no sistema antigo** (`Infrastructure.Azure/Clients/AzureWebJobClient.cs`): o cliente usa o Azure Resource
Manager (`https://management.azure.com`, pacotes `Azure.Identity` e `Azure.ResourceManager.AppService`) com um
`TokenCredential` escolhido assim:

1. com `TenantId`, `ClientId` e `ClientSecret` preenchidos na seção `Azure`: service principal (`ClientSecretCredential`);
2. senão: `DefaultAzureCredential`, com o `TenantId` fixado. Na Azure, ela usa a **identidade gerenciada** do App Service;
   na máquina do desenvolvedor, o login do Visual Studio ou do `az login`.

No `appsettings.json` do sistema antigo, `TenantId` está preenchido e `ClientId` e `ClientSecret` estão vazios: a
recuperação roda com o `DefaultAzureCredential`, ou seja, pela identidade gerenciada do App Service, sem segredo. Para
funcionar, essa identidade precisa de permissão no App Service (listar, parar e iniciar WebJobs); a atribuição de papel
fica na Azure e não aparece no código.

**Decisão (30/10/2026): só o `DefaultAzureCredential`**, com o `TenantId` da seção `Recovery`, sem a opção de segredo
(saem `ClientId` e `ClientSecret`). Na Azure, a identidade gerenciada do App Service; no local, o `az login`. Se um dia
precisar de service principal, ele entra por variável de ambiente (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`,
`AZURE_CLIENT_SECRET`), que o `DefaultAzureCredential` já lê, sem campo de segredo na configuração. A identidade
gerenciada precisa estar ligada no App Service e ter permissão de listar, parar e iniciar WebJobs nele.

# Fase 5 — Documentação e o que entra depois

## Passo 8 — Documentação do host

`README.md` do host: como rodar no local (`--job <IdJob>`) e na Azure, configuração, como a agenda funciona, como ligar
ou desligar um job em `auditing.Jobs`, a divisão com o WebApi, os perfis e o script de publicação, e a recuperação.

## Passo 9 — Módulos e jobs que ainda não estão no repositório

Frete Rápido, as escritas recorrentes do Movidesk e as procedures do passo 4.2 entram um a um, conforme forem migrados
(como no passo 6 do WebApi). O módulo Azure do sistema antigo não volta como módulo: a recuperação fica no host
(passo 7). Para cada um: o módulo, os jobs em `auditing.Jobs` (com `RunningDailyAt` e `Queue`), a entrada no mapa
compartilhado e o WebJob no perfil de publicação.

---

## Decisões

Tomadas em 30/10/2026:

1. Base do host: worker do .NET com agenda própria, sem o Azure WebJobs SDK (3.1).
2. Um host de agenda por vez: o WebApi (Hangfire) e o WebJob ficam na mesma solution, mas nunca agendam ao mesmo tempo;
   a coluna `Queue` não divide jobs entre eles (4.3).
3. Script de publicação em PowerShell 7, com os perfis no `publish-profiles.psd1` (6).
4. Heartbeat off = job ativo sem execução terminada há mais que 1 × o intervalo (7.1), com exceções por job só para a
   recuperação (limite próprio e jobs ignorados), por causa das execuções longas.
5. Checagem do heartbeat a cada 15 minutos (7.1).
6. Autenticação na API da Azure só com o `DefaultAzureCredential`, pela identidade gerenciada do App Service (7.3).
7. Nome do WebJob com a versão do método: `<Title> - V<versão>` (6.3); a confirmar no primeiro deploy se a Azure aceita
   espaço e ponto no nome.
