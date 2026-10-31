# Host WebApi

Host web do sistema de integrações: registra os módulos, expõe as rotas que disparam cada leitura e escrita e, quando
ligado, roda o **Hangfire**, que agenda os jobs recorrentes a partir de `auditing.Jobs`. Até 30/10/2026 o projeto se
chamava `Hangfire`; as decisões e o histórico de cada passo estão em [docs/PLANO-HOST-WEBAPI.md](docs/PLANO-HOST-WEBAPI.md).

| Item | Valor |
|---|---|
| Plataforma | .NET 10 (`net10.0`), ASP.NET Core |
| Agendador | Hangfire 1.8 com storage SQL Server, opcional (`Hangfire:Enabled`) |
| Endereço local | `http://localhost:5252` (Swagger em `/swagger`, dashboard em `/hangfire`) |
| Autenticação da API | desligada por enquanto: o `[Authorize]` dos controllers está comentado |

---

## Os dois modos

O mesmo projeto sobe de dois jeitos, escolhidos por `Hangfire:Enabled`:

| `Hangfire:Enabled` | O host sobe como |
|---|---|
| `false` (padrão do `appsettings.json`) | **só API**: controllers e Swagger. Nada do Hangfire é registrado e o storage dele nem é acessado. Os jobs rodam pelas rotas, chamadas por quem quiser disparar |
| `true` | **API + Hangfire**: o mesmo da API, mais o storage, o servidor, o dashboard, a agenda montada a partir de `auditing.Jobs` e os jobs de manutenção |

Com o Hangfire ligado, a seção `Hangfire` é validada na subida: faltando um parâmetro obrigatório (a connection string do
storage, o usuário e a senha do dashboard...), o host não sobe e a mensagem lista o que falta.

## Como rodar

Só API:

```bash
dotnet run --project src/Backend/Hosts/WebApi --launch-profile http
```

Com o Hangfire, as credenciais do dashboard vêm de variável de ambiente, nunca do `appsettings.json`:

```bash
Hangfire__Enabled=true Hangfire__Dashboard__UserName="<usuário>" Hangfire__Dashboard__Password="<senha>" dotnet run --project src/Backend/Hosts/WebApi --launch-profile http
```

Para usar user-secrets no lugar das variáveis, o projeto precisa de um `UserSecretsId`, que ainda não tem: rodar uma vez
`dotnet user-secrets init --project src/Backend/Hosts/WebApi` (ele grava o id no `WebApi.csproj`) e depois
`dotnet user-secrets set "Hangfire:Dashboard:Password" "<senha>" --project src/Backend/Hosts/WebApi`.

Antes de ligar o Hangfire num banco, ele precisa ter `auditing.Jobs` com as colunas `IsActive`, `RunningDailyAt` e
`Queue` (Script001 do Shared). Um banco que rodou os scripts antes de 30/10/2026 precisa do ajuste descrito no plano, §4.8.

O [WebApi.http](WebApi.http) tem exemplos de rotas de cada módulo. Cada chamada roda a integração de verdade contra a API
e o banco configurados.

## Estrutura

```
WebApi/
├── Controllers/          rotas por módulo (DataWarehouse, General, LinxCommerce, LinxMicrovix, Shippment, Totvs)
├── Extensions/
│   ├── BuilderExtensions.cs    controllers, Swagger e CORS
│   ├── ServicesExtensions.cs   registro dos módulos e, com Hangfire:Enabled, do Hangfire (AddHangfireService)
│   └── AppUseExtensions.cs     pipeline e, com Hangfire:Enabled, dashboard e jobs de manutenção (UseHangfire)
├── Scheduling/
│   ├── HangfireSettings.cs, HangfireSettingsValidator.cs   seção Hangfire e a validação na subida
│   ├── DashboardBasicAuthorizationFilter.cs                usuário e senha do dashboard
│   ├── JobExpirationTimeoutFilter.cs                       retenção dos jobs concluídos
│   ├── SkipConcurrentExecutionFilter.cs                    trava de concorrência
│   ├── SchedulerRefreshJob.cs, FailedJobsCleanupJob.cs     jobs de manutenção
│   ├── JobCron.cs                                          agenda de auditing.Jobs → cron
│   └── RecurringJobs/                                      mapas IdJob → método, um por módulo
├── appsettings.json
├── WebApi.http
└── docs/PLANO-HOST-WEBAPI.md
```

