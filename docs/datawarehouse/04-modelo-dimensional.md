# DW do NEWBLOOMERS · 04. Modelo dimensional e histórico

> Guia 4 de 12 do guia do DW, versão 7. Seções deste arquivo: 6 e 7. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 6. Histórico (snapshots)

Exemplo para `totvs.TotvsInvoices`. A `dw_landing` guarda o estado atual, com `row_hash` (SHA-256 das colunas de negócio). O snapshot abre e fecha versões. A chave é a PK da origem. O snapshot é incremental: compara só as linhas gravadas na landing desde a execução anterior, controladas por `loaded_at` e por um watermark próprio em `dw_control.Watermark` (`source = 'dw_history'`, `entity = 'TotvsInvoices'`), e não a tabela inteira. O mesmo mecanismo vale para saldos, custos e preços (`TotvsBalances`, `TotvsCosts`, `TotvsPrices`), que são a única forma de ter histórico de estoque e de valores (7.4).

```sql
-- dw_landing.TotvsInvoices: espelho de totvs.TotvsInvoices, com as colunas que o DW usa (nomes da origem)
CREATE TABLE dw_landing.TotvsInvoices (
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
    source_updated_at     DATETIME      NOT NULL,   -- maxChangeFilterDate; se vier nulo, integrated_or_updated_at
    row_hash              BINARY(32)    NOT NULL,   -- SHA-256 das colunas de negócio
    -- gravado em todo INSERT e em todo UPDATE com mudança de hash; o snapshot lê só as linhas acima do watermark dele
    loaded_at             DATETIME2(3)  NOT NULL CONSTRAINT df_landing_TotvsInvoices_loaded DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_landing_TotvsInvoices PRIMARY KEY (doc_company, branchCode, invoiceSequence, invoiceDate)
);
CREATE INDEX ix_landing_TotvsInvoices_loaded_at ON dw_landing.TotvsInvoices (loaded_at);   -- filtro do snapshot incremental
GO

-- dw_history.TotvsInvoices: uma linha por versão
CREATE TABLE dw_history.TotvsInvoices (
    doc_company      NVARCHAR(14)  NOT NULL,
    branchCode       INT           NOT NULL,
    invoiceSequence  INT           NOT NULL,
    invoiceDate      DATETIME      NOT NULL,
    valid_from       DATETIME2(3)  NOT NULL,   -- source_updated_at da versão
    valid_to         DATETIME2(3)  NULL,       -- source_updated_at da versão seguinte
    is_current       BIT           NOT NULL,
    row_hash         BINARY(32)    NOT NULL,
    invoiceStatus    NVARCHAR(20)  NULL,
    totalValue       DECIMAL(18,6) NULL,       -- demais colunas de negócio
    CONSTRAINT pk_history_TotvsInvoices PRIMARY KEY (doc_company, branchCode, invoiceSequence, invoiceDate, valid_from)
);
GO

-- dw_history.sp_TotvsInvoices_Snapshot: chamada pelo job, depois da carga da landing.
-- Incremental: só as linhas que a carga gravou na landing desde o último snapshot (loaded_at acima do watermark).
-- Na primeira execução não há watermark, então todas as linhas entram.
CREATE OR ALTER PROCEDURE dw_history.sp_TotvsInvoices_Snapshot
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @since DATETIME2(3) = ISNULL(
        (SELECT last_value FROM dw_control.Watermark WHERE source = 'dw_history' AND entity = 'TotvsInvoices'),
        '1900-01-01');
    DECLARE @until DATETIME2(3) = (SELECT MAX(loaded_at) FROM dw_landing.TotvsInvoices);

    IF @until IS NULL OR @until <= @since
        RETURN;   -- a carga não gravou nada desde o último snapshot

    BEGIN TRANSACTION;

    -- 0. candidatas: só as linhas gravadas desde o último snapshot (a tabela temporária evita ler a landing duas vezes)
    SELECT doc_company, branchCode, invoiceSequence, invoiceDate, source_updated_at, row_hash, invoiceStatus, totalValue
      INTO #candidates
      FROM dw_landing.TotvsInvoices
     WHERE loaded_at > @since
       AND loaded_at <= @until;

    -- 1. fecha a versão vigente das candidatas cujo hash mudou
    UPDATE h
       SET h.valid_to   = s.source_updated_at,
           h.is_current = 0
      FROM dw_history.TotvsInvoices AS h
      JOIN #candidates AS s
        ON s.doc_company = h.doc_company
       AND s.branchCode = h.branchCode
       AND s.invoiceSequence = h.invoiceSequence
       AND s.invoiceDate = h.invoiceDate
     WHERE h.is_current = 1
       AND h.row_hash <> s.row_hash;

    -- 2. abre a versão nova: candidatas novas, ou as que acabaram de ser fechadas
    INSERT INTO dw_history.TotvsInvoices
        (doc_company, branchCode, invoiceSequence, invoiceDate, valid_from, valid_to, is_current, row_hash, invoiceStatus, totalValue)
    SELECT s.doc_company, s.branchCode, s.invoiceSequence, s.invoiceDate,
           s.source_updated_at, NULL, 1, s.row_hash, s.invoiceStatus, s.totalValue
      FROM #candidates AS s
      LEFT JOIN dw_history.TotvsInvoices AS h
        ON h.doc_company = s.doc_company
       AND h.branchCode = s.branchCode
       AND h.invoiceSequence = s.invoiceSequence
       AND h.invoiceDate = s.invoiceDate
       AND h.is_current = 1
     WHERE h.doc_company IS NULL;

    -- 3. avança o watermark na mesma transação: se o snapshot falhar, a próxima execução relê as mesmas linhas
    MERGE dw_control.Watermark AS w
    USING (SELECT 'dw_history' AS source, 'TotvsInvoices' AS entity) AS o
       ON w.source = o.source AND w.entity = o.entity
    WHEN MATCHED THEN
        UPDATE SET last_value = @until,
                   updated_at = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
    WHEN NOT MATCHED THEN
        INSERT (source, entity, last_value) VALUES (o.source, o.entity, @until);

    COMMIT TRANSACTION;
END;
```

