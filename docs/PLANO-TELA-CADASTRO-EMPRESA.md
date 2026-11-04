# Plano: cadastro de empresas e integrações pela tela (fora do DbUp)

Branch: `implementacao-tela-cadastro`. Status: **4.1 e 4.2 feitas**; as fases seguintes ainda não começaram.

Base: [PROPOSTA-DOC-COMPANY-AUDITING.md](../src/Backend/Shared/docs/PROPOSTA-DOC-COMPANY-AUDITING.md). Este plano
substitui a fase F2 (seed por procedure a partir das tabelas `*Parameters`) daquela proposta e muda o nome da coluna.

## 1. Decisões de 01/11/2026

1. O sistema ETL não atende só a NEWBLOOMERS: vai atender outros clientes. **Dados de cliente não ficam no DbUp.** A
   empresa, as integrações que ela usa e as credenciais de cada integração são cadastradas numa tela própria do WebApp.
2. A coluna de empresa no schema `auditing` se chama **`Company`** (não `DocCompany`).
3. `Company` faz parte da **PK de todas as tabelas de auditing** com empresa: `Jobs`, `Logs`, `Messages` e `Records`.
4. Para testes, os scripts de inserção de parâmetros que existem hoje não se perdem: um script `.ps1` aplica todos de uma
   vez num banco de teste, sem cadastrar empresa por empresa na tela (passo 3).
5. O catálogo do produto (`auditing.Types`, `auditing.Apps` e o catálogo de jobs) **fica no DbUp**.
6. `monitor.AppImage` e `monitor.AppSetting` também usam **`Company`**: um nome só para a empresa no banco.
7. **Toda empresa tem os jobs dos hosts.** Cada empresa tem o seu próprio 14000 (`StuckJobsRecovery`), que nasce junto
   com a empresa, sem precisar contratar nada.
8. Credenciais em **texto puro** nesta fase.
9. A tela de cadastro tem um botão **Testar conexão** por módulo (passo 4.6).

Decisões de 02/11/2026:

10. **Os parâmetros não mudam.** O `doc_company` de cada `*Parameters` continua com o CNPJ que está lá (marca ou
    holding). A ligação com a empresa do painel é a tabela de/para **`monitor.HoldingCompanies`** (passo 4.2.1).
11. Uma holding só entra em `monitor.HoldingCompanies` se algum módulo tiver parâmetros com o CNPJ dela. Cliente de um
    CNPJ só é a própria holding (`X → X`).

Continuam valendo as decisões de 31/10/2026 da proposta: ativação é `UPDATE` em `auditing.Jobs.IsActive` por (job,
empresa), sem tabela de ativação; uma execução do agendador por (job, empresa); a empresa do painel é a holding; banco
zerado, sem backfill.

## 2. O que fica no DbUp e o que sai

O DbUp passa a criar **estrutura e catálogo do produto**. Tudo o que depende de um cliente sai.

| Fica no DbUp (produto) | Sai do DbUp (cliente) |
|---|---|
| schemas, tabelas, procedures, views, tipos | linhas de `*Parameters` (credenciais e endereços de cada cliente) |
| `auditing.Types` (Script005 do Shared) | linhas de `auditing.Jobs` (agora uma por empresa) |
| `auditing.Apps`: catálogo de módulos | |
| `auditing.JobCatalog`: catálogo de jobs, sem empresa (4.1), inclusive o 14000 dos hosts | |
| tabelas de referência do DataWarehouse (`Script006_Insert_DataWarehouse_Reference.sql`) | |

### 2.1 Scripts de parâmetros (saem do DbUp) e de jobs (ficam, no catálogo)

Feito em 4.1. Os scripts de parâmetros foram para `Seeds/SQLServer`. Os de jobs ficaram onde estavam e passaram a inserir
em `auditing.JobCatalog`; os nomes dos arquivos (`..._Into_Jobs_Table.sql`) não mudaram.

