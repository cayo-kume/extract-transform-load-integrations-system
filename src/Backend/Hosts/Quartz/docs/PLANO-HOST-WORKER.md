# Plano do host Worker (antigo host Quartz)

O host `src/Backend/Hosts/Quartz` (depois do passo 1, `src/Backend/Hosts/Worker`) é hoje só o modelo
`dotnet new worker`: um `BackgroundService` que escreve no log a cada segundo, sem módulos, sem configuração e sem a
biblioteca Quartz.NET (o nome do projeto é o único "Quartz" que ele tem). Este plano leva o projeto a ser a **terceira
opção de orquestração** dos jobs recorrentes, ao lado do Hangfire do WebApi e dos WebJobs: um processo só, que roda todos
os jobs ativos de `auditing.Jobs` com o agendador **Quartz.NET**.

O plano segue o [PLANO-HOST-WEBAPI.md](../../WebApi/docs/PLANO-HOST-WEBAPI.md) ponto a ponto (a tabela abaixo diz como
fica cada um) e reaproveita o que os outros dois hosts já construíram: a agenda em `auditing.Jobs`, os mapas
`IdJob → método`, o `JobCron`, o `ScheduledJobRunner`, a versão dos jobs e a trava `IJobLock` do Shared. O ponto
principal é a **concorrência** (passo 5). Cada passo é um commit, e depois de cada um o host tem que compilar sem erros e
com os mesmos avisos. Os testes de banco rodam num banco de teste próprio na instância SQL Server do Docker (container
`sql-server`), apagado no fim, nunca no LocalDB.

Situação em 01/11/2026:

| Item | Estado |
|---|---|
| Projeto | `Quartz.csproj` (`Microsoft.NET.Sdk.Worker`, .NET 10), só com `Microsoft.Extensions.Hosting` |
| Código | `Program.cs` e `Worker.cs` do modelo, no namespace `Quartz`, o mesmo da biblioteca Quartz.NET: com o pacote instalado, os dois nomes colidem (mais um motivo para o passo 1) |
| Módulos | nenhum referenciado nem registrado |
| Configuração | `appsettings.json` e `appsettings.Development.json` só com `Logging`; `UserSecretsId` já existe no `.csproj` |
| Referências | `.slnx`, `README.md` da raiz ("worker alternativo (não usado)" e "O host `Quartz` não é usado"), `PLANO-HOST-WEBAPI.md` (§4.7, opção G) e a proposta do Linx Microvix Outbound |

### Os três orquestradores

| | WebApi + Hangfire | WebJob | **Worker + Quartz** |
|---|---|---|---|
| Processo | o host web, com `Hangfire:Enabled` | um processo por job, um WebJob por job na Azure | **um processo para todos os jobs** |
| Agenda | `auditing.Jobs`, relida pelo `SchedulerRefresh` | `auditing.Jobs`, relida pelo processo de cada job | `auditing.Jobs`, relida por um job do Quartz (4.4) |
| Estado do agendador | storage do Hangfire no banco | nenhum | **`AdoJobStore` no banco das integrações, em cluster** (3.3) |
| Painel | dashboard do Hangfire | portal da Azure | **nenhum**: `auditing` e o painel do WebApp (3.2) |
| Onde roda | App Service ou VM | App Service (um WebJob por job) | **serviço do Windows** (6.1) |
| Concorrência | filtro do Hangfire + `IJobLock` | `IJobLock` no laço do processo | **`[DisallowConcurrentExecution]` + `IJobLock`** (passo 5) |
| Rotas manuais | no próprio host | no WebApi | no WebApi, em modo só API |
| Recuperação de jobs travados | — | job 14000 | — |

A regra decidida no PLANO-HOST-WEBJOB.md §4.3 passa a valer para os três: **um host de agenda por vez** no mesmo banco.
Uma combinação natural com o Worker é o WebApi em modo só API (`Hangfire:Enabled = false`) para as rotas manuais e o
Worker para a agenda.

### O que foi tratado no Hangfire e como fica no Worker

| Ponto do PLANO-HOST-WEBAPI.md | No Worker |
|---|---|
| 1. Renomear o projeto | passo 1: `Quartz` → `Worker` |
| 2. Limpar o legado | passo 2: só o modelo do `dotnet new worker` sai; não há código legado |
| 3. Agendador opcional (`Hangfire:Enabled`) | passo 3: o host inteiro é a opção (sobe ou não sobe); dentro dele, `Quartz:Enabled` para subir sem agenda (3.1) |
| 3.1 Seção do `appsettings.json` validada na subida | seção `Quartz`, com `QuartzSettings` e validação (3.1) |
| 3.2 Dashboard com autenticação | sem dashboard e, portanto, sem autenticação de dashboard (3.2) |
| 3.3 Carga do agendador no banco | `AdoJobStore` no mesmo banco das integrações, em cluster, sem histórico para expirar (3.3) |
| 3.4 Limpeza da tela de Failed | não existe: o Quartz não guarda execuções terminadas nem falhas (3.4) |
| 4.1 Colunas `IsActive`, `RunningDailyAt`, `Queue` | já feitas; `Queue` não é usada pelo Worker (4.1) |
| 4.2 Do registro ao cron | o `JobCron` dá o cron de 5 campos; o Worker converte para o cron do Quartz, que tem segundos (4.2) |
| 4.3 Fuso horário | `Quartz:TimeZone` (`America/Sao_Paulo`), no trigger de cada job (4.3) |
| 4.4 Job que relê a tabela | `SchedulerRefreshJob` do Quartz (4.4) |
| 4.5 Job 9003 / 4.6 revisão dos scripts | já feitos no WebApi; nada a fazer |
| 4.7 Concorrência | passo 5, o centro do plano |
| 4.10 Versão dos jobs | o nome do job no Quartz leva a versão; sem versão, o host não sobe (4.5) |
| 5. Autenticação da API | não se aplica: o Worker não tem API |
| 6. Módulos que ainda não estão no host | passo 7, os mesmos do WebApi e do WebJob |
| 7. README | passo 8 |

