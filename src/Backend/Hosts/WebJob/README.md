# Host WebJob

Host que roda os jobs recorrentes como **WebJobs contínuos na Azure**, no lugar do WebJob do sistema antigo. Cada WebJob
é um processo deste projeto que roda **um job só** (`--job <IdJob>`), com a agenda lida de `auditing.Jobs`. Até
30/10/2026 o projeto se chamava `AzureWebJob`; as decisões e o histórico de cada passo estão em
[docs/PLANO-HOST-WEBJOB.md](docs/PLANO-HOST-WEBJOB.md).

| Item | Valor |
|---|---|
| Plataforma | .NET 10 (`net10.0`), worker (`Microsoft.NET.Sdk.Worker`), sem o Azure WebJobs SDK |
| Agenda | `auditing.Jobs`, relida a cada ciclo; nada de `[TimerTrigger]` no código |
| Um processo | um job (`--job <IdJob>`); na Azure, um WebJob por job, chamado `<Title> - V<versão>` |
| Publicação | `Publish/publish-webjob.ps1` com os perfis de `Publish/publish-profiles.psd1` |
| Recuperação | job 14000 `StuckJobsRecovery`, só neste host |

---

## Um host de agenda por vez

O WebApi (com o Hangfire) e o WebJob ficam na mesma solution para a equipe escolher como orquestrar os jobs, mas **nunca
agendam ao mesmo tempo** no mesmo banco: ou o Hangfire do WebApi, ou os WebJobs publicados.

* **Do WebApi para o WebJob:** `Hangfire:Enabled = false` no WebApi (as rotas continuam disponíveis para disparo manual)
  e só então publicar ou iniciar os WebJobs.
* **Do WebJob para o WebApi:** parar os WebJobs no App Service (ou tirá-los do perfil e publicar com `-RemoveMissing`) e
  só então ligar o Hangfire.

Se, numa troca, os dois ficarem ligados por um tempo, a trava (abaixo) ainda impede que um mesmo job rode junto nos dois.
A coluna `Queue` é só a fila do Hangfire: não divide jobs entre os hosts.

## Como rodar no local

O job vem da linha de comando. A connection string fica fora do `appsettings.json`: user-secrets (o projeto já tem o
`UserSecretsId`) ou variável de ambiente.

```bash
dotnet user-secrets set "ConnectionStrings:Connection" "<connection string>" --project src/Backend/Hosts/WebJob
```

```bash
dotnet run --project src/Backend/Hosts/WebJob -- --job 9002
```

Listar os jobs que o host conhece (IdJob, título e versão, separados por tabulação), sem subir o host nem acessar o banco:

```bash
dotnet run --project src/Backend/Hosts/WebJob -- --list-jobs
```

Sem `--job`, ou com um `TimeZone` inválido, o processo encerra com código 1. Com um job que não está nos mapas, ele avisa
no log e fica parado (na Azure, um WebJob contínuo que termina é reiniciado em laço). Testes de banco: num banco de teste
na instância SQL Server do Docker, nunca no LocalDB.

## Como roda na Azure

Cada WebJob publicado é uma pasta `App_Data/jobs/continuous/<nome>` no App Service, com o build, o `settings.job`
(`"is_singleton": true`: com mais de uma instância no App Service, ele roda numa só) e um `run.cmd` que chama o host com o
`--job` daquele WebJob (`dotnet WebJob.dll --job 8006`, ou `WebJob.exe --job 8006` com `SelfContained`).

O zip não leva `ConnectionStrings`: elas ficam nas configurações do App Service (Connection strings ou Application
settings, ex.: `ConnectionStrings__Connection`) e valem para todos os WebJobs dele. O mesmo vale para qualquer
parâmetro que mude por ambiente (ex.: `Recovery__AppServiceName`).

## Estrutura

```
WebJob/
├── Program.cs                      --list-jobs, --job e o JobScheduleWorker
├── Extensions/ServicesExtensions.cs   registro dos módulos, do executor e da recuperação
├── Scheduling/
│   ├── WebJobSettings.cs           seção WebJob
│   ├── JobScheduleWorker.cs        a agenda do processo: lê auditing.Jobs, espera, trava e roda
│   └── WebJobRecurringJobs.cs      os mapas compartilhados mais os jobs só deste host (14000)
├── Recovery/
│   ├── RecoverySettings.cs         seção Recovery
│   ├── AzureWebJobsClient.cs       API do Azure Resource Manager: listar, parar e iniciar WebJobs
│   └── StuckJobsRecovery.cs        RecoverStuckJobs(): heartbeat e reinício
├── Publish/
│   ├── publish-webjob.ps1          script de publicação
│   └── publish-profiles.psd1       perfis (dev, prod)
├── settings.job
├── appsettings.json
└── docs/PLANO-HOST-WEBJOB.md
```

