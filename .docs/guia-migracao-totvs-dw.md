# Guia de migração: schema `totvs` novo (repo ETL) × schema `dw`

Objetivo: trocar as tabelas do schema `totvs` do NEWBLOOMERS pelas do repositório
`extract-transform-load-integrations-system` (`src/Backend/Modules/Totvs/Infrastructure.Totvs/Scripts/SQLServer`)
sem que a carga do `dw` (`dw.P_Carga_ETL` e as 17 etapas de `dw.etl_steps`) pare de funcionar.

Levantamento feito em 2026-11-06 no NEWBLOOMERS de dev (`sql-dev-webjobs`), lendo as dependências que o SQL Server registra
(`sys.dm_sql_referenced_entities`, que inclui as colunas trazidas por `nf.*`) e as definições dos objetos.
As tabelas novas **ainda não existem** no banco: os rascunhos de SQL deste guia não foram executados.

---

## 1. Resumo

| O que muda | Impacto no `dw` |
|---|---|
| As tabelas ganham o prefixo `Totvs` (`totvs.Invoices` → `totvs.TotvsInvoices`) | Todas as referências mudam de nome. Como os nomes não colidem, **as tabelas antigas e as novas podem coexistir** no mesmo schema durante a migração |
| `doc_company` (CNPJ da holding) entra em todas as tabelas e chaves | Hoje há uma única empresa em `TotvsParameters`. Nada quebra, mas se entrar uma segunda empresa todo join precisa incluir `doc_company` |
| Chave da nota: `(branchCode, invoiceSequence, invoiceDate)` | As filhas (`InvoicesItems*`, `InvoicesPayments`, `InvoicesSalesOrders`) **perdem** `branchCnpj`, `invoiceCode` e `invoiceSerialCode`, que o `dw` usa nos joins |
| Produto: `branchInfoCode` (matriz da marca) entra na chave de `TotvsSkus*` e `TotvsProducts*` | Join só por `code`/`productCode` pode duplicar linhas se o produto vier em mais de uma marca. As tabelas `Products*` perdem `referenceId`, `colorCode` e `productSku` |
| Colunas de objetos 1:1 com prefixo (`eletronic*`, `sat*`, `shippingCompany*`, `person*`, `preferences*`, `cardInformation*`) | 25 colunas usadas pelo `dw` mudam de nome (seção 5) |
| `totvs.Operations` **não existe** no projeto novo | **Bloqueador**: é usada por 9 objetos do `dw` e por `totvs.Gera_Prefixo_Pedidos` (seção 3) |
| Textos passam de `varchar` para `nvarchar`, alguns tamanhos e tipos mudam | Sem quebra direta. Há conversão implícita contra as colunas `varchar` das dimensões (seção 6) |

**Estratégia recomendada:** criar uma **camada de compatibilidade**, um schema `dw_src` com uma view por tabela antiga, com o
mesmo nome e as mesmas colunas que o `dw` lê, montada sobre as tabelas novas. No `dw`, a troca vira mecânica:
`totvs.X` → `dw_src.X`. Como as tabelas antigas continuam existindo durante o paralelo, dá para comparar o resultado de cada
etapa lendo do antigo e do novo antes de virar a chave.

Reescrever o `dw` para ler as tabelas `TotvsX` direto (sem as views) fica como fase 2, opcional, objeto por objeto.

---

## 2. O que o `dw` usa hoje

29 tabelas do `totvs`, lidas por 22 objetos do `dw` e por `totvs.Gera_Prefixo_Pedidos`. Este último fica no schema `totvs`,
mas é usado pelo `dw` e lê `dw.Eligible_Invoices`.

| Tabela atual | Linhas | Tabela nova | Objetos do `dw` que leem |
|---|---:|---|---|
| Branches | 28 | TotvsBranches | P_Dim_Loja, P_Dim_Pedido, 3 fatos, bate_tabelas, vw_total_acumulado(_teste), Gera_Prefixo_Pedidos |
| CostsBranches | 1.765.610 | **TotvsCosts** | fn_preco_custo |
| PricesBranches | 132.006 | **TotvsPrices** | fn_preco_custo |
| Invoices | 114.322 | TotvsInvoices | quase todos (seção 7) |
| InvoicesItems | 354.726 | TotvsInvoicesItems | bate_nota, P_Fato_Canal_Movimentacao, P_FixMissingInvoiceEntities |
| InvoicesItemsProducts | 350.986 | TotvsInvoicesItemsProducts | P_Fato_Canal_Movimentacao |
| InvoicesItemsTaxes | 1.333.246 | TotvsInvoicesItemsTaxes | bate_nota, P_Fato_Canal_Movimentacao |
| InvoicesPayments | 75.789 | TotvsInvoicesPayments | descricao_pagamento |
| InvoicesSalesOrders | 8.499 | TotvsInvoicesSalesOrders | P_Dim_Pedido, P_Fato_Canal_Movimentacao, P_Fato_Flash_Vendas, Gera_Prefixo_Pedidos |
| LegalEntities | 2.242 | TotvsLegalEntities | P_Dim_Cliente, P_Dim_Fornecedor, P_Dim_Transportadora, P_Dim_Pedido, 2 fatos |
| LegalEntitiesAddresses / Emails / Phones | 1.274 / 419 / 691 | TotvsLegalEntitiesAddresses / Emails / Phones | P_Dim_Cliente |
| People | 34.860 | TotvsPeople | P_Dim_Cliente, P_Dim_Fornecedor, P_Dim_Pedido, 2 fatos |
| PeopleAddresses / Emails / Phones | 35.681 / 22.140 / 30.645 | TotvsPeopleAddresses / Emails / Phones | P_Dim_Cliente |
| **Operations** | 96 | **não existe** | calcula_id_canal, P_Dim_Pedido, 3 fatos, bate_nota, bate_tabelas, vw_total_acumulado(_teste), Gera_Prefixo_Pedidos |
| ProductsAdditionalFields | 54.726 | TotvsProductsAdditionalFields | produtos_detalhes |
| ProductsClassifications | 232.135 | TotvsProductsClassifications | P_Dim_Produto, produtos_detalhes |
| ProductsSuppliers | 19.476 | TotvsProductsSuppliers | 2 fatos de canal, produtos_detalhes |
| PurchaseOrders | 477 | TotvsPurchaseOrders | P_Dim_Pedido, P_Fato_Canal_Movimentacao |
| SaleOrders | 9.054 | TotvsSaleOrders | P_Dim_Pedido, 3 fatos, Gera_Prefixo_Pedidos |
| SaleOrdersInvoices | 8.429 | TotvsSaleOrdersInvoices | P_Fato_Flash_Vendas |
| SaleOrdersProducts | 33.258 | TotvsSaleOrdersProducts | 2 fatos de canal |
| Sellers | 144 | TotvsSellers | calcula_id_canal, P_Dim_Vendedor, P_Fato_Canal_Movimentacao |
| Skus / SkusColors / SkusColorsProducts | 7.126 / 10.165 / 22.079 | TotvsSkus / TotvsSkusColors / TotvsSkusColorsProducts | P_Dim_Produto, 2 fatos de canal, produtos_detalhes |