---

## Fases de execução

| Fase | O que entrega | Passos | Depende de | Pronta quando |
|---|---|---|---|---|
| 1 — Preparação | o host renomeado e limpo | 1 e 2 | — | `Worker` compila na solution, sem o modelo e sem referência a `Quartz` como nome de projeto |
| 2 — Quartz e agenda | o Quartz ligado, com a seção validada e a agenda de `auditing.Jobs` | 3 e 4 | fase 1 | com jobs ativos no banco de teste, o Worker os agenda com o cron e o fuso certos, e relê a tabela a cada ciclo |
| 3 — Concorrência | as duas camadas de proteção e o aviso de pulos | 5 | fase 2 | todos os cenários do 5.6 conferidos no Docker |
| 4 — Publicação | o Worker como serviço do Windows | 6 | fase 3 | o script instala e atualiza o serviço numa máquina de teste, e o serviço roda um job de teste |
| 5 — Documentação e o que entra depois | README e os módulos que ainda faltam | 7 e 8 | as anteriores | — |

---

# Fase 1 — Preparação

## Passo 1 — Renomear o projeto para Worker

1. `git mv src/Backend/Hosts/Quartz src/Backend/Hosts/Worker` (histórico preservado) e `Quartz.csproj` → `Worker.csproj`.
2. Namespace `Quartz` → `Worker` em todo o projeto; perfil `Quartz` → `Worker` no `launchSettings.json`. O
   `UserSecretsId` fica como está (mudar perde os segredos de quem já tem).
3. `.slnx`: o caminho novo, na ordem alfabética da pasta `Hosts` (Hosts.Scheduling, WebApi, WebJob, Worker).
4. Referências: a árvore e o texto do `README.md` da raiz, o §4.7 do PLANO-HOST-WEBAPI.md ("host Quartz" → "host Worker
   (antes Quartz)"), o PLANO-HOST-WEBJOB.md, o comentário do `Hosts.Scheduling.csproj` (que lista os hosts que dividem o
   projeto) e a proposta do Linx Microvix Outbound.
5. `bin/` e `obj/` da pasta antiga apagados.

## Passo 2 — Limpar o modelo

1. Sai o `Worker.cs` do modelo (a classe `Worker` no namespace `Worker` também colidiria).
2. O `Program.cs` fica só com o `Host.CreateApplicationBuilder`, pronto para o passo 3.

---

# Fase 2 — Quartz e agenda

## Passo 3 — Base do host e Quartz

1. **Pacotes:** `Quartz.Extensions.Hosting` (traz o `Quartz` e a integração com a injeção de dependência) e
   `Quartz.Serialization.SystemTextJson` (o `AdoJobStore` do 3.3 serializa o `JobDataMap`), na versão estável mais
   recente do Quartz.NET 3.x, conferida no NuGet na hora; `Microsoft.Extensions.Hosting.WindowsServices` para o 6.1.
2. **Módulos:** os mesmos do WebApi e do WebJob (`ServicesExtensions.AddServices`), a referência ao `Hosts.Scheduling` e
   o `AddScheduledJobRunner()`. Os três hosts passam a repetir o mesmo registro de módulos; centralizar isso numa
   extensão compartilhada fica como melhoria à parte, fora deste plano.
3. **Quartz:** `AddQuartz` com o nome do scheduler, o pool de threads e o store da seção `Quartz`, e
   `AddQuartzHostedService` com `WaitForJobsToComplete = true` (no desligamento, espera as execuções em andamento
   terminarem). O `HostOptions.ShutdownTimeout` vem da seção (`Shutdown:TimeoutInMinutes`), para o serviço não ser
   derrubado no meio de um job longo.
4. **Fábrica de jobs:** a do `Quartz.Extensions.DependencyInjection`, que cria um escopo por execução: o
   `ScheduledJobRunner` e os services são scoped, como nos outros hosts.
5. **Log:** o Quartz escreve pelo `ILogger`; `"Quartz": "Warning"` no `Logging:LogLevel` para não encher o log com cada
   disparo.

### 3.1 Seção `Quartz` do `appsettings.json`

Todos os parâmetros numa seção só, lida por `QuartzSettings` e validada na subida (`QuartzSettingsValidator`), como no
WebApi: com um parâmetro obrigatório faltando, o host não sobe e o log diz qual.

```json
"Quartz": {
  "Enabled": true,
  "SchedulerName": "",
  "TimeZone": "America/Sao_Paulo",

  "Storage": {
    "ConnectionStringName": "Connection",
    "TablePrefix": "[quartz].[QRTZ_",
    "Clustered": true,
    "ClusterCheckinIntervalInSeconds": 20
  },

  "Threads": {
    "MaxConcurrency": 20
  },

  "Misfire": {
    "ThresholdInMinutes": 15
  },

  "Schedule": {
    "RefreshIntervalInMinutes": 10
  },

  "Concurrency": {
    "MaxConsecutiveSkips": 5
  },

  "Shutdown": {
    "TimeoutInMinutes": 5
  }
}
```

