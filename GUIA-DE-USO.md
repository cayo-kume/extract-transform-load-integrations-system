<div align="center">

![NEWBLOOMERS · Sistema de Integrações · ETL](docs/images/banner.png)

</div>

# 🚀 Guia de uso

<div align="center">

![.NET 10](docs/images/badges/net-10.svg) ![SQL Server](docs/images/badges/sql-server-dapper-dbup.svg) ![Blazor](docs/images/badges/blazor-mudblazor-9.svg)

![Hangfire](docs/images/badges/hangfire-1-8-sq.svg) ![Quartz.NET](docs/images/badges/quartz-net-3-19-sq.svg) ![Azure WebJobs](docs/images/badges/azure-webjobs-sq.svg)

</div>

<p align="center">
  <b>Do banco vazio às integrações rodando: o passo a passo para usar cada projeto do repositório.</b><br>
  <sub>O panorama do sistema está no <a href="README.md">README</a>; os detalhes de cada projeto, no README dele.</sub>
</p>

<p align="center">
  <a href="#-os-projetos">Projetos</a> ·
  <a href="#-pré-requisitos">Pré-requisitos</a> ·
  <a href="#-1-sql-server">Banco</a> ·
  <a href="#-2-tabelas">Tabelas</a> ·
  <a href="#-3-empresa-e-parâmetros">Empresa</a> ·
  <a href="#-4-rotas-do-webapi">Rotas</a> ·
  <a href="#-5-agenda">Agenda</a> ·
  <a href="#-6-painel">Painel</a> ·
  <a href="#-7-onde-ver-o-que-rodou">Acompanhar</a> ·
  <a href="#-solução-de-problemas">Problemas</a>
</p>

---

## 🗺️ O caminho

```mermaid
%%{init: {'theme':'base','themeVariables':{'primaryColor':'#1e1b2e','primaryTextColor':'#ffffff','primaryBorderColor':'#7c5cff','lineColor':'#f5a524','textColor':'#1e1b2e','clusterBkg':'#f5f3ff','clusterBorder':'#7c5cff','titleColor':'#1e1b2e','edgeLabelBackground':'#1e1b2e'}}}%%
flowchart LR
    A["🐳 1. SQL Server"] --> B["🗄️ 2. Tabelas<br/>Invoke-Migrators"]
    B --> C["🏢 3. Empresa<br/>e parâmetros"]
    C --> D["🌐 4. WebApi<br/>rotas e Swagger"]
    C --> E["⏱️ 5. Agenda<br/>um host"]
    D --> F["🖥️ 6. Painel"]
    E --> F
    F --> G["📊 7. auditing"]

    classDef accent fill:#7c5cff,stroke:#7c5cff,color:#ffffff
    classDef warm fill:#f5a524,stroke:#f5a524,color:#1e1b2e
    class B,C accent
    class E warm
```

> [!IMPORTANT]
> Num banco novo **nada roda até existir uma empresa**: os migrators criam só a estrutura e o catálogo do produto, e os
> jobs de cada empresa nascem **inativos**.

---

## 📦 Os projetos

