# Proposta: `TotvsOperations` no repo ETL

Revisão 1 — 2026-11-06. **Proposta para aprovar: nenhum arquivo do repo `extract-transform-load-integrations-system` foi
alterado.** Complementa a seção 3 do [guia-migracao-totvs-dw.md](guia-migracao-totvs-dw.md).

## 1. Por que

O `dw` depende de `totvs.Operations` em 9 objetos e em `totvs.Gera_Prefixo_Pedidos`. As colunas usadas são `operationCode`,
`description`, `isFinancial`, `invoiceDataOperationsType`, `invoiceDataDocumentModel`, `invoiceDataOperationMode` e
`invoiceDataPurposeNFE`. Em `P_Dim_Pedido_ETL`, `bate_nota` e `vw_total_acumulado` o join com `Operations` é `INNER`:
uma operação ausente tira a nota do `dw`.

O repo ETL descartou o recurso (`PROPOSTA-PADRONIZACAO-TOTVS.md`, decisão 30: "não existe rota de operações em nenhum swagger
da Totvs; o `SearchOperations` do código antigo nunca existiu"). **A premissa não confere:**

- Os 7 swaggers comparados no `PLANO-TOTVS-METODOS-FALTANTES.md` são accounts-payable, person, seller, product,
  purchase-order, sales-order e fiscal. **A API `general/v2` não está entre eles.**
- O recurring-jobs chama `GET /general/v2/operations` (`newbloomers-recurring-jobs/src/Totvs/Application.Totvs/Services/GeneralService.cs:88`),
  e a rota responde: `totvs.Operations` tem 96 linhas, com `maxChangeFilterDate` de 2026-03-04 a 2026-11-05.
- O `SearchOperations` que "nunca existiu" era só a rota comentada do controller antigo. O job usa outro método.

Proposta: incluir a API **General** como a 8ª API do plano, com uma rota de leitura (catálogo).

## 2. O que o código antigo faz (referência)

| Item | Valor |
|---|---|
| Rota | `GET general/v2/operations` |
| Parâmetros | `StartChangeDate`, `EndChangeDate` (hoje −5 meses até amanhã), `Page`, `PageSize` = 500 |
| Envelope | `count`, `totalPages`, `hasNext`, `totalItems`, `items[]` (o `Page<T>` que o ETL já tem) |
| Item | `operationCode`, `description`, `isFinancial`, `isInactive`, `maxChangeFilterDate`, `invoiceData{…}`, `calculations{…}`, `values[]`, `balances[]`, `classifications[]` |
| `invoiceData` | `operationsType`, `documentType`, `documentModel`, `taxRuleCode`, `operationMode`, `purposeNFE`, `presenceIndicator`, `danfeType`, `linkedOperationCode` |
| `calculations` | `accountingCostType`, `orderBalanceType`, `negativeBalanceType` |
| `values[]` | `valueType`, `code`, `description`, `isDefault` |
| `balances[]` | `code`, `description`, `historicCode`, `historicDescription`, `movementType`, `isDefault`, `isKardex` |
| `classifications[]` | `typeCode`, `typeName`, `code`, `name` |
| Gravação | `totvs.Operations`, com objetos 1:1 achatados como `invoiceData*` e `calculations*`. As filhas `OperationsValues`, `OperationsBalances` e `OperationsClassifications` existem, mas **têm 0 linhas** |
| Correção silenciosa | `operationsType` é **cortado para 1 caractere** (`Entities/General/Operations/Operation.cs:52`) |

Dados atuais (dev): `invoiceDataOperationsType` só tem `S` (63) e `E` (33). `invoiceDataOperationMode` tem
`1`…`7`, `D` e `F`. `invoiceDataDocumentModel` tem 0, 35, 55, 59, 65 e 81. As 74 operações usadas nas notas estão
todas cadastradas.

## 3. Especificação (no formato do PLANO)

### 3.1 `GetOperations` — `GET general/v2/operations` — tipo: **catálogo** (120 min)

| Item | Proposta |
|---|---|
| Fluxo | O de `GetEmailTypes` (`TotvsPersonCatalogsService`): para cada empresa, janela pela maior `maxChangeFilterDate` do banco (T9), paginação com `PageSize` 500, cache `Totvs:Operations:<cnpj>`, somente novos ou alterados para `untreated` e depois `sp_TotvsOperations_Sync` |
| Carga inicial | Sem linha no banco, `StartChangeDate` = 2000-01-01. **A janela de −5 meses do código antigo não traz uma operação que não muda desde antes disso.** As 96 de hoje só existem porque o job antigo roda desde março |
| Chave | `doc_company`, `operationCode` |
| Validações (`Validations/Reads/General`) | `operationCode` inteiro > 0. `documentType`, `documentModel`, `taxRuleCode`, `purposeNFE`, `presenceIndicator`, `danfeType` e `linkedOperationCode` inteiros (como no antigo). Inconsistência vai para `auditing.Messages`, sem corrigir o dado |
| `operationsType` | Gravar **o valor que a API manda**, sem cortar (nvarchar(20)). O corte para 1 caractere fica na view `dw_src.Operations` (`LEFT(..., 1)`), que é quem precisa de `S`/`E`. **A confirmar com chamada real** o que a API devolve (`S`/`E`, `Saída`/`Entrada` ou `Output`/`Input`). Se vier `Output`/`Input`, o corte daria `O`/`I` e a view precisa de `CASE` |
| Exclusão | Nada é apagado: uma operação removida na Totvs fica no banco, como as filiais e os catálogos de pessoa |
| Rota no controller | `GET TotvsJobs/Totvs/Operations` (leitura recorrente, como `GET Branches`) |

### 3.2 Persistência (`untreated` + `totvs`)

`TotvsOperations` — chave `doc_company`, `operationCode`:

| Coluna | Tipo | Origem |
|---|---|---|
| `integrated_or_updated_at` | datetime | |
| `doc_company` | nvarchar(14) not null | |
| `operationCode` | int not null | |
| `description` | nvarchar(300) | |
| `isFinancial`, `isInactive` | bit | |
| `maxChangeFilterDate` | datetime | |
| `invoiceDataOperationsType` | nvarchar(20) | sem corte (3.1) |
| `invoiceDataDocumentType`, `invoiceDataDocumentModel`, `invoiceDataTaxRuleCode` | int | |
| `invoiceDataOperationMode` | nvarchar(60) | valores `1`…`7`, `D`, `F`: texto, não número |
| `invoiceDataPurposeNFE`, `invoiceDataPresenceIndicator`, `invoiceDataDanfeType`, `invoiceDataLinkedOperationCode` | int | |
| `calculationsAccountingCostType`, `calculationsOrderBalanceType`, `calculationsNegativeBalanceType` | nvarchar(60) | |

Os nomes das colunas são os do banco atual, de propósito: a view `dw_src.Operations` vira um `SELECT` direto.
Tamanhos tirados do banco atual; **trocar pelos do swagger `general/v2`** quando ele for baixado.

**Filhas (`values`, `balances`, `classifications`): proposta de não criar agora.** No banco atual elas têm 0 linhas, embora o
código antigo grave as três. Ou a API não as preenche sem algum parâmetro, ou vêm vazias. O `dw` não usa nenhuma.
Ficam para quando uma chamada real mostrar conteúdo (decisão O2).

### 3.3 Arquivos

| Camada | Arquivo | Modelo |
|---|---|---|
| Domain | `Dtos/Reads/General/Operation.cs` (com `InvoiceData` e `Calculations`, tudo `string`) | `Dtos/Reads/People/Catalogs/EmailType.cs` |
| Domain | `Entities/Reads/General/Operation.cs` | `Entities/Reads/People/Catalogs/EmailType.cs` |
| Domain | `Abstractions/Repositorys/ITotvsGeneralRepository.cs` | `ITotvsPersonCatalogsRepository.cs` |
| Domain | `Enums/EnumJob.cs`: `TotvsGetOperations = 13300` | |
| Application | `Abstractions/Services/ITotvsGeneralService.cs`, `Services/TotvsGeneralService.cs` (`GetOperations`) | `TotvsPersonCatalogsService.GetEmailTypes` |
| Application | `Abstractions/Handlers/Commands/ITotvsGeneralCommandHandler.cs`, `Handlers/Commands/TotvsGeneralCommandHandler.cs` | os de PersonCatalogs |
| Application | `Validations/Reads/General/OperationValidator.cs` | `Validations/Reads/People/Catalogs` |
| Infrastructure | `Repositorys/TotvsGeneralRepository.cs` (`GetOperationsLastChange`, `GetOperations`, `BulkInsertIntoTotvsOperations`) | `TotvsPersonCatalogsRepository.cs` |
| Infrastructure | `DependencyInjection.cs`: 3 `AddScoped` | linhas 62–64 |
| Infrastructure | `Scripts/SQLServer/General/Script039_Create_Totvs_General_Tables.sql` | `PersonCatalogs/Script026` |
| Infrastructure | `Scripts/SQLServer/General/Script040_Create_Totvs_General_Sync_Procedures.sql` (`sp_Create_Merge_Procedure` para `TotvsOperations`) | `PersonCatalogs/Script027` |
| Infrastructure | `Scripts/SQLServer/General/Script041_Insert_Totvs_General_Jobs_Into_Jobs_Table.sql` | `PersonCatalogs/Script028` |
| Hosts | `Hosts.Scheduling/RecurringJobs/TotvsRecurringJobs.cs`: `RecurringJobDefinition.For<ITotvsGeneralService>(13300, s => s.GetOperations())` | linhas 24–27 |
| Hosts | `WebApi/Controllers/Totvs/TotvsController.cs`: `GET Operations` | `GET Branches` |
| Docs | `PLANO-TOTVS-METODOS-FALTANTES.md`: nova linha 8 (General) na tabela do topo e seção nova. `PROPOSTA-PADRONIZACAO-TOTVS.md`: decisão 30 revista | |
| Testes | os do catálogo de pessoa, no mesmo projeto | |

Numeração: **job 13300** (faixa General 13300–13309, logo depois da Fiscal 13250–13299) e **procedure de sync 13651**
(a última usada na faixa 13500+ é 13650). Scripts **039–041** (o último do módulo é o 038). Conferir os três números
na hora de implementar, porque outra sessão pode ter usado.

### 3.4 Efeito no `dw`

Com a tabela nova, a view da camada de compatibilidade fica assim:

```sql
CREATE OR ALTER VIEW dw_src.Operations AS
SELECT operationCode, CAST([description] AS varchar(300)) AS [description], isFinancial, isInactive, maxChangeFilterDate,
       CAST(LEFT(invoiceDataOperationsType, 1) AS char(1)) AS invoiceDataOperationsType,   -- ver 3.1
       invoiceDataDocumentType, invoiceDataDocumentModel, invoiceDataTaxRuleCode,
       CAST(invoiceDataOperationMode AS varchar(60)) AS invoiceDataOperationMode,
       invoiceDataPurposeNFE, invoiceDataPresenceIndicator, invoiceDataDanfeType, invoiceDataLinkedOperationCode,
       calculationsAccountingCostType, calculationsOrderBalanceType, calculationsNegativeBalanceType
FROM totvs.TotvsOperations;
```

Validação no paralelo (deve voltar vazio):

```sql
SELECT operationCode, description, isFinancial, invoiceDataOperationsType, invoiceDataDocumentModel,
       invoiceDataOperationMode, invoiceDataPurposeNFE
FROM totvs.Operations
EXCEPT
SELECT operationCode, description, isFinancial, invoiceDataOperationsType, invoiceDataDocumentModel,
       invoiceDataOperationMode, invoiceDataPurposeNFE
FROM dw_src.Operations;
```

## 4. Decisões para aprovar

| # | Decisão | Proposta |
|---|---|---|
| O1 | Incluir a API General no plano do ETL e rever a decisão 30 | Sim |
| O2 | Filhas `values`, `balances`, `classifications` | Não criar agora (0 linhas hoje; o `dw` não usa) |
| O3 | `operationsType` | Gravar cru; o corte para `S`/`E` fica na view do `dw` |
| O4 | Frequência | 120 min, como os outros catálogos (o antigo roda com que frequência? conferir no Hangfire) |
| O5 | Numeração | Job 13300, sync 13651, scripts 039–041 |

## 5. A confirmar antes de implementar

1. Baixar o swagger de `general/v2` (`https://apitotvsmoda.bhan.com.br/api/totvsmoda/general/v2/swagger/index.html`),
   seguindo o padrão dos outros 7, para os tamanhos e para ver se há outras rotas na API General.
2. Uma chamada real para ver o valor de `operationsType` e se as filhas vêm preenchidas.
3. O repo ETL está em `main` com alterações não commitadas de outra sessão (WebApp, appsettings, Shared Script009). A
   implementação precisa de branch própria, e o commit não pode levar esses arquivos.
