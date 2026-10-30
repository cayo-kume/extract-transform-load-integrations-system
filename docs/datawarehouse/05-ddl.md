# DW do NEWBLOOMERS · 05. DDL de referência

> Guia 5 de 12 do guia do DW, versão 7. Seções deste arquivo: 8. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 8. DDL de referência

Scripts em `src/Backend/Modules/DataWarehouse/Infrastructure.DataWarehouse/Scripts/SQLServer/`, numerados, com o `migrator.csx` do padrão do repositório, sem connection string padrão. Cada script roda uma vez. Os nomes precisam ser únicos no repositório, porque o DbUp grava o nome de cada arquivo. Procedures e views de negócio usam `CREATE OR ALTER`; alteração entra em script novo.

Nomes (1.2, item 8): schemas em `snake_case`; tabelas, views e procedures em PascalCase inglês; colunas em `snake_case` inglês, menos nas landings e históricos, que repetem as colunas da origem.

Horário padrão das colunas de carga, o mesmo das procedures de auditoria:

```sql
SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
```

### 8.1 Schemas e watermarks

```sql
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'dw_landing')
    EXEC('CREATE SCHEMA dw_landing');
-- repetir para: dw_history, dw_staging, dw_intermediate, datawarehouse, datamart, dw_reference, dw_control, dw_personal_data, dw_outbound

CREATE TABLE dw_control.Watermark (
    source       VARCHAR(50)  NOT NULL,   -- totvs, linxmicrovix, linxcommerce, movidesk, aftersale, dw_history, e cada módulo de saída (4.2)
    entity       VARCHAR(100) NOT NULL,   -- tabela de origem, entidade do snapshot ou view de dw_outbound
    last_value   DATETIME2(3) NOT NULL,   -- horário de Brasília: integrated_or_updated_at da origem (landing), loaded_at da landing (snapshot, seção 6) ou updated_at da view (consumidor)
    updated_at   DATETIME2(3) NOT NULL CONSTRAINT df_Watermark_updated DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_Watermark PRIMARY KEY (source, entity)
);
```

### 8.2 Landing das origens

O DDL da `dw_landing.TotvsInvoices` está na seção 6. As demais seguem o mesmo padrão, com a PK da origem e as colunas que o DW usa, com os mesmos nomes, para manter a rastreabilidade. O nome da landing e do histórico é o nome da tabela de origem, sem o schema: `dw_landing.TotvsInvoicesItemsProducts` para `totvs.TotvsInvoicesItemsProducts`, `dw_landing.LinxMovimento` para `linxmicrovix.LinxMovimento`, `dw_landing.LinxCommerceOrders` para `linxcommerce.LinxCommerceOrders`. A procedure de carga faz MERGE pela PK. Colunas identificadoras de pessoa não entram em nenhuma landing (5.2, regra 6). Como a origem está no mesmo banco, o DW não passa pela área de carga bruta `untreated`.

As landings do Linx guardam o `timestamp` do Microvix como atributo, não como watermark (5.2, regra 2), e são carregadas uma vez (7.5).

### 8.3 Dimensão de cliente, com dado pessoal separado

