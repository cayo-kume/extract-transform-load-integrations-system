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
./tools/seeds/Import-TestSeeds.ps1 -Database 'EtlTeste' -Modules Totvs,DataWarehouse -ActivateJobs
```

| Parâmetro | Padrão | Uso |
|---|---|---|
| `-Database` | obrigatório | banco de teste já migrado; **recusa** `Company` e qualquer nome sem `Test`/`Teste`, salvo com `-Force` |
| `-Server` | `localhost,1433` | instância do container `sql-server` do Docker |
| `-Credential` | pede com `Get-Credential` | usuário e senha do SQL; a senha vai para o `sqlcmd` pela variável `SQLCMDPASSWORD`, nunca pela linha de comando |
| `-Company` | `44065634000106` | empresa dos jobs (holding NEWBLOOMERS), só dígitos |
| `-CompanyName` | `NEWBLOOMERS` | razão social gravada em `monitor.Company` quando a empresa ainda não existe |
| `-Modules` | todos | `Totvs`, `LinxMicrovix`, `LinxCommerce`, `Movidesk`, `AfterSale`, `DataWarehouse` (só jobs) |
| `-ActivateJobs` | desligado | ativa os jobs da empresa nos módulos pedidos; sem ele, nascem inativos |
| `-Force` | desligado | aceita um banco fora da regra de nome |

O que ele faz, em ordem:

1. Confere que os migrators já rodaram (`auditing.JobCatalog` com jobs e `monitor.Company`). Não roda migrator: isso
   continua sendo o `migrator.csx`.
2. Cadastra a empresa com `auditing.sp_Add_Company` (passo 4.1), se ainda não existir. Isso já cria os jobs dos hosts.
3. Para cada módulo, roda o script `*_Insert_Values_Into_*_Parameters_Table.sql` de `Seeds/SQLServer` com `sqlcmd -b`,
   numa transação. Script fora do git que não existir na máquina (LinxMicrovix, LinxCommerce) gera um aviso e o módulo
   fica sem parâmetros, sem parar o resto; se o script ainda estiver no caminho antigo (`Scripts/SQLServer/Parameters`,
   que o DbUp executa), o aviso pede para movê-lo.
4. Para cada módulo, chama `auditing.sp_Add_Company_App @company, @idParent`, que copia o catálogo de jobs do módulo para
   `auditing.Jobs` com `Company = @company`.
5. Com `-ActivateJobs`, faz o `UPDATE auditing.Jobs SET IsActive = 1` só da empresa e dos módulos pedidos.
6. Imprime um resumo por módulo: parâmetros inseridos, jobs novos, jobs da empresa, jobs ativos e avisos.

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

Ficam para as próximas fases: as procedures de log (`sp_Log_*`) e a `vw_JobsHeartbeat` ainda não conhecem `Company`
(4.3), e os agendadores ainda leem `auditing.Jobs` sem empresa (4.4). Até lá, um job que roda em um banco com empresa
cadastrada falha ao gravar o log.

### 4.2 Script de teste (`tools/seeds/Import-TestSeeds.ps1`) — feito

- [x] Script da seção 3.
- [x] Idempotência pelo próprio `.ps1` (seção 3, regras), sem reescrever os scripts de parâmetros.
- [x] Teste no Docker, em banco zerado e apagado no fim: migrators e depois o `.ps1` → a holding com 464 jobs inativos
      (1 dos hosts, 122 Totvs, 284 LinxMicrovix, 12 LinxCommerce, 16 Movidesk, 24 AfterSale e 5 DataWarehouse) e os
      parâmetros de Totvs, Movidesk e AfterSale; LinxMicrovix e LinxCommerce avisados (scripts fora do git).
      `-Modules Totvs,DataWarehouse -ActivateJobs` ativou 127 jobs. Uma terceira execução completa não inseriu nada e não
      duplicou nenhuma linha. `-Database Company` foi recusado.
- [x] Documentado em `docs/MIGRATOR-CSX.md` (ordem: migrators, depois o `.ps1`).

**Achado do teste e decisão de 01/11/2026:** os parâmetros do Movidesk (`38367316000199`) e do AfterSale
(`38367316000199` e `42538267000268`) estavam com o CNPJ das **marcas** (Misha e Open Era), não com o da holding
(`44065634000106`), que só aparecia na Totvs. Decidido: o `doc_company` dos parâmetros é o da **holding**.

- **Movidesk:** feito. O seed tem uma linha só e passou a usar a holding.
- **AfterSale:** bloqueado. São **duas contas** (Open Era e Misha), cada uma com o seu token, e o módulo usa o
  `doc_company` como chave em todas as tabelas (PKs e FKs compostas). Trocar as duas linhas para a holding deixaria duas
  linhas da mesma empresa em `AfterSaleParameters` (o `.ps1` descartaria a segunda) e misturaria os dados das duas contas
  na mesma chave. Precisa de decisão: uma coluna nova de conta (marca) em `AfterSaleParameters` e nas chaves do módulo,
  com a holding no `doc_company`, ou só uma das contas por enquanto.

### 4.3 Log por empresa (fase F3 da proposta)

- [ ] `sp_Log_Start` recebe `@company` e localiza o job por `Title`, `IdType` e `Company`; `sp_Log_Message`,
      `sp_Log_Error` e `sp_Log_End` copiam `Company` de `Logs`.
- [ ] `LoggerRepository`, entidades (`Log`, `Message`, `Record`, `JobSchedule`) e `LoggerService` com `Company`.
- [ ] `auditing.vw_JobsHeartbeat` por (job, empresa).

### 4.4 Agendador e services (fase F4 da proposta)

- [ ] Agenda por (job, empresa) no WebApi (Hangfire), no WebJob e no Worker.
- [ ] Services recebem a empresa e processam só ela; as procedures de sincronização recebem a empresa da execução.

### 4.5 Repositório do WebApp (Dapper, sem API e sem Shared)

- [ ] `ICompanyRepository`: listar, criar, editar e desativar empresas (`monitor.Company`).
- [ ] `IModuleParametersRepository`: ler e gravar a linha de `*Parameters` do módulo para a empresa. Cada módulo tem
      colunas próprias, então a tela usa uma definição por módulo (campo, rótulo, obrigatório, segredo), num único lugar
      do WebApp.
- [ ] Contratar e remover integração: chama `sp_Add_Company_App` e `sp_Remove_Company_App`.
- [ ] Mock primeiro, atrás das mesmas interfaces, como o resto do WebApp.

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
