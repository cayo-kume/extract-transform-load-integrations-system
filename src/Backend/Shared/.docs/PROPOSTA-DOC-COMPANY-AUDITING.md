# Proposta: doc_company nas tabelas de auditing (refatoração em fases)

Status: **proposta, nada foi alterado**. Os schemas de `auditing` só mudam quando a fase correspondente for aprovada.
Decisões de 31/10/2026 que esta proposta segue:

1. Não existe `monitor.JobActivation` nem ativação por módulo em tabela própria. Ativar ou desativar um job para uma
   empresa é um `UPDATE` na coluna `IsActive` de `auditing.Jobs`, na linha daquela empresa. Ativar um módulo é o mesmo
   `UPDATE` em todos os jobs do `IdParent` da empresa.
2. `doc_company` entra na chave de `auditing.Jobs`.
3. A visão de uma empresa mostra só ela. Nenhuma tela ou consulta da empresa selecionada lê logs de outra empresa.
4. Paginação padrão: 50 linhas nas grids de Jobs e Integrações, 5 em Execuções e 25 em Logs de Erros.
5. O agendador roda **uma execução por (job, empresa)**.
6. `monitor.AppImage` usa `DocCompany = '*'` para a imagem padrão do módulo.
7. **Não há logs antigos.** A solução sobe em um banco zerado: não existe backfill nem marcação `LEGACY`.
8. **A empresa do painel é a holding** (seção 2.3), não a marca nem a filial.

---

## 1. Situação atual (confirmada no código)

| Objeto | Hoje | Problema para multi-empresa |
|---|---|---|
| `auditing.Jobs` | PK `(IdJob, IdType)` | um job é único para todas as empresas, então `IsActive` também é global |
| `auditing.Logs` | PK `(IdJob, IdType, Execution)`, FK para `Jobs` | a execução não diz para qual empresa foi |
| `auditing.Messages` / `auditing.Records` | FK para `Logs` por `(IdJob, IdType, Execution)` | herdam a falta de empresa |
| `auditing.Apps` | PK `IdParent` | é catálogo de módulos, não há problema em ser global |
| agendador | lê `IsActive` de `auditing.Jobs` (`JobScheduleRepository`, `SchedulerRefreshJob` no WebApi, `JobScheduleWorker` no WebJob) | a agenda é uma só, sem empresa |
| services dos módulos | cada execução percorre **todas** as empresas das tabelas `*Parameters` | um job não roda para uma empresa só |
| `auditing.sp_Log_Start` | localiza o job por `Title` e `IdType` | com uma linha por empresa, o título deixa de identificar o job |

As tabelas `*Parameters` dos módulos já guardam `doc_company` (por exemplo `AfterSaleParameters`), então a lista de
holdings existe e não precisa de cadastro novo (seção 2.3).

---

## 2. Modelo proposto

### 2.1 Tabelas de auditing

| Tabela | Mudança | Motivo |
|---|---|---|
| `auditing.Jobs` | `+ DocCompany VARCHAR(20) NOT NULL`; PK passa a `(IdJob, IdType, DocCompany)`; `IsActive` continua existindo | ativação por empresa |
| `auditing.Logs` | `+ DocCompany VARCHAR(20) NOT NULL`; PK passa a `(IdJob, IdType, DocCompany, Execution)`; FK para `Jobs` com `DocCompany` | a visão filtra por empresa |
| `auditing.Messages` | `+ DocCompany`; FK para `Logs` com `DocCompany` | isolamento |
| `auditing.Records` | `+ DocCompany`; FK para `Logs` com `DocCompany` | isolamento |
| `auditing.Apps` | sem mudança (catálogo global) | a personalização por empresa fica em `monitor.*` (2.2) |
| `auditing.Types` | sem mudança | |

Índices a rever: todo filtro do painel passa a começar por `DocCompany`, então os índices de `Logs` e `Messages` ganham a
coluna no início.

### 2.2 Personalização por empresa (fora de `auditing`)

Imagens e parâmetros que o painel edita por empresa ficam em um schema novo, `monitor`, criado pelo migrator do Shared:

