# Plano do host WebApi (antigo host Hangfire)

O host `src/Backend/Hosts/WebApi` (até o passo 1, `src/Backend/Hosts/Hangfire`) é hoje uma API de disparo manual: os controllers chamam os services dos módulos, mas
o Hangfire (storage, servidor, dashboard e jobs recorrentes) está todo comentado. Este plano leva o projeto, passo a passo,
a ser o host da API e, opcionalmente (passo 3), do agendamento. Cada passo é um commit, e depois de cada um o host tem que compilar sem erros e
com os mesmos avisos.

Situação em 30/10/2026:

| Item | Estado |
|---|---|
| Módulos registrados no DI | Shared, LinxCommerce, LinxMicrovix Outbound, AfterSale, Movidesk, Totvs e DataWarehouse |
| Módulos comentados | Dootax, Jadlog, Stone, TotalExpress, FlashCourier, Oto, Mobsim e Pagarme (controllers vazios) |
| Rotas | cerca de 500: recorrentes sem parâmetro, sob demanda com `X-User-Name` e escritas da Totvs atrás de `Totvs:Writes:<grupo>:Enabled` |
| Hangfire | pacotes referenciados; `AddHangfireService`, `UseHangfireDashboard` e os recurring jobs comentados |
| Autenticação | `[Authorize]` comentado nos controllers ativos e JWT do Azure AD comentado |
| Agenda | `auditing.Jobs` já tem o intervalo de cada job (`IntervalTimeInMinutes`, 0 = sob demanda) |

---

## Passo 1 — Renomear o projeto para WebApi

O projeto deixa de se chamar Hangfire: ele é a API do sistema, e o Hangfire passa a ser só uma das peças dele. O nome
atual também faz o namespace do projeto (`Hangfire.*`) se misturar com o da biblioteca (`Hangfire`, `Hangfire.SqlServer`).

1. Pasta `src/Backend/Hosts/Hangfire` → `src/Backend/Hosts/WebApi` (com `git mv`, para manter o histórico).
2. `Hangfire.csproj` → `WebApi.csproj`, e o caminho do projeto no `etl-integrations-system.slnx`.
3. `Hangfire.http` → `WebApi.http`, com a variável `@WebApi_HostAddress`.
4. Namespaces: `Hangfire.Controllers.*` e `Hangfire.Extensions*` → `WebApi.Controllers.*` e `WebApi.Extensions*`. Juntar
   aqui os que fogem do padrão: `WebApi.Controllers` do `AuthController` (já certo) e `Hangfire.Controllers.Carries` dos
   controllers de `Shippment/` → `WebApi.Controllers.Shippment`.
5. Onde o código usava tipos da biblioteca sem `using` porque estava dentro do namespace `Hangfire`
   (`AutomaticRetryAttribute`, `CompatibilityLevel` no `ServicesExtensions`), acrescentar `using Hangfire;`.
6. Atualizar os caminhos `Hosts/Hangfire/...` nos documentos: `README.md` da raiz, os README dos módulos (AfterSale,
   Movidesk, LinxCommerce, LinxMicrovix Outbound e Totvs) e os docs dos módulos (DataWarehouse, AfterSale, Movidesk e
   Totvs), 26 ocorrências em 17 arquivos. O texto "host Hangfire" nos documentos vira "host WebApi" onde fala do projeto.
7. Fechar o Visual Studio antes (a pasta `.vs` fica presa) e apagar `bin/` e `obj/` da pasta antiga.

Conferência: build do `WebApi.csproj`, Swagger abrindo em `http://localhost:5252/swagger` e `grep` sem sobras de
`Hosts/Hangfire` e `namespace Hangfire`.

**Feito em 30/10/2026.** O que foi conferido:

* `git mv` da pasta (histórico preservado), `WebApi.csproj`, `WebApi.http` e o `.slnx` (o projeto passou para depois do
  Quartz, na ordem alfabética da pasta `Hosts`). `bin/` e `obj/` da pasta antiga apagados.
* 56 arquivos `.cs` com o namespace novo; `Hangfire.Controllers.Carries` virou `WebApi.Controllers.Shippment`;
  `using Hangfire;` no `ServicesExtensions`.
* Build do `WebApi.csproj` sem erros e com os mesmos 1431 avisos de antes (comparados contra um build do commit
  anterior, com o caminho do host normalizado; só a linha de dois avisos do `ServicesExtensions` andou uma posição, por
  causa do `using` novo).
* Host no ar em `http://localhost:5252`: Swagger com título `WebApi` e as mesmas rotas (465 atributos `Http*` antes e
  depois, 457 caminhos no `swagger.json`).
* Documentos: 28 caminhos `Hosts/Hangfire` em 16 arquivos, todos os links relativos para `Hosts/WebApi` conferidos. O
  texto "host Hangfire" mudou onde descreve o projeto (README da raiz, docs do DW, títulos "6.3 Host" e as tabelas de
  mudanças do host nas propostas dos módulos, proposta de log do AfterSale). Ficaram como estavam os registros de
  resultado (builds e testes já feitos, `REVISAO-PADRAO-CODIGO-E-SCRIPTS.md`, histórico de versões do DW, situação de
  origem das propostas e o roteiro já executado do `GetTickets` do Movidesk).
* O Visual Studio estava aberto durante a troca: ao voltar para ele, recarregar a solução.

## Passo 2 — Limpar o que veio do legado

1. `ConfigurationHelperExtensions` (configuração estática) e o `GeneralIntegrationsRecurringJobsExtensions` vazio saem;
   a configuração chega por `IConfiguration`/options.
2. Excluir `Controllers/Procedures/GeneralController.cs` e `Controllers/Procedures/OperacoesController.cs` (os dois com
   a mesma rota, `ProceduresJobs/Operacoes`, e todo o corpo comentado com o `IIntegrationsCoreRepository` antigo) e
   `Controllers/N8N/N8NController.cs` (vazio), junto com as pastas `Procedures/` e `N8N/`.
3. `Program.cs`: a leitura de `ConfigureServer:ServerName`, que não existe no `appsettings.json`.
4. `WebApi.http`: trocar o modelo padrão (`/todos`) por exemplos das rotas reais.

**Feito em 30/10/2026.** O que foi conferido:

* Saíram `ConfigurationHelperExtensions`, `GeneralIntegrationsRecurringJobsExtensions` (e a pasta `RecurringJobs/`, que
  volta no passo 4 com os mapas por módulo), `Controllers/Procedures/` e `Controllers/N8N/`. Nenhum outro arquivo usava
  a configuração estática.
* `Program.cs` não lê mais `ConfigureServer:ServerName`, e `UseApplication` perdeu o parâmetro `serverName`, que não
  era usado. O `AddHangfireService` (não chamado) ainda lê `ConfigureServer`; ele é reescrito no passo 3.