```sql
CREATE TABLE datawarehouse.DimCustomer (
    sk               CHAR(32)     NOT NULL CONSTRAINT pk_DimCustomer PRIMARY KEY,   -- MD5(doc_company|source_erp|customer_code)
    doc_company      NVARCHAR(14) NOT NULL,
    source_erp       VARCHAR(20)  NOT NULL,     -- totvs, linxmicrovix, linxcommerce: os códigos dos ERPs colidem
    customer_code    NVARCHAR(50) NOT NULL,     -- code em TotvsPeople e TotvsLegalEntities (personCode nas notas); cod_cliente em LinxClientesFornec; CustomerID em LinxCommerceCustomers
    doc_hash         CHAR(64)     NULL,         -- SHA-256(pepper + CPF/CNPJ); o pepper chega como parâmetro (12.3). Igual nos ERPs para o mesmo documento
    doc_masked       VARCHAR(20)  NULL,         -- ex.: ***.***.***-12
    person_type      CHAR(1)      NULL,         -- F / J
    state            CHAR(2)      NULL,         -- UF
    age_range        VARCHAR(20)  NULL          -- calculada de dw_personal_data.birth_date
);                                              -- SCD1 (pergunta 11): sem valid_from e valid_to

-- dw_personal_data.DimCustomerIdentified: só a identificação atual, sem versões. Só o grupo dw_pii lê (12.2).
-- Carregada por dw_personal_data.sp_Customer_Load, direto de totvs.TotvsPeople, TotvsLegalEntities e das tabelas
-- de endereço, e-mail e telefone (registro padrão de cada uma), de linxmicrovix.LinxClientesFornec e de
-- linxcommerce.LinxCommerceCustomers, por MERGE na chave, com o watermark da 5.2.
CREATE TABLE dw_personal_data.DimCustomerIdentified (
    doc_company      NVARCHAR(14)  NOT NULL,
    source_erp       VARCHAR(20)   NOT NULL,
    customer_code    NVARCHAR(50)  NOT NULL,
    person_type      CHAR(1)       NOT NULL,    -- F (TotvsPeople, LinxClientesFornec sem CNPJ) / J (TotvsLegalEntities, LinxClientesFornec com CNPJ)
    name             NVARCHAR(100) NULL,
    tax_id           NVARCHAR(14)  NULL,        -- CPF/CNPJ em claro; é daqui que sai o doc_hash de DimCustomer
    birth_date       DATE          NULL,        -- vira age_range em DimCustomer
    email            NVARCHAR(100) NULL,
    phone            NVARCHAR(20)  NULL,
    address          NVARCHAR(120) NULL,
    city             NVARCHAR(60)  NULL,
    state            CHAR(2)       NULL,        -- copiada para DimCustomer
    updated_at       DATETIME2(3)  NOT NULL,    -- integrated_or_updated_at da origem, horário de Brasília
    CONSTRAINT pk_DimCustomerIdentified PRIMARY KEY (doc_company, source_erp, customer_code)
);
```

A carga de cliente roda em duas procedures, nessa ordem (10.1, passos 5 e 6): `dw_personal_data.sp_Customer_Load`, que lê as tabelas de origem e grava a identificação em claro; e `datawarehouse.sp_DimCustomer_Load @pepper`, que lê `dw_personal_data` e grava em `DimCustomer` só `doc_hash` (`CONVERT(CHAR(64), HASHBYTES('SHA2_256', CONCAT(@pepper, tax_id)), 2)`), `doc_masked`, `person_type`, `state` e `age_range`. O pepper chega como parâmetro, fornecido pelo job; ele não existe em tabela nem em script (12.3). Nenhuma coluna desta tabela existe em `dw_landing` ou `dw_history` (5.2, regra 6). O mesmo CPF gera o mesmo `doc_hash` nos três ERPs, e é por ele que o cliente do legado Linx se liga ao cliente da Totvs. `dw_outbound.OtoCustomers` (`name`, `email`, `phone`) e `dw_outbound.LogisticsShipments` (`name`, `address`, `city`, `state`, `phone`) são as únicas views que projetam colunas desta tabela (4.2).

### 8.4 Fato de venda