| Tabela | Colunas | Observação |
|---|---|---|
| `monitor.AppImage` | `IdParent`, `DocCompany`, `ContentType`, `Data VARBINARY(MAX)`, `UpdatedAt` | PK `(IdParent, DocCompany)`; a imagem padrão do módulo usa `DocCompany = '*'` |
| `monitor.AppSetting` | `IdParent`, `DocCompany`, `Name`, `Value`, `IsSecret`, `UpdatedAt` | só para o que não existe em nenhuma tabela `*Parameters` (ver pergunta 1) |

Por que um schema separado e não `auditing`: `auditing` é o registro do que rodou; `monitor` é configuração de
apresentação e de acesso. Misturar as duas faz uma mudança de imagem parecer um evento de auditoria.

---

### 2.3 Holding, filiais e marcas

**Decisão:** o `doc_company` do painel é o **CNPJ da holding**. É com ele que o usuário entra e que o seletor de empresa
funciona. Filiais e marcas não viram empresas do painel.

O que o código já faz hoje:

| Módulo | Linha de login | Filiais (de/para com a holding) |
|---|---|---|
| Totvs | `totvs.TotvsParameters`, uma linha: holding NEWBLOOMERS (`44065634000106`), com a regra "uma linha só" documentada no módulo | `totvs.TotvsBranches`, com o CNPJ de cada loja e a holding ao qual ela pertence |
| LinxMicrovix | `linxmicrovix.LinxMicrovixParameters`, uma linha: holding NEWBLOOMERS (`44065634000106`) | `linxmicrovix.LinxGrupoLojas` (lojas da holding) e `linxmicrovix.B2CConsultaEmpresas` |
| AfterSale, Movidesk, LinxCommerce | `*Parameters`; a linha da holding ainda precisa ser conferida (seção 7.2) | não encontrada nesta análise |

Misha e Open Era **não** têm linha de login. São marcas dentro da holding, identificadas pela raiz do CNPJ da filial
emitente (`38367316` Misha, `42538267` Open Era), conforme `DataWarehouse/.docs/regras-prefixo-pedidos.md`. Isso vale para
a série do pedido e para o `DimBrand` do DW, não para a autenticação.

**Recomendação:**
1. Manter a holding como a única empresa de login e de seletor. Não criar `doc_company` para Misha nem Open Era.
2. Não criar um de/para novo entre holding e filiais: a relação já existe em `TotvsBranches` e em `LinxGrupoLojas`.
   O que falta é uma **visão única** dessa relação para o painel, por exemplo `shared.CompanyBranch (DocCompany, BranchCnpj,
   Brand, Module)`, alimentada pelos módulos. Ela serve para filtrar por marca depois, sem mudar a chave de auditing.
3. Verificar antes da F2 se a holding aparece **uma vez** em cada `*Parameters`. Hoje Totvs e LinxMicrovix têm uma linha
   cada; AfterSale, Movidesk e LinxCommerce precisam ser conferidos.

## 3. Alterações na code base, por camada

### 3.1 Domínio e entidades (`Shared/Domain.Shared/Entities/Auditing`)
- `Log`, `Message`, `Record` e `JobSchedule` ganham `DocCompany`.
- `Log` recebe a empresa no construtor, para que nenhuma execução seja gravada sem ela.

### 3.2 Scripts de banco (`Shared/Infrastructure.Shared/Scripts/SQLServer`)
- `Script001_Create_Schemas_Procedures_And_Tables.sql`: as colunas e as chaves da seção 2.1 entram no `CREATE` original,
  como manda o padrão do repositório (`PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md`, 3.3). Um banco que já rodou o script
  recebe as mudanças por um script à parte.
- `Script002_Create_Auditing_Procedures.sql`:
  - `sp_Log_Start` recebe `@docCompany` e localiza o job por `Title`, `IdType` e `DocCompany`;
  - `sp_Log_Message` e `sp_Log_Error` copiam `DocCompany` de `Logs`.
- `Script004_Create_Dbo_Procedures.sql`: as procedures `sp_*_Sync` que chamam `sp_Log_Message` passam a receber a
  empresa da execução.