| Parâmetro | Uso |
|---|---|
| `Enabled` | `false`: o host sobe sem o Quartz (útil para conferir configuração e conexão sem agendar nada). Diferente do WebApi, o padrão é `true`: o Worker só existe para agendar |
| `SchedulerName` | nome do scheduler nos logs (e no cluster); vazio usa o nome da máquina |
| `TimeZone` | fuso de todos os triggers (4.3) |
| `Storage:ConnectionStringName` | a entrada de `ConnectionStrings` do store: **`Connection`, o mesmo banco das integrações**, como o storage do Hangfire (3.3). Para mudar, outra entrada e apontar para ela |
| `Storage:TablePrefix` | schema e prefixo das tabelas do Quartz (`[quartz].[QRTZ_`) |
| `Storage:Clustered` | **`true` desde o início**, mesmo com um Worker só (3.3) |
| `Storage:ClusterCheckinIntervalInSeconds` | de quanto em quanto tempo cada Worker avisa que está vivo: **20**, o padrão do Quartz |
| `Threads:MaxConcurrency` | execuções ao mesmo tempo: **20**, como o `WorkerCount` do Hangfire (o padrão do Quartz é 10) |
| `Misfire:ThresholdInMinutes` | quanto atraso um disparo tolera antes de ser descartado (5.2): **15** |
| `Schedule:RefreshIntervalInMinutes` | intervalo do `SchedulerRefreshJob` (4.4): **10** |
| `Concurrency:MaxConsecutiveSkips` | pulos seguidos de um job até o aviso no log (5.4): **5** |
| `Shutdown:TimeoutInMinutes` | quanto o desligamento espera as execuções em andamento |

**O nome da seção.** A documentação do Quartz.NET usa uma seção `Quartz` com chaves planas
(`"quartz.scheduler.instanceName"`) lida pelo `QuartzOptions`. Aqui ela não é usada: a seção `Quartz` é nossa,
estruturada como a do Hangfire, e o código passa cada valor ao `AddQuartz` (decisão de 01/11/2026: fica `Quartz`).

### 3.2 Painel

O Quartz.NET não tem dashboard. O que existe são projetos de terceiros (CrystalQuartz, SilkierQuartz, Quartzmin), que
precisam de um host web e, quase todos, do store no banco, com manutenção incerta.

**Decisão (01/11/2026): sem painel no Worker.** O que roda e o que falhou fica onde já fica para os três hosts: `auditing.Logs` e
`auditing.Messages`, a view `auditing.vw_JobsHeartbeat` e o painel do WebApp (PLANO-WEBAPP.md), que lê o `auditing` e
vale para qualquer orquestrador. O Worker escreve no log, a cada releitura, o resumo da agenda (quantos jobs, o que
entrou, mudou e saiu) e, na subida, a lista dos jobs agendados com o próximo disparo. Como não há painel, **não há
autenticação de dashboard** a fazer (o 3.2 do WebApi não se aplica).

Se um dia a equipe quiser painel, a alternativa é um dos de terceiros sobre o mesmo store, com autenticação igual à do
Hangfire (Basic, credenciais por variável de ambiente), num passo próprio.

### 3.3 Onde o Quartz guarda o estado

No Hangfire, o storage no banco era obrigatório e era a maior carga (PLANO-HOST-WEBAPI.md §3.3). O Quartz tem duas
opções:

| Store | Como funciona | A favor | Contra |
|---|---|---|---|
| **`RAMJobStore`** (avaliado, não adotado) | jobs e triggers em memória; nada no banco | **carga zero do agendador no banco**; nenhuma tabela, nenhum script. Serve aqui porque a agenda é remontada de `auditing.Jobs` na subida e a cada releitura (4.4), e o histórico já está em `auditing.Logs` | um Worker só (dois Workers em memória agendariam os mesmos jobs; a trava do passo 5 impede que rodem juntos, mas cada disparo vira um pulo no outro); disparos perdidos enquanto o processo estava fora não são lembrados (com a regra de misfire do 5.2 eles seriam descartados de qualquer jeito) |
| **`AdoJobStore` no SQL Server** (adotado) | jobs, triggers e disparos em tabelas `QRTZ_*`; com `Clustered`, vários Workers dividem a agenda e cada disparo roda em um só | cluster; `[DisallowConcurrentExecution]` vale entre os Workers | 11 tabelas, que o Quartz **não cria sozinho** (diferente do Hangfire): o script oficial `tables_sqlServer.sql` entra no DbUp, num schema próprio (`quartz`); a cada disparo, gravações em `QRTZ_FIRED_TRIGGERS` e `QRTZ_TRIGGERS` e o `SELECT ... UPDLOCK` em `QRTZ_LOCKS`; o check-in do cluster a cada 20 s |

Mesmo no banco, o Quartz pesa menos que o Hangfire: ele não guarda execuções terminadas (não há `Succeeded`, `Deleted` ou
`Failed` para expirar), então as tabelas não crescem.

**Decisão (01/11/2026): `AdoJobStore` no SQL Server desde o início**, no mesmo banco das integrações
(`Storage:ConnectionStringName = "Connection"`, como o storage do Hangfire), sem o `RAMJobStore`:

