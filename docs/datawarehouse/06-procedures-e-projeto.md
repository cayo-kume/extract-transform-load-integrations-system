# DW do NEWBLOOMERS · 06. Procedures, checagens e projeto

> Guia 6 de 12 do guia do DW, versão 7. Seções deste arquivo: 9. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 9. Projeto no repositório

```
src/Backend/Modules/DataWarehouse/
├── Application.DataWarehouse/        job que chama as procedures na ordem da 10.1 e grava o resultado das checagens (9.4)
├── Domain.DataWarehouse/             contratos e entidades (Entities/Control: Watermark, CheckResult)
└── Infrastructure.DataWarehouse/
    └── Scripts/SQLServer/            migrator.csx e scripts numerados (tabelas, procedures, views, seeds)
```

- Os projetos entram em `etl-integrations-system.slnx`, na pasta `/src/Backend/Modules/DataWarehouse/`, com a convenção `Application.X`, `Domain.X` e `Infrastructure.X` do repositório. `Domain.DataWarehouse` e `Infrastructure.DataWarehouse` existem desde a fase 0; `Application.DataWarehouse` entrou na fase 1, com o job.
- O `migrator.csx` do DW exige a connection string como argumento e não tem padrão no código. Sem argumento, com a string inválida ou incompleta, ele para com uma mensagem que diz o que falta (servidor, banco, usuário e senha ou autenticação integrada) e imprime só servidor e banco, nunca usuário ou senha.
- **Scripts previstos** (o nome do módulo está no nome do arquivo, como nos demais, para não colidir no banco):