```sql
CREATE TABLE datawarehouse.FactSalesItem (
    source_erp               VARCHAR(20)    NOT NULL,   -- totvs, linxmicrovix
    source_key               VARCHAR(200)   NOT NULL,   -- PK da origem com '|': Totvs doc_company|branchCode|invoiceSequence|invoiceDate|itemSequence|productCode; Linx doc_company|cnpj_emp|transacao
    doc_company              NVARCHAR(14)   NOT NULL,
    invoice_date             DATE           NOT NULL,
    store_sk                 CHAR(32)       NOT NULL,
    channel_sk               CHAR(32)       NOT NULL,
    product_sk               CHAR(32)       NOT NULL,
    customer_sk              CHAR(32)       NOT NULL,
    seller_sk                CHAR(32)       NOT NULL,
    operation_sk             CHAR(32)       NOT NULL,
    -- degeneradas da Totvs (nulas no Linx)
    branch_code              INT            NULL,
    invoice_sequence         INT            NULL,
    invoice_datetime         DATETIME       NULL,
    item_sequence            INT            NULL,       -- item da nota (referência e cor)
    product_code             INT            NULL,       -- produto do item (tamanho)
    source_order             VARCHAR(50)    NULL,       -- orderBranchCode|orderCode de TotvsInvoicesSalesOrders
    -- degeneradas do Linx (nulas na Totvs)
    store_tax_id             NVARCHAR(14)   NULL,       -- cnpj_emp
    transaction_code         INT            NULL,       -- transacao
    sale_id                  UNIQUEIDENTIFIER NULL,     -- identificador: venda a que a linha pertence (repete nos itens da mesma venda)
    -- medidas
    is_sale                  BIT            NOT NULL,   -- tipo do pedido é venda (LJ e RC não são, 7.2)
    is_cancelled             BIT            NOT NULL,
    is_invoiced              BIT            NOT NULL,   -- nota emitida e não cancelada: entra no faturado (7.3)
    is_realized_operation    BIT            NOT NULL,
    quantity                 DECIMAL(18,6)  NOT NULL,
    gross_amount             DECIMAL(18,6)  NOT NULL,
    discount_amount          DECIMAL(18,6)  NOT NULL,
    freight_amount           DECIMAL(18,6)  NOT NULL,   -- rateado do item pelos produtos (Totvs)
    tax_amount               DECIMAL(18,6)  NULL,       -- TotvsInvoicesItemsTaxes rateado; LinxMovimento valor_icms + pis + cofins + ipi
    invoiced_amount          DECIMAL(18,6)  NOT NULL,   -- líquido com sinal: operationType Input (entrada, devolução) negativo
    list_price               DECIMAL(18,6)  NULL,       -- dw_history.TotvsPrices vigente na data: código 1 VENDA LOJA no canal loja, 4 VENDA ECOMMERCE no e-commerce (8.7); Linx preco_tabela_epoca
    original_price           DECIMAL(18,6)  NULL,       -- código 5 VENDA ORIGINAL vigente na data: preço antes da remarcação
    promotional_price        DECIMAL(18,6)  NULL,       -- código 6 VENDA PROMOCIONAL vigente na data, se houver
    unit_cost                DECIMAL(18,6)  NULL,       -- dw_history.TotvsCosts vigente na data, o costCode com papel margin_cost em ValueCodes: 2 ULTIMA COMPRA (pergunta 33); Linx preco_custo
    loaded_at                DATETIME2(3)   NOT NULL CONSTRAINT df_FactSalesItem_loaded DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_FactSalesItem PRIMARY KEY (source_erp, source_key)
);
CREATE INDEX ix_FactSalesItem_invoice_date_store ON datawarehouse.FactSalesItem (invoice_date, store_sk);
```

- O grão é a linha de produto vendido (7.2): na Totvs, item × produto de `TotvsInvoicesItemsProducts`; no Linx, a linha de `LinxMovimento`. A PK (`source_erp`, `source_key`) garante o grão nos dois ERPs sem que as colunas de um precisem existir no outro; as colunas degeneradas ficam para rastrear a origem. Índice columnstore ou rowstore: decidir com a carga de teste.
- `source_key` é montada com `CONCAT_WS('|', ...)` nas procedures, com as datas em `yyyyMMdd`, para ser determinística.
- FKs para as dimensões: validar o custo na carga. Se pesarem, a integridade fica nas checagens da seção 11.
- `FactOrderItem` segue o mesmo padrão: (`source_erp`, `source_key`) na PK, degeneradas de cada origem (`order_code` e `product_code` na Totvs; `order_id` e `order_item_id` no Microvix B2C e no Linx Commerce) e, para os consumidores (4.2), as partes do número do pedido: `order_series` (`MI-`, `OA-`, `MX-`), `order_type` (`VD`, `LJ`, `VF`, `RC`, `RV`, `ST`, `PDV` ou nulo), `branch_code`, `order_code` e `erp_suffix` (`T` para Totvs).