## Configuração

| Seção | Uso |
|---|---|
| `ConnectionStrings:Connection` | banco das integrações (módulos, `auditing` e, por padrão, o storage do Hangfire e a trava de concorrência) |
| `ConnectionStrings:AuditingConnection` | opcional: banco de `auditing`. Sem ela, o Shared usa a `Connection` trocando `NEWBLOOMERS` por `AUDITING` no nome |
| `Hangfire` | tudo do Hangfire (tabela abaixo) |
| `AfterSale`, `Movidesk`, `LinxMicrovix`, `Totvs`... | `BaseUrl` e as opções de cada módulo (ver o README do módulo) |

Seção `Hangfire` (valores do `appsettings.json`):

| Parâmetro | Valor | Uso |
|---|---|---|
| `Enabled` | `false` | liga o Hangfire |
| `TimeZone` | `America/Sao_Paulo` | fuso de toda a agenda (crons e `RunningDailyAt`) |
| `Storage:ConnectionStringName` | `Connection` | connection string do storage. Para um banco só do Hangfire, criar outra entrada em `ConnectionStrings` e apontar para ela |
| `Storage:SchemaName` | `HangFire` | schema das tabelas do Hangfire, que ele mesmo cria (`PrepareSchemaIfNecessary`), fora do DbUp |
| `Storage:JobExpirationTimeoutInMinutes` | `30` | por quanto tempo um job concluído fica no storage (o padrão do Hangfire é 1 dia) |
| `Storage:QueuePollIntervalInSeconds` | `15` | de quanto em quanto tempo o servidor consulta as filas |
| `Storage:CommandBatchMaxTimeoutInMinutes`, `SlidingInvisibilityTimeoutInMinutes` | `5` | timeouts do storage SQL Server |
| `Server:ServerName` | vazio | nome do servidor no dashboard; vazio usa o nome da máquina |
| `Server:Queues` | `[ "default" ]` | filas que este servidor atende |
| `Server:WorkerCount` | `20` | workers |
| `Schedule:RefreshIntervalInMinutes` | `10` | intervalo do `SchedulerRefresh` (tem que virar cron: dividir a hora ou ser horas que dividem o dia) |
| `Concurrency:MaxConsecutiveSkips` | `5` | pulos seguidos de um job até o aviso no log |
| `Maintenance:FailedJobsRetentionInDays` | `2` | depois de quantos dias uma falha sai da tela de Failed |
| `Dashboard:Path` | `/hangfire` | rota do dashboard |
| `Dashboard:ReadOnly` | `false` | dashboard só para consulta |
| `Dashboard:StatsPollingIntervalInSeconds` | `30` | atualização das estatísticas do dashboard aberto |
| `Dashboard:UserName`, `Password` | vazios | credenciais do dashboard: variável de ambiente (`Hangfire__Dashboard__Password`) ou user-secrets |

## Rotas por módulo

Todas as rotas aparecem no Swagger (em `RELEASE`, os controllers ficam fora dele). As leituras recorrentes não têm
parâmetro; as sob demanda e as escritas pedem o CNPJ da empresa (`{docCompany}`, 14 dígitos) e o cabeçalho
`X-User-Name`, gravado no log de requisições do módulo.