| Módulo | Parâmetros (credenciais) → `Seeds/SQLServer` | Jobs → `auditing.JobCatalog` (no DbUp) |
|---|---|---|
| Shared | — | `Script007_Insert_Hosts_Jobs_Into_Jobs_Table.sql` (o 14000) |
| Totvs | `Script003_Insert_Values_Into_Totvs_Parameters_Table.sql` | 12 scripts `Insert_Totvs_*_Jobs` |
| LinxMicrovix | `Script003_Insert_Values_Into_LinxMicrovix_Parameters_Table.sql` (fora do git; o `.example` foi junto) | 29 scripts (`Erp/*` e `B2C/*`) |
| LinxCommerce | `Script003_Insert_Values_Into_LinxCommerce_Parameters_Table.sql` (fora do git; o `.example` foi junto) | 4 scripts |
| Movidesk | `Script003_Insert_Values_Into_Movidesk_Parameters_Table.sql` | 3 scripts |
| AfterSale | `Script005_Insert_Values_Into_AfterSale_Parameters_Table.sql` | 4 scripts |
| DataWarehouse | — | `Parameters/Script007_Insert_DataWarehouse_Jobs_Into_Jobs_Table.sql` |
| Dootax, Mobsim, Oto, Pagarme, FlashCourier, Jadlog, Stone, TotalExpress | — | `Script002_Insert_*_Values_Into_Jobs_Table.sql` (só comentários) |

**Para onde foram:** os migrators leem `Scripts/SQLServer` com subpastas (`IncludeSubDirectories = true`), então mover
para uma subpasta não bastava. Os scripts de parâmetros ficam numa pasta irmã, fora do alcance do DbUp:

```
Infrastructure.<Módulo>/
  Scripts/SQLServer/   ← DbUp (estrutura e catálogo, inclusive os jobs)
  Seeds/SQLServer/     ← só o .ps1 de teste lê (parâmetros de uma empresa)
```

O `.gitignore` ignora os dois scripts de credenciais que ficam fora do git (LinxMicrovix e LinxCommerce) no caminho novo e
**continua ignorando o caminho antigo**: uma cópia local que ficou em `Scripts/SQLServer/Parameters` não vai para o git,
mas **o DbUp ainda a executa**. Quem tem essa cópia precisa movê-la para `Seeds/SQLServer`. Os scripts que estão no git
por decisão do usuário (Totvs, Movidesk, AfterSale) continuam no git, só mudaram de pasta.

## 3. Script `.ps1` de dados de teste — feito em 4.2

**Objetivo:** subir um banco de teste completo (empresa, parâmetros e jobs de todos os módulos) com um comando, usando os
scripts de `Seeds/` (passo 2.1), sem passar pela tela de cadastro.

Arquivo: [`tools/seeds/Import-TestSeeds.ps1`](../tools/seeds/Import-TestSeeds.ps1) (PowerShell 7).

```powershell
./tools/seeds/Import-TestSeeds.ps1 -Database 'EtlTeste'
./tools/seeds/Import-TestSeeds.ps1 -Database 'EtlTeste' -Modules Totvs,DataWarehouse
```

| Parâmetro | Padrão | Uso |
|---|---|---|
| `-Database` | obrigatório | banco de teste já migrado; **recusa** `Company` e qualquer nome sem `Test`/`Teste`, salvo com `-Force` |
| `-Server` | `localhost,1433` | instância do container `sql-server` do Docker |
| `-Credential` | pede com `Get-Credential` | usuário e senha do SQL; a senha vai para o `sqlcmd` pela variável `SQLCMDPASSWORD`, nunca pela linha de comando |
| `-Company` | `44065634000106` | empresa dos jobs (holding NEWBLOOMERS), só dígitos |
| `-CompanyName` | `NEWBLOOMERS` | razão social gravada em `monitor.Company` quando a empresa ainda não existe |
| `-Modules` | todos | `Totvs`, `LinxMicrovix`, `LinxCommerce`, `Movidesk`, `AfterSale`, `DataWarehouse` (só jobs) |
| `-Force` | desligado | aceita um banco fora da regra de nome |

O que ele faz, em ordem:

1. Confere que os migrators já rodaram (`auditing.JobCatalog` com jobs e `monitor.Company`). Não roda migrator: isso
   continua sendo o `migrator.csx`.
2. Cadastra a empresa com `auditing.sp_Add_Company` (passo 4.1), se ainda não existir. Isso já cria os jobs dos hosts.
3. Para cada módulo, roda o script `*_Insert_Values_Into_*_Parameters_Table.sql` de `Seeds/SQLServer` com `sqlcmd -b`,
   numa transação. Script fora do git que não existir na máquina (LinxMicrovix, LinxCommerce) gera um aviso e o módulo
   fica sem parâmetros, sem parar o resto; se o script ainda estiver no caminho antigo (`Scripts/SQLServer/Parameters`,
   que o DbUp executa), o aviso pede para movê-lo.
