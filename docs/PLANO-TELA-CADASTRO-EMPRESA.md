# Plano: cadastro de empresas e integrações pela tela (fora do DbUp)

Branch: `implementacao-tela-cadastro`. Status: **plano**; só o passo 1.1 (chaves de `auditing`) já está no código.

Base: [PROPOSTA-DOC-COMPANY-AUDITING.md](../src/Backend/Shared/docs/PROPOSTA-DOC-COMPANY-AUDITING.md). Este plano
substitui a fase F2 (seed por procedure a partir das tabelas `*Parameters`) daquela proposta e muda o nome da coluna.

## 1. Decisões de 01/11/2026

1. O sistema ETL não atende só a NEWBLOOMERS: vai atender outros clientes. **Dados de cliente não ficam no DbUp.** A
   empresa, as integrações que ela usa e as credenciais de cada integração são cadastradas numa tela própria do WebApp.
2. A coluna de empresa no schema `auditing` se chama **`Company`** (não `DocCompany`).
3. `Company` faz parte da **PK de todas as tabelas de auditing** com empresa: `Jobs`, `Logs`, `Messages` e `Records`.
4. Para testes, os scripts de inserção de parâmetros que existem hoje não se perdem: um script `.ps1` aplica todos de uma
   vez num banco de teste, sem cadastrar empresa por empresa na tela (passo 3).

Continuam valendo as decisões de 31/10/2026 da proposta: ativação é `UPDATE` em `auditing.Jobs.IsActive` por (job,
empresa), sem tabela de ativação; uma execução do agendador por (job, empresa); a empresa do painel é a holding; banco
zerado, sem backfill.

## 2. O que fica no DbUp e o que sai

O DbUp passa a criar **estrutura e catálogo do produto**. Tudo o que depende de um cliente sai.

| Fica no DbUp (produto) | Sai do DbUp (cliente) |
|---|---|
| schemas, tabelas, procedures, views, tipos | linhas de `*Parameters` (credenciais e endereços de cada cliente) |
| `auditing.Types` (Script005 do Shared) | linhas de `auditing.Jobs` (agora uma por empresa) |
| `auditing.Apps`: catálogo de módulos (ver pergunta 1) | |
| catálogo de jobs, sem empresa (ver 4.1 e pergunta 1) | |
| tabelas de referência do DataWarehouse (`Script006_Insert_DataWarehouse_Reference.sql`) | |

### 2.1 Scripts que saem do DbUp

| Módulo | Parâmetros (credenciais) | Jobs |
|---|---|---|
| Shared | — | `Script007_Insert_Hosts_Jobs_Into_Jobs_Table.sql` (job 14000 dos hosts, ver pergunta 3) |
| Totvs | `Parameters/Script003_Insert_Values_Into_Totvs_Parameters_Table.sql` | 12 scripts `Insert_Totvs_*_Jobs` |
| LinxMicrovix | `Parameters/Script003_Insert_Values_Into_LinxMicrovix_Parameters_Table.sql` (fora do git; há `.example`) | 29 scripts (`Erp/*` e `B2C/*`) |
| LinxCommerce | `Parameters/Script003_Insert_Values_Into_LinxCommerce_Parameters_Table.sql` (fora do git; há `.example`) | 4 scripts |
| Movidesk | `Parameters/Script003_Insert_Values_Into_Movidesk_Parameters_Table.sql` | 3 scripts |
| AfterSale | `Parameters/Script005_Insert_Values_Into_AfterSale_Parameters_Table.sql` | 4 scripts |
| DataWarehouse | — | `Parameters/Script007_Insert_DataWarehouse_Jobs_Into_Jobs_Table.sql` |
| Dootax, Mobsim, Oto, Pagarme, FlashCourier, Jadlog, Stone, TotalExpress | — | `Script002_Insert_*_Values_Into_Jobs_Table.sql` (hoje só comentários) |