1. **Tabelas.** As 11 tabelas `QRTZ_*` do script oficial do Quartz.NET para SQL Server (`tables_sqlServer.sql`, da mesma
   versão do pacote), no schema `quartz`, num script do próprio host
   (`Worker/Scripts/SQLServer/Script001_Create_Quartz_Tables.sql`) com um migrator `migrator.csx` como os dos módulos
   (docs/MIGRATOR-CSX.md). O Quartz não cria nem atualiza as tabelas: numa troca de versão do pacote que mude o schema,
   entra um script novo.
2. **Cluster ligado desde o início** (`Storage:Clustered = true`), mesmo com um Worker só. Dois schedulers sem cluster
   nas mesmas tabelas é configuração não suportada pelo Quartz (os dois disparam o mesmo trigger e se atrapalham na
   recuperação); com o cluster ligado, subir um segundo Worker por engano, ou de propósito, é seguro. O custo é o check-in
   a cada 20 s e o `QRTZ_LOCKS` em cada aquisição de trigger, pequeno perto do resto.
3. **O `JobDataMap` só com texto** (`UseProperties = true`), serializado pelo `SystemTextJson`: o `IdJob` e o fim da
   última execução (5.2).
4. **Carga.** A cada disparo, gravações pequenas em `QRTZ_TRIGGERS`, `QRTZ_FIRED_TRIGGERS` e (pelo 5.2)
   `QRTZ_JOB_DETAILS`; não há histórico de execuções, então as tabelas não crescem e não há retenção a configurar. A
   agenda continua vindo de `auditing.Jobs`: o store guarda a cópia que o `SchedulerRefreshJob` (4.4) mantém em dia.
5. Os bancos que já existem recebem o script pelo migrator, como qualquer script novo.

### 3.4 Falhas

Com `AutomaticRetry = 0`, o Hangfire guardava cada falha na tela de `Failed`, e foi preciso um job para limpá-la. No
Quartz, uma execução que lança exceção termina e o trigger segue para o próximo disparo; nada fica guardado. O
`ScheduledJobRunner` já grava o erro em `auditing.Logs` e `auditing.Messages` e não deixa a exceção subir. **Não há
limpeza a fazer.** Para não ter retentativa, o job nunca lança `JobExecutionException` com `RefireImmediately`.

## Passo 4 — Agenda a partir de `auditing.Jobs`

### 4.1 Colunas

As mesmas do Hangfire e do WebJob, já no Script001 do Shared e nos 61 scripts de insert (PLANO-HOST-WEBAPI.md §4.1 e
§4.6): `IsActive`, `IntervalTimeInMinutes` e `RunningDailyAt`. Só os jobs de integração (`IdType = 2`), ativos e com
método no mapa entram na agenda. `Queue` não é usada: o Worker não tem filas, só o pool de threads. Se um dia houver dois
Workers com papéis diferentes, a `Queue` pode virar o filtro de quem agenda o quê, mas isso fica fora deste plano.

### 4.2 Do registro ao cron do Quartz

O `JobCron.From(intervalo, RunningDailyAt)` do `Hosts.Scheduling` dá o cron de 5 campos, com as mesmas regras dos outros
hosts (intervalo que divide a hora, horas que dividem o dia, diário na hora de `RunningDailyAt`; o resto fica fora, com
aviso). O cron do Quartz tem segundos na frente e exige `?` em um dos campos de dia. A conversão é uma função nova no
`JobCron` (`ToQuartz`), testada com os casos de hoje:

| Job | `JobCron.From` | Quartz |
|---|---|---|
| 3 minutos | `*/3 * * * *` | `0 */3 * * * ?` |
| 60 minutos | `0 * * * *` | `0 0 * * * ?` |
| 120 minutos | `0 */2 * * *` | `0 0 */2 * * ?` |
| diário às 23:00 | `0 23 * * *` | `0 0 23 * * ?` |

Cada job da agenda vira, no Quartz, um `JobDetail` (o job) e um `CronTrigger` (quando ele roda):

* `JobKey`: nome `<IdJob>-<Title> - V<versão>` (o mesmo `RecurringJobMap.RecurringJobId` do Hangfire), grupo
  `integrations`;
* `JobDataMap` com o `IdJob`;
* classe `QuartzJobRunner` (5.3), uma só para todos os jobs, que chama o `ScheduledJobRunner`;
* trigger com o cron, o fuso (4.3) e a regra de misfire do 5.2.

### 4.3 Fuso horário

`Quartz:TimeZone` (`America/Sao_Paulo`) em todo trigger (`CronScheduleBuilder.InTimeZone`). Um fuso inválido impede o
host de subir (validação do 3.1).

### 4.4 Job que relê a tabela

`SchedulerRefreshJob`, um job do próprio Worker, como o `SchedulerRefresh` do Hangfire:

1. Roda na subida e depois a cada `Schedule:RefreshIntervalInMinutes` (10).
2. Lê `auditing.Jobs` (`IJobScheduleRepository.GetIntegrationJobs`), compara com os jobs do grupo `integrations` no
   scheduler e faz só a diferença: agenda os novos, troca o trigger dos que mudaram de cron, remove (`DeleteJob`) os
   inativos, sem intervalo ou fora do mapa. Um `UPDATE` na tabela vale, no máximo, no ciclo seguinte.
3. Não está em `auditing.Jobs` e não pode ser desligado pela tabela; só some com `Quartz:Enabled = false`. Tem
   `[DisallowConcurrentExecution]`.