| | Projeto | Pasta | Para que serve | README |
|:---:|---|---|---|:---:|
| 🌐 | **WebApi** | `src/Backend/Hosts/WebApi` | Host web: rotas de leitura e escrita de todos os módulos, Swagger e, opcional, o Hangfire | [📖](src/Backend/Hosts/WebApi/README.md) |
| ☁️ | **WebJob** | `src/Backend/Hosts/WebJob` | Um processo por job, publicado como WebJob contínuo na Azure | [📖](src/Backend/Hosts/WebJob/README.md) |
| 🪟 | **Worker** | `src/Backend/Hosts/Worker` | Serviço do Windows com o Quartz.NET: todos os jobs num processo | [📖](src/Backend/Hosts/Worker/README.md) |
| 🧩 | **Hosts.Scheduling** | `src/Backend/Hosts/Hosts.Scheduling` | Biblioteca comum aos três hosts: liga cada `IdJob` ao método do módulo | — |
| 🟣 | **Módulos** | `src/Backend/Modules/*` | Totvs, LinxMicrovix, LinxCommerce, Movidesk, AfterSale e DataWarehouse (bibliotecas, não sobem sozinhos) | [README](README.md#-módulos) |
| ⚫ | **Shared** | `src/Backend/Shared` | Auditoria, cliente HTTP, repositório comum, trava de concorrência e empresa da execução | — |
| 🖥️ | **WebApp** | `src/Frontend/WebApp` | Painel Monitor de Integrações (Blazor): dados mockados por padrão, ou o banco com `Database:Enabled` | [📖](src/Frontend/WebApp/README.md) |
| 📱 | **MobileApp** | `src/Mobile/MobileApp` | App .NET MAUI, em construção | — |
| 🗄️ | **Invoke-Migrators** | `tools/migrators` | Tela (ou console) que sobe as tabelas de todas as aplicações na ordem certa | [MIGRATOR-CSX](docs/MIGRATOR-CSX.md) |
| 🎛️ | **Invoke-HostsConfig** | `tools/hosts` | Tela que liga e desliga os jobs da empresa e grava os User Secrets dos hosts e do WebApp | [passo 3](#️-configurar-jobs-e-user-secrets-invoke-hostsconfig) |
| 🧪 | **Import-TestSeeds** | `tools/seeds` | Carrega uma empresa de teste: parâmetros, CNPJs e jobs | [plano](docs/PLANO-TELA-CADASTRO-EMPRESA.md#3-script-ps1-de-dados-de-teste--feito-em-42) |

---

## ✅ Pré-requisitos

| | Ferramenta | Como instalar ou conferir |
|:---:|---|---|
| 💻 | **.NET 10 SDK** | `dotnet --version` (10.x) |
| 🐚 | **PowerShell 7** | `pwsh --version`; os scripts de `tools/` e de publicação exigem a 7 |
| 📜 | **dotnet-script** | `dotnet tool install -g dotnet-script` (roda os `migrator.csx`) |
| 🗄️ | **SQL Server** | uma instância local, de rede ou no Docker (passo 1) |
| 🧰 | **sqlcmd** | só para o banco de teste (`Import-TestSeeds.ps1`); precisa estar no `PATH` |
| ☁️ | **Azure CLI** | só para publicar o WebJob (`az login`) |

---

## 🐳 1. SQL Server

Qualquer SQL Server serve. Para testes, o padrão do projeto é o container **`sql-server`** do Docker, na porta **1433**:

```bash
docker run -d --name sql-server -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<senha forte>" -p 1433:1433 mcr.microsoft.com/mssql/server:2025-latest
```

> [!WARNING]
> Testes sempre num **banco de teste próprio** (com `Test` ou `Teste` no nome), nunca no LocalDB e nunca no banco de
> produção. Apague o banco de teste no fim.

---

## 🗄️ 2. Tabelas

A tela [`tools/migrators/Invoke-Migrators.ps1`](tools/migrators/Invoke-Migrators.ps1) roda o `migrator.csx` (DbUp) de cada
aplicação marcada, na ordem **Shared → integrações → DataWarehouse → host de agenda**, para no primeiro erro e mostra a
senha como `****` no log. O banco é criado pelo DbUp se não existir.

```powershell
./tools/migrators/Invoke-Migrators.ps1
```

<div align="center">

![Tela do Invoke-Migrators](docs/images/tools/invoke-migrators.png)

</div>

| | Campo da tela | O que preencher |
|:---:|---|---|
| 🖧 | **Servidor, Porta, Banco** | ex.: `localhost`, `1433`, `EtlTeste` |
| 🔑 | **Usuário e Senha** | login do SQL Server; o botão **Testar conexão** confere antes de rodar |
| ⏱️ | **Host de agenda** | o host que vai agendar neste banco: **WebApi (Hangfire)**, **WebJob (Azure)** ou **Worker (Quartz.NET)**. Só o Worker tem tabelas próprias (schema `quartz`) |
| 🧪 | **Banco de teste** | marcado, roda o `Import-TestSeeds.ps1` no fim, com o CNPJ e a razão social da tela (passo 3). **Ativar os jobs** liga os jobs da empresa: eles chamam as APIs reais |
| 👤 | **Usuários de banco do WebApp** | opcional: com a senha do `etl_monitor` e/ou do `etl_query`, o migrator do Shared cria os logins e dá as permissões (procedures do Script009) |
| ☑️ | **Catálogo** | as aplicações a subir; o **Shared** é obrigatório |

Sem tela, no console:

```powershell
./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -SchedulingHost WebApi
./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -Apps Totvs,DataWarehouse -SchedulingHost Worker -TestSeeds
./tools/migrators/Invoke-Migrators.ps1 -NoGui -Database EtlTeste -SchedulingHost Worker -MonitorPassword (Read-Host -AsSecureString) -QueryPassword (Read-Host -AsSecureString)
```

O migrator só cria a estrutura. Quais jobs rodam e a configuração dos projetos ficam na segunda tela,
[`Invoke-HostsConfig.ps1`](#️-configurar-jobs-e-user-secrets-invoke-hostsconfig) (passo 3).

<details>
<summary><b>🔧 Um migrator à mão</b></summary>

De dentro da pasta `Scripts/SQLServer` da aplicação, depois do Shared:

```bash
dotnet script migrator.csx -- "Server=localhost,1433;Database=EtlTeste;User Id=sa;Password=...;TrustServerCertificate=True;"
```

No Windows, o caminho dos módulos passa de 260 caracteres: mapeie a pasta com `subst X: "<pasta SQLServer>"` e rode a
partir de `X:`. Detalhes e solução de problemas em [MIGRATOR-CSX.md](docs/MIGRATOR-CSX.md).

</details>

---

## 🏢 3. Empresa e parâmetros

A empresa é a **holding** (CNPJ, só dígitos) em `monitor.Company`. Cada módulo guarda as credenciais por CNPJ na tabela
`*Parameters` dele, e `monitor.HoldingCompanies` liga esses CNPJs (marcas, filiais) à holding.

```mermaid
%%{init: {'theme':'base','themeVariables':{'primaryColor':'#1e1b2e','primaryTextColor':'#ffffff','primaryBorderColor':'#7c5cff','lineColor':'#f5a524','textColor':'#1e1b2e','clusterBkg':'#f5f3ff','clusterBorder':'#7c5cff','titleColor':'#1e1b2e','edgeLabelBackground':'#1e1b2e'}}}%%
flowchart TB
    A["sp_Add_Company<br/>empresa + jobs dos hosts"] --> B["parâmetros do módulo<br/>Seeds/SQLServer"]
    B --> C["sp_Add_Holding_Company<br/>CNPJ → holding"]
    C --> D["sp_Add_Company_App<br/>jobs do módulo, inativos"]
    D --> E["UPDATE IsActive = 1<br/>só o que deve rodar"]

    classDef warm fill:#f5a524,stroke:#f5a524,color:#1e1b2e
    class E warm
```

**Num banco de teste**, o [`tools/seeds/Import-TestSeeds.ps1`](tools/seeds/Import-TestSeeds.ps1) faz tudo isso com os
scripts de `Infrastructure.<Módulo>/Seeds/SQLServer` (ou marque **Banco de teste** na tela do passo 2):

```powershell
./tools/seeds/Import-TestSeeds.ps1 -Database 'EtlTeste'
./tools/seeds/Import-TestSeeds.ps1 -Database 'EtlTeste' -Modules Totvs,Movidesk -ActivateJobs
```

> [!CAUTION]
> Os scripts de `Seeds/` têm **credenciais reais**. Os jobs nascem inativos e o script recusa banco sem `Test`/`Teste` no
> nome (salvo `-Force`). Os scripts de LinxMicrovix e LinxCommerce ficam fora do git: sem eles, o módulo fica sem
> parâmetros e o script avisa.

**Fora de teste**, a empresa vai entrar pela tela de cadastro do WebApp (em construção, fase 4.6 do
[plano](docs/PLANO-TELA-CADASTRO-EMPRESA.md)). Até lá, pelas procedures:

```sql
EXEC auditing.sp_Add_Company @company = '44065634000106', @name = N'NEWBLOOMERS';
-- parâmetros do módulo (INSERT na tabela *Parameters) e, para cada CNPJ deles:
EXEC auditing.sp_Add_Holding_Company @company = '44065634000106', @docCompany = '<CNPJ do parâmetro>';
EXEC auditing.sp_Add_Company_App @company = '44065634000106', @idParent = 30;  -- 30 = Movidesk
```

### 🎛️ Configurar jobs e User Secrets (Invoke-HostsConfig)

Com o banco migrado e a empresa cadastrada, a tela [`tools/hosts/Invoke-HostsConfig.ps1`](tools/hosts/Invoke-HostsConfig.ps1)
faz o resto, sem SQL nem `dotnet user-secrets` à mão:

```powershell
./tools/hosts/Invoke-HostsConfig.ps1
./tools/hosts/Invoke-HostsConfig.ps1 -Database EtlTeste -SchedulingHost Worker
```

| | Parte da tela | O que faz |
|:---:|---|---|
| 🖧 | **Banco** | servidor, porta, banco, usuário e senha; **Carregar empresas** lê `monitor.Company` |
| ⏱️ | **Host de agenda** | WebApi (Hangfire), Worker (Quartz.NET) ou WebJob (Azure): o escolhido fica ligado e os outros desligados |
| ⚙️ | **Configuração dos projetos** | opcional, grava os User Secrets do WebApi, Worker, WebJob e WebApp: connection strings deste banco, **APIs desligadas** (`BaseUrl` em `http://127.0.0.1:9/`, marcado por padrão), usuário e senha do dashboard do Hangfire, pepper do DataWarehouse, `Admin:Enabled`, chave do assistente e as senhas do `etl_monitor` e do `etl_query` |
| ☑️ | **Jobs** | os jobs da empresa agrupados por aplicação (marcado é `IsActive = 1`), com filtro e botões para ativar ou desativar a aplicação ou a lista filtrada |

**Salvar** grava os jobs alterados numa transação e depois os User Secrets. O log mostra só os nomes das chaves. Com jobs
ativos e as APIs de verdade, a tela pede confirmação. Reinicie o host e o WebApp depois de salvar.

**Ligar e desligar jobs à mão** é um `UPDATE` na linha da empresa:

```sql
-- um job
UPDATE auditing.Jobs SET IsActive = 1 WHERE IdType = 2 AND IdJob = 3000 AND Company = '44065634000106';
-- um módulo inteiro (faixa de IdJob do módulo, ex.: Movidesk 3000 a 3499), só os com agenda
UPDATE auditing.Jobs SET IsActive = 1 WHERE IdType = 2 AND IdJob BETWEEN 3000 AND 3499
   AND IntervalTimeInMinutes > 0 AND Company = '44065634000106';
```

---

## 🌐 4. Rotas do WebApi

```bash
dotnet run --project src/Backend/Hosts/WebApi --launch-profile http
```

O Swagger abre em **`http://localhost:5252/swagger`**, com uma tag por service de cada módulo.

<div align="center">

![Swagger do WebApi](docs/images/swagger/visao-geral.png)

</div>

| Tipo de rota | Exemplo | Cabeçalhos |
|---|---|---|
| 🔄 **Leitura recorrente** (carga do banco) | `POST /MovideskJobs/Movidesk/Tickets` | `X-Company: <holding>` (sem `{docCompany}` na rota, é ele que diz a empresa) |
| 🎯 **Leitura sob demanda** | `GET /TotvsJobs/Totvs/Customers/{docCompany}/{code}` | `X-User-Name` (obrigatório); `X-Company` opcional |
| ✍️ **Escrita** | `PATCH /MovideskJobs/Movidesk/Tickets/{docCompany}/{id}` | `X-User-Name`; corpo validado antes de chamar a API |

```http
GET http://localhost:5252/TotvsJobs/Totvs/Customers/44065634000106/123
X-User-Name: fulano
X-Company: 44065634000106
```

> [!CAUTION]
> Cada rota roda a integração **de verdade** contra a API configurada. Para testar sem tráfego real, aponte as
> `BaseUrl` dos módulos para uma porta local fechada: `Movidesk__BaseUrl=http://127.0.0.1:9/` (idem `Totvs`,
> `LinxMicrovix`, `AfterSale`). A Totvs de produção é **só leitura** nos testes.

---

## ⏱️ 5. Agenda

A agenda é a mesma (`auditing.Jobs`, uma linha por job e empresa) e **só um host agenda por vez** no mesmo banco. O
`WebApi` em modo só API combina com qualquer um, para o disparo manual pelas rotas.

| Host | Ligar | Desligar |
|---|---|---|
| 🌐 **WebApi (Hangfire)** | `Hangfire__Enabled=true` + usuário e senha do dashboard por variável de ambiente ou user-secrets; dashboard em `http://localhost:5252/hangfire` | `Hangfire:Enabled = false` (padrão) |
| ☁️ **WebJob (Azure)** | `dotnet run --project src/Backend/Hosts/WebJob -- --job 9002` no local; na Azure, `Publish/publish-webjob.ps1 -Profile <perfil>` | parar os WebJobs no App Service |
| 🪟 **Worker (Quartz.NET)** | `dotnet run --project src/Backend/Hosts/Worker` no local; serviço com `Publish/publish-worker.ps1 -Profile <perfil>` (como administrador) | `Stop-Service EtlWorker` ou `Quartz:Enabled = false` |

```powershell
# WebApi com o Hangfire (dashboard em http://localhost:5252/hangfire)
$env:Hangfire__Enabled = 'true'; $env:Hangfire__Dashboard__UserName = '<usuário>'; $env:Hangfire__Dashboard__Password = '<senha>'
dotnet run --project src/Backend/Hosts/WebApi --launch-profile http

# WebJob: lista os jobs que conhece, sem banco
dotnet run --project src/Backend/Hosts/WebJob -- --list-jobs
```

> [!TIP]
> A tela [`Invoke-HostsConfig.ps1`](#️-configurar-jobs-e-user-secrets-invoke-hostsconfig) grava tudo isso nos User Secrets
> de cada host. À mão, a connection string do WebJob e do Worker fica fora do `appsettings.json`:
> `dotnet user-secrets set "ConnectionStrings:Connection" "<connection string>" --project src/Backend/Hosts/Worker`.

**Carga inicial limitada (testes).** Com o banco vazio, alguns jobs leem o histórico inteiro (Totvs e Linx Microvix
desde 2000). A chave `InitialLoad:MaxMonths` limita a primeira carga aos últimos N meses, nos três hosts; a leitura
incremental (com dado no banco) não muda. Fica **desligada no git** e entra por variável de ambiente:

```powershell
$env:InitialLoad__MaxMonths = '3'
```

Vale para Totvs, Linx Microvix, Movidesk e LinxCommerce. No Microvix, a carga inicial passa a mandar a data e
`filtra_somente_timestamp = 0` (detalhes em [docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md](docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md#1-carga-inicial-limitada-a-3-meses)).

**A carga inicial completa leva dois ciclos.** No primeiro, os jobs que dependem de filiais (Totvs) e lojas (Linx
Microvix) rodam junto com os que as carregam e saem com pouco ou nada, com o aviso "sem lojas ... rode o ... antes" (nível
aviso: o job não fica em erro). No ciclo seguinte, os dados vêm. Para ter tudo no primeiro ciclo, rode antes
`TotvsGetBranchesList` e `LinxGrupoLojas` pela tela Sob Demanda ([melhorias, 5.2](docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md#52-ponto-5-carga-completa-em-2-ciclos)).

---

## 🖥️ 6. Painel

```bash
dotnet run --project src/Frontend/WebApp --launch-profile http
```

Abre em **`http://localhost:5067`**, sem API. Por padrão, os dados são **mockados**; para ler o banco, veja abaixo.

<div align="center">

![Visão geral do painel](docs/images/webapp/visao-geral.png)

</div>

| Chave (user-secrets do WebApp) | Efeito |
|---|---|
| `Admin:Enabled = true` | mostra as telas **Sob Demanda** e **Executar Querys** (só em `Development`) |
| `Assistant:ApiKey` | liga o **assistente de IA** (Claude); as perguntas e os dados consultados vão para a API da Anthropic |
| `Database:Enabled = true` | troca os mocks pelo banco: painel e cadastro em `ConnectionStrings:Connection` (usuário `etl_monitor`) e Executar Querys em `ConnectionStrings:QueryConnection` (`etl_query`) |
| `LoggedCompany:DocCompany` | CNPJ da empresa logada em `monitor.Company` (padrão `44065634000106`) |

```bash
dotnet user-secrets set "Admin:Enabled" "true" --project src/Frontend/WebApp
```

**Painel com o banco.** A tela [`Invoke-HostsConfig.ps1`](#️-configurar-jobs-e-user-secrets-invoke-hostsconfig) grava as
chaves abaixo. À mão, na máquina de desenvolvimento, contra o SQL Server do Docker (troque `SEU_BANCO` e `SUA_SENHA`; o
banco precisa dos migrators e da empresa cadastrada), e reinicie o WebApp depois:

```bash
dotnet user-secrets set "Database:Enabled" "true" --project src/Frontend/WebApp
```

```bash
dotnet user-secrets set "ConnectionStrings:Connection" "Server=localhost,1433;Database=SEU_BANCO;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True" --project src/Frontend/WebApp
```

```bash
dotnet user-secrets set "ConnectionStrings:QueryConnection" "Server=localhost,1433;Database=SEU_BANCO;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True" --project src/Frontend/WebApp
```

Em homologação e produção, use os usuários `etl_monitor` (`Connection`) e `etl_query` (`QueryConnection`), criados pelas
procedures do Script009 do Shared quando a tela do `Invoke-Migrators.ps1` recebe as senhas deles
([README do WebApp](src/Frontend/WebApp/README.md#️-banco-de-dados-fase-5)).

---

## 📊 7. Onde ver o que rodou

| Onde | O quê |
|---|---|
| `auditing.Logs` | cada execução (job, empresa, início e fim) |
| `auditing.Messages` | erros e avisos de cada execução |
| `auditing.Records` | o registro integrado (JSON ou XML) pela chave |
| `<schema>.<Módulo>Requests` | cada chamada sob demanda e escrita, com usuário, corpo e resposta |
| `auditing.vw_JobsHeartbeat` | a última execução de cada job e empresa |

```sql
-- últimas execuções com erro de uma empresa
SELECT TOP 50 L.IdJob, J.Title, L.StartDate, L.EndDate, M.Message
FROM auditing.Logs L
JOIN auditing.Jobs J ON J.IdJob = L.IdJob AND J.IdType = L.IdType AND J.Company = L.Company
JOIN auditing.Messages M ON M.Execution = L.Execution AND M.[Level] = 3
WHERE L.Company = '44065634000106'
ORDER BY L.StartDate DESC;
```

---

## 🩺 Solução de problemas

| Sintoma | Causa provável | O que fazer |
|---|---|---|
| Nada roda na agenda | nenhuma empresa ativa, ou jobs com `IsActive = 0` | passo 3: cadastrar a empresa e ligar os jobs |
| `sp_Log_Start` recusa com **50005** | execução sem empresa | informar `X-Company` na rota (ou usar uma rota com `{docCompany}`) |
| **50001** ao abrir o log | a empresa não tem aquele job | `sp_Add_Company_App` do módulo para a empresa |
| **400** numa rota com `{docCompany}` | CNPJ sem holding em `monitor.HoldingCompanies` ou `X-Company` diferente da holding | `sp_Add_Holding_Company` ou corrigir o cabeçalho |
| Migrator falha ao carregar a DLL do SQL Client | caminho com mais de 260 caracteres | usar o `Invoke-Migrators.ps1` (já usa `subst`) |
| `Skipped: previous execution ... still running` | a execução anterior da mesma empresa ainda roda | normal; se for sempre, aumentar o intervalo do job |
| `Rescheduled: job:<IdJob> running for another company` | outra empresa está rodando o mesmo job | normal: volta em 1 minuto (Hangfire e Worker) |
| Um script de credenciais roda no migrator | cópia antiga em `Scripts/SQLServer/Parameters` | mover para `Seeds/SQLServer` |
| Migrator falha com `Connection Timeout Expired` no Azure SQL | banco serverless pausado (acorda em até ≈ 90 s) | rodar de novo, ou `Connect Timeout=120` ([MIGRATOR-CSX.md](docs/MIGRATOR-CSX.md)) |
| Worker cai com `Database schema validation failed ... created the database tables` | quase sempre o banco fora do ar ou a connection string, não tabela faltando | conferir a conexão; como serviço, o Windows reinicia o processo a cada minuto até o banco voltar |
| Serviço `EtlWorker` para logo depois de iniciar | sem `appsettings.Production.json` (connection string) na pasta do serviço | criar o arquivo na pasta e rodar o `publish-worker.ps1` de novo |
| Uma execução que caiu no meio não aparece no painel | o log em `auditing.Logs` só é gravado no fim da execução | normal por enquanto: a próxima execução roda; a queda fica só no log do host |
| Execução sem nenhuma mensagem e erro de PK em `auditing.Logs` no log do host | o bulk insert de `auditing.Messages` falhou (deadlock ou "schema change") e o `CommitError` tentou gravar o log de novo | ponto em aberto dos [testes de hospedagem](docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md#83-pontos-em-aberto); a próxima execução grava normalmente |
| O mesmo job roda duas vezes por ciclo | dois hosts de agenda ligados no mesmo banco | deixar só um (passo 5) |
| Painel publicado mostra "Atualizado às" 3 h adiantado | servidor em UTC (container, App Service) e a tela usa a hora da máquina | os horários das execuções estão certos; ver o [README do WebApp](src/Frontend/WebApp/README.md#-publicado-production) |
| Executar Querys e Sob Demanda somem do menu | painel em `Production` | esperado: as duas só existem em `Development` |
| Erro `access a socket in a way forbidden by its access permissions` no App Service | `BaseUrl` numa porta local (`127.0.0.1:9`) dentro da sandbox da Azure | esperado nos testes sem API |

---

## 📚 Mais detalhes

| Assunto | Onde |
|---|---|
| Panorama, módulos, banco e DW | [README](README.md) |
| Migrators e a tela | [docs/MIGRATOR-CSX.md](docs/MIGRATOR-CSX.md) |
| Cadastro de empresas | [docs/PLANO-TELA-CADASTRO-EMPRESA.md](docs/PLANO-TELA-CADASTRO-EMPRESA.md) |
| Padrões de código, scripts e cache | [docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md) · [docs/PADRAO-CACHE-INTEGRACOES.md](docs/PADRAO-CACHE-INTEGRACOES.md) |
| Data warehouse | [src/Backend/Modules/DataWarehouse/docs](src/Backend/Modules/DataWarehouse/docs/00-ordem-de-leitura.md) |
| Hospedagem testada (Docker, nginx, serviço do Windows, Azure) e pontos em aberto | [docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md](docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md) |

---

<div align="center">

![NEWBLOOMERS](docs/newbloomers.png)

<sub>Sistema de integrações · feito com .NET 10</sub>

</div>