- Novo `Script00X_Create_Monitor_Schema.sql` para `monitor.AppImage` e `monitor.AppSetting`.

### 3.3 Repositórios e agendador
- `LoggerRepository`: o `INSERT` em `Logs`, `Messages` e `Records` grava `DocCompany`.
- `JobScheduleRepository`: a consulta da agenda passa a devolver `(IdJob, IdType, DocCompany, IsActive, ...)`.
- Agendador (`Hosts/WebApi/Scheduling/SchedulerRefreshJob.cs` e `Hosts/WebJob/Scheduling/JobScheduleWorker.cs`): a unidade
  agendada passa a ser **(job, empresa)**. Cada execução leva a empresa, e a execução de uma empresa não interfere na de
  outra (decidido em 7.1).

### 3.4 Aplicação (`Shared/Application.Shared`)
- `LoggerService`: a empresa vem do contexto da execução (o agendador ou a rota a informam), não de um campo global.
  `AddLog(job)` passa a ser `AddLog(job, docCompany)`.

### 3.5 Módulos (`Modules/*/Application.*`)
- Os services param de percorrer todas as empresas por padrão. Cada um recebe `docCompany` e processa só essa empresa.
  Os módulos que já têm `docCompany` nas rotas (AfterSale, Movidesk, LinxMicrovix, Totvs, LinxCommerce, DataWarehouse)
  só precisam repassar o valor.
- As procedures de sincronização (`IdType = 1`) recebem a empresa da execução que as chama.
- Mudança de semântica: hoje uma execução cobre todas as empresas em uma passada. Depois, cada empresa tem sua execução.
  Isso multiplica o número de execuções pelo número de empresas ativas (ver seção 6).

### 3.6 Seed dos jobs por empresa
- Hoje cada script `Insert_*_Jobs_Into_Jobs_Table.sql` insere uma linha por job. Com a chave nova, o job precisa existir
  uma vez por empresa.
- Proposta: os scripts continuam inserindo uma linha "molde", e uma procedure `auditing.sp_Seed_Jobs_For_Company
  @docCompany` copia os jobs do módulo para a empresa com `IsActive = 0`.
- Empresa nova: quem cadastra uma empresa em `*Parameters` chama a procedure. Sem isso, a empresa não aparece na agenda.
- `IsActive` nasce `0` (já decidido no plano do host): nenhuma empresa começa rodando tudo.

### 3.7 WebApp (`src/Frontend/WebApp`)
- `IMonitorRepository`: todos os métodos recebem `docCompany`, e toda consulta sobre `auditing.*` tem
  `WHERE DocCompany = @docCompany`. Não existe método que atravesse empresas.
- O seletor de empresa alimenta esse parâmetro. Até a autenticação (fase 6 do plano do WebApp), o valor vem do seletor
  e é só de interface: a garantia de isolamento vem do servidor, na fase 6.
- Ativar e desativar job ou módulo fazem `UPDATE auditing.Jobs SET IsActive` na linha da empresa. O mock do WebApp passa a
  guardar `IsActive` por empresa, e o `MonitorRepository` faz o mesmo `UPDATE`.
- `MockCatalog`: cada job do catálogo precisa existir por empresa do mock (NEWBLOOMERS e XPTO).
- `Executar Querys` **não** entra nessa restrição: a tela já consulta qualquer banco, e o controle é o acesso (fase 6).
  Se a visão por empresa for exigida também nessa tela, vira uma fase própria.

---

## 4. Fases de refatoração

