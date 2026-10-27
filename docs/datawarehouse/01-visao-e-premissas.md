# DW do NEWBLOOMERS · 01. Visão e premissas

> Guia 1 de 11 do guia do DW, versão 5. Seções deste arquivo: 0, 1 e 2. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

> **Versão 5.** Este guia trata só da modelagem do banco do DW: schemas, tabelas, views e procedures. Ferramentas de BI (Power BI e Metabase), operação do job, ambientes, CI/CD, custos e credenciais ficaram fora e serão tratados em outra etapa da implementação. A seção 21 lista o que saiu.
> O DW é construído num banco zerado, criado pelos migrators (DbUp) deste repositório, do mesmo jeito que AfterSale e Movidesk. Nenhuma premissa depende do banco de desenvolvimento atual (`sql-dev-webjobs`), dos seus dados ou dos seus schemas.
> Verificações de código feitas em 23/10/2026 e 24/10/2026, no estado atual do repositório.
> Itens marcados com **⚠️** continuam em aberto e dependem de decisão de vocês.

---

## 0. Decisões em uma página

| Tema | Escolha | Por quê |
|---|---|---|
| Onde o DW vive | No banco zerado deste repositório, como mais um módulo (`DataWarehouse`), em schemas próprios (4.1). **Decidido** (3.1) | Os módulos de integração já gravam nesse banco. Sem cópia entre bancos |
| Plataforma | Azure SQL Database, o mesmo tipo de banco dos módulos: T-SQL, com PK, FK e UNIQUE impostas de verdade | O time já trabalha com SQL Server |
| Ingestão | Procedures de MERGE dentro do banco, chamadas por um job em ordem fixa (10.1): a `dw_landing` recebe as linhas das tabelas confiáveis por `integrated_or_updated_at`, com 3 dias de sobreposição. **Decidido** (3.2) | A origem está no mesmo banco; nada é transferido |
| Fontes | Tabelas confiáveis dos módulos com tabelas de dados neste repositório: Totvs (fase 1), Movidesk e AfterSale (fase 2). Os demais entram quando tiverem tabelas aqui (1.1) | Só essas fontes têm chave, watermark e contrato conhecidos |
| Histórico | Snapshots SCD2 em T-SQL, incrementais, com hash SHA-256 para detectar mudança. A cada hora para notas e pedidos, diária para cadastros (pergunta 9, 3.3) | A fonte guarda só o estado atual. O DW começa do zero na primeira carga (pergunta 1) |
| Transformação | T-SQL versionado nos `migrator.csx` do módulo. **Decidido** (3.4) | Segue o princípio "develop completely with .NET" do README e usa a linguagem que o repositório já usa |
| Modelagem | Kimball (estrela) em `datawarehouse`; agregados em `datamart`. Sem Data Vault (3.6) | Ferramentas de BI leem bem a estrela; equipe pequena |
| Chaves | Chaves naturais são as PKs das tabelas de origem (incluem `doc_company`). Chave substituta é hash determinístico (MD5 via `HASHBYTES`), não `IDENTITY` (7.1) | Reprocessar uma dimensão não muda as chaves gravadas nos fatos |
| Métricas canônicas | Calculadas em T-SQL, como colunas nos agregados de `datamart` (7.3) | As ferramentas de BI só agregam; nenhuma recalcula a regra |
| Fuso horário | Horário de Brasília sem fuso, como no código (`GetBatchTime()`). `datetimeoffset` convertido para Brasília na entrada (5.2, regra 2) | Padrão do repositório |
| Migrações | `Scripts/SQLServer/migrator.csx` (DbUp via dotnet-script), um por módulo, com scripts numerados para tabelas, procedures, views e seeds (seção 9) | Padrão de AfterSale e Movidesk. Cada script roda uma vez (1.3) |
| Auditoria | Procedures `auditing.sp_Log_*` do módulo Shared, no mesmo banco. Resultados das checagens em `dw_control.check_result` (3.9, 9.2) | Mesmo contrato do sistema. A limpeza de 15 dias do `auditing` não apaga o histórico de checagens |
| Dado pessoal | Colunas identificadoras só em `dw_personal_data`; `dim_cliente` guarda hash com pepper e documento mascarado (5.2 regra 6, 8.3, 12) | Minimização: o dado identificado existe no DW num lugar só |

---

## 1. Premissa e inventário do banco zerado

### 1.1 O que o banco zerado terá

O banco nasce vazio, e os migrators criam tudo. O `migrator.csx` cria o banco se ele não existir (`EnsureDatabase`). A ordem importa: o DW lê as tabelas dos módulos, e as views são validadas na criação.