Fora do `dw`, também leem essas tabelas e **quebram se as antigas forem apagadas**: as 11 procs `general.p_Volo_Integracao_*_Totvs`,
`migracao.P_Insere_Produtos_De_Para`, `operations.VW_ESTOQUE_OPERACOES`, `operations.VW_ESTOQUE_TRANSITO_OPERACOES(2)`,
a view `totvs.eligible_invoices` e as procs antigas `totvs.p_*_Sincronizacao`, que somem junto com as tabelas antigas.
Elas estão fora do escopo deste guia, mas **impedem o passo 8.6** (apagar as tabelas antigas) até serem tratadas.

---

## 3. Bloqueador: `totvs.Operations`

O `PROPOSTA-PADRONIZACAO-TOTVS.md` do repo ETL (linha 1319) descarta `TotvsOperations` dizendo que "não existe rota de operações
em nenhum swagger da Totvs". **Isso não confere com o código atual:** o `GeneralService` do recurring-jobs chama
`/general/v2/operations` (`newbloomers-recurring-jobs/src/Totvs/Application.Totvs/Services/GeneralService.cs:88`), e a tabela tem
96 linhas com `maxChangeFilterDate` até 2026-11-05.

Colunas que o `dw` usa: `operationCode`, `description`, `isFinancial`, `invoiceDataOperationsType` (`'S'`/`'E'`),
`invoiceDataDocumentModel`, `invoiceDataOperationMode`, `invoiceDataPurposeNFE`. Nenhuma delas vem nas notas nem nos pedidos novos:
eles trazem só `operationCode`, `operationType` e `operatioName`.

Opções, em ordem de preferência:

1. **Implementar `TotvsOperations` no repo ETL** usando a rota `/general/v2/operations`, no mesmo padrão das outras
   (untreated + totvs, chave `doc_company, operationCode`). A view `dw_src.Operations` passa a ler dela.
2. **Até lá, manter o job antigo de operações rodando** no recurring-jobs e **não apagar** `totvs.Operations`.
   A view `dw_src.Operations` lê a tabela antiga. Não há conflito de nome, porque a nova se chamaria `TotvsOperations`.
3. Congelar `totvs.Operations` como tabela de cadastro manual. Não recomendado: operação nova criada na Totvs some do `dw`
   (o join com `Operations` é `INNER` em `P_Dim_Pedido`, `bate_nota` e `vw_total_acumulado`).

---

## 4. Mudanças de chave que afetam joins

### 4.1 Notas fiscais

Hoje o `dw` liga a nota às filhas pela chave antiga:

```sql
JOIN totvs.InvoicesItems nfi
  ON nf.branchCnpj = nfi.branchCnpj AND nf.serialCode = nfi.invoiceSerialCode
 AND nf.invoiceCode = nfi.invoiceCode AND nf.invoiceSequence = nfi.invoiceSequence
```

No modelo novo, as filhas só têm `(doc_company, branchCode, invoiceSequence, invoiceDate)`, e o número da sequência do item se
chama `itemSequence` (antes `invoiceItemsSequence`). A camada `dw_src` recompõe `branchCnpj`, `invoiceCode` e `invoiceSerialCode`
juntando a filha com o cabeçalho (seção 9), e os joins atuais continuam valendo. Numa reescrita nativa, o join passa a ser:

```sql
JOIN totvs.TotvsInvoicesItems nfi
  ON nfi.doc_company = nf.doc_company AND nfi.branchCode = nf.branchCode
 AND nfi.invoiceSequence = nf.invoiceSequence AND nfi.invoiceDate = nf.invoiceDate
```

**Risco que já existe hoje e que fica mais visível:** `(branchCode, invoiceSequence)` **não é único**. Há 114.015 notas distintas
por `(branchCode, invoiceSequence, invoiceDate)`, mas só 112.489 por `(branchCode, invoiceSequence)`. Pontos que juntam só por
sequência e podem multiplicar linhas:

- `dw.eligible_invoices`: a chave é `branchCode, invoiceSequence`, sem data (105.430 linhas, 104.348 chaves distintas).
- `LEFT JOIN totvs.InvoicesSalesOrders o ON nf.invoiceSequence = o.invoiceSequence` (só a sequência, sem filial), em
  `P_Dim_Pedido_ETL`, `P_Fato_Canal_Movimentacao_ETL` e `Gera_Prefixo_Pedidos`.

A camada de compatibilidade preserva esse comportamento. Corrigir é recomendado, mas como passo separado (seção 10).

### 4.2 Nota × pedido (`InvoicesSalesOrders`)