4. Liga cada CNPJ (`doc_company`) do script de parâmetros à empresa com `auditing.sp_Add_Holding_Company` (4.2.1). Um
   CNPJ que já é de outra holding é recusado pela procedure e vira aviso.
5. Para cada módulo, chama `auditing.sp_Add_Company_App @company, @idParent`, que copia o catálogo de jobs do módulo para
   `auditing.Jobs` com `Company = @company`.
6. (Até 04/11/2026, com `-ActivateJobs`, ativava os jobs. Saiu: os jobs ficam inativos e quem ativa é a tela
   `tools/hosts/Invoke-HostsConfig.ps1`, que só ativa os de um módulo com a credencial dele.)
7. Imprime um resumo por módulo: parâmetros inseridos, CNPJs, jobs novos, jobs da empresa, jobs ativos e avisos. No fim,
   avisa sobre CNPJs de qualquer `*Parameters` que não estão em `monitor.HoldingCompanies` (nenhum serviço os lê).

Regras:

- **Idempotente sem mexer nos scripts de credenciais:** o `.ps1` inclui o script do módulo (`:r`) numa transação e, no
  fim, descarta as linhas novas de empresas que já tinham parâmetros no módulo (a regra é uma linha por empresa em cada
  `*Parameters`). Vale também para os scripts fora do git, que ninguém precisa reescrever. A procedure não duplica job.
- **Sem tráfego real por acidente:** os scripts têm credenciais de produção. Por isso os jobs nascem inativos e o resumo
  lembra de apontar `<Módulo>__BaseUrl` para `http://127.0.0.1:9/` antes de subir um host contra o banco de teste.
- Usa o `sqlcmd` que já está instalado. Sem dependência do módulo `SqlServer` do PowerShell.
- Também serve para a primeira carga da NEWBLOOMERS num ambiente novo, até a tela existir: fora do banco de teste exige
  `-Force`.

## 4. Fases

### 4.1 Banco (Shared) — feito

- [x] `Script001`: `Company VARCHAR(20) NOT NULL` em `Jobs`, `Logs`, `Messages` e `Records`, na PK das quatro e nas FKs
      (`Logs` → `Jobs`; `Messages` e `Records` → `Logs`).
- [x] `Script001`: `auditing.JobCatalog` (PK `(IdJob, IdType)`, os mesmos `CHECK` de `Jobs`), com FK para `Types` e
      `FK_Jobs_JobCatalog`: todo job de uma empresa vem do catálogo.
- [x] `Script008`: `monitor.Company` (PK `Company`, só dígitos), `FK_Jobs_Company` (todo job é de uma empresa
      cadastrada), `monitor.AppImage` e `monitor.AppSetting` com `Company`.
- [x] Scripts `Insert_*_Jobs` de todos os módulos e o Script007 do Shared inserem em `auditing.JobCatalog`.
- [x] `Script002`: `auditing.sp_Add_Company_App @company, @idParent` copia os jobs do módulo do catálogo para
      `auditing.Jobs`, inativos e sem duplicar, e devolve `JobsCreated`; recusa empresa não cadastrada (erro 50002).
      `auditing.sp_Remove_Company_App` desativa, sem apagar.
- [x] `Script008`: `auditing.sp_Add_Company @company, @name, @tradeName` insere em `monitor.Company` (se não existir) e
      chama `sp_Add_Company_App @company, 140`. Toda empresa nasce com o seu 14000.
- [x] Scripts de parâmetros movidos para `Seeds/SQLServer`; `.gitignore` atualizado (2.1).
- [x] Teste no Docker, em banco zerado e apagado no fim: migrators do Shared, Totvs, LinxMicrovix, LinxCommerce, Movidesk,
      AfterSale e DataWarehouse sem erro; 961 jobs no catálogo; `auditing.Jobs`, `monitor.Company` e todas as
      `*Parameters` vazias. Procedures: `sp_Add_Company` cria o 14000 da empresa; `sp_Add_Company_App` da Totvs cria 122
      jobs inativos e, rodada de novo, 0; `sp_Remove_Company_App` desativa; empresa não cadastrada dá 50002.