| Script | Conteúdo | Fase |
|---|---|---|
| `Script001_Create_DataWarehouse_Schemas.sql` | Schemas `dw_landing`, `dw_history`, `dw_staging`, `dw_intermediate`, `datawarehouse`, `datamart`, `dw_reference`, `dw_control`, `dw_personal_data`, `dw_outbound` | 0 |
| `Script002_Create_DataWarehouse_Watermarks.sql` | `dw_control.Watermark`, `dw_control.sp_Watermark_Get` e `dw_control.sp_Watermark_Set` (usadas pelos consumidores, 12.2) | 0 |
| `Script003_Create_DataWarehouse_Check_Results.sql` | `dw_control.CheckResult` (9.2) | 0 |
| `Script004_Create_DataWarehouse_Landing_Sources.sql` | `dw_control.LandingSource`: catálogo das origens (9.5) | 1 |
| `Script005_Create_DataWarehouse_Reference_Tables.sql` | Tabelas de `dw_reference`: `Holidays`, `BrandSeries` (série do pedido pela raiz do CNPJ), `ChannelRules`, `StoreErpMap`, `ProductErpMap` e `ValueCodes`. Os dados entram no 070 | 1 |
| `Script006_Insert_DataWarehouse_Jobs_Into_Jobs_Table.sql` | App 90 e jobs 9000 (`DataWarehouseHourly`), 9001 (`DataWarehouseDaily`) e 9002 (`DataWarehouseShortCycle`) em `auditing` (9.4) | 1 |
| `Script007_Alter_DataWarehouse_Landing_Sources_Add_Is_Enabled.sql` | Coluna `is_enabled` em `dw_control.LandingSource`: o job só lê as origens ligadas (9.5) | 2 |
| `Script010_Create_DataWarehouse_Landing_Generator.sql` | `dw_landing.sp_Create_Landing`: gera landing, carga e detecção de exclusão a partir do catálogo (9.5) | 1 |
| `Script011_Create_DataWarehouse_Landing_Totvs.sql` | Catálogo e landing das 23 origens Totvs da fase 1, inclusive `TotvsInvoicesItemsProducts`, `TotvsInvoicesSalesOrders`, `TotvsCosts` e `TotvsPrices` | 1 |
| `Script008_Alter_DataWarehouse_Landing_Sources_Once.sql` | `history_frequency = 'once'` no catálogo: snapshot só na carga do legado (9.5) | 3 |
| `Script012_Create_DataWarehouse_Landing_LinxMicrovix.sql` | Catálogo (desligado) e landing de `LinxMovimento`, `LinxMovimentoPrincipal`, `LinxMovimentoDevolucoesItens`, `LinxB2CPedidos`, `LinxB2CPedidosItens`, `LinxLojas`, `LinxProdutos`, `LinxVendedores` e `LinxClientesFornec`, sem as colunas de dado pessoal (7.5, 8.3) | 3 |
| `Script013_Create_DataWarehouse_Landing_LinxCommerce.sql` | Catálogo e landing do Linx Commerce; só depois da primeira carga real (1.2, item 17) | 3 |
| `Script014_Create_DataWarehouse_Landing_Phase2.sql` | Catálogo e landing de `TotvsBalances` (nasce desligada, 7.4), `MovideskTickets`, `MovideskTicketsActions`, `MovideskTicketsClients`, `AfterSaleRefunds`, `AfterSaleRefundsProducts` e `AfterSaleReverses`, sem as colunas de dado pessoal (8.3) | 2 |
| `Script015_Create_DataWarehouse_Landing_Totvs_Phase4.sql` | Catálogo e landing de `TotvsPurchaseOrders`, `TotvsPurchaseOrdersProducts`, `TotvsDuplicates`, `TotvsReceptionSales`, `TotvsRepresentatives`, `TotvsOperationalAreas`, `TotvsPromotions` e as cinco `*Requests`, sem documento, nome do cliente, observação e corpo das requisições. `TotvsCommissionsPaid` fica sem landing: o documento do comissionado está na chave | 4 |
| `Script020_Create_DataWarehouse_History_Generator.sql` | `dw_history.sp_Create_History`: gera histórico SCD2 e snapshot incremental (9.5) | 1 |
| `Script021_Create_DataWarehouse_History_Totvs.sql` | Históricos das 9 origens Totvs com `history_frequency`: notas e pedidos (hora); lojas, pessoas, empresas, referências, produtos, custos e preços (dia) | 1 |
| `Script022_Create_DataWarehouse_History_Linx.sql` | Históricos de `LinxLojas` e `LinxProdutos`, snapshot único (7.5) | 3 |
| `Script023_Create_DataWarehouse_History_Phase2.sql` | Histórico de `TotvsBalances`, com snapshot diário (7.4) | 2 |
| `Script025_Create_DataWarehouse_Staging_Views.sql` | Não criado: a tipagem e a renomeação ficaram nas views de `dw_intermediate`, que já leem a landing e o histórico com os nomes do DW. O schema `dw_staging` existe (001) e fica vazio | — |
| `Script026_Create_DataWarehouse_Intermediate.sql` | Três views, sem tabela nem procedure: `dw_intermediate.vw_OrderParts` (pedido: canal, tipo, série e partes do número, com `ChannelRules` inline; lida pelo passo 6 e pelas views de saída, 10.3), `vw_SalesItem` (grão item × produto da nota, com rateio de frete e imposto, canal, sinal, preço e custo da data) e `vw_OrderItem` (grão produto do pedido). As procedures de fato leem as views; como a recarga é completa (pergunta 8), uma tabela intermediária só copiaria a view. o de/para e as linhas Linx entram na fase 3 (Script028 e Script042) | 1 |
| `Script027_Create_DataWarehouse_Intermediate_Phase2.sql` | `dw_intermediate.fn_StockPosition(@position_date)` (posição de estoque no fim de um dia, com loja, produto e custo da época; 8.5), `vw_Ticket` (ticket com origem, tipo, cliente principal, contagem de ações e SLA) e `vw_Refund` (reembolso com a reversa e o resumo dos produtos) | 2 |
| `Script028_Create_DataWarehouse_Intermediate_Linx.sql` | `dw_intermediate.vw_ProductMatch` (de/para de produto, pergunta 34), `vw_LinxSalesItem` e `vw_LinxOrderItem` (7.5) | 3 |
| `Script030_Create_DataWarehouse_Personal_Data.sql` | `dw_personal_data.DimCustomerIdentified` e `sp_Customer_Load` (8.3; pessoas e empresas com endereço, e-mail e telefone), `OrderShippingAddress` e `sp_OrderShippingAddress_Load` (endereço de entrega do pedido, lido de `totvs.TotvsSaleOrders`, para `LogisticsShipments`) | 1 |
| `Script032_Create_DataWarehouse_Personal_Data_Phase2.sql` | `dw_personal_data.ServiceCustomerIdentified` e `sp_ServiceCustomer_Load` (documento dos clientes do Movidesk e do AfterSale) e `datawarehouse.fn_DocHash` (o mesmo SHA-256 com pepper de `DimCustomer`, 8.3) | 2 |
| `Script033_Create_DataWarehouse_Personal_Data_Linx.sql` | `dw_personal_data.sp_LinxCustomer_Load` (clientes Linx em `DimCustomerIdentified`) | 3 |
| `Script034_Alter_DataWarehouse_Dimensions_Linx.sql` | `DimStore.source_erp`, `DimOperation.operation_code` como texto, e as procedures de loja, produto, cliente, vendedor, operação e marca com os membros Linx (7.5) | 3 |
| `Script035_Create_DataWarehouse_Dimensions_Phase4.sql` | `DimSalesRepresentative`, `DimRegion` e `DimPromotion` com as procedures de carga, e `sp_DimSupplier_Load` com os fornecedores de compras e duplicatas (7.6) | 4 |
| `Script031_Create_DataWarehouse_Dimensions.sql` | Dimensões (tabelas e procedures `sp_Dim*_Load`; `sp_DimCustomer_Load` lê `dw_personal_data` e recebe o pepper) | 1 |
| `Script040_Create_DataWarehouse_Facts_Totvs.sql` | `FactSalesItem` (grão item × produto), `FactOrderItem`, `FactInvoicePayment` | 1 |
| `Script041_Create_DataWarehouse_Fact_Stock.sql` | `FactMonthlyStock` e `sp_FactMonthlyStock_Load @position_date` (8.5) | 2 |
| `Script042_Create_DataWarehouse_Facts_Linx.sql` | `dw_intermediate.sp_LinxLegacy_Load`, que grava as linhas Linx em `FactSalesItem` e `FactOrderItem` (carga única, 7.5), e `product_code` dos fatos como `bigint` | 3 |
| `Script043_Create_DataWarehouse_Facts_Totvs_Phase4.sql` | `FactPurchaseOrderItem`, `FactAccountsPayable`, `FactCommission` (lê a origem e recebe o pepper), `FactStoreReception` e `FactApiRequest` (incremental), com as procedures de carga (7.6) | 4 |
| `Script044_Create_DataWarehouse_Facts_AfterSales.sql` | `FactTicket` e `FactRefund`, com `sp_FactTicket_Load` e `sp_FactRefund_Load` (recebem o pepper), e o índice de `DimCustomer.doc_hash` | 2 |
| `Script050_Create_DataWarehouse_Datamart.sql` | Agregados (`datamart.DailySales` e procedures `sp_*_Refresh`) | 1 |
| `Script055_Create_DataWarehouse_Outbound_Views.sql` | Views `dw_outbound.<Consumidor><Entidade>` (4.2, 8.6), uma por módulo de saída, conforme cada um for migrado; as da logística leem `vw_OrderParts` e `dw_personal_data` (10.3) | 1 em diante |
| `Script060_Create_DataWarehouse_Checks.sql` | Views de checagem: `datawarehouse.vw_Check_FactSalesItemGrain`, `vw_Check_UnknownMembers`, `vw_Check_ItemProducts`, `vw_Check_OrderTypeUnknown`, `vw_Check_MissingChildren` e `datamart.vw_Check_InvoicedReconciliation` (9.2, 9.3) | 1 |
| `Script061_Create_DataWarehouse_Checks_Phase2.sql` | `datawarehouse.vw_Check_StockNegativeOrNoCost`, e `vw_Check_UnknownMembers` e `vw_Check_MissingChildren` refeitas para cobrir `FactMonthlyStock` e as filhas do Movidesk | 2 |
| `Script062_Create_DataWarehouse_Checks_Linx.sql` | `datawarehouse.vw_Check_ErpOverlap` (bloqueia), `vw_Check_ProductMatch`, e `datamart.vw_Check_InvoicedReconciliation` refeita com as linhas Linx (9.3) | 3 |
| `Script063_Create_DataWarehouse_Checks_Phase4.sql` | `vw_Check_UnknownMembers` refeita com os fatos de compras, contas a pagar e recepção de loja | 4 |
| `Script070_Insert_DataWarehouse_Reference.sql` | Seeds: `dw_reference.Holidays` (2010 a 2035, Páscoa calculada), `BrandSeries` (série por raiz do CNPJ), `ChannelRules` (canal e tipo do pedido, de `docs/regras-prefixo-pedidos.md`), `StoreErpMap` e `ProductErpMap` (exceções, 7.5) e `ValueCodes` com os 8 custos e 6 preços da PRDFL003 (8.7; o `stockCode` entra depois da primeira carga) | 1 |

