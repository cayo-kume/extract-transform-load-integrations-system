# DW do NEWBLOOMERS · 04. Modelo dimensional e histórico

> Guia 4 de 11 do guia do DW, versão 5. Seções deste arquivo: 6 e 7. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 6. Histórico (snapshots)

Exemplo para `totvs.TotvsInvoices`. A `dw_landing` guarda o estado atual, com `row_hash` (SHA-256 das colunas de negócio). O snapshot abre e fecha versões. A chave é a PK da origem. O snapshot é incremental: compara só as linhas gravadas na landing desde a execução anterior, controladas por `carregado_em` e por um watermark próprio em `dw_control.watermark` (fonte `dw_history`, entidade `totvs_invoices`), e não a tabela inteira.

```sql
-- dw_landing.totvs_invoices: espelho de totvs.TotvsInvoices, com as colunas que o DW usa
CREATE TABLE dw_landing.totvs_invoices (
    doc_company           NVARCHAR(14)  NOT NULL,
    branchCode            INT           NOT NULL,
    invoiceSequence       INT           NOT NULL,
    invoiceDate           DATETIME      NOT NULL,
    invoiceStatus         NVARCHAR(20)  NULL,
    eletronicAccessKey    NVARCHAR(50)  NULL,
    operationCode         INT           NULL,
    operatioName          NVARCHAR(100) NULL,
    operationType         NVARCHAR(20)  NULL,
    issueDate             DATETIME      NULL,
    personCode            INT           NULL,
    totalValue            DECIMAL(18,6) NULL,
    shippingValue         DECIMAL(18,6) NULL,
    origin                NVARCHAR(20)  NULL,
    origem_atualizado_em  DATETIME      NOT NULL,   -- maxChangeFilterDate; se vier nulo, integrated_or_updated_at
    row_hash              BINARY(32)    NOT NULL,   -- SHA-256 das colunas de negócio
    -- gravado em todo INSERT e em todo UPDATE com mudança de hash; o snapshot lê só as linhas acima do watermark dele
    carregado_em          DATETIME2(3)  NOT NULL CONSTRAINT df_landing_inv_load DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_landing_totvs_invoices PRIMARY KEY (doc_company, branchCode, invoiceSequence, invoiceDate)
);
CREATE INDEX ix_landing_totvs_invoices_carregado_em ON dw_landing.totvs_invoices (carregado_em);   -- filtro do snapshot incremental
GO

-- dw_history.totvs_invoices: uma linha por versão
CREATE TABLE dw_history.totvs_invoices (
    doc_company      NVARCHAR(14)  NOT NULL,
    branchCode       INT           NOT NULL,
    invoiceSequence  INT           NOT NULL,
    invoiceDate      DATETIME      NOT NULL,
    valid_from       DATETIME2(3)  NOT NULL,   -- origem_atualizado_em da versão
    valid_to         DATETIME2(3)  NULL,       -- origem_atualizado_em da versão seguinte
    is_current       BIT           NOT NULL,
    row_hash         BINARY(32)    NOT NULL,
    invoiceStatus    NVARCHAR(20)  NULL,
    totalValue       DECIMAL(18,6) NULL,       -- demais colunas de negócio
    CONSTRAINT pk_history_totvs_invoices PRIMARY KEY (doc_company, branchCode, invoiceSequence, invoiceDate, valid_from)
);
GO

-- dw_history.sp_totvs_invoices_snapshot: chamada pelo job, depois da carga da landing.
-- Incremental: só as linhas que a carga gravou na landing desde o último snapshot (carregado_em acima do watermark).
-- Na primeira execução não há watermark, então todas as linhas entram.
CREATE OR ALTER PROCEDURE dw_history.sp_totvs_invoices_snapshot
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @desde DATETIME2(3) = ISNULL(
        (SELECT ultimo_valor FROM dw_control.watermark WHERE fonte = 'dw_history' AND entidade = 'totvs_invoices'),
        '1900-01-01');
    DECLARE @ate   DATETIME2(3) = (SELECT MAX(carregado_em) FROM dw_landing.totvs_invoices);

    IF @ate IS NULL OR @ate <= @desde
        RETURN;   -- a carga não gravou nada desde o último snapshot

    BEGIN TRANSACTION;

    -- 0. candidatas: só as linhas gravadas desde o último snapshot (a tabela temporária evita ler a landing duas vezes)
    SELECT doc_company, branchCode, invoiceSequence, invoiceDate, origem_atualizado_em, row_hash, invoiceStatus, totalValue
      INTO #candidatas
      FROM dw_landing.totvs_invoices
     WHERE carregado_em > @desde
       AND carregado_em <= @ate;

    -- 1. fecha a versão vigente das candidatas cujo hash mudou
    UPDATE h
       SET h.valid_to   = s.origem_atualizado_em,
           h.is_current = 0
      FROM dw_history.totvs_invoices AS h
      JOIN #candidatas AS s
        ON s.doc_company = h.doc_company
       AND s.branchCode = h.branchCode
       AND s.invoiceSequence = h.invoiceSequence
       AND s.invoiceDate = h.invoiceDate
     WHERE h.is_current = 1
       AND h.row_hash <> s.row_hash;

    -- 2. abre a versão nova: candidatas novas, ou as que acabaram de ser fechadas
    INSERT INTO dw_history.totvs_invoices
        (doc_company, branchCode, invoiceSequence, invoiceDate, valid_from, valid_to, is_current, row_hash, invoiceStatus, totalValue)
    SELECT s.doc_company, s.branchCode, s.invoiceSequence, s.invoiceDate,
           s.origem_atualizado_em, NULL, 1, s.row_hash, s.invoiceStatus, s.totalValue
      FROM #candidatas AS s
      LEFT JOIN dw_history.totvs_invoices AS h
        ON h.doc_company = s.doc_company
       AND h.branchCode = s.branchCode
       AND h.invoiceSequence = s.invoiceSequence
       AND h.invoiceDate = s.invoiceDate
       AND h.is_current = 1
     WHERE h.doc_company IS NULL;

    -- 3. avança o watermark na mesma transação: se o snapshot falhar, a próxima execução relê as mesmas linhas
    MERGE dw_control.watermark AS w
    USING (SELECT 'dw_history' AS fonte, 'totvs_invoices' AS entidade) AS o
       ON w.fonte = o.fonte AND w.entidade = o.entidade
    WHEN MATCHED THEN
        UPDATE SET ultimo_valor = @ate,
                   atualizado_em = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
    WHEN NOT MATCHED THEN
        INSERT (fonte, entidade, ultimo_valor) VALUES (o.fonte, o.entidade, @ate);

    COMMIT TRANSACTION;
END;
```