**Corrigido em 4.3:** `sp_Add_Company_App` copiava só os jobs com `IdParent` = módulo e deixava de fora as procedures
(`IdType = 1`), cujo `IdParent` é o `IdJob` do job que as chama (ex.: `3500 sp_MovideskTickets_Sync`, `IdParent 3000`).
Agora a cópia usa `auditing.fn_JobCatalogTree(@idParent)` (Script002), que devolve o módulo com as procedures abaixo
dele; `sp_Remove_Company_App` e o `.ps1` usam a mesma função. Com isso, a holding recebe os 961 jobs do catálogo.

### 4.2 Script de teste (`tools/seeds/Import-TestSeeds.ps1`) — feito

- [x] Script da seção 3.
- [x] Idempotência pelo próprio `.ps1` (seção 3, regras), sem reescrever os scripts de parâmetros.
- [x] Teste no Docker, em banco zerado e apagado no fim: migrators e depois o `.ps1` → a holding com 464 jobs inativos
      (1 dos hosts, 122 Totvs, 284 LinxMicrovix, 12 LinxCommerce, 16 Movidesk, 24 AfterSale e 5 DataWarehouse) e os
      parâmetros de Totvs, Movidesk e AfterSale; LinxMicrovix e LinxCommerce avisados (scripts fora do git).
      `-Modules Totvs,DataWarehouse -ActivateJobs` ativou 127 jobs. Uma terceira execução completa não inseriu nada e não
      duplicou nenhuma linha. `-Database Company` foi recusado.
- [x] Documentado em `docs/MIGRATOR-CSX.md` (ordem: migrators, depois o `.ps1`).

**Achado do teste:** os parâmetros do Movidesk (`38367316000199`) e do AfterSale (`38367316000199` e `42538267000268`)
estão com o CNPJ das **marcas** (Misha e Open Era), não com o da holding (`44065634000106`), que só aparece na Totvs.
Resolvido em 4.2.1 sem mexer nos parâmetros (decisões 10 e 11).

### 4.2.1 De/para holding → CNPJs (`monitor.HoldingCompanies`) — feito

- [x] `Script008`: `monitor.HoldingCompanies (Company PK, Holding FK → monitor.Company, Name, CreatedAt)`, só
      dígitos, índice por `Holding`. Um CNPJ é de uma holding só. Sem FK nas `*Parameters`: a ligação é por join.
- [x] `Script008`: `auditing.sp_Add_Holding_Company @company, @docCompany, @name` liga um CNPJ de parâmetros à
      empresa. Rodar de novo não faz nada. Recusa empresa não cadastrada (50002), CNPJ de outra holding (50003) e CNPJ
      que é outra empresa do painel (50004).
- [x] `sp_Add_Company` recusa, antes da transação, um CNPJ que já é marca de outra holding (50003).
- [x] A holding **não** entra sozinha: só quando tem parâmetros com o próprio CNPJ (decisão 11). A tela chama
      `sp_Add_Holding_Company` ao gravar os parâmetros de um módulo; o `.ps1` faz o mesmo com os CNPJs dos seeds.
- [x] Teste no Docker, em banco zerado e apagado no fim: o `.ps1` ligou `44065634000106` (Totvs), `38367316000199`
      (Movidesk e AfterSale) e `42538267000268` (AfterSale) à NEWBLOOMERS; a segunda execução não duplicou. O join do
      AfterSale pela holding devolve as duas contas. Cliente de um CNPJ só (`X → X`) funciona. Misha como empresa do
      painel, outro cliente pegando a Misha e outro cliente pegando a NEWBLOOMERS foram recusados sem deixar linha. Um
      CNPJ de parâmetro sem holding gera aviso no `.ps1`.

Nos services (4.4), os parâmetros de uma execução saem por:

```sql
SELECT p.*
FROM <schema>.<Módulo>Parameters p
JOIN monitor.HoldingCompanies hc ON hc.Company = p.doc_company
WHERE hc.Holding = @company
```

Uma execução (job, empresa) cobre todos os CNPJs da holding naquele módulo, como o AfterSale com as duas marcas.

### 4.3 Log por empresa (fase F3 da proposta) — feito

- [x] `sp_Log_Start` recebe `@company` (último parâmetro) e localiza o job por `Title`, `IdType` e `Company`. Sem
      empresa, recusa com 50005; job que a empresa não tem, 50001. `sp_Log_Message`, `sp_Log_Error` e `sp_Log_End`
      copiam `Company` de `Logs`.