- **Numeração por camada:** 001–009 schemas e watermarks; 010–019 `dw_landing`; 020–029 `dw_history`, `dw_staging` e `dw_intermediate`; 030–039 dado pessoal e dimensões; 040–049 fatos; 050–059 `datamart` e `dw_outbound`; 060–069 checagens; 070 em diante, seeds e alterações posteriores, sempre em script novo. Scripts de fases posteriores entram quando a fase começar, com o próximo número livre da camada.
- **Nomes:** `dw_landing.<Tabela>` e `dw_history.<Tabela>` repetem o nome da origem; procedures `dw_landing.sp_<Tabela>_Load`, `dw_landing.sp_<Tabela>_DetectMissing`, `dw_history.sp_<Tabela>_Snapshot`, `dw_personal_data.sp_<Entidade>_Load`, `dw_intermediate.sp_LinxLegacy_Load` (fase 3), `datawarehouse.sp_<Tabela>_Load` e `datamart.sp_<Tabela>_Refresh`; views `dw_intermediate.vw_<Entidade>`, `dw_outbound.<Consumidor><Entidade>`; checagens `datawarehouse.vw_Check_<Nome>`. Tudo em PascalCase inglês; colunas em `snake_case` inglês (1.2, item 8).
- No Windows, o caminho do `migrator.csx` passa de 260 caracteres. Usar `subst` para rodar, como descrito em `docs/MIGRATOR-CSX.md`.