* `WebApi.http` com uma leitura recorrente e uma sob demanda por módulo (Totvs, Movidesk, AfterSale, Linx Commerce,
  Linx Microvix e DW), todas conferidas contra o `swagger.json`. Só leituras; as escritas ficam no Swagger.
* Build sem erros: 1422 avisos, os 1431 do passo 1 menos os 9 dos dois arquivos excluídos, nenhum novo.
* Host no ar: Swagger com os mesmos 457 caminhos do passo 1 (os controllers excluídos não tinham rotas ativas).

## Passo 3 — Hangfire opcional

O mesmo projeto sobe de dois jeitos, escolhidos por um booleano no `appsettings.json`:

```json
"Hangfire": {
  "Enabled": false
}
```

| `Hangfire:Enabled` | O host sobe como |
|---|---|
| `false` (padrão) | **só API**: controllers e Swagger. Nada do Hangfire é registrado: sem storage, sem servidor, sem dashboard e sem jobs recorrentes; o banco do Hangfire nem é acessado |
| `true` | **API + Hangfire**: o mesmo da API, mais storage, servidor, dashboard e a agenda do passo 4 |

1. `AddServices` registra os módulos sempre; `AddHangfireService` só roda com `Hangfire:Enabled = true`. No
   `UseApplication`, o mesmo para o dashboard e o registro da agenda.
2. Com o Hangfire ligado: storage SQL Server, servidor e `AutomaticRetry` com 0 tentativas (como no
   `AddHangfireService` comentado).

### 3.1 Seção `Hangfire` do `appsettings.json`

Todos os parâmetros do Hangfire ficam numa seção só, no lugar de `ConfigureServer` (que o código lê e não existe no
`appsettings.json`). Ela é lida por uma classe de options (`HangfireSettings`) e validada na subida apenas quando
`Enabled = true`: com o Hangfire ligado e um parâmetro obrigatório vazio, o host não sobe e o log diz qual falta.

```json
"Hangfire": {
  "Enabled": false,
  "TimeZone": "America/Sao_Paulo",

  "Storage": {
    "ConnectionStringName": "Connection",
    "SchemaName": "HangFire",
    "PrepareSchemaIfNecessary": true,
    "CommandBatchMaxTimeoutInMinutes": 5,
    "SlidingInvisibilityTimeoutInMinutes": 5,
    "JobExpirationTimeoutInMinutes": 30,
    "QueuePollIntervalInSeconds": 15
  },

  "Server": {
    "ServerName": "",
    "Queues": [ "default" ],
    "WorkerCount": 20
  },

  "Schedule": {
    "RefreshIntervalInMinutes": 10
  },

  "Concurrency": {
    "MaxConsecutiveSkips": 5
  },

  "Maintenance": {
    "FailedJobsRetentionInDays": 2
  },

  "Dashboard": {
    "Path": "/hangfire",
    "ReadOnly": false,
    "StatsPollingIntervalInSeconds": 30,
    "UserName": "",
    "Password": ""
  }
}
```

| Parâmetro | Uso |
|---|---|
| `TimeZone` | fuso da agenda (4.3) |
| `Storage:ConnectionStringName` | nome da connection string em `ConnectionStrings`. **`Connection`: o storage fica no mesmo banco das integrações** (decisão de 30/10/2026, 3.3). Para mudar depois, basta criar outra entrada (ex.: `Hangfire`) e apontar para ela, sem mudar código. Sai o `Replace("[catalog]", ...)` do legado |
| `Storage:SchemaName` | schema das tabelas do Hangfire (`HangFire`, o padrão da biblioteca), separado dos schemas dos módulos |
| `Storage:PrepareSchemaIfNecessary` | o Hangfire cria e atualiza as próprias tabelas na subida. As tabelas dele ficam fora do DbUp |
| `Storage:CommandBatchMaxTimeoutInMinutes`, `SlidingInvisibilityTimeoutInMinutes` | os 5 minutos do `AddHangfireService` comentado |
| `Storage:JobExpirationTimeoutInMinutes` | por quanto tempo um job concluído (`Succeeded` ou `Deleted`) fica no storage: **30 minutos** (o padrão do Hangfire é 1 dia), para as tabelas ficarem pequenas (3.3) |
| `Storage:QueuePollIntervalInSeconds` | de quanto em quanto tempo o servidor consulta as filas: **15 segundos**. Um job pode começar até 15 s depois do disparo, em troca de bem menos consultas (3.3) |
| `Server:ServerName` | nome do servidor no dashboard; vazio usa o nome da máquina |
| `Server:Queues` | filas que este servidor atende. Com mais de um servidor, cada um lista as suas (como o legado fazia com `srv-vm-app01`/`srv-vm-app02`) |
| `Server:WorkerCount` | número de workers: **20** (o legado usava 50), para não abrir conexões demais no banco com os ciclos de 3 minutos; se ajusta pelo dashboard |
| `Schedule:RefreshIntervalInMinutes` | intervalo do `SchedulerRefresh`, o job que relê `auditing.Jobs` (4.4): **10 minutos** |
| `Concurrency:MaxConsecutiveSkips` | pulos seguidos de um job até o aviso no log (4.7): **5** |
| `Maintenance:FailedJobsRetentionInDays` | depois de quantos dias um job em `Failed` sai da tela de falhas (3.4): **2** |
| `Dashboard:Path` | rota do dashboard |
| `Dashboard:ReadOnly` | dashboard só para consulta (sem disparar, reexecutar ou apagar jobs): **`false`** |
| `Dashboard:StatsPollingIntervalInSeconds` | de quanto em quanto tempo o dashboard aberto atualiza as estatísticas: **30 segundos** (o padrão é 2) (3.3) |
| `Dashboard:UserName`, `Password` | credenciais do dashboard (3.2). **Ficam vazias no `appsettings.json`**: o valor vem de variável de ambiente (`Hangfire__Dashboard__Password`) ou de user-secrets, nunca do git |

### 3.2 Autenticação do dashboard

O dashboard tem autenticação própria, independente do passo 5 (que continua adiado para a API):

1. Um `IDashboardAuthorizationFilter` do host que exige **Basic Authentication** com `Dashboard:UserName` e
   `Dashboard:Password`; sem credencial ou com credencial errada, devolve 401 e o navegador pede usuário e senha.
2. A comparação da senha é em tempo constante (`CryptographicOperations.FixedTimeEquals`).
3. Com `Enabled = true` e `UserName` ou `Password` vazios, o host não sobe (validação do 3.1): não existe dashboard
   aberto por esquecimento, nem no ambiente local.
4. Basic manda a senha em cada requisição, então o dashboard só é usado por HTTPS fora do `localhost` (o
   `UseHttpsRedirection` já está no pipeline).
5. Quando o passo 5 entrar, o filtro pode trocar o Basic pelo login do Azure AD; o dashboard segue a mesma regra.