- O snapshot depende de `loaded_at` marcar mudança de conteúdo. Por isso a procedure de carga da landing (`dw_landing.sp_TotvsInvoices_Load`) grava `loaded_at` em todo INSERT e só atualiza uma linha existente quando o `row_hash` muda (`WHEN MATCHED AND target.row_hash <> source.row_hash`), gravando `loaded_at` nesse UPDATE. As linhas relidas pela sobreposição de 3 dias (5.2, regra 1) sem alteração não mudam de `loaded_at` e não entram no snapshot.
- O watermark do snapshot fica em `dw_control.Watermark`, com `source = 'dw_history'` e a entidade da tabela. O `last_value` dele é o `loaded_at` da landing, não o `integrated_or_updated_at` da origem (8.1). Zerar esse watermark refaz o snapshot da tabela inteira, sem duplicar: as linhas com o mesmo hash da versão vigente não geram versão nova.
- A data da mudança é `source_updated_at`. Se duas versões da mesma nota tiverem a mesma data, a PK de `dw_history` colide. A carga de teste precisa confirmar que `maxChangeFilterDate` é preenchida e muda a cada alteração; se não mudar, a PK passa a incluir `loaded_at` (seção 15).
- Para tabelas com `lastChangeDate` em vez de `maxChangeFilterDate` (como `TotvsSaleOrdersProducts`), o campo muda, e a lógica é a mesma.
- Saldos, custos e preços mudam a cada venda e a cada reajuste. O snapshot deles é diário, no fim do dia, para gerar uma versão por dia em que houve mudança, e não uma por venda (7.4, pergunta 10). O das tabelas Linx roda uma vez, na carga do legado (7.5).
- A frequência é horária para notas e pedidos e diária para cadastros, saldos, custos e preços (pergunta 9, 3.3).

---

## 7. Modelo dimensional

Os fatos e as dimensões que recebem mais de um ERP levam `source_erp` (`totvs`, `linxmicrovix`, `linxcommerce`) na chave natural, porque os códigos dos ERPs colidem entre si (um `cod_cliente` do Linx não é o `code` da Totvs). A loja é a exceção: os dois ERPs falam do mesmo CNPJ, então a loja Linx é ligada à linha da `DimStore` pelo CNPJ (7.5). O produto Linx tem de/para por referência, cor e tamanho (pergunta 34).

### 7.1 Dimensões