### 9.1 Exemplo de procedure de fato (MERGE)

```sql
CREATE OR ALTER PROCEDURE datawarehouse.sp_FactSalesItem_Load
AS
BEGIN
    SET NOCOUNT ON;

    MERGE datawarehouse.FactSalesItem AS target
    USING (
        SELECT v.source_erp,
               v.source_key,                                                                                           -- CONCAT_WS('|', ...) da PK da origem, montado em dw_intermediate
               v.doc_company,
               v.invoice_date,
               CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT_WS('|', v.doc_company, v.branch_code)), 2)                   AS store_sk,
               CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT_WS('|', v.doc_company, v.source_erp, v.product_code)), 2)    AS product_sk
               -- demais chaves e medidas
          FROM dw_intermediate.vw_SalesItem AS v  -- view; Totvs e Linx no mesmo formato; recarga completa a cada execução (pergunta 8); otimizar depois de medir
    ) AS source
       ON target.source_erp = source.source_erp
      AND target.source_key = source.source_key
    WHEN MATCHED THEN
        UPDATE SET target.invoice_date = source.invoice_date,
                   target.store_sk = source.store_sk,
                   target.product_sk = source.product_sk
    WHEN NOT MATCHED THEN
        INSERT (source_erp, source_key, doc_company, invoice_date, store_sk, product_sk)
        VALUES (source.source_erp, source.source_key, source.doc_company, source.invoice_date, source.store_sk, source.product_sk);
END;
```

Os nomes de coluna são ilustrativos. A chave do MERGE é (`source_erp`, `source_key`), e a checagem de grão (9.2) confirma que ela é única. A `store_sk` do Linx é resolvida pelo CNPJ em `dw_intermediate` (7.5), por isso a expressão acima já recebe `branch_code`; a `product_sk` do Linx usa `COALESCE(matched_product_sk, sk)` do membro Linx (pergunta 34). Sem filtro de janela: a recarga é completa a cada execução (pergunta 8), e a otimização vem depois da medição.

### 9.2 Checagens (substituem os testes do dbt)

Cada checagem é uma view que devolve as linhas com erro. O job conta as linhas de cada view e grava o resultado em `dw_control.CheckResult`. Só a de grão (`vw_Check_FactSalesItemGrain`) bloqueia: com ela, a execução termina como falha; as demais ficam registradas e numa mensagem do log, e quem acompanha decide.