- [x] `dbo.sp_Create_Merge_Procedure` 0.9.2 (Script003): a procedure `sp_*_Sync` gerada recebe `@Company` e passa para
      `sp_Log_Start`. As de manutenção do Script004 também recebem `@Company`.
- [x] Empresa da execução: `ICompanyContext` (Application.Shared), um por escopo. Quem inicia a execução chama
      `SetCompany` (ou `AddLog(job, company)`); trocar de empresa no mesmo escopo é recusado.
- [x] `LoggerService.AddLog(job)` lê a empresa do contexto e recusa sem ela; `Log`, `Message` e `Record` têm `Company`
      (o `Log` não nasce sem ela); `LoggerRepository` grava `Company` em `Logs`, `Messages` e `Records`.
- [x] `SharedRepository.CallDbProcMerge` passa `@Company` do contexto para a `sp_*_Sync`: os 437 `AddLog` e as chamadas
      de sync dos services não mudaram.
- [x] `auditing.vw_JobsHeartbeat` por (job, empresa); `JobSchedule` e `JobHeartbeat` com `Company` nas consultas do
      `JobScheduleRepository` (o filtro por empresa fica para 4.4).
- [x] Teste no Docker, em banco zerado e apagado no fim: uma execução do 14000 pelo `LoggerService` gravou a empresa em
      `Logs`, `Messages` (informação e erro) e `Records`, e a `sp_MovideskTickets_Sync` chamada dela gravou o log filho
      com a mesma empresa; o heartbeat mostra a execução na empresa. Sem empresa: `AddLog` e `sp_Log_Start` (50005)
      recusam; empresa sem o job: 50001. `sp_Log_Message` e `sp_Log_Error` gravam a empresa.

A empresa da execução passou a ser informada pelo agendador e pelas rotas em 4.4.

### 4.4 Agendador e services (fase F4 da proposta) — feito

- [x] `JobScheduleRepository`: a agenda tem uma linha por (job, empresa), só das empresas ativas em `monitor.Company`;
      `GetIntegrationJobByCompany(idJob)` para o WebJob; o heartbeat é lido por empresa.
- [x] `ScheduledJobRunner.Run(idJob, company)`: informa a empresa ao `ICompanyContext` do escopo antes do service.
- [x] WebApi (Hangfire): um recurring job por (job, empresa), `<IdJob>-<Title> - V<versão> - <empresa>`. Duas travas: a
      da empresa (execução anterior da mesma empresa rodando: pula, como antes) e a do job, que vale entre empresas
      porque todas escrevem nas mesmas tabelas (`untreated` e a `sp_*_Sync`, com `sp_TryApplock`). Com a do job ocupada
      por outra empresa, a execução volta para a fila em 1 minuto, em vez de ser perdida.
- [x] WebJob: o processo do job atende todas as empresas ativas, uma de cada vez, cada uma com a sua agenda e o seu
      escopo; a mesma dupla de travas, para não rodar junto com o Hangfire. O 14000 de cada empresa lê o heartbeat só
      dela; o WebJob reiniciado é o do job, que atende todas.
- [x] Rotas do WebApi (`CompanyContextFilter`, global): a empresa vem do cabeçalho `X-Company` ou, sem ele, da holding do
      `{docCompany}` da rota em `monitor.HoldingCompanies`. CNPJ sem holding, `X-Company` que não é a holding do
      `docCompany` ou `X-Company` com algo além de dígitos dão 400. Sem nenhum dos dois, a rota segue sem empresa e o
      log recusa.
- [x] Services: `CreateGetCompanysQuery(company)` de Totvs, LinxMicrovix, LinxCommerce, Movidesk e AfterSale lê os
      parâmetros pelo join com `monitor.HoldingCompanies` (4.2.1); os repositórios pegam a empresa do `ICompanyContext`.
      Os services não mudaram: continuam percorrendo `GetCompanys()`, agora só os CNPJs da holding. As `sp_*_Sync`
      recebem a empresa pelo `SharedRepository` (4.3).