| Dimensão | Grão | Fontes | SCD | Observação |
|---|---|---|---|---|
| `DimDate` | dia | gerada por procedure; `dw_reference.Holidays` | — | Ano, mês, semana ISO, dia útil, feriado, data em horário de Brasília. Cobre 2010 a 2035, para alcançar o legado Linx (ajustar) |
| `DimStore` | loja (`doc_company`, `branchCode`) | `totvs.TotvsBranches` (`cnpj`, `description`, `fantasyName`, `branchGroupName`); `linxmicrovix.LinxLojas` só para a loja sem CNPJ na Totvs | 2 (snapshot) | Muda de nome, marca e situação. O CNPJ é o atributo que liga o legado Linx (7.5); a raiz do CNPJ define a série do pedido (`MI-`, `OA-`), como em `docs/regras-prefixo-pedidos.md` |
| `DimProduct` | produto (`doc_company`, `source_erp`, código do produto) | Totvs: `TotvsSkusColorsProducts` (`code` = `productCode` das notas, pedidos, saldos, custos e preços; tamanho, `isActive`, `isBlocked`, datas de venda), com a hierarquia referência (`TotvsSkus`: `referenceCode`, `name`, `gridCode`) → cor (`TotvsSkusColors`) → tamanho (`size`, `TotvsGridsSizes`) e a categoria (`TotvsSkusReferenceCategories`, `TotvsCategories`, com `parentCategoryCode`). Linx: `LinxProdutos` (`cod_produto`, `referencia`, `desc_cor`, `desc_tamanho`, `desc_linha`, `desc_marca`, `desc_colecao`, `desativado`) | 2 (snapshot) | Linha, coleção, categoria e situação mudam. O produto Linx recebe `matched_product_sk` quando `referencia`, `desc_cor` e `desc_tamanho` casam com `referenceCode`, cor e `size` da Totvs; sem casamento, fica membro próprio (pergunta 34, 7.5) |
| `DimBrand` | marca | derivada de produto: Totvs, a matriz dona da referência (raiz do CNPJ `38367316` Misha, `42538267` Open Era) ou uma classificação, a confirmar na carga de teste; Linx, `LinxProdutos.desc_marca` | 1 | Misha, Open Era, Margaux e outras (nomes usados no Oto deste repositório e em `docs/regras-prefixo-pedidos.md`) |
| `DimCustomer` | cliente (`doc_company`, `source_erp`, código) | Totvs: `TotvsPeople`, `TotvsLegalEntities`; Linx: `LinxClientesFornec`, `LinxCommerceCustomers`. UF, faixa etária, hash e documento mascarado vêm de `dw_personal_data` (8.3) | 1 (pergunta 11) | Dado pessoal tratado (seção 12 e 5.2, regra 6). O `doc_hash` é o mesmo nos dois ERPs para o mesmo CPF, então liga o cliente Linx ao cliente Totvs sem expor o documento |
| `DimSeller` | vendedor (`doc_company`, `source_erp`, código) | Totvs: `TotvsSellers`; Linx: `LinxVendedores` (`cod_vendedor`, `nome_vendedor`, `tipo_vendedor`, `ativo`) | 1 | Membros próprios por ERP; a loja do vendedor Linx vem de `cnpj_emp` |
| `DimSupplier` | fornecedor | `totvs.TotvsProductsSuppliers`; desde a fase 4, também os fornecedores de `TotvsPurchaseOrders` e `TotvsDuplicates` | 1 | O nome vem de `TotvsProductsSuppliers` e, na falta, do pedido de compra |
| `DimPaymentMethod` | forma de pagamento | Totvs: `TotvsInvoicesPayments`; Linx: as colunas `forma_*` e `total_*` de `LinxMovimento` (dinheiro, cheque, cartão, crediário, convênio, pix, depósito), viradas em linhas | 1 | Colunas de forma de pagamento da Totvs a confirmar na carga de teste |
| `DimOperation` | operação | Totvs: `TotvsInvoices` (`operationCode`, `operatioName`, `operationType`); Linx: `LinxMovimento` (`cod_natureza_operacao`, `natureza_operacao`, `operacao`, `tipo_transacao`) | 1 | Confirmada (pergunta 26). `source_erp` na chave |
| `DimChannel` | canal × tipo de pedido | `dw_reference.ChannelRules` (tabela carregada por script) | 1 | Canal (pergunta 15): na Totvs, pedido com `integrationCode` não nulo é e-commerce, nulo é loja; a nota herda o canal do pedido que fatura (`TotvsInvoicesSalesOrders`), e nota sem pedido é loja. No Linx, `LinxMovimento` é loja, `LinxB2CPedidos` e `LinxCommerceOrders` são e-commerce (`WebSiteName` e `MarketPlaceBrand` como subcanal). Tipo do pedido, pelas regras de `docs/regras-prefixo-pedidos.md`: `VD` venda direta (`representativeName LIKE '%VENDA DIRETA%' OR '%VD%'`), `LJ` reposição de lojas (`'%REPOSICAO LOJAS%'`), `VF` venda fácil ou prateleira infinita (`operationName LIKE '%VENDA PRATELEIRA%'`), `RC` recebimento, `RV` reversa, `ST` e-commerce, `PDV` nota de loja (`inclusionComponentCode LIKE '%PDVFM%'`). `LJ` e `RC` não são venda (7.2). A função herdada `calcula_id_canal` não é usada |

