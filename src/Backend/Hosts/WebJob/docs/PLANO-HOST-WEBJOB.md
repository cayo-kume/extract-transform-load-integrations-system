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
   um projeto compartilhado pelos hosts (`src/Backend/Hosts/Hosts.Scheduling`, uma class library). Não podem ir para
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

**Feito em 30/10/2026 (2.1 e 2.2).**

* **Projeto `src/Backend/Hosts/Hosts.Scheduling`** (class library, sem Hangfire), no `.slnx` e referenciado pelo WebApi:
  os mapas (`RecurringJobs/`, com `git mv`, namespace `Hosts.Scheduling.RecurringJobs`), o `JobCron`, o
  `ScheduledJobRunner` e o `AddScheduledJobRunner()`. O `RecurringJobDefinition` não depende mais do Hangfire: guarda o
  método compilado e o chama no escopo da execução (`Invoke`).
* **`ScheduledJobRunner`** (scoped): chama o método do mapa e, numa exceção, grava a mensagem, fecha e grava o log, e
  relança. Ele não cria escopo: o host cria um por execução (o Hangfire já faz isso), e o service e o `ILoggerService`
  vêm do mesmo escopo. Só grava com log aberto e ainda não gravado (`ILoggerService.HasUncommittedLog`, novo): um service
  que já gravou o log antes de relançar não ganha uma segunda linha, que a chave de `auditing.Logs` recusaria.
* **WebApi:** o Hangfire executa `WebApi.Scheduling.HangfireJobRunner.Run(idJob, nome)`, que repassa para o
  `ScheduledJobRunner`; o nome do recurring job aparece no dashboard (`[JobDisplayName("{1}")]`). Na subida, o log diz
  quantos jobs os mapas têm.
* O `WebJobExceptionHandlingMiddleware` do Shared ficou como estava: ninguém deste repositório o usa; sai quando o
  sistema antigo sair.

**Achado e corrigido no Shared:** a interface declara `AddMessage(message, sqlCommand, exceptionMessage,
stackTraceMessage)`, mas o `LoggerService` implementava `(message, exceptionMessage, stackTraceMessage, commandSQL)`.
Como todas as chamadas passam pela interface, os três textos chegavam trocados em `auditing.Messages` (o comando SQL em
`ExceptionMessage`, a mensagem da exceção em `ExceptionStackTrace` e a pilha em `SqlCommand`), nas rotas
(`ExceptionHandler`) e no DW. A ordem da classe passou a ser a da interface; os chamadores não mudam.

Conferido num banco de teste próprio na instância SQL Server do Docker (apagado no fim; o `Company` não foi tocado), com
os migrators de todos os módulos e o host com o Hangfire ligado:

* os mapas carregaram 327 jobs, e 3000, 8006 e 9002 entraram na agenda com os mesmos nomes de antes
  (`3000-MovideskGetTickets - V1.0.0`...);
* com `movidesk.MovideskParameters` renomeada só no banco de teste, o 3000 caiu logo depois de abrir o log: `Failed` no
  Hangfire, **uma** linha em `auditing.Logs` com `EndDate` e a mensagem em `auditing.Messages` com `IsError = 1` e cada
  texto na sua coluna (`SqlCommand` com o `SELECT`, `ExceptionMessage` com "Invalid object name", a pilha em
  `ExceptionStackTrace`);
* o 9002 rodou pelo mesmo caminho e terminou `Succeeded`;
* build sem erros e com os mesmos avisos.

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

**Feito em 30/10/2026 (3.1 e 3.2).** Saiu o `Worker.cs`; o `WebJob.csproj` referencia o `Hosts.Scheduling` e o
`Infrastructure` dos seis módulos; `Extensions/ServicesExtensions.cs` registra o Shared, os módulos (os mesmos do WebApi)
e o `ScheduledJobRunner`. O `appsettings.json` tem só o `Logging` e as seções dos módulos (`AfterSale`, `Movidesk`,
`LinxMicrovix` e `Totvs`, com as escritas da Totvs desligadas), sem `ConnectionStrings`: a connection string vem de
variável de ambiente (`ConnectionStrings__Connection`) ou de user-secrets (o `UserSecretsId` já existe). Build sem erros
e sem avisos no host; o host sobe e, em Development, a validação do DI na subida passa com todos os módulos.

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