**Para onde vão:** os migrators leem `Scripts/SQLServer` com subpastas (`IncludeSubDirectories = true`), então mover para
uma subpasta não basta. Os scripts vão para uma pasta irmã, fora do alcance do DbUp:

```
Infrastructure.<Módulo>/
  Scripts/SQLServer/   ← DbUp (estrutura e catálogo)
  Seeds/SQLServer/     ← só o .ps1 de teste lê (parâmetros e jobs de uma empresa)
```

O `.gitignore` acompanha a mudança de caminho dos dois scripts de credenciais que já ficam fora do git (LinxMicrovix e
LinxCommerce). Os que estão no git por decisão do usuário (Totvs, Movidesk, AfterSale) continuam no git, só mudam de pasta.

## 3. Script `.ps1` de dados de teste

**Objetivo:** subir um banco de teste completo (empresa, parâmetros e jobs de todos os módulos) com um comando, usando os
scripts de `Seeds/` (passo 2.1), sem passar pela tela de cadastro.

Arquivo proposto: `tools/seeds/Import-TestSeeds.ps1`.

```powershell
./tools/seeds/Import-TestSeeds.ps1 -Server 'localhost,1433' -Database 'EtlTeste' -Company '44065634000106'
```

| Parâmetro | Padrão | Uso |
|---|---|---|
| `-Server` | `localhost,1433` | instância do container `sql-server` do Docker |
| `-Database` | obrigatório | banco de teste; **recusa** `Company` e qualquer nome sem `Test`/`Teste`, salvo com `-Force` |
| `-Credential` | pede com `Get-Credential` | usuário e senha do SQL; a senha nunca vai para a linha de comando nem para log |
| `-Company` | `44065634000106` | empresa dos jobs (holding NEWBLOOMERS) |
| `-Modules` | todos | filtro, ex.: `-Modules Totvs,Movidesk` |
| `-ActivateJobs` | desligado | sem ele os jobs nascem com `IsActive = 0` |

O que ele faz, em ordem:

1. Confere que o banco existe e que os migrators já rodaram (`auditing.Jobs` e as tabelas `*Parameters` existem). Não roda
   migrator: isso continua sendo o `migrator.csx`.
2. Cadastra a empresa (`monitor.Company`, passo 4.1), se ainda não existir.
3. Para cada módulo, executa o script de parâmetros de `Seeds/SQLServer` com `sqlcmd -b`. Script fora do git que não
   existir na máquina (LinxMicrovix, LinxCommerce) gera um aviso e o módulo é pulado, sem parar o resto.
4. Para cada módulo, chama `auditing.sp_Add_Company_App @company, @idParent` (passo 4.1), que copia o catálogo de jobs
   do módulo para `auditing.Jobs` com `Company = @company`.
5. Com `-ActivateJobs`, faz o `UPDATE auditing.Jobs SET IsActive = 1` só da empresa e dos módulos pedidos.
6. Imprime um resumo por módulo: parâmetros inseridos, jobs criados, jobs ativos, avisos.

Regras:

- **Idempotente:** os scripts de parâmetros passam a usar `IF NOT EXISTS (... WHERE doc_company = ...)`, e a procedure
  não duplica job. Rodar duas vezes não muda nada.
- **Sem tráfego real por acidente:** os scripts têm credenciais de produção. Por isso os jobs nascem inativos e o resumo
  lembra de apontar `<Módulo>__BaseUrl` para `http://127.0.0.1:9/` antes de subir um host contra o banco de teste.
- Usa o `sqlcmd` que já está instalado (ODBC 17). Sem dependência do módulo `SqlServer` do PowerShell.
- Também serve para a primeira carga da NEWBLOOMERS num ambiente novo, até a tela existir. Fora do banco de teste exige
  `-Force`.

## 4. Fases

### 4.1 Banco (Shared)

- [x] `Script001`: `Company VARCHAR(20) NOT NULL` em `Jobs`, `Logs`, `Messages` e `Records`, na PK das quatro e nas FKs
      (`Logs` → `Jobs`; `Messages` e `Records` → `Logs`).