O `branchCode` da tabela antiga é a **filial do pedido** (`salesOrderDto.branchCode`, em `Domain.Totvs/Entities/Fiscal/SaleOrder.cs:27`).
Na nova, `branchCode` é a **filial da nota**, e a do pedido se chama `orderBranchCode`.
`dw_src.InvoicesSalesOrders` expõe `orderBranchCode AS branchCode` para os joins
`so.branchCode = o.branchCode` e `po.branchCode = o.branchCode` continuarem certos. **Este é o erro mais fácil de cometer numa
reescrita**: trocar só o nome da tabela compila, roda e liga a nota ao pedido errado.

### 4.3 Produtos

- `TotvsSkus`, `TotvsSkusColors` e `TotvsSkusColorsProducts` têm `branchInfoCode` na chave. Se o mesmo produto vier pelas duas
  marcas, os joins por `code` duplicam linhas. Hoje `SkusColorsProducts` já tem 22.079 linhas para 22.056 códigos (ver a memória
  sobre `SkusColorsProducts.code` não ser único). A view precisa escolher uma linha por produto (seção 9.6).
  **Decisão pendente: qual `branchInfoCode` vence quando o produto vem nas duas marcas.**
- `TotvsProductsClassifications`, `TotvsProductsSuppliers` e `TotvsProductsAdditionalFields` têm só `productCode`.
  `referenceId`, `colorCode` e `productSku`, usados por `P_Dim_Produto_ETL`, `produtos_detalhes` e pelos fatos, vêm do
  join com `TotvsSkusColorsProducts`.
- `TotvsSkusColors.colorCode` era `SkusColors.code`.

### 4.4 Pedidos de venda

`SaleOrdersProducts.saleOrderCode` e `SaleOrdersInvoices.saleOrderCode` passam a se chamar `orderCode`.
`TotvsSaleOrdersInvoices` tem `serial` na chave.

### 4.5 Endereços de pessoa

A chave antiga era `(personCode, sequenceCode)`. A nova inclui `addressTypeCode`: o mesmo `sequenceCode` pode aparecer com
dois tipos de endereço. Confira como `P_Dim_Cliente_ETL` escolhe o endereço. Se ele não filtra o tipo, pode passar a ver
duas linhas por cliente.

---

## 5. Colunas renomeadas (somente as que o `dw` usa)

Comparação feita sem diferenciar maiúsculas de minúsculas, porque a collation do banco é `SQL_Latin1_General_CP1_CI_AS`.
Por isso `observationNFe` × `observationNFE` não é mudança.

| Tabela antiga | Coluna antiga | Coluna nova | Observação |
|---|---|---|---|
| Invoices | accessKey | eletronicAccessKey | varchar(44) → nvarchar(50) |
| Invoices | electronicInvoiceStatus | eletronicElectronicInvoiceStatus | |
| Invoices | receipt | eletronicReceipt | int → bigint. Hoje é sempre 0 (o int estoura) |
| Invoices | receivementDate | eletronicReceivementDate | |
| Invoices | disableProtocol | eletronicDisableProtocol | int → bigint. Hoje sempre 0 |
| Invoices | disableDate | eletronicDisableDate | |
| Invoices | satCode | satSatCode | |
| Invoices | satSequence | satSatSequence | |
| Invoices | shippingCompanyCode | shippingCompanyShippingCompanyCode | |
| Invoices | shippingCompanyName | shippingCompanyShippingCompanyName | |
| Invoices | shippingCompanyType | shippingCompanyShippingCompanyType | |
| Invoices | personCpfCnpj | personPersonCpfCnpj | |
| Invoices | personType | personPersonType | **int → nvarchar(5)**. Hoje é sempre 0 |
| Invoices | productionOrderCode | productionOrderOrderCode | |
| Invoices | productionOrderRelationshipType | productionOrderOrderRelationshipType | |
| InvoicesItemsProducts / InvoicesItemsTaxes | invoiceItemsSequence | itemSequence | |
| InvoicesItems* / InvoicesPayments / InvoicesSalesOrders | branchCnpj, invoiceCode, invoiceSerialCode | (não existem) | vêm do cabeçalho (4.1) |
| InvoicesSalesOrders | branchCode | **orderBranchCode** | ver 4.2 |
| InvoicesPayments | cardFlag | cardInformationCardFlag | |
| InvoicesPayments | cardFlagCode | cardInformationCardFlagCode | |
| InvoicesPayments | cardOperatorName | cardInformationCardOperatorName | |
| LegalEntities | shippingCompanyName | preferencesShippingCompanyName | no código antigo já vinha de `preferences.shippingCompanyName` |
| LegalEntitiesAddresses / Emails / Phones | legalEntityCode | personCode | |
| SaleOrders | shippingNumber | shippingAddressNumber | varchar(60) → int. Vinha de `shippingAddress.number` |
| SaleOrdersProducts / SaleOrdersInvoices | saleOrderCode | orderCode | |
| SkusColors | code | colorCode | |
| ProductsAdditionalFields / Classifications / Suppliers | referenceId, colorCode, productSku | (não existem) | vêm de `TotvsSkusColorsProducts` (4.3) |
| CostsBranches → TotvsCosts | branchCode, productCode, costCode, costName, cost | iguais | |
| PricesBranches → TotvsPrices | branchCode, productCode, priceCode, priceName, price, promotionalPrice | iguais | o `Id` antigo deixa de existir. O `dw` não o usa |

As demais colunas usadas pelo `dw` mantêm o nome. Isso inclui todo o resto de `Invoices` (`personName`, `personCode`,
`operatioName` com o erro de grafia vindo da API, `shippingCompanyFreitghtType`…), `Branches`, `People*`, `PurchaseOrders`,
`Sellers`, `Skus` e `SkusColorsProducts`.

---

## 6. Tipos

- Texto: `varchar` vira `nvarchar`. As dimensões do `dw` são `varchar` (`Dim_Pedido.CHAVE_NF varchar(100)`). Comparar
  `nvarchar` com `varchar` funciona, mas o `varchar` é convertido e pode perder o uso de índice em joins e `MERGE` grandes.
  **Nas views de compatibilidade, faça `CAST` para o tipo antigo** e o `dw` enxerga exatamente o que enxerga hoje.