**Feito em 30/10/2026 (4.1 e 4.2).**

* **`WebJob.Scheduling.JobScheduleWorker`** (`BackgroundService`): o processo roda o job de `--job <IdJob>` (seção
  `WebJob`: `TimeZone` `America/Sao_Paulo` e `RecheckIntervalInMinutes` 5). Sem `--job`, ou com um fuso inválido, o
  processo encerra com código 1; com um job fora dos mapas, avisa e fica parado (um WebJob contínuo que termina é
  reiniciado pela Azure em laço). A cada ciclo, ele lê a linha em `auditing.Jobs` (`IJobScheduleRepository.GetIntegrationJob`,
  novo), calcula o próximo horário, espera em pedaços de no máximo `RecheckIntervalInMinutes` e **relê a linha logo antes
  de rodar**: um job desligado durante a espera não roda nem no horário já calculado. O log registra o estado quando ele
  muda (ativo, inativo, sem agenda) e o próximo horário.
* **`JobCron.NextOccurrence`** (Hosts.Scheduling): o próximo horário pelas mesmas regras do cron do Hangfire, no fuso,
  sem pacote de cron (o Hangfire traz o dele embutido).
* **Jobs de procedures:** `RecurringJobDefinition.ForProcedure(idJob, procedure, versão)`, executada pelo
  `ICommandRepository` como `StoredProcedure`, e o `ProceduresRecurringJobs.cs` com as 33 procedures do sistema antigo
  comentadas (o `P_Volo_Integracao_Pedidos`, que roda uma vez por `EXECUTION`, anotado para virar método próprio), cada
  uma com o intervalo que tinha lá.

Conferido no banco de teste do Docker (apagado no fim):

* sem `--job`: mensagem crítica e código de saída 1; `--job 99999`: aviso e processo parado; `--job 9001` (diário):
  "todo dia às 23:00" e a próxima execução às 23:00 do mesmo dia;
* `--job 9002` com intervalo 1: rodou nos minutos cheios (21:28, 21:29...), uma linha em `auditing.Logs` por execução;
  desligado na tabela logo depois de uma execução, o horário seguinte não rodou e o log passou a "inativo" (na primeira
  versão, sem a releitura antes de rodar, ainda saía uma execução a mais; corrigido);
* `--job 3000` com `movidesk.MovideskParameters` renomeada: a execução falhou, o erro foi para `auditing.Logs` e
  `auditing.Messages` (cada texto na sua coluna) e o processo seguiu para o horário seguinte.

## Passo 5 — Concorrência

1. A trava é o `IJobLock` do Shared (`sp_getapplock` no banco das integrações), a mesma do WebApi e com as mesmas chaves
   (`job:<IdJob>` e `group:datawarehouse`). Um job nunca roda junto, nem entre um WebJob e o WebApi.
2. Ocupada: a execução é pulada, com o registro no log e o aviso depois de `MaxConsecutiveSkips` pulos seguidos, como no
   WebApi.
3. `settings.job` de cada WebJob com `"is_singleton": true`: se o App Service tiver mais de uma instância, o WebJob roda
   numa só.

**Feito em 30/10/2026.** O `JobScheduleWorker` pede a trava do `IJobLock` (Shared) com a chave do mapa
(`job:<IdJob>` ou `group:datawarehouse`, as mesmas do filtro do Hangfire) antes de cada execução e a solta no fim. Ocupada,
o horário é pulado sem esperar, com o registro no log e o aviso a cada `WebJob:MaxConsecutiveSkips` (5) pulos seguidos; o
contador é do processo e zera quando o job roda. O `settings.job` (`"is_singleton": true`) está na raiz do projeto e vai
para a saída do build, para entrar no zip de cada WebJob (fase 3).