| Ordem | Migrator | Cria | Papel no DW |
|---|---|---|---|
| 1 | Shared | Schemas `auditing`, `general` e `untreated`; tabelas de auditoria; procedures `sp_Log_*`, `sp_Create_Merge_Procedure`, `sp_Etl_Tables_Maintenance` e `sp_Delete_Auditing_Logs` | Base de todos os módulos |
| 2 | Totvs | Schema `totvs` (86 tabelas criadas por script), cargas em `untreated` e procedures de sincronização | Fonte da fase 1 |
| 3 | Movidesk | Schema `movidesk` (34 tabelas) e cargas em `untreated` | Fonte da fase 2 |
| 4 | AfterSale | Schema `aftersale` (39 tabelas) e cargas em `untreated` | Fonte da fase 2 |
| 5 | DataWarehouse (a criar) | `datawarehouse`, `datamart`, `dw_landing`, `dw_history`, `dw_staging`, `dw_intermediate`, `dw_reference`, `dw_control`, `dw_personal_data` | O DW |

Linx, LinxCommerce, Oto, Pagarme, Jadlog, TotalExpress, Stone e FlashCourier não criam tabelas de dados neste repositório: só inserem em `auditing.Apps` e `auditing.Jobs`. Por isso não são fonte do DW hoje (1.2, item 6).

### 1.2 Achados que mudam o desenho

1. **A fonte guarda só o estado atual.** As tabelas confiáveis são atualizadas por `MERGE` e guardam só a versão vigente. Um cancelamento sobrescreve o status anterior, e sem snapshot próprio esse histórico se perde. **Por isso o histórico começa na primeira carga do banco**, nas tabelas que já estão no padrão (seção 16). O histórico do DW atual não entra: decidido na pergunta 1.
2. **Filhas excluídas na origem somem da tabela confiável.** O merge das filhas usa condição de exclusão: no Movidesk, `@delete_by_ticket`; no Totvs, as filhas que saíram de uma nota do lote são apagadas (`Script026_Create_Totvs_Invoices_Sync_Procedures.sql`). Essa condição depende de o merge da tabela pai rodar antes. Os cabeçalhos não são apagados: nota excluída na Totvs fica com `invoiceStatus = 'Deleted'`.
3. **`untreated` não é histórico.** `dbo.sp_Etl_Tables_Maintenance` trunca todas as tabelas do schema e, na mesma rotina, chama `dbo.sp_Delete_Auditing_Logs`.
4. **O princípio do repositório é "develop completely with .NET"** (README). A transformação ficou em T-SQL pelo mesmo motivo (3.4).
5. **O padrão de horário é Brasília sem fuso.** `GetBatchTime()` (`TotvsService.cs:31`) alimenta `integrated_or_updated_at`, e os logs de auditoria usam o mesmo fuso.
6. **A migração para o padrão está pela metade.** Totvs, Movidesk e AfterSale têm tabelas de dados e sincronização. Linx, Oto, Pagarme e as integrações de logística não têm tabelas de dados aqui. Esses módulos não entram no DW até terem tabelas neste repositório. A migração termina em dezembro de 2026, e os projetos com saída de dados seguem o padrão de AfterSale e Movidesk.
7. **Dois ERPs, um ativo.** Só o Totvs gera linhas novas. Linx Microvix e Linx Commerce foram descontinuados na Newbloomers. O módulo Linx Microvix continua com integrações recorrentes, para poder ser usado em outro cliente (`docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md`, §10.1): a primeira execução de cada método traz o histórico inteiro, e as seguintes só trazem o que mudou, o que na Newbloomers não gera linhas novas. No DW, esse histórico entra como legado, com o mapa CNPJ × ERP × período para olhar o passado (perguntas 13 e 15).
8. **Convenção de nomes.** As tabelas do módulo são `PascalCase` com o prefixo do módulo (`TotvsInvoices`). O DW usa `snake_case`. Não é conflito técnico, mas precisa ficar explícito no código.
9. **As chaves do Totvs incluem `doc_company`**, a empresa do grupo. Por exemplo, a PK de `totvs.TotvsInvoices` é (`doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`). Toda chave natural do DW inclui `doc_company`.
10. **Tabelas de log de requisições não têm `integrated_or_updated_at`:** `AfterSaleRequests` e `MovideskRequests`. Não são dado de negócio.
11. **Pedidos de e-commerce têm prefixo no número.** `MI-` ou `OA-`. O Oto e o Stone deste repositório filtram por esse prefixo. A regra que gera o prefixo não está neste repositório. Para o DW, o critério de e-commerce é outro: pedido de `TotvsSaleOrders` com `integrationCode` não nulo (pergunta 15).
12. **Cargas iniciais por data de alteração.** Notas, pedidos, produtos, vendedores e pessoas são filtrados pela data de alteração na Totvs (`filter.change` e `StartChangeDate`). Cadastro entra se foi criado ou alterado desde 2000 (5.2, regra 5).