- [x] Teste no Docker, em banco zerado e apagado no fim, com `<Módulo>__BaseUrl` em `http://127.0.0.1:9/`: duas
      empresas com o 3000 (Movidesk) ativo a cada minuto, a NEWBLOOMERS com parâmetros (Open Era) e a X sem.
      - WebJob `--job 3000` por 3 minutos: a cada minuto, uma execução por empresa, cada uma com o seu log; a da X
        terminou sem chamar a API ("0 registro(s)"), a da NEWBLOOMERS tentou a API e gravou o erro de conexão.
      - WebApi com Hangfire: a agenda criou `3000-MovideskGetTickets - V1.0.0 - 11111111000111` e `... - 44065634000106`;
        no mesmo minuto, uma rodou e a outra foi adiada ("Rescheduled: job:3000 running for another company") e rodou
        depois. Rotas: `Tickets/38367316000199/1` rodou na NEWBLOOMERS; com `X-Company` de outra holding, com CNPJ sem
        holding e com `X-Company` inválido, 400.

- [x] **Execução que falha depois de gravar o log** (achado no teste, já acontecia antes): o `ExceptionHandler` das rotas
      gravava o log de novo e a chave de `auditing.Logs` recusava (500 sem a mensagem do erro); o `ScheduledJobRunner`
      descartava o erro. Agora `LoggerService.CommitAllChanges`, chamado de novo, grava só as mensagens e registros novos
      na mesma execução (`ILoggerRepository.LogAppend`); `HasUncommittedLog` saiu. Testado no Docker: a rota
      `Tickets/38367316000199/1` devolve o 500 tratado, com uma linha em `Logs` e o erro em `Messages`.

Fora desta fase:

- **Execução adiada e o disparo seguinte:** uma execução adiada pode rodar logo antes do disparo seguinte da mesma
  empresa; as duas rodam em sequência (a trava da empresa impede rodarem juntas).
- **Jobs só de procedure** (`RecurringJobDefinition.ForProcedure`, todos comentados) não passam `@Company`: ao
  descomentar um, a procedure precisa receber a empresa.
- **Oto** e o host **Quartz** (modelo vazio) ficaram fora.

### 4.5 Repositório do WebApp (Dapper, sem API e sem Shared) — feito no mock; Dapper sem teste no banco

Feita na branch `implementacao-40-melhorias`, depois do merge desta branch (commit `f051827`).

- [x] `ICompanyRepository` (`src/Frontend/WebApp/Data/ICompanyRepository.cs`): listar, criar (`sp_Add_Company`), editar
      razão social e nome fantasia e ativar ou desativar empresas em `monitor.Company`. O CNPJ não muda: é a chave dos jobs
      e dos logs. Aceita o CNPJ formatado e grava só os dígitos (11 ou 14).
- [x] `IModuleParametersRepository`: lê e grava a linha de `*Parameters` do módulo por CNPJ (uma linha por CNPJ). A
      definição de campos fica em `Data/ModuleDefinitions.cs` (Movidesk, AfterSale, LinxCommerce, LinxMicrovix e Totvs;
      DataWarehouse não tem parâmetros), com rótulo, obrigatório, segredo, tipo (texto, `https://` ou GUID) e tamanho.
      Ao gravar, numa transação: valida, chama `auditing.sp_Add_Holding_Company` e faz o INSERT ou o UPDATE. Segredos
      nunca voltam (a leitura diz só se estão definidos) e segredo vazio mantém o gravado. Tabela e colunas do SQL só vêm
      da definição; um módulo sem migrator rodado (tabela ausente) devolve lista vazia.
- [x] CNPJs da empresa: `GetHoldingCompaniesAsync`, de `monitor.HoldingCompanies`, com os módulos em que cada CNPJ tem
      parâmetros.
- [x] Contratar e remover integração: `AddAppAsync` (`sp_Add_Company_App`, devolve os jobs criados) e `RemoveAppAsync`
      (`sp_Remove_Company_App`). `GetAppsAsync` lista o catálogo (`auditing.Apps`) com os jobs da empresa em cada módulo,
      contando a árvore de `auditing.fn_JobCatalogTree`.
- [x] Mock primeiro (`Data/Mocks/MockCompanyRepository.cs`), com as mesmas regras dos erros 50002, 50003 e 50004 das
      procedures, testado com um programa à parte (21 casos). O Dapper (`Data/Sql/`) compila, mas **ainda não rodou contra
      o banco**: o teste é no banco do Docker, com `tools/seeds/Import-TestSeeds.ps1`.
- Os erros de regra chegam à tela como `CompanyRegistrationException`, com o número da procedure em `Code`.
- Configuração: mock por padrão; `Database:Enabled = true` (User Secrets), a mesma chave da fase 5 do WebApp, liga o
  Dapper sobre `ConnectionStrings:Connection`.