4. Registra no log o resumo e os avisos (job sem método, intervalo que não vira cron, o 14000 ativo, que só existe no
   WebJob).
5. Remover um job que está rodando não interrompe a execução: o `DeleteJob` tira o job e o trigger, e a execução em
   andamento termina normalmente.

### 4.5 Versão dos jobs

O nome do job leva a versão do `[JobVersion]` (PLANO-HOST-WEBAPI.md §4.10). Com uma versão nova, a releitura seguinte
(ou a subida) agenda `...V1.0.1` e remove `...V1.0.0`. Os mapas são montados e validados na subida: um método sem
`[JobVersion]`, sem título ou um `IdJob` repetido impedem o host de subir, com a lista do que falta.

Um detalhe que vem do Quartz e importa para a concorrência: na troca de versão, o job antigo e o novo são **dois jobs
diferentes** para o Quartz. Se o antigo ainda estiver rodando quando o novo disparar, o `[DisallowConcurrentExecution]`
não segura (5.1); quem segura é o `IJobLock`, porque a chave é `job:<IdJob>`, sem a versão.

---

# Fase 3 — Concorrência

## Passo 5 — Concorrência

O problema é o mesmo do Hangfire (PLANO-HOST-WEBAPI.md §4.7): um job de 3 minutos cuja execução passa de 3 minutos não
pode começar outra execução por cima, porque as duas leem a mesma janela, escrevem nas mesmas `untreated`, chamam as
mesmas `sp_*_Sync` e gravam o mesmo parâmetro de última leitura. A diferença é que o Quartz tem proteção nativa, e o
Hangfire grátis não tinha.

### 5.1 O que o Quartz oferece

| Recurso | O que faz | Limite |
|---|---|---|
| `[DisallowConcurrentExecution]` na classe do job | o scheduler **não dispara** um `JobDetail` enquanto a execução anterior **do mesmo `JobKey`** roda: o trigger fica bloqueado, sem ocupar thread nem conexão, e é liberado quando ela termina | vale **por `JobKey`**, não por classe: com uma classe só para todos os jobs (4.2), cada `IdJob` continua independente, o que é o certo. Mas não cobre: jobs diferentes que dividem dados (os do DW, `group:datawarehouse`); o job antigo e o novo na troca de versão (4.5); um Worker e o Hangfire ou um WebJob numa troca de host. E não pula: o disparo bloqueado **roda atrasado** quando a anterior termina (5.2) |
| Regra de misfire do trigger | o que fazer com o disparo atrasado demais (bloqueado pelo item acima, esperando thread ou com o processo fora) | ver 5.2 |
| Cluster no `AdoJobStore` | cada disparo roda em um Worker só, e o `[DisallowConcurrentExecution]` vale entre os Workers | ligado desde o início (3.3) |
| `[PersistJobDataAfterExecution]` | o `JobDataMap` alterado pela execução é gravado no store | usado no 5.2 para guardar o fim da última execução, visível em qualquer Worker do cluster |

### 5.2 Pular como no Hangfire

**Decisão (01/11/2026): a mesma regra do Hangfire.** No Hangfire, um disparo que acontece **enquanto a execução anterior
do mesmo job roda** é pulado (vai para `Deleted`), e um disparo que só espera um worker livre roda quando o worker
aparece, atrasado, sem pular. O Worker faz o mesmo.

**Por que o misfire do Quartz, sozinho, não dá isso.** Com o `[DisallowConcurrentExecution]`, o disparo das 10:03 de um
job que ainda roda a execução das 10:00 fica bloqueado e, quando ela termina, o Quartz olha só o **atraso** do disparo:
até o limiar de misfire (`misfireThreshold`), ele roda atrasado; acima, vale a instrução do trigger
(`DoNothing` descarta, `FireAndProceed` roda uma vez, `IgnoreMisfires` roda todos). O atraso não diz **por que** o disparo
atrasou:

* com um limiar pequeno (o Quartz não aceita 0; o mínimo prático é 1 s), o job lento pula como no Hangfire, mas qualquer
  disparo que espere uma thread livre também pula. Nos minutos cheios em que disparam os 154 jobs de 120 minutos, e às
  23:00 com os 48 diários, só os primeiros 20 (o pool) rodariam; os outros seriam descartados até o ciclo seguinte, que
  teria o mesmo problema. O Hangfire, nesse caso, enfileira e roda todos;
* com um limiar grande, o job lento não pula: o disparo bloqueado roda emendado na execução anterior, que é o acúmulo que
  a trava existe para evitar.

**Como fica: o pulo decide pela execução anterior, não pelo atraso.**

1. **No job.** O `QuartzJobRunner` tem `[PersistJobDataAfterExecution]` e grava no `JobDataMap`, ao terminar, o fim da
   execução (`LastEndUtc`). Ao começar, compara o horário agendado do disparo (`context.ScheduledFireTimeUtc`) com o
   `LastEndUtc`: **disparo agendado antes do fim da execução anterior = disparo que aconteceu com ela rodando → pula**,
   sem pedir a trava e sem abrir conexão. Agendado depois do fim: roda, por mais atrasado que esteja (esperou thread).
   Como o `JobDataMap` fica no store (3.3), a regra vale entre os Workers do cluster.