### 1.3 Padrões que reaproveitamos

Verificados no código deste repositório.

- **Auditoria** (módulo Shared, `Scripts/SQLServer/Script002_Create_Auditing_Procedures.sql`): `auditing.sp_Log_Start(@jobTitle, @parentExecutionGUID, @idJob OUT, @execution OUT, @idType = 1)`, `auditing.sp_Log_Message(@execution, @message, @logLevel)`, `auditing.sp_Log_Error(@Execution, @err_msg)` e `auditing.sp_Log_End(@Execution, @MergeOutputSTR)`. Tabelas: `auditing.Apps`, `Jobs`, `Logs`, `Messages`, `Records`, `Types`.
- **Carga e sincronização das tabelas confiáveis:** `untreated.<tabela>` recebe a carga bruta. `dbo.sp_Create_Merge_Procedure(@schema_name, @table_name, @execute, @delete_condition)` gera `<schema>.sp_<tabela>_Sync` e é chamado em scripts numerados (ex.: `Script005_Create_AfterSale_Sync_Procedures.sql`). O MERGE tem exclusão opcional por condição.
- **Migrações:** `Scripts/SQLServer/migrator.csx` (DbUp via dotnet-script), um por módulo. Ele executa os arquivos em ordem de nome e grava cada nome na tabela de controle do DbUp (`SchemaVersions`, o padrão). Como todos os módulos usam o mesmo banco e a mesma tabela de controle, os nomes dos scripts precisam ser únicos no repositório. Hoje não há nomes repetidos (verificado). Cada script roda uma vez, então alterações de procedure e view entram em script novo, com `CREATE OR ALTER`. O `migrator.csx` do DW não traz connection string padrão (seção 16, passo 1).
- **Hash SHA-256:** usado nos serviços de AfterSale, Movidesk e LinxCommerce.
- **Watermarks:** `integrated_or_updated_at` (ETL, uniforme) e `maxChangeFilterDate` ou `lastChangeDate` (origem Totvs). A seção 5.1 detalha.

---

## 2. Escopo e fases

| Fase | Domínio | Fontes (tabelas neste repositório) | Objetos do banco entregues | Critério de aceite |
|---|---|---|---|---|
| 0 | Base | — | Módulo `DataWarehouse` com `migrator.csx`; schemas, `dw_control.watermark` e `dw_control.check_result` (scripts 001 a 009, seção 9) | Banco inteiro criado do zero sem erro pelos migrators, na ordem da 1.1 |
| 1 | Vendas e cadastros (Totvs) | `totvs.TotvsInvoices`, `TotvsInvoicesItems`, `TotvsInvoicesPayments`, `TotvsSaleOrders`, `TotvsSaleOrdersProducts`, `TotvsPeople`, `TotvsLegalEntities`, `TotvsSkus`, `TotvsSkusColors`, `TotvsSellers`, `TotvsBranches`, `TotvsProductsSuppliers`; só para `dw_personal_data`: `TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones` e as equivalentes de `TotvsLegalEntities` (5.2, regra 6) | Landing, snapshots, `dw_personal_data`, dimensões, `fato_venda_item`, `fato_pedido_item`, `fato_pagamento`, `datamart.vendas_diarias`, views de checagem | Carga repetida dá o mesmo resultado; checagens verdes; total mensal por loja explicado contra o fechamento de referência, no período carregado |
| 2 | Pós-venda (Movidesk e AfterSale) | `movidesk.MovideskTickets`, `MovideskTicketsActions`, `MovideskPeopleEmails`; `aftersale.AfterSaleRefunds`, `AfterSaleReverses` | `fato_atendimento` (tickets) e `fato_reembolso` (AfterSale), que têm grãos diferentes | Totais batem com as tabelas de origem |
| 3 | Linx Microvix e Linx Commerce (histórico, legado), e-commerce, estoque, logística, Pagarme | Entram quando o módulo criar tabelas de dados neste repositório (1.1), até dezembro de 2026. O histórico do Linx Microvix chega na primeira execução das integrações recorrentes do módulo; como nenhuma loja vende por Linx, só o Totvs gera linhas novas (pergunta 13) | Fatos e dimensões correspondentes, no mesmo padrão | Mesmo critério das fases 1 e 2 |

**Dependência de negócio:** a fase 1 cobre todas as lojas ativas, porque nenhuma vende por Linx hoje. O passado das lojas que vendiam por Linx entra com o histórico Linx, ligado pelo mapa CNPJ × ERP × período (pergunta 13).