```sql
CREATE OR ALTER VIEW datawarehouse.vw_Check_FactSalesItemGrain AS
SELECT source_erp, source_key, COUNT(*) AS rows_found
  FROM datawarehouse.FactSalesItem
 GROUP BY source_erp, source_key
HAVING COUNT(*) > 1;
```

Limites com porcentagem (ex.: membro desconhecido acima de 5%) entram como `HAVING` ou como comparação no próprio job.

```sql
CREATE TABLE dw_control.CheckResult (
    check_name       VARCHAR(100) NOT NULL,
    executed_at      DATETIME2(3) NOT NULL,   -- horário de Brasília
    error_rows       INT          NOT NULL,
    passed           BIT          NOT NULL,
    CONSTRAINT pk_CheckResult PRIMARY KEY (check_name, executed_at)
);
```

### 9.3 Checagens de reconciliação

- `datamart.vw_Check_InvoicedReconciliation` compara o total mensal por loja de `datamart.DailySales` com a soma direta de `dw_landing.TotvsInvoicesItems`, pela mesma regra, e devolve as lojas em que a diferença passa de R$ 1,00. Mostra se a divergência com o fechamento de referência é real ou só diferença de definição (seção 7.3).
- `datawarehouse.vw_Check_ItemProducts` compara, por item da nota, a soma de `dw_landing.TotvsInvoicesItemsProducts` com o valor do item em `dw_landing.TotvsInvoicesItems`; devolve os itens em que o rateio não fecha (7.2).
- `datawarehouse.vw_Check_ErpOverlap` devolve os pares CNPJ e dia com venda faturada nos dois ERPs (7.5).
- `datawarehouse.vw_Check_ProductMatch` devolve, por marca, a proporção de produtos Linx sem `matched_product_sk` (7.5, pergunta 34).
- `datawarehouse.vw_Check_OrderTypeUnknown` devolve os pedidos cujo tipo não casou com nenhuma regra de `dw_reference.ChannelRules`; é a inconsistência 8 de `docs/regras-prefixo-pedidos.md` (`CASE` sem `ELSE`), que no DW vira alerta em vez de número sem prefixo.

### 9.4 Registro de execuções

O job é `Application.DataWarehouse.Services.DataWarehouseService`, com cinco entradas: `RunHourly` (job 9000), `RunDaily` (9001: a horária mais a detecção de exclusões, os snapshots diários, `DimDate` e, no dia 1, a posição de estoque do mês que fechou), `RunShortCycle` (9002, 10.3) `RunMonthlyStock` (9003: refaz a posição de um fim de mês, 8.5) e `RunLinxLegacy` (9004: carga única do legado Linx, 10.2). Ele só chama as procedures na ordem da 10.1; a regra fica nelas. O registro usa o `ILoggerService` do Shared, como os outros módulos: uma linha em `auditing.Logs` por execução (`IdType` 2, integração, o mesmo que o logger grava) e uma mensagem em `auditing.Messages` por procedure, com as linhas afetadas e o tempo. Uma falha interrompe a execução e vira mensagem de erro. O resultado de cada checagem vai para `dw_control.CheckResult`. A retenção de cada um é decidida na seção 3.9.

- **Uma carga por vez:** as três entradas escrevem nas mesmas landings, então o serviço tem uma trava no processo; uma chamada com outra carga em andamento volta sem rodar. A trava é liberada mesmo se a gravação do log falhar. O agendador que for codificado precisa da mesma trava entre processos (no Hangfire, `DisableConcurrentExecution`).
- **Pepper:** vem da configuração `DataWarehouse:Pepper` (variável de ambiente ou segredo, nunca `appsettings`, git, tabela ou log) e só passa como parâmetro das procedures que calculam o `doc_hash`: `sp_DimCustomer_Load`, `sp_FactTicket_Load`, `sp_FactRefund_Load` e `sp_FactCommission_Load` (8.3). Sem ele, essas tabelas não são atualizadas, o log diz isso e o resto da carga segue.
- **Disparo:** enquanto os agendadores não existem, o host Hangfire expõe `POST DataWarehouseJobs/DataWarehouse/RunHourly`, `RunDaily`, `RunShortCycle`, `RunMonthlyStock?positionDate=yyyy-MM-dd` e `RunLinxLegacy` (200 concluída, 500 falha ou checagem bloqueante, 409 outra carga em andamento). Os intervalos dos jobs ficam em `auditing.Jobs` (60, 1440 e 3 minutos; o 9003 e o 9004 são manuais).