- **Ponto em aberto:** contratar cria os jobs inativos e remover só os desativa, então o banco não distingue "contratado e
  ainda não ligado" de "removido". O repositório devolve jobs e jobs ativos; se a tela precisar da diferença, falta uma
  marca de contratação (por exemplo `monitor.CompanyApp`), a decidir no backend.

### 4.6 Tela de cadastro (WebApp)

Mudança de layout só com mockup aprovado antes.

- [ ] Mockup: lista de empresas; formulário da empresa (CNPJ, razão social, nome fantasia, ativa); aba de integrações com
      os módulos do catálogo, cada um com as credenciais e parâmetros dele.
- [ ] Segredos: nunca voltam para a tela depois de gravados. O campo mostra "definido" e aceita só substituir.
- [ ] Ao contratar uma integração, os jobs nascem inativos. A ativação continua na tela de Integrações e na Visão geral.
- [ ] Validação de CNPJ e de duplicidade.
- [ ] Botão **Testar conexão** por módulo (ver 4.6.1).

#### 4.6.1 Testar conexão

O WebApp não usa a API do host nem o Shared ([decisões do WebApp](../src/Frontend/WebApp/docs/PLANO-WEBAPP.md)), então o
teste fica no próprio WebApp:

- Uma interface `IConnectionTester` por módulo, registrada junto da definição de campos do módulo (4.5). Recebe os
  valores **do formulário**, ainda não gravados, e devolve sucesso ou a mensagem de erro do provedor.
- O teste faz só a chamada mínima de autenticação, **sempre de leitura**: o pedido de token (Totvs, AfterSale), ou um
  GET pequeno quando o módulo não tem token (Movidesk, LinxMicrovix, LinxCommerce). Nunca escreve no provedor, inclusive
  na Totvs de produção.
- Tempo limite curto (10 s). Nenhum segredo vai para o log nem para a mensagem de erro mostrada.
- A lógica de autenticação fica duplicada entre o WebApp e o módulo. Ela é pequena; se crescer, vira uma fase própria
  para extrair um pacote comum ou expor o teste pela API.
- LinxCommerce não tem credenciais de teste: o testador dele é verificado só contra uma API simulada local.
- Testes: a mesma regra dos outros testes, sem tráfego real em automação. O teste real é manual, pelo botão.

### 4.7 Autenticação (fase F6 da proposta)

- [ ] Só um administrador acessa o cadastro. A empresa da sessão é validada no servidor.

## 5. Riscos

- **Credenciais em texto puro:** as tabelas `*Parameters` guardam segredos sem criptografia, por decisão desta fase. Com
  vários clientes, o acesso ao banco passa a ser o único controle; a criptografia fica para uma fase futura.
- **Jobs dos hosts por empresa:** com N empresas há N jobs 14000. Cada um lê o heartbeat só da sua empresa, então a
  recuperação de um WebJob travado acontece pelo job da empresa dona da execução.
- **Credenciais de produção no `.ps1`:** os scripts de `Seeds/` têm credenciais reais. Jobs inativos por padrão e a
  trava de nome do banco reduzem o risco de tráfego real ou escrita em produção a partir de um teste.
- **Ordem de deploy:** num ambiente novo, o sistema sobe sem nenhuma empresa. Nada roda até alguém cadastrar uma pela
  tela ou pelo `.ps1`.

## 6. Perguntas respondidas em 01/11/2026

| Pergunta | Resposta |
|---|---|
| O catálogo do produto (`Types`, `Apps`, catálogo de jobs) fica no DbUp? | Sim (decisão 5) |
| `monitor.*` passa a usar `Company`? | Sim, já aplicado no `Script008` (decisão 6) |
| De quem são os jobs dos hosts (14000)? | De todas as empresas: cada uma tem o seu, criado por `sp_Add_Company` (decisão 7) |
| Criptografar as credenciais? | Não nesta fase: texto puro (decisão 8) |
| Botão para testar a conexão do módulo? | Sim, no próprio WebApp (decisão 9, passo 4.6.1) |
| Parâmetros com CNPJ das marcas: trocar para a holding? | Não (02/11/2026): de/para em `monitor.HoldingCompanies` (decisões 10 e 11, passo 4.2.1) |