Os mapas `IdJob → método` (`RecurringJobs/`), o `JobCron` e o `ScheduledJobRunner` ficam no projeto
`src/Backend/Hosts/Hosts.Scheduling`, que o WebJob e o WebApi dividem.

## Configuração

| Seção | Uso |
|---|---|
| `ConnectionStrings:Connection` | banco das integrações (módulos, `auditing` e a trava de concorrência) |
| `ConnectionStrings:AuditingConnection` | opcional: banco de `auditing`. Sem ela, o Shared usa a `Connection` trocando `NEWBLOOMERS` por `AUDITING` no nome |
| `WebJob` | a agenda do processo (tabela abaixo) |
| `Recovery` | a recuperação dos WebJobs travados (ver [Recuperação](#recuperação-de-jobs-travados)) |
| `AfterSale`, `Movidesk`, `LinxMicrovix`, `Totvs`... | `BaseUrl` e as opções de cada módulo (ver o README do módulo) |

Seção `WebJob`:

| Parâmetro | Valor | Uso |
|---|---|---|
| `IdJob` | — | o job do processo; vem de `--job` |
| `TimeZone` | `America/Sao_Paulo` | fuso da agenda (intervalos e `RunningDailyAt`), o mesmo do Hangfire do WebApi |
| `RecheckIntervalInMinutes` | `5` | de quanto em quanto tempo o processo relê `auditing.Jobs` enquanto espera |
| `MaxConsecutiveSkips` | `5` | pulos seguidos (trava ocupada) até o aviso no log |

## Agenda

A agenda vem de `auditing.Jobs`, com as mesmas colunas e regras do Hangfire do WebApi:

| Coluna | Uso |
|---|---|
| `IsActive` | só com 1 o job roda. Nasce 0 em todo insert: nada roda sem um `UPDATE` |
| `IntervalTimeInMinutes` | intervalo; 0 = sob demanda, não roda |
| `RunningDailyAt` | preenchida, o job é diário e roda nessa hora (só com intervalo 1440). Os diários dos scripts estão às 23:00 |
| `Queue` | não usada neste host (é a fila do Hangfire) |

Os horários seguem as regras do cron do WebApi, no fuso de `WebJob:TimeZone`: 3 roda nos minutos múltiplos de 3, 60 no
minuto 0 de cada hora, 120 a cada 2 horas cheias, e um diário na hora de `RunningDailyAt`. Um intervalo que não divide a
hora nem o dia fica sem agenda, com aviso no log.

A cada ciclo, o processo:

1. lê a linha do job em `auditing.Jobs` (`IdType = 2`) e calcula o próximo horário;
2. espera, em pedaços de no máximo `RecheckIntervalInMinutes`;
3. relê a linha logo antes de rodar: um job desligado durante a espera não roda nem no horário já calculado;
4. pede a trava e roda o método do mapa pelo `ScheduledJobRunner`, que grava em `auditing.Logs` e `auditing.Messages`
   (inclusive o erro, quando a execução falha) e volta ao item 1.

Uma mudança na tabela vale, no máximo, depois de `RecheckIntervalInMinutes` ou no horário seguinte, sem republicar.

```sql
-- liga um job
UPDATE [auditing].[Jobs] SET IsActive = 1 WHERE IdType = 2 AND IdJob = 3000;

-- muda o intervalo
UPDATE [auditing].[Jobs] SET IntervalTimeInMinutes = 15 WHERE IdType = 2 AND IdJob = 3000;

-- muda a hora de um diário
UPDATE [auditing].[Jobs] SET RunningDailyAt = '02:30' WHERE IdType = 2 AND IdJob = 9001;

-- desliga (o WebJob continua no ar, esperando)
UPDATE [auditing].[Jobs] SET IsActive = 0 WHERE IdType = 2 AND IdJob = 3000;
```

Um job ativo só roda se tiver WebJob publicado: ligar na tabela não cria o WebJob, e um WebJob publicado de um job
inativo fica só esperando.

Os jobs de procedures (`ProceduresRecurringJobs.cs` em `Hosts.Scheduling`) estão comentados até as procedures existirem
neste repositório; ao descomentar um, ele precisa da linha em `auditing.Jobs` e do WebJob no perfil.

## Concorrência

Antes de cada execução, o processo pede a trava do `IJobLock` do Shared (`sp_getapplock` no banco das integrações, sem
esperar), com as mesmas chaves do Hangfire do WebApi: `job:<IdJob>`, e `group:datawarehouse` para os jobs do DW, que
escrevem nas mesmas landings. Ocupada, o horário é pulado com o registro no log, e depois de `MaxConsecutiveSkips` pulos
seguidos sai um aviso: o job está lento demais para o intervalo. Como a chave é a mesma, um job nunca roda junto nem
entre um WebJob e o WebApi.

## Versão dos jobs

Cada método chamado por um job recorrente tem `[JobVersion("MAJOR.MINOR.PATCH")]` no método da classe do service, e a
versão vai no nome do WebJob: `LinxClientesFornec - V1.0.0`. Ao mudar o código de um job, mude a versão no mesmo método
(PATCH numa correção, MINOR numa mudança compatível, MAJOR quando muda o que o job grava de um jeito que quebra quem lê) e
republique o job: o script cria o WebJob da versão nova e apaga o da anterior. Um método do mapa sem o atributo, ou um job
sem título, impede o host de subir.

## Publicação

O `Publish/publish-webjob.ps1` (PowerShell 7 e Azure CLI, com `az login`) publica um WebJob contínuo por job do perfil.

**Perfis** (`Publish/publish-profiles.psd1`, só nomes de recursos, nenhum segredo; o arquivo não vai para o zip):

| Chave | Uso |
|---|---|
| `DefaultProfile` | perfil usado sem `-Profile` (`dev`) |
| `SubscriptionId` | assinatura; vazio = a padrão do `az login` |
| `ResourceGroup`, `AppServiceName` | onde ficam os WebJobs |
| `NameFormat` | `Spaces` = `<Title> - V<versão>`; `Safe` = `<Title>-V<versão com _>` (ex.: `LinxClientesFornec-V1_0_0`), o formato reserva se a Azure não aceitar espaço e ponto no nome |
| `SelfContained` | `$false`: o App Service precisa do runtime do .NET 10; `$true`: publica para `win-x64` com o runtime junto (zip maior) |
| `Jobs` | os `IdJob` que viram WebJob neste ambiente (ex.: `@(8000, 3000, 8006)`) |

**Como publica:** um `dotnet publish` (Release); `WebJob.dll --list-jobs` dá o título e a versão de cada job; para cada
`IdJob` do perfil, um zip com `App_Data/jobs/continuous/<nome>` (o build, o `run.cmd` e o `settings.job`), publicado com
`az webapp deploy --type zip --clean false`. O WebJob é substituído no lugar, e o da versão anterior do mesmo job é
apagado.

```powershell
# o que o perfil publicaria e os WebJobs que já existem no App Service, sem publicar
./src/Backend/Hosts/WebJob/Publish/publish-webjob.ps1 -Profile dev -ListJobs

# build e zips, sem chamar a Azure
./src/Backend/Hosts/WebJob/Publish/publish-webjob.ps1 -Profile dev -DryRun

# publica todos os jobs do perfil, ou só alguns
./src/Backend/Hosts/WebJob/Publish/publish-webjob.ps1 -Profile prod
./src/Backend/Hosts/WebJob/Publish/publish-webjob.ps1 -Profile prod -Jobs 8006,3000

# publica e apaga os WebJobs deste host cujo job saiu do perfil
./src/Backend/Hosts/WebJob/Publish/publish-webjob.ps1 -Profile prod -RemoveMissing
```

Outros parâmetros: `-ProfilesFile` (outro arquivo de perfis, para pipeline ou teste).

**Ainda não conferido numa publicação real:** se a Azure aceita espaço e ponto no nome (senão, `NameFormat = 'Safe'`),
se o App Service tem o runtime do .NET 10 (senão, `SelfContained = $true`) e a troca de versão. Cada zip tem cerca de
15 MB: com muitos jobs no mesmo App Service, conferir o espaço em disco do plano.

## Recuperação de jobs travados

O job **14000 `StuckJobsRecovery`** (app 140 `Hosts`, a cada 15 minutos) roda o `RecoverStuckJobs()` num WebJob próprio,
só neste host (no Hangfire do WebApi ele não tem método). Ele vale para todos os jobs ativos e só chama a Azure quando
algum está com o **heartbeat off**:

1. lê `auditing.vw_JobsHeartbeat` (uma consulta, apoiada no índice `IX_Logs_Heartbeat`);
2. **off** = job ativo sem execução terminada há mais que 1 × o intervalo (`IntervalTimeInMinutes` + `GraceMinutes`, que
   é 0); num diário, sem execução terminada desde o último horário de `RunningDailyAt`. Um job que nunca rodou fica fora
   até a primeira execução;
3. nenhum off: registra isso no log e termina, sem chamar a Azure;
4. algum off: lista os WebJobs do App Service e reinicia (stop e start) o do job, achado pelo título em qualquer versão,
   com as proteções do sistema antigo: um WebJob reiniciado não é reiniciado de novo antes de `RestartCooldownMinutes`, um
   WebJob parado à mão não é retomado e um em estado transitório espera o próximo ciclo.

Tudo vai para o log do job 14000 em `auditing`: os jobs off, o motivo e o que foi reiniciado.

**Execuções longas:** os services gravam o log só no fim, então uma execução longa e saudável aparece como off e seria
reiniciada no meio (também uma de 3 minutos que está rodando na hora da checagem). Para esses jobs, um limite próprio em
`Recovery:Thresholds` ou a exclusão em `Recovery:IgnoredJobs`.

Seção `Recovery`:

| Parâmetro | Valor | Uso |
|---|---|---|
| `DryRun` | `false` | `true`: registra no log o que reiniciaria, sem chamar a Azure |
| `TenantId`, `SubscriptionId`, `ResourceGroup`, `AppServiceName` | vazios | onde estão os WebJobs |
| `NameFormat` | `Spaces` | o mesmo do perfil de publicação |
| `GraceMinutes` | `0` | folga somada ao intervalo (0 = a regra de 1 × o intervalo) |
| `RestartCooldownMinutes` | `30` | espera mínima entre dois reinícios do mesmo WebJob, e o tempo que ele tem para terminar a primeira execução |
| `Thresholds` | `{}` | limite próprio por `IdJob`, em minutos (ex.: `{ "13017": 240 }`), no lugar do intervalo |
| `IgnoredJobs` | `[]` | `IdJob` que a recuperação nunca reinicia |

**Autenticação na Azure:** só o `DefaultAzureCredential`, sem segredo na configuração. Na Azure, a **identidade
gerenciada** do App Service, que precisa estar ligada e ter permissão de listar, parar e iniciar os WebJobs dele; no
local, o `az login`. Um service principal, se um dia precisar, entra pelas variáveis `AZURE_CLIENT_ID`,
`AZURE_TENANT_ID` e `AZURE_CLIENT_SECRET`, que o `DefaultAzureCredential` já lê.

Para ligar: `UPDATE [auditing].[Jobs] SET IsActive = 1 WHERE IdType = 2 AND IdJob = 14000;` e o 14000 no `Jobs` do
perfil. Para começar só observando, `Recovery__DryRun=true` no App Service.

**Ainda não conferido na Azure real:** o reinício, num App Service de teste, com a identidade gerenciada e a permissão.

## Um job ou um módulo novo

* **Job recorrente novo num módulo do host:** a linha no script de jobs da service (com `RunningDailyAt` e `Queue`, ver
  [PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](../../../../docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md) §3.1), a entrada no
  mapa do módulo em `src/Backend/Hosts/Hosts.Scheduling/RecurringJobs/<Módulo>RecurringJobs.cs`, `[JobVersion("1.0.0")]`
  no método da classe do service e o `IdJob` no `Jobs` do perfil de publicação.
* **Módulo novo:** referência no `WebJob.csproj` e registro no `ServicesExtensions` (também no WebApi), o mapa
  `<Módulo>RecurringJobs.cs` incluído no `RecurringJobMap` e o script de jobs com as colunas novas.
* **Job só deste host** (como o 14000): a entrada em `Scheduling/WebJobRecurringJobs.cs`.