2. **O limiar de misfire fica grande** (`Misfire:ThresholdInMinutes`, **15**): é a tolerância para um disparo esperar
   thread nas rajadas dos minutos cheios. Acima disso, `WithMisfireHandlingInstructionDoNothing`: o disparo é descartado
   e o job volta no horário seguinte do cron. É o que acontece também depois de o Worker ficar fora: os disparos perdidos
   não são executados de uma vez na volta. A diferença para o Hangfire fica só aqui: o Hangfire nunca descarta um disparo
   que espera worker; o Worker descarta depois de 15 minutos de espera, e o log mostra (5.3, item 2).
3. **Um job lento atrás de vários disparos.** O trigger bloqueado guarda só o próximo disparo, não todos os perdidos.
   Uma execução de 10 minutos de um job de 3: ao terminar, o disparo mais antigo dentro dos 15 minutos é liberado e
   pulado pelo item 1, o seguinte também, até o primeiro horário depois do fim, que roda normalmente. Cada um é um pulo
   barato no log (sem trava, sem conexão), e juntos contam para o aviso de pulos seguidos, como os pulos do Hangfire.

| Situação (job de 3 min) | Hangfire | Worker |
|---|---|---|
| execução das 10:00 leva 4 min | 10:03 pulado; 10:06 roda | 10:03 pulado (agendado antes do fim, 10:04); 10:06 roda |
| disparo das 10:03 espera 2 min por thread | roda às 10:05 | roda às 10:05 (agendado depois do fim da anterior) |
| disparo espera thread mais de 15 min | roda quando houver worker | descartado pelo misfire; volta no horário seguinte |
| Worker (ou servidor Hangfire) fora por 1 hora | `MisfireHandling` do recurring job | os disparos perdidos são descartados; volta no horário seguinte |

### 5.3 A combinação: duas camadas

Como no Hangfire, a regra que vale para qualquer disparo é a do `IJobLock` (a trava da aplicação, no banco das
integrações). O Quartz entra como primeira camada, mais barata:

| Opção | Como funciona | A favor | Contra |
|---|---|---|---|
| **N. Só o nativo** (`[DisallowConcurrentExecution]` + `DoNothing`) | o Quartz não dispara o mesmo `JobKey` por cima | sem código de trava; sem conexão parada por job | não cobre os 4 casos do 5.1 (grupo do DW, troca de versão, dois Workers em memória, troca de host) |
| **T. Só o `IJobLock`** | o job pede a trava antes de rodar e pula se ocupada, como o WebJob | cobre tudo; código já existe | o disparo sobreposto ocupa uma thread e abre uma conexão só para descobrir que vai pular; o Quartz não sabe do pulo |
| **N + T** | o `[DisallowConcurrentExecution]` evita a maioria dos disparos sobrepostos sem custo; o `IJobLock` pega o que sobra | cobre tudo; a trava só trabalha nos casos raros; a mesma chave dos outros hosts | duas regras para entender (este passo as documenta) |

**Decisão (01/11/2026): as duas camadas, N + T**, com o pulo do 5.2.

**O desenho:**

1. **`QuartzJobRunner`** (`IJob`, uma classe só para todos os jobs da agenda), com `[DisallowConcurrentExecution]` e
   `[PersistJobDataAfterExecution]`. Lê o `IdJob` do `JobDataMap`, acha a definição no mapa e:
   1. **disparo agendado antes do fim da execução anterior** (`ScheduledFireTimeUtc < LastEndUtc`, 5.2): não roda,
      registra no log `Skipped: previous execution of job:<IdJob> still running at <horário agendado>` e conta o pulo;
   2. senão, pede a trava do `IJobLock` com a chave do mapa (`job:<IdJob>` ou `group:datawarehouse`, as mesmas do
      Hangfire e do WebJob), sem esperar;
   3. **conseguiu:** zera o contador de pulos do job, roda pelo `ScheduledJobRunner.Run(definição)`, grava o `LastEndUtc`
      e solta a trava num `finally`, com sucesso ou erro;
   4. **não conseguiu:** não roda, registra no log `Skipped: previous execution of <chave> still running`, guarda o
      motivo no `context.Result` e conta o pulo. O `LastEndUtc` não muda (a execução que segura a trava é de outro
      `JobKey` ou de outro host).
2. **Descartes pelo misfire.** Um disparo que esperou mais que `Misfire:ThresholdInMinutes` é descartado pelo
   `DoNothing` e não chega ao job: quem fica sabendo é um `ITriggerListener` do Worker (`TriggerMisfired`), que registra
   no log `Misfired: <IdJob> waited more than <n> minutes` e conta no mesmo contador. Que o `TriggerMisfired` é chamado
   nesse caso com o `AdoJobStore` em cluster é uma das primeiras coisas a provar no protótipo.
3. **Aviso de pulos seguidos:** depois de `Concurrency:MaxConsecutiveSkips` (5) pulos seguidos do mesmo job, um aviso
   (`warn`) no log: o job está lento demais para o intervalo, e a correção é aumentar o intervalo na tabela. O contador é
   do processo, como nos outros hosts.
4. **`SchedulerRefreshJob`** com `[DisallowConcurrentExecution]` e a chave `job:scheduler-refresh` no `IJobLock`, como no
   Hangfire: com dois Workers, só um relê a tabela por vez.
5. **Uma conexão só para a trava**, separada das do job, aberta durante a execução e fechada no fim (é o
   `SqlServerJobLock`, que já faz isso). Se o processo cair, a conexão fecha e a trava é solta sozinha.