Conferência: com `false`, o host sobe sem tocar nas tabelas do Hangfire e as rotas funcionam; com `true`, o dashboard
pede usuário e senha, recusa credencial errada e mostra o servidor com o nome, as filas e os workers configurados.

**Feito em 30/10/2026 (3.1 a 3.4).** O código ficou em `Scheduling/` (`HangfireSettings`, `HangfireSettingsValidator`,
`DashboardBasicAuthorizationFilter`, `JobExpirationTimeoutFilter` e `FailedJobsCleanupJob`), no `AddHangfireService`
do `ServicesExtensions` e no `UseHangfire` do `AppUseExtensions`. O que foi conferido, com o storage num banco
temporário do LocalDB (apagado no fim) e credenciais de teste geradas na hora, por variável de ambiente:

* `Enabled = false` (padrão): Swagger 200, `/hangfire` 404 e nenhuma linha do Hangfire no log.
* `Enabled = true` com a connection string inexistente e sem credenciais: o host não sobe, e a
  `OptionsValidationException` lista os dois problemas.
* `Enabled = true` completo: as 11 tabelas do Hangfire criadas no schema `HangFire`; dashboard com 401 e
  `WWW-Authenticate: Basic` sem credencial, 401 com senha errada e 200 com a certa; servidor com o nome, a fila
  `default` e 20 workers; `data-pollinterval="30000"` no dashboard; `FailedJobsCleanup` registrado com
  `15 * * * *` e `America/Sao_Paulo`.
* Retenção: um job concluído recebeu `ExpireAt` 30 minutos depois de terminar.
* Limpeza: dois jobs com falha; com a falha de um deles recuada 3 dias, a limpeza passou só esse para `Deleted` (com
  `ExpireAt` de 30 minutos) e o outro continuou na tela de Failed.
* Build sem erros: 1420 avisos, os 1422 do passo 2 menos os 2 do `AddHangfireService` antigo, nenhum novo.

Achados durante a implementação:

* `Server:Queues` não tem valor padrão na classe: o binder do .NET soma os itens do `appsettings.json` aos que já estão
  num array, e a fila saía repetida (`["default","default"]`). O servidor ainda usa `Distinct()` por segurança.
* A data de falha que a API de monitoramento do SQL Server devolve (`FailedAt`) é a data em que o job entrou em
  `Failed` (`HangFire.State.CreatedAt`), não o `FailedAt` gravado no JSON do estado.
* `SetDataCompatibilityLevel` passou de `Version_170` (legado) para `Version_180`: o storage nasce agora, sem dados
  antigos para ler.
* Não há `WithJobExpirationTimeout` no Hangfire 1.8.23; a retenção é um filtro (`IApplyStateFilter`) que só tem efeito
  nos estados finais.
* O filtro de concorrência do 4.7 ainda não existe: até o passo 4, duas limpezas ao mesmo tempo só fariam a segunda não
  achar nada para apagar (a troca de estado exige que o job ainda esteja em `Failed`).

### 3.3 Carga do Hangfire no banco

**Cache não alivia o Hangfire.** O que o Hangfire faz no banco não é leitura repetida, é gravação de estado: cada
execução passa por `Enqueued`, `Processing` e `Succeeded`, e cada passagem grava em `Job`, `State` e `JobParameter`.
Além disso ele consulta as filas, confere a agenda e manda heartbeats. Esse estado tem de ser durável e visto por todos
os servidores; guardá-lo num cache quebraria as garantias dele.

**Tamanho da carga.** Pela agenda atual, cerca de 40 execuções por minuto, quase todas dos jobs de 3 minutos: algumas
centenas de gravações pequenas por minuto, pouco para um SQL Server. O que costuma pesar são as tabelas crescendo e o
dashboard aberto. Medir no protótipo antes de mexer.

**Alavancas, em ordem:**

1. **Storage fora do banco das integrações.** Já previsto: `Storage:ConnectionStringName` apontando para outra entrada
   de `ConnectionStrings`. Um banco separado na mesma instância evita disputa de tabelas e de log com os módulos e o DW;
   uma instância separada isola também CPU e disco.
2. **Retenção do histórico.** Um job concluído fica guardado por 1 dia por padrão; com 40 execuções por minuto, são cerca
   de 57 mil jobs. Um filtro que reduz o `JobExpirationTimeout` (parâmetro novo, ex.:
   `Storage:JobExpirationTimeoutInHours`) mantém as tabelas pequenas.
3. **Intervalo de consulta das filas** (`QueuePollInterval`, ex.: `Storage:QueuePollIntervalInSeconds`): alguns
   segundos trocam um pouco de atraso no início de cada job por menos consultas.
4. **Dashboard.** Ele atualiza as estatísticas a cada 2 segundos enquanto está aberto: aumentar o `StatsPollingInterval`
   (ex.: `Dashboard:StatsPollingIntervalInSeconds`) e não deixá-lo aberto à toa.
5. **Storage em memória** (`Hangfire.InMemory`, pacote oficial), como alternativa: o Hangfire não toca no banco. Serve
   aqui porque a agenda é remontada de `auditing.Jobs` pelo `SchedulerRefresh` (4.4) e porque a trava da opção B (4.7)
   fica no banco das integrações, não no storage. O preço: só um servidor, o histórico do dashboard some a cada
   reinício e uma execução em andamento quando o processo cai não é retomada (volta no próximo disparo). Viável enquanto
   tudo roda num servidor só (4.9); precisa ser trocado quando vier o segundo.

**Onde cache ajuda: nos services.** Se cada execução busca no banco dados que quase não mudam (empresas, tokens,
parâmetros), o padrão `docs/PADRAO-CACHE-INTEGRACOES.md` (`IMemoryCache`) tira essas consultas do banco das integrações
a cada 3 minutos. O `SchedulerRefresh` já funciona como cache da agenda: lê `auditing.Jobs` a cada 10 minutos, não a
cada execução.

**Decisão (30/10/2026):** o storage fica **no mesmo banco das integrações** (`Storage:ConnectionStringName =
"Connection"`), com a carga reduzida pelos itens 2 a 4:

| Alavanca | Valor | Padrão do Hangfire |
|---|---|---|
| Retenção dos jobs concluídos (`Storage:JobExpirationTimeoutInMinutes`) | 30 minutos | 1 dia |
| Consulta das filas (`Storage:QueuePollIntervalInSeconds`) | 15 segundos | consulta contínua |
| Atualização do dashboard (`Dashboard:StatsPollingIntervalInSeconds`) | 30 segundos | 2 segundos |

Com 30 minutos de retenção, o dashboard mostra só a última meia hora de execuções concluídas; o histórico que importa
continua em `auditing.Logs` e `auditing.Messages`. A retenção vale para os estados finais (`Succeeded` e `Deleted`, que
inclui os pulos do 4.7); um job em `Failed` não expira sozinho; quem o tira da tela é a limpeza do 3.4, depois de 2 dias.
Banco separado e storage em memória ficam como alternativas, sem código, se a carga pesar.