### 8.5 Fato de estoque mensal

```sql
CREATE TABLE datawarehouse.FactMonthlyStock (
    position_date            DATE           NOT NULL,   -- último dia do mês
    store_sk                 CHAR(32)       NOT NULL,
    product_sk               CHAR(32)       NOT NULL,
    stock_code               INT            NOT NULL,   -- depósito da Totvs; o vendável é o da pergunta 33
    doc_company              NVARCHAR(14)   NOT NULL,   -- degeneradas, para rastrear a origem
    branch_code              INT            NOT NULL,
    product_code             INT            NOT NULL,
    stock_description        NVARCHAR(60)   NULL,
    quantity                 DECIMAL(18,6)  NOT NULL,   -- stock
    reserved_by_order        DECIMAL(18,6)  NOT NULL,   -- salesOrder
    incoming_from_purchase   DECIMAL(18,6)  NOT NULL,   -- purchaseOrder
    unit_cost                DECIMAL(18,6)  NULL,       -- dw_history.TotvsCosts vigente no fim de position_date, código com papel margin_cost (2 ULTIMA COMPRA)
    loaded_at                DATETIME2(3)   NOT NULL CONSTRAINT df_FactMonthlyStock_loaded DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_FactMonthlyStock PRIMARY KEY (position_date, store_sk, product_sk, stock_code)
);
```

- Carregado por `datawarehouse.sp_FactMonthlyStock_Load @position_date` (sem data, o último dia do mês anterior), que lê a versão de `dw_history.TotvsBalances` vigente no fim do dia: `valid_from < dia seguinte` e `valid_to` nulo ou a partir do dia seguinte. Loja e produto são os da época (SCD2). Reexecutar uma data substitui as linhas dela.
- Posições diárias: `dw_intermediate.fn_StockPosition(@position_date)`, função de tabela com o mesmo critério e as mesmas colunas, não materializada; `sp_FactMonthlyStock_Load` usa a mesma função. Uma view não recebe a data como parâmetro, por isso é função.

### 8.6 Views de saída

```sql
-- dw_outbound.WmsOrders: o que o módulo de logística envia ao WMS. Formato do número como o WMS espera (MI-VD12345T).
CREATE OR ALTER VIEW dw_outbound.WmsOrders AS
SELECT o.source_erp,
       o.source_key,
       CONCAT(o.order_series, o.order_type, o.order_code, o.erp_suffix)  AS document_number,   -- MI-VD12345T; e-commerce sem tipo: MI-1234567T
       o.order_series,
       o.order_type,
       o.branch_code,
       o.order_code,
       s.cnpj                                                            AS branch_tax_id,
       c.doc_hash                                                        AS customer_hash,
       o.order_date,
       o.status,
       o.updated_at                                                      -- maior integrated_or_updated_at das origens da linha: watermark do consumidor
  FROM dw_intermediate.vw_OrderItem AS o
  JOIN datawarehouse.DimStore    AS s ON s.sk = o.store_sk
  JOIN datawarehouse.DimCustomer AS c ON c.sk = o.customer_sk
 WHERE o.order_type IN ('VD', 'LJ', 'VF', 'RC') OR o.order_type IS NULL;   -- os tipos que o WMS recebe (docs/regras-prefixo-pedidos.md, §2)
```

