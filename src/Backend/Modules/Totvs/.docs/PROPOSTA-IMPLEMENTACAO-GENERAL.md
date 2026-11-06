# Proposta: implementação da API General (`general/v2`) no módulo Totvs

Revisão 3 — 2026-11-06. **Decisões G1–G8 aprovadas (seção 12); chamadas de confirmação feitas em produção, só leitura
(seção 9); nenhum código foi alterado.** Substitui o
[diff-totvs-operations-etl.md](diff-totvs-operations-etl.md), que tratava só de `operations`, e entra como a **8ª API** do
[PLANO-TOTVS-METODOS-FALTANTES.md](PLANO-TOTVS-METODOS-FALTANTES.md), no mesmo formato das seções 2 a 8.

Swagger oficial: `https://www30.bhan.com.br:9443/api/totvsmoda/general/v2/swagger/v1/swagger.json`, título "Api General",
versão **2.8.20**, baixado em 06/11/2026. O host é o mesmo `Totvs:BaseUrl` de produção dos hosts
(`https://www30.bhan.com.br:9443/api/totvsmoda`), e o token de `authorization/v2` vale para o `general/v2` (confirmado em
06/11/2026). O `apitotvsmoda.bhan.com.br` das outras 7 APIs é só a documentação pública.

## 1. Por que

- O `dw` usa `totvs.Operations` em 9 objetos e em `totvs.Gera_Prefixo_Pedidos`. Em `P_Dim_Pedido_ETL`, `bate_nota` e
  `vw_total_acumulado` o join é `INNER`, então uma operação ausente tira a nota do `dw` ([guia-migracao-totvs-dw.md](../../../../../.docs/guia-migracao-totvs-dw.md), seção 3).
- A integração **Frete Rápido** lê `totvs.TransactionsClassifications` para buscar o "TIPO DO FRETE" da nota
  (`FreteRapidoRepository.cs:245`, join por `transactionBranchCode`, `transactionDate`, `transactionCode` e `typeCode`).
  Essa tabela é gravada pelo segundo job do `GeneralService` antigo, **que o diff anterior não cobria**.
- A decisão 30 da `PROPOSTA-PADRONIZACAO-TOTVS.md` ("não existe rota de operações em nenhum swagger") parte de 7
  swaggers, e nenhum deles é o `general/v2`. **Precisa ser revista (G1).**

## 2. O que o código antigo faz (`newbloomers-recurring-jobs`, `GeneralService.cs`)

| Job antigo | Rota | Frequência | Detalhes | Grava |
|---|---|---|---|---|
| `TotvsOperations` | `GET general/v2/operations` | **3 min** (`0 */3 * * * *`) | Janela de hoje −5 meses até amanhã, `PageSize` 500, **sem `Expand`**. Corta `operationsType` em 1 caractere e `description` em 300 | `totvs.Operations` (96 linhas). As filhas `OperationsValues`, `OperationsBalances` e `OperationsClassifications` têm 0 linhas |
| `TotvsTransactionsClassifications` | `POST general/v2/transactions/search` | **5 min** | Todas as lojas de `Branches`, só as operações `[10, 14, 510, 1002, 1004]` (`Totvs:Transactions`), janela das últimas **48 h**, `PageSize` 500. Corta `typeName` em 60, `code` em 60 e `name` em 250 | `totvs.TransactionsClassifications` (só as classificações, sem o cabeçalho da transação) |

**Por que as filhas estão vazias (confirmado):** `values`, `balances`, `classifications` e também `calculations` **só vêm
com `Expand`**. Sem `Expand`, a produção devolve `calculations` nulo e as listas vazias para as 95 operações. Com `Expand`,
vêm 96 valores e 104 saldos. No banco antigo, as 96 linhas têm `calculations*` nulas.

**`operationsType` (confirmado):** só vem `S` (60) e `E` (35). O corte para 1 caractere não muda nada, e a view do `dw` não
precisa de `LEFT` nem de `CASE`.

## 3. Rotas do swagger

São 17 rotas. Em 6 tags: General (2), Transaction (8), ProductCount (2), PaymentPlan (2), Devolution (2) e Omni (1).

| # | Rota | Tag | Tipo proposto | Seção | Job |
|---|---|---|---|---|---|
| 1 | `GET operations` | General | **Catálogo** (substitui o job antigo) | §4.1 | 13300 |
| 2 | `GET payment-conditions` | General | **Catálogo** | §4.2 | 13301 |
| 3 | `GET classifications` | Transaction | **Catálogo** (domínio das classificações de transação, TRAFM101) | §4.3 | 13302 |
| 4 | `POST transactions/search` | Transaction | **Leitura recorrente** (substitui o job antigo). POST, mas é consulta | §5.1 | 13303 |
| 5 | `GET transactions/search` | Transaction | Não usar: igual à 4, mas com a lista de lojas na URL | §5.1 | — |
| 6 | `GET transactions` | Transaction | Sob demanda, sem persistência | §6.1 | 13304 |
| 7 | `GET payment-plans` | PaymentPlan | Sob demanda, sem persistência | §6.2 | 13305 |
| 8 | `POST payment-plan-simulate` | PaymentPlan | Sob demanda, sem persistência. POST, mas é consulta | §6.3 | 13306 |
| 9 | `GET devolutions/search` | Devolution | Sob demanda, sem persistência | §6.4 | 13307 |
| 10 | `POST devolutions/create` | Devolution | Escrita | §7.1 | 13310 |
| 11 | `POST product-counts` | ProductCount | Escrita | §7.2 | 13311 |
| 12 | `POST product-counts/additional-count` | ProductCount | Escrita (altera contagem) | §7.3 | 13312 |
| 13 | `POST transactions` | Transaction | Escrita (cria transação) | §7.4 | 13313 |
| 14 | `POST transaction-classification-relationship/create` | Transaction | Escrita | §7.5 | 13314 |
| 15 | `POST relationship-counts` | Transaction | Escrita | §7.6 | 13315 |
| 16 | `POST transaction-receiving` | Transaction | Escrita, **efeito financeiro** | §7.7 | 13316 |
| 17 | `POST financial-compensations` | Omni | Escrita, **efeito financeiro** | §7.8 | 13317 |