### 3.4 Limpeza da tela de Failed

Com `AutomaticRetry` em 0 tentativas, toda execução que falha vai para o estado `Failed` e fica lá: `Failed` não é um
estado final, então a retenção do 3.3 não vale para ele, e a tela de falhas do dashboard cresce sem limite. Um job do
próprio Hangfire limpa essa tela.

1. **O job.** `FailedJobsCleanup`, um recurring job do host, registrado na subida junto com o `SchedulerRefresh` (4.4).
   Como ele, não está em `auditing.Jobs`, não pode ser desligado pela tabela e só some com `Hangfire:Enabled = false`.
2. **Quando roda.** A cada hora, no minuto 15 (`15 * * * *`, no fuso de `Hangfire:TimeZone`), fora dos minutos cheios
   em que disparam os jobs de 60 e 120 minutos e longe das 23:00 dos diários.
3. **O que faz.** Pela API de monitoramento do Hangfire (`JobStorage.Current.GetMonitoringApi().FailedJobs(...)`), lê os
   jobs em `Failed` em lotes (ex.: 500 por vez) e, para cada um com `FailedAt` mais antigo que
   `Maintenance:FailedJobsRetentionInDays` (2 dias), chama `BackgroundJob.Delete(id)`. O job passa para `Deleted`, que é
   final, e sai do storage depois dos 30 minutos de retenção do 3.3.
4. **O que se perde.** Só a linha do dashboard. O erro de cada execução continua em `auditing.Logs` e
   `auditing.Messages`, gravado pelo service; os 2 dias são o tempo para alguém ver a falha no dashboard e reexecutar,
   se quiser.
5. **Concorrência.** Passa pelo filtro do 4.7 com a chave `job:failed-jobs-cleanup`: duas limpezas nunca rodam juntas,
   nem com dois servidores.
6. **Log.** Cada execução registra quantos jobs foram removidos e a data de falha mais antiga que ficou.

Conferência: com o Hangfire ligado, um job de teste que lança exceção aparece em `Failed`; com
`FailedJobsRetentionInDays` reduzido no ambiente local (ou a data de falha simulada), a limpeza seguinte o move para
`Deleted`, e uma falha mais nova que o limite continua na tela.

## Passo 4 — Agenda dos jobs recorrentes (a partir de `auditing.Jobs`)

Só vale com `Hangfire:Enabled = true`. A agenda vem inteira de `auditing.Jobs`: a tabela diz **se** o job roda e
**quando**; o host diz **o que** ele executa. Mudar a agenda é mudar a tabela, sem deploy nem reinício (4.4).

### 4.1 Colunas novas em `auditing.Jobs`

| Coluna | Tipo | Uso |
|---|---|---|
| `IsActive` | `BIT NOT NULL DEFAULT 0` | **decide se o job entra na agenda**. Com 0, o job fica fora, qualquer que seja o intervalo. Substitui a chave por módulo no `appsettings.json` |
| `RunningDailyAt` | `TIME(0) NULL` | **preenchida, o job é diário** e roda uma vez por dia nessa hora, no fuso de `Hangfire:TimeZone` (4.3). Nula, o job segue `IntervalTimeInMinutes` |
| `Queue` | `NVARCHAR(50) NOT NULL` | fila do Hangfire em que o job entra. Sem `DEFAULT`: todo insert diz a fila |

1. As colunas entram no `CREATE TABLE [auditing].[Jobs]` do script original
   (`Shared/Infrastructure.Shared/Scripts/SQLServer/Script001_Create_Schemas_Procedures_And_Tables.sql`), não num
   `ALTER` novo, como no padrão do repositório. Num banco que já rodou o script, a mudança é aplicada à parte (o DbUp não
   reexecuta um script que já está no journal).
2. Um `CHECK` mantém o diário coerente: `RunningDailyAt IS NULL OR IntervalTimeInMinutes = 1440`. Os diários continuam
   com 1440 no intervalo, para a tabela continuar legível sozinha, e a hora vem de `RunningDailyAt`.
3. Um `CHECK` no formato da fila: só minúsculas, dígitos, `_` e `-` (o Hangfire recusa outros caracteres). A fila padrão
   é `default`; um job numa fila que nenhum servidor atende (`Server:Queues`, 3.1) nunca roda, e o `SchedulerRefresh`
   (4.4) avisa no log.
4. `IsActive` nasce 0: um banco novo não começa rodando os cerca de 300 jobs de uma vez. Liga-se job a job (ou módulo
   a módulo, pela faixa de `IdJob`) com `UPDATE`. Por ter `DEFAULT`, ela não precisa estar nos scripts de insert.
5. `RunningDailyAt` e `Queue` **obrigam a mudar os scripts de insert de todos os módulos** (4.6): `Queue` porque é
   `NOT NULL` sem `DEFAULT`, e `RunningDailyAt` porque os diários precisam da hora. Para os scripts ficarem uniformes,
   as duas colunas entram na lista de colunas de todos os `INSERT`, com `NULL` em `RunningDailyAt` nos não diários.
6. Os diários (1440) ganham `RunningDailyAt`. São 48: as 44 leituras diárias do LinxMicrovix,
   `LinxCommerceSearchOrderStatus` e `LinxCommerceSearchSalesRepresentative` (7001 e 7005),
   `TotvsPostSearchProductCodes` (13097) e `DataWarehouseDaily` (9001). Todos com `RunningDailyAt = '23:00'`
   (decisão de 30/10/2026). Com o mesmo horário, os 48 disparam no mesmo minuto; se pesar, a hora de cada um muda na
   tabela, sem código.
7. Só os jobs de integração (`IdType = 2`) entram na agenda. Os de procedure (`IdType = 1`, as `sp_<Tabela>_Sync`) ficam
   fora: quem chama cada uma é o service da leitura, pelo `SharedRepository`, logo depois do bulk insert. Eles estão na
   tabela para o log (`auditing.sp_Log_Start` procura o job pelo `Title` e pelo `IdType`), e recebem `RunningDailyAt`
   nula e `Queue = 'default'`, assim como os sob demanda (intervalo 0), que rodam pelos controllers.

### 4.2 Do registro da tabela ao cron

1. Um mapa por módulo `IdJob → método do service` (em `Scheduling/RecurringJobs/<Módulo>RecurringJobs.cs`), porque os
   títulos dos jobs nem sempre são o nome do método (`MovideskGetTickets` → `GetTickets()`).
2. Para cada job ativo (`IdType = 2` e `IsActive = 1`) que está no mapa:

   | Caso | Cron |
   |---|---|
   | `RunningDailyAt` preenchida | `mm HH * * *`, da `RunningDailyAt` |
   | intervalo que divide a hora (3 → `*/3 * * * *`) | a cada N minutos |
   | intervalo em horas que divide o dia (60 → `0 * * * *`, 120 → `0 */2 * * *`) | a cada N horas, no minuto 0 |
   | qualquer outro (ex.: 7, 90, 43200) | não entra na agenda; aviso no log com o `IdJob` |

   O job é registrado na fila de `Queue`.