- [x] `Script008_Create_Monitor_Schema.sql`: `monitor.AppImage` e `monitor.AppSetting` (hoje com `DocCompany`, ver
      pergunta 2).
- [ ] `monitor.Company`: `Company VARCHAR(20)` (PK, CNPJ), `Name`, `TradeName`, `IsActive`, `CreatedAt`, `UpdatedAt`.
      É a lista de empresas do seletor e do login. Não guarda credencial.
- [ ] Catálogo de jobs: `auditing.JobCatalog` com as colunas de `auditing.Jobs` sem `Company` e sem `IsActive`. Os
      scripts `Insert_*_Jobs` atuais viram inserts nesse catálogo e **ficam no DbUp** (são produto, ligados ao
      `EnumJob` do código), se a pergunta 1 for aceita.
- [ ] `auditing.sp_Add_Company_App @company, @idParent`: copia os jobs do catálogo do módulo para `auditing.Jobs`, com
      `IsActive = 0`, sem duplicar. `auditing.sp_Remove_Company_App` desativa (não apaga, por causa das FKs de `Logs`).
- [ ] Mover os scripts da seção 2.1 para `Seeds/SQLServer` e atualizar o `.gitignore`.
- [ ] Teste: banco zerado no Docker, migrators de todos os módulos sem erro, `auditing.Jobs` vazio.

### 4.2 Script de teste (`tools/seeds/Import-TestSeeds.ps1`)

- [ ] Script da seção 3.
- [ ] Scripts de parâmetros idempotentes.
- [ ] Teste: banco zerado + migrators + `.ps1` → a holding com os parâmetros de todos os módulos e o catálogo completo em
      `auditing.Jobs`, todos inativos; segunda execução sem mudança.
- [ ] Documentar em `docs/MIGRATOR-CSX.md` (ordem: migrators, depois o `.ps1`).

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

### 4.7 Autenticação (fase F6 da proposta)

- [ ] Só um administrador acessa o cadastro. A empresa da sessão é validada no servidor.

## 5. Riscos

- **Credenciais em texto puro:** hoje as tabelas `*Parameters` guardam segredos sem criptografia. Com vários clientes,
  isso pesa mais (ver pergunta 4).
- **Credenciais de produção no `.ps1`:** os scripts de `Seeds/` têm credenciais reais. Jobs inativos por padrão e a
  trava de nome do banco reduzem o risco de tráfego real ou escrita em produção a partir de um teste.
- **Ordem de deploy:** num ambiente novo, o sistema sobe sem nenhuma empresa. Nada roda até alguém cadastrar uma pela
  tela ou pelo `.ps1`.

## 6. Perguntas em aberto

1. **Catálogo no DbUp:** `auditing.Apps`, `auditing.Types` e o catálogo de jobs (`auditing.JobCatalog`) descrevem o
   produto, não o cliente, e dependem do código (`EnumJob`). Eles ficam no DbUp? (Recomendado: sim. Sem isso, a tela
   não sabe quais jobs um módulo tem.)
2. **Nome no `monitor`:** `monitor.AppImage` e `monitor.AppSetting` também passam a usar `Company` em vez de
   `DocCompany`? (Recomendado: sim, um nome só no banco.)
3. **Jobs dos hosts (14000, `StuckJobsRecovery`):** não pertencem a um cliente. Hoje o Script007 usa `Company = ' '`.
   Ficam no DbUp com um valor fixo de sistema (por exemplo `'*'`, como a imagem padrão), ou viram configuração do host,
   fora de `auditing.Jobs`?
4. **Criptografia das credenciais:** manter texto puro nesta fase, ou criptografar já no cadastro (Always Encrypted,
   chave no Key Vault)?
5. **Testar conexão:** a tela deve ter um botão para testar a credencial do módulo? Isso exige chamar o módulo, e o
   WebApp hoje não usa API nem Shared.