Contagem: **3 catálogos, 1 leitura recorrente, 4 sob demanda e 8 escritas**. A 5 (`GET transactions/search`) fica fora.

Regras comuns, iguais às da Fiscal (§8 do PLANO):

- Os parâmetros `ValidationResult.IsValid`, `ValidationResult.Errors` e `ValidationResult.RuleSetsExecuted` dos `GET` vazam
  do modelo interno da Totvs e **nunca são enviados**.
- Os enums do swagger são `integer` mas só listam os nomes. **Na resposta, vêm os códigos da descrição, não os nomes**
  (confirmado): `calculations` como número (`1`, `99`, `7`, `5`), `valueType` e `movementType` como letra (`P`/`C`,
  `E`/`S`/`N`), `status` da transação como número (`4`, `6`). As leituras gravam o código. Nas escritas, usar o número da
  descrição (regra do PLANO, §1.3).
- Arrays na query no formato `X=1&X=2` (padrão do ASP.NET), confirmado com `OperationCodeList`.
- Lista vazia responde **200 com `items: []`**, inclusive em `GET operations`, que declara 404 no swagger (confirmado).

## 4. Catálogos

Os três seguem o fluxo de `TotvsPersonCatalogsService.GetEmailTypes`: para cada empresa, uma janela de alteração, paginação,
cache `Totvs:<Recurso>:<cnpj>`, só os registros novos ou alterados vão para `untreated`, e depois roda a procedure de sync.
Nada é apagado: um registro removido na Totvs fica no banco, como as filiais e os outros catálogos.

### 4.1 `GetOperations` — `GET general/v2/operations` — job 13300

| Item | Proposta |
|---|---|
| Parâmetros | `StartChangeDate`, `EndChangeDate`, `Order` = `maxChangeFilterDate`, `Page`, `PageSize` = **1000** (máximo do swagger; o antigo usa 500), **`Expand` = `calculations,values,balances,classifications`**. Não enviar `IsInactive` (traz ativas e inativas) nem `OperationTypeList` |
| Janela | `MAX(maxChangeFilterDate)` do banco − 1 h. Banco vazio: `GetInitialLoad(2000-01-01)`, como os catálogos de pessoa. A janela de −5 meses do antigo não traz uma operação que não muda há mais tempo |
| Resposta | `OperationResponseModel` (o `Page<T>` que o módulo já tem). Vazio = 200 com `items: []`. Hoje são **95 operações em 1 página** de 1000. `maxChangeFilterDate` só vem preenchido quando as datas são enviadas |
| Frequência | **3 min**, como o antigo (G4). Sem mudança, a janela incremental devolve 0 itens em uma chamada. Com 120 min (o padrão dos catálogos), uma nota com operação nova sumiria do `dw` por até 2 h por causa do `INNER JOIN` |
| Chave | `doc_company`, `operationCode` |
| Cache | `Totvs:Operations:<cnpj>` |
| Rota do controller | `GET TotvsJobs/Totvs/Operations` |

**DTO `Dtos/Reads/General/Operation.cs`** (tudo `string`, como os `Dtos/Reads`), com `InvoiceData`, `Calculations`,
`OperationValue`, `OperationBalance` e `OperationClassification`.

**Validações (`Validations/Reads/General/OperationValidator.cs`).** Uma inconsistência vai para `auditing.Messages`, e o dado
é gravado sem correção.
- `operationCode` inteiro > 0.
- `operationsType` em `E`/`S`. Outro valor gera aviso, e o valor é gravado mesmo assim.
- `documentType`, `documentModel`, `taxRuleCode`, `purposeNFE`, `presenceIndicator`, `danfeType` e `linkedOperationCode`
  inteiros **ou nulos**. Em produção, `documentModel` e `purposeNFE` vêm nulos em 1 operação; `presenceIndicator`, `danfeType`
  e `linkedOperationCode` vêm nulos na maioria.
- `operationMode` com 1 caractere (`1`…`9`, `A`…`H`). Hoje vêm `1`–`5`, `7`, `D` e `F`.
- `calculations.*` inteiros ou nulos.
- Filhas: `values[].code` e `balances[].code` inteiros; `valueType` em `P`/`C`; `movementType` em `E`/`S`/`N`;
  `classifications[].typeCode` inteiro.

**Tabelas** (`untreated` e `totvs`). Os nomes das colunas da principal são os do banco atual, para a view do `dw` ser um
`SELECT` direto:

| Tabela | Chave | Colunas |
|---|---|---|
| `TotvsOperations` | `doc_company`, `operationCode` | `integrated_or_updated_at`, `description` nvarchar(300), `isFinancial`, `isInactive` bit, `maxChangeFilterDate` datetime, `invoiceDataOperationsType` nvarchar(20), `invoiceDataDocumentType`, `invoiceDataDocumentModel`, `invoiceDataTaxRuleCode` int, `invoiceDataOperationMode` nvarchar(60), `invoiceDataPurposeNFE`, `invoiceDataPresenceIndicator`, `invoiceDataDanfeType`, `invoiceDataLinkedOperationCode` int (anuláveis), `calculationsAccountingCostType`, `calculationsOrderBalanceType`, `calculationsNegativeBalanceType` int (anuláveis; vêm como código) |
| `TotvsOperationsValues` | `doc_company`, `operationCode`, `valueType`, `code` | `valueType` nvarchar(1), `description` nvarchar(100), `isDefault` bit |
| `TotvsOperationsBalances` | `doc_company`, `operationCode`, `code` | `description` nvarchar(100), `historicCode` int, `historicDescription` nvarchar(100), `movementType` nvarchar(1), `isDefault`, `isKardex` bit |
| `TotvsOperationsClassifications` | `doc_company`, `operationCode`, `typeCode`, `code` | `typeName`, `name` nvarchar(100) |

O swagger não dá tamanho para os textos. Os tamanhos saíram da produção com folga: `description` da operação até 69
(`nvarchar(300)`, o do banco atual), descrições de valor e saldo até 17, `historicDescription` até 50. As filhas são
**substituídas inteiras** no sync de cada operação alterada, como as classificações de pessoa. Uma linha que sai da Totvs sai
do banco junto com a substituição. `balances[].code` é único por operação (104 saldos, nenhuma repetição), e `values` é único
por `valueType` + `code`. `classifications` vem vazia nas 95 operações de produção: a tabela é criada mesmo assim (G2).

**Operação removida na Totvs:** a 8000 ("SAIDA FISICO TRANSFERINDO PARA DEFEITO") está no banco antigo, mas não vem da API
nem com `OperationCodeList=8000`. Nenhuma nota do banco antigo usa essa operação, então a tabela nova fica sem ela, e a
validação do paralelo a exclui (seção 8).

**Mudança em relação ao diff anterior (G2):** as filhas **entram agora**. Estavam vazias por falta de `Expand`, não por falta
de dado. Isso segue a regra de ler os dados completos.

### 4.2 `GetPaymentConditions` — `GET general/v2/payment-conditions` — job 13301

Condições de pagamento (GERFM001). É o domínio de `paymentConditionCode` das notas e dos pedidos. Também é o campo
obrigatório do `POST transactions` (§7.4).

| Item | Proposta |
|---|---|
| Parâmetros | `Page`, `PageSize` = **100**. O swagger diz que o máximo é 1000, mas a produção recusa: `"Value 1000 of the field pageSize greater than limit of 100."`. Sem datas e sem `PaymentConditionCodeList` |
| Janela | **Leitura inteira a cada execução, sem datas.** A rota aceita a chamada sem `StartChangeDate`/`EndChangeDate`, e a resposta é idêntica à com datas desde 2000. O `PaymentConditionModel` **não traz `maxChangeFilterDate`**, como o centro de custo da Fiscal (§8.3 do PLANO). Hoje são **21 condições** em 1 página |
| "Só regrava quando muda" | Hash SHA-256 do JSON de cada condição numa coluna `json_hash`, igual ao `GetCostCenter` |
| Frequência | 120 min |
| Chave | `doc_company`, `code` |
| Rota do controller | `GET TotvsJobs/Totvs/PaymentConditions` |

**Tabelas:** `TotvsPaymentConditions` (`code` int, `name` nvarchar(100), `installment` int, `discountPercentage`,
`interestPercentage` decimal(9,4), `isBlocked` bit, `json_hash`) e `TotvsPaymentConditionsInstallmentDays` (`doc_company`,
`code`, `installmentNumber` (posição no array, começando em 1), `days` int). A filha é substituída inteira junto com a
condição. Em produção, `name` tem até 28 caracteres, e `discountPercentage`, `interestPercentage` e `isBlocked` vêm nulos na
maioria: as colunas são anuláveis e o dado é gravado como vem.

**Validações:** `code` inteiro > 0. `installment` igual ao número de itens de `installmentDays` (aviso, sem bloquear; nas 21 de
hoje sempre bate). Os dias em ordem crescente (aviso).

### 4.3 `GetTransactionClassifications` — `GET general/v2/classifications` — job 13302

Tipos e classificações de transação (TRAFM101). É o domínio de `typeCode`/`code` do §5.1 e das escritas §7.1 e §7.5. Mesmo
fluxo do `GetClassifications` de pessoa.

| Item | Proposta |
|---|---|
| Parâmetros | `StartChangeDate`, `EndChangeDate`, `Page`, `PageSize` = 1000. Sem `TypeCodeList` (máximo de 120 itens, não é necessário) |
| Janela | `MAX(maxChangeFilterDate)` − 1 h; banco vazio desde 2000-01-01 |
| Frequência | 120 min |
| Chave | `doc_company`, `typeCode`, `code` |
| Cache | `Totvs:TransactionClassifications:<cnpj>` |
| Rota do controller | `GET TotvsJobs/Totvs/TransactionClassifications` |