Fase 4 (7.6): `DimSalesRepresentative` (`TotvsRepresentatives`), `DimRegion` (`TotvsOperationalAreas`) e `DimPromotion` (`TotvsPromotions`). Fora do escopo, por não terem tabela de dados aqui: `DimCarrier` (Jadlog, TotalExpress, Stone) e `DimWarehouse` (os depósitos da Totvs são o `stockCode` de `TotvsBalances`, tratado em 7.4).

**Membro desconhecido** em todas: uma linha com chave natural `0`, descrição "NOT INFORMED" e chave substituta fixa. Uma checagem alerta se a proporção de linhas nesse membro passar do limite. Chave natural nula vai para o membro desconhecido, antes de gerar a chave substituta.

**Chaves substitutas:** `sk = CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT_WS('|', <chave natural>)), 2)`. A chave natural é a PK da origem, com `doc_company` e, onde há mais de um ERP, `source_erp`. A expressão fica inline nas procedures, e não em função escalar, por desempenho. Em dimensões SCD2, a chave inclui `valid_from`. Os fatos gravam a `sk` da versão vigente na data do evento. Reprocessar uma dimensão não muda as chaves. A v1 usava `IDENTITY`, e nesse caso um full refresh quebraria os fatos.

### 7.2 Fatos

| Fato | Grão | Fontes | Carga |
|---|---|---|---|
| `FactSalesItem` | linha de produto vendido: Totvs, item × produto da nota (`TotvsInvoicesItemsProducts`: `doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`, `itemSequence`, `productCode`); Linx, linha de `LinxMovimento` (`cnpj_emp`, `transacao`). Chave do fato: (`source_erp`, `source_key`), 8.4 | Totvs: `TotvsInvoicesItemsProducts` (quantidade e valores do produto), `TotvsInvoicesItems` (CFOP, NCM, frete, seguro e acréscimo do item, rateados pelos produtos), `TotvsInvoicesItemsTaxes` (impostos), `TotvsInvoices` (status, operação, cliente, data), `TotvsInvoicesSalesOrders` (canal e tipo do pedido). Linx: `LinxMovimento`, `LinxMovimentoPrincipal` (`estornada`), `LinxMovimentoDevolucoesItens` | MERGE pela chave, recarga completa a cada execução (pergunta 8, 9.1); otimizar depois de medir. Linx: carga única (7.5) |
| `FactOrderItem` | linha de produto do pedido: Totvs (`doc_company`, `branchCode`, `orderCode`, `productCode`); Linx Microvix B2C (`id_pedido_item`); Linx Commerce (`OrderID`, `OrderItemID`). Chave (`source_erp`, `source_key`) | `totvs.TotvsSaleOrdersProducts` e `TotvsSaleOrders` (canal, tipo e partes do número do pedido, 4.2); `linxmicrovix.LinxB2CPedidosItens` e `LinxB2CPedidos`; `linxcommerce.LinxCommerceOrdersItems` e `LinxCommerceOrders` | MERGE, recarga completa. Linx: carga única. É a origem de `dw_outbound.OtoOrders` e `WmsOrders` |
| `FactInvoicePayment` | pagamento da nota (`doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`, `documentNumber`, `installment`) | `totvs.TotvsInvoicesPayments`; Linx, só se o fechamento exigir: `LinxMovimentoPlanos` | MERGE |
| `FactMonthlyStock` | loja × produto × `stockCode` × fim de mês (7.4) | `dw_history.TotvsBalances` | Fase 2. Posição do fim do último dia de cada mês, gravada pelo ciclo diário do dia 1; posições diárias por `dw_intermediate.fn_StockPosition` sobre o histórico (8.5) |
| `FactTicket` | ticket (`doc_company`, `ticket_id`) | `movidesk.MovideskTickets`, `MovideskTicketsActions` (ações públicas e internas), `MovideskTicketsClients` (cliente principal: o de menor id); origem e tipo pelas tabelas de domínio do módulo | Fase 2. Recarga completa. Status, categoria, equipe, níveis de serviço, SLA de resposta e de solução cumpridos, tempos. Cliente pelo `doc_hash` do documento do Movidesk: `customer_sk` é o cliente da Totvs com o mesmo hash, e `customer_doc_hash` fica no fato para quem não tem cadastro na Totvs |
| `FactRefund` | reembolso (`doc_company`, `refund_id`) | `aftersale.AfterSaleRefunds`, `AfterSaleRefundsProducts` (linhas, quantidades e o motivo do produto de menor id como motivo principal), `AfterSaleReverses` (marca, tipo, frete pelo cliente) | Fase 2. Separado de `FactTicket` porque o grão é outro. Recarga completa. Valores pedidos e recebidos, número do pedido do e-commerce. Cliente pelo `doc_hash`, como em `FactTicket` |

