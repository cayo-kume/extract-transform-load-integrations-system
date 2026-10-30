# Pacote de DLLs para Integração com a Totvs Moda

## O que é a Totvs Moda?
A **Totvs Moda** é o ERP da Totvs para o varejo de moda: cadastro de pessoas e lojas, produtos (referências, cores e tamanhos), pedidos de compra e de venda, notas fiscais e financeiro. A Newbloomers usa o ERP para as marcas **Misha** e **Open Era**, com todas as lojas sob a holding.

### Conceitos usados pela integração
* **Empresas (filiais)**: as lojas da holding, cada uma com um código (`branchCode`). A holding Newbloomers (CNPJ `44065634000106`) é a empresa `900` na Totvs.
* **Matriz da marca**: a empresa base de cada marca nas buscas de produtos (`1` para Misha, `101` para Open Era).
* **Pessoas**: clientes pessoa física e pessoas jurídicas, com endereços, telefones, e-mails, classificações e estatísticas por loja.
* **Referências e produtos**: a referência é o modelo; cada cor e tamanho dela é um produto (SKU), com códigos de barras, classificações e fornecedores.
* **Documentos**: pedidos de compra, pedidos de venda, notas fiscais e duplicatas a pagar.

---

## Sobre o pacote

Módulo do sistema de integrações (ETL) responsável por **ler** os dados da API da Totvs Moda e gravá-los no banco SQL Server:

* de forma **recorrente**, pelos jobs do Hangfire, com leitura incremental por data de alteração;
* **sob demanda**, a partir de um sistema externo, buscando um único registro por chave e registrando quem pediu cada chamada.

O módulo começou a **escrever** na Totvs (plano em [`docs/PLANO-TOTVS-METODOS-FALTANTES.md`](../../../../docs/PLANO-TOTVS-METODOS-FALTANTES.md)): por enquanto, só a troca de estágio de uma parcela a pagar. Toda escrita é validada antes de chamar a API, fica registrada em `totvs.TotvsRequests` e só roda com a chave de configuração do seu grupo ligada.

| Item | Valor |
|---|---|
| Plataforma | .NET 10 (`net10.0`) |
| Arquitetura | Clean Architecture em três assemblies: Domain, Application e Infrastructure |
| API consumida | Totvs Moda — `https://www30.bhan.com.br:9443/api/totvsmoda/` (autenticação OAuth2, fluxo `password`) |
| Documentação da API | Swagger de cada API em `.../api/totvsmoda/<api>/v2/swagger/index.html` (`person`, `seller`, `product`, `purchase-order`, `sales-order`, `accounts-payable`, `fiscal`) |
| Banco de dados | SQL Server (schemas `untreated`, `totvs` e `auditing`; o `Script001` cria `totvs` quando não existe) |
| Multiempresa | Sim: cada CNPJ de `totvs.TotvsParameters` tem as suas credenciais, e as buscas usam **todas as lojas** dele gravadas em `totvs.TotvsBranches`. O CNPJ (`doc_company`) é a primeira coluna da chave de todas as tabelas |
| Host | `src/Backend/Hosts/Hangfire` (controller `TotvsController`) |

### Comportamentos da API que a integração trata
A documentação não descreve tudo; o que segue foi conferido com chamadas reais (detalhes em [`docs/PROPOSTA-PADRONIZACAO-TOTVS.md`](../../../../docs/PROPOSTA-PADRONIZACAO-TOTVS.md), §5.2):

* **Datas no horário local, sem fuso.** Um `Z` no filtro é ignorado; os filtros vão no formato `yyyy-MM-ddTHH:mm:ss`.
* **`maxChangeFilterDate` só vem quando o filtro `change` é enviado.** As leituras por chave também mandam um `change`, para a data vir preenchida e o job não regravar o registro.
* **Limite do `change` por rota:** notas fiscais aceitam no máximo **6 meses**; referências, **1 ano**. As leituras recorrentes usam fatias de 7 ou 30 dias.
* **Enums vêm pelo nome** (`"Issued"`, `"Output"`), não pela letra do swagger; a exceção é o status das duplicatas (`"N"`).
* **A paginação das referências é por produto:** `count` é o total de produtos, e uma referência pode vir partida em duas páginas. Com `change`, vem só com os produtos alterados na janela.
* **Notas com `referencedInvoices`:** para algumas devoluções de venda a busca responde `400` (`InvoiceDomainService.GetReferencedInvoices`). O campo está **comentado** no `expand` das notas; o contorno (reler a página em partes) continua no método para quando for religado.
* **`branchCodeList` é obrigatório** nas buscas de documentos.