6. **Carga:** com o nativo e a regra do 5.2 na frente, o `sp_getapplock` só é chamado quando o job vai mesmo rodar, uma
   vez por execução, como no Hangfire (cerca de 40 por minuto). O PLANO-HOST-WEBAPI.md §4.7 já mostrou que a trava não encosta nas
   tabelas, não entra em deadlock (espera zero) e não segura transação; vale igual aqui.

### 5.4 Cenários

| Cenário | O que acontece |
|---|---|
| Job de 3 min leva 4 min | o disparo seguinte é bloqueado pelo Quartz, liberado no fim e pulado pela regra do 5.2 (agendado antes do fim); o job volta no horário seguinte |
| Job de 3 min leva 3 min e 20 s | o mesmo: o disparo aconteceu com a anterior rodando, então pula, como no Hangfire |
| Rajada dos minutos cheios (154 jobs de 120 min, pool de 20) | os disparos esperam thread e rodam atrasados, sem pular (agendados depois do fim da anterior); só os que esperam mais de 15 minutos são descartados, com registro no log |
| Dois jobs do DW no mesmo minuto | o Quartz dispara os dois (são `JobKey`s diferentes); o primeiro pega `group:datawarehouse`, o segundo pula pela trava |
| Troca de versão com o job rodando | o `...V1.0.1` dispara enquanto o `...V1.0.0` roda (o `LastEndUtc` do novo está vazio); a trava `job:<IdJob>` pula o novo |
| Segundo Worker no mesmo banco | o cluster entrega cada disparo a um Worker só, e o `[DisallowConcurrentExecution]` e o `LastEndUtc` valem entre eles; a trava sobra como garantia |
| Worker e Hangfire (ou WebJob) ligados juntos numa troca de host | a regra é um host por vez; se acontecer, a mesma chave nos três hosts impede que um job rode junto |
| Rota manual no WebApi com o job rodando no Worker | roda junto: as rotas ficam fora da trava (decisão de 30/10/2026), uso manual e consciente. O DW mantém a própria trava no processo (409) |
| O Worker cai no meio de um job | a conexão da trava fecha e a trava é solta; o cluster marca o disparo em andamento como órfão (o job não pede `RequestsRecovery`, então não é reexecutado) e libera o `JobKey`; na volta, os disparos perdidos há mais de 15 minutos são descartados (`DoNothing`) |
| Desligamento do serviço | `WaitForJobsToComplete`: o Quartz para de disparar e espera as execuções em andamento, até `Shutdown:TimeoutInMinutes`; passado o tempo, o processo é encerrado e cai no caso acima |
| Execução presa (API que não responde) | segura a trava e o `JobKey`; todos os disparos seguintes pulam até ela terminar. A defesa é o timeout do `HttpClient` de cada módulo e o `commandTimeout` das procedures, como no Hangfire; o aviso de pulos seguidos mostra o caso no log |

Pular é seguro nas leituras por janela (a execução seguinte lê a partir da última data gravada); a ressalva do
PLANO-HOST-WEBAPI.md §4.7 vale igual: conferir em cada service antes de ligar o `IsActive` dos seus jobs.

### 5.5 Pool de threads

Com o pulo, uma execução nunca espera outra, então o `Threads:MaxConcurrency` (20) só precisa cobrir os jobs rodando ao
mesmo tempo. Nos minutos cheios em que disparam os 154 jobs de 120 minutos (e às 23:00, os 48 diários), mais de 20
disparos chegam juntos: os que não acham thread esperam uma livre, como no Hangfire, e só os que esperam mais que
`Misfire:ThresholdInMinutes` (15) são descartados e voltam no ciclo seguinte. O log de descartes mostra se isso
acontece; a correção é espalhar os horários na tabela (`RunningDailyAt`), aumentar o pool ou o limiar.

### 5.6 Conferência (protótipo)

No banco de teste do Docker:

1. as tabelas `quartz.QRTZ_*` criadas pelo migrator, e o Worker sobe em cluster com elas;
2. um job de teste que dorme mais que o intervalo (a cada minuto, só no teste): o disparo seguinte pula pela regra do 5.2,
   com o pulo no log, e nenhuma execução roda em paralelo;
3. o mesmo dormindo um pouco mais que o intervalo (ex.: 70 s num job de 1 min): pula, como no Hangfire;
4. pool reduzido a 1 thread e dois jobs no mesmo minuto: o segundo roda atrasado, sem pular; com o limiar reduzido (só no
   teste), o que espera demais é descartado e o `TriggerMisfired` registra;
5. com `group:datawarehouse` presa por outra sessão: o 9002 pula pela trava; solta, roda;
6. troca de versão com o job rodando: o novo pula pela trava;
7. dois Workers no mesmo banco, em cluster: nenhum job roda em paralelo, e cada disparo roda num só;
8. cinco pulos seguidos dão o aviso; o contador zera quando o job roda;
9. derrubar o processo no meio de um job solta a trava (`sys.dm_tran_locks` sem `APPLICATION` sobrando) e o outro Worker
   (ou o mesmo, na volta) segue a agenda;
10. parar o serviço com um job rodando: a parada espera o job.

---

# Fase 4 — Publicação

## Passo 6 — O Worker como opção de deploy

### 6.1 Serviço do Windows

**Decisão (01/11/2026): o Worker roda como serviço do Windows**, numa VM (o modelo do Hangfire do sistema antigo, que
rodava em `srv-vm-app01`/`srv-vm-app02`). Contêiner, `systemd` e um WebJob único foram avaliados e ficam de fora.