Fase 4 (7.6): `FactPurchaseOrderItem`, `FactAccountsPayable`, `FactCommission`, `FactStoreReception` e `FactApiRequest`. Fora do escopo desta versão: `FactShipment` (logística) e pagamentos de outros canais (Pagarme). Entram quando as fontes tiverem tabelas neste repositório.

**Grão do fato de venda, item × produto:** na Totvs, o item da nota (`TotvsInvoicesItems`, uma referência e cor) se abre em produtos (`TotvsInvoicesItemsProducts`, um por tamanho), cada um com quantidade e valores próprios (1.2, item 13). O fato usa o produto, que é o grão das vendas, dos saldos, dos custos e dos preços. O que só existe no item (frete, seguro, acréscimo, impostos de `TotvsInvoicesItemsTaxes`) é rateado pelos produtos do item, proporcional ao valor líquido. Item sem produto (serviço ou item sem grade) entra com `productCode` no membro desconhecido. Checagem: a soma dos produtos de um item bate com o valor do item.

**Fonte de verdade por canal (para evitar contagem dupla):**

- **Venda faturada:** a nota do Totvs. Hoje todas as lojas faturam pelo Totvs: a venda por Linx foi descontinuada (pergunta 13). O passado das lojas que vendiam por Linx entra pelo legado (7.5), ligado a `DimStore` pelo CNPJ, para saber quanto cada loja vendeu nos anos em que vendia pela Linx. A mesma nota nunca entra por dois ERPs: um CNPJ tem Linx até a data da migração e Totvs depois, e a checagem de unicidade por chave da nota pega sobreposição.
- **Pedido de e-commerce:** pedido de `totvs.TotvsSaleOrders` com `integrationCode` não nulo (pergunta 15). A nota que fatura esse pedido recebe o mesmo canal por `TotvsInvoicesSalesOrders` (1.2, item 14). O prefixo `MI-`/`OA-` do número (1.2, item 11), usado pelo Oto e pelo Stone, não é o critério do DW: o DW o produz, pelas partes do número, para os consumidores (4.2). Nunca somar pedido e nota do mesmo cliente como se fossem vendas separadas: o pedido mede captado; a nota mede faturado.
- **Reposição de lojas (`LJ`) e recebimento (`RC`) não são venda:** são transferência entre o CD e a loja e compra do fornecedor. Entram em `FactOrderItem` com o tipo em `DimChannel`, e `datamart.DailySales` os exclui de captado e faturado.

### 7.3 Métricas canônicas (uma definição, em um lugar)

Regras herdadas do legado, que ainda precisam ser confirmadas com quem usa os números. Ficam em T-SQL, como colunas nos agregados de `datamart` (ex.: `invoiced_amount`, `captured_amount`). As ferramentas de BI só agregam essas colunas; nenhuma recalcula a regra.