Conferido no banco de teste do Docker (apagado no fim): com `group:datawarehouse` presa por outra sessão, o 9002 (a cada
minuto, `MaxConsecutiveSkips = 2` só no teste) pulou 21:36 e 21:37, com o aviso no segundo pulo; solta a trava, rodou às
21:38 e 21:39 (duas linhas em `auditing.Logs`), e nenhuma trava `APPLICATION` sobrou no banco. A chave é a mesma do
Hangfire, então a trava vale entre um WebJob e o WebApi.

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

### 6.5 Execução (feito em 30/10/2026, sem o primeiro deploy)

* **`Publish/publish-webjob.ps1`** (PowerShell 7) e **`Publish/publish-profiles.psd1`** (perfis `dev`, com o resource group
  e o App Service do sistema antigo, e `prod`, vazio; `Jobs` vazio nos dois até a decisão de quais jobs vão para cada
  ambiente). Parâmetros: `-PublishProfile` (alias `-Profile`), `-Jobs`, `-ListJobs`, `-RemoveMissing`, `-DryRun` (build e
  zips, sem chamar a Azure) e `-ProfilesFile` (outro arquivo de perfis, para pipeline ou teste).
* **Como publica:** um `dotnet publish` (Release); `WebJob.dll --list-jobs` dá o `IdJob`, o título e a versão de cada job;
  para cada `IdJob` do perfil, um zip com `App_Data/jobs/continuous/<nome>/` (o build, o `run.cmd` com `--job <IdJob>` e o
  `settings.job`) publicado com `az webapp deploy --type zip --clean false`, o mesmo comando que o sistema antigo usa em
  produção. **Mudança em relação ao 6.2:** no lugar do `PUT` na API do Kudu (autenticação não testada), o zip deploy já
  comprovado; o WebJob é substituído no lugar, sem apagar e recriar. Depois de publicar, o script apaga o WebJob da
  versão anterior do mesmo job.
* **Título no mapa:** `RecurringJobDefinition.Title`, tirado do `EnumJob` do módulo (o `Domain.<Módulo>` que o
  `Application.<Módulo>` referencia); o `RecurringJobMap` recusa um job sem título, como recusa um sem versão. O
  `ForProcedure` recebe o título. Os 327 títulos do `--list-jobs` são iguais aos `Title` dos scripts de `auditing.Jobs`.
* **No perfil:** `NameFormat` (`Spaces` = `<Title> - V<versão>`; `Safe` = `<Title>-V<versão com _>`, o formato reserva
  do 6.3) e `SelfContained` (`$false`: `run.cmd` chama `dotnet WebJob.dll` e o App Service precisa do runtime do
  .NET 10; `$true`: publica para `win-x64` com o runtime junto e chama `WebJob.exe`).

Conferido em modo simulação (`-DryRun`), com um arquivo de perfis de teste: `-ListJobs` mostrou
`LinxClientesFornec - V1.0.0`, `MovideskGetTickets - V1.0.0` e `DataWarehouseShortCycle - V1.0.0` (e
`LinxClientesFornec-V1_0_0` no formato `Safe`); a publicação simulada montou os três zips (15 MB cada, sem
`ConnectionStrings` no `appsettings.json` e sem os arquivos do script), com `run.cmd`, `settings.job` e a pasta
`App_Data/jobs/continuous/<nome>`; o WebJob extraído do zip subiu da própria pasta com `--job 8006`; job fora do perfil e
perfil inexistente param com a mensagem certa.

**Falta (precisa de um App Service de teste e do ok para publicar):** o primeiro deploy real, que confirma se a Azure
aceita espaço e ponto no nome (senão, `NameFormat = 'Safe'`), se o App Service tem o runtime do .NET 10 (senão,
`SelfContained = $true`) e a troca de versão apagando o WebJob anterior. Os zips têm cerca de 15 MB por WebJob: com
centenas de jobs no mesmo App Service, conferir o espaço em disco do plano.

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

### 7.4 Execução (feito em 30/10/2026, sem a Azure real)