- `SaleOrders.quantity` e `SaleOrdersProducts.quantity`: int → decimal(18,6).
- `InvoicesItems.cfop`: varchar → int. O `dw` não usa.
- `InvoicesPayments.documentNumber`: int → bigint.
- Tamanhos menores no novo, com risco de truncar na ingestão (não no `dw`): `Branches.description` 300 → 60,
  `LegalEntities.name` 250 → 60, `People.name` 250 → 60. Vale conferir com os dados reais depois da primeira carga.

---

## 7. Checklist por objeto (na ordem de `dw.etl_steps`)

Com a camada `dw_src`, em cada objeto a alteração é **trocar `totvs.` por `dw_src.`** nas tabelas da seção 2. As linhas abaixo dizem
o que conferir em cada um.

| # | Objeto | O que mudar / conferir |
|---|---|---|
| 10 | `P_Dim_Loja_ETL` (desativada) | Branches |
| 20 | `P_Dim_Cliente_ETL` | People, PeopleAddresses/Emails/Phones, LegalEntities*. Conferir a duplicação de endereço (4.5) |
| 30 | `P_Dim_Fornecedor_ETL` | People, LegalEntities (`fantasyName`). `migracao.Fornecedor` não muda |
| 40 | `P_Dim_Vendedor_ETL` | Sellers |
| 50 | `P_Dim_Transportadora_ETL` | LegalEntities: `shippingCompanyName` vira `preferencesShippingCompanyName` (a view resolve) |
| 60 | `P_Dim_Produto_ETL` | Skus, SkusColorsProducts, ProductsClassifications. Regra de `branchInfoCode` (4.3) |
| 70 | `P_Dim_Produto_Detalhe_ETL` | Só lê `dw.produtos_detalhes`: mudar a view `produtos_detalhes` (Skus*, Products*) |
| 80 | `P_Dim_Frete_ETL` | Invoices (`shippingCompanyName` e `shippingCompanyFreitghtType`) |
| 90 | `P_Dim_Pagamento_ETL` | Invoices, `dw.descricao_pagamento` (InvoicesPayments: colunas `card*`) |
| 100 | `P_FilterEligibleInvoices_ETL` | Invoices (`accessKey`, `origin`, `operationType`) |
| 110 | `P_Dim_Pedido_ETL` | **Usa `nf.*` de `totvs.Invoices`**: o `MERGE` depende de todos os nomes de coluna antigos. Com `dw_src.Invoices` funciona (a view tem a lista completa). Numa reescrita nativa, trocar `nf.*` por uma lista explícita. Também usa Operations, InvoicesSalesOrders (4.2), SaleOrders, PurchaseOrders, People e LegalEntities |
| 115 | `P_FixMissingInvoiceEntities_ETL` | Invoices, InvoicesItems |
| 120 | `P_Fato_Canal_Movimentacao_ETL` | Toda a família de notas, joins de itens (4.1), Operations, Sellers, Skus*, ProductsSuppliers, SaleOrders/Products, PurchaseOrders. Também `dw.calcula_id_canal` e `dw.fn_preco_custo` |
| 130 | `P_Fato_Flash_Vendas_ETL` | Invoices, InvoicesSalesOrders, SaleOrders, SaleOrdersInvoices (`saleOrderCode`), Operations, Branches |
| 140 | `P_Fato_Canal_Movimentacao_Captado_ETL` | SaleOrders (`shippingNumber`), SaleOrdersProducts, Skus*, ProductsSuppliers, LegalEntities, People, Operations |
| 150 | `merge_canal_movimentacao_alphaville` | Não lê `totvs`. Sem mudança |
| 160 | `P_DeleteInvoices_ETL` (desativada) | Invoices (`accessKey`) |

Objetos chamados pelas etapas ou usados por relatórios:

| Objeto | O que mudar |
|---|---|
| `dw.calcula_id_canal` | Operations, Sellers |
| `dw.descricao_pagamento` | InvoicesPayments |
| `dw.fn_preco_custo` | CostsBranches e PricesBranches. Também usada por `operations.VW_ESTOQUE_OPERACOES` |
| `dw.produtos_detalhes` (view) | Skus*, Products*. Também usada por `operations.VW_ESTOQUE_OPERACOES` |
| `dw.p_addInvoiceToFilter_ETL`, `dw.p_PurgeReplacedInvoices_ETL` | Invoices |
| `dw.bate_nota`, `dw.bate_tabelas` (views de conferência) | Invoices, Items, Taxes, Operations, Branches |
| `dw.vw_total_acumulado`, `dw.vw_total_acumulado_teste` | Invoices, Operations, Branches |
| `totvs.Gera_Prefixo_Pedidos` | Invoices, InvoicesSalesOrders, SaleOrders, Branches, Operations. Fica no schema `totvs`, mas é do `dw` |

Sem mudança, porque não leem `totvs`: `P_Carga_ETL`, `P_RunStep_ETL`, `sp_Insere_Dim_Tempo`, `CANAL_MOVIMENTACAO`,
`CANAL_MOVIMENTACAO_CAPTADO` (que já está quebrada por usar `dw.Dim_NotaFiscal`), `dashboard_vendas(_d)`, `vw_fato_flash_vendas`,
`Somente_Numeros`, `fn_CalculaPascoa` e `fn_IsEligibleInvoice`.

Para listar as ocorrências a trocar num objeto:

```sql
SELECT OBJECT_SCHEMA_NAME(object_id) + '.' + OBJECT_NAME(object_id) AS objeto
FROM sys.sql_modules
WHERE (OBJECT_SCHEMA_NAME(object_id) = 'dw' OR object_id = OBJECT_ID('totvs.Gera_Prefixo_Pedidos'))
  AND definition LIKE '%totvs.%';
```

Lembretes de scripts de procedure (ver a memória sobre sqlcmd): começar com `SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;`,
rodar `sqlcmd` com `-I` e conferir `sys.sql_modules.uses_quoted_identifier = 1` depois do `ALTER`.

---

## 8. Ordem de execução