- **Faturado, ramo Totvs (`invoiced_amount`):** notas com `invoiceStatus` em (Authorized, Generated, Issued, Normal) e `eletronicElectronicInvoiceStatus` em (Authorized, Generated). Valor = total menos frete. Positivo para saída; negativo para entrada e para devolução (modo de operação 3). Confirmar os nomes de status contra os valores que a Totvs devolve na carga de teste.
- **Faturado, ramo Linx (legado):** linhas de `LinxMovimento` com `cancelado = 'N'` e `desc_sefaz_situacao = 'Autorizada'`, e `estornada` falso em `LinxMovimentoPrincipal`. Devolução pelas linhas de `LinxMovimentoDevolucoesItens` e pela natureza de operação. O Flash usava filtros de status diferentes para cada ERP, o que pode explicar parte de uma divergência com o passado.
- **Captado (`captured_amount`, o Flash):** pedidos do Totvs fora dos status PartiallyAnswered, Attended, Canceled e InComposition, pela data do pedido, excluídos os tipos `LJ` e `RC` (7.2).
- **Realizado (`is_realized_operation`):** autorizada eletronicamente, ou não cancelada e originada de PEDFP144 ou PDVFM001. Vem da lógica do legado, e a origem dos códigos (PEDFP144, PDVFM001) precisa de confirmação.
- **Custo e margem:** `unit_cost` da linha de venda é o custo vigente do produto na loja na data da venda, lido de `dw_history.TotvsCosts` com o `costCode` que tiver o papel `margin_cost` em `dw_reference.ValueCodes` (8.7): o código 2 `ULTIMA COMPRA` (pergunta 33, decidida). No Linx, `LinxMovimento.preco_custo`. Margem = líquido menos custo × quantidade. A medida se chama `unit_cost`, e não "custo médio", porque o custo escolhido é o da última compra; os outros sete tipos de custo ficam só no histórico.
- **Preço de tabela:** `list_price` da linha é o preço vigente em `dw_history.TotvsPrices` do código com papel `list_price_store` (1 `VENDA LOJA`) no canal loja e `list_price_ecommerce` (4 `VENDA ECOMMERCE`) no e-commerce; no Linx, `LinxMovimento.preco_tabela_epoca`. Desconto real = preço de tabela menos preço praticado. A linha leva também `original_price` (5 `VENDA ORIGINAL`, antes da remarcação) e `promotional_price` (6 `VENDA PROMOCIONAL`), para medir remarcação e promoção (8.4).

⚠️ Antes de publicar qualquer total, as definições precisam ser reconciliadas com o DW atual e aprovadas por quem usa os números.

### 7.4 Estoque, custos e preços (Totvs)

`totvs.TotvsBalances` guarda a posição atual de cada produto em cada loja e depósito (`stockCode`): `stock`, `salesOrder` (reservado por pedido), `purchaseOrder` (a receber), `inputTransaction`, `outputTransaction` e outras colunas de produção. `TotvsCosts` e `TotvsPrices` guardam o custo e o preço vigentes por loja, produto e código. Nenhuma das três tem histórico: cada venda ou reajuste sobrescreve a linha (1.2, item 15).

- **Histórico por mudança, não por dia (pergunta 10, decidida).** As três tabelas entram em `dw_landing` e `dw_history` como as demais (seção 6), com snapshot diário no fim do dia. Cada versão do histórico é uma posição que durou pelo menos um dia. A posição de qualquer data é lida com `valid_from <= data < valid_to`. Não é "append" nem "substituir o dia": é o mesmo SCD2 do resto do DW.
- **`FactMonthlyStock`:** uma linha por loja, produto, `stockCode` e último dia do mês, com `quantity`, `reserved_by_order`, `incoming_from_purchase` e o custo vigente, para valorizar o estoque. É o fato que o `datamart` agrega; posições diárias saem da função `dw_intermediate.fn_StockPosition`, sobre `dw_history.TotvsBalances`, quando alguém precisar (8.5).
- **Retenção (pergunta 14, decidida):** histórico desde a primeira carga, sem limite, até a medição do volume na carga de teste. O teto é uma versão por produto × loja × depósito por dia em que houve mudança: com 30 lojas e 50 mil produtos ativos, 1,5 milhão por dia; o número real é bem menor, porque só o que vendeu ou foi reajustado muda. Se a medição exigir, a retenção passa a ser por versão (apagar versões fechadas com mais de N anos), nunca o fato mensal (arquivo 12).
- **Códigos:** o módulo lê todos os `stockCode`, `costCode` e `priceCode`, e cada linha traz o nome do código (`stockDescription`, `costName`, `priceName`). O catálogo de custo e preço veio da tela PRDFL003 e está em `dw_reference.ValueCodes` (8.7): oito custos e seis preços. Os papéis de preço e o custo da margem (2 `ULTIMA COMPRA`) estão atribuídos; o depósito vendável (`stockCode`, que não está na PRDFL003) é o que resta da pergunta 33.
- **Carga inicial:** a carga dos saldos na Totvs leva cerca de 27 minutos por loja (README do módulo); o DW só lê depois que a tabela confiável estiver completa.

### 7.5 Legado Linx: Microvix e Commerce

Os dois módulos estão no padrão do repositório, mas a Newbloomers não vende mais por eles (1.2, item 7). O DW faz uma carga única do histórico e mantém o watermark, para absorver correções, sem agenda própria.