3. Job ativo sem entrada no mapa: aviso no log. Job da agenda que ficou inativo, com intervalo 0 ou saiu do mapa:
   removido do Hangfire.
4. Nenhum job roda duas vezes ao mesmo tempo: o mecanismo de concorrência do 4.7 pula a execução quando a anterior
   ainda não terminou.
5. Os jobs de 60 e 120 minutos continuam disparando todos no minuto 0 (as 154 leituras de 120 minutos, 123 delas do
   LinxMicrovix e 30 da Totvs, às 00:00, 02:00...). Se isso pesar no banco ou nas APIs, a saída é passar parte deles
   para diários com hora própria (`RunningDailyAt`) ou para outra fila.

### 4.3 Fuso horário

O cron do Hangfire é em UTC por padrão. A agenda usa `America/Sao_Paulo`, num parâmetro `Hangfire:TimeZone` da seção do
3.1 (`"TimeZone": "America/Sao_Paulo"`), passado em todo `AddOrUpdate`. A `RunningDailyAt` vale nesse fuso, e o
`DataWarehouseService` já usa o horário de Brasília para o "dia 1" do estoque mensal.

### 4.4 Job que relê a tabela

A agenda é montada por um job do próprio Hangfire (`SchedulerRefresh`), e não só na subida:

1. Roda na subida do host e depois a cada `Hangfire:Schedule:RefreshIntervalInMinutes` (10), parâmetro novo
   na seção do 3.1.
2. Lê `auditing.Jobs`, compara com a agenda atual do Hangfire e faz só a diferença: inclui, altera o cron ou a fila e
   remove (4.2). Um `UPDATE` na tabela vale, no máximo, no próximo ciclo.
3. Avisa no log os jobs ativos com fila fora de `Server:Queues`.
4. Passa pelo mecanismo de concorrência (4.7) e registra cada execução no log, com o que entrou, mudou e saiu.
5. Ele mesmo não está em `auditing.Jobs` nem pode ser desligado pela tabela; roda na fila `default` e só some com
   `Hangfire:Enabled = false`.

### 4.5 Job 9003 (`DataWarehouseMonthlyStock`)

É a entrada `RunMonthlyStock(positionDate)` do `DataWarehouseService`: refaz a posição de estoque de um fim de mês
(`FactMonthlyStock`) a partir do histórico de saldos, chamando `datawarehouse.sp_FactMonthlyStock_Load`; sem data,
refaz o último dia do mês anterior. Serve para **reprocessar** um mês. A carga mensal normal já roda dentro do
`DataWarehouseDaily` (9001), no dia 1 de cada mês (horário de Brasília).

A documentação do DW o trata como manual, mas o `Script007_Insert_DataWarehouse_Jobs_Into_Jobs_Table.sql` grava
`IntervalTimeInMinutes = 43200`. Pela regra do 4.2 ele já ficaria fora da agenda (43200 não vira cron), por isso
**ele passa para 0 no script original** (decisão de 30/10/2026), como o 9004, e continua disponível pela rota
`POST DataWarehouseJobs/DataWarehouse/RunMonthlyStock`. A correção entra na revisão do 4.6.

### 4.6 Revisão de todos os scripts de insert em `auditing.Jobs`

As colunas `RunningDailyAt` e `Queue` (4.1) quebram todo `INSERT INTO [auditing].[Jobs]` que não as lista, então este
passo vem **junto** com a mudança do `CREATE TABLE`, no mesmo commit, e antes de qualquer migrator rodar com a tabela
nova. Nenhum script pode ficar de fora.

**Inventário (30/10/2026): 61 scripts.**

| Grupo | Scripts | Situação |
|---|---|---|
| LinxMicrovix Outbound | 29 | ativos |
| Totvs | 12 | ativos |
| AfterSale | 4 | ativos |
| LinxCommerce | 4 | ativos |
| Movidesk | 3 | ativos |
| DataWarehouse | 1 (`Parameters/Script007`, com três `insert` em minúsculas e o job 9003) | ativo |
| Dootax, Mobsim, Oto, Pagarme, FlashCourier, Jadlog, Stone e TotalExpress | 8 (`Script002_Insert_<Módulo>_Values_Into_Jobs_Table.sql`) | o `INSERT` inteiro comentado (`--INSERT`) |

O inventário se refaz na hora de executar, porque podem ter entrado scripts novos (a busca não diferencia maiúsculas,
por causa do DW):

```bash
grep -rliE --include=*.sql "insert into \[?auditing\]?\.\[?jobs\]?" src/Backend
```

**Em cada script:**

1. A lista de colunas passa a ser `(IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)`.
2. Toda linha de `VALUES` ganha os dois valores, na mesma posição: `NULL, 'default'` nos não diários e `'23:00', 'default'`
   nos diários. Todos os jobs ficam na fila `default` (4.9).
3. Os jobs com `IntervalTimeInMinutes = 1440` recebem a `RunningDailyAt` (4.1, item 6).
4. Nos 8 scripts comentados, a mesma mudança dentro do comentário, para o módulo já nascer certo quando for migrado
   (passo 6).
5. O 9003 do DW passa de 43200 para 0 (4.5).

**Conferência, depois de editar:**

1. Contagem: o mesmo número de scripts da busca acima aparece na busca pela lista nova de colunas, e nenhum fica com a
   lista antiga:

   ```bash
   grep -rliE --include=*.sql "insert into \[?auditing\]?\.\[?jobs\]?.*IntervalTimeInMinutes, IdType\)" src/Backend
   ```

   tem que voltar vazio.
2. Cada linha de `VALUES` tem 8 valores (um script de conferência conta as vírgulas fora das aspas por linha).
3. Os 48 diários têm `RunningDailyAt`, e nenhum job fora de 1440 tem.
4. Rodar os migrators de todos os módulos num banco local vazio (Shared primeiro) sem erro, e consultar:

   ```sql
   SELECT IdJob, Title, IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive
   FROM [auditing].[Jobs]
   WHERE IdType = 2 AND (RunningDailyAt IS NOT NULL OR IntervalTimeInMinutes NOT IN (0, 3, 60, 120));
   ```

   tem que listar só os 48 diários.
5. Comparar a quantidade de linhas de `auditing.Jobs` por `IdType` com a de antes da mudança: nenhum job some ou
   aparece.
6. Atualizar os documentos que mostram o `INSERT` de jobs (o `PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md` e os README dos
   módulos), para os próximos scripts já nascerem com as colunas novas.

### 4.7 Concorrência na execução dos jobs