1. **No host:** `builder.Services.AddWindowsService(o => o.ServiceName = ...)` (pacote
   `Microsoft.Extensions.Hosting.WindowsServices`). Fora de um serviço (no `dotnet run`), a chamada não faz nada: o mesmo
   build roda no console para teste. Como serviço, o diretório de trabalho é o do executável (o `appsettings.json` é
   achado) e o log vai também para o Log de Eventos do Windows (origem com o nome do serviço, só `Warning` para cima).
2. **Publicação:** `Publish/publish-worker.ps1` (PowerShell 7) com os perfis num `Publish/publish-profiles.psd1` ao lado,
   como no WebJob. Cada perfil: a máquina (`ComputerName`, vazio = a local), a pasta de instalação, o nome do serviço, a
   conta do serviço e `SelfContained` (com o runtime junto, `win-x64`, a VM não precisa do .NET 10 instalado).
3. **O que o script faz:** `dotnet publish` (Release); **parar o serviço** (a parada espera os jobs em andamento, até
   `Shutdown:TimeoutInMinutes`, 3.1); copiar o build para a pasta (na máquina remota, pelo compartilhamento administrativo
   ou por uma sessão `PSRemoting`), sem apagar o `appsettings.Production.json` de lá; na primeira vez, **criar o
   serviço** (`New-Service`, início automático atrasado, e `sc.exe failure` para reiniciar sozinho se o processo cair);
   **iniciar** e conferir que ficou `Running`. Parâmetros como os do WebJob: `-PublishProfile` (alias `-Profile`),
   `-DryRun` (só o build e o pacote), `-Uninstall` (para e remove o serviço).
4. **Configuração na máquina:** a connection string e o que muda por ambiente ficam fora do pacote, num
   `appsettings.Production.json` só da VM (com `DOTNET_ENVIRONMENT=Production` no serviço) ou em variáveis de ambiente do
   serviço; nunca no git.
5. **Conta do serviço:** uma conta sem privilégio (ex.: `NT SERVICE\<nome do serviço>`, a conta virtual) com acesso de
   leitura à pasta. Com autenticação integrada no SQL Server, a conta precisa de login no banco; com usuário e senha, ela
   vem da configuração do item 4.
6. **Primeira subida:** com o cluster, o `instanceId` é `AUTO` (nome da máquina + horário), e um segundo Worker em outra
   VM só precisa do mesmo perfil apontando para ela.

### 6.2 Um host por vez

O README dos três hosts (WebApi, WebJob e Worker) diz o que desligar de um lado antes de ligar o outro:

* para ligar o Worker: `Hangfire:Enabled = false` no WebApi e os WebJobs parados;
* para sair do Worker: parar o serviço (ou `Quartz:Enabled = false`) antes de ligar o outro.

**Decisão (01/11/2026): aviso na subida, sem impedir.** Na subida do Quartz, o Worker confere se há servidor do Hangfire
com heartbeat recente no mesmo banco (`HangFire.Server`, se a tabela existir) e, se houver, escreve um aviso (`warn`) no
log dizendo que dois hosts estão agendando. Ele sobe assim mesmo: a trava já impede que um job rode junto, e travar a
subida por causa de um servidor do Hangfire que caiu sem se desregistrar deixaria o Worker parado à toa. Os WebJobs não
são conferidos (precisaria da API da Azure).

---

# Fase 5 — Documentação e o que entra depois

## Passo 7 — Módulos que ainda não estão no host

Os mesmos do passo 6 do WebApi e do passo 9 do WebJob, conforme forem migrados: registro no `ServicesExtensions` e
referência no `.csproj` dos três hosts, o mapa em `Hosts.Scheduling` e o script de jobs com as colunas novas. O 14000
(`StuckJobsRecovery`) continua só no WebJob.

## Passo 8 — Documentação do host

`README.md` do Worker: como rodar no local e no destino do 6.1, a seção `Quartz`, como a agenda funciona e como ligar ou
desligar um job, as duas camadas de concorrência com os cenários do 5.4, a versão dos jobs, onde ver o que rodou (sem
dashboard) e a regra de um host por vez. Os READMEs do WebApi, do WebJob e da raiz passam a citar o Worker como a
terceira opção.

---

## Decisões

Tomadas em 01/11/2026:

1. **Store do Quartz** (3.3): `AdoJobStore` no SQL Server desde o início, no mesmo banco das integrações, schema
   `quartz`, com o cluster ligado.
2. **Painel** (3.2): nenhum; o que rodou fica no `auditing` e no painel do WebApp.
3. **Pulo** (5.2): como no Hangfire: o disparo que acontece com a execução anterior do mesmo job rodando é pulado; o
   que só espera thread roda atrasado. No Quartz, pelo `LastEndUtc` no `JobDataMap`, com o limiar de misfire em 15
   minutos só para as esperas longas.
4. **Concorrência** (5.3): as duas camadas, `[DisallowConcurrentExecution]` + `IJobLock`, com as chaves dos outros hosts.
5. **Onde roda** (6.1): serviço do Windows, com script de publicação e perfis como no WebJob.
6. **Um host por vez** (6.2): a regra nos READMEs e um aviso no log na subida quando o Hangfire está agendando no mesmo
   banco, sem impedir a subida.
7. **Nome da seção** (3.1): `Quartz`.
