<div align="center">

![NEWBLOOMERS · Sistema de Integrações · ETL](../../../../.docs/images/banner.png)

</div>

# 🌐 Host WebApi

<div align="center">

![.NET 10](../../../../.docs/images/badges/net-10.svg) ![ASP.NET Core](../../../../.docs/images/badges/asp-net-core-swagger.svg) ![Hangfire](../../../../.docs/images/badges/hangfire-1-8-opcional.svg)

![Rotas](../../../../.docs/images/badges/rotas-464-sq.svg) ![Porta](../../../../.docs/images/badges/localhost-5252-sq.svg) ![Empresa](../../../../.docs/images/badges/empresa-x-company-sq.svg) ![Autenticação](../../../../.docs/images/badges/autenticao-desligada-sq.svg)

</div>

<p align="center">
  <b>Registra os módulos, expõe as rotas de cada leitura e escrita e, quando ligado, agenda os jobs com o Hangfire.</b>
</p>

<p align="center">
  <a href="#-os-dois-modos">Modos</a> ·
  <a href="#️-como-rodar">Como rodar</a> ·
  <a href="#️-rotas-por-módulo">Rotas</a> ·
  <a href="#-empresa-da-execução-nas-rotas">Empresa</a> ·
  <a href="#️-agenda-dos-jobs-com-o-hangfire-ligado">Agenda</a> ·
  <a href="#-concorrência">Concorrência</a> ·
  <a href="#-configuração">Configuração</a>
</p>

---

Host web do sistema de integrações: registra os módulos, expõe as rotas que disparam cada leitura e escrita e, quando
ligado, roda o **Hangfire**, que agenda os jobs recorrentes a partir de `auditing.Jobs`. Até 30/10/2026 o projeto se
chamava `Hangfire`; as decisões e o histórico de cada passo estão em [.docs/PLANO-HOST-WEBAPI.md](.docs/PLANO-HOST-WEBAPI.md).

| | Item | Valor |
|:---:|---|---|
| 💻 | **Plataforma** | .NET 10 (`net10.0`), ASP.NET Core |
| ⏱️ | **Agendador** | Hangfire 1.8 com storage SQL Server, opcional (`Hangfire:Enabled`) |
| 🔗 | **Endereço local** | `http://localhost:5252` (Swagger em `http://localhost:5252/swagger`, dashboard do Hangfire em `http://localhost:5252/hangfire`) |
| 🏢 | **Empresa** | por (job, empresa) na agenda; nas rotas, o `{docCompany}` ou, sem ele, o `X-Company` (CNPJ da empresa) |
| 🔐 | **Autenticação da API** | desligada por enquanto: o `[Authorize]` dos controllers está comentado |

<div align="center">

![Swagger do WebApi](../../../../.docs/images/swagger/visao-geral.png)

<sub><b>Swagger</b> — uma tag por service, agrupadas por módulo</sub>

</div>

---

## 🔀 Os dois modos

O mesmo projeto sobe de dois jeitos, escolhidos por `Hangfire:Enabled`:

| `Hangfire:Enabled` | O host sobe como |
|---|---|
| 🌐 `false` (padrão do `appsettings.json`) | **só API**: controllers e Swagger. Nada do Hangfire é registrado e o storage dele nem é acessado. Os jobs rodam pelas rotas, chamadas por quem quiser disparar |
| ⏱️ `true` | **API + Hangfire**: o mesmo da API, mais o storage, o servidor, o dashboard, a agenda montada a partir de `auditing.Jobs` e os jobs de manutenção |

```mermaid
%%{init: {'theme':'base','themeVariables':{'primaryColor':'#1e1b2e','primaryTextColor':'#ffffff','primaryBorderColor':'#7c5cff','lineColor':'#f5a524','textColor':'#1e1b2e','clusterBkg':'#f5f3ff','clusterBorder':'#7c5cff','titleColor':'#1e1b2e','edgeLabelBackground':'#1e1b2e'}}}%%
flowchart LR
    START["▶️ dotnet run"] --> CFG{"Hangfire:Enabled?"}
    CFG -- false --> API["🌐 Só API<br/>controllers · Swagger"]
    CFG -- true --> VAL{"seção Hangfire<br/>válida?"}
    VAL -- não --> STOP["⛔ não sobe<br/>lista o que falta"]
    VAL -- sim --> FULL["⏱️ API + Hangfire<br/>storage · servidor · dashboard"]
    FULL --> REF["SchedulerRefresh<br/>agenda de auditing.Jobs"]
    FULL --> MAN["FailedJobsCleanup<br/>manutenção"]

    classDef accent fill:#7c5cff,stroke:#7c5cff,color:#ffffff
    classDef warm fill:#f5a524,stroke:#f5a524,color:#1e1b2e
    class API,FULL accent
    class STOP warm
```