| Módulo | Prefixo | Rotas | Detalhes |
|---|---|---|---|
| Totvs | `TotvsJobs/Totvs` | 123 | [README](../../Modules/Totvs/README.md); escritas só com `Totvs:Writes:<grupo>:Enabled` |
| Linx Microvix (ERP e B2C) | `LinxMicrovixJobs/LinxMicrovix` | 284 | [README](../../Modules/LinxMicrovix/LinxMicrovix%20-%20Outbound%20Web%20Service/README.md) |
| Linx Commerce | `LinxCommerceJobs/LinxCommerce` | 12 | [README](../../Modules/LinxCommerce/README.md) |
| Movidesk | `MovideskJobs/Movidesk` | 16 | [README](../../Modules/General/Movidesk/README.md) |
| AfterSale | `AfterSaleJobs/AfterSale` | 24 | [README](../../Modules/General/AfterSale/README.md) |
| Data warehouse | `DataWarehouseJobs/DataWarehouse` | 5 | [funcionamento e fluxo](../../Modules/DataWarehouse/docs/DATAWAREHOUSE-FUNCIONAMENTO-E-FLUXO.md); 200 concluída, 500 falha, 409 outra carga rodando |

Os controllers de Dootax, Oto, FlashCourier, JadLog, StoneLogistica e TotalExpress existem, mas estão vazios: esses
módulos entram no host conforme forem migrados (plano, passo 6).

## Agenda dos jobs (com o Hangfire ligado)

A agenda vem inteira de `auditing.Jobs`: a tabela diz **se** e **quando** cada job roda e em que fila; o host diz
**o que** ele executa.

| Coluna | Uso |
|---|---|
| `IsActive` | só os jobs com 1 entram na agenda. Nasce 0 em todo insert: nada roda sem um `UPDATE` |
| `IntervalTimeInMinutes` | intervalo; 0 = sob demanda, fora da agenda |
| `RunningDailyAt` | preenchida, o job é diário e roda nessa hora (só com intervalo 1440). Os diários dos scripts estão às 23:00 |
| `Queue` | fila do Hangfire (`default` até existir um segundo servidor) |

Só os jobs de integração (`IdType = 2`) entram na agenda; os de procedure (`IdType = 1`) são chamados pelo próprio
service, depois da carga.

**Do registro ao cron:** 3 vira `*/3 * * * *`, 60 vira `0 * * * *`, 120 vira `0 */2 * * *` e um diário às 23:00 vira
`0 23 * * *`, no fuso de `Hangfire:TimeZone`. Um intervalo que o cron não representa (que não divide a hora ou o dia)
fica fora da agenda, com aviso no log.

**O `SchedulerRefresh`** monta a agenda: roda na subida e a cada `Schedule:RefreshIntervalInMinutes`. Ele lê
`auditing.Jobs`, inclui na agenda os jobs ativos que têm método no mapa, altera os que mudaram e remove os que ficaram
inativos, sem intervalo ou saíram do mapa. Um `UPDATE` na tabela vale, no máximo, no ciclo seguinte. Os recurring jobs
da agenda se chamam `<IdJob>-<Title> - V<versão>` (ex.: `9002-DataWarehouseShortCycle - V1.0.0`), com a versão do
`[JobVersion]` do método (abaixo); o resultado de cada releitura aparece no
dashboard e no log (`Agenda: N job(s) na agenda...`), com os avisos.

Ligar e desligar jobs:

```sql
-- liga um job
UPDATE [auditing].[Jobs] SET IsActive = 1 WHERE IdType = 2 AND IdJob = 3000;

-- liga um módulo inteiro, pela faixa de IdJob (ex.: Movidesk, 3000 a 3499)
UPDATE [auditing].[Jobs] SET IsActive = 1 WHERE IdType = 2 AND IdJob BETWEEN 3000 AND 3499 AND IntervalTimeInMinutes > 0;

-- muda a hora de um diário
UPDATE [auditing].[Jobs] SET RunningDailyAt = '02:30' WHERE IdType = 2 AND IdJob = 9001;

-- desliga
UPDATE [auditing].[Jobs] SET IsActive = 0 WHERE IdType = 2 AND IdJob = 3000;
```