1. **Criar as tabelas novas** (`untreated.TotvsX` e `totvs.TotvsX`) pelo migrator do repo ETL. Não há colisão de nomes:
   o `untreated` antigo tem `Invoices`, `Operations`, `SaleOrders` e `Skus`, e o novo usa `TotvsInvoices`…
2. **Resolver `Operations`** (seção 3).
3. **Fazer a carga inicial e rodar os jobs novos em paralelo** com os antigos, pelo menos alguns dias, para as duas bases ficarem
   na mesma janela.
4. **Criar o schema `dw_src` e as views** (seção 9) e rodar as consultas de validação (seção 10) até baterem.
5. **Repontar o `dw`**: trocar `totvs.` por `dw_src.` nos objetos da seção 7, um passo de cada vez, comparando a saída.
   Sugestão: primeiro as funções e views (`fn_preco_custo`, `descricao_pagamento`, `produtos_detalhes`, `calcula_id_canal`),
   depois as dimensões e por último os fatos.
   Backup das tabelas `dw` antes do primeiro `P_Carga_ETL` repontado (no mesmo padrão `_bkpAAAAMMDD` que já existe).
   **Rollback:** reaplicar as definições antigas. Guarde a saída de `sys.sql_modules` antes de alterar.
6. **Só depois de tratar os consumidores fora do `dw`** (seção 2, último parágrafo): desligar os jobs antigos e apagar as tabelas
   antigas.

---

## 9. Rascunho da camada `dw_src`

Rascunho **não executado**: as tabelas novas ainda não existem no banco. Cada view expõe só o que o `dw` lê, com os nomes e tipos
antigos. Antes de usar, confira com `sys.dm_sql_referenced_entities` se nenhum objeto lê uma coluna que ficou de fora.

```sql
SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;
GO
IF SCHEMA_ID('dw_src') IS NULL EXEC('CREATE SCHEMA dw_src');
GO
```

### 9.1 Cadastros simples

```sql
CREATE OR ALTER VIEW dw_src.Branches AS
SELECT code, CAST([description] AS varchar(300)) AS [description], CAST(cnpj AS varchar(14)) AS cnpj,
       personCode, CAST(personName AS varchar(300)) AS personName, CAST(fantasyName AS varchar(60)) AS fantasyName,
       CAST(branchGroupName AS varchar(60)) AS branchGroupName, lastchange, maxChangeFilterDate
FROM totvs.TotvsBranches;
GO
CREATE OR ALTER VIEW dw_src.Sellers AS
SELECT sellerCode, CAST(sellerName AS varchar(250)) AS sellerName, personCode,
       CAST(personName AS varchar(250)) AS personName, maxChangeFilterDate
FROM totvs.TotvsSellers;
GO
CREATE OR ALTER VIEW dw_src.CostsBranches AS
SELECT branchCode, productCode, costCode, CAST(costName AS varchar(60)) AS costName, cost
FROM totvs.TotvsCosts;
GO
CREATE OR ALTER VIEW dw_src.PricesBranches AS
SELECT branchCode, productCode, priceCode, CAST(priceName AS varchar(60)) AS priceName, price, promotionalPrice
FROM totvs.TotvsPrices;
GO
-- enquanto TotvsOperations não existir (seção 3), lê a tabela antiga
CREATE OR ALTER VIEW dw_src.Operations AS
SELECT * FROM totvs.Operations;
GO
```

### 9.2 Notas