**O problema.** Um recurring job do Hangfire não sabe se a execução anterior terminou: a cada disparo do cron, ele
enfileira uma execução nova. Se `LinxClientesFornec` roda a cada 3 minutos e uma execução (a `xpto`) passa de 3 minutos,
por lentidão na API ou no banco, o disparo seguinte começa outra execução do mesmo job **enquanto a `xpto` ainda roda**.
As duas leem a mesma janela, fazem bulk insert nas mesmas tabelas `untreated`, chamam as mesmas `sp_<Tabela>_Sync` e
gravam o mesmo parâmetro de última leitura. O resultado vai de trabalho dobrado na API a deadlock e watermark
regredindo. Com a API ou o banco lentos, cada execução nova pesa mais, e as execuções se acumulam.

**O que o Hangfire oferece, e por que não basta:**

| Recurso | O que faz | Por que não resolve |
|---|---|---|
| `[DisableConcurrentExecution(timeoutInSeconds)]` | trava distribuída pelo nome do método: a segunda execução **espera** a primeira terminar, até o timeout, e depois falha | não pula, espera. Enquanto espera, ocupa um worker; com 3 minutos e uma API travada, os workers vão sendo tomados por execuções paradas na fila da trava. Ao passar do timeout, a execução falha (`DistributedLockTimeoutException`), o que, com `AutomaticRetry = 0`, enche o dashboard de falhas que não são erro. E a trava é **por método**, não por argumento: num executor genérico (`Run(idJob)`), todos os jobs dividiriam a mesma trava |
| `MisfireHandling` do recurring job (1.8) | o que fazer com os disparos perdidos enquanto o servidor estava fora | trata disparos perdidos, não execuções sobrepostas |
| `SlidingInvisibilityTimeout` do storage | uma execução longa não é entregue a outro worker enquanto o servidor que a pegou está vivo | evita a mesma execução rodar duas vezes, não duas execuções do mesmo job |
| Hangfire.Throttling (mutex e semáforo) | o que precisamos, inclusive "descartar se já está rodando" | é do Hangfire Pro, pago |

**Em avaliação.** O mecanismo ainda não está escolhido. As alternativas abaixo foram comparadas, e a opção A está
detalhada mais adiante porque foi a primeira proposta, não porque ganhou.

| Opção | Como funciona | A favor | Contra |
|---|---|---|---|
| **A. Filtro no Hangfire que pula** (`SkipConcurrentExecutionFilter`) | antes de executar, trava distribuída no storage do Hangfire com espera zero; ocupada, a execução vai para `Deleted` | só o host muda; nada nos módulos; o pulo aparece no dashboard | a trava vive no storage do Hangfire: não vale para as rotas dos controllers, nem no modo só API, nem nos hosts Quartz e AzureWebJob; o cancelamento de dentro do filtro precisa ser provado num protótipo |
| **B. Trava da aplicação, no banco das integrações** | um `IJobLock` no Shared, com `sp_getapplock` (espera zero, preso à sessão) no banco dos módulos, chave por job ou grupo. Um executor único do host (`JobRunner`) pega a trava e chama o service; as rotas recorrentes passam pelo mesmo executor | **vale para qualquer disparo**: Hangfire, rota manual (resolve a decisão das rotas: ocupado = 409), modo só API e os outros hosts; sem trava órfã, como na A | código novo no Shared; o pulo, sozinho, aparece como `Succeeded` no dashboard (o executor registra o pulo no `auditing` e, se quiser, a A entra só para marcar o estado) |
| **C. Job que se reagenda** (atraso fixo) | sem recurring job: ao terminar, com sucesso ou erro, a execução agenda a próxima para daqui a `IntervalTimeInMinutes` (`BackgroundJob.Schedule`) | sobreposição impossível por construção, sem trava; o intervalo passa a ser "3 minutos de descanso", não "a cada 3 minutos" | a corrente pode quebrar (exceção antes do reagendamento, job apagado no dashboard) e precisa de um vigia (o `SchedulerRefresh`); os diários continuam precisando de cron; a página de recurring jobs do dashboard fica vazia; duas correntes do mesmo job nascem fácil se o vigia errar, e aí volta a precisar de trava |
| **D. Checar `auditing.Logs` antes de rodar** | não roda se houver execução do job com `StartDate` e sem `EndDate` | usa a tabela que já existe | uma execução que cai deixa `EndDate` nulo e bloqueia o job até alguém limpar (precisa de prazo de validade); duas execuções que checam ao mesmo tempo passam as duas (precisa de trava de qualquer forma) |
| **E. `DisableConcurrentExecution`** (nativo) | a segunda execução espera a primeira | nativo, uma linha | espera em vez de pular, ocupa worker, falha no timeout, trava por método (tabela acima) |
| **F. Hangfire Pro (Throttling)** | mutex com "descartar se rodando" | pronto e mantido | pago |
| **G. Quartz.NET** (`[DisallowConcurrentExecution]`) | o agendador não dispara o job enquanto a execução anterior roda; com a instrução de misfire "não fazer nada", o disparo perdido é descartado. Em cluster (JobStore no banco), vale entre servidores | nativo, sem código de trava; o repositório já tem o host Quartz | é trocar de agendador: sem o dashboard do Hangfire, e o plano inteiro do passo 3 e 4 muda; também não cobre as rotas manuais |

**Leitura da comparação.** A, B e G resolvem o caso do job de 3 minutos. A diferença é **onde a regra mora**:

* na A, no Hangfire: protege só o que o Hangfire dispara;
* na B, na aplicação: protege o job, venha o disparo de onde vier. Com três hosts no repositório (Hangfire, Quartz e
  AzureWebJob) e rotas manuais para cada leitura recorrente, é a que não precisa ser refeita em cada um;
* na G, no agendador: nativa, mas só se o Quartz for o agendador.

**Decisão (30/10/2026): combinação de A com B.** A trava é a da B, no banco das integrações e no Shared; quem a usa é
o filtro da A, que também marca o pulo no dashboard. As rotas dos controllers **ficam fora da trava**. O desenho está
logo abaixo.

**Opção B e o banco (deadlock e carga com o DW).** O DW fica no mesmo banco das integrações, então a pergunta é se a
trava da B pode travar o fluxo dele. Não pode:

1. **A trava não encosta nas tabelas.** `sp_getapplock` cria um lock do tipo `APPLICATION` sobre um nome
   (`job:8006`), não sobre tabela, página ou linha. Os locks do DW são de linha, página e tabela das landings, fatos e
   dimensões. Lock só conflita com lock do mesmo recurso: a trava de um job nunca bloqueia nem é bloqueada pelo DW. Só
   outro pedido de `job:8006` disputa `job:8006`.
2. **Com espera zero, ela não entra em deadlock.** Deadlock é um ciclo de sessões em que cada uma espera a outra. Com
   `@LockTimeout = 0`, o pedido nunca espera: consegue na hora ou volta "ocupado" na hora. Os locks que o job pega
   depois (bulk insert, `sp_*_Sync`) são os mesmos de hoje, com ou sem a trava.