Para não esperar o ciclo, o `SchedulerRefresh` pode ser disparado na hora pela página de jobs recorrentes do dashboard.

## Versão dos jobs

Cada método de service chamado por um job recorrente tem `[JobVersion("MAJOR.MINOR.PATCH")]` no método da classe do
service (não na interface), e a versão vai no nome do recurring job. Para saber o que roda em cada ambiente, basta olhar
o dashboard. Ao mudar o código de um job, mude a versão no mesmo método: PATCH numa correção, MINOR numa mudança
compatível, MAJOR quando muda o que o job grava de um jeito que quebra quem lê. Com a versão nova, o `SchedulerRefresh`
troca o job antigo pelo novo na releitura seguinte. Um método do mapa sem o atributo impede o host de subir com o
Hangfire ligado.

## Concorrência

Um recurring job do Hangfire não sabe se a execução anterior terminou. Para dois disparos do mesmo job nunca rodarem
juntos, um filtro do host (`SkipConcurrentExecutionFilter`) pede, antes de cada execução, uma trava do `IJobLock` do
Shared (`sp_getapplock` no banco das integrações, sem esperar e presa à sessão):

* conseguiu: a execução roda e solta a trava no fim, com sucesso ou erro;
* não conseguiu (a anterior ainda está rodando, neste ou em outro servidor): a execução não roda, vai para `Deleted` com
  o motivo `Skipped: previous execution of <chave> still running` e o worker fica livre. O disparo seguinte tenta de novo.

A chave é `job:<IdJob>`. Os jobs do DW dividem a chave `group:datawarehouse`, porque escrevem nas mesmas landings. Depois
de `Concurrency:MaxConsecutiveSkips` pulos seguidos, sai um aviso no log: o job está lento demais para o intervalo, e a
correção é aumentar o intervalo na tabela.

As **rotas dos controllers ficam fora da trava**: um disparo manual durante a execução agendada roda junto com ela. As
rotas são para uso manual e consciente (o DW continua com a própria trava no processo, que devolve 409).

## Dashboard e manutenção

* O dashboard pede usuário e senha (Basic Authentication, `Dashboard:UserName` e `Password`). Basic manda a senha em cada
  requisição: fora do `localhost`, só por HTTPS.
* Jobs concluídos (`Succeeded` e `Deleted`, inclusive os pulos) saem do storage depois de
  `Storage:JobExpirationTimeoutInMinutes`. O histórico que importa fica em `auditing.Logs` e `auditing.Messages`.
* Uma falha fica em `Failed` (as tentativas automáticas estão em 0) até o `FailedJobsCleanup`, que roda a cada hora, no
  minuto 15, e passa para `Deleted` as falhas com mais de `Maintenance:FailedJobsRetentionInDays`.
* `SchedulerRefresh` e `FailedJobsCleanup` são do próprio host: não estão em `auditing.Jobs` e só somem com
  `Hangfire:Enabled = false`.

## Um job ou um módulo novo

* **Job recorrente novo num módulo do host:** a linha no script de jobs da service (com `RunningDailyAt` e `Queue`, ver
  [PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](../../../../docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md) §3.1), a rota no
  controller, a entrada no mapa do módulo em `Scheduling/RecurringJobs/<Módulo>RecurringJobs.cs`, com o método sem
  parâmetros do service, e `[JobVersion("1.0.0")]` no método da classe do service. Sem o mapa, o `SchedulerRefresh` avisa e deixa o job fora da agenda.
* **Módulo novo:** referência no `WebApi.csproj`, registro no `ServicesExtensions`, controller, o mapa
  `<Módulo>RecurringJobs.cs` incluído no `RecurringJobMap` e o script de jobs com as colunas novas (plano, passo 6).
