# Proposta: schema `auditing` com Holders, Companies e Catalog

Data: 04/11/2026. Fonte: banco `Company` do container `sql-server` (Docker, porta 1433), alterado à mão depois do
migrator das 18:16 (as alterações têm horário entre 21:55 e 21:57). Este documento só propõe. Nenhum script e nenhum código
foi alterado.

Regra do usuário:

- Os scripts de insert de jobs que o DbUp roda gravam em `auditing.Types` e `auditing.Catalog`, e só nessas tabelas.
- `auditing.Holders`, `auditing.Companies` e `auditing.Jobs` são gravadas pelo WebApp, na tela de cadastro que ainda vai
  ser feita, com INSERT direto. Nenhuma das três tem seed no DbUp.
- Correção vai no próprio script de criação (Script001, Script002...), não em script novo com `ALTER`.

## Andamento

- **Seção 2 (scripts do DbUp): feita em 04/11/2026.**
  - Alterados: Script001, Script002, Script006, Script007, Script008 e Script009 do Shared, e os scripts de insert de
    jobs dos módulos.
  - A função se chama `auditing.fn_CatalogTree`.
  - Em `monitor.AppImage` e `monitor.AppSetting`, a coluna `Company` virou `Holder`.
  - O script do DataWarehouse (`insert into` em minúsculas) não estava na primeira cópia da agenda. Os 5 jobs dele foram
    acrescentados ao `.csv`, que ficou com 961 jobs.
  - **Teste:** os migrators rodaram num banco descartável no Docker, com Shared e as 6 aplicações. Ele foi apagado no fim.
    - O `Catalog` ficou igual ao `.csv` (961 jobs, mesmo `IsOnDemand`).
    - As regras de holding, empresa e job funcionaram: a PK de Companies e a FK de Jobs recusaram o que deviam, e
      `sp_Log_Start`, `sp_Log_Message`, `sp_Log_End` e `sp_Log_Error` gravaram com `Holder`.
    - A view `vw_JobsHeartbeat` e a FK de `JobAcknowledgement` funcionaram.
    - Nenhum objeto aponta para tabela que não existe, exceto o `auditing.sp_Log_nd` do Script004, um erro de digitação
      que já existia.