- O snapshot depende de `carregado_em` marcar mudança de conteúdo. Por isso a procedure de carga da landing (`dw_landing.sp_totvs_invoices_load`) grava `carregado_em` em todo INSERT e só atualiza uma linha existente quando o `row_hash` muda (`WHEN MATCHED AND destino.row_hash <> origem.row_hash`), gravando `carregado_em` nesse UPDATE. As linhas relidas pela sobreposição de 3 dias (5.2, regra 1) sem alteração não mudam de `carregado_em` e não entram no snapshot.
- O watermark do snapshot fica em `dw_control.watermark`, com fonte `dw_history` e a entidade da tabela. O `ultimo_valor` dele é o `carregado_em` da landing, não o `integrated_or_updated_at` da origem (8.1). Zerar esse watermark refaz o snapshot da tabela inteira, sem duplicar: as linhas com o mesmo hash da versão vigente não geram versão nova.
- A data da mudança é `origem_atualizado_em`. Se duas versões da mesma nota tiverem a mesma data, a PK de `dw_history` colide. A carga de teste precisa confirmar que `maxChangeFilterDate` é preenchida e muda a cada alteração; se não mudar, a PK passa a incluir `carregado_em` (seção 15).
- Para tabelas com `lastChangeDate` em vez de `maxChangeFilterDate` (como `TotvsSaleOrdersProducts`), o campo muda, e a lógica é a mesma.
- A frequência é horária para notas e pedidos e diária para cadastros (pergunta 9, 3.3).

