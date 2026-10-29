# DW do NEWBLOOMERS · 06. Procedures, checagens e projeto

> Guia 6 de 12 do guia do DW, versão 7. Seções deste arquivo: 9. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 9. Projeto no repositório

```
src/Backend/Modules/DataWarehouse/
├── Application.DataWarehouse/        job que chama as procedures na ordem da 10.1 e grava o resultado das checagens (fase 1)
├── Domain.DataWarehouse/             contratos e entidades (Entities/Control: Watermark, CheckResult)
└── Infrastructure.DataWarehouse/
    └── Scripts/SQLServer/            migrator.csx e scripts numerados (tabelas, procedures, views, seeds)
```

- Os projetos entram em `etl-integrations-system.slnx`, na pasta `/src/Backend/Modules/DataWarehouse/`, com a convenção `Application.X`, `Domain.X` e `Infrastructure.X` do repositório. `Domain.DataWarehouse` e `Infrastructure.DataWarehouse` existem desde a fase 0; `Application.DataWarehouse` entra na fase 1, com o job.
- O `migrator.csx` do DW exige a connection string como argumento e não tem padrão no código. Sem argumento, com a string inválida ou incompleta, ele para com uma mensagem que diz o que falta (servidor, banco, usuário e senha ou autenticação integrada) e imprime só servidor e banco, nunca usuário ou senha.
- **Scripts previstos** (o nome do módulo está no nome do arquivo, como nos demais, para não colidir no banco):

| Script | Conteúdo | Fase |
|---|---|---|
| `Script001_Create_DataWarehouse_Schemas.sql` | Schemas `dw_landing`, `dw_history`, `dw_staging`, `dw_intermediate`, `datawarehouse`, `datamart`, `dw_reference`, `dw_control`, `dw_personal_data`, `dw_outbound` | 0 |
| `Script002_Create_DataWarehouse_Watermarks.sql` | `dw_control.Watermark`, `dw_control.sp_Watermark_Get` e `dw_control.sp_Watermark_Set` (usadas pelos consumidores, 12.2) | 0 |
| `Script003_Create_DataWarehouse_Check_Results.sql` | `dw_control.CheckResult` (9.2) | 0 |
| `Script010_Create_DataWarehouse_Landing_Totvs.sql` | `dw_landing.Totvs*` das fases 1 e 2 (uma tabela por fonte), inclusive `TotvsInvoicesItemsProducts`, `TotvsInvoicesSalesOrders`, `TotvsBalances`, `TotvsCosts` e `TotvsPrices` | 1 |
| `Script011_Create_DataWarehouse_Landing_Totvs_Procedures.sql` | Procedures `dw_landing.sp_Totvs*_Load` | 1 |
| `Script012_Create_DataWarehouse_Landing_LinxMicrovix.sql` | `dw_landing.Linx*` do Microvix e procedures de carga (7.5) | 3 |
| `Script013_Create_DataWarehouse_Landing_LinxCommerce.sql` | `dw_landing.LinxCommerce*` e procedures de carga; só depois da primeira carga real (1.2, item 17) | 3 |
| `Script014_Create_DataWarehouse_Landing_Totvs_Phase4.sql` | `dw_landing.Totvs*` de compras, duplicatas, comissões, recepção, representantes e áreas, e as cinco `*Requests` (7.6) | 4 |
| `Script020_Create_DataWarehouse_History_Totvs.sql` | `dw_history.Totvs*` e procedures `sp_*_Snapshot` de notas, pedidos e cadastros | 1 |
| `Script021_Create_DataWarehouse_History_Totvs_Values.sql` | `dw_history.TotvsBalances`, `TotvsCosts`, `TotvsPrices` e snapshots diários (7.4) | 2 |
| `Script022_Create_DataWarehouse_History_Linx.sql` | `dw_history.Linx*`, snapshot único (7.5) | 3 |
| `Script025_Create_DataWarehouse_Staging_Views.sql` | Views `dw_staging.<Tabela>` de tipagem e renomeação | 1 |
| `Script026_Create_DataWarehouse_Intermediate.sql` | `dw_intermediate.vw_OrderParts` (view sobre a landing: canal, tipo e partes do número do pedido, com `ChannelRules` inline; lida pelo passo 6 e pelas views da logística, 10.3), `dw_intermediate.SalesItem`, `OrderItem` (formato unificado, com `source_erp`) e procedures de regra; `sp_ProductMatch_Load` e `sp_LinxLegacy_Load` entram na fase 3 | 1 |
| `Script030_Create_DataWarehouse_PersonalData.sql` | `dw_personal_data.DimCustomerIdentified` e `dw_personal_data.sp_Customer_Load` (8.3) | 1 |
| `Script031_Create_DataWarehouse_Dimensions.sql` | Dimensões (tabelas e procedures `sp_Dim*_Load`; `sp_DimCustomer_Load` lê `dw_personal_data` e recebe o pepper) | 1 |
| `Script040_Create_DataWarehouse_Facts_Totvs.sql` | `FactSalesItem` (grão item × produto), `FactOrderItem`, `FactInvoicePayment` | 1 |
| `Script041_Create_DataWarehouse_Fact_Stock.sql` | `FactMonthlyStock` e `dw_intermediate.vw_StockPosition` (8.5) | 2 |
| `Script042_Create_DataWarehouse_Facts_Linx.sql` | Procedures que gravam as linhas Linx em `FactSalesItem` e `FactOrderItem` (carga única, 7.5) | 3 |
| `Script043_Create_DataWarehouse_Facts_Totvs_Phase4.sql` | `FactPurchaseOrderItem`, `FactAccountsPayable`, `FactCommission`, `FactStoreReception`, `FactApiRequest`, `DimSalesRepresentative`, `DimRegion`, `DimPromotion` (7.6) | 4 |
| `Script050_Create_DataWarehouse_Datamart.sql` | Agregados (`datamart.DailySales` e procedures `sp_*_Refresh`) | 1 |
| `Script055_Create_DataWarehouse_Outbound_Views.sql` | Views `dw_outbound.<Consumidor><Entidade>` (4.2, 8.6), uma por módulo de saída, conforme cada um for migrado; as da logística leem `vw_OrderParts` e `dw_personal_data` (10.3) | 1 em diante |
| `Script060_Create_DataWarehouse_Checks.sql` | Views de checagem (`datawarehouse.vw_Check_*`) | 1 |
| `Script070_Insert_DataWarehouse_Reference.sql` | Seeds: `dw_reference.Holidays`, `ChannelRules` (canal, série por raiz do CNPJ e tipo do pedido, de `docs/regras-prefixo-pedidos.md`), `StoreErpMap` e `ProductErpMap` (exceções, 7.5) e `ValueCodes` com os 8 custos e 6 preços da PRDFL003 (8.7; o `stockCode` entra depois da primeira carga) | 1 |