3. **Sem transação aberta durante o job.** A trava é da sessão (`@LockOwner = 'Session'`), não de uma transação: não
   segura log nem prende linhas.
4. **Carga pequena.** Cerca de 40 chamadas rápidas por minuto, cada uma uma entrada na memória do lock manager. O custo
   real é uma conexão parada por job em execução, no máximo uma por worker (20). A opção A faz o mesmo `sp_getapplock`,
   no storage do Hangfire, que pelo 3.1 está no mesmo banco: A e B não diferem em carga. Quem pesa mais é o próprio
   Hangfire (3.3).
5. **A trava diminui o risco.** O que pode gerar deadlock com o DW são duas execuções do mesmo job escrevendo nas mesmas
   `untreated` e `trusted` enquanto o DW lê essas tabelas, e é isso que a trava impede. A disputa entre uma integração
   gravando e o DW lendo continua como hoje, e não vem da trava.

Cuidados na implementação:

* a trava sempre no mesmo banco (o Shared tem `Connection` e `AuditingConnection`; o lock vale no banco em que foi
  pedido);
* `sp_releaseapplock` num `finally`, antes de devolver a conexão ao pool;
* uma conexão só para a trava, separada das do job, para o timeout de uma consulta do job não derrubar a trava.

No protótipo: com um job de teste rodando junto com uma carga do DW, `sys.dm_tran_locks` com
`resource_type = 'APPLICATION'` mostra só as travas dos jobs, e a sessão do DW em `sys.dm_exec_requests` não aparece
bloqueada por elas.

**A combinação em detalhe**

1. **A trava (da B).** Uma abstração `IJobLock` no `Application.Shared`, implementada no `Infrastructure.Shared` com
   `sp_getapplock` no banco das integrações (`Connection`): espera zero (`@LockTimeout = 0`), presa à sessão
   (`@LockOwner = 'Session'`), numa conexão só dela, aberta durante a execução e fechada no fim. Se o processo cair, a
   conexão fecha e a trava é solta sozinha, sem trava órfã. Por estar no Shared e no nosso banco, ela não depende do
   storage do Hangfire (vale igual se o storage mudar, 3.3) e pode ser usada pelos hosts Quartz e AzureWebJob.
2. **O filtro (da A).** Um filtro global do Hangfire no host (`SkipConcurrentExecutionFilter`, `IServerFilter` +
   `IElectStateFilter`), registrado uma vez no `AddHangfire` e aplicado a todos os jobs da agenda. Como o filtro global
   é uma instância única, ele obtém o `IJobLock` por uma fábrica (não por injeção scoped).
3. **Antes de executar** (`OnPerforming`), o filtro pede a trava ao `IJobLock`.
4. **Conseguiu:** a execução roda, e o filtro solta a trava ao terminar (`OnPerformed`), com sucesso ou erro.
5. **Não conseguiu** (outra execução do mesmo job ainda está rodando, neste ou em outro servidor): a execução **não
   roda** e vai para o estado `Deleted` no dashboard, com o motivo `Skipped: previous execution of <IdJob> still
   running`. O worker fica livre na hora, e o disparo seguinte do cron tenta de novo. A forma de cancelar a execução e
   trocar o estado de dentro do filtro é a primeira coisa a provar no protótipo.
6. **Chave por job, não por método:** `job:<IdJob>`. Os jobs que dividem dados entram num **grupo**, com a chave do
   grupo, definido no mapa do 4.2 (ex.: os cinco jobs do DW na chave `group:datawarehouse`, porque escrevem nas mesmas
   landings; hoje isso é o `SemaphoreSlim` estático do `DataWarehouseService`, que não vale entre processos).
7. O `SchedulerRefresh` (4.4) passa pelo mesmo filtro, com a chave `job:scheduler-refresh`.
8. O filtro substitui o `DisableConcurrentExecution` que o plano previa nos itens 4.2 e 4.4.
9. **As rotas ficam fora** (decisão de 30/10/2026): os controllers continuam chamando o service direto, sem a trava.

Pular é seguro quando a leitura trabalha por janela: a execução seguinte lê a partir da última data gravada e pega o
que a pulada leria. Isso precisa ser conferido em cada service antes de ligar o `IsActive` dos seus jobs; uma leitura
que não guarda a última data perde, no máximo, um ciclo, mas é bom saber quais são.

**O que vem junto:**

1. **Execução travada.** Se uma execução fica presa (API que não responde), ela segura a trava, e todos os disparos
   seguintes são pulados enquanto ela não termina. A defesa é o timeout de cada chamada: o `HttpClient` de cada módulo
   (o legado usava 20 minutos) e o `commandTimeout` das procedures (`StoredProcedureTimeoutSeconds` no
   `SharedRepository`). A revisão confere esses timeouts em todos os módulos registrados no host.
2. **Aviso de pulos seguidos.** O filtro conta os pulos seguidos por job e grava um aviso no log quando passam de um
   limite (`Hangfire:Concurrency:MaxConsecutiveSkips`, 5, ou seja, 15 minutos sem rodar um job de 3 minutos).
   Um job que vive sendo pulado está lento demais para o intervalo, e a correção é aumentar o intervalo na tabela.
3. **Disparo manual pelas rotas.** As rotas ficam fora da trava: um
   `POST LinxMicrovixJobs/LinxMicrovix/LinxClientesFornec` durante a execução agendada roda junto com ela. As rotas são
   para uso manual e consciente, e o README do host (passo 7) avisa disso. O DW continua com a própria trava no
   processo (409), como hoje.
4. **Workers.** Com o pulo, uma execução nunca espera outra, então o `WorkerCount` (3.1) só precisa cobrir os jobs
   rodando ao mesmo tempo, e não as filas de espera.

Conferência (protótipo): um job de teste que dorme mais que o intervalo; com o Hangfire ligado, as execuções
seguintes aparecem como `Deleted` com o motivo de pulo, nenhuma roda em paralelo (nem com dois processos no mesmo
banco), o aviso sai depois do limite de pulos, derrubar o processo no meio solta a trava e, com uma carga do DW rodando
junto, `sys.dm_tran_locks` mostra só as travas `APPLICATION` dos jobs, sem sessão do DW bloqueada por elas.

### 4.8 Execução (feito em 30/10/2026)

O que entrou:

* **Banco:** `IsActive`, `RunningDailyAt` e `Queue` no `CREATE TABLE [auditing].[Jobs]` do Script001 do Shared, com o
  `DF_Jobs_IsActive` e os `CHECK` `CK_Jobs_RunningDailyAt` e `CK_Jobs_Queue`. Os 61 scripts de insert com a lista nova de
  colunas (993 linhas de `VALUES`, inclusive as comentadas dos 8 módulos fora do host): 48 diários com `'23:00'`, todos
  em `'default'`, e o 9003 com 0. O insert de `auditing.Apps` do Script007 do DW, no mesmo arquivo, ficou como estava.