Com o Hangfire ligado, a seção `Hangfire` é validada na subida: faltando um parâmetro obrigatório (a connection string do
storage, o usuário e a senha do dashboard...), o host não sobe e a mensagem lista o que falta.

> [!WARNING]
> **Um host de agenda por vez:** o Hangfire deste host, os WebJobs do host [WebJob](../WebJob/README.md) e o serviço do
> host [Worker](../Worker/README.md) (Quartz.NET) leem a mesma agenda de `auditing.Jobs` e só um agenda por vez no mesmo
> banco. Antes de ligar o Hangfire, parar os WebJobs no App Service e o serviço do Worker; antes de usar um dos outros
> dois, `Hangfire:Enabled = false` aqui (as rotas continuam para disparo manual: o modo só API combina com qualquer um
> deles). Se dois ficarem ligados por um tempo numa troca, a trava de concorrência ([abaixo](#-concorrência)) ainda impede
> que um mesmo job rode **ao mesmo tempo** nos dois, mas não que rode **duas vezes** no mesmo ciclo: no teste com o
> Hangfire e o Worker no mesmo banco, 10 de 12 jobs rodaram duas vezes por ciclo (o segundo começa quando o primeiro já
> terminou). Os dados não duplicaram (o merge é por chave), mas as APIs foram lidas em dobro. Desde 03/11/2026, cada
> host se registra em `auditing.SchedulingHosts` com heartbeat (a cada 30 s, expira em 2 min) e só o tipo que está no ar
> há mais tempo executa os jobs da agenda: o Hangfire ligado com o Worker no ar sobe, avisa no log e deixa os jobs da
> agenda em `Deleted` com o motivo, até o Worker sair. Para ligar os dois de propósito numa migração:
> `Scheduling:AllowOtherHostTypes = true` ([melhorias, 4.2](../../../../.docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md#42-ponto-7-dois-agendadores-no-mesmo-banco)).

---

## ▶️ Como rodar

**Só API:**

```bash
dotnet run --project src/Backend/Hosts/WebApi --launch-profile http
```

**Com o Hangfire**, as credenciais do dashboard vêm de variável de ambiente, nunca do `appsettings.json`:

```bash
Hangfire__Enabled=true Hangfire__Dashboard__UserName="<usuário>" Hangfire__Dashboard__Password="<senha>" dotnet run --project src/Backend/Hosts/WebApi --launch-profile http
```

O dashboard abre em **`http://localhost:5252/hangfire`** (perfil `http`, a mesma porta das rotas e do Swagger) e pede
o usuário e a senha acima. Com `Hangfire:Enabled = false` (padrão), `/hangfire` responde 404.

> [!WARNING]
> Num banco de teste carregado com o `Import-TestSeeds.ps1`, os jobs ativos têm credenciais reais: antes de ligar o
> Hangfire, aponte `AfterSale__BaseUrl`, `Movidesk__BaseUrl`, `LinxMicrovix__BaseUrl` e `Totvs__BaseUrl` para
> `http://127.0.0.1:9/`, senão os jobs da agenda chamam as APIs de verdade.

> [!TIP]
> Na máquina de desenvolvimento, o `tools/migrators/Invoke-Migrators.ps1` grava no `appsettings.json` deste projeto o
> `Hangfire:Enabled` (ligado com o WebApi como host de agenda) e o usuário e a senha do dashboard, da seção **Dashboard
> do Hangfire (WebApi)** da tela (ou `-HangfireUser` e `-HangfirePassword` com `-NoGui`), e tira as mesmas chaves dos
> User Secrets. As outras chaves estão na [tabela de User Secrets](#-user-secrets-na-máquina-de-desenvolvimento).

> [!IMPORTANT]
> Antes de ligar o Hangfire num banco, ele precisa ter `auditing.Jobs` com as colunas `IsActive`, `RunningDailyAt`,
> `Queue`, `Holder` e `Company` (Script001 do Shared) e **pelo menos uma holding com empresas** em `auditing.Holders` e
> `auditing.Companies`, com os módulos contratados (cadastro do WebApp: tela inicial e Cadastro › Módulos e Credenciais,
> ou o [`tools/seeds/Import-TestSeeds.ps1`](../../../../tools/seeds/Import-TestSeeds.ps1) num banco de teste). Sem
> empresa, a agenda fica vazia. Um banco que rodou os scripts antes de 30/10/2026 precisa do ajuste descrito no plano, §4.8.
>
> A tela [`tools/migrators/Invoke-Migrators.ps1`](../../../../tools/migrators/Invoke-Migrators.ps1) com o host
> **WebApi (Hangfire)** marcado sobe as tabelas na ordem certa; no fim, o log lembra de ligar `Hangfire:Enabled` aqui e
> parar os outros hosts. A holding, as empresas, as credenciais dos módulos e os jobs ativos ficam no WebApp; ligar o
> Hangfire é a configuração deste host (`Hangfire:Enabled`).

> [!CAUTION]
> O [WebApi.http](WebApi.http) tem exemplos de rotas de cada módulo. Cada chamada roda a integração **de verdade** contra
> a API e o banco configurados.

---

## 🗂️ Estrutura

```text
WebApi/
├── Controllers/          rotas por módulo (DataWarehouse, General, LinxCommerce, LinxMicrovix, Shippment, Totvs)
├── Extensions/
│   ├── BuilderExtensions.cs    controllers, Swagger, CORS e o filtro global de empresa
│   ├── CompanyContextFilter.cs empresa da execução nas rotas ({docCompany} ou X-Company, em auditing.Companies); sem ela, 400
│   ├── CompanyHeaderOperationFilter.cs   o X-Company obrigatório no Swagger nas rotas sem {docCompany}
│   ├── AllowWithoutCompanyAttribute.cs   rota sem job e sem empresa (o token)
│   ├── ServicesExtensions.cs   registro dos módulos e, com Hangfire:Enabled, do Hangfire (AddHangfireService)
│   └── AppUseExtensions.cs     pipeline e, com Hangfire:Enabled, dashboard e jobs de manutenção (UseHangfire)
├── Scheduling/
│   ├── HangfireSettings.cs, HangfireSettingsValidator.cs   seção Hangfire e a validação na subida
│   ├── DashboardBasicAuthorizationFilter.cs                usuário e senha do dashboard
│   ├── JobExpirationTimeoutFilter.cs                       retenção dos jobs concluídos
│   ├── SkipConcurrentExecutionFilter.cs                    travas da empresa e do job
│   ├── SchedulerRefreshJob.cs, FailedJobsCleanupJob.cs     jobs de manutenção
│   └── HangfireJobRunner.cs                                o que o Hangfire executa: repassa ao ScheduledJobRunner
├── appsettings.json
├── WebApi.http
└── .docs/PLANO-HOST-WEBAPI.md
```

Os mapas `IdJob → método` (`RecurringJobs/`), o `JobCron` e o `ScheduledJobRunner` ficam no projeto
`src/Backend/Hosts/Hosts.Scheduling`, que o WebApi, o WebJob e o Worker dividem.

---

## 🔧 Configuração

| Seção | Uso |
|---|---|
| `ConnectionStrings:Connection` | banco das integrações (módulos, `auditing`, `monitor` e, por padrão, o storage do Hangfire e a trava de concorrência). Fica fora do `appsettings.json`: User Secrets na máquina de desenvolvimento (`dotnet user-secrets set`, [abaixo](#-user-secrets-na-máquina-de-desenvolvimento)) e variável de ambiente `ConnectionStrings__Connection` no servidor |
| `ConnectionStrings:AuditingConnection` | opcional: banco de `auditing` em outro banco. Sem ela, o Shared usa a própria `Connection` (auditing fica no mesmo banco) |
| `Hangfire` | tudo do Hangfire (tabela abaixo) |
| `AfterSale`, `Movidesk`, `LinxMicrovix`, `Totvs`... | `BaseUrl` e as opções de cada módulo (ver o README do módulo) |

<details>
<summary><b>⏱️ Seção <code>Hangfire</code> (valores do <code>appsettings.json</code>)</b></summary>

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

</details>

### 🔑 User Secrets na máquina de desenvolvimento

Os User Secrets do WebApi são gravados à mão (o `WebApi.csproj` tem `UserSecretsId`):

| Chave | Valor |
|---|---|
| `AfterSale:BaseUrl`, `Movidesk:BaseUrl`, `LinxMicrovix:BaseUrl`, `Totvs:BaseUrl` | `http://127.0.0.1:9/` para testes sem chamar as APIs; sem a chave, vale a do `appsettings.json` (o Linx Commerce não tem `BaseUrl`) |
| `DataWarehouse:Pepper` | opcional: pepper do hash de CPF/CNPJ no DW |

A connection string (`Connection`), o `Hangfire:Enabled` e o usuário e a senha do dashboard
(`Hangfire:Dashboard:UserName` e `Password`) ficam no `appsettings.json`, gravados pelo
`tools/migrators/Invoke-Migrators.ps1` no fim da migração; o script tira as mesmas chaves dos User Secrets. `auditing` fica
no mesmo banco.

```powershell
dotnet user-secrets set "Totvs:BaseUrl" "http://127.0.0.1:9/" --project src/Backend/Hosts/WebApi
```

---

## 🛣️ Rotas por módulo

Todas as rotas aparecem no Swagger, menos as do Linx Commerce (em `RELEASE`, os controllers ficam fora dele). As leituras recorrentes (as cargas)
pedem o cabeçalho **`X-Company`**, o CNPJ da empresa, que o Swagger mostra como campo obrigatório; as sob demanda e as
escritas pedem o CNPJ na rota (`{docCompany}`, 14 dígitos) e o cabeçalho **`X-User-Name`**, gravado no log de requisições
do módulo. `Swagger:DefaultCompany`, no `appsettings.json`, já preenche o campo `X-Company` com um CNPJ.

| Módulo | Prefixo | Rotas | Detalhes |
|---|---|:---:|---|
| 🟣 **Totvs** | `TotvsJobs/Totvs` | **123** | [README](../../Modules/Totvs/README.md); escritas só com `Totvs:Writes:<grupo>:Enabled` |
| 🟣 **Linx Microvix** (ERP e B2C) | `LinxMicrovixJobs/LinxMicrovix` | **284** | [README](../../Modules/LinxMicrovix/LinxMicrovix%20-%20Outbound%20Web%20Service/README.md) |
| 🟣 **Linx Commerce** | `LinxCommerceJobs/LinxCommerce` | **12** | [README](../../Modules/LinxCommerce/README.md); obsoletas e fora do Swagger: sem credenciais para testar |
| 🟣 **Movidesk** | `MovideskJobs/Movidesk` | **16** | [README](../../Modules/General/Movidesk/README.md) |
| 🟣 **AfterSale** | `AfterSaleJobs/AfterSale` | **24** | [README](../../Modules/General/AfterSale/README.md) |
| 🟠 **Data warehouse** | `DataWarehouseJobs/DataWarehouse` | **5** | [funcionamento e fluxo](../../Modules/DataWarehouse/.docs/DATAWAREHOUSE-FUNCIONAMENTO-E-FLUXO.md); 200 concluída, 500 falha, 409 outra carga rodando |

<details>
<summary><b>🖼️ Exemplo: rotas do AfterSale no Swagger</b></summary>

<div align="center">

![Rotas do AfterSale no Swagger](../../../../.docs/images/swagger/aftersale.png)

</div>

</details>

Os controllers de Dootax, Oto, FlashCourier, JadLog, StoneLogistica e TotalExpress existem, mas estão vazios: esses
módulos entram no host conforme forem migrados (plano, passo 6).

---

## 🏢 Empresa da execução nas rotas

Toda execução grava a holding e a empresa em `auditing.Logs`, `Messages` e `Records`
([proposta do schema auditing](../../../../.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)). Nas rotas, quem informa é o
**`CompanyContextFilter`**, global, antes da action:

* o **`{docCompany}`** da rota; ou
* sem ele, o cabeçalho **`X-Company`** (CNPJ da empresa, só dígitos; não é o da holding);
* com os dois, precisam ser a mesma empresa;
* a empresa precisa estar em `auditing.Companies`, e a holding sai dela (`ICompanyRepository`);
* sem nenhum dos dois, **400** antes da action (o job não teria empresa para o log), salvo as rotas com
  `[AllowWithoutCompany]`, que não rodam job (o token).

No Swagger, o **`CompanyHeaderOperationFilter`** põe o `X-Company` como campo obrigatório em toda rota sem `{docCompany}`.
No DataWarehouse e nos Hosts, o CNPJ é o da própria holding, onde esses jobs estão cadastrados.

```mermaid
%%{init: {'theme':'base','themeVariables':{'primaryColor':'#1e1b2e','primaryTextColor':'#ffffff','primaryBorderColor':'#7c5cff','lineColor':'#f5a524','textColor':'#1e1b2e','clusterBkg':'#f5f3ff','clusterBorder':'#7c5cff','titleColor':'#1e1b2e','edgeLabelBackground':'#1e1b2e'}}}%%
flowchart LR
    REQ["Requisição"] --> H{"X-Company<br/>só dígitos?"}
    H -- não --> E1["400<br/>X-Company inválido"]
    H -- "sim ou ausente" --> D{"rota tem<br/>docCompany?"}
    D -- não --> NO{"tem<br/>X-Company?"}
    NO -- não --> E4["400<br/>informe o X-Company"]
    NO -- sim --> C1["empresa = X-Company"]
    D -- sim --> EQ{"X-Company igual<br/>ao docCompany?"}
    EQ -- diferente --> E3["400<br/>X-Company diferente da rota"]
    EQ -- "igual ou ausente" --> C2["empresa = docCompany"]
    C1 --> HC{"empresa em<br/>auditing.Companies?"}
    C2 --> HC
    HC -- não --> E2["400<br/>CNPJ não cadastrado"]
    HC -- sim --> RUN["▶️ action<br/>ICompanyContext"]

    classDef accent fill:#7c5cff,stroke:#7c5cff,color:#ffffff
    classDef err fill:#dc2626,stroke:#dc2626,color:#ffffff
    class RUN accent
    class E1,E2,E3,E4 err
```

> [!NOTE]
> Para disparar uma leitura recorrente pela rota, mande o `X-Company` (no Swagger, o campo da rota). Sem ele e sem
> `{docCompany}`, a resposta é 400 "Informe o cabeçalho X-Company".

```http
POST http://localhost:5252/MovideskJobs/Movidesk/Tickets
X-Company: 44065634000106

GET  http://localhost:5252/MovideskJobs/Movidesk/Tickets/38367316000199/1
X-User-Name: fulano
```

---

## ⏱️ Agenda dos jobs (com o Hangfire ligado)

A agenda vem inteira de `auditing.Jobs`: a tabela diz **se** e **quando** cada job roda, para **qual empresa** e em que
fila; o host diz **o que** ele executa.

| Coluna | Uso |
|---|---|
| `IsActive` | só os jobs com 1 entram na agenda. Nasce 0 em todo insert: nada roda sem um `UPDATE` |
| `IntervalTimeInMinutes` | intervalo; 0 = sob demanda, fora da agenda |
| `RunningDailyAt` | preenchida, o job é diário e roda nessa hora (só com intervalo 1440). Os diários dos scripts estão às 23:00 |
| `Queue` | fila do Hangfire (`default` até existir um segundo servidor) |
| `Holder`, `Company` | holding e empresa da linha: uma linha por (job, empresa), criada pelo WebApp (Cadastro › Módulos e Credenciais) a partir do catálogo `auditing.Catalog`, com a agenda sugerida |

Só os jobs de integração (`IdType = 2`) entram na agenda; os de procedure (`IdType = 1`) são chamados pelo próprio
service, depois da carga; os do catálogo com `IsOnDemand = 1` também ficam fora.

**Do registro ao cron:** 3 vira `*/3 * * * *`, 60 vira `0 * * * *`, 120 vira `0 */2 * * *` e um diário às 23:00 vira
`0 23 * * *`, no fuso de `Hangfire:TimeZone`. Um intervalo que o cron não representa (que não divide a hora ou o dia)
fica fora da agenda, com aviso no log.

**O `SchedulerRefresh`** monta a agenda: roda na subida e a cada `Schedule:RefreshIntervalInMinutes`. Ele lê
`auditing.Jobs`, inclui na agenda os jobs ativos que têm método no mapa, altera os que mudaram e remove os que ficaram
inativos, sem intervalo ou saíram do mapa. Um `UPDATE` na tabela vale, no máximo, no ciclo
seguinte. Os recurring jobs da agenda são **um por (job, empresa)** e se chamam
`<IdJob>-<Title> - V<versão> - <empresa>` (ex.: `9002-DataWarehouseShortCycle - V1.0.0 - 44065634000106`), com a versão
do `[JobVersion]` do método ([abaixo](#️-versão-dos-jobs)). O resultado de cada releitura aparece no dashboard e no log
(`Agenda: N job(s) na agenda...`), com os avisos.

```mermaid
%%{init: {'theme':'base','themeVariables':{'primaryColor':'#1e1b2e','primaryTextColor':'#ffffff','primaryBorderColor':'#7c5cff','lineColor':'#f5a524','textColor':'#1e1b2e','clusterBkg':'#f5f3ff','clusterBorder':'#7c5cff','titleColor':'#1e1b2e','edgeLabelBackground':'#1e1b2e'}}}%%
flowchart LR
    CAT[("auditing.Catalog<br/>catálogo do produto")] -- "WebApp: contratar módulo" --> JOBS[("auditing.Jobs<br/>uma linha por empresa")]
    JOBS --> REF["🔄 SchedulerRefresh<br/>subida + a cada 10 min"]
    MAP["Hosts.Scheduling<br/>IdJob → método"] --> REF
    REF --> RJ["Recurring jobs<br/>IdJob-Title - Vx.y.z - empresa"]
    RJ --> RUN["HangfireJobRunner<br/>ScheduledJobRunner.Run(job, empresa)"]

    classDef accent fill:#7c5cff,stroke:#7c5cff,color:#ffffff
    classDef warm fill:#f5a524,stroke:#f5a524,color:#1e1b2e
    class REF accent
    class RJ warm
```

**Ligar e desligar jobs:**

```sql
-- liga um job de uma empresa
UPDATE [auditing].[Jobs] SET IsActive = 1 WHERE IdType = 2 AND IdJob = 3000 AND Company = '<CNPJ da empresa>';

-- liga um módulo inteiro de uma empresa, pela faixa de IdJob (ex.: Movidesk, 3000 a 3499)
UPDATE [auditing].[Jobs] SET IsActive = 1 WHERE IdType = 2 AND IdJob BETWEEN 3000 AND 3499 AND IntervalTimeInMinutes > 0 AND Company = '<CNPJ da empresa>';

-- muda a hora de um diário (todas as empresas)
UPDATE [auditing].[Jobs] SET RunningDailyAt = '02:30' WHERE IdType = 2 AND IdJob = 9001;

-- desliga (todas as empresas)
UPDATE [auditing].[Jobs] SET IsActive = 0 WHERE IdType = 2 AND IdJob = 3000;
```

> [!TIP]
> Para não esperar o ciclo, o `SchedulerRefresh` pode ser disparado na hora pela página de jobs recorrentes do dashboard.

---

## 🏷️ Versão dos jobs

Cada método de service chamado por um job recorrente tem **`[JobVersion("MAJOR.MINOR.PATCH")]`** no método da classe do
service (não na interface), e a versão vai no nome do recurring job. Para saber o que roda em cada ambiente, basta olhar
o dashboard. Ao mudar o código de um job, mude a versão no mesmo método: **PATCH** numa correção, **MINOR** numa mudança
compatível, **MAJOR** quando muda o que o job grava de um jeito que quebra quem lê. Com a versão nova, o
`SchedulerRefresh` troca o job antigo pelo novo na releitura seguinte.

> [!IMPORTANT]
> Um método do mapa sem o atributo impede o host de subir com o Hangfire ligado.

---

## 🔒 Concorrência

Um recurring job do Hangfire não sabe se a execução anterior terminou. Para dois disparos do mesmo job nunca rodarem
juntos, um filtro do host (**`SkipConcurrentExecutionFilter`**) pede, antes de cada execução, travas do `IJobLock` do
Shared (`sp_getapplock` no banco das integrações, sem esperar e presas à sessão). Com um recurring job por (job, empresa),
são **duas travas**:

| | Trava | Chave | Ocupada |
|:---:|---|---|---|
| 🏢 | **da empresa** | `<chave do job>:<empresa>` (ex.: `job:3000:44065634000106`) | a execução anterior da mesma empresa ainda roda: **pula** — vai para `Deleted` com o motivo `Skipped: previous execution of <chave> still running` e o worker fica livre |
| 🧩 | **do job** | `job:<IdJob>` (os jobs do DW dividem `group:datawarehouse`, porque escrevem nas mesmas landings) | outra empresa roda o mesmo job, que escreve nas mesmas tabelas (`untreated` e a `sp_*_Sync`): **volta para a fila em 1 minuto** (`Rescheduled: job:<IdJob> running for another company`), sem perder o horário |

```mermaid
%%{init: {'theme':'base','themeVariables':{'primaryColor':'#1e1b2e','primaryTextColor':'#ffffff','primaryBorderColor':'#7c5cff','lineColor':'#f5a524','textColor':'#1e1b2e','clusterBkg':'#f5f3ff','clusterBorder':'#7c5cff','titleColor':'#1e1b2e','edgeLabelBackground':'#1e1b2e'}}}%%
flowchart LR
    T["⏰ disparo do cron<br/>(job, empresa)"] --> L1{"trava da empresa<br/>job:IdJob:empresa"}
    L1 -- ocupada --> SKIP["⏭️ Deleted<br/>Skipped"]
    L1 -- livre --> L2{"trava do job<br/>job:IdJob"}
    L2 -- "ocupada por<br/>outra empresa" --> RES["🔁 volta para a fila<br/>em 1 minuto"]
    L2 -- livre --> RUN["▶️ roda<br/>e solta as travas no fim"]

    classDef accent fill:#7c5cff,stroke:#7c5cff,color:#ffffff
    classDef warm fill:#f5a524,stroke:#f5a524,color:#1e1b2e
    class RUN accent
    class SKIP,RES warm
```

Depois de `Concurrency:MaxConsecutiveSkips` pulos seguidos, sai um aviso no log: o job está lento demais para o
intervalo, e a correção é aumentar o intervalo na tabela. Uma execução adiada pode rodar logo antes do disparo seguinte
da mesma empresa; as duas rodam em sequência (a trava da empresa impede rodarem juntas).

> [!NOTE]
> **As rotas que rodam um job da agenda pedem a mesma trava** (desde 03/11/2026; no teste A6, o disparo manual rodou junto
> com o agendado e leu a API em dobro). Quando a rota abre o log de um job que está no mapa da agenda, o `RouteJobLock`
> pede a trava do job (`job:<IdJob>`, ou a do grupo do DW) sem esperar. Ocupada, a rota devolve **409**
> (`JOB_ALREADY_RUNNING`) sem gravar nada em `auditing`; a trava fica com a requisição até o fim. As rotas por chave
> (ex.: um ticket pelo número) e as de escrita não são jobs da agenda e não pedem trava
> ([melhorias, 4.1](../../../../.docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md#41-ponto-6-rotas-do-webapi-sem-a-trava)).

---

## 🧹 Dashboard e manutenção

* 🔐 O dashboard pede usuário e senha (Basic Authentication, `Dashboard:UserName` e `Password`). Basic manda a senha em
  cada requisição: **fora do `localhost`, só por HTTPS**.
* 🗑️ Jobs concluídos (`Succeeded` e `Deleted`, inclusive os pulos) saem do storage depois de
  `Storage:JobExpirationTimeoutInMinutes`. O histórico que importa fica em `auditing.Logs` e `auditing.Messages`.
* ❌ Uma falha fica em `Failed` (as tentativas automáticas estão em 0) até o `FailedJobsCleanup`, que roda a cada hora,
  no minuto 15, e passa para `Deleted` as falhas com mais de `Maintenance:FailedJobsRetentionInDays`.
* ⚙️ `SchedulerRefresh` e `FailedJobsCleanup` são do próprio host: não estão em `auditing.Jobs` e só somem com
  `Hangfire:Enabled = false`.

---

## 🧪 Comportamento testado

Conferido nos [testes de hospedagem](../../../../.docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md) de 02 e 03/11/2026 (cenários
C1, C2, C3, A1, A2, A5, A6, A8 e A10), com os 12 jobs do conjunto mínimo a cada 5 minutos:

| Situação | O que acontece |
|---|---|
| 🚀 Subida | O primeiro ciclo sai na virada do intervalo (o cron não roda na subida). Se o host estava fora do ar, os recorrentes atrasados rodam **uma vez** logo na subida, e o ritmo volta ao normal |
| 💥 Processo derrubado no meio | As execuções em andamento vão para `Deleted`, sem reexecutar; o ciclo seguinte é normal. **Elas não deixam registro em `auditing.Logs`**, que só é gravado no fim |
| 🗄️ Banco parado e religado | O Hangfire reconecta sozinho e roda os atrasados; nada fica preso |
| ❌ Falha de API | Erro em `auditing.Messages`, job em `Failed` sem novas tentativas (nenhuma rajada contra a API) e de volta no ciclo seguinte |
| 🐧 Container Linux | Funciona na imagem `mcr.microsoft.com/dotnet/aspnet:10.0` (Debian, com ICU e tzdata: o fuso `E. South America Standard Time` é convertido e o log sai em horário de Brasília). Evitar as imagens Alpine e chiseled, que vêm sem eles. Atrás do nginx só em HTTP o `UseHttpsRedirection` não age (o host não usa `UseForwardedHeaders`) |
| ☁️ Azure SQL | O schema `HangFire` é criado no Azure SQL. Com o banco serverless pausado, o instalador recebe `Database ... is not currently available`, **repete sozinho** até o banco acordar (15 a 90 s) e o host sobe |
| ☁️ App Service (Always On) | O Hangfire roda e cria o schema, mas num plano B1 junto com 12 WebJobs e o painel o HTTP do WebApi não respondeu (100% de CPU e 96% de memória). Para hospedar o Hangfire no App Service, um plano próprio ou maior |
| 🔁 Troca de host | Parar o Worker e ligar o Hangfire (ou o contrário) não duplica nem perde execução; sobra uma linha da instância antiga em `quartz.QRTZ_SCHEDULER_STATE`, sem efeito |

---

## ➕ Um job ou um módulo novo

* **Job recorrente novo num módulo do host:** a linha no script de jobs da service (vai para `auditing.Catalog`, sem agenda;
  a agenda sugerida é uma linha na [`.docs/AGENDA-SUGERIDA-JOBS.csv`](../../../../.docs/AGENDA-SUGERIDA-JOBS.csv), ver [PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](../../../../.docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md)
  §3.1), a rota no controller, a entrada no mapa do módulo em
  `src/Backend/Hosts/Hosts.Scheduling/RecurringJobs/<Módulo>RecurringJobs.cs`, com o método sem parâmetros do service, e
  `[JobVersion("1.0.0")]` no método da classe do service. Para as empresas que já têm o módulo, contratar o módulo
  de novo no WebApp cria a linha em `auditing.Jobs` (inativa, sem duplicar as outras). Sem o mapa, o `SchedulerRefresh` avisa e
  deixa o job fora da agenda.
* **Módulo novo:** referência no `WebApi.csproj`, registro no `ServicesExtensions`, controller, o mapa
  `<Módulo>RecurringJobs.cs` incluído no `RecurringJobMap` e o script de jobs com as colunas novas (plano, passo 6).

---

<div align="center">

![NEWBLOOMERS](../../../../.docs/newbloomers.png)

<sub>Host WebApi · <a href="../../../../README.md">voltar ao README do repositório</a></sub>

</div>