- **Numeração por camada:** 001–009 schemas e watermarks; 010–019 `dw_landing`; 020–029 `dw_history`, `dw_staging` e `dw_intermediate`; 030–039 dado pessoal e dimensões; 040–049 fatos; 050–059 `datamart` e `dw_outbound`; 060–069 checagens; 070 em diante, seeds e alterações posteriores, sempre em script novo. Scripts de fases posteriores entram quando a fase começar, com o próximo número livre da camada.
- **Nomes:** `dw_landing.<Tabela>` e `dw_history.<Tabela>` repetem o nome da origem; procedures `dw_landing.sp_<Tabela>_Load`, `dw_history.sp_<Tabela>_Snapshot`, `dw_personal_data.sp_<Entidade>_Load`, `dw_intermediate.sp_<Entidade>_Load`, `datawarehouse.sp_<Tabela>_Load` e `datamart.sp_<Tabela>_Refresh`; views `dw_staging.<Tabela>`, `dw_outbound.<Consumidor><Entidade>`; checagens `datawarehouse.vw_Check_<Nome>`. Tudo em PascalCase inglês; colunas em `snake_case` inglês (1.2, item 8).
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
          FROM dw_intermediate.SalesItem AS v     -- Totvs e Linx no mesmo formato; recarga completa a cada execução (pergunta 8); otimizar depois de medir
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

Cada checagem é uma view que devolve as linhas com erro. O job conta as linhas de cada view, grava o resultado em `dw_control.CheckResult` e falha se alguma tiver linhas.

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

O job grava início e fim com `auditing.sp_Log_Start` e `auditing.sp_Log_End`, e uma mensagem por procedure com `auditing.sp_Log_Message`. O resultado de cada checagem vai para `dw_control.CheckResult`. A retenção de cada um é decidida na seção 3.9.