**Tabela** `TotvsTransactionClassificationTypes`: `typeCode` int, `typeName`, `code`, `name`, `maxChangeFilterDate`. O nome
é diferente da tabela por transação do §5.1 para não confundir as duas.

## 5. Leitura recorrente

### 5.1 `PostSearchTransactions` — `POST general/v2/transactions/search` — job 13303

Substitui o `TotvsTransactionsClassifications` antigo. Usa a versão `POST`: a lista de lojas vai no corpo, não na URL, e é
a que o antigo usa.

| Item | Proposta |
|---|---|
| Corpo | `branchCodeList*` (lojas da empresa), `startChangeDate`, `endChangeDate`, `order` = `maxChangeFilterDate`, `page`, `pageSize` = **500** (máximo), `expand` = `originDestination,additionalInformation,installment,itemPromotionalEngines,itemCommissions,scannedBarcodes`. **Sem `operationCodeList`** (G5) |
| Janela | `MAX(maxChangeFilterDate)` − 1 h, em **fatias de 30 dias**. A API aceita no máximo 60 dias entre `startChangeDate` e `endChangeDate`. Carga inicial desde 01/01/2025 (igual às notas) |
| Frequência | **5 min**, como o antigo (o Frete Rápido depende disso) |
| Chave da transação | `doc_company`, `branchCode`, `transactionDate` (só a data), `transactionCode`. É a chave de `Invoices.transactionBranchCode/transactionDate/transactionCode` |
| Cache | `Totvs:Transactions:<cnpj>` (chave → `maxChangeFilterDate`) |
| Rota do controller | `GET TotvsJobs/Totvs/Transactions` |
| Lojas | Do `TotvsBranches` da empresa. Lojas demais para um corpo: blocos de 100 (**a confirmar** se há limite) |

**Tabelas** (`untreated` e `totvs`). As filhas são substituídas inteiras quando a transação muda.

| Tabela | Chave além da transação | Colunas |
|---|---|---|
| `TotvsTransactions` | — | `customerCode`, `representativeCode`, `operationCode`, `sellerCode`, `guideCode`, `paymentConditionCode`, `priceTableCode`, `settlementCode` int; `status`, `modality`, `transactionType` nvarchar; `grossAmountValue`, `totalValue`, `discountValue` decimal(14,2); `lastchangeDate`, `maxChangeFilterDate` datetime; `additionalSimulationPercent`, `additionalSimulationValue`, `additionalTerminalCode` (1:1 achatado); `fiscalBranchCode`, `fiscalInvoiceSequence`, `fiscalInvoiceDate`, `fiscalInvoiceCode`, `fiscalSerialCode`, `fiscalIssueDate`, `fiscalQuantity`, `fiscalTotalValue` (`relatedFiscalDocument` 1:1 achatado); `originBranchCode`, `originTransactionCode`, `originTransactionDate` (1:1 achatado) |
| `TotvsTransactionsClassifications` | `typeCode` | `typeName`, `code`, `name` (sem corte) |
| `TotvsTransactionsItems` | `itemSequence` | `productCode`, `measuredUnit`, `quantity` decimal(12,3), `value`, `unitDiscountValue`, `netValue` decimal(14,3), `sellerCode`, `priceTable` |
| `TotvsTransactionsItemsPromotionalEngines` | `itemSequence`, `sequence` | `engineSequence`, `engineDescription` nvarchar(400), `mechanicsSequence`, `mechanicsDescription` nvarchar(400), `previousValue`, `currentValue` decimal(18,6) |
| `TotvsTransactionsItemsCommissions` | `itemSequence`, `commissionedPersonCode`, `commissionedType` | `commissionBaseValue`, `billingCommissionPercent`, `billingCommissionValue`, `receiptCommissionPercent`, `receiptCommissionValue` |
| `TotvsTransactionsItemsScannedBarcodes` | `itemSequence`, `barCode` | `isRfid` bit, `rfidStatus` |
| `TotvsTransactionsInstallments` | `installmentNumber` | `dueDate`, `installmentValue` |
| `TotvsTransactionsDestinations` | `branchCodeDestination`, `transactionCodeDestination`, `transactionDateDestination` | — |

**`itemSequence`:** o `TransactionItemsDataModel` **não tem número de item**, e o `productCode` não serve de chave: em 2 das 58
transações de um dia da loja 1, o mesmo produto aparece em mais de um item. A chave é a posição no array (1, 2, 3…). Duas
chamadas seguidas devolveram respostas idênticas byte a byte, então a ordem é estável. Como as filhas são substituídas
inteiras a cada mudança, uma troca de ordem não duplicaria nada, só renumeraria.

**O que veio em produção** (loja 1, alterações de 05/11/2026, 58 transações em 1 página):