---

## 7. Modelo dimensional

### 7.1 Dimensões

| Dimensão | Grão | Fontes | SCD | Observação |
|---|---|---|---|---|
| `dim_tempo` | dia | gerada por procedure; `dw_reference.feriados` | — | Ano, mês, semana ISO, dia útil, feriado, data em horário de Brasília. Cobre 2020 a 2035 (ajustar) |
| `dim_loja` | loja (`doc_company`, `branchCode`) | `totvs.TotvsBranches` | 2 (snapshot) | Muda de nome, marca e situação |
| `dim_produto` | SKU | `totvs.TotvsSkus`, `totvs.TotvsSkusColors` | 2 (snapshot) | Linha, coleção e situação mudam |
| `dim_marca` | marca | derivada de produto | 1 | Misha, Open Era, Margaux e outras (nomes usados no Oto deste repositório) |
| `dim_cliente` | cliente (`doc_company`, código) | `totvs.TotvsPeople`, `totvs.TotvsLegalEntities`; UF, faixa etária, hash e documento mascarado vêm de `dw_personal_data` (8.3) | 1 (pergunta 11) | Dado pessoal tratado (seção 12 e 5.2, regra 6). CPF/CNPJ como hash com pepper |
| `dim_vendedor` | vendedor | `totvs.TotvsSellers` | 1 | |
| `dim_fornecedor` | fornecedor | `totvs.TotvsLegalEntities`, `totvs.TotvsProductsSuppliers` | 1 | |
| `dim_pagamento` | forma de pagamento | `totvs.TotvsInvoicesPayments` | 1 | Colunas de forma de pagamento a confirmar na carga de teste |
| `dim_operacao` | operação da nota | `totvs.TotvsInvoices` (`operationCode`, `operatioName`, `operationType`) | 1 | Confirmada (pergunta 26) |
| `dim_canal` | canal | `dw_reference.regra_canal` (tabela carregada por script) | 1 | Regra decidida (pergunta 15): pedido de `TotvsSaleOrders` com `integrationCode` não nulo é e-commerce; nulo é loja. A tabela mapeia cada `integrationCode` para o nome do canal. A função herdada `calcula_id_canal` não é usada |

Fora do escopo desta versão, por não terem tabela de dados aqui: `dim_transportadora` (Jadlog, TotalExpress, Stone), `dim_deposito` (estoque) e a dimensão de LinxProdutos/LinxLojas/LinxVendedores (Linx).

**Membro desconhecido** em todas: uma linha com chave natural `0`, descrição "NÃO INFORMADO" e chave substituta fixa. Uma checagem alerta se a proporção de linhas nesse membro passar do limite. Chave natural nula vai para o membro desconhecido, antes de gerar a chave substituta.

**Chaves substitutas:** `sk = CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT_WS('|', <chave natural>)), 2)`. A chave natural é a PK da origem, com `doc_company`. A expressão fica inline nas procedures, e não em função escalar, por desempenho. Em dimensões SCD2, a chave inclui `valid_from`. Os fatos gravam a `sk` da versão vigente na data do evento. Reprocessar uma dimensão não muda as chaves. A v1 usava `IDENTITY`, e nesse caso um full refresh quebraria os fatos.

### 7.2 Fatos