* **Shared:** `IJobLock` (`Application.Shared`) e `SqlServerJobLock` (`Infrastructure.Shared/Locks`, `sp_getapplock` com
  `Microsoft.Data.SqlClient`, que já vem pelo `dbup-sqlserver`); `JobSchedule` e `IJobScheduleRepository`/
  `JobScheduleRepository`, que lê `auditing.Jobs` pela conexão de auditoria (a mesma do `LoggerService`).
* **Host (`Scheduling/`):** os mapas dos 6 módulos (327 jobs: Totvs 43, Movidesk 3, AfterSale 12, Linx Commerce 6, Linx
  Microvix 260 e DW 3, o DW com a chave `group:datawarehouse`), o `RecurringJobMap`, o `JobCron`, o `SchedulerRefreshJob`
  e o `SkipConcurrentExecutionFilter`. Os recurring jobs da agenda se chamam `<IdJob>-<Title>`.

Como os mapas foram feitos: para cada job recorrente dos scripts, o método sem parâmetros do service que grava aquele
`EnumJob` no log. Os 327 deram exatamente um método cada; todos estão na interface do service e têm rota no host. Todos
são leituras (os `Post*` da Totvs são buscas).

O que foi conferido, num banco vazio do LocalDB (apagado no fim):

* Migrators de todos os módulos (Shared, Totvs, Linx Microvix, Linx Commerce, Movidesk, AfterSale e DW) sem erro.
* `auditing.Jobs`: 498 linhas de procedure e 462 de integração, as mesmas contagens dos scripts do commit anterior; 327
  recorrentes, todos em `default` e inativos; a consulta do 4.6 devolveu só os 48 diários, todos 1440 e 23:00; 9003 com 0.
  Os três `CHECK` recusaram um diário com 60, uma fila com maiúscula e um insert sem fila.
* Host com o Hangfire ligado nesse banco: o `SchedulerRefresh` rodou na subida (agenda vazia). Com 8000 (120 min), 8034
  (diário), 9002 (3 min) e 8001 (intervalo 7) ativos, a releitura registrou `0 */2 * * *`, `0 23 * * *` e
  `*/3 * * * *`, em `default` e `America/Sao_Paulo`, e deixou o 8001 fora com aviso. Desativados, os três saíram da agenda.
* Trava: com `group:datawarehouse` presa por outra sessão, o 9002 foi para `Deleted` com
  `Skipped: previous execution of group:datawarehouse still running`; com a trava solta, rodou (`Succeeded`). Cinco pulos
  seguidos deram o aviso (`warn`) no log; o contador zerou quando o job rodou. Nenhuma trava `APPLICATION` sobrou no banco.
* Build sem erros: 1420 avisos, os mesmos do passo 3.

Achados:

* O Hangfire 1.8 já leva a execução cancelada no `OnPerforming` para `Deleted`, com o motivo genérico
  `Canceled by filter`; o `IElectStateFilter` só troca o motivo pelo do pulo.
* O contador de pulos seguidos é por processo: com dois servidores, cada um conta os seus.
* O `dotnet-script` estoura o limite de caminho do Windows no cache dos migrators das pastas mais fundas (Linx Commerce,
  Movidesk, AfterSale, DW). Rodar com `DOTNET_SCRIPT_CACHE_LOCATION` apontando para uma pasta curta resolve.
* **Bancos que já rodaram os scripts** (o `Company` local e qualquer outro): o DbUp não reaplica o Script001 do Shared nem
  os scripts de jobs dos módulos, então esses bancos ficam sem as colunas novas e o host, com o Hangfire ligado, falha ao
  ler a agenda. Eles precisam de um ajuste à parte: as três colunas (com `Queue` preenchida com `'default'` antes do
  `NOT NULL`), os dois `CHECK`, `RunningDailyAt = '23:00'` nos 48 diários e o 9003 com 0.

### 4.9 Decisões

Tomadas em 30/10/2026:

1. `RunningDailyAt = '23:00'` nos 48 diários.
2. Todos os jobs na fila `default` até existir o segundo servidor; `Server:Queues` fica `[ "default" ]`.
3. Concorrência: combinação de A com B (4.7), com a trava no Shared e no banco das integrações, usada pelo filtro do
   host.
4. As rotas dos controllers ficam fora da trava.
5. Storage do Hangfire no mesmo banco das integrações, com retenção de 30 minutos, consulta das filas a cada 15
   segundos e dashboard atualizando a cada 30 segundos (3.3).
6. O job 9003 passa de 43200 para 0 no script original do DW (4.5).
7. Valores iniciais (3.1): `WorkerCount` 20, `Schedule:RefreshIntervalInMinutes` 10,
   `Concurrency:MaxConsecutiveSkips` 5 e `Dashboard:ReadOnly` `false`.
8. Um job do próprio Hangfire limpa a tela de `Failed` depois de 2 dias (3.4).

## Passo 5 — Autenticação (não implementar agora)

Fica fora deste plano por enquanto: o `[Authorize]` continua comentado e o JWT do Azure AD desligado. Quando entrar:

1. Reativar o JWT do Azure AD e o `[Authorize]` nos controllers ativos.
2. `AuthController`: a configuração `AzureAd` (Instance, TenantId e o escopo da API) fora do código e do git.
3. Swagger com OAuth2 client credentials, no lugar do bloco comentado no `BuilderExtensions`.

O dashboard do Hangfire não espera este passo: ele já tem autenticação própria (3.2).

## Passo 6 — Módulos que ainda não estão no host

Dootax, Jadlog, Stone, TotalExpress, FlashCourier, Oto, Mobsim e Pagarme entram um a um, conforme cada módulo for
migrado. Para cada um: registro no `ServicesExtensions`, referência no `.csproj`, controller, mapa de jobs recorrentes
(passo 4) e o script de insert de jobs descomentado, já com `RunningDailyAt` e `Queue` (4.6).
Os controllers vazios desses módulos ficam como estão até lá.

## Passo 7 — Documentação do host

`README.md` do host: como rodar como só API ou com o Hangfire (`Hangfire:Enabled`), configuração, rotas por módulo,
como a agenda funciona e como ligar ou desligar um job em `auditing.Jobs`.

**Feito em 30/10/2026:** [README.md](../README.md), com os dois modos, como rodar, a estrutura do projeto, a configuração
(seção `Hangfire` inteira), as rotas por módulo com o link do README de cada um, a agenda (colunas, conversão em cron,
`SchedulerRefresh` e os `UPDATE` para ligar e desligar jobs), a concorrência, o dashboard e a manutenção, e o que fazer
num job ou módulo novo. O README da raiz aponta para ele. Os links relativos foram conferidos.