- Uma view por consumidor e entidade (4.2). O módulo lê `WHERE updated_at > @watermark - 3 dias`, grava o que enviou e avança o próprio watermark em `dw_control.Watermark` (`source = 'wms'`, `entity = 'WmsOrders'`).
- `dw_outbound.OtoOrders` monta `MI-VD-1-12345` (com hífens e sem `T`, como o Oto espera) a partir das mesmas partes; `dw_outbound.OtoCustomers` junta `DimCustomerIdentified` e projeta nome, e-mail e telefone.
- As views da logística (`dw_outbound.LogisticsShipments`, lida por Jadlog, TotalExpress, Stone e FlashCourier) seguem o ciclo curto de 3 minutos (10.3): leem `dw_intermediate.vw_OrderParts` (view sobre `dw_landing.TotvsSaleOrders`, `TotvsInvoices` e `TotvsInvoicesSalesOrders`, com `dw_reference.ChannelRules` aplicada inline), `dw_personal_data.OrderShippingAddress` (endereço de entrega) e `DimCustomerIdentified` (telefone e e-mail). Na fase 1, `WmsOrders` também lê `vw_OrderParts`, para refletir o pedido sem esperar o passo 6; o exemplo acima mostra o formato do número. É a segunda view com dado pessoal em claro. O exemplo acima, `WmsOrders`, é de um consumidor do ciclo horário.

### 8.7 Códigos de valor

`dw_reference.ValueCodes` guarda o catálogo de tipos de valor da Totvs (tela PRDFL003, "Lista/Manutenção Tipo de Valor", levantado em 29/10/2026) e o papel de cada código no DW. As procedures de carga leem o papel, nunca o código fixo: trocar o custo da margem é um `UPDATE` nesta tabela, sem mexer em procedure.

```sql
CREATE TABLE dw_reference.ValueCodes (
    value_type      CHAR(1)       NOT NULL,   -- C custo (costCode), P preço (priceCode), S saldo (stockCode)
    code            INT           NOT NULL,
    name            NVARCHAR(60)  NOT NULL,   -- descrição da PRDFL003 (custo e preço) ou stockDescription de TotvsBalances (saldo)
    currency_code   INT           NULL,       -- 1 REAL, 2 DOLAR, 3 EURO
    role            VARCHAR(30)   NULL,       -- papel no DW; nulo para os códigos que o DW só guarda no histórico
    CONSTRAINT pk_ValueCodes PRIMARY KEY (value_type, code),
    CONSTRAINT uq_ValueCodes_role UNIQUE (role)   -- um código por papel
);

INSERT INTO dw_reference.ValueCodes (value_type, code, name, currency_code, role) VALUES
('C', 1, 'CUSTO ICMS',        1, NULL),
('C', 2, 'ULTIMA COMPRA',     1, 'margin_cost'),           -- custo da margem (pergunta 33, decidida): unit_cost de FactSalesItem e FactMonthlyStock
('C', 3, 'MEDIO S/ IMPOSTO',  1, NULL),
('C', 4, 'MEDIO C/ IMPOSTO',  1, NULL),
('C', 5, 'REMESSA',           1, NULL),
('C', 6, 'SEM IMPOSTO',       1, NULL),
('C', 7, 'CUSTO FINAL',       1, NULL),                    -- criado em 12/02/2026
('C', 8, 'CUSTO ICMS',        1, NULL),                    -- mesma descrição do código 1; criado em 08/09/2026
('P', 1, 'VENDA LOJA',        1, 'list_price_store'),
('P', 2, 'VENDA DOLAR',       2, NULL),
('P', 3, 'VENDA EURO',        3, NULL),
('P', 4, 'VENDA ECOMMERCE',   1, 'list_price_ecommerce'),
('P', 5, 'VENDA ORIGINAL',    1, 'original_price'),
('P', 6, 'VENDA PROMOCIONAL', 1, 'promotional_price');
-- 'S' (stockCode): não está na PRDFL003. Preencher depois da primeira carga de TotvsBalances, com stockDescription,
-- e marcar o depósito vendável com o papel sellable_stock (pergunta 33).
```

- Os papéis `margin_cost` (2 `ULTIMA COMPRA`, decidido), `list_price_store`, `list_price_ecommerce`, `original_price` e `promotional_price` estão atribuídos; `sellable_stock` fica nulo até a pergunta 33 fechar o depósito vendável.
- Dois códigos de custo têm a mesma descrição (`CUSTO ICMS`, códigos 1 e 8). O DW mapeia por código, não por nome, e a carga de teste confere se os dois estão preenchidos em `TotvsCosts`.
- Os códigos em dólar e euro (preço 2 e 3) ficam só no histórico, sem papel: o DW mede em real.