| Dado | Resultado |
|---|---|
| Volume | 58 transações e 653 itens no dia; até 154 itens numa transação. A loja 1049 não teve nada no dia |
| Operações | 14 códigos, entre eles 14 e 510 (que o antigo lê) e 6, 42, 512, 2003, 2004 (que o antigo ignora) |
| Códigos | `status` número (`4` atendida, `6` cancelada), `modality` `"1"`/`"2"`, `transactionType` `S`/`E`. `maxChangeFilterDate` sempre preenchido |
| Expand | `installment` em 50, `relatedFiscalDocument` em 52 (datas `yyyy-MM-dd`), `originDestination` em todas (quase sempre vazio), `additionalInformation` em todas (campos nulos), `commissions` em 374 itens. `promotionalEngines` e `scannedBarcodes` vazios. `additionalItem.priceTable` nulo |
| Classificações | **Nenhuma.** No banco antigo, `totvs.TransactionsClassifications` tem **1 linha** (loja 1, 28/10/2026): a classificação de frete é rara. A tabela nova cobre esse caso, mas o volume de teste vai ser pequeno |

**Validações (`Validations/Reads/General/TransactionValidator.cs`):** `branchCode`, `transactionCode` inteiros > 0;
`transactionDate` data válida; `operationCode` inteiro; `classifications[].typeCode` inteiro; valores numéricos. Aviso quando
o `operationCode` não existe em `TotvsOperations` (dá para ver se o catálogo atrasou).

**Compatibilidade com o Frete Rápido:** a view `dw_src.TransactionsClassifications` mantém as colunas da tabela antiga:

```sql
CREATE OR ALTER VIEW dw_src.TransactionsClassifications AS
SELECT c.branchCode, c.transactionDate, c.transactionCode, c.typeCode,
       c.typeName, c.code, c.[name], t.operationCode, t.maxChangeFilterDate
FROM totvs.TotvsTransactionsClassifications c
JOIN totvs.TotvsTransactions t
  ON t.doc_company = c.doc_company AND t.branchCode = c.branchCode
 AND t.transactionDate = c.transactionDate AND t.transactionCode = c.transactionCode;
```

A tabela antiga só tem as operações `[10, 14, 510, 1002, 1004]`. A view nova traz todas. Isso não muda o resultado do Frete
Rápido, que filtra pela nota (o join é pela chave da transação da nota). A validação no paralelo é feita com o mesmo filtro
de operação (seção 8).

## 6. Leituras sob demanda, sem persistência

Usam a base do §8.1 do PLANO: `Result` com o conteúdo, nova tentativa em 401/429/5xx, `TotvsRequests` com o JSON, sem
`auditing.Records`. As respostas da rota são 200, 404, 422, 429 e 502.

| § | Método | Rota da Totvs | Rota do controller | Entrada |
|---|---|---|---|---|
| 6.1 | `GetTransaction` | `GET transactions` | `GET Totvs/{docCompany}/Transactions/{branchCode}/{transactionCode}?transactionDate=` | `BranchCode`, `TransactionCode`, `TransactionDate`, `Expand` = `itemPromotionalEngines,originDestination` |
| 6.2 | `GetPaymentPlans` | `GET payment-plans` | `GET Totvs/{docCompany}/PaymentPlans?branchCode=` | `BranchCode` (planos **ativos** da loja) |
| 6.3 | `PostPaymentPlanSimulate` | `POST payment-plan-simulate` | `POST Totvs/{docCompany}/PaymentPlans/Simulate` | `branchCode*`, `paymentPlanCode*`, `saleValue*`, `simulationValue`, `paymentItems[]` |
| 6.4 | `GetSearchDevolution` | `GET devolutions/search` | `GET Totvs/{docCompany}/Devolutions/{branchCode}/{devolutionCode}` | `BranchCode*`, `DevolutionCode*`, `Expand` = `classifications,items` |

Por que esses não são persistidos: os planos de pagamento e a simulação são do PDV (valor do momento). A transação avulsa e a
devolução já são cobertas pelo §5.1, ou são consulta pontual depois de uma escrita (§7.1).

Validações de entrada: códigos inteiros > 0 dentro dos tamanhos do swagger (loja 4 dígitos, transação e devolução 9). Na 6.3,
`installmentQuantity` > 0 para `CreditCard` e `Invoice`, e `installments[]` só para `Invoice`.

## 7. Escritas

Todas seguem a base de escrita do §1 do PLANO (`Write`, chave de idempotência, `TotvsRequests`, sem nova tentativa
automática depois de 5xx, 201/200 → sucesso, 400 → 422 com as mensagens da Totvs). Testes de escrita **só em homologação**,
com o usuário pessoal. Produção só para leitura.