* **Banco (Shared):** o índice `IX_Logs_Heartbeat` em `auditing.Logs (IdJob, IdType, EndDate)` no `CREATE` do Script001
  (script original); a view `auditing.vw_JobsHeartbeat` (`Script006_Create_Auditing_Views.sql`: por job de integração,
  a agenda, a última execução terminada e a execução aberta mais antiga, só dados); e o app 140 `Hosts` com o job
  **14000 `StuckJobsRecovery`** (`Script007_Insert_Hosts_Jobs_Into_Jobs_Table.sql`, a cada 15 minutos, `IsActive = 0`
  como todo job). `Domain.Shared.Enums.EnumJob` ganhou o `StuckJobsRecovery = 14000`. Num banco que já rodou o Script001,
  o índice entra à parte.
* **Shared:** `JobHeartbeat` e `IJobScheduleRepository.GetActiveJobsHeartbeat()`.
* **WebJob, `Recovery/StuckJobsRecovery.cs`:** `IStuckJobsRecovery.RecoverStuckJobs()` (`[JobVersion("1.0.0")]`), no
  log de `auditing` do job 14000. Lê o heartbeat; sem job off, registra isso e termina **sem chamar a Azure**; com algum
  off, lista os WebJobs e reinicia o do job (achado pelo título, em qualquer versão), com cooldown
  (`RestartCooldownMinutes`, 30), WebJob parado à mão não retomado e estado transitório esperando o próximo ciclo.
* **`Recovery/AzureWebJobsClient.cs`:** a API REST do Azure Resource Manager (listar, `stop` e `start` dos WebJobs
  contínuos) com o token do `DefaultAzureCredential` (`Azure.Identity` 1.21.0, já no cache local); sem o pacote
  `Azure.ResourceManager.AppService` do sistema antigo.
* **Só neste host:** o 14000 fica no mapa do WebJob (`WebJobRecurringJobs`), não no compartilhado. No Hangfire do
  WebApi, se ativo, o `SchedulerRefresh` avisa que ele não tem método no host. `RecurringJobMap.Validate` passou a ser
  público para o WebJob validar o mapa dele, e o `ScheduledJobRunner` ganhou o `Run(definição)`.
* **Seção `Recovery`:** `DryRun`, `TenantId`, `SubscriptionId`, `ResourceGroup`, `AppServiceName`, `NameFormat` (o mesmo
  do perfil de publicação), `GraceMinutes` (0), `RestartCooldownMinutes` (30), `Thresholds` (minutos por `IdJob`) e
  `IgnoredJobs`.

**A regra de off (decisão de 30/10/2026): 1 × o intervalo.** Off = job ativo sem execução terminada há mais que o
intervalo; no diário, sem execução terminada desde o último horário de `RunningDailyAt`. O limite é o intervalo +
`GraceMinutes`, que fica em **0**. Efeito conhecido: um job que está rodando normalmente na hora da checagem (a execução
de 3 minutos que começou às 10:06 ainda não terminou às 10:06:30, e a última terminada é das 10:03) aparece como off e
pode ser reiniciado no meio; o cooldown (`RestartCooldownMinutes`) segura um novo reinício do mesmo WebJob, e o
`Thresholds` por job dá limite próprio a quem demora. `GraceMinutes` continua configurável (o teste abaixo usou 5,
quando era o padrão). Um job ativo que **nunca** rodou fica fora da checagem até a primeira execução (um job recém-ligado
ainda não teve a vez dele).

Conferido no banco de teste do Docker (apagado no fim), com `Recovery:DryRun`: com o 3000 parado há 30 minutos, o 9002 e
o diário 9001 em dia e o 8006 ativo sem execução, a execução do 14000 marcou só o 3000 como off ("há mais de 8
minuto(s)") e registrou que não reiniciou nada; com `Thresholds:3000 = 60` e `DryRun` desligado, registrou "Nenhum job
com o heartbeat off" e não chamou a Azure. A view e o índice foram criados pelos migrators.

**Falta (precisa da Azure):** o reinício real, num App Service de teste, com a identidade gerenciada ligada e com
permissão nos WebJobs.

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