- **Seção 3 (C#, `.ps1`, WebApp e documentação): a fazer.** Até lá, os hosts, o WebApp e os `.ps1` não funcionam num
  banco migrado com estes scripts.

## 0. Decisões de 04/11/2026

| | Decisão |
|---|---|
| D1 | Uma holding (`Holders`) tem n empresas (`Companies`). Uma empresa pode ser a sua própria holding. Exemplos: a holding NEWBLOOMERS tem as empresas Misha e Open Era; a holding fictícia Abacartes tem uma empresa só, a própria Abacartes (`Holder = Company`). Os jobs e o log são por empresa: a chave de Jobs e Logs é `Holder + Company`. |
| D2 | A agenda (`IntervalTimeInMinutes`, `RunningDailyAt` e `Queue`) fica só em `auditing.Jobs`. O `Catalog` tem só uma coluna que diz se o job é sob demanda. |
| D3 | As linhas de `auditing.Jobs` são criadas por INSERT direto do WebApp. `sp_Add_Company_App`, `sp_Remove_Company_App`, `sp_Add_Company` e `sp_Add_Holding_Company` saem. |
| D4 | A tabela é `auditing.Companies` (fora do `dbo`), e `FantasyName` é `varchar(250)`. |
| P1 | Os jobs dos hosts (IdParent 140, como o 14000 StuckJobsRecovery) são um por holding. Ficam na linha da empresa que é a própria holding (`Company = Holder`). Por isso toda holding tem essa linha em `Companies`, mesmo sem parâmetros no próprio CNPJ (a NEWBLOOMERS também). |
| P2 | A agenda que hoje está nos scripts vira uma sugestão fixa na tela do WebApp. A cópia foi tirada antes de as colunas saírem: [`docs/AGENDA-SUGERIDA-JOBS.csv`](AGENDA-SUGERIDA-JOBS.csv). |

---

## 1. O que mudou no banco (diferença em relação aos scripts)

### 1.1 Tabelas novas

| Tabela | Colunas | Chave | Observações |
|---|---|---|---|
| `auditing.Holders` | `FantasyName nvarchar(250) NULL`, `Document varchar(20) NOT NULL` | PK `PK_Holders (Document)` | É a holding. |
| `dbo.Companies` | `FantasyName nchar(10) NULL`, `Company varchar(20) NOT NULL`, `Holder varchar(20) NOT NULL` | PK `PK_Companies (Company, Holder)` | FK `FK_Companies_Holders (Holder) → auditing.Holders (Document)`. Vai para `auditing` (D4). |

### 1.2 `auditing.JobCatalog` → `auditing.Catalog`

- O nome mudou de `JobCatalog` para `Catalog`. A PK continua `PK_JobCatalog (IdJob, IdType)`.
- As colunas `IntervalTimeInMinutes`, `RunningDailyAt` e `Queue` saíram, junto com `CK_JobCatalog_RunningDailyAt` e
  `CK_JobCatalog_Queue`.
- Ficaram as colunas `IdParent, IdJob, Title nvarchar(80), Description nvarchar(800), IdType`.
- A FK para Types virou `FK_Catalog_Types` (antes era `FK_JobCatalog`).

### 1.3 `auditing.Jobs`

- As colunas `IdParent`, `Title` e `Description` saíram: agora só existem no Catalog.
- A coluna `Holder varchar(20) NOT NULL` entrou.
- A PK virou `PK_Jobs_1 (IdJob, Holder, Company, IdType)`. Antes era `PK_Jobs (IdJob, IdType, Company)`.
- FKs:
  - `FK_Jobs_Catalog (IdJob, IdType) → auditing.Catalog`: nova, no lugar de `FK_Jobs_JobCatalog`.
  - `FK_Jobs_Companies (Company, Holder) → dbo.Companies`: nova.
  - `FK_Jobs_Company (Company) → monitor.Company`: **continua lá** (vem do Script008).
  - `FK_Jobs (IdType) → auditing.Types`: saiu. Não faz falta, porque o tipo já é garantido pelo Catalog.
- `CK_Jobs_RunningDailyAt`, `CK_Jobs_Queue` e `DF_Jobs_IsActive` continuam.

### 1.4 `Logs`, `Messages` e `Records`

Nas três tabelas entrou `Holder varchar(20) NOT NULL` logo depois de `IdJob`, e ele faz parte da PK e da FK:

| Tabela | PK nova | FK nova |
|---|---|---|
| Logs | `PK_Logs (IdJob, Holder, Company, Execution, IdType)` | `FK_Logs_Jobs → Jobs (IdJob, Holder, Company, IdType)` |
| Messages | `PK_Messages (IdExecutionMessage, IdJob, Holder, Company, Execution, IdType)` | `FK_Messages_Logs → Logs` |
| Records | `PK_Records_1 (IdRecord, IdJob, Holder, Company, Execution, IdType)` | `FK_Records_Logs → Logs` |

### 1.5 O que se perdeu na edição (provavelmente o designer do SSMS recriou as tabelas)

Isto não parece intencional. Proponho manter tudo no script, mas corrigido para incluir `Holder` (ver 2.3).

- Índices que sumiram: `IX_Logs_Heartbeat`, `IX_Logs_JobStartDate`, `IX_Messages_Execution` e `IX_Records_Execution`.
  Ficaram só `IX_Logs_StartDate` e `IX_Logs_Execution`.
- A compressão `DATA_COMPRESSION = ROW` saiu de Logs e Messages, e o `FILLFACTOR = 90` saiu das PKs.
- `monitor.JobAcknowledgement` perdeu a `FK_JobAcknowledgement_Jobs`, porque ela apontava para a PK antiga de Jobs.
- O `dbo.sysdiagrams` foi criado pelo diagrama do SSMS. Ele não entra no DbUp.

### 1.6 Pontos que o script corrige

1. **`Companies` passa para `auditing`** (D4).
2. **`Companies.FantasyName` passa de `nchar(10)` para `varchar(250) NULL`** (D4). O banco está em
   `Latin1_General_100_CI_AS_SC_UTF8`, então o `varchar` guarda acentos sem precisar de `nvarchar`.
   `Holders.FantasyName` está como `nvarchar(250)`. Para as duas ficarem iguais, a proposta usa `varchar(250)` nas duas.
3. **A PK de `Companies` passa a ser `(Company)`, com UNIQUE `(Holder, Company)`.** Uma empresa pertence a uma holding só
   (D1). O UNIQUE serve de alvo para a FK composta de Jobs, e o `Holder` sai de graça a partir do `Company` (ver 2.4).
4. **`monitor.Company` e `monitor.HoldingCompanies` saem**, junto com a `FK_Jobs_Company`. `Holders` + `Companies`
   fazem o papel dos dois (ver 2.6).
5. **O Catalog ganha `IsOnDemand`** (D2). Ele marca os jobs que hoje têm `IntervalTimeInMinutes = 0` (os do
   TotvsController e afins).

### 1.7 Exemplo de dados (D1)

| `auditing.Holders` (Document, FantasyName) | `auditing.Companies` (Company, Holder, FantasyName) |
|---|---|
| `<CNPJ NEWBLOOMERS>`, NEWBLOOMERS | `<CNPJ NEWBLOOMERS>`, `<CNPJ NEWBLOOMERS>`, NEWBLOOMERS |
| | `<CNPJ Misha>`, `<CNPJ NEWBLOOMERS>`, Misha |
| | `<CNPJ Open Era>`, `<CNPJ NEWBLOOMERS>`, Open Era |
| `<CNPJ Abacartes>`, Abacartes | `<CNPJ Abacartes>`, `<CNPJ Abacartes>`, Abacartes |

- **Jobs de integração da NEWBLOOMERS:** ficam em `(Holder = NEWBLOOMERS, Company = Misha)` e
  `(Holder = NEWBLOOMERS, Company = Open Era)`, cada um com a própria agenda e a própria ativação.
- **Jobs dos hosts (P1):** ficam em `(Holder = NEWBLOOMERS, Company = NEWBLOOMERS)`.
- **Linha da própria holding:** ao criar a holding, a tela grava junto a linha `Companies (Holder, Holder)` e os jobs
  do IdParent 140. Se a holding também tiver parâmetros no próprio CNPJ, as integrações dela usam essa mesma linha.
- **Abacartes:** tem uma linha só, que serve para tudo.

---

## 2. Alterações propostas nos scripts do DbUp

Todas vão no script de criação original, em `src/Backend/Shared/Infrastructure.Shared/Scripts/SQLServer/`.

### 2.1 Script001: tabelas

```sql
-- holdings clientes: cadastradas pela tela do WebApp, nunca pelo DbUp
CREATE TABLE [auditing].[Holders](
    [Document] [varchar](20) NOT NULL,
    [FantasyName] [varchar](250) NULL,
    CONSTRAINT [CK_Holders_Document] CHECK (LEN([Document]) > 0 AND [Document] NOT LIKE '%[^0-9]%'),
 CONSTRAINT [PK_Holders] PRIMARY KEY CLUSTERED ([Document] ASC)
) ON [PRIMARY]
GO

-- empresas de cada holding (o doc_company das tabelas *Parameters): uma holding tem n empresas, uma empresa é de uma
-- holding só, e a empresa pode ser a própria holding (Company = Holder). Cadastradas pela tela do WebApp
CREATE TABLE [auditing].[Companies](
    [Company] [varchar](20) NOT NULL,
    [Holder] [varchar](20) NOT NULL,
    [FantasyName] [varchar](250) NULL,
    CONSTRAINT [CK_Companies_Company] CHECK (LEN([Company]) > 0 AND [Company] NOT LIKE '%[^0-9]%'),
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([Company] ASC),
 CONSTRAINT [UQ_Companies_Holder] UNIQUE ([Holder] ASC, [Company] ASC)
) ON [PRIMARY]
GO

-- catálogo de jobs do produto, sem empresa e sem agenda: os scripts Insert_*_Jobs dos módulos inserem aqui
CREATE TABLE [auditing].[Catalog](
	[IdParent] [int] NULL,
	[IdJob] [int] NOT NULL,
	[Title] [nvarchar](80) NULL,
	[Description] [nvarchar](800) NULL,
	[IdType] [int] NOT NULL,
	-- 1: executado sob demanda (controllers), não é agendado; 0: recorrente, agenda em auditing.Jobs
	[IsOnDemand] [bit] NOT NULL CONSTRAINT [DF_Catalog_IsOnDemand] DEFAULT (0),
 CONSTRAINT [PK_Catalog] PRIMARY KEY CLUSTERED ([IdJob] ASC, [IdType] ASC)
 WITH (FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

-- jobs de cada empresa, com a agenda e a ativação: gravados pela tela do WebApp (INSERT direto)
CREATE TABLE [auditing].[Jobs](
	[IdJob] [int] NOT NULL,
	[IdType] [int] NOT NULL,
	[Holder] [varchar](20) NOT NULL,
	[Company] [varchar](20) NOT NULL,
	[IntervalTimeInMinutes] [int] NOT NULL,
	[RunningDailyAt] [time](0) NULL,
	[Queue] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Jobs_IsActive] DEFAULT (0),
	CONSTRAINT [CK_Jobs_RunningDailyAt] CHECK ([RunningDailyAt] IS NULL OR [IntervalTimeInMinutes] = 1440),
	CONSTRAINT [CK_Jobs_Queue] CHECK (LEN([Queue]) > 0 AND [Queue] COLLATE Latin1_General_BIN NOT LIKE '%[^a-z0-9_-]%'),
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED ([IdJob] ASC, [IdType] ASC, [Holder] ASC, [Company] ASC)
 WITH (FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
```

- Em Logs, Messages e Records entra `[Holder] [varchar](20) NOT NULL` depois de `IdType`. As PKs passam a ser
  `(IdJob, IdType, Holder, Company, Execution)`, com o `IdExecutionMessage` ou o `IdRecord` na frente, como hoje. Os
  nomes ficam `PK_Logs`, `PK_Messages` e `PK_Records`, e voltam `DATA_COMPRESSION = ROW` e `FILLFACTOR = 90` como no
  script atual.
- A ordem das colunas da PK sugerida é `IdJob, IdType, Holder, Company`. Ela mantém o padrão `IdJob, IdType` do script
  atual. Não é obrigatória: a ordem do banco (`IdJob, Holder, Company, IdType`) também funciona. Basta ser a mesma em
  todas as FKs.
- **Job sob demanda em Jobs:** uma CHECK não consegue ler o Catalog. Por isso, quando `Catalog.IsOnDemand = 1`, o WebApp
  grava `IntervalTimeInMinutes = 0`. O agendador continua ignorando os jobs com intervalo 0, como faz hoje, ou passa a ler
  `IsOnDemand` com um JOIN no Catalog.
- `Types`, `Apps` e `SchedulingHosts` ficam como estão.

### 2.2 Script001: FKs

```sql
ALTER TABLE [auditing].[Companies] ADD CONSTRAINT [FK_Companies_Holders] FOREIGN KEY([Holder])
REFERENCES [auditing].[Holders] ([Document])
GO
ALTER TABLE [auditing].[Catalog] ADD CONSTRAINT [FK_Catalog_Types] FOREIGN KEY([IdType])
REFERENCES [auditing].[Types] ([IdType])
GO
ALTER TABLE [auditing].[Jobs] ADD CONSTRAINT [FK_Jobs_Catalog] FOREIGN KEY([IdJob], [IdType])
REFERENCES [auditing].[Catalog] ([IdJob], [IdType])
GO
ALTER TABLE [auditing].[Jobs] ADD CONSTRAINT [FK_Jobs_Companies] FOREIGN KEY([Holder], [Company])
REFERENCES [auditing].[Companies] ([Holder], [Company])   -- alvo: UQ_Companies_Holder
GO
ALTER TABLE [auditing].[Logs] ADD CONSTRAINT [FK_Logs_Jobs] FOREIGN KEY([IdJob], [IdType], [Holder], [Company])
REFERENCES [auditing].[Jobs] ([IdJob], [IdType], [Holder], [Company])
GO
ALTER TABLE [auditing].[Messages] ADD CONSTRAINT [FK_Messages_Logs] FOREIGN KEY([IdJob], [IdType], [Holder], [Company], [Execution])
REFERENCES [auditing].[Logs] ([IdJob], [IdType], [Holder], [Company], [Execution])
GO
ALTER TABLE [auditing].[Records] ADD CONSTRAINT [FK_Records_Logs] FOREIGN KEY([IdJob], [IdType], [Holder], [Company], [Execution])
REFERENCES [auditing].[Logs] ([IdJob], [IdType], [Holder], [Company], [Execution])
GO
```

Os nomes antigos (`FK_Jobs`, `FK_Logs`, `FK_Messages`, `FK_Records`, `FK_JobCatalog` e `FK_Jobs_JobCatalog`) saem.

### 2.3 Script001: índices

Os índices voltam ao script com `Holder`. O painel filtra pela holding logada; a empresa é um filtro dentro dela:

| Índice | Chave | INCLUDE |
|---|---|---|
| `IX_Logs_Heartbeat` | `Holder, Company, IdJob, IdType, EndDate` | — |
| `IX_Logs_StartDate` | `Holder, StartDate` | `IdJob, IdType, Company, EndDate, Execution` |
| `IX_Logs_JobStartDate` | `Holder, Company, IdJob, IdType, StartDate DESC` | `EndDate, ExecutionParent` |
| `IX_Logs_Execution` | `Execution` | — |
| `IX_Messages_Execution` | `Holder, Company, IdJob, IdType, Execution` | `Level` |
| `IX_Records_Execution` | `Holder, Company, IdJob, IdType, Execution` | — |

### 2.4 Script002: procedures de log e de catálogo

- **`sp_Log_Start`**: o título está no Catalog e o `Holder` sai de `auditing.Companies`:
  ```sql
  SELECT @holder = Holder FROM [auditing].[Companies] WHERE Company = @company;   -- PK em Company
  -- sem linha: THROW 50002 'Empresa ... não cadastrada em auditing.Companies.'
  SELECT @idJob = j.IdJob
  FROM [auditing].[Jobs] j
  JOIN [auditing].[Catalog] c ON c.IdJob = j.IdJob AND c.IdType = j.IdType
  WHERE c.Title = @jobTitle AND j.IdType = @idType AND j.Holder = @holder AND j.Company = @company;
  INSERT INTO [auditing].[Logs] (IdJob, IdType, Holder, Company, StartDate, Execution, ExecutionParent) ...
  ```
  A assinatura não muda. O que muda é o valor de `@company`: antes era a holding, agora é a empresa da execução (D1).
  Quem chama a procedure precisa passar a empresa (ver seção 3).
- **`sp_Log_Message`, `sp_Log_Error` e `sp_Log_End`**: copiam `Holder` junto com `IdJob, IdType, Company` de
  `auditing.Logs` no `INSERT INTO Messages`.
- **`fn_JobCatalogTree`**: fica, porque a tela e os `.ps1` precisam dela para listar os jobs de um módulo (com as
  procedures abaixo) antes de fazer o INSERT. `[auditing].[JobCatalog]` passa a `[auditing].[Catalog]`, e a função pode
  ser renomeada para `fn_CatalogTree`.
- **`sp_Add_Company_App` e `sp_Remove_Company_App` saem** (D3). Na tela, contratar um módulo vira um
  `INSERT INTO auditing.Jobs (...) SELECT ... FROM auditing.fn_CatalogTree(@idParent)` com a agenda informada na tela, e
  remover vira um `UPDATE ... SET IsActive = 0`.

### 2.5 Script006: `vw_JobsHeartbeat`

`IdParent` e `Title` passam a vir do Catalog. `Holder` entra no SELECT e nos OUTER APPLY:

```sql
SELECT c.IdParent, j.IdJob, j.Holder, j.Company, c.Title, j.IntervalTimeInMinutes, j.RunningDailyAt, j.Queue, j.IsActive, ...
FROM [auditing].[Jobs] j
JOIN [auditing].[Catalog] c ON c.IdJob = j.IdJob AND c.IdType = j.IdType
OUTER APPLY (... WHERE l.IdJob = j.IdJob AND l.IdType = j.IdType AND l.Holder = j.Holder AND l.Company = j.Company ...)
WHERE j.IdType = 2 AND c.IsOnDemand = 0
```

O filtro `c.IsOnDemand = 0` tira os jobs sob demanda do heartbeat. Eles não têm intervalo, então nunca ficam "off".

### 2.6 Script008: schema `monitor`

- `monitor.Company` e `monitor.HoldingCompanies` saem, junto com o `IX_HoldingCompanies_Holding`, a
  `FK_HoldingCompanies_Company` e a `FK_Jobs_Company`. Os papéis deles passam para `auditing.Holders` e
  `auditing.Companies`.
- `sp_Add_Company` e `sp_Add_Holding_Company` saem (D3). A regra "uma empresa é de uma holding só" passa a ser garantida
  pela PK de Companies.
- `monitor.AppImage` e `monitor.AppSetting`: a coluna `Company` passa a guardar a holding. Proposta: renomear para
  `Holder`, mantendo `'*'` como padrão. Ela não tem FK, então pode ficar como está se o nome não incomodar.
- `monitor.JobAcknowledgement`: entra a coluna `Holder`. A PK fica `(IdJob, IdType, Holder, Company)`, e a
  `FK_JobAcknowledgement_Jobs` volta apontando para a PK nova de Jobs.

### 2.7 Script009: permissões do `etl_monitor`

- `GRANT INSERT, UPDATE` em `auditing.Holders` e `auditing.Companies`.
- `GRANT INSERT` em `auditing.Jobs`. O `GRANT UPDATE` em `Jobs (IsActive, IntervalTimeInMinutes, RunningDailyAt, Queue)`
  fica.
- Os `GRANT EXECUTE` de `sp_Add_Company`, `sp_Add_Holding_Company`, `sp_Add_Company_App` e `sp_Remove_Company_App`
  saem. O `GRANT SELECT ON SCHEMA::[auditing]` já cobre a `fn_CatalogTree`.

### 2.8 Scripts de insert de jobs (61 arquivos)

São o Script007 do Shared e os `Script*_Insert_*_Jobs_Into_Jobs_Table.sql` e `Script002_Insert_*_Values_Into_Jobs_Table.sql`
dos módulos (Totvs, LinxMicrovix, LinxCommerce, Movidesk, AfterSale, DataWarehouse, Dootax, Mobsim, Oto, Pagarme e
Shippment).

```sql
INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
(130, 13001, 'TotvsPostSearchLegalEntity', 'Integração das pessoas jurídicas ...', 2, 0),
(130, 13010, 'TotvsGetCustomerByCode', 'Integração de uma pessoa física por código ...', 2, 1),
...
```

- Saem `IntervalTimeInMinutes`, `RunningDailyAt` e `Queue`.
- `IsOnDemand = 1` nas linhas que hoje têm `IntervalTimeInMinutes = 0`, e `0` em todas as outras.
- **A agenda que está hoje nos scripts vira sugestão fixa na tela (P2).** A cópia é
  [`docs/AGENDA-SUGERIDA-JOBS.csv`](AGENDA-SUGERIDA-JOBS.csv), com as colunas `IdJob;IdType;IdParent;Title;`
  `IntervalTimeInMinutes;RunningDailyAt;Queue;IsOnDemand`.
  - **Origem:** saiu dos 61 scripts, carregados no Docker num banco temporário e já apagado, em 04/11/2026.
  - **Conteúdo:** 956 jobs, sendo 498 procedures e 458 integrações. Desses, 133 são sob demanda (intervalo 0) e 47 são
    diários (1440 + hora). Todos usam a fila `default`.
  - **No WebApp:** os valores viram uma lista fixa com a sugestão de cada `(IdJob, IdType)`. Ao contratar um módulo, a
    tela preenche intervalo, hora e fila com essa sugestão, e quem cadastra pode mudar antes do INSERT.
  - **Nos `.ps1`:** o `Import-TestSeeds.ps1` e o `Invoke-HostsConfig.ps1` leem o mesmo `.csv` para gravar os jobs de teste.
  - **Quando um módulo ganhar job novo:** o `Catalog` recebe a linha pelo DbUp, e a sugestão entra no `.csv` e na lista
    do WebApp no mesmo commit.
  - **A coluna `IsOnDemand` do `.csv`:** é o valor que vai para o `Catalog` (1 onde o intervalo é 0).
- Os comentários que citam `auditing.JobCatalog`, `sp_Add_Company` e "cada empresa recebe o seu 14000" são atualizados.
- O `Script005` (Types) e o insert em `auditing.Apps` do Script007 ficam como estão.

### 2.9 Banco que já rodou o migrator

O DbUp não reaplica script registrado em `SchemaVersions`. O banco do Docker é descartável: a ideia é apagá-lo e rodar
os migrators de novo depois da mudança. Qualquer outro banco que já tenha rodado os Script001 a Script009 vai precisar de
um ajuste à parte, que você decide.

---

## 3. O que fora do DbUp é afetado

Esta proposta não altera nada disto. A lista é para o planejamento.

**Consequência de D1 no runtime.** Hoje o agendador cria um job por holding (`Company` = holding). Os services leem
todos os CNPJs dela com `JOIN monitor.HoldingCompanies ... WHERE Holding = @company`, numa execução só. Com job por
empresa, isto muda:

- O agendador passa a registrar um job por linha `(Holder, Company)` de `auditing.Jobs`. A chave do Hangfire, do Quartz
  ou do WebJob passa a incluir o Company.
- O contexto da execução (`ICompanyContext` / `CompanyContextFilter`) passa a levar `Holder` e `Company`.
- Os services filtram os `*Parameters` direto por `doc_company = @company`, e o `HoldingCompanyRepository` deixa de
  existir ou passa a ler `auditing.Companies`.
- As chamadas a `sp_Log_Start` (C#, Script003 e Script004) passam a receber a empresa da execução.

**Arquivos afetados**:

- **Ferramentas**:
  - `tools/hosts/Invoke-HostsConfig.ps1` e `tools/seeds/Import-TestSeeds.ps1`: em vez das procedures que saem, passam a
    fazer INSERT direto em Holders, Companies e Jobs. Também usam `fn_JobCatalogTree` e leem `Jobs.Title`/`IdParent`.
  - `tools/migrators/Invoke-Migrators.ps1` também cita essas tabelas.
- **Backend (Shared e hosts)**:
  - `JobScheduleRepository`, `JobSchedule`, `JobHeartbeat`, `HoldingCompanyRepository` / `IHoldingCompanyRepository`,
    `ICompanyContext` e `CompanyContextFilter`.
  - `SchedulerRefreshJob` (WebApi e Worker), `JobScheduleWorker`, `JobCron`, `StuckJobsRecovery` e
    `Hosts.Scheduling/RecurringJobs/*`.
  - Os services e handlers dos módulos (Totvs, LinxMicrovix, LinxCommerce, Movidesk e AfterSale) que montam o filtro
    por holding.
- **WebApp**:
  - `SqlCompanyRepository`, `SqlMonitorRepository`, `SqlModuleParametersRepository`, `ReadyQueries`, os mocks
    (`MockCatalog`, `MockCompanyRepository`) e os modelos (`CompanyModels`, `JobModel` e `LoggedCompany`).
  - A tela de cadastro grava Holders, Companies e Jobs, e a agenda de cada job.
- **Documentação**: `docs/PLANO-TELA-CADASTRO-EMPRESA.md` (4.1, 4.2.1 e 4.3), `README.md`, `GUIA-DE-USO.md`,
  `DEPLOY.md`, `docs/MIGRATOR-CSX.md` e os READMEs dos hosts.

---

## 4. Pontos em aberto

Nenhum. As decisões D1 a D4, P1 e P2 estão na seção 0.