| § | Método | Rota | Risco | Validações principais (swagger) |
|---|---|---|---|---|
| 7.1 | `PostDevolutions` | `POST devolutions/create` | Médio (cria devolução no estágio) | `branchCode*` (4), `operationCode*` (4), exatamente um entre `personCode` (9) e `personCpfCnpj` (11/14 dígitos), `returnDate*`, `fiscalDocumentType*`, `items*` ≥ 1 com exatamente um entre `productCode` (9) e `productSku` (40); nota de origem por `invoiceBranchCode`+`invoiceSequence`+`invoiceDate` (os três juntos) **ou** `accessKey` (44), nunca os dois; `descriptionDevolution` 60, `descriptionDocOrigin` 30, `invoiceSerie` 3, `authorizationNumber` 20; quantidades e valores com 9 inteiros e 3 decimais; classificações `classificationTypeCode` (4) e `classificationCode` (10) |
| 7.2 | `PostProductCounts` | `POST product-counts` | Médio (cria contagem, padrão "Encerrada") | `branchCode*`, `items*` ≥ 1 com `productCode` ou `productBarCode`; lote por `barCode` (40) **ou** `branchCode`+`code`+`sequence`, nunca os dois; `countType` e `status` nos enums; `productionOrder`: `originPlaceCode` e `originProgrammingSequenceCode` obrigatórios só com `relationshipType` = 3; `additionalFields[].value` string, número ou data `DD/MM/YYYY` |
| 7.3 | `PostProductCountsAdditionalCount` | `POST product-counts/additional-count` | Médio (altera contagem existente) | `branchCode*`, `countNumber*` (9); demais como a 7.2, sem itens |
| 7.4 | `PostTransactions` | `POST transactions` | **Alto** (cria transação de venda/PDV) | `branchCode`, `operationCode`, `paymentConditionCode` (conferidos em `TotvsOperations` e `TotvsPaymentConditions`), exatamente um entre `customerCode` e `customerCpfCnpj` (a não ser com `isFinalCNSR`), exatamente um entre `sellerCode` e `sellerCpfCnpj`, no máximo um entre `representactiveCode` e `representactiveCpfCnpj`; `totalAmountTransaction*` = soma de `(value − discountValue) × quantity` dos itens (a Totvs confere); itens com `quantity*`, `value*`, `cfop` (5), exatamente um entre `productCode` e `productSku`; `observations[].observation` 80; `componentCode`, `mobileVersion`, `peripheralPdvCode`, `versionPdv` 20; `status` só os 4 do `TransactionStatusType` |
| 7.5 | `PostTransactionClassificationRelationship` | `POST transaction-classification-relationship/create` | Baixo | `branchCode*`, `transactionCode*`, `transactionDate*`, `classifications*` ≥ 1 com `typeCode*` e `code*` (conferidos em `TotvsTransactionClassificationTypes`) |
| 7.6 | `PostRelationshipCounts` | `POST relationship-counts` | Baixo | `branchCode*` (4), `transactionCode*` (9), `transactionDate*` (data), `counts*` ≥ 1 com `branchCode*` e `countNumber*` |
| 7.7 | `PostTransactionReceiving` | `POST transaction-receiving` | **Alto, financeiro** | `branchCode*`, `transactionCode*`, `transactionDate*`, `totalAmount*` = soma de `receipts[].documentAmount`, `terminalCode*`, `receipts*` ≥ 1. A Totvs só aceita transação "Em andamento", documento interno, operação de saída e modalidade venda |
| 7.8 | `PostFinancialCompensations` | `POST financial-compensations` | **Alto, financeiro** (RCF do Omni) | `orderCode*`, `splitCode*`, `document*` (CNPJ, 14 dígitos), `value*` > 0, `accessKey*` (44), `items*` ≥ 1 com `productCode*` e `integrationCode*` |

Rotas do controller: `POST TotvsJobs/Totvs/{docCompany}/<Recurso>`, como as escritas das outras APIs (`Devolutions`,
`ProductCounts`, `ProductCounts/AdditionalCount`, `Transactions`, `Transactions/Classifications`, `Transactions/Counts`,
`Transactions/Receiving`, `FinancialCompensations`).