| Fato | Grão (PK da origem) | Fontes | Carga |
|---|---|---|---|
| `fato_venda_item` | item de nota (`doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`, `sequence`) | `totvs.TotvsInvoicesItems` | MERGE pela PK, recarga completa a cada execução (pergunta 8, 9.1); otimizar depois de medir |
| `fato_pedido_item` | item de pedido (`doc_company`, `branchCode`, `orderCode`, `productCode`) | `totvs.TotvsSaleOrdersProducts` | MERGE, recarga completa |
| `fato_pagamento` | pagamento da nota (`doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`, `documentNumber`, `installment`) | `totvs.TotvsInvoicesPayments` | MERGE |
| `fato_atendimento` | ticket (`doc_company`, `id`) | `movidesk.MovideskTickets`, `MovideskTicketsActions` | Fase 2 |
| `fato_reembolso` | reembolso (`doc_company`, `id`) | `aftersale.AfterSaleRefunds` | Fase 2. Separado de atendimento porque o grão é outro |

Fora do escopo desta versão: `fato_estoque_diario` (estoque), `fato_entrega` (logística) e fatos de Linx e e-commerce. Entram quando as fontes tiverem tabelas neste repositório.

**Fonte de verdade por canal (para evitar contagem dupla):**

- **Venda faturada:** a nota do Totvs. Hoje todas as lojas faturam pelo Totvs: a venda por Linx foi descontinuada (pergunta 13). O passado das lojas que vendiam por Linx entra com o histórico de Linx Microvix e Linx Commerce (carga única, legado), ligado a `dim_loja` pelo mapa CNPJ × ERP × período em `dw_reference`, para saber quanto cada loja vendeu nos anos em que vendia pela Linx. A mesma nota nunca entra por dois ERPs. Checagem de unicidade por chave da nota.
- **Pedido de e-commerce:** pedido de `totvs.TotvsSaleOrders` com `integrationCode` não nulo (pergunta 15). O prefixo `MI-`/`OA-` do número (1.2, item 11), usado pelo Oto e pelo Stone, não é o critério do DW. Nunca somar pedido e nota do mesmo cliente como se fossem vendas separadas.

### 7.3 Métricas canônicas (uma definição, em um lugar)

Regras herdadas do legado, que ainda precisam ser confirmadas com quem usa os números. Ficam em T-SQL, como colunas nos agregados de `datamart` (ex.: `valor_faturado`, `valor_captado`). As ferramentas de BI só agregam essas colunas; nenhuma recalcula a regra.

- **Faturado, ramo Totvs:** notas com `invoiceStatus` em (Authorized, Generated, Issued, Normal) e `eletronicElectronicInvoiceStatus` em (Authorized, Generated). Valor = total menos frete. Positivo para saída; negativo para entrada e para devolução (modo de operação 3). Confirmar os nomes de status contra os valores que a Totvs devolve na carga de teste.
- **Captado (Flash):** pedidos do Totvs fora dos status PartiallyAnswered, Attended, Canceled e InComposition, pela data do pedido.
- **Realizado (`fl_operacao_realizada`):** autorizada eletronicamente, ou não cancelada e originada de PEDFP144 ou PDVFM001. Vem da lógica do legado, e a origem dos códigos (PEDFP144, PDVFM001) precisa de confirmação.
- **Faturado, ramo Linx (histórico, legado):** `DESC_SEFAZ_SITUACAO = 'Autorizada'` e `CANCELADO = 'N'`, para quando o histórico Linx entrar (pergunta 13). O Flash usa filtros de status diferentes para cada ERP, o que pode explicar parte de uma divergência futura.

⚠️ Antes de publicar qualquer total, as definições precisam ser reconciliadas com o DW atual e aprovadas por quem usa os números.

### 7.4 Fora do escopo desta versão: estoque, logística e pagamentos de outros canais

Estoque, logística e pagamentos de outros canais não têm tabelas de dados neste repositório. Quando o módulo correspondente criar as tabelas, a modelagem entra no mesmo padrão. O volume dessas fontes precisa ser medido na carga de teste do banco zerado antes de qualquer decisão sobre retenção ou granularidade (perguntas 10 e 14).