| Fase | Entrega | Critério de conclusão |
|---|---|---|
| **F0 — Decisões** | respostas às perguntas da seção 7 | documento aprovado |
| **F1 — Modelo** | `Script001` com as colunas e chaves; `monitor.*` no migrator | o banco zerado sobe sem erro; FKs íntegras |
| **F2 — Seed** | `sp_Seed_Jobs_For_Company`; scripts de jobs passam a usar a procedure; visão `shared.CompanyBranch` (seção 2.3) | cada holding das `*Parameters` tem o catálogo completo, com `IsActive = 0` |
| **F3 — Log** | `sp_Log_Start`, `sp_Log_Message`, `sp_Log_Error`, `LoggerRepository`, entidades e `LoggerService` com `DocCompany` | uma execução de teste grava a empresa em `Logs`, `Messages` e `Records` |
| **F4 — Agendador e services** | agenda por (job, empresa); services recebem `docCompany`; procedures recebem a empresa | um job ativo só para NEWBLOOMERS não roda para XPTO |
| **F5 — WebApp** | `IMonitorRepository` com empresa; seletor ligado; ativação por `UPDATE`; `monitor.AppImage` e `monitor.AppSetting` | a visão da XPTO não contém nenhuma linha da NEWBLOOMERS (teste de isolamento) |
| **F6 — Autenticação** | empresa da sessão validada no servidor; grupos do Entra ID | um usuário sem vínculo com uma empresa não consegue consultá-la, nem mudando o parâmetro na URL ou na chamada |
| **F7 — Limpeza** | remover mocks de ativação e o código que ainda percorre todas as empresas | busca por `*Parameters` sem filtro de empresa devolve só o que for necessário |

Cada fase é um commit ou um conjunto de commits, com o build e os testes do repositório passando ao final.

---

## 5. Banco zerado

A solução sobe em um banco zerado (decisão de 31/10/2026). Consequências:

- Não há migração de dados nem backfill. Não existe marcação `LEGACY`.
- O `Script001` é o único lugar onde o modelo existe: não há script de `ALTER` para um banco que já rodou o script
  antigo. O padrão do repositório (`PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md`, 3.3) continua valendo para qualquer banco
  criado depois de F1.
- O seed de F2 roda no deploy do banco novo, para cada holding encontrada nas tabelas `*Parameters`.
- Os bancos de teste ficam como estão hoje: podem ser recriados do zero, sem risco de perda.

---

## 6. Riscos

- **Multiplicação:** 960 jobs × N empresas. Com 2 empresas, são 1.920 linhas em `auditing.Jobs`. Aceitável; o custo é
  o de execuções.
- **Execuções:** um job diário roda uma vez por empresa ativa. Os 48 jobs diários das 23:00 viram 48 × N execuções no
  mesmo minuto. Se pesar, o horário por empresa (`RunningDailyAt` por linha) resolve, sem código.
- **Regressão:** services que hoje processam todas as empresas em uma passada mudam de comportamento. Os testes de
  integração de cada módulo precisam passar com uma empresa e com duas.
- **Isolamento:** até a fase 6, a separação por empresa é de interface. Não deve haver acesso externo ao painel nessa
  etapa.

---

## 7. Decisões e perguntas

### 7.1 Decidido em 31/10/2026

| Tema | Decisão |
|---|---|
| Ativação | `UPDATE` em `auditing.Jobs.IsActive` por (job, empresa); sem `monitor.JobActivation` |
| Visão | a empresa selecionada não vê logs de outra |
| Agendador | uma execução por (job, empresa) |
| Imagem padrão | `monitor.AppImage` com `DocCompany = '*'` |
| Histórico | não há logs antigos: banco zerado, sem backfill nem `LEGACY` |
| Empresa do painel | a holding (seção 2.3) |
| Paginação | 50 em Jobs e Integrações, 5 em Execuções, 25 em Logs de Erros |

### 7.2 Em aberto

1. **Chaves de API e parâmetros (seção 2.2):** a fonte única é a tabela `*Parameters` de cada módulo (recomendado), ou o
   painel guarda cópia em `monitor.AppSetting`? Com cópia, há duas fontes de verdade para a mesma chave.
2. **Holding nas tabelas de parâmetros (seção 2.3):** a holding deve aparecer em toda linha de `*Parameters`, inclusive
   nos módulos que hoje não a têm (conferir AfterSale, Movidesk e LinxCommerce)? Isso responde à sua primeira pergunta:
   a holding é a chave de login, e as filiais saem do de/para que já existe.
3. **Visão `shared.CompanyBranch` (seção 2.3):** criamos a visão única de holding → filial → marca na F2, ou só quando o
   painel precisar filtrar por marca?