Os JSONs usados para conferir a modelagem estão na pasta [`Responsed Jsons`](#pasta-responsed-jsons), com os dados reais trocados por dados fictícios.

---

## Arquitetura

```
Totvs/
├── Domain.Totvs          ← contratos, DTOs, entidades e enums (sem dependência de infraestrutura)
├── Application.Totvs     ← services (regras de integração), queries SQL e validações
├── Infrastructure.Totvs  ← repositórios, injeção de dependência e scripts de banco
└── Responsed Jsons       ← uma resposta de exemplo por rota (dados fictícios)
```

### Referências entre projetos

```
Infrastructure.Totvs ──► Application.Totvs ──► Domain.Totvs
        │                        │                    │
        ▼                        ▼                    ▼
Infrastructure.Shared    Application.Shared     Domain.Shared
```

| Assembly | Pacotes NuGet | Referências |
|---|---|---|
| `Domain.Totvs` | `Newtonsoft.Json` 13.0.4 | `Domain.Shared` |
| `Application.Totvs` | `Microsoft.Extensions.Caching.Memory` 10.0.8 | `Application.Shared`, `Domain.Totvs` |
| `Infrastructure.Totvs` | — | `Infrastructure.Shared`, `Application.Totvs`, `Domain.Totvs` |

Do projeto Shared o módulo usa:
* `IApiClient.SendAsync` (cliente HTTP que devolve status, corpo e cabeçalhos);
* `ILoggerService` (auditoria);
* `ISharedRepository` (consultas, bulk insert e chamada das procedures de merge);
* `ConverterValidatorsExtensions` e `NullableConverters` (conversões de string para os tipos das entidades, mantendo `null` quando a API não manda o valor).

---

## Domain.Totvs

```
Domain.Totvs/
├── Abstractions/Repositorys/   ← interfaces dos repositórios
├── Dtos/Reads/                 ← respostas das buscas da API
│   ├── Authorization/          (Token)
│   ├── Base/                   (Page<T>: count, totalPages, hasNext, totalItems, items)
│   ├── People/                 (Branches, Customers, LegalEntities e Persons: as filhas comuns a PF e PJ)
│   ├── Sellers/
│   ├── Products/               (Reference, Color, Product, Grid, Detail, Barcode, Classification...)
│   ├── PurchaseOrders/
│   ├── SalesOrders/
│   ├── AccountsPayable/        (Duplicate, Expense, CommissionPaid, AccountMovement...)
│   └── Invoices/               (Invoice, Item, ItemProduct, ItemTax, Payment, SalesOrder...)
├── Entities/
│   ├── Parameters/             (Company → totvs.TotvsParameters)
│   ├── Reads/                  ← o que é gravado a partir das buscas, nas mesmas pastas dos DTOs
│   └── Writes/
│       └── Requests/           (Request → totvs.TotvsRequests)
└── Enums/                      (EnumJob)
```

### Convenções

* **DTOs** espelham o JSON da API: propriedades `string` com os nomes da API. É **uma classe por arquivo**.
* **Entidades** têm os tipos das colunas e os **mesmos nomes das colunas**, porque o bulk insert mapeia as colunas pelo nome da propriedade.
  * Convertem os campos do DTO no construtor e cortam os textos no tamanho da coluna.
  * Campos opcionais ficam anuláveis (`int?`, `decimal?`, `DateTime?`), para gravar `null` quando a API não manda o valor.
* **Objetos 1:1** da API (por exemplo, `eletronic` e `person` da nota, `individualAdditionals` da PF, `cardInformation` do pagamento) viram **colunas do pai**, com o nome do objeto como prefixo (`eletronicAccessKey`, `personPersonName`).
* Toda entidade tem `doc_company` (CNPJ) e `integrated_or_updated_at`. Um registro e todas as suas filhas recebem **o mesmo horário de lote** (`TotvsService.GetBatchTime()`), que é a base da exclusão das filhas no merge.
* Filhas sem chave na API usam a posição na lista como `sequence`. Filhas de documentos levam a chave inteira do pai (ex.: loja + número do pedido).
* Listas filhas usam `[SkipProperty]` para não virarem coluna no bulk insert.
* `EnumJob` deve ficar em sincronia com os scripts de jobs de cada recurso (`Script0NN_Insert_Totvs_*_Jobs_Into_Jobs_Table.sql`).

### Repositórios (interfaces)

| Interface | Responsabilidade |
|---|---|
| `ITotvsRepository` | Empresas (`GetCompanys`), lojas da empresa e da marca (`GetBranchCodes`, `GetBrandBranchCodes`) e log das chamadas sob demanda (`InsertIntoTotvsRequests`) |
| `ITotvsPeopleRepository` | Filiais, PF e PJ (bulk insert + merges), última alteração e consulta por códigos para o cache |
| `ITotvsSellersRepository` | Vendedores e filhas |
| `ITotvsProductsRepository` | Referências (`TotvsSkus`) e filhas, por empresa e matriz |
| `ITotvsPurchaseOrdersRepository` | Pedidos de compra e filhas |
| `ITotvsSalesOrdersRepository` | Pedidos de venda e filhas |
| `ITotvsAccountsPayableRepository` | Duplicatas e comissões pagas, com filhas |
| `ITotvsInvoicesRepository` | Notas fiscais e filhas |

---

## Application.Totvs

```
Application.Totvs/
├── Abstractions/
│   ├── Handlers/Commands/      (ITotvsCommandHandler e um ITotvs<Service>CommandHandler por service)
│   └── Services/               (ITotvsService e um por recurso)
├── Handlers/Commands/          (monta as queries SQL: um Totvs<Service>CommandHandler por service, usado pelo repositório dele, e o TotvsCommandHandler com as comuns)
├── Services/                   (TotvsService, TotvsPeopleService, TotvsSellersService, TotvsProductsService,
│                                TotvsPurchaseOrdersService, TotvsSalesOrdersService, TotvsAccountsPayableService, TotvsInvoicesService)
└── Validations/                (FluentValidation por DTO, nas mesmas pastas dos DTOs)
```

Os nomes dos métodos são os da **documentação oficial** (`PostSearch*`, `GetBranchesList`), e cada método faz as próprias chamadas à API; não há classe base.

### Services e métodos

**`TotvsService`**: operações comuns aos outros services

| Método | O que faz |
|---|---|
| `GetBatchTime` | Horário do lote (horário de Brasília), gravado igual no registro e em todas as filhas |
| `GetCompanys` / `GetCompany` | Empresas de `totvs.TotvsParameters` (em cache) / uma empresa pelo CNPJ |
| `GetBranchCodes` / `GetBrandBranchCodes` | Lojas da empresa / lojas da marca (mesma raiz de CNPJ da matriz), lidas de `totvs.TotvsBranches` sem cache |
| `GetToken` / `RemoveToken` | Token do `authorization/v2/token` em cache até 5 min antes do `expires_in`; um 401 tira o token do cache |
| `SaveRequest` | Grava uma chamada sob demanda em `totvs.TotvsRequests`, ligada ao `Execution` do log aberto por quem chama. A rota é gravada sem o token, e o filtro enviado vai em `request_body`. Documentos da pessoa física, conta bancária e chave PIX são mascarados nos dois corpos (T6, `TotvsMasking`): ficam os 2 últimos caracteres, e campo de CPF ou CNPJ só é mascarado quando traz CPF. A rota e o `field_key_value` ficam inteiros, para a busca e a trava de repetição |

**Leituras recorrentes**

| Service | Método | Rota da API | Tabelas | Job | Intervalo |
|---|---|---|---|---|---|
| `TotvsPeopleService` | `GetBranchesList` | `GET person/v2/branchesList` | `TotvsBranches` e endereços | 13007 | 120 min |
| `TotvsPeopleService` | `PostSearchCustomers` | `POST person/v2/individuals/search` | `TotvsPeople` e 12 filhas | 13002 | 3 min |
| `TotvsPeopleService` | `PostSearchLegalEntity` | `POST person/v2/legal-entities/search` | `TotvsLegalEntities` e 15 filhas | 13001 | 3 min |
| `TotvsSellersService` | `PostSearchSellers` | `POST seller/v2/search` | `TotvsSellers` e 2 filhas | 13006 | 120 min |
| `TotvsSellersService` | `GetOperationalArea` | `GET seller/v2/operational-area`, `-state`, `-cep` e `-city` (inteiras, com hash por região) | `TotvsOperationalAreas` e 3 filhas | 13070 | 120 min |
| `TotvsProductsService` | `PostSearchReferences` | `POST product/v2/references/search` (uma chamada por marca) | `TotvsSkus` e 21 filhas/netas | 13000 | 120 min |
| `TotvsProductPricesService` | `PostSearchPrices` | `POST product/v2/prices/search` (todos os códigos, uma chamada por loja; carga inicial por produto) | `TotvsPrices` e outras promoções | 13015 | 3 min |
| `TotvsProductPricesService` | `PostSearchCosts` | `POST product/v2/costs/search` (idem) | `TotvsCosts` | 13016 | 3 min |
| `TotvsProductPricesService` | `PostSearchPriceTables` | `POST product/v2/price-tables/search` (tabelas vigentes de `TotvsPriceTables`, uma chamada por tabela) | `TotvsPriceTablesItems` e preços | 13092 | 120 min |
| `TotvsProductPricesService` | `GetPromotion` | `GET product/v2/promotion` (cada loja inteira, com hash) | `TotvsPromotions` | 13093 | 120 min |
| `TotvsProductsService` | `PostSearchBalances` | `POST product/v2/balances/search` (idem, com pendentes e locais) | `TotvsBalances` e locais | 13017 | 3 min |
| `TotvsProductsService` | `GetSimilar` | `GET product/v2/similar` (inteira, com hash) | `TotvsProductsSimilar` e itens | 13094 | 120 min |
| `TotvsProductsService` | `PostSearchBatch` | `POST product/v2/batch/search` (cada loja inteira, com hash) | `TotvsBatches`, itens e campos adicionais | 13095 | 120 min |
| `TotvsProductsService` | `PostSearchProductCodes` | `POST product/v2/product-codes/search` (conferência por marca; recupera as referências que faltam) | `TotvsSkus` e filhas | 13097 | diário |
| `TotvsPurchaseOrdersService` | `PostSearchPurchaseOrders` | `POST purchase-order/v2/search` | `TotvsPurchaseOrders` e 2 filhas | 13004 | 3 min |
| `TotvsSalesOrdersService` | `PostSearchSalesOrders` | `POST sales-order/v2/orders/search` | `TotvsSaleOrders` e 10 filhas/netas | 13005 | 3 min |
| `TotvsSalesOrdersService` | `GetDiscountType` | `GET sales-order/v2/discount-type` | `TotvsSaleDiscountTypes` | 13190 | 120 min |
| `TotvsSalesOrdersService` | `GetClassifications` | `GET sales-order/v2/classifications` | `TotvsSaleClassifications` | 13191 | 120 min |
| `TotvsSalesOrdersService` | `GetBatchItems` | `GET sales-order/v2/batch-items` (lotes de pedidos) | `TotvsSaleOrderBatches` | 13192 | 120 min |
| `TotvsSalesOrdersService` | `GetBillingSuggestions` | `GET sales-order/v2/billing-suggestions` | `TotvsBillingSuggestions`, pedidos e itens | 13194 | 3 min |
| `TotvsAccountsPayableService` | `PostSearchDuplicates` | `POST accounts-payable/v2/duplicates/search` | `TotvsDuplicates` e 2 filhas | 13008 | 3 min |
| `TotvsAccountsPayableService` | `PostSearchComissionsPaid` | `POST accounts-payable/v2/comissions-paid/search` (uma chamada por loja) | `TotvsCommissionsPaid` e movimentos | 13009 | 3 min |
| `TotvsInvoicesService` | `PostSearchInvoices` | `POST fiscal/v2/invoices/search` | `TotvsInvoices` e 9 filhas/netas | 13003 | 3 min |
| `TotvsInvoicesService` | `GetInvoicesDisable` | `GET fiscal/v2/invoices/disable` (todas as lojas na rota) | `TotvsInvoicesDisable` | 13250 | 120 min |
| `TotvsInvoicesService` | `GetCostCenter` | `GET fiscal/v2/cost-center` (inteira, com hash) | `TotvsCostCenters` e classificações | 13251 | 120 min |
| `TotvsRepresentativesService` | `PostSearchRepresentatives` | `POST person/v2/representatives/search` | `TotvsRepresentatives` e 10 filhas | 13030 | 120 min |
| `TotvsRepresentativesService` | `GetSearchReceptionSale` | `GET person/v2/reception-sale/search` (pela data do movimento, com hash) | `TotvsReceptionSales` e formas de pagamento | 13031 | 3 min |
| `TotvsPersonCatalogsService` | `GetClassifications` | `GET person/v2/classifications` | `TotvsPersonClassifications` | 13032 | 120 min |
| `TotvsPersonCatalogsService` | `GetEmailTypes` | `GET person/v2/email-types` | `TotvsEmailTypes` | 13033 | 120 min |
| `TotvsPersonCatalogsService` | `GetPhoneTypes` | `GET person/v2/phone-types` | `TotvsPhoneTypes` | 13034 | 120 min |
| `TotvsPersonCatalogsService` | `GetContactTypes` | `GET person/v2/contact-types` | `TotvsContactTypes` | 13035 | 120 min |
| `TotvsProductCatalogsService` | `GetClassificationType` | `GET product/v2/classificationType` | `TotvsClassificationTypes` | 13080 | 120 min |
| `TotvsProductCatalogsService` | `GetClassifications` | `GET product/v2/classifications` | `TotvsProductClassifications` | 13081 | 120 min |
| `TotvsProductCatalogsService` | `GetCategory` | `GET product/v2/category` | `TotvsCategories` | 13082 | 120 min |
| `TotvsProductCatalogsService` | `GetGrid` | `GET product/v2/grid` | `TotvsGrids` e 1 filha | 13083 | 120 min |
| `TotvsProductCatalogsService` | `GetMeasurementUnit` | `GET product/v2/measurement-unit` | `TotvsMeasurementUnits` e 1 filha | 13084 | 120 min |
| `TotvsProductCatalogsService` | `PostSearchColors` | `POST product/v2/colors/search` (inteira, com hash) | `TotvsColors` e 1 filha | 13085 | 120 min |
| `TotvsProductCatalogsService` | `GetAdditionalFieldsTypes` | `GET product/v2/additional-fields-types` | `TotvsAdditionalFieldsTypes` | 13086 | 120 min |
| `TotvsProductCatalogsService` | `GetProductGrouperConfiguration` | `GET product/v2/product-grouper-configuration` | `TotvsProductGrouperConfigurations` e 1 filha | 13087 | 120 min |
| `TotvsProductCatalogsService` | `GetInstructionItems` | `GET product/v2/instruction-items` | `TotvsInstructionItems` | 13088 | 120 min |
| `TotvsProductCatalogsService` | `PostCompositions` | `POST product/v2/compositions` | `TotvsCompositions` e 1 filha | 13089 | 120 min |
| `TotvsProductCatalogsService` | `GetPriceTablesHeaders` | `GET product/v2/price-tables-headers` | `TotvsPriceTables` e 5 filhas | 13090 | 120 min |
| `TotvsProductCatalogsService` | `GetPriceTableScales` | `GET product/v2/price-table-scales` | `TotvsPriceTableScales` e 1 filha | 13091 | 120 min |

**Escritas** (sistema externo, com `X-User-Name`)

| Service | Método | Rota da API | Job | Chave de configuração |
|---|---|---|---|---|
| `TotvsAccountsPayableService` | `PostDuplicatesInstallmentsStage` | `POST accounts-payable/v2/duplicates/installments/stage` | 13020 | `Totvs:Writes:AccountsPayable:Enabled` |
| `TotvsAccountsPayableService` | `PostDuplicates` | `POST accounts-payable/v2/duplicates` (com a trava de repetição) | 13018 | `Totvs:Writes:AccountsPayable:Enabled` |
| `TotvsAccountsPayableService` | `PostGroupDuplicates` | `POST accounts-payable/v2/group-duplicates` (com a trava de repetição) | 13019 | `Totvs:Writes:AccountsPayable:Enabled` |
| `TotvsPeopleService` | `PostMessages` | `POST person/v2/messages` (com a trava de repetição) | 13044 | `Totvs:Writes:People:Enabled` |
| `TotvsPeopleService` | `PutChangeMessages` | `PUT person/v2/messages/change` | 13045 | `Totvs:Writes:People:Enabled` |
| `TotvsPeopleService` | `DeleteMessages` | `DELETE person/v2/messages` | 13046 | `Totvs:Writes:People:Enabled` |
| `TotvsPeopleService` | `PostIndividualCustomers` | `POST person/v2/individual-customers` (inclusão e alteração) | 13040 | `Totvs:Writes:People:Enabled` |
| `TotvsPeopleService` | `PostLegalCustomers` | `POST person/v2/legal-customers` (inclusão e alteração) | 13041 | `Totvs:Writes:People:Enabled` |
| `TotvsRepresentativesService` | `PostCustomerRepresentatives` | `POST person/v2/customer-representatives` | 13042 | `Totvs:Writes:People:Enabled` |
| `TotvsPeopleService` | `PostRemoveClassificationRelationship` | `POST person/v2/classification-relationship/remove` | 13043 | `Totvs:Writes:People:Enabled` |
| `TotvsBonusService` | `PostBonusDueDate` | `POST person/v2/bonus-due-date` (efeito financeiro; com a trava de repetição) | 13047 | `Totvs:Writes:Bonus:Enabled` |
| `TotvsBonusService` | `PostBonusConsume` | `POST person/v2/bonus-consume` (efeito financeiro; saldo conferido antes; com a trava de repetição) | 13048 | `Totvs:Writes:Bonus:Enabled` |
| `TotvsProductCatalogsService` | `PostColors` | `POST product/v2/colors` (inclusão e alteração) | 13124 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductCatalogsService` | `PostGrids` | `POST product/v2/grids` (só inclui: código ou nome existente volta 400 `AlreadyExist`; com a trava de repetição) | 13126 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductCatalogsService` | `PostClassifications` | `POST product/v2/classifications` (inclusão e alteração; sem o tipo 6000) | 13125 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostBarcodes` | `POST product/v2/barcodes` | 13120 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostUpdateBarcodes` | `POST product/v2/barcodes/update` | 13121 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostCreateClassificationRelationship` | `POST product/v2/classification-relationship/create` (sem o tipo 6000) | 13122 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostUpdateClassificationRelationship` | `POST product/v2/classification-relationship/update` (sem o tipo 6000) | 13123 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostReferences` | `POST product/v2/references` (com a trava de repetição; sem preços) | 13127 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostCreateBatch` | `POST product/v2/batch/create` (com a trava de repetição) | 13135 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PutBatchInfo` | `PUT product/v2/batch-info` | 13136 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PostBatchPreferentialSalesOrder` | `POST product/v2/batch-preferentialsalesorder` | 13137 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PutProducts` | `PUT product/v2/products/{code}/{branchCode}` | 13128 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PutBranchInfo` | `PUT product/v2/branch-info/{branchCode}` | 13129 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductsService` | `PutData` | `PUT product/v2/data` | 13130 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductPricesService` | `PostCreateValues` | `POST product/v2/values/create` (códigos e lojas permitidos, trava de variação; código que já existe: 400 `AlreadyExist`) | 13131 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductPricesService` | `PostUpdateValues` | `POST product/v2/values/update` (códigos e lojas permitidos, trava de variação) | 13132 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductPricesService` | `PostUpdatePromotionValues` | `POST product/v2/promotion-values/update` | 13133 | `Totvs:Writes:Products:Enabled` |
| `TotvsProductPricesService` | `PostPromotionItems` | `POST product/v2/promotion-items` (com a trava de repetição) | 13134 | `Totvs:Writes:Products:Enabled` |
| `TotvsPurchaseOrdersService` | `PostPurchaseOrdersChangeStatus` | `POST purchase-order/v2/orders/change-status` | 13182 | `Totvs:Writes:PurchaseOrders:Enabled` |
| `TotvsPurchaseOrdersService` | `PostPurchaseOrders` | `POST purchase-order/v2/orders` (com a trava de repetição) | 13180 | `Totvs:Writes:PurchaseOrders:Enabled` |
| `TotvsPurchaseOrdersService` | `PostPurchaseOrdersQuantityItems` | `POST purchase-order/v2/quantity-items` | 13181 | `Totvs:Writes:PurchaseOrders:Enabled` |
| `TotvsPurchaseOrdersService` | `PostPurchaseOrderBatchLink` | `POST purchase-order/v2/purchase-order/batch-link` | 13184 | `Totvs:Writes:PurchaseOrders:Enabled` |
| `TotvsPurchaseOrdersService` | `PostPurchaseOrdersCancel` | `POST purchase-order/v2/orders/cancel` (irreversível) | 13183 | `Totvs:Writes:PurchaseOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostAdditionalOrder` | `POST sales-order/v2/additional-order` | 13201 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostShippingOrder` | `POST sales-order/v2/shipping-order` | 13202 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostObservationsOrder` | `POST sales-order/v2/observations-order` (com a trava de repetição) | 13203 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostAdditionalItems` | `POST sales-order/v2/additional-items` | 13207 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostOrdersChangeStatus` | `POST sales-order/v2/orders/change-status` | 13199 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostHeader` | `POST sales-order/v2/header` | 13200 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostItems` | `POST sales-order/v2/items` (com a trava de repetição) | 13205 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `DeleteItems` | `DELETE sales-order/v2/items` | 13206 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostQuantityItems` | `POST sales-order/v2/quantity-items` | 13208 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostRelationshipCounts` | `POST sales-order/v2/relationship-counts` (com a trava de repetição) | 13211 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostRelationshipBatchItems` | `POST sales-order/v2/relationship-batch-items` (com a trava de repetição) | 13212 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `DeleteBatchItems` | `DELETE sales-order/v2/batch-items` | 13213 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostSuggestBatchItemsNuance` | `POST sales-order/v2/suggest-batch-items-nuance` (com a trava de repetição) | 13214 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostB2COrders` | `POST sales-order/v2/b2c-orders` (orderId conferido antes; com a trava de repetição) | 13197 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostOrdersCancel` | `POST sales-order/v2/orders/cancel` (irreversível) | 13198 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostPdvTransactions` | `POST sales-order/v2/pdv-transactions` (com a trava de repetição) | 13204 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostPriceItems` | `POST sales-order/v2/price-items` (pedido do e-commerce, inclusive o B2C: 400 `CannotChangedOrderEcommerce`) | 13209 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsSalesOrdersService` | `PostCancelItems` | `POST sales-order/v2/cancel-items` (com a trava de repetição; pedido do e-commerce, inclusive o B2C: 400 `CannotChangedOrderEcommerce`) | 13210 | `Totvs:Writes:SalesOrders:Enabled` |
| `TotvsInvoicesService` | `PostInvoicesManifestations` | `POST fiscal/v2/invoices/manifestations` (efeito fiscal a confirmar; XML fora do log) | 13258 | `Totvs:Writes:Invoices:Enabled` |

**Leituras sob demanda** (um registro por chave)

| Service | Método | Busca na API | Job |
|---|---|---|---|
| `TotvsPeopleService` | `GetCustomerByCode` | `individuals/search` com `personCodeList` | 13010 |
| `TotvsPeopleService` | `GetLegalEntityByCode` | `legal-entities/search` com `personCodeList` | 13011 |
| `TotvsSalesOrdersService` | `GetSalesOrderByKey` | `orders/search` com `branchCodeList` + `orderCodeList` | 13012 |
| `TotvsInvoicesService` | `GetInvoiceByKey` | `invoices/search` com `branchCodeList` + `invoiceSequenceList` | 13013 |
| `TotvsInvoicesService` | `GetXmlContents` | `GET fiscal/v2/xml-contents/{accessKey}` (sem gravar) | 13252 |
| `TotvsInvoicesService` | `PostSearchDanfe` | `xml-contents` + `POST fiscal/v2/danfe-search` (devolve o PDF) | 13253 |
| `TotvsInvoicesService` | `PostTransactionPrint` | `POST fiscal/v2/transaction/print` (devolve o PDF) | 13254 |
| `TotvsInvoicesService` | `GetSearchInvoicesItemDetail` | `GET fiscal/v2/invoices/item-detail-search` (sem gravar) | 13255 |
| `TotvsInvoicesService` | `GetInvoicesPendingConditionalProducts` | `GET fiscal/v2/invoices/pending-conditional-products` (sem gravar) | 13256 |
| `TotvsInvoicesService` | `PostSearchInvoiceProducts` | `POST fiscal/v2/invoice-products/search` (sem gravar) | 13257 |
| `TotvsPurchaseOrdersService` | `GetPurchaseOrderByKey` | `purchase-order/v2/search` com `branchCodeList` + `orderCodeList` (grava) | 13185 |
| `TotvsPeopleService` | `GetBranches`, `GetPersonStatistics` | `GET person/v2/branches/{branchId}`, `GET person/v2/person-statistics` (sem gravar) | 13036, 13037 |
| `TotvsBonusService` | `PostListBalanceBonus` | `POST person/v2/list-balance-bonus` (sem gravar) | 13038 |
| `TotvsProductsService` | `GetProducts`, `GetKardexMovement`, `GetGridFrequency`, `GetMeasurementConversions`, `GetCompositionProduct`, `GetCompositionGroupProduct` | `product/v2/products/{code}`, `kardex-movement`, `grid-frequency`, `measurement-conversions`, `composition-product`, `composition-group-product` (sem gravar) | 13100–13105 |
| `TotvsSalesOrdersService` | `GetInvoices`, `GetPendingItems`, `GetChangedBatchItems` | `sales-order/v2/invoices`, `pending-items`, `changed-batch-items` (sem gravar) | 13195, 13196, 13193 |
| `TotvsRepresentativesService` | `GetRepresentativeByCode` | `representatives/search` com `representativeCodeList` | 13039 |
| `TotvsProductsService` | `GetProductByCode` | `references/search` com `productCodeList` (acha a referência) e `referenceCodeList` (lê a referência inteira) | 13014 |

### Fluxo das leituras recorrentes

Seguem o padrão de [`docs/PADRAO-CACHE-INTEGRACOES.md`](../../../../docs/PADRAO-CACHE-INTEGRACOES.md):

1. abre um log em `auditing.Logs` com o job do método;
2. para cada empresa de `TotvsParameters`, obtém o token e as lojas de `TotvsBranches`. Sem lojas, registra a mensagem e segue para a próxima empresa (por isso o `GetBranchesList` roda primeiro);
3. calcula a janela: desde o maior `maxChangeFilterDate` gravado, menos 1 hora de folga. Com o banco vazio, a carga inicial é:

   | Recurso | Carga inicial |
   |---|---|
   | Filiais, PF, PJ, vendedores e produtos | desde 01/01/2000 |
   | Notas fiscais e pedidos de venda | desde 01/01/2025 |
   | Pedidos de compra | últimos 5 meses |
   | Duplicatas | últimos 10 meses |
   | Comissões pagas | últimos 10 meses pela data do fechamento, relidos a cada execução (a rota não tem filtro de alteração) |

4. lê a janela em **fatias** de 30 dias (produtos: 30 na carga inicial e 7 depois; filiais e vendedores, que são poucos, numa chamada só). Cada fatia é gravada ao terminar; uma fatia com erro interrompe a empresa, e a próxima execução continua do `MAX(maxChangeFilterDate)`;
5. trata a resposta: 401 renova o token e tenta de novo; 429 espera o `Retry-After`; 502/503/504 espera 10 s. JSON inválido conta como falha;
6. monta um cache em memória **por empresa** (`IMemoryCache`, 2 h absoluto / 30 min deslizante) com o que já está gravado e descarta o que não mudou;
7. valida os DTOs (FluentValidation) e registra as inconsistências em `auditing.Messages`, sem barrar a gravação;
8. converte para entidades, faz o bulk insert em `untreated` e chama as procedures `totvs.sp_<Tabela>_Sync`, do pai para os filhos;
9. grava cada registro integrado (com o JSON recebido) em `auditing.Records` e fecha o log num `finally`.

| Chave de cache | Conteúdo |
|---|---|
| `Totvs:Parameters` | Empresas e credenciais de `TotvsParameters` |
| `Totvs:Token:<cnpj>` | Token da empresa |
| `Totvs:Branches:<cnpj>`, `Totvs:Customers:<cnpj>`, `Totvs:LegalEntities:<cnpj>`, `Totvs:Sellers:<cnpj>` | Cadastros gravados (código → registro) |
| `Totvs:Skus:<cnpj>:<matriz>` | Referências gravadas, por marca |
| `Totvs:PurchaseOrders:<cnpj>`, `Totvs:SalesOrders:<cnpj>`, `Totvs:Duplicates:<cnpj>`, `Totvs:Invoices:<cnpj>` | Documentos gravados (chave → registro) |
| `Totvs:CommissionsPaidHash:<cnpj>` | Hash do JSON de cada fechamento de comissão |

### Fluxo das leituras sob demanda

1. abre um log em `auditing.Logs` com o job do método;
2. busca a empresa pelo CNPJ recebido. Se ela não estiver cadastrada, devolve `null` sem chamar a API;
3. chama a busca da API com a lista de chave e `pageSize` 1 e grava o registro **sempre**, pelo mesmo método de gravação do job (mesmas tabelas, merges, validação e `auditing.Records`); o cache do job é atualizado se estiver carregado;
4. grava cada chamada em `totvs.TotvsRequests`, também quando ela falha antes de ter resposta;
5. devolve `true` (gravado), `false` (não existe na Totvs) ou `null` (empresa não cadastrada ou resposta inválida).

Particularidades:
* **Nota:** primeiro com `change` dos últimos 6 meses; se não vier, uma segunda chamada sem `change`, e a nota é gravada sem `maxChangeFilterDate`.
* **Produto:** a busca por `productCodeList` traz a referência só com o produto pedido; por isso uma segunda busca por `referenceCodeList` lê a referência inteira, unindo as páginas. A referência é gravada sem `maxChangeFilterDate`.

---

## Infrastructure.Totvs

```
Infrastructure.Totvs/
├── DependencyInjection.cs      (AddScopedTotvsServices)
├── Repositorys/                (TotvsRepository e um por recurso)
└── Scripts/
    ├── SQLServer/              (migrator.csx, Parameters/ com os scripts gerais e uma pasta por service)
    ├── MySQL/                  (migrator.csx)
    └── PostgreSQL/             (migrator.csx)
```

### Injeção de dependência

`AddScopedTotvsServices(IServiceCollection, IConfiguration)` registra:

* `IApiClient` com a chave `"TotvsAPI"` e o `HttpClient` nomeado `"TotvsAPI"` (base `Totvs:BaseUrl`, timeout de 20 minutos);
* os services, os repositórios e os command handlers, um por service mais o comum `ITotvsCommandHandler` (`Scoped`);
* todos os validadores do assembly numa chamada só (`AddValidatorsFromAssemblyContaining<SellerValidator>()`).

O host chama o método em `Hosts/Hangfire/Extensions/ServicesExtensions.cs`.

### Scripts de banco (SQL Server)

O DbUp executa cada script uma única vez. Alterar um script que já rodou não muda um banco existente: para isso é preciso um script novo. Os scripts gerais do módulo ficam em `Parameters/` e cada service tem a sua pasta, com um trio de scripts: tabelas, procedures e jobs (os recorrentes, os sob demanda e as escritas do service). O `migrator.csx` lê as subpastas e executa na ordem do número do arquivo, não na da pasta.

| Pasta | Script | Conteúdo |
|---|---|---|
| `Parameters/` | `Script001` | Schema `totvs` e `totvs.TotvsParameters` |
| `Parameters/` | `Script002` | App `130 - Totvs` em `auditing.Apps` |
| `Parameters/` | `Script003` | A holding em `TotvsParameters`, com as credenciais de produção, como o AfterSale e o Movidesk. As de homologação ficam no mesmo arquivo, **comentadas**: no banco de homologação ou local, rodar à mão no lugar do insert de produção (duas linhas da mesma empresa fariam o host usar a credencial errada) |
| `Parameters/` | `Script004` | `totvs.TotvsRequests` (log das chamadas sob demanda e das escritas, com `status_code` e `integrated_or_updated_at`) |
| `People/` | `Script005`–`007` | Filiais e pessoas físicas e jurídicas (leituras, escritas e sob demanda) |
| `Sellers/` | `Script008`–`010` | Vendedores e regiões de atuação |
| `Products/` | `Script011`–`013` | Produtos, saldos, similares e lotes |
| `PurchaseOrders/` | `Script014`–`016` | Pedidos de compra |
| `SalesOrders/` | `Script017`–`019` | Pedidos de venda, catálogos da API Sales Order e sugestões de faturamento |
| `AccountsPayable/` | `Script020`–`022` | Duplicatas e comissões pagas |
| `Invoices/` | `Script023`–`025` | Notas fiscais, inutilizações, centros de custo e leituras da API Fiscal |
| `PersonCatalogs/` | `Script026`–`028` | Catálogos da API Person (classificações e tipos de e-mail, telefone e contato) |
| `Representatives/` | `Script029`–`031` | Representantes e recepção de venda |
| `ProductCatalogs/` | `Script032`–`034` | Catálogos da API Product |
| `ProductPrices/` | `Script035`–`037` | Preços, custos, itens das tabelas de preço e promoções |
| `Bonus/` | `Script038` | Jobs das escritas e leituras de bônus (sem tabelas) |

**Regras gerais das tabelas e dos merges:**
* Toda tabela de dados existe em dois schemas: `untreated` (recebe o bulk insert, sem chave) e `totvs` (tabela final, com PK). A exceção é `totvs.TotvsRequests`, que só recebe inserts.
* As procedures `totvs.sp_<Tabela>_Sync` são geradas por `dbo.sp_Create_Merge_Procedure`, do projeto Shared.
* Nas tabelas filhas, o merge **apaga** as linhas que não vieram quando o pai foi regravado (o `integrated_or_updated_at` do pai é mais recente). As netas têm FK com `on delete cascade`.
* **Exceção:** `TotvsSkusColors` e `TotvsSkusColorsProducts` não apagam, porque a API devolve a referência só com os produtos alterados na janela. Um produto excluído na Totvs fica no banco.
* Os registros principais **nunca são apagados**: uma nota excluída fica com `invoiceStatus = 'Deleted'`.

---

## Endpoints expostos (`TotvsController`)

Rota base: `TotvsJobs/Totvs`. O verbo segue a documentação oficial: `POST` nas buscas, `GET` nas filiais e nas leituras sob demanda.

### Leituras recorrentes (disparam a integração de todas as empresas)

| Método | Rota | Service |
|---|---|---|
| GET | `Branches` | `TotvsPeopleService.GetBranchesList` |
| POST | `Customers` | `TotvsPeopleService.PostSearchCustomers` |
| POST | `LegalEntities` | `TotvsPeopleService.PostSearchLegalEntity` |
| POST | `Sellers` | `TotvsSellersService.PostSearchSellers` |
| GET | `OperationalAreas` | `TotvsSellersService.GetOperationalArea` |
| POST | `Products` | `TotvsProductsService.PostSearchReferences` |
| POST | `Products/Prices` | `TotvsProductPricesService.PostSearchPrices` |
| POST | `Products/Costs` | `TotvsProductPricesService.PostSearchCosts` |
| POST | `Products/PriceTables` | `TotvsProductPricesService.PostSearchPriceTables` |
| GET | `Products/Promotions` | `TotvsProductPricesService.GetPromotion` |
| GET | `Products/Similar` | `TotvsProductsService.GetSimilar` |
| POST | `Products/Batches` | `TotvsProductsService.PostSearchBatch` |
| POST | `Products/ProductCodes` | `TotvsProductsService.PostSearchProductCodes` |
| POST | `Products/Balances` | `TotvsProductsService.PostSearchBalances` |
| POST | `PurchaseOrders` | `TotvsPurchaseOrdersService.PostSearchPurchaseOrders` |
| POST | `SalesOrders` | `TotvsSalesOrdersService.PostSearchSalesOrders` |
| GET | `SalesOrderDiscountTypes` | `TotvsSalesOrdersService.GetDiscountType` |
| GET | `SalesOrderClassifications` | `TotvsSalesOrdersService.GetClassifications` |
| GET | `SalesOrderBatches` | `TotvsSalesOrdersService.GetBatchItems` |
| GET | `BillingSuggestions` | `TotvsSalesOrdersService.GetBillingSuggestions` |
| POST | `Duplicates` | `TotvsAccountsPayableService.PostSearchDuplicates` |
| POST | `ComissionsPaid` | `TotvsAccountsPayableService.PostSearchComissionsPaid` |
| POST | `Invoices` | `TotvsInvoicesService.PostSearchInvoices` |
| GET | `InvoicesDisable` | `TotvsInvoicesService.GetInvoicesDisable` |
| GET | `CostCenters` | `TotvsInvoicesService.GetCostCenter` |
| POST | `Representatives` | `TotvsRepresentativesService.PostSearchRepresentatives` |
| GET | `ReceptionSales` | `TotvsRepresentativesService.GetSearchReceptionSale` |
| GET | `People/Classifications` | `TotvsPersonCatalogsService.GetClassifications` |
| GET | `People/EmailTypes` | `TotvsPersonCatalogsService.GetEmailTypes` |
| GET | `People/PhoneTypes` | `TotvsPersonCatalogsService.GetPhoneTypes` |
| GET | `People/ContactTypes` | `TotvsPersonCatalogsService.GetContactTypes` |
| GET | `Products/ClassificationTypes` | `TotvsProductCatalogsService.GetClassificationType` |
| GET | `Products/Classifications` | `TotvsProductCatalogsService.GetClassifications` |
| GET | `Products/Categories` | `TotvsProductCatalogsService.GetCategory` |
| GET | `Products/Grids` | `TotvsProductCatalogsService.GetGrid` |
| GET | `Products/MeasurementUnits` | `TotvsProductCatalogsService.GetMeasurementUnit` |
| POST | `Products/Colors` | `TotvsProductCatalogsService.PostSearchColors` |
| GET | `Products/AdditionalFieldsTypes` | `TotvsProductCatalogsService.GetAdditionalFieldsTypes` |
| GET | `Products/GrouperConfigurations` | `TotvsProductCatalogsService.GetProductGrouperConfiguration` |
| GET | `Products/InstructionItems` | `TotvsProductCatalogsService.GetInstructionItems` |
| POST | `Products/Compositions` | `TotvsProductCatalogsService.PostCompositions` |
| GET | `Products/PriceTablesHeaders` | `TotvsProductCatalogsService.GetPriceTablesHeaders` |
| GET | `Products/PriceTableScales` | `TotvsProductCatalogsService.GetPriceTableScales` |

### Leituras sob demanda (chamadas por sistema externo)

Todas exigem:
* o cabeçalho **`X-User-Name`** (até 100 caracteres);
* o CNPJ da empresa na rota (`{docCompany}`, 14 dígitos);
* códigos numéricos maiores que zero.

| Método | Rota | Service |
|---|---|---|
| GET | `Customers/{docCompany}/{code}` | `GetCustomerByCode` |
| GET | `LegalEntities/{docCompany}/{code}` | `GetLegalEntityByCode` |
| GET | `SalesOrders/{docCompany}/{branchCode}/{orderCode}` | `GetSalesOrderByKey` |
| GET | `PurchaseOrders/{docCompany}/{branchCode}/{orderCode}` | `GetPurchaseOrderByKey` |
| GET | `Invoices/{docCompany}/{branchCode}/{invoiceSequence}` | `GetInvoiceByKey` |
| GET | `Products/{docCompany}/{code}` | `GetProductByCode` |
| GET | `Representatives/{docCompany}/{code}` | `GetRepresentativeByCode` |

| Código | Quando |
|---|---|
| **200** | Registro gravado |
| **404** | O registro não existe na Totvs |
| **422** | Empresa não cadastrada ou resposta inválida da API (detalhes no log da execução) |

**Leituras sem persistência** (onda 2 do plano, base T11): devolvem o conteúdo da Totvs (JSON no `data` do `Result`, ou o arquivo)
sem gravar tabela; a chamada fica em `totvs.TotvsRequests` (nas de arquivo, só um resumo, nunca o XML ou o PDF).

| Método | Rota | Service |
|---|---|---|
| GET | `ElectronicInvoices/{docCompany}/{accessKey}/Xml` | `GetXmlContents` |
| GET | `ElectronicInvoices/{docCompany}/{accessKey}/Danfe?nfeDocumentType=1` | `PostSearchDanfe` (PDF; a NFC-e dá erro na Totvs) |
| GET | `Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Print?invoiceDate=yyyy-MM-dd&template=1&type=1` | `PostTransactionPrint` (PDF) |
| GET | `Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Items?invoiceDate=yyyy-MM-dd` | `GetSearchInvoicesItemDetail` |
| GET | `ElectronicInvoices/{docCompany}/{accessKey}/Items` | `GetSearchInvoicesItemDetail` |
| GET | `ConditionalProducts/{docCompany}/{branchCode}?personCode=N` (ou `personCpfCnpj`) | `GetInvoicesPendingConditionalProducts` (404 sem saldo) |
| POST | `InvoiceProducts/{docCompany}/Search` (filtro no corpo; produto, nota ou chave de acesso obrigatórios) | `PostSearchInvoiceProducts` |
| GET | `Branches/{docCompany}/{branchId}` (código ou CNPJ) | `GetBranches` |
| GET | `PersonStatistics/{docCompany}?customerCode=N` (ou `customerCpfCnpj`), `&branchCode=` | `GetPersonStatistics` |
| POST | `ListBalanceBonus/{docCompany}` (`personCpf` e `branchList` no corpo) | `PostListBalanceBonus` (404 para CPF inexistente) |
| GET | `Products/{docCompany}/Codes/{code}?branchCode=` | `GetProducts` (código, pack, barras, RFID, item de lote) |
| GET | `Products/{docCompany}/Kardex?productCode&branchCode&startDate&endDate&balanceType` | `GetKardexMovement` (até 180 dias) |
| GET | `Products/{docCompany}/{productCode}/GridFrequency?page&pageSize` | `GetGridFrequency` |
| GET | `Products/{docCompany}/MeasurementConversions?productCode=` (ou `groupSequence`) | `GetMeasurementConversions` |
| GET | `Products/{docCompany}/CompositionProduct` e `.../CompositionGroupProduct` (`productCodeList`, `groupCodeList` = `groupSequenceCode`, `referenceCodeList`) | `GetCompositionProduct`, `GetCompositionGroupProduct` |
| GET | `SalesOrders/{docCompany}/{branchCode}/{orderCode}/Invoices?invoiceStatus=` | `GetInvoices` |
| GET | `SalesOrders/{docCompany}/{branchCode}/{orderCode}/PendingItems` | `GetPendingItems` (200 mesmo sem pendência) |
| GET | `SalesOrders/{docCompany}/{branchCode}/{orderCode}/ChangedBatchItems?startDate&endDate&batchCode` | `GetChangedBatchItems` |

| Código | Quando |
|---|---|
| **200** | O JSON (`Result.data`) ou o arquivo |
| **404** | A Totvs não achou (inclui o 400 `KeyNotFound` do `xml-contents`, que a Totvs devolve também para a nota cancelada) |
| **422** | Empresa não cadastrada, 400 da Totvs (com as mensagens) ou impressão sem sucesso no Uniface |
| **429** | Limite da Totvs, com `Retry-After` |
| **502** | 5xx da Totvs, sem resposta ou JSON inválido |

### Escritas (chamadas por sistema externo)

Mesmas exigências das leituras sob demanda (`X-User-Name` e `{docCompany}`), com o corpo validado **antes** de chamar a Totvs. Nas
escritas de pedido de venda, a **chave do pedido** é `branchCode` + `orderCode` (loja da empresa) **ou** `orderId`, um ou outro.

| Método | Rota | Corpo | Service |
|---|---|---|---|
| POST | `Duplicates/{docCompany}/Installments/Stage` | `branchCnpj` (loja da empresa), `supplierCpfCnpj` **ou** `supplierCode`, `duplicateCode`, `installmentCode`, `newStage` (1 título não conferido, 2 liberado para pagamento, 5 título aceito) | `PostDuplicatesInstallmentsStage` |
| POST | `Duplicates/{docCompany}` | `branchCnpj` (loja da empresa), `supplierCpfCnpj`, `duplicateCode`, `installments` (cada uma com portador, emissão, vencimento, chegada, `document`, `prevision`, `stage`, valor e pelo menos uma despesa, com rateio por `proratedPercentage` somando 100 **ou** por `proratedValue`) | `PostDuplicates` |
| POST | `GroupDuplicates/{docCompany}` | a duplicata nova (`branchCnpj`, `supplierCpfCnpj`, `duplicateCode`, `document` 1 ou 18, `dueDate`) e pelo menos duas parcelas **reais** em `groupInstallments` (as de previsão a Totvs recusa; lojas da empresa) | `PostGroupDuplicates` (devolve a chave gerada) |
| POST | `Messages/{docCompany}` | `personCode` **ou** `personCpfCnpj`; `messages` (`description` até 200, `priority`) | `PostMessages` (devolve as `sequence`) |
| PUT | `Messages/{docCompany}/Change` | `filter` (`personCode` **ou** `personCpfCnpj`, `sequence`); `description` (obrigatória: a Totvs recusa a alteração sem ela); `status` (1 não lida, 2 lida, 3 cancelada) e `priority` opcionais | `PutChangeMessages` |
| DELETE | `Messages/{docCompany}/{personCode}/{sequence}` | — (exclusão definitiva, só de mensagem **não lida**; prefira o `PUT` com `status` 3) | `DeleteMessages` |
| POST | `IndividualCustomers/{docCompany}` | `branchInsertCode` (loja da empresa), `cpf` (com dígito verificador), `name` e o que mudar: campo ausente não muda; `""`, `0` e `"NotInformed"` zeram; enums **pelo nome** (`gender`: `Female`...); listas de telefones, endereços (obrigatório na inclusão quando o ERP tem `IN_OBRIGATORIO_ENDERECO`), e-mails, classificações (só inclui), limites por loja da empresa, contas bancárias e `individualAdditionals` | `PostIndividualCustomers` (devolve o `customerCode`) |
| POST | `LegalCustomers/{docCompany}` | `branchInsertCode`, `cnpj` (com dígito verificador), `name` e o que mudar, com as mesmas regras; sócios (`cpfPartner` **ou** `partnerCode`, participação até 100), contatos e `preferences` | `PostLegalCustomers` (devolve o `customerCode`) |
| POST | `CustomerRepresentatives/{docCompany}` | `representativeCode` **ou** `representativeCpfCnpj`; `customerCodeList` **ou** `customerCpfCnpjList` (até 500, sem repetição) | `PostCustomerRepresentatives` (sem rota para desfazer) |
| POST | `ClassificationRelationship/{docCompany}/Remove` | `personCode`, `classifications` (`classificationTypeCode` e `classificationCode`) | `PostRemoveClassificationRelationship` |
| POST | `BonusDueDate/{docCompany}` | `branchCode`, `personCode`, `bonusType` (1 a 15), `movementDate`, `bonusValue`, validade (`startDate`, `endDate` não vencida); `usedValue` e a origem (transação, fatura ou liquidação, cada uma completa) | `PostBonusDueDate` (gera crédito; sem estorno) |
| POST | `BonusConsume/{docCompany}` | `personCode` **ou** `personCpfCnpj`, `branchCode`, `usedValue` | `PostBonusConsume` (com o documento, saldo da loja conferido antes: 422 se menor; sem estorno) |
| POST | `Products/{docCompany}/Colors` | `code` (10), `name` (40); `colorInfoAdditionals` por loja da empresa (`branchCode`, `auxiliaryCode`, `auxiliaryName`, `pantoneCode`, usos) | `PostColors` (um `code` existente é renomeado em todas as referências) |
| POST | `Products/{docCompany}/Grids` | `gridCode` (opcional, 6), `name` (40), `type`; `gridSizes` (`name` até 10, sem repetição) | `PostGrids` (devolve o `gridCode`) |
| POST | `Products/{docCompany}/Classifications` | `typeCode` (4, nunca 6000), `description` (40), `auxiliaryDescription` (100); `classifications` (`code` 10, sem repetição) | `PostClassifications` (inclui ou renomeia) |
| POST | `Products/{docCompany}/BarCodes` | `products` (`productCode`; `barCodes` com `barCode` 40, `quantity` inteira > 0, no máximo um `isMainCode`, `inRfid`); código sem repetição no corpo | `PostBarcodes` |
| POST | `Products/{docCompany}/BarCodes/Update` | `products` (`productCode`; `barCodes` com `barCode`, `quantity` decimal > 0, `isMainCode`) | `PostUpdateBarcodes` |
| POST | `Products/{docCompany}/ClassificationRelationships/Create` | `products` (`productCode` **ou** `referenceId`; `classifications` com `typeCode`, nunca 6000, e `code`) | `PostCreateClassificationRelationship` |
| POST | `Products/{docCompany}/ClassificationRelationships/Update` | o mesmo corpo | `PostUpdateClassificationRelationship` |
| POST | `Products/{docCompany}/References` | máscara, grade, um grupo por nível (grupos que já existem no ERP, PRDFL018, com a descrição; a combinação precisa ser nova), empresas (lojas da empresa), SKUs com nome, cor, tamanho e unidade, cada um com a classificação 6000 da marca (`2` ou `4`); sem `values` | `PostReferences` (devolve os `productCode`) |
| POST | `Products/{docCompany}/Batches/Create` | `batchs` (loja da empresa, produto, saldo; itens com nota de inspeção por `supplierCode` **ou** `supplierCpfCnpj`) | `PostCreateBatch` |
| PUT | `Products/{docCompany}/BatchInfo` | `batchItems` (loja + lote + sequência **ou** `itemClient`; `dataChanged` com pelo menos um dado) | `PutBatchInfo` |
| POST | `Products/{docCompany}/BatchPreferentialSalesOrder` | item por lote + sequência **ou** `batchItemBarCode`; pedidos por loja e `orderCode` **ou** `orderId`. Lista vazia só com `?clear=true` (só apaga) | `PostBatchPreferentialSalesOrder` |
| PUT | `Products/{docCompany}/{code}/{branchCode}` | pelo menos um entre `ncmCode` (8 dígitos), `cstCode`, `weight` e os indicadores (`isInactive` tira de venda na loja) | `PutProducts` |
| PUT | `Products/{docCompany}/BranchInfo/{branchCode}` | `filter` (`productCodeList` **ou** `barCodeList`, até 500; grupo e referência não aceitos) e pelo menos um indicador ou `spedItemType` | `PutBranchInfo` |
| PUT | `Products/{docCompany}/Data` | `filter` (idem) e pelo menos um entre `ncmCode`, `measuredUnit`, `cstCode`, `weight`, `cestCode`, `prefixEanGtin` (nunca vazio) | `PutData` |
| POST | `Products/{docCompany}/Values/Create` | `products` (até 500; `values` com `branchCode`, `valueType` `C`/`P`, `valueCode`, `value` > 0). Query `confirmVariation` | `PostCreateValues` (só códigos e lojas permitidos; variação acima do limite: 422 sem `?confirmVariation=true`) |
| POST | `Products/{docCompany}/Values/Update` | o mesmo corpo. Query `confirmVariation` | `PostUpdateValues` (idem; sem valor gravado para conferir: 422 sem `?confirmVariation=true`) |
| POST | `Products/{docCompany}/PromotionValues/Update` | `branchCode`, `promotionCode`, `items` (`productCode`, `promotionValue` > 0) | `PostUpdatePromotionValues` (com a promoção no banco: só aberta e com valor abaixo do anterior) |
| POST | `Products/{docCompany}/PromotionItems` | o mesmo corpo | `PostPromotionItems` (com a promoção no banco: só aberta) |
| POST | `PurchaseOrders/{docCompany}/ChangeStatus` | `branchCode` (loja da empresa), `orderCode`, `status` (1 em andamento, 5 bloqueado) | `PostPurchaseOrdersChangeStatus` (com o pedido no banco, só entre essas duas situações) |
| POST | `PurchaseOrders/{docCompany}` | loja da empresa, fornecedor e comprador (código **ou** CPF/CNPJ), operação, condição, `status` (1 ou 5), `supplierOrderCode` (**obrigatório**), `totalAmountOrder` e pelo menos um item (`productCode` **ou** `productSku`, valor, quantidade, `itemsValues` com `C`/`P`); classificações, observações, despesas, descontos, campos e dados adicionais | `PostPurchaseOrders` (devolve loja e número; o mesmo `supplierOrderCode` do fornecedor já gravado volta 422) |
| POST | `PurchaseOrders/{docCompany}/QuantityItems` | `branchCode`, `orderCode`, `items` (`productCode`, `quantity` ≥ 0, sem repetição) | `PostPurchaseOrdersQuantityItems` (com o pedido no banco: atendido ou cancelado, produto fora do pedido e quantidade abaixo da recebida voltam 422) |
| POST | `PurchaseOrders/{docCompany}/BatchLink` | `branchCode` (do lote), `batchCode` + `sequence` **ou** `batchItemBarCode`, `orderCode`, `invoice` (obrigatória: a nota de entrada, por `accessKey` **ou** `serialCode` + `invoiceCode`) | `PostPurchaseOrderBatchLink` (cria ou troca o vínculo; 404 e 409 da Totvs repassados) |
| POST | `PurchaseOrders/{docCompany}/Cancel` | `branchCode`, `orderCode` e o fornecedor esperado (`supplierCode` **ou** `supplierCpfCnpj`, não vai à Totvs) | `PostPurchaseOrdersCancel` (com o pedido no banco: outro fornecedor, já cancelado, atendido, parcialmente recebido ou distribuído voltam 422) |
| POST | `SalesOrders/{docCompany}/Additional` | chave do pedido; `shippingService`, `trackingId`, `ecommerceStage`, `omniSiteId`, `compensationDays`, `anticipationDay`, `entryType` (`N`/`S`), `experienceType` (1 a 5) | `PostAdditionalOrder` |
| POST | `SalesOrders/{docCompany}/Shipping` | chave do pedido; transportadora e redespacho (código **ou** CPF/CNPJ, com o tipo de frete), `freightPercentage`, `freightValue`, `shippingAddress` | `PostShippingOrder` (muda o total do pedido; recusado em pedido bloqueado ou do e-commerce) |
| POST | `SalesOrders/{docCompany}/Observations` | chave do pedido; `observations` (`observation` até 80, `visualizationType` 1 ou 2) e/ou `invoiceObservations` | `PostObservationsOrder` (inclui linhas; recusado em pedido do e-commerce) |
| POST | `SalesOrders/{docCompany}/Items/Additional` | chave do pedido; `items` (`productCode` **ou** `productSku`; `rollMeasureCode`, `billingForecastDate`) | `PostAdditionalItems` (recusado em pedido do e-commerce) |
| POST | `SalesOrders/{docCompany}/Status` | chave do pedido; `statusOrder` **pelo nome** (`InProgress`, `BillingReleased`, `Blocked`, `InComposition`, `InAnalysis`); motivo do bloqueio | `PostOrdersChangeStatus` |
| POST | `SalesOrders/{docCompany}/Header` | chave do pedido e pelo menos um entre `averagePaymentPeriod`, `operationCode`, `priceTableCode` e `priorityCode` | `PostHeader` (com o pedido no banco, operação e tabela de preço só em andamento ou bloqueado) |
| POST | `SalesOrders/{docCompany}/Items` | chave do pedido; `items` (`productCode` **ou** `productSku`, quantidade, preço, descontos, seriais, comissionados) | `PostItems` |
| DELETE | `SalesOrders/{docCompany}/Items` | query: `branchCode` + `orderCode` **ou** `orderId`; `productCode` **ou** `productSku` | `DeleteItems` |
| POST | `SalesOrders/{docCompany}/Items/Quantity` | chave do pedido; `items` (`productCode` **ou** `productSku`, `quantity` nova) | `PostQuantityItems` (com o item no banco, a nova precisa ser maior que a atual) |
| POST | `SalesOrders/{docCompany}/Counts` | `branchCode`, `orderCode`; `counts` (`branchCountCode`, `countCode`; sugestão aos pares) | `PostRelationshipCounts` |
| POST | `SalesOrders/{docCompany}/BatchItems` | `branchCode`, `orderCode`; `items` (`code` do produto, `isUpdateQuantity`, `batchItems` por loja + lote + sequência **ou** `barCode`, com a quantidade) | `PostRelationshipBatchItems` |
| DELETE | `SalesOrders/{docCompany}/BatchItems` | query: chave do pedido, `productCode` **ou** `productSku`, `batchSequence` | `DeleteBatchItems` |
| POST | `SalesOrders/{docCompany}/BatchItems/SuggestNuance` | só a chave do pedido | `PostSuggestBatchItemsNuance` (o ERP escolhe os lotes) |
| POST | `SalesOrders/{docCompany}/B2C` | `orderId`, loja, data, cliente e representante (código **ou** CPF/CNPJ), operação, condição, prioridade (1 a 99: o 0 a Totvs recusa como vazio), `statusOrder` (1, 5, 8), total e itens; pagamentos (soma igual ao total; cartão com NSU, autorização, operadora e bandeira), descontos, classificações, observações, endereço, brindes, comissionados, terceiros | `PostB2COrders` (orderId existente: 422 com o pedido; sem como conferir: não inclui) |
| POST | `SalesOrders/{docCompany}/Cancel` | chave do pedido; `reasonCancellationCode`, `ReasonCancellationDescription` (R maiúsculo) | `PostOrdersCancel` (com o pedido no banco: cancelado ou atendido voltam 422) |
| POST | `SalesOrders/{docCompany}/Items/Price` | chave do pedido; `items` (`productCode` **ou** `productSku`, `price` > 0) | `PostPriceItems` |
| POST | `SalesOrders/{docCompany}/Items/Cancel` | chave do pedido; `items` (produto, `cancelQuantity` > 0, `reasonCancellationCode`) | `PostCancelItems` (com o item no banco, até a pendente) |
| POST | `SalesOrders/{docCompany}/PdvTransactions` | chave do pedido; `transactionId`, loja, código e data da transação | `PostPdvTransactions` |
| POST | `InvoicesManifestations/{docCompany}` | `items` (até 8): `accessKey` (44 dígitos), `branchCnpj` (loja da empresa), emissor, número, emissão, série, `nsu`, `operationType` e `invoiceStatus` pelo nome, total, `invoiceXml`, `documentType` (55, 57, 59, 81) | `PostInvoicesManifestations` (o XML vai à Totvs, mas só o tamanho fica em `TotvsRequests`) |

| Código | Quando |
|---|---|
| **200** | Escrita aceita pela Totvs (chave e resposta da Totvs no corpo) |
| **400** | Corpo inválido (lista de campos e mensagens), sem chamar a Totvs |
| **404** | A Totvs respondeu 404: o registro não existe (lote, pedido ou nota do vínculo com lote) |
| **409** | A mesma escrita não idempotente (rota + chave + corpo) foi aceita ou ficou sem resposta certa nos últimos 10 minutos: recusada sem chamar a Totvs. Também o 409 da Totvs (conflito de vínculo) |
| **422** | Escrita desligada na configuração, empresa ou loja não cadastrada, ou a Totvs recusou (mensagens da Totvs no corpo) |
| **429** | A Totvs limitou as chamadas também na nova tentativa (`Retry-After`) |
| **502** | A Totvs foi chamada e não respondeu ou respondeu 5xx: **a escrita pode ter sido feita**, confira antes de repetir |

Diferente das leituras, uma escrita só tenta de novo em 401 (renova o token) e 429; em 502/503/504 não, porque a Totvs pode ter gravado antes de o gateway cair.

> O `[Authorize]` do controller está comentado, como no Movidesk e no AfterSale. A autenticação dos endpoints de inbound vai ser outro mecanismo, ainda a definir; até lá, as escritas ficam desligadas por configuração em produção.

---

## Auditoria (jobs)

Os jobs ficam em `auditing.Jobs`, com os mesmos ids em `EnumJob`.

| Faixa | Tipo (`IdType`) | Conteúdo |
|---|---|---|
| 13000–13499 | 2 (Integração) | Leituras, com `IdParent = 130` (app Totvs) |
| 13500–13999 | 1 (Procedure) | Procedures `sp_<Tabela>_Sync` (13500–13650), com `IdParent` = job da leitura e o mesmo intervalo |

| Id | Job | Intervalo |
|---|---|---|
| 13000 | `TotvsPostSearchReferences` | 120 min |
| 13001 | `TotvsPostSearchLegalEntity` | 3 min |
| 13002 | `TotvsPostSearchCustomers` | 3 min |
| 13003 | `TotvsPostSearchInvoices` | 3 min |
| 13004 | `TotvsPostSearchPurchaseOrders` | 3 min |
| 13005 | `TotvsPostSearchSalesOrders` | 3 min |
| 13006 | `TotvsPostSearchSellers` | 120 min |
| 13007 | `TotvsGetBranchesList` | 120 min |
| 13008 | `TotvsPostSearchDuplicates` | 3 min |
| 13009 | `TotvsPostSearchComissionsPaid` | 3 min |
| 13010 | `TotvsGetCustomerByCode` | sob demanda |
| 13011 | `TotvsGetLegalEntityByCode` | sob demanda |
| 13012 | `TotvsGetSalesOrderByKey` | sob demanda |
| 13013 | `TotvsGetInvoiceByKey` | sob demanda |
| 13014 | `TotvsGetProductByCode` | sob demanda |
| 13015 | `TotvsPostSearchPrices` | 3 min |
| 13016 | `TotvsPostSearchCosts` | 3 min |
| 13017 | `TotvsPostSearchBalances` | 3 min |
| 13032 | `TotvsPersonGetClassifications` | 120 min |
| 13033 | `TotvsGetEmailTypes` | 120 min |
| 13034 | `TotvsGetPhoneTypes` | 120 min |
| 13035 | `TotvsGetContactTypes` | 120 min |
| 13030 | `TotvsPostSearchRepresentatives` | 120 min |
| 13031 | `TotvsGetSearchReceptionSale` | 3 min |
| 13039 | `TotvsGetRepresentativeByCode` | sob demanda |
| 13070 | `TotvsGetOperationalArea` | 120 min |
| 13080 | `TotvsGetClassificationType` | 120 min |
| 13081 | `TotvsProductGetClassifications` | 120 min |
| 13082 | `TotvsGetCategory` | 120 min |
| 13083 | `TotvsGetGrid` | 120 min |
| 13084 | `TotvsGetMeasurementUnit` | 120 min |
| 13085 | `TotvsPostSearchColors` | 120 min |
| 13086 | `TotvsGetAdditionalFieldsTypes` | 120 min |
| 13087 | `TotvsGetProductGrouperConfiguration` | 120 min |
| 13088 | `TotvsGetInstructionItems` | 120 min |
| 13089 | `TotvsPostCompositions` | 120 min |
| 13090 | `TotvsGetPriceTablesHeaders` | 120 min |
| 13091 | `TotvsGetPriceTableScales` | 120 min |
| 13092 | `TotvsPostSearchPriceTables` | 120 min |
| 13093 | `TotvsGetPromotion` | 120 min |
| 13094 | `TotvsGetSimilar` | 120 min |
| 13095 | `TotvsPostSearchBatch` | 120 min |
| 13097 | `TotvsPostSearchProductCodes` | diário |
| 13190 | `TotvsGetSalesOrderDiscountType` | 120 min |
| 13191 | `TotvsGetSalesOrderClassifications` | 120 min |
| 13192 | `TotvsGetSalesOrderBatchItems` | 120 min |
| 13194 | `TotvsGetSalesOrderBillingSuggestions` | 3 min |
| 13250 | `TotvsGetInvoicesDisable` | 120 min |
| 13251 | `TotvsGetCostCenter` | 120 min |
| 13252 | `TotvsGetXmlContents` | sob demanda, sem gravar |
| 13253 | `TotvsPostSearchDanfe` | sob demanda, sem gravar |
| 13254 | `TotvsPostTransactionPrint` | sob demanda, sem gravar |
| 13255 | `TotvsGetSearchInvoicesItemDetail` | sob demanda, sem gravar |
| 13256 | `TotvsGetInvoicesPendingConditionalProducts` | sob demanda, sem gravar |
| 13257 | `TotvsPostSearchInvoiceProducts` | sob demanda, sem gravar |
| 13258 | `TotvsPostInvoicesManifestations` | escrita |
| 13036–13038 | `TotvsGetBranches`, `TotvsGetPersonStatistics`, `TotvsPostListBalanceBonus` | sob demanda, sem gravar |
| 13100–13105 | `TotvsGetProducts`, `TotvsGetKardexMovement`, `TotvsGetGridFrequency`, `TotvsGetMeasurementConversions`, `TotvsGetCompositionProduct`, `TotvsGetCompositionGroupProduct` | sob demanda, sem gravar |
| 13185 | `TotvsGetPurchaseOrderByKey` | sob demanda |
| 13193, 13195, 13196 | `TotvsGetSalesOrderChangedBatchItems`, `TotvsGetSalesOrderInvoices`, `TotvsGetSalesOrderPendingItems` | sob demanda, sem gravar |
| 13018 | `TotvsPostDuplicates` | escrita |
| 13019 | `TotvsPostGroupDuplicates` | escrita |
| 13020 | `TotvsPostDuplicatesInstallmentsStage` | escrita |
| 13040–13043 | `TotvsPostIndividualCustomers`, `TotvsPostLegalCustomers`, `TotvsPostCustomerRepresentatives`, `TotvsPostRemoveClassificationRelationship` | escrita |
| 13044–13048 | `TotvsPostMessages`, `TotvsPutChangeMessages`, `TotvsDeleteMessages`, `TotvsPostBonusDueDate`, `TotvsPostBonusConsume` | escrita |
| 13120–13127 | `TotvsPostBarcodes`, `TotvsPostUpdateBarcodes`, `TotvsPostCreateClassificationRelationship`, `TotvsPostUpdateClassificationRelationship`, `TotvsPostColors`, `TotvsPostClassifications`, `TotvsPostGrids`, `TotvsPostReferences` | escrita |
| 13128–13134 | `TotvsPutProducts`, `TotvsPutBranchInfo`, `TotvsPutData`, `TotvsPostCreateValues`, `TotvsPostUpdateValues`, `TotvsPostUpdatePromotionValues`, `TotvsPostPromotionItems` | escrita |
| 13135–13137 | `TotvsPostCreateBatch`, `TotvsPutBatchInfo`, `TotvsPostBatchPreferentialSalesOrder` | escrita |
| 13180–13184 | `TotvsPostPurchaseOrders`, `TotvsPostPurchaseOrdersQuantityItems`, `TotvsPostPurchaseOrdersChangeStatus`, `TotvsPostPurchaseOrdersCancel`, `TotvsPostPurchaseOrderBatchLink` | escrita |
| 13199–13203, 13205–13208 | `TotvsPostSalesOrderOrdersChangeStatus`, `TotvsPostSalesOrderHeader`, `TotvsPostSalesOrderAdditionalOrder`, `TotvsPostSalesOrderShippingOrder`, `TotvsPostSalesOrderObservationsOrder`, `TotvsPostSalesOrderItems`, `TotvsDeleteSalesOrderItems`, `TotvsPostSalesOrderAdditionalItems`, `TotvsPostSalesOrderQuantityItems` | escrita |
| 13197, 13198, 13204, 13209, 13210 | `TotvsPostSalesOrderB2COrders`, `TotvsPostSalesOrderOrdersCancel`, `TotvsPostSalesOrderPdvTransactions`, `TotvsPostSalesOrderPriceItems`, `TotvsPostSalesOrderCancelItems` | escrita |
| 13211–13214 | `TotvsPostSalesOrderRelationshipCounts`, `TotvsPostSalesOrderRelationshipBatchItems`, `TotvsDeleteSalesOrderBatchItems`, `TotvsPostSalesOrderSuggestBatchItemsNuance` | escrita |

Toda execução grava `auditing.Logs` e, conforme o caso:
* `auditing.Messages`, com mensagens e erros de validação;
* `auditing.Records`, com os registros integrados e o JSON recebido.

---

## Configuração

| Onde | Chave | Uso |
|---|---|---|
| `appsettings.json` do host | `Totvs:BaseUrl` | Endereço da API; homologação é só outro valor no `appsettings` do ambiente |
| `appsettings.json` do host | `Totvs:Writes:<grupo>:Enabled` | Liga as escritas de um grupo (`AccountsPayable`, `Bonus`, `Invoices`, `People`, `Products`, `PurchaseOrders`, `SalesOrders`). Sem a chave, ou com `false`, a rota devolve 422 sem chamar a Totvs |
| `appsettings.json` do host | `Totvs:Writes:Products:AllowedValueCodes`, `AllowedValueBranches`, `MaxValueVariationPercent` | Escrita de preço e custo (PR7): tipos e códigos permitidos (`P1,C1`: P preço, C custo), lojas permitidas (`1,101`) e a variação máxima contra o valor gravado (padrão 20). Lista vazia recusa toda escrita de valor |
| `appsettings.json` do host | `ConnectionStrings` | Conexão com o SQL Server (dados e auditoria) |
| Banco | `totvs.TotvsParameters` | CNPJ (`doc_company`) e credenciais (`client_id`, `client_secret`, `username`, `password`) de cada empresa |

As credenciais vão para o banco pelo `Script003` (e pelo de homologação, à mão) e nunca são gravadas em logs.

---

## Pasta `Responsed Jsons`

Uma resposta real de cada rota, com os dados trocados por **dados fictícios** para não expor informações no histórico do git:

* textos viram `Exemplo <campo>`; e-mails, `exemplo@exemplo.com.br`;
* CPF, CNPJ, CEP, telefones e chaves de acesso viram dígitos fictícios do mesmo tamanho;
* códigos e valores viram números fictícios, e datas de nascimento, `1990-01-01`;
* ficam como vieram só os booleanos, os enums (`status`, `type`...), as UFs, as datas de registro e o envelope da página (`count`, `totalPages`);
* as listas foram cortadas em 2 itens.

| Pasta | Conteúdo |
|---|---|
| `Authorization/Token/` | Estrutura da resposta do login (o token nunca é gravado) |
| `Branches/GetBranchesList/` | Página de filiais, com endereços |
| `Customers/PostSearchCustomers/` | Página de pessoas físicas com o `expand` completo e estatísticas |
| `LegalEntities/PostSearchLegalEntity/` | Página de pessoas jurídicas com o `expand` completo |
| `Sellers/PostSearchSellers/` | Página de vendedores |
| `Products/PostSearchReferences/` | Página de referências de uma marca, com o `expand` completo |
| `PurchaseOrders/PostSearchPurchaseOrders/` | Página de pedidos de compra |
| `SalesOrders/PostSearchSalesOrders/` | Página de pedidos de venda |
| `Duplicates/PostSearchDuplicates/` | Página de duplicatas a pagar |
| `CommissionsPaid/PostSearchComissionsPaid/` | Resposta vazia: não há fechamentos de comissão na base |
| `Invoices/PostSearchInvoices/` | Página de notas fiscais (sem `referencedInvoices`) |

As leituras sob demanda usam as mesmas buscas, com a lista de chave; a resposta tem o mesmo formato das pastas acima.

### Rotas do plano de métodos faltantes (montadas pelo swagger)

As 107 rotas novas (catálogos, leituras recorrentes e sem persistência, escritas) têm um exemplo **montado a partir do swagger oficial**, não
de uma chamada real: mostram a estrutura, os tipos e os enums de cada corpo, com valores fictícios pelas mesmas regras de cima (textos
`Exemplo <campo>`, documentos e CEP com dígitos fictícios, enums no formato que a rota usa, listas com 1 item, XML e PDF como `<base64>`).
Por virem do schema, os pares "um ou outro" (código **ou** CPF/CNPJ, `productCode` **ou** `productSku`...) aparecem com os dois lados: o
corpo de verdade leva só um, como o validador da rota exige.

| Arquivo | Conteúdo |
|---|---|
| `json-sent-to-api-end-point.json` | Corpo enviado à Totvs (escritas e buscas `POST`) |
| `parameters-sent-to-api-end-point.json` | Parâmetros de query (leituras `GET` e remoções `DELETE`), sem os `ValidationResult.*` e o `LastChangeUser`, que nunca são enviados |
| `json-returned-by-api-end-point.json` | Resposta da Totvs (200 ou 201) |

| Pasta | Métodos |
|---|---|
| `PersonCatalogs/` | `GetClassifications`, `GetEmailTypes`, `GetPhoneTypes`, `GetContactTypes` |
| `Representatives/`, `ReceptionSales/` | `PostSearchRepresentatives`, `PostCustomerRepresentatives`; `GetSearchReceptionSale` |
| `People/`, `Bonus/`, `Branches/` | `GetPersonStatistics`, mensagens, `PostRemoveClassificationRelationship`; `PostListBalanceBonus`, `PostBonusDueDate`, `PostBonusConsume`; `GetBranches` |
| `Customers/`, `LegalEntities/` | `PostIndividualCustomers`; `PostLegalCustomers` |
| `Sellers/` | `GetOperationalArea` e as rotas por CEP, cidade e estado |
| `ProductCatalogs/` | Os 12 catálogos da PR-1, `PostColors`, `PostGrids`, `PostClassifications` |
| `ProductPrices/` | Preços, custos, tabelas de preço, promoções e as escritas de valor e de promoção |
| `Products/` | Saldos, similares, lotes, códigos, kardex, grade, conversões, composições e as escritas de produto (códigos de barras, classificações, referência, lotes, dados fiscais) |
| `PurchaseOrders/` | Situação, inclusão, quantidade, cancelamento e vínculo com lote |
| `SalesOrders/` | Catálogos, lotes, sugestões, notas e pendentes do pedido e as 18 escritas |
| `Invoices/` | Inutilizações, centros de custo, XML, DANFE, impressão de transação, itens, condicional, produtos das notas e manifestação |
| `Duplicates/` | `PostDuplicates`, `PostGroupDuplicates`, `PostDuplicatesInstallmentsStage` |

---

## Pendências

O plano de métodos faltantes (`docs/PLANO-TOTVS-METODOS-FALTANTES.md`) está implementado em todas as ondas; o que falta está no fim da §10 de
cada fase ("Não verificado"). Em resumo:

* **Execução em banco e pelo host (plano, §10.44):** os scripts 001 a 079 rodaram num SQL Server local (DbUp, com a base compartilhada), as
  151 procedures de sincronização executam, e os jobs batem com o `EnumJob`. O host respondeu certo às rotas que não dependem da Totvs
  (400 de validação e `?clear=true`, 422 de escrita desligada e de empresa não cadastrada) e, com a empresa de homologação cadastrada, ao
  caminho com a Totvs: carga das lojas, leitura 200 e 404, escrita 200, 409 da trava, 422 da recusa da Totvs e a gravação em `TotvsRequests`
  com o `status_code` e o CPF mascarado. Sem verificar pelo host: 429 e 502 (não dá para provocar).
* **Homologação:** as ondas 3, 4 e 5 foram testadas (plano, §10.19, §10.25 e §10.32 a §10.43), inclusive a troca de estágio da parcela
  (AP-1), a inclusão de referência, os lotes e o consumo de bônus. Sem verificar: a sugestão por nuance (503 sem corpo; a produção não usa
  lote), a classificação no tipo de teste 9001 (`ORA-02291`), o vínculo com contagem real (o formato foi conferido; falta uma contagem sem
  pedido), bloquear e cancelar pedido de compra parcialmente recebido e o `batch-link` (falta o recebimento do 1|143) e o efeito fiscal da
  manifestação (feita com uma nota fictícia; com a área fiscal).
* **Produção:** espera a autenticação dos endpoints de inbound (T2) e a liberação de cada grupo de escrita pelo negócio, inclusive as listas
  de códigos e lojas de preço (PR7).
* **Preços, custos e saldos** (jobs 13015–13017): a carga inicial dos saldos leva horas (~27 min por loja, estimado) e precisa de proteção
  contra execução simultânea quando os jobs da Totvs forem registrados no Hangfire.
* **`referencedInvoices`:** comentado no `expand` das notas até a Totvs corrigir o erro 400. O `item-detail-search` não serve de contorno
  (F3, plano §10.35 e §10.42): para a devolução que falha, ele responde 200, mas com `referencedInvoices` vazio. Acontece também em
  produção. Para a Totvs: o 400 do `invoices/search` com a nota 2/31392 de 28/09/2026 da homologação e a 6/41422 de 18/10/2026 da
  produção (devoluções de venda, CFOP 1202).
---

## Documentação complementar

Na pasta `docs/` da raiz do repositório:

| Documento | Assunto |
|---|---|
| `PROPOSTA-PADRONIZACAO-TOTVS.md` | Proposta, decisões e plano em fases da padronização do módulo, com o que foi conferido em cada API |
| `PADRAO-CACHE-INTEGRACOES.md` | Padrão de cache das leituras |
| `ALTERACOES-MERGE-PROCEDURE-AUDITORIA.md` | Procedure de merge e auditoria |