| Origem | Destino no DW | Ligação com a Totvs |
|---|---|---|
| `linxmicrovix.LinxMovimento` (linha de produto vendido), `LinxMovimentoPrincipal` (`valor_nota`, `estornada`), `LinxMovimentoDevolucoesItens` | `FactSalesItem`, `source_erp = linxmicrovix`, `source_key = doc_company|cnpj_emp|transacao` | Loja pelo CNPJ (`cnpj_emp` = `TotvsBranches.cnpj`); data por `data_documento`; produto pelo de/para (abaixo); vendedor e cliente nos membros Linx das dimensões |
| `linxmicrovix.LinxB2CPedidos`, `LinxB2CPedidosItens` | `FactOrderItem`, `source_erp = linxmicrovix`, canal e-commerce | `cod_cliente_erp` liga ao cliente Linx; `cnpj_origem_venda` liga à loja |
| `linxcommerce.LinxCommerceOrders`, `LinxCommerceOrdersItems`, `LinxCommerceOrdersInvoices` | `FactOrderItem`, `source_erp = linxcommerce`, canal e-commerce (`WebSiteName`, `MarketPlaceBrand`) | `CustomerCPF`/`CustomerCNPJ` viram `doc_hash`; a nota do pedido (`LinxCommerceOrdersInvoices`) não entra como venda faturada, para não contar duas vezes com o ERP da época |
| `linxmicrovix.LinxLojas` | `DimStore` (só a loja sem CNPJ na Totvs) e exceções do mapa em `dw_reference.StoreErpMap` | CNPJ |
| `linxmicrovix.LinxProdutos` | `DimProduct`, membros `linxmicrovix`, com `matched_product_sk` | De/para por referência, cor e tamanho (pergunta 34) |
| `linxmicrovix.LinxVendedores`, `LinxClientesFornec`; `linxcommerce.LinxCommerceCustomers` | `DimSeller` e `DimCustomer`, membros Linx; dado identificador em `dw_personal_data` (5.2, regra 6) | Cliente pelo `doc_hash` do CPF; vendedor sem ligação |

- **De/para de produto (pergunta 34, decidida):** a view `dw_intermediate.vw_ProductMatch` casa `LinxProdutos.referencia`, `desc_cor` e `desc_tamanho` com `TotvsSkus.referenceCode`, o nome da cor em `TotvsSkusColors` e o `size` de `TotvsSkusColorsProducts`, depois de normalizar os textos (maiúsculas, sem acento, sem espaço nas pontas). `sp_DimProduct_Load` grava o resultado em `DimProduct.matched_product_sk` do membro Linx, apontando para a versão vigente do produto Totvs, a cada execução; sem casamento, o membro Linx fica sem ligação, e a comparação por produto entre os anos usa `COALESCE(matched_product_sk, sk)`. Uma checagem mede a proporção de produtos Linx casados, por marca; a meta é definida depois da primeira execução. Os pares que o texto não resolver podem ser corrigidos em `dw_reference.ProductErpMap`, carregada por script, que tem prioridade sobre o texto. Com mais de um produto Totvs igual, vale o da mesma empresa e, depois, o de menor empresa, matriz e código; com o mesmo código Linx em mais de um portal, vale o de menor portal.
- **Período por loja:** a data de corte de cada CNPJ é a última venda Linx autorizada e a primeira nota Totvs. A checagem de unicidade por chave da nota e uma checagem de sobreposição por CNPJ e dia garantem que nenhum dia soma os dois ERPs. Exceções (CNPJ que mudou, loja que migrou em etapas) ficam em `dw_reference.StoreErpMap`, com `cnpj`, `source_erp`, `valid_from` e `valid_to`: um CNPJ com período para o Linx só tem as linhas Linx dentro dele (`vw_LinxSalesItem`). É assim que se resolve um par apontado por `vw_Check_ErpOverlap`, que bloqueia.
- **Linx Commerce** só entra quando houver carga real (1.2, item 17). As colunas acima vêm dos scripts, não de uma resposta da API.
- **Carga única:** `dw_intermediate.sp_LinxLegacy_Load` lê as views `vw_LinxSalesItem` e `vw_LinxOrderItem` e grava as linhas Linx em `FactSalesItem` e `FactOrderItem`; roda na fase 3 (entrada `RunLinxLegacy`, job 9004) e fica disponível para reexecução. As origens Linx ficam desligadas no catálogo, fora dos ciclos horário e diário; o snapshot de lojas e produtos (`history_frequency = 'once'`) roda na mesma carga.
- **Membros Linx nas dimensões:** as procedures de dimensão da fase 1 passaram a carregar Totvs e Linx juntas, a cada hora: `DimStore` (a loja Linx com CNPJ na Totvs é a mesma linha; a loja só do Linx ganha linha com `source_erp = linxmicrovix` e o código da empresa no Microvix), `DimProduct` (membros Linx com o de/para), `DimCustomer` (sexo e situação do Linx), `DimSeller` (`seller_code = cnpj_emp-cod_vendedor`), `DimOperation` (natureza de operação; `operation_code` virou texto) e `DimBrand` (marcas Linx que não estão em `BrandSeries`). O cliente Linx vai para `dw_personal_data.DimCustomerIdentified` por `sp_LinxCustomer_Load`, sem fornecedores e transportadores.
- **Regras da linha Linx (`vw_LinxSalesItem`):** venda é operação `S` e `DS` (devolução, negativa); `E` e `DE` entram com `is_sale = 0`. Faturada como na 7.3. Canal e-commerce quando a linha veio de um pedido B2C (`id_venda_produto_b2c`), senão loja. Valor líquido (`valor_liquido`), custo (`preco_custo`) e preço de tabela da época (`preco_tabela_epoca`) da própria linha; `original_price` e `promotional_price` ficam nulos. Pedido B2C (`vw_LinxOrderItem`): loja pela venda de origem, captado quando o pedido está ativo.