```sql
CREATE OR ALTER VIEW dw_src.Invoices AS
SELECT branchCode, CAST(branchCnpj AS varchar(14)) AS branchCnpj, invoiceSequence, invoiceDate,
       personCode, CAST(personName AS varchar(250)) AS personName, invoiceCode, CAST(serialCode AS varchar(60)) AS serialCode,
       CAST(invoiceStatus AS varchar(60)) AS invoiceStatus, transactionBranchCode, transactionDate, transactionCode,
       inclusionComponentCode, peripheralPdvCode, versionPdv, mobileVersion, idDocumentPDV, userCode,
       CAST([origin] AS varchar(60)) AS [origin], documentType, CAST(operationType AS varchar(60)) AS operationType,
       operationCode, operatioName, issueDate, releaseDate, exitTime, lastchangeDate, maxChangeFilterDate,
       paymentConditionCode, paymentConditionName, discountPercentage, [quantity], productValue, additionalValue,
       shippingValue, insuranceValue, ipiValue, baseIcmsValue, icmsValue, icmsSubStValue, totalValue, terminalCode,
       sellerCpf, observationNFE AS observationNFe,
       CAST(eletronicAccessKey AS varchar(44))               AS accessKey,
       CAST(eletronicElectronicInvoiceStatus AS varchar(60)) AS electronicInvoiceStatus,
       eletronicReceipt                                      AS receipt,          -- bigint (antes int)
       eletronicReceivementDate                              AS receivementDate,
       eletronicDisableProtocol                              AS disableProtocol,  -- bigint (antes int)
       eletronicDisableDate                                  AS disableDate,
       ecfCoupon, ecfSerialMachine,
       satSatCode AS satCode, satDvNumber, satCnfNumber, satCancellationKey, satSatSequence AS satSequence, satSerialMachine,
       shippingCompanyShippingCompanyCode AS shippingCompanyCode,
       CAST(shippingCompanyShippingCompanyName AS varchar(250)) AS shippingCompanyName,
       CAST(shippingCompanyShippingCompanyType AS varchar(60))  AS shippingCompanyType,
       shippingCompanyFreitghtType, shippingCompanyFreitghtTypeRedispatch, shippingCompanyRedispatchCarrierCode,
       shippingCompanyRedispatchCarrierName, shippingCompanyCityCodeRedispatch, shippingCompanyCityNameRedispatch,
       shippingCompanyCityUfRedispatch, shippingCompanyFreightValue, shippingCompanyPublicPlaceTypeName,
       shippingCompanyPublicPlaceName, shippingCompanyPublicPlaceCode, shippingCompanyNeighborhood, shippingCompanyCityName,
       shippingCompanyStateAbbreviation, shippingCompanyCep, shippingCompanyPostOfficeBox,
       shippingCompanyNumberStateRegistration, shippingCompanyCpfCnpj, shippingCompanyPlaqueCode, shippingCompanyPlaqueUf,
       shippingCompanyPackageNumber, shippingCompanyGrossWeight, shippingCompanyNetWeight, shippingCompanySpecies,
       shippingCompanyBrandName, shippingCompanyTrackingCode,
       TRY_CAST(personPersonType AS int) AS personType,   -- hoje sempre 0; conferir o que a API manda
       CAST(personPersonCpfCnpj AS varchar(14)) AS personCpfCnpj,
       personPostOfficeBox, personAddressNumber, personCep, personAddress, personPublicPlace, personComplement,
       personNeighborhood, personCity, personStateAbbreviation, personFoneNumber, personRgIe, personCityCode,
       productionOrderBranchCode, productionOrderCycleCode, productionOrderOrderCode AS productionOrderCode,
       productionOrderOrderRelationshipType AS productionOrderRelationshipType
FROM totvs.TotvsInvoices;
GO
CREATE OR ALTER VIEW dw_src.InvoicesItems AS
SELECT CAST(nf.branchCnpj AS varchar(14)) AS branchCnpj, CAST(nf.serialCode AS varchar(60)) AS invoiceSerialCode,
       nf.invoiceCode, i.invoiceSequence, i.[sequence], CAST(i.[code] AS varchar(44)) AS [code],
       CAST(i.[name] AS varchar(250)) AS [name], i.[quantity], i.grossValue, i.discountValue, i.netValue, i.freightValue,
       i.branchCode, i.invoiceDate   -- chave nova, para reescritas
FROM totvs.TotvsInvoicesItems i
JOIN totvs.TotvsInvoices nf ON nf.doc_company = i.doc_company AND nf.branchCode = i.branchCode
                           AND nf.invoiceSequence = i.invoiceSequence AND nf.invoiceDate = i.invoiceDate;
GO
CREATE OR ALTER VIEW dw_src.InvoicesItemsProducts AS
SELECT CAST(nf.branchCnpj AS varchar(14)) AS branchCnpj, CAST(nf.serialCode AS varchar(60)) AS invoiceSerialCode,
       nf.invoiceCode, p.invoiceSequence, p.itemSequence AS invoiceItemsSequence, p.productCode, p.productName,
       p.dealerCode, p.[quantity], p.unitGrossValue, p.unitDiscountValue, p.unitNetValue, p.grossValue, p.discountValue,
       p.netValue, p.branchCode, p.invoiceDate
FROM totvs.TotvsInvoicesItemsProducts p
JOIN totvs.TotvsInvoices nf ON nf.doc_company = p.doc_company AND nf.branchCode = p.branchCode
                           AND nf.invoiceSequence = p.invoiceSequence AND nf.invoiceDate = p.invoiceDate;
GO
CREATE OR ALTER VIEW dw_src.InvoicesItemsTaxes AS
SELECT CAST(nf.branchCnpj AS varchar(14)) AS branchCnpj, CAST(nf.serialCode AS varchar(60)) AS invoiceSerialCode,
       nf.invoiceCode, t.invoiceSequence, t.itemSequence AS invoiceItemsSequence, t.[code], t.[name], t.cst,
       t.taxPercentage, t.calculationBasisValue, t.taxValue, t.branchCode, t.invoiceDate
FROM totvs.TotvsInvoicesItemsTaxes t
JOIN totvs.TotvsInvoices nf ON nf.doc_company = t.doc_company AND nf.branchCode = t.branchCode
                           AND nf.invoiceSequence = t.invoiceSequence AND nf.invoiceDate = t.invoiceDate;
GO
CREATE OR ALTER VIEW dw_src.InvoicesPayments AS
SELECT CAST(nf.branchCnpj AS varchar(14)) AS branchCnpj, CAST(nf.serialCode AS varchar(60)) AS invoiceSerialCode,
       pg.invoiceSequence, pg.documentNumber, pg.expirationDate, pg.paymentValue, pg.documentTypeCode,
       CAST(pg.documentType AS varchar(60)) AS documentType, pg.installment, pg.bearerCode, pg.bearerName,
       pg.cardInformationCardOperatorCode AS cardOperatorCode,
       CAST(pg.cardInformationCardOperatorName AS varchar(60)) AS cardOperatorName,
       CAST(pg.cardInformationCardFlag AS varchar(60)) AS cardFlag,
       pg.cardInformationCardFlagCode AS cardFlagCode,
       pg.branchCode, pg.invoiceDate
FROM totvs.TotvsInvoicesPayments pg
JOIN totvs.TotvsInvoices nf ON nf.doc_company = pg.doc_company AND nf.branchCode = pg.branchCode
                           AND nf.invoiceSequence = pg.invoiceSequence AND nf.invoiceDate = pg.invoiceDate;
GO
CREATE OR ALTER VIEW dw_src.InvoicesSalesOrders AS
SELECT CAST(nf.branchCnpj AS varchar(14)) AS branchCnpj, CAST(nf.serialCode AS varchar(60)) AS invoiceSerialCode,
       nf.invoiceCode, so.invoiceSequence,
       so.orderBranchCode AS branchCode,       -- filial do PEDIDO, como na tabela antiga (seção 4.2)
       so.orderCode, CAST(so.orderId AS varchar(60)) AS orderId, CAST(so.customerOrderCode AS varchar(60)) AS customerOrderCode,
       so.branchCode AS invoiceBranchCode, so.invoiceDate
FROM totvs.TotvsInvoicesSalesOrders so
JOIN totvs.TotvsInvoices nf ON nf.doc_company = so.doc_company AND nf.branchCode = so.branchCode
                           AND nf.invoiceSequence = so.invoiceSequence AND nf.invoiceDate = so.invoiceDate;
GO
```

### 9.3 Pessoas