### 9.5 Landing e histórico gerados pelo catálogo

A landing e o histórico seguem o padrão do repositório para o merge das integrações (`dbo.sp_Create_Merge_Procedure`): em vez de uma tabela e uma procedure escritas à mão por origem, um catálogo e dois geradores. Cada fase insere as linhas das suas origens em `dw_control.LandingSource` e chama os geradores no próprio script.

| Coluna de `dw_control.LandingSource` | Uso |
|---|---|
| `source_schema`, `source_table` | Origem (`totvs.TotvsInvoices`). A landing e o histórico têm o mesmo nome; o schema é o `source` do watermark |
| `included_columns` | Lista de colunas, quando só algumas podem entrar (pessoas e empresas, 5.2 regra 6). A PK, `integrated_or_updated_at` e a data da origem entram sempre |
| `excluded_columns` | Colunas de dado pessoal que não entram (nome, documento, endereço e telefone do cliente em notas e pedidos) |
| `source_updated_column` | Data da alteração na origem (`maxChangeFilterDate`, `lastChangeDate`); sem ela, `integrated_or_updated_at` |
| `detect_missing` | Filha que o merge da origem apaga: gera a detecção de exclusão (5.2, regra 3) |
| `history_frequency` | `hourly`, `daily` ou `once`: gera o histórico. A frequência é de quem chama o snapshot: os ciclos horário e diário chamam `hourly` e `daily` (10.1); a carga do legado chama `once` (10.2) |
| `phase` | Fase do plano em que a origem entrou |
| `is_enabled` | Origem ligada (padrão). Os ciclos agendados só chamam as procedures das ligadas; a de saldos nasce desligada e é ligada depois da carga inicial na Totvs (1.2, item 15), e as do Linx ficam desligadas, porque só a carga do legado as lê (10.2) |

- **`dw_landing.sp_Create_Landing`** cria `dw_landing.<Tabela>` com as colunas permitidas da origem (mesmos nomes e tipos, mesma PK), mais `source_updated_at`, `row_hash`, `loaded_at` e `missing_since`, e um índice em `loaded_at`. Gera `sp_<Tabela>_Load` (MERGE pelo watermark menos 3 dias; só atualiza, e só grava `loaded_at`, quando o hash muda ou a chave tinha sumido; avança o watermark na mesma transação) e, nas filhas, `sp_<Tabela>_DetectMissing`. O hash é SHA-256 de todas as colunas de negócio, sem a PK e sem as colunas de controle (`integrated_or_updated_at`, `maxChangeFilterDate`, `lastChangeDate`, `lastchange`, `json_hash`): regravar a linha sem mudar o negócio não gera versão.
- **`dw_history.sp_Create_History`** cria `dw_history.<Tabela>` (PK + `valid_from`, `valid_to`, `is_current`, `row_hash` e as colunas de negócio) com um índice único filtrado que garante uma só versão vigente por chave, e gera `sp_<Tabela>_Snapshot` como na seção 6. Quando a data da origem não avança (`maxChangeFilterDate` repetida), a versão nova começa 1 ms depois da vigente, e a PK não colide (risco R2).
- **As procedures geradas não se editam:** uma mudança no gerador ou no catálogo entra em script novo, que chama o gerador de novo (`create or alter`). A tabela só é criada se não existir; mudar colunas de uma landing que já existe pede um `ALTER TABLE` em script próprio.
- A landing guarda todas as colunas permitidas da origem, e não só as que o modelo usa hoje: uma coluna nova no modelo não exige recarregar a origem, e o histórico já a tem desde o início.
- O índice filtrado do histórico exige `QUOTED_IDENTIFIER ON`, o padrão do .NET e do DbUp; as procedures geradas guardam essa opção. Uma sessão do `sqlcmd` sem `-I` não consegue escrever nessas tabelas fora das procedures.