### 7.6 Fase 4: compras, contas a pagar, comissões, recepção de loja, representantes e auditoria de envios

Tabelas que os módulos já carregam e que viram fatos próprios depois das fases 1 a 3, no mesmo padrão:

| Fato ou dimensão | Grão (PK da origem) | Fonte | Observação |
|---|---|---|---|
| `FactPurchaseOrderItem` | item do pedido de compra (`doc_company`, `branchCode`, `orderCode`, `productCode`) | `totvs.TotvsPurchaseOrdersProducts`, `TotvsPurchaseOrders` (fornecedor, comprador, status, datas de entrega) | Quantidades pedida, recebida (`settledQuantity`), cancelada e extra; `DimSupplier` |
| `FactAccountsPayable` | parcela da duplicata (`doc_company`, `branchCode`, `supplierCode`, `duplicateCode`, `installmentCode`) | `totvs.TotvsDuplicates` | Emissão, vencimento, baixa, valor, juros, desconto, pago; `status` |
| `FactCommission` | fechamento por comissionado (`doc_company`, `closingBranchCode`, `closingCode`, `commissioned_doc_hash`) | `totvs.TotvsCommissionsPaid`, lida direto da origem (sem landing, porque o documento está na chave) | CPF/CNPJ do comissionado vira `doc_hash` com o pepper (5.2, regra 6), e o documento em claro não é copiado; a base está vazia hoje (README do módulo) |
| `FactStoreReception` | atendimento recebido na loja (`doc_company`, `branchCode`, `movementDate`, `sequence`) | `totvs.TotvsReceptionSales` | Cliente, vendedor, guia e tipo de recepção: base para taxa de conversão (recepções × vendas por loja e dia) |
| `FactApiRequest` | chamada a um SaaS (`source_module`, `request_id`); carga incremental pelo `loaded_at` da landing, sem apagar | `totvs.TotvsRequests`, `linxmicrovix.LinxMicrovixRequests`, `linxcommerce.LinxCommerceRequests`, `movidesk.MovideskRequests`, `aftersale.AfterSaleRequests` (1.2, item 10) | Quem (`user_name`), quando (`request_date`), o quê (`http_method`, `route`, `field_key_value`), em qual SaaS (`source_module`) e com que resultado (`is_success`, `status_code`). Sem corpo da requisição: `request_body` e `response_body` ficam na origem. Responde "em tal dia, tal usuário inseriu, alterou ou excluiu tal registro em tal SaaS" |
| `DimSalesRepresentative` | representante (`doc_company`, `code`) | `totvs.TotvsRepresentatives` | SCD1. Carteira em `TotvsSellersCustomers` e `TotvsPeopleRepresentatives`, se o B2B precisar |
| `DimRegion` | área operacional (`doc_company`, `operationalAreaCode`) | `totvs.TotvsOperationalAreas` (CEPs, cidades e estados nas filhas) | SCD1 |
| `DimPromotion` | promoção por loja e produto (`doc_company`, `branchCode`, `promotionCode`, `productCode`) | `totvs.TotvsPromotions` | SCD1. `TotvsPricesOtherPromotions`, `TotvsPriceTables` e `TotvsPriceTablesItems` (preço B2B) ficam para quando forem pedidos |

Fora do escopo desta versão, por não terem tabela de dados aqui: logística (`FactShipment`) e pagamentos de outros canais (Pagarme). Entram quando os módulos criarem as tabelas (1.1); até lá, esses módulos são consumidores (4.2).