```sql
CREATE OR ALTER VIEW dw_src.People AS
SELECT [code], CAST(cpf AS varchar(14)) AS cpf, isInactive, CAST(gender AS varchar(60)) AS gender, birthDate,
       CAST([name] AS varchar(250)) AS [name], insertDate, maxChangeFilterDate,
       isCustomer, isSupplier, isRepresentative, isShippingCompany, isEmployee
FROM totvs.TotvsPeople;
GO
CREATE OR ALTER VIEW dw_src.PeopleAddresses AS
SELECT personCode, sequenceCode, addressTypeCode, cep, cityName, countryName, stateAbbreviation
FROM totvs.TotvsPeopleAddresses;            -- chave nova tem addressTypeCode (seção 4.5)
GO
CREATE OR ALTER VIEW dw_src.PeopleEmails AS
SELECT personCode, [sequence], CAST(email AS varchar(60)) AS email, isDefault FROM totvs.TotvsPeopleEmails;
GO
CREATE OR ALTER VIEW dw_src.PeoplePhones AS
SELECT personCode, [sequence], CAST([number] AS varchar(20)) AS [number], isDefault FROM totvs.TotvsPeoplePhones;
GO
CREATE OR ALTER VIEW dw_src.LegalEntities AS
SELECT [code], CAST(cnpj AS varchar(14)) AS cnpj, isInactive, CAST([name] AS varchar(250)) AS [name],
       CAST(fantasyName AS varchar(250)) AS fantasyName, CAST(uf AS varchar(20)) AS uf, insertDate, maxChangeFilterDate,
       isCustomer, isSupplier, isShippingCompany,
       CAST(preferencesShippingCompanyName AS varchar(250)) AS shippingCompanyName
FROM totvs.TotvsLegalEntities;
GO
CREATE OR ALTER VIEW dw_src.LegalEntitiesAddresses AS
SELECT personCode AS legalEntityCode, sequenceCode, addressTypeCode, cep, cityName, countryName, stateAbbreviation
FROM totvs.TotvsLegalEntitiesAddresses;
GO
CREATE OR ALTER VIEW dw_src.LegalEntitiesEmails AS
SELECT personCode AS legalEntityCode, [sequence], CAST(email AS varchar(60)) AS email, isDefault
FROM totvs.TotvsLegalEntitiesEmails;
GO
CREATE OR ALTER VIEW dw_src.LegalEntitiesPhones AS
SELECT personCode AS legalEntityCode, [sequence], CAST([number] AS varchar(20)) AS [number], isDefault
FROM totvs.TotvsLegalEntitiesPhones;
GO
```

### 9.4 Pedidos

```sql
CREATE OR ALTER VIEW dw_src.SaleOrders AS
SELECT branchCode, orderCode, guideCode, sellerCode, customerCode, CAST(customerCpfCnpj AS varchar(14)) AS customerCpfCnpj,
       CAST(customerName AS varchar(250)) AS customerName, representativeCode,
       CAST(representativeName AS varchar(250)) AS representativeName, operationCode, [quantity],
       grossValue, discountValue, netValue, freightValue, totalAmountOrder, insertDate, orderDate, maxChangeFilterDate,
       CAST(statusOrder AS varchar(60)) AS statusOrder, experienceType, hasFinancialProcessed, ecommerceStage,
       shippingCompanyCode, shippingCompanyCpfCnpj, shippingCompanyName, shippingServiceName,
       CAST(shippingAddressNumber AS varchar(60)) AS shippingNumber
FROM totvs.TotvsSaleOrders;
GO
CREATE OR ALTER VIEW dw_src.SaleOrdersProducts AS
SELECT branchCode, orderCode AS saleOrderCode, productCode, [quantity], price, originalPrice, discountPercentage, insertDate
FROM totvs.TotvsSaleOrdersProducts;
GO
CREATE OR ALTER VIEW dw_src.SaleOrdersInvoices AS
SELECT branchCode, orderCode AS saleOrderCode, [code], serial, issueDate, totalValue
FROM totvs.TotvsSaleOrdersInvoices;
GO
CREATE OR ALTER VIEW dw_src.PurchaseOrders AS
SELECT branchCode, orderCode, maxChangeFilterDate FROM totvs.TotvsPurchaseOrders;
GO
```

### 9.5 Produtos

Antes de criar, rode na base nova `SELECT code, COUNT(*) FROM totvs.TotvsSkusColorsProducts GROUP BY code HAVING COUNT(*) > 1`
e defina a regra de marca (4.3). O rascunho abaixo fica com o menor `branchInfoCode`.