Nenhuma escrita tem caso de uso conhecido hoje. Mesmo assim, **as 8 entram** (G6), pela mesma regra da Fiscal ("implementada
mesmo sem caso de uso", F1/T14). As de risco alto (7.4, 7.7 e 7.8) criam venda ou movimentam financeiro. Por isso são as
últimas da ordem de execução e só são testadas em homologação, com transação e valores de teste criados para isso.

### 7.9 Estranho no swagger

- `CreateDevolutionItemRequestCommand` marca `productCode` **e** `productSku` como obrigatórios, mas a descrição diz que
  não podem vir juntos. Vale a descrição: exatamente um dos dois.
- `PaymentPlanSimulateQuery.branchCode` está descrito como "Código do plano de pagamento". É a loja.
- `InsertCompleteTransactionCommand` escreve `representactiveCode`/`representactiveCpfCnpj` com erro de digitação. O DTO usa
  o nome **como está** no swagger.
- `StatusPlanType` tem a descrição de "Tipo de documento das faturas", mas os valores são Ativo/Inativo.
- `transactionDate` é `date` em `POST transactions`, `relationship-counts` e `transaction-receiving`, e `date-time` nas
  demais. Mandar `yyyy-MM-dd` nas três primeiras.
- `GET payment-plans` não descreve `BranchCode` nem o marca como obrigatório.
- `GET operations` declara 404, mas a lista vazia vem 200 com `items: []`.
- `GET payment-conditions` diz que o máximo de `PageSize` é 1000, mas a produção limita em 100.
- Os enums são declarados com nomes, mas a resposta traz os códigos (seção 3).

## 8. Efeito no `dw` e validação no paralelo

```sql
CREATE OR ALTER VIEW dw_src.Operations AS
SELECT operationCode, CAST([description] AS varchar(300)) AS [description], isFinancial, isInactive, maxChangeFilterDate,
       CAST(invoiceDataOperationsType AS char(1)) AS invoiceDataOperationsType,
       ISNULL(invoiceDataDocumentType, 0) AS invoiceDataDocumentType,
       ISNULL(invoiceDataDocumentModel, 0) AS invoiceDataDocumentModel,
       ISNULL(invoiceDataTaxRuleCode, 0) AS invoiceDataTaxRuleCode,
       CAST(invoiceDataOperationMode AS varchar(60)) AS invoiceDataOperationMode,
       ISNULL(invoiceDataPurposeNFE, 0) AS invoiceDataPurposeNFE,
       ISNULL(invoiceDataPresenceIndicator, 0) AS invoiceDataPresenceIndicator,
       ISNULL(invoiceDataDanfeType, 0) AS invoiceDataDanfeType,
       ISNULL(invoiceDataLinkedOperationCode, 0) AS invoiceDataLinkedOperationCode,
       CAST(calculationsAccountingCostType AS nvarchar(60)) AS calculationsAccountingCostType,
       CAST(calculationsOrderBalanceType AS nvarchar(60)) AS calculationsOrderBalanceType,
       CAST(calculationsNegativeBalanceType AS nvarchar(60)) AS calculationsNegativeBalanceType
FROM totvs.TotvsOperations;
```

Os `ISNULL(..., 0)` reproduzem o antigo, que grava `0` quando a API manda nulo (no banco antigo, a operação com
`documentModel` e `purposeNFE` nulos na API está com `0`). A tabela nova guarda o nulo, e só a view converte.

Com mais de uma empresa, a view precisa de um `WHERE doc_company = '<cnpj>'` ou de `DISTINCT`, porque `totvs.Operations` não
tem `doc_company`. Hoje só uma empresa usa a Totvs. **Confirmar antes de criar a view.**

Validações (devem voltar vazias):

```sql
-- operações
SELECT operationCode, description, isFinancial, invoiceDataOperationsType, invoiceDataDocumentModel,
       invoiceDataOperationMode, invoiceDataPurposeNFE
FROM totvs.Operations
WHERE operationCode <> 8000   -- removida na Totvs, sem notas (seção 4.1)
EXCEPT
SELECT operationCode, description, isFinancial, invoiceDataOperationsType, invoiceDataDocumentModel,
       invoiceDataOperationMode, invoiceDataPurposeNFE
FROM dw_src.Operations;

-- classificações de transação (só as operações que o antigo lê, e só o período que os dois cobrem)
SELECT branchCode, transactionDate, transactionCode, typeCode, code, [name]
FROM totvs.TransactionsClassifications
WHERE transactionDate >= '2025-01-01'
EXCEPT
SELECT branchCode, transactionDate, transactionCode, typeCode, code, [name]
FROM dw_src.TransactionsClassifications
WHERE operationCode IN (10, 14, 510, 1002, 1004);
```

A 2ª consulta pode mostrar diferença em `name` com mais de 250 caracteres (o antigo corta). Essa diferença é esperada.

Os jobs antigos `TotvsOperations` e `TotvsTransactionsClassifications` continuam ligados até as duas validações ficarem vazias
por alguns dias. Só depois o `dw` e o Frete Rápido passam a ler das views.

## 9. Confirmações feitas (06/11/2026, produção, só leitura)

Script com lista fechada de rotas (`authorization/v2/token`, `GET operations`, `GET payment-conditions` e
`POST transactions/search`), credenciais lidas do seed `Seeds/SQLServer/Script003` sem impressão, TLS ligado. O banco antigo
foi lido em `sql-dev-webjobs` (NEWBLOOMERS), só com `SELECT`. `POST payment-plan-simulate` não foi chamado.

| # | Pergunta | Resposta |
|---|---|---|
| 1 | `GET operations` com `Expand` completo | 200. 95 operações em 1 página. `operationsType` só `S`/`E`. Enums como código. `values` (96) e `balances` (104) preenchidos, `classifications` vazia. Sem repetição de `balances[].code`. Textos curtos (até 69). Vazio = 200 com `items: []` (§4.1) |
| 2 | `calculations*` nulas no banco antigo | Sim: 0 de 96 preenchidas. Sem `Expand`, a API devolve `calculations` nulo e as listas vazias |
| 3 | `GET payment-conditions` com e sem datas | `PageSize` 1000 → 400 (limite real 100). Com 100: 200, 21 condições, mesma resposta com e sem datas (§4.2) |
| 4 | `POST transactions/search`, 1 loja, 1 dia | 200. 58 transações e 653 itens (loja 1). Duas chamadas idênticas byte a byte. `maxChangeFilterDate` sempre preenchido. Nenhuma classificação (§5.1) |
| 5 | Host do `general/v2` | O mesmo `Totvs:BaseUrl` de produção dos hosts, com o mesmo token |
| — | Diferença com o banco antigo | O antigo tem 96 operações e a API 95: a 8000 foi removida na Totvs e nenhuma nota a usa (§4.1) |

Respostas salvas no scratchpad da sessão, fora do repositório (têm dados de clientes).

## 10. Arquivos

| Camada | Arquivo | Modelo |
|---|---|---|
| Domain | `Dtos/Reads/General/` (`Operation`, `PaymentCondition`, `TransactionClassificationType`, `Transaction` e as classes aninhadas), `Dtos/Writes/General/` (um por escrita) | `Dtos/Reads/People/Catalogs`, `Dtos/Writes/AccountsPayable` |
| Domain | `Entities/Reads/General/` (uma por tabela) | `Entities/Reads/People/Catalogs` |
| Domain | `Abstractions/Repositorys/ITotvsGeneralRepository.cs` | `ITotvsPersonCatalogsRepository.cs` |
| Domain | `Enums/EnumJob.cs`: 13300–13307 e 13310–13317 | |
| Application | `Abstractions/Services/ITotvsGeneralService.cs`, `Services/TotvsGeneralService.cs` (um service por API, como a Fiscal) | `TotvsPersonCatalogsService`, `TotvsInvoicesService` |
| Application | `Abstractions/Handlers/Commands/ITotvsGeneralCommandHandler.cs`, `Handlers/Commands/TotvsGeneralCommandHandler.cs` | os de PersonCatalogs |
| Application | `Validations/Reads/General/`, `Validations/Writes/General/` | `Validations/Reads/People/Catalogs`, `Validations/Writes/AccountsPayable` |
| Infrastructure | `Repositorys/TotvsGeneralRepository.cs` | `TotvsPersonCatalogsRepository.cs` |
| Infrastructure | `DependencyInjection.cs`: `AddScoped` do service, handler e repository | |
| Infrastructure | `Scripts/SQLServer/General/Script039_Create_Totvs_General_Tables.sql`, `Script040_Create_Totvs_General_Sync_Procedures.sql`, `Script041_Insert_Totvs_General_Jobs_Into_Jobs_Table.sql` | `PersonCatalogs/Script026–028`, `Invoices/Script025` (hash) |
| Hosts | `Hosts.Scheduling/RecurringJobs/TotvsRecurringJobs.cs`: 13300 (3 min), 13301 e 13302 (120 min), 13303 (5 min) | |
| Hosts | `WebApi/Controllers/Totvs/TotvsController.cs`: rotas das seções 4 a 7 | `GET Branches`, rotas da Fiscal |
| Docs | `PLANO-TOTVS-METODOS-FALTANTES.md`: linha 8 (General) na tabela do topo e seção 9 com o conteúdo daqui. `PROPOSTA-PADRONIZACAO-TOTVS.md`: decisão 30 revista. `diff-totvs-operations-etl.md`: marcar como substituído | |
| Testes | Unitários dos validadores e das entidades, e integração dos syncs no banco de teste do Docker, com a API simulada (`BaseUrl` para `127.0.0.1:9` ao subir um host) | os do catálogo de pessoa e da Fiscal |

**Numeração** (conferida em 06/11/2026; conferir de novo ao implementar, porque outra sessão pode usar):

- **Jobs** na faixa General **13300–13349**, logo depois da Fiscal (13250–13299): leituras de 13300 a 13307, escritas de 13310
  a 13317.
- **Procedures de sync** a partir de **13651**. A última usada é 13650 (`sp_TotvsCostCentersClassifications_Sync`). São 4
  de operações, 2 de condições de pagamento, 1 de classificações de transação e 8 de transações: **13651–13665**.
- **Scripts** **039–041**. O último do módulo é o `Bonus/Script038`.

## 11. Ordem de execução

1. ~~Chamadas de confirmação da seção 9 (só leitura).~~ Feitas em 06/11/2026.
2. §4.1 operações, com as filhas → view `dw_src.Operations` → validação no paralelo. **Desbloqueia o `dw`.**
3. §4.3 classificações de transação e §5.1 transações → view `dw_src.TransactionsClassifications` → validação no paralelo.
   **Desbloqueia o Frete Rápido.**
4. §4.2 condições de pagamento.
5. §6 sob demanda.
6. §7 escritas de risco baixo e médio (7.1, 7.2, 7.3, 7.5, 7.6), testadas em homologação.
7. §7 escritas de risco alto (7.4, 7.7, 7.8), testadas em homologação.

## 12. Decisões

| # | Decisão | Situação | Resolução |
|---|---|---|---|
| G1 | Incluir a API General como a 8ª API do plano e rever a decisão 30 | **Decidida em 06/11/2026** | Sim: vira a seção 9 do PLANO, e a decisão 30 é corrigida |
| G2 | Filhas de operação (`values`, `balances`, `classifications`) | **Decidida em 06/11/2026** | **Criar agora, com `Expand`.** Muda a O2 do diff anterior: estavam vazias por falta de `Expand` |
| G3 | `operationsType` | **Decidida em 06/11/2026** | Gravar o valor que vier em `nvarchar(20)` (o swagger diz `E`/`S`), sem corte, com aviso em `auditing.Messages` se vier outro |
| G4 | Frequência de `GetOperations` | **Decidida em 06/11/2026** | **3 min**, como o antigo |
| G5 | Filtro de operação em `transactions/search` | **Decidida em 06/11/2026** | Ler **todas** as operações. O filtro `[10, 14, 510, 1002, 1004]` fica só na validação |
| G6 | Escritas | **Decidida em 06/11/2026** | **Todas as 8**, mesmo sem caso de uso (regra da Fiscal). As de risco alto ficam por último |
| G7 | Expand de `transactions/search` | **Decidida em 06/11/2026** | Todos os grupos (seção 5.1), com as 8 tabelas |
| G8 | Numeração | **Decidida em 06/11/2026** | Jobs 13300–13317, sync 13651–13665, scripts 039–041 (conferir de novo ao implementar) |