```sql
CREATE OR ALTER VIEW dw_src.SkusColorsProducts AS
SELECT referenceId, CAST(colorCode AS varchar(60)) AS colorCode, [code], CAST(sku AS varchar(60)) AS sku,
       CAST([name] AS varchar(250)) AS [name], CAST(size AS varchar(60)) AS size, isActive, insertDate, branchInfoCode
FROM (SELECT p.*, ROW_NUMBER() OVER (PARTITION BY p.doc_company, p.[code] ORDER BY p.branchInfoCode) AS rn
      FROM totvs.TotvsSkusColorsProducts p) x
WHERE rn = 1;
GO
CREATE OR ALTER VIEW dw_src.Skus AS
SELECT s.referenceId, CAST(s.referenceCode AS varchar(60)) AS referenceCode, CAST(s.[name] AS varchar(250)) AS [name],
       CAST(s.[description] AS varchar(250)) AS [description], s.insertDate, s.maxChangeFilterDate
FROM (SELECT s.*, ROW_NUMBER() OVER (PARTITION BY s.doc_company, s.referenceId ORDER BY s.branchInfoCode) AS rn
      FROM totvs.TotvsSkus s) s
WHERE rn = 1;
GO
CREATE OR ALTER VIEW dw_src.SkusColors AS
SELECT referenceId, CAST(colorCode AS varchar(60)) AS [code], CAST([name] AS varchar(60)) AS [name]
FROM (SELECT c.*, ROW_NUMBER() OVER (PARTITION BY c.doc_company, c.referenceId, c.colorCode ORDER BY c.branchInfoCode) AS rn
      FROM totvs.TotvsSkusColors c) c
WHERE rn = 1;
GO
-- as três abaixo pegam referenceId / colorCode / productSku do produto, na mesma marca
CREATE OR ALTER VIEW dw_src.ProductsClassifications AS
SELECT cp.referenceId, CAST(cp.colorCode AS varchar(60)) AS colorCode, pc.productCode,
       CAST(cp.sku AS varchar(60)) AS productSku, pc.typeCode, CAST(pc.typeName AS varchar(60)) AS typeName,
       CAST(pc.[code] AS varchar(60)) AS [code], CAST(pc.[name] AS varchar(60)) AS [name]
FROM totvs.TotvsProductsClassifications pc
JOIN dw_src.SkusColorsProducts cp ON cp.[code] = pc.productCode AND cp.branchInfoCode = pc.branchInfoCode;
GO
CREATE OR ALTER VIEW dw_src.ProductsSuppliers AS
SELECT cp.referenceId, CAST(cp.colorCode AS varchar(60)) AS colorCode, ps.productCode,
       CAST(cp.sku AS varchar(60)) AS productSku, ps.[code], CAST(ps.cpfCnpj AS varchar(14)) AS cpfCnpj,
       CAST(ps.[name] AS varchar(250)) AS [name], CAST(ps.originalCode AS varchar(60)) AS originalCode, ps.isDefault
FROM totvs.TotvsProductsSuppliers ps
JOIN dw_src.SkusColorsProducts cp ON cp.[code] = ps.productCode AND cp.branchInfoCode = ps.branchInfoCode;
GO
CREATE OR ALTER VIEW dw_src.ProductsAdditionalFields AS
SELECT cp.referenceId, CAST(cp.colorCode AS varchar(60)) AS colorCode, af.productCode,
       CAST(cp.sku AS varchar(60)) AS productSku, af.[code], CAST(af.[name] AS varchar(60)) AS [name],
       CAST(af.[value] AS varchar(250)) AS [value]
FROM totvs.TotvsProductsAdditionalFields af
JOIN dw_src.SkusColorsProducts cp ON cp.[code] = af.productCode AND cp.branchInfoCode = af.branchInfoCode;
GO
```

---

## 10. Validação durante o paralelo

Rode depois de as duas bases estarem na mesma janela. A diferença esperada é zero, ou explicada.

```sql
-- 1) contagem por tabela
SELECT 'Invoices' t, (SELECT COUNT(*) FROM totvs.Invoices) antigo, (SELECT COUNT(*) FROM dw_src.Invoices) novo
UNION ALL SELECT 'InvoicesItems', (SELECT COUNT(*) FROM totvs.InvoicesItems), (SELECT COUNT(*) FROM dw_src.InvoicesItems)
UNION ALL SELECT 'SaleOrders', (SELECT COUNT(*) FROM totvs.SaleOrders), (SELECT COUNT(*) FROM dw_src.SaleOrders)
UNION ALL SELECT 'SkusColorsProducts', (SELECT COUNT(*) FROM totvs.SkusColorsProducts), (SELECT COUNT(*) FROM dw_src.SkusColorsProducts);
-- Invoices: a antiga tem 307 linhas a mais que as notas distintas (a chave antiga não inclui invoiceDate); a nova não deve ter

-- 2) notas que existem só de um lado
SELECT accessKey, branchCode, invoiceSequence, invoiceDate FROM totvs.Invoices
EXCEPT
SELECT accessKey, branchCode, invoiceSequence, invoiceDate FROM dw_src.Invoices;

-- 3) valores por mês e operação (o que os fatos somam)
SELECT FORMAT(invoiceDate,'yyyyMM') mes, operationCode, SUM(totalValue) total, COUNT(*) qtd FROM totvs.Invoices GROUP BY FORMAT(invoiceDate,'yyyyMM'), operationCode
EXCEPT
SELECT FORMAT(invoiceDate,'yyyyMM'), operationCode, SUM(totalValue), COUNT(*) FROM dw_src.Invoices GROUP BY FORMAT(invoiceDate,'yyyyMM'), operationCode;

-- 4) vínculo nota × pedido (pega o erro da seção 4.2)
SELECT invoiceSequence, branchCode, orderCode FROM totvs.InvoicesSalesOrders
EXCEPT
SELECT invoiceSequence, branchCode, orderCode FROM dw_src.InvoicesSalesOrders;

-- 5) a mesma etapa nas duas fontes: copiar a proc para um nome _novo apontando para dw_src, gravando numa cópia da tabela
--    destino, e comparar com EXCEPT (ex.: dw.Dim_Pedido x dw.Dim_Pedido_novo)
```

---

## 11. Pendências e decisões

1. **Operations:** implementar `TotvsOperations` no ETL ou manter o job antigo (seção 3). Corrigir a afirmação do
   `PROPOSTA-PADRONIZACAO-TOTVS.md` de que a rota não existe.
2. **Regra de marca para produtos** (`branchInfoCode`, seção 4.3).
3. **Endereços** com `addressTypeCode` na chave: conferir `P_Dim_Cliente_ETL` (seção 4.5).
4. **Chave de nota sem data** em `dw.eligible_invoices` e no join `InvoicesSalesOrders` só por `invoiceSequence`: é um problema
   que já existe. Corrigir depois da migração, num passo próprio, para não misturar mudança de origem com mudança de regra.
5. **`personType`, `receipt` e `disableProtocol`:** hoje são sempre 0. Ver o que a ingestão nova grava antes de confiar neles.
6. **Consumidores fora do `dw`** (seção 2): precisam de plano próprio antes de apagar as tabelas antigas. Alternativa: depois de
   apagar as antigas, recriar as views `dw_src` com os nomes antigos dentro do schema `totvs`, desde que cubram também as colunas
   que esses objetos usam.
7. O levantamento usa o catálogo do SQL Server. Não aparece aqui o que lê `totvs` de fora do banco (Power BI, planilhas).
