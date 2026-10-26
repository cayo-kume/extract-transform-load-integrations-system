# Proposta — padronização do módulo Totvs (padrão Movidesk / AfterSale)

Documento para revisão. **Nada foi alterado no código ainda.** As decisões tomadas em 23/10/2026 já estão
aplicadas no texto e resumidas na §10. A implementação será feita **um service de cada vez** (§9).

Objetivo: listar tudo o que precisa mudar em `src/Backend/Modules/Totvs` (e nos pontos do host que o
referenciam) para que o módulo siga o mesmo padrão dos módulos **Movidesk** e **AfterSale**.

Ao contrário do Oto, o Totvs tem a **mesma natureza** do Movidesk e do AfterSale: ele **lê** a API (Totvs Moda,
`/api/totvsmoda`) e **grava** no banco. Por isso a referência principal é o fluxo de **leitura recorrente** do
Movidesk (`GetTickets`, `GetPeople`, `GetServices`) e o padrão de cache de `docs/PADRAO-CACHE-INTEGRACOES.md`.

---

## 1. Situação atual (inventário)

| Item | Hoje | Passa a ser |
|---|---|---|
| Projetos | `Domain.Totvs`, `Application.Totvs`, `Infrastructure.Totvs` (estão no `.slnx`) | Os mesmos (§2) |
| Referências | Já apontam para `src/Backend/Shared` (`..\..\..\shared\...`, caminho correto para `Modules/Totvs`) | Ajustes de pacotes (§2.1) |
| Host | `Hangfire.csproj` não referencia o Totvs. `AddScopedTotvsServices` está comentado em `ServicesExtensions.cs` e o `TotvsController` está inteiro comentado | Ligado no host, um service por vez (§6.3, §9) |
| Recursos lidos | Clientes PF, pessoas jurídicas, filiais, produtos (referências/SKUs), notas fiscais, pedidos de compra, pedidos de venda, vendedores, duplicatas e comissões pagas (contas a pagar) | Os mesmos |
| Services | 7 services. 4 (`Fiscal`, `PurchaseOrder`, `SalesOrder`, `Seller`) usam a classe base `TotvsBaseSyncService`; 3 (`Person`, `Product`, `AccountsPayable`) repetem o fluxo inteiro à mão | **Nenhum service usa a `TotvsBaseSyncService`** (ela é removida): cada método faz as próprias chamadas à API, como no Movidesk (§4.1, §4.2) |
| Credenciais | `client_id`, `client_secret`, `username` e `password` **fixos no código**, copiados em 7 lugares (base + 6 métodos). Um login por execução, sem cache | Em `totvs.TotvsParameters`, com **cache das credenciais e do token** obtido no `authorization/v2/token` (§4.4, §5.1) |
| Multiempresa | Não existe. `branchCodeList = { 1 }` e `branchCode = 1` fixos | **Multiempresa**: todas as lojas da holding Newbloomers, lidas de `totvs.TotvsBranches`; nenhuma busca fica presa à filial 1 (§4.2, §5.1) |
| Janela de leitura | Fixa por recurso: pessoas desde `2000-01-01` **a cada execução**; produtos e contas a pagar, 10 meses; o resto, 5 meses | Pelo banco (`MAX(maxChangeFilterDate)`), com as janelas de hoje só na carga inicial (§4.2) |
| Banco | Nenhum script de tabela ou procedure. Os dados vão para `untreated.<Tabela>` (schema padrão do `BulkInsertIntoTableRaw` obsoleto) e as merges chamam `[totvs].sp_<Tabela>_Sync`, que não são criadas pelo módulo | Schema `totvs` com scripts por service (§5) |
| Scripts | `Script001` (insert em `auditing.Apps`, app 130) e `Script002` (insert de jobs **todo comentado**) | Numeração refeita (§5.4) |
| Jobs | `Domain.Totvs.Enums.EnumJob` só tem `None`; os ids comentados (1900–1909) não batem com a faixa do `Script002` (13000–13999) | Faixa 13000–13999 (§7) |
| Documentação | Sem `README.md` e sem pasta de JSONs de referência | README e `Responsed Jsons` (§9) |

Conclusão: o módulo **não compila**. O `dotnet build` de `Infrastructure.Totvs` para em `Application.Totvs` com
10 erros `CS0117` (`EnumJob` não contém `TotvsCustomers`, `TotvsFiscal`...): os services usam
`Domain.Shared.Enums.EnumJob`, que só tem `None`. Corrigido isso, `Infrastructure.Totvs` também falha:
os 7 repositórios recebem `IIntegrationsSharedRepository`, que não existe mais no Shared (hoje é
`ISharedRepository`). Diferente do Oto, as entidades, DTOs e validadores são reaproveitáveis: a migração é de
convenções, banco e correções. Os services são reescritos método a método, sem a classe base.

---

## 2. Projetos, referências e nomes

### 2.1 `.csproj`

| Projeto | Hoje | Padrão (Movidesk) |
|---|---|---|
| `Domain.Totvs` | `Newtonsoft.Json` 13.0.4 + `Microsoft.Extensions.Caching.Memory` 10.0.8 | Só `Newtonsoft.Json` 13.0.4 (o cache é usado na Application, não no Domain) |
| `Application.Totvs` | Sem pacote | `Microsoft.Extensions.Caching.Memory` **10.0.8** |
| `Infrastructure.Totvs` | `Microsoft.Extensions.Http` 10.0.8; referencia só `Application.Totvs` | Sem pacote (o `Infrastructure.Shared` já traz o `HttpClient`); referenciar também `Domain.Totvs`, como o Movidesk |

Os caminhos `..\..\..\shared\...` continuam (o Totvs fica um nível acima dos módulos de `General`).

### 2.2 Prefixo "Totvs" nos nomes

No Movidesk e no AfterSale tudo leva o nome do módulo: `MovideskTicketsService`, `IMovideskPeopleRepository`,
tabela `MovideskTickets`. No Totvs nada leva:

| Hoje | Passa a ser |
|---|---|
| `PersonService` / `IPersonService` | `TotvsPeopleService` / `ITotvsPeopleService` |
| `ProductService` | `TotvsProductsService` |
| `FiscalService` | `TotvsInvoicesService` |
| `PurchaseOrderService` | `TotvsPurchaseOrdersService` |
| `SalesOrderService` | `TotvsSalesOrdersService` |
| `SellerService` | `TotvsSellersService` |
| `AccountsPayableService` | `TotvsAccountsPayableService` |
| `*Repository`, `I*Repository` | Mesmo prefixo (`TotvsPeopleRepository`...) |
| 7 command handlers (`PersonCommandHandler`...) | Um só `TotvsCommandHandler` / `ITotvsCommandHandler`, como no Movidesk |
| `TotvsBaseSyncService` | Removida (§4.1) |
| Tabelas `People`, `Skus`, `Invoices`... | `totvs.TotvsPeople`, `totvs.TotvsSkus`, `totvs.TotvsInvoices`... (ver §5) |

### 2.3 Nomes dos métodos

**Os nomes dos métodos não mudam (decidido).** Eles seguem a nomenclatura da documentação oficial da Totvs
(verbo + rota: `POST .../search` → `PostSearch*`, `GET .../branchesList` → `GetBranchesList`) e permanecem assim,
mesmo que o Movidesk e o AfterSale usem `Get*` nas leituras:

| Método (mantido) | Rota da API |
|---|---|
| `PostSearchCustomers` | `POST /person/v2/individuals/search` |
| `PostSearchLegalEntity` | `POST /person/v2/legal-entities/search` |
| `GetBranchesList` | `GET /person/v2/branchesList` |
| `PostSearchReferences` | `POST /product/v2/references/search` |
| `PostSearchInvoices` | `POST /fiscal/v2/invoices/search` |
| `PostSearchPurchaseOrders` | `POST /purchase-order/v2/search` |
| `PostSearchSalesOrders` | `POST /sales-order/v2/orders/search` |
| `PostSearchSellers` | `POST /seller/v2/search` |
| `PostSearchDuplicates` | `POST /accounts-payable/v2/duplicates/search` |
| `PostSearchComissionsPaid` | `POST /accounts-payable/v2/comissions-paid/search` (grafia da própria rota) |

### 2.4 Namespaces e pastas

| Item | Hoje | Padrão |
|---|---|---|
| `DependencyInjection.cs` | `Infrastructure.Totvs` | `Infrastructure.Totvs.DependencyInjection` |
| `Entities/Accounts Payable/` (com espaço) | Namespace `Domain.Totvs.Entities.AccountsPayable` | Pasta `Entities/Reads/AccountsPayable/` |
| `Entities/<Recurso>/` | `Domain.Totvs.Entities.<Recurso>` | `Entities/Reads/<Recurso>/` (`Domain.Totvs.Entities.Reads.<Recurso>`) |
| `Dtos/<Recurso>/` | `Domain.Totvs.Dtos.<Recurso>` | `Dtos/Reads/<Recurso>/` |
| `Validations/<Recurso>/` | ok | ok (já segue o padrão do Movidesk) |
| `Entities/Token.cs` | Entidade | `Dtos/Reads/Authorization/Token.cs` (é resposta da API, não vai para o banco) |
| Arquivos de DTO em minúsculas | `Customerobservation.cs`, `Shippingcompany.cs`, `Socialnetwork.cs`, `Additionalfield.cs`, `Webdata.cs`, `Webdetail.cs` | `CustomerObservation.cs`, `ShippingCompany.cs`... (classe e arquivo com o mesmo nome da entidade) |

Também saem os `using` sem uso (`System.Linq`, `System.Text`, `System.Threading.Tasks` em `EnumJob.cs` e
`IAccountsPayableCommandHandler.cs`) e as variáveis nunca usadas (`somenteNovosList` em `PersonService`).

---

## 3. Domain.Totvs

### 3.1 Estrutura proposta

```
Domain.Totvs/
├── Abstractions/Repositorys/
│   ├── ITotvsRepository.cs                 (parâmetros, última alteração por recurso)
│   ├── ITotvsPeopleRepository.cs           (clientes PF, pessoas jurídicas, filiais)
│   ├── ITotvsProductsRepository.cs
│   ├── ITotvsInvoicesRepository.cs
│   ├── ITotvsPurchaseOrdersRepository.cs
│   ├── ITotvsSalesOrdersRepository.cs
│   ├── ITotvsSellersRepository.cs
│   └── ITotvsAccountsPayableRepository.cs
├── Dtos/
│   └── Reads/                               ← resposta da API (tudo string, { get; init; })
│       ├── Authorization/Token.cs
│       ├── Base/Page.cs                     (Page<T>: count, totalPages, hasNext, totalItems, items)
│       ├── People/{Customers,LegalEntities,Branches}/...
│       ├── Products/...
│       ├── Invoices/...
│       ├── PurchaseOrders/...
│       ├── SalesOrders/...
│       ├── Sellers/...
│       └── AccountsPayable/...
├── Entities/
│   ├── Parameters/Company.cs                (→ totvs.TotvsParameters)
│   └── Reads/<mesmas pastas dos DTOs>       (→ totvs.Totvs*)
└── Enums/EnumJob.cs
```

### 3.2 Convenções que mudam

* **Um `Root` por recurso vira `Page<T>`.** Hoje há 8 classes `Root` idênticas (mais `ComissionPaidRoot`), todas
  com `count`, `totalPages`, `hasNext`, `totalItems` e `items`. Um genérico em `Dtos/Reads/Base` substitui todas.
* **DTOs com `{ get; init; }`.** Hoje 840 propriedades usam `{ get; set; }` e só 171 usam `init`. O Movidesk usa
  `init` em todos os DTOs de leitura.
* **Entidades com `doc_company` e `integrated_or_updated_at`.** Nenhuma entidade do Totvs tem as duas colunas.
  No Movidesk elas estão em todas as tabelas (pai e filhas): `doc_company` entra na chave primária e
  `integrated_or_updated_at` é o horário do lote, que a merge das filhas usa para apagar o que saiu da API
  (`@delete_condition`, ver §5.3). O construtor passa a receber `(doc_company, batchTime, dto)`, como em
  `Domain.Movidesk.Entities.Reads.Services.Service`.
* **Filhas com a chave inteira do pai.** As filhas hoje só levam o código do pai (`invoiceSequence`, `orderCode`,
  `code`). Notas e pedidos da Totvs são únicos **por filial** (`branchCode` está no pai, mas não nas filhas), então
  com mais de uma filial as filhas de documentos diferentes se misturam. As filhas passam a ter
  `doc_company` + `branchCode` + código do pai.
* **Datas e números anuláveis.** Há 73 propriedades `DateTime` e nenhuma `DateTime?` nas entidades. Toda data nula
  vira `1990-01-01` (`ConvertToDateTimeValidation`). Exemplo concreto: `Invoice.disableDate` de uma nota **não**
  cancelada fica "cancelada em 01/01/1990"; `Customer.birthDate` e `hireDate` e as datas dos pedidos passam pelo mesmo
  problema. O padrão é o da `docs/PROPOSTA-DATAS-NULAS-AFTERSALE.md`: `DateTime?` com
  `NullableConverters.ToNullableDateTime`, e o mesmo para os opcionais `int?`, `decimal?` e `bool?`
  (`ToNullableInt32`, `ToNullableDecimal`, `ToNullableBoolean`). Chaves continuam não anuláveis.
* **Objetos 1:1 viram colunas do pai, com o nome do objeto como prefixo.** Revisão completa na §5.2.8: nenhum
  objeto 1:1 das APIs justifica tabela própria.
* **Corte de texto com `StringLengthValidation`.** Há 370 `x?.Length > n ? x.Substring(0, n) : x` nas entidades.
  O Movidesk usa `ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128)`.
* **Setters privados.** `Customer` (e `LegalEntity`) mistura `{ get; set; }` e `{ get; private set; }`; tudo passa
  a `private set`.
* **Sem `Responses` dentro da entidade.** Hoje cada entidade pai guarda `Dictionary<string, string> Responses` com
  o próprio DTO serializado de novo (`JsonConvert.SerializeObject(dto)`), só para o `AddRecord`. No Movidesk o
  service monta um `Dictionary<id, json>` a partir da resposta da API (`JArray.Parse`) e passa para o método que
  grava (ver `SaveServices`). Assim o registro em `auditing.Records` é o JSON **original** da API, e a entidade
  só tem colunas.
* **`Parameters/Company.cs`** herda `CompanyBase` (`doc_company`), como nos outros módulos, com as credenciais da
  Totvs (ver §5.1).
* **`EnumJob`** com os ids reais (ver §7) e o comentário apontando para o script de jobs que existir de fato.

---

## 4. Application.Totvs

### 4.1 Services

Os nomes dos métodos são mantidos (§2.3).

| Service | Métodos (nomes mantidos) | Observação |
|---|---|---|
| `TotvsService` (comum, novo) | `GetBatchTime`, `GetCompanys`, `GetToken(company)` | Mesmo papel do `MovideskService`: horário do lote, parâmetros e token, os dois em cache (§4.4). Tira a autenticação repetida de 7 lugares |
| `TotvsPeopleService` | `PostSearchCustomers`, `PostSearchLegalEntity`, `GetBranchesList` | Hoje `PersonService`, 446 linhas com o fluxo copiado três vezes |
| `TotvsProductsService` | `PostSearchReferences` | Hoje fluxo próprio, com cache estático (ver §4.4) |
| `TotvsInvoicesService` | `PostSearchInvoices` | Hoje herda da `TotvsBaseSyncService` |
| `TotvsPurchaseOrdersService` | `PostSearchPurchaseOrders` | Hoje herda da `TotvsBaseSyncService` |
| `TotvsSalesOrdersService` | `PostSearchSalesOrders` | Hoje herda da `TotvsBaseSyncService` |
| `TotvsSellersService` | `PostSearchSellers` | Hoje herda da `TotvsBaseSyncService` |
| `TotvsAccountsPayableService` | `PostSearchDuplicates`, `PostSearchComissionsPaid` | Hoje fluxo próprio, com cache estático |

**Sem classe base (decidido).** Nenhum service usa a `TotvsBaseSyncService`, que é removida. Cada método
implementa no próprio corpo as chamadas à API (busca paginada, tratamento da resposta, desserialização), como
o `GetTickets`, o `GetPeople` e o `GetServices` do Movidesk. O que é comum fica no `ITotvsService`, injetado nos
services como o `IMovideskService`: horário do lote, parâmetros da holding e o token (o login no
`authorization/v2/token` é o único ponto centralizado, porque é ele que guarda credenciais e token em cache).
Partes de gravação podem virar métodos privados do próprio service (como o `SaveServices` do Movidesk).

### 4.2 Fluxo de leitura (escrito em cada método)

Cada método segue o mesmo desenho do `GetTickets`/`GetServices` do Movidesk, escrito no próprio método:

1. abre o log em `auditing.Logs` com o job do método;
2. para cada empresa de `totvs.TotvsParameters` (a holding Newbloomers, §5.1), obtém o token pelo
   `ITotvsService.GetToken` (credenciais e token em cache, ver §4.4);
3. calcula a janela **a partir do banco**: `MAX(maxChangeFilterDate)` do recurso para a empresa, menos uma folga
   (ex.: 1 hora). É o mesmo desenho do `GetTicketsLastUpdate` do Movidesk. Hoje clientes e pessoas jurídicas
   releem **tudo desde 2000** a cada execução. Com o banco vazio, vale a carga inicial **decidida** (revisada em
   26/10/2026, decisão 3 da §10.1):

   | Recurso | Carga inicial (banco vazio) |
   |---|---|
   | Clientes PF, pessoas jurídicas e vendedores | desde `2000-01-01` |
   | Produtos | desde `2000-01-01`, em fatias de **30 dias** na carga inicial e de 7 dias nas execuções seguintes (§5.2.4) |
   | Notas fiscais e pedidos de venda | desde `2025-01-01` |
   | Pedidos de compra | últimos 5 meses |
   | Duplicatas | últimos 10 meses |
   | Comissões pagas | últimos 10 meses **pela data do fechamento** (a rota não tem filtro de alteração; a janela é relida a cada execução, §5.2.7) |
   | Filiais | desde `2000-01-01` (a rota aceita `StartChangeDate`/`EndChangeDate`, §5.2.2) |

4. pagina a busca com **todas as lojas da holding** (multiempresa), lidas de `totvs.TotvsBranches`
   (`branchCodeList` deixa de ser `{ 1 }`). Cada rota usa as lojas de um jeito, conforme a documentação (§5.2.2 a
   §5.2.7): `branchCodeList` em notas, pedidos, vendedores e duplicatas; `option.branchStaticDataList` nas
   estatísticas de pessoas; um laço por loja (`closingCompanyCode`) nas comissões; e, nos produtos, a empresa base
   `option.branchInfoCode` com todas as lojas em `change.branchInfoCodeList`. Por isso o `GetBranchesList` é o primeiro método a ser implementado, e os outros
   recursos só rodam para a empresa que já tem filiais gravadas: sem filiais, o recurso registra a mensagem em
   `auditing.Messages` e segue para a próxima empresa;
5. chama a API com `IApiClient.SendAsync` e trata o `ApiResponse`: 401 renova o token e tenta uma vez; 429 espera
   o `Retry-After` e tenta uma vez; outro erro vira mensagem em `auditing.Messages` e o recurso segue para a
   próxima empresa. Hoje `PostAsync` devolve só o corpo e o código assume sucesso: um erro vira
   `NullReferenceException` em `root.totalPages`;
6. deduplica a página por chave (o `ProductService` já faz isso para `referenceId`; passa a valer para todos);
7. compara com o cache de estado (§4.4) e fica só com o novo/alterado;
8. valida cada DTO (FluentValidation), registra as mensagens e converte para entidade com o `batchTime` do lote;
9. grava (bulk em `untreated` + merges em `totvs`), atualiza o cache e registra cada registro em
   `auditing.Records` com o JSON original;
10. fecha o log (`SetLogEndDate` + `CommitAllChanges`) num `finally`. Hoje uma exceção deixa o log aberto.

Os `try { ... } catch (Exception) { throw; }` sem efeito (8 ocorrências) saem; a desserialização passa a tratar
`JsonException` com mensagem e `continue`, como no `GetServices`. O `if/else` que escreve a mesma mensagem de
"Concluída com sucesso" nos dois ramos vira uma linha só.

### 4.3 Validações (FluentValidation)

Os validadores já seguem o padrão do Movidesk (`MustBeValidInt32`, `MustBeValidDateTime`, `MaximumLength` com
mensagem, `RuleForEach(...).SetValidator(...)`). Mudanças:

* acompanham a renomeação de pastas e DTOs (§2.4);
* os tamanhos passam a citar o script de tabela (como o comentário de `Movidesk.Validations.Services.ServiceValidator`),
  porque hoje não há script e os limites (`250`, `60`, `14`...) não têm fonte;
* registro com **uma** chamada `AddValidatorsFromAssemblyContaining<...>()` (§6.1).

### 4.4 Cache (padrão `docs/PADRAO-CACHE-INTEGRACOES.md`)

| Chave | Conteúdo | Expiração |
|---|---|---|
| `Totvs:Parameters` | Linhas de `totvs.TotvsParameters` (`doc_company` e credenciais da holding) | 2 h absoluto / 30 min deslizante |
| `Totvs:Token:<doc_company>` | `access_token` obtido no `authorization/v2/token` | `expires_in` da resposta (3600 s no teste) menos 5 min. Hoje é feito um login por execução; um 401 tira o token do cache e faz um novo login |
| `Totvs:<Recurso>:<doc_company>` | Cache de estado (§3.2 do padrão): chave → entidade do banco | 2 h absoluto / 30 min deslizante (hoje 1 h) |

Mudanças em relação ao que existe:

* **`ProductService` e `AccountsPayableService` usam `static List<>`** no lugar do `IMemoryCache`: a lista nunca
  expira, só é carregada quando está vazia (com as chaves da primeira execução) e é compartilhada entre
  execuções simultâneas sem proteção. No `ProductService` ela nem é atualizada depois de gravar, então os SKUs
  da primeira execução são regravados em toda execução até o processo reiniciar. Os dois passam ao
  `IMemoryCache` como os outros.
* **O cache guarda a entidade, não o DTO.** Hoje `GetExisting*` lê o banco para dentro do DTO (tudo `string`) e
  compara com `DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))`. A coluna é `datetime`; o Dapper
  converte para `string` na cultura do servidor (`pt-BR`: `22/10/2026`), e o `Parse` em `en-US` lê dia como mês
  ou lança exceção. Com a entidade, a comparação fica
  `ConvertToDateTimeValidation(x.maxChangeFilterDate) <= y.maxChangeFilterDate`, como no padrão (§4).
* A chave leva o `doc_company`, porque o cache passa a ser por empresa.

### 4.5 Command handler (queries)

* Um `TotvsCommandHandler` com as queries de todos os recursos, como o `MovideskCommandHandler`.
* As queries passam a ler `totvs.Totvs*` filtrando por `DOC_COMPANY` (hoje `[TOTVS].[PEOPLE]`, sem empresa).
* Nova query `CreateGetBranchCodesQuery(doc_company)`: os códigos de `totvs.TotvsBranches` da empresa, usados no
  `branchCodeList` de todas as buscas (§4.2).
* Novas queries `CreateGet<Recurso>LastChangeQuery(doc_company)` (`MAX(MAXCHANGEFILTERDATE)`) para a janela (§4.2).
* Os códigos da Totvs são numéricos: o `IN` passa a usar os números sem aspas (como `CreateGetTicketsByIdsQuery`),
  e o repositório continua sem chamar a query com lista vazia.

---

## 5. Banco de dados

### 5.0 Schema `totvs` (decidido)

Todas as tabelas do módulo ficam no schema próprio **`totvs`**, no lugar do `general` usado por Movidesk e
AfterSale. O resto da convenção não muda:

* o bulk continua indo para `untreated` (o `sp_Create_Merge_Procedure` sempre lê `untreated.<mesmo nome>`);
* por isso o prefixo `Totvs` **fica** no nome da tabela, mesmo dentro do schema `totvs`: o `untreated` é
  compartilhado por todos os módulos, e `untreated.People` ou `untreated.Invoices` colidiriam com outros;
* as merges ficam em `totvs.sp_Totvs<Tabela>_Sync` (o `CallDbProcMerge("totvs", ...)` de hoje já usa esse schema);
* o primeiro script cria o schema (`if not exists (select 1 from sys.schemas where name = 'totvs') exec('create schema totvs')`),
  como o `Script001` do AfterSale faz com `general`.

### 5.1 `totvs.TotvsParameters` (novo; tira as credenciais do código)

Mesmo formato de `MovideskParameters`/`AfterSaleParameters`, com as credenciais do fluxo `password` da Totvs.
Não há coluna de filiais: elas vêm de `totvs.TotvsBranches` (§4.2).

**Uma linha só: a holding Newbloomers.** As credenciais da Totvs são as mesmas para todas as lojas, então há duas
formas de preencher a tabela:

| Opção | Como funciona | Avaliação |
|---|---|---|
| **A. Holding (recomendada)** | Um insert, no script de valores, da holding Newbloomers (`doc_company` = CNPJ da holding). O `GetBranchesList` grava em `totvs.TotvsBranches` todas as lojas que a holding enxerga, e as demais buscas usam essas lojas no `branchCodeList` | Um login e um token para tudo; nenhum job a mais; as credenciais ficam numa linha só (trocar a senha é um `update`). O CNPJ de cada loja continua disponível em `TotvsBranches.cnpj` |
| B. Uma linha por loja | Um job no `TotvsService` lê as lojas e insere cada CNPJ em `TotvsParameters`, repetindo as credenciais | Credenciais copiadas em N linhas, N logins com o mesmo usuário e um job só para manter a tabela |

Fica a opção **A**: `doc_company` em todas as tabelas do módulo é o CNPJ da holding, **`44065634000106`**, e a loja
de cada registro é o `branchCode` (que já está nas notas, pedidos etc. e passa a estar nas filhas, §3.2). O
`TotvsService` lê a tabela pelo `GetCompanys`, com cache (§4.4); nenhum job de cadastro de empresas é criado.

Conferido no `GET /person/v2/branchesList` em 23/10/2026: o usuário de integração enxerga **25 empresas**, e a
própria holding é uma delas (código `900`, "HOLDING - SP", CNPJ `44065634000106`):

| Faixa | Empresas |
|---|---|
| 1–15 | Misha (matriz `1`, CNPJ raiz `38367316`; lojas, CD `8`; `5` e `10` com "INATIVA" no nome) |
| 101–108 | Open Era (matriz `101`, CNPJ raiz `42538267`; e-commerce `102` e lojas) |
| 900 | Holding (`44065634000106`) |
| 901 | Margaux ("INATIVA" no nome) |

O filtro `BranchCNPJPool=44065634000106` da rota **não** devolve nenhuma empresa (o pool não é a holding), então as
lojas vêm da lista completa, sem filtro de pool. A rota não tem campo de situação: as inativas vêm junto e entram no
`branchCodeList` (não causam erro; só não trazem movimento).

```sql
create table [totvs].TotvsParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key,
	[doc_company] nvarchar(14) not null,   --CNPJ da empresa (chave em todas as tabelas do módulo)
	client_id nvarchar(100) not null,
	client_secret nvarchar(200) not null,
	username nvarchar(100) not null,
	[password] nvarchar(200) not null
);
```

**As credenciais do código não funcionam mais.** Em 23/10/2026 o login com elas respondeu 400
(`"Invalid user/password."`), e as credenciais válidas informadas para o teste diferem das do código em
`client_id`, `client_secret`, `username` e `password` (só o `grant_type` é igual). Ou seja, mesmo compilando, o
módulo de hoje não autenticaria. O script de valores de `TotvsParameters` usa as credenciais válidas (trocadas,
conforme a decisão abaixo).

**Segurança (decidido):** as credenciais de produção estão hoje no código e, portanto, no histórico do git. Na
migração para `TotvsParameters` elas serão **trocadas na Totvs**, e só as novas entram na tabela (pelo script de
valores, que fica **fora do git**: no repositório vai só o modelo `Script003_..._Table.sql.example`, e o `.sql` com
as credenciais está no `.gitignore`; o Movidesk, ao contrário, versiona o token no `Script011`). Remover do código
não as tira do histórico.

### 5.2 Tabelas de dados

Mesma convenção do Movidesk e do AfterSale: par `untreated.Totvs<Tabela>` (sem chave, recebe o bulk) e
`totvs.Totvs<Tabela>` (com chave primária), com `integrated_or_updated_at` e `doc_company` em todas.

| Recurso | Tabela pai (chave) | Filhas |
|---|---|---|
| Clientes PF | `TotvsPeople` (`doc_company`, `code`) | `Addresses`, `Phones`, `Emails`, `Observations`, `CustomerObservations`, `AdditionalFields`, `Classifications`, `References`, `Relateds`, `Statistics`, `Representatives`, `Familiars` |
| Pessoas jurídicas | `TotvsLegalEntities` (`doc_company`, `code`) | `AdditionalFields`, `Addresses`, `Classifications`, `Contacts`, `CustomerObservations`, `Emails`, `Observations`, `Partners`, `PaymentMethods`, `Phones`, `References`, `Relateds`, `Representatives`, `SocialNetworks`, `Statistics` |
| Filiais | `TotvsBranches` (`doc_company`, `code`) | `Addresses` |
| Produtos | `TotvsSkus` (`doc_company`, `branchInfoCode`, `referenceId`; §5.2.4) | `Observations`, `Details`, `Compositions`, `ReferenceCategories`, `Colors` → `MeasurementsTableOmni` → `Medidas`, `Colors` → `Products` → `BarCodes`, `Classifications`, `AdditionalFields`, `Suppliers`, `Manufacturers`, `ReferenceCodeSequences`, `WebData`, `WebDetails`, `BranchesBlocked`, `ConservationInstructions` → `Items` |
| Notas fiscais | `TotvsInvoices` (`doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`; confirmada no swagger, ver §5.2.1) | `Payments`, `SalesOrders`, `ReferencedTaxes`, `ObservationsNF`, `Items` → `Taxes`, `Products`, `BarCodes`, `ProductsReferenced` |
| Pedidos de compra | `TotvsPurchaseOrders` (`doc_company`, `branchCode`, `orderCode`) | `Products`, `Requests` (§5.2.6) |
| Pedidos de venda | `TotvsSaleOrders` (`doc_company`, `branchCode`, `orderCode`) | `Products` → `BatchItems`, `Invoices`, `Observations`, `InvoiceObservations`, `RepresentativeObservations`, `Classifications`, `Discounts`, `Commissioneds`, `Counts` |
| Vendedores | `TotvsSellers` (`doc_company`, `sellerCode`) | `Customers`, `BranchInformations` |
| Contas a pagar | `TotvsDuplicates` (`doc_company`, `branchCode`, `supplierCode`, `duplicateCode`, `installmentCode`; §5.2.7), `TotvsCommissionsPaid` (`doc_company`, `closingBranchCode`, `closingCode`, `commissionedCpfCnpj`; §5.2.7) | `TotvsDuplicatesExpenses`, `TotvsDuplicatesClassifications`, `TotvsCommissionsPaidAccountMovements` |

Os nomes das filhas seguem os atuais (`SellersCustomer` e `SellersBranchInformation` passam ao plural).

### 5.2.1 Notas fiscais: chave e conferência com a API

Fonte: swagger da API Fiscal (`/api/totvsmoda/fiscal/v2/swagger/index.html`, OpenAPI em
`.../swagger/v1/swagger.json`), rota `POST /api/totvsmoda/fiscal/v2/invoices/search` (`InvoiceSearchInDto` →
`InvoiceSearchOutDto` → `InvoiceDataModel`), conferida em 23/10/2026.

**Chave da nota: `doc_company` + `branchCode` + `invoiceSequence` + `invoiceDate`.**

* `branchCode` é a "Empresa", `invoiceSequence` é o "Fatura" (código sequencial) e `invoiceDate` é a "Data fatura"
  (data de movimento), os três do componente FISFL030. Os três são **não anuláveis** no schema.
* A própria API identifica uma nota por esses três campos: `GET /fiscal/v2/invoices/item-detail-search` recebe
  `BranchCode`, `InvoiceDate` e `InvoiceSequence` (ou `AccessKey`). São também os campos aceitos em `order`, ao
  lado de `maxChangeFilterDate`.
* `invoiceCode` (número da nota) + `serialCode` não servem de chave: são anuláveis e se repetem entre modelos de
  documento. `eletronic.accessKey` também não: só existe em nota eletrônica.
* `invoiceDate` é `date-time` na API; a coluna fica `datetime not null` (não `date`), gravada como vier.

Chave das filhas (todas começam com a chave da nota):

| Tabela | Origem na API | Chave própria | Observação |
|---|---|---|---|
| `TotvsInvoicesItems` | `items[]` (`InvoiceItemDataModel`) | `sequence` ("Item") | Anulável no schema; item sem `sequence` é rejeitado na validação |
| `TotvsInvoicesItemsProducts` | `items[].products[]` | `sequence` do item + `productCode` | |
| `TotvsInvoicesItemsTaxes` | `items[].taxes[]` | `sequence` do item + `code` (imposto) | |
| `TotvsInvoicesItemsBarCodes` | `items[].barCodes[]` | `sequence` do item + `barcode` | |
| `TotvsInvoicesItemsProductsReferenced` | `items[].products[].referencedInvoices[]` | `sequence` do item + `productCode` + `invoiceSequenceItem` | NFs referenciadas de itens devolvidos |
| `TotvsInvoicesPayments` | `payments[]` (`TransactionPaymentDataModel`) | `documentNumber` + `installment` | `documentNumber` é `int64` na API (coluna `bigint`) |
| `TotvsInvoicesSalesOrders` | `salesOrder[]` | `orderBranchCode` + `orderCode` do pedido | Mesma chave do pedido (ver abaixo). Vínculo sem os dois é descartado com mensagem no log |
| `TotvsInvoicesObservationsNF` | `observationNF[]` | `sequence` | |
| `TotvsInvoicesReferencedTaxes` | `referencedTaxInvoice[]` | `referencedInvoiceBranchCode` + `invoiceCode` + `invoiceSerialCode` | `accessKey` só em eletrônicas |

**Chave do vínculo nota → pedido**, conferida no swagger da API de pedidos de venda
(`/api/totvsmoda/sales-order/v2/swagger/index.html`, rota `POST /sales-order/v2/orders/search`, em 23/10/2026):

* no pedido (`OrderDataModel`), `branchCode` ("Empresa", até 4 dígitos) e `orderCode` ("Número", até 9 dígitos)
  são **não anuláveis**; `orderId` ("Cód. único", até 100 caracteres) é anulável;
* a API de pedidos identifica um pedido por `BranchCode` + `OrderCode` **ou** por `OrderId`, nunca pelos três
  juntos (`GET /sales-order/v2/invoices`, `pending-items`, `DELETE items`);
* na nota (`InvoiceSalesOrderDataModel`) os mesmos campos vêm anuláveis, mas apontam para essa chave.

Por isso a chave do vínculo é a da nota + `orderBranchCode` + `orderCode` (o `branchCode` do pedido ganha o
prefixo para não confundir com o `branchCode` da nota; um pedido pode ser de outra filial). `orderId` e
`customerOrderCode` ficam como colunas. Um vínculo sem `branchCode` ou `orderCode` não entra na chave: o validador
registra a mensagem e ele é descartado. A mesma consulta confirma a chave de `TotvsSaleOrders` da §5.2
(`doc_company`, `branchCode`, `orderCode`).

`eletronic`, `ecf`, `sat`, `shippingCompany`, `person` e `productionOrder` são objetos únicos: continuam como
colunas da própria nota, com o nome do objeto como prefixo e todos os campos (§5.2.8).

Outros pontos da documentação que mudam o service de notas:

* **`expand` fora da lista da API.** A documentação aceita `shippingCompany`, `salesOrder`, `person`, `items`,
  `barCodes`, `taxes`, `payments`, `referencedInvoices`, `rateDifferential`, `relatedProductionOrder`,
  `observationNF`, `observationNFE` e `referencedTaxInvoices`. O código envia `eletronic`, `ecf`, `sat`,
  `productionOrder`, `products`, `digitalWalletInformation` e `referencedTaxInvoice` (singular), que não estão na
  lista, e não envia `referencedInvoices`, `relatedProductionOrder` nem `observationNFE`. O `expand` passa a ser o
  da documentação. **Resultado do teste real** (23/10/2026, filial 1, alterações de 21 e 22/10, 5 notas):

  | `expand` enviado | Resposta |
  |---|---|
  | O do código | 200. `productionOrder` e `referencedTaxInvoice` vêm **nulos** |
  | O da documentação | 200. Mesmo conteúdo, mais `productionOrder` (objeto) e `referencedTaxInvoice` (lista) |
  | Vazio, `"xyzabc"` ou só `"eletronic"` | 200. Só a capa da nota e `eletronic`; nenhum grupo expandido |

  Conclusões: valores fora da lista são **ignorados em silêncio** (nenhum erro 400); `eletronic`, `ecf` e `sat`
  vêm sempre, sem `expand`; `products` do item vem junto com `items`; e **o código de hoje perde a ordem de
  produção e as notas referenciadas** porque usa `productionOrder` (o certo é `relatedProductionOrder`) e
  `referencedTaxInvoice` no singular (o certo é `referencedTaxInvoices`). `observationNFE` veio nulo nos dois
  casos e `digitalWalletInformation` (dentro de `payments`) não apareceu; como não existem notas com esses campos
  preenchidos, os dois seguem **a documentação** (decisão 13 da §10).
* **O que mais o teste mostrou:**
  * `invoiceDate` vem só com a data (`"2026-10-20"`) e `maxChangeFilterDate` sem fuso (`"2026-10-21T08:18:27"`);
  * `items[].sequence` não se repetiu dentro da nota (65 a 90 itens por nota), o que confirma a chave dos itens;
  * `productionOrder` vem preenchido com zeros (`branchCode`, `cycleCode` e `orderCode` = 0,
    `orderRelationshipType = "0"`) quando a nota não tem ordem de produção: zero vira `null` na entidade;
  * `invoiceStatus` e `operationType` vêm pelo **nome** do enum (`"Issued"`, `"Output"`), não pela letra do swagger
    (`E`, `S`); as colunas e as comparações usam o nome;
  * `count` é o total de notas da busca (152) e `totalItems` é o total da página (5);
  * cada nota com o `expand` completo tem ~220 KB; com o `pageSize` decidido (100, o máximo) a página passa de
    20 MB, dentro do timeout de 20 min do `HttpClient`;
  * `accessKey` das notas referenciadas (`referencedTaxInvoice[]` e `products[].referencedInvoices[]`) não tem
    tamanho no swagger e passou de 44 caracteres em notas reais: as duas colunas ficam com 100 (a da `eletronic`
    vai para 50: o swagger diz 44, que é a chave da NF-e, mas a NFS-e (`documentType` 81) vem com a chave nacional
    de 50 dígitos, testado).
* **Erro da API com `referencedInvoices`** (testado em 23/10/2026). Para algumas devoluções de venda a busca responde
  `400` com `[{"code":"GeneralMessage","message":"Object reference not set to an instance of an object..",
  "detailedMessage":"InvoiceDomainService.GetReferencedInvoices"}]`, e a página inteira se perde; sem
  `referencedInvoices` no `expand` a mesma página responde 200. Isolado em uma nota (loja 7, sequência 42572,
  21/10/2026, "021 - DEVOLUCAO DE VENDA - FISICO"); em 21 a 23/10 foram 9 notas de 1.272. **Decisão: o
  `referencedInvoices` fica comentado no `expand`** do service, com comentário explicando o problema da API;
  `TotvsInvoicesItemsProductsReferenced` fica vazia enquanto isso. Se for necessário, basta trocar pela linha comentada:
  o contorno continua no service. Contorno: num `400` com `GetReferencedInvoices`, a página é relida em páginas 10 vezes menores (100 → 10 → 1) e só a nota
  que falha sozinha é lida sem `referencedInvoices`, com mensagem no log identificando-a (as outras filhas dela vêm
  normalmente). A ordem por `maxChangeFilterDate` não muda entre as chamadas; uma nota alterada no meio da releitura
  volta na execução seguinte (janela de 1 h). Custo medido com ele ligado: os 3 dias levaram 160 chamadas e ~7 min
  em vez de 13.
* **`branchCodeList` é obrigatório** no filtro (`InvoiceFilterModel.required`), o que reforça a leitura das
  filiais de `TotvsBranches` (§4.2).
* **`pageSize` máximo 100** (o código já usa 100).
* **`maxChangeFilterDate` só vem preenchido quando `filter.change.startDate` e `endDate` são enviados.** O fluxo
  sempre envia a janela (§4.2); o validador passa a registrar nota sem `maxChangeFilterDate`.
* **`filter.change` é aplicado primeiro** e os demais filtros restringem o resultado ("E"). Os filtros por data
  de emissão (`startIssueDate`/`endIssueDate`, máximo 6 meses; `amountLastDays`, máximo 180 dias) não são usados.
* **Datas anuláveis confirmadas pela API:** `issueDate`, `lastchangeDate`, `maxChangeFilterDate`,
  `eletronic.receivementDate` e `eletronic.disableDate` são `nullable` no schema; viram `DateTime?` (§3.2).
* **Situação da nota:** `invoiceStatus` (cancelada, normal, emitida, excluída, denegada; na resposta, pelo nome:
  `Canceled`, `Normal`, `Issued`, `Deleted`, `Denied`) já vem na própria nota. Notas excluídas na Totvs continuam
  no banco com `invoiceStatus = 'Deleted'`; nada é apagado.

### Demais APIs: fontes e resumo (detalhe em 5.2.2 a 5.2.7)

Fontes (swagger de cada API, `.../swagger/v1/swagger.json`), conferidas em 23/10/2026 e testadas com chamadas reais
(holding, todas as 25 empresas, janelas curtas):

| API | Swagger |
|---|---|
| Person | `https://www30.bhan.com.br:9443/api/totvsmoda/person/v2/swagger/index.html` |
| Product | `https://www30.bhan.com.br:9443/api/totvsmoda/product/v2/swagger/index.html` |
| Seller | `https://www30.bhan.com.br:9443/api/totvsmoda/seller/v2/swagger/index.html` |
| Purchase Order | `https://www30.bhan.com.br:9443/api/totvsmoda/purchase-order/v2/swagger/index.html` |
| Accounts Payable | `https://apitotvsmoda.bhan.com.br/api/totvsmoda/accounts-payable/v2/swagger/index.html` |

Resumo do que a documentação define para as rotas usadas:

| Rota | `pageSize` máx. (código hoje) | Filtro obrigatório | `expand` aceito | `order` aceito |
|---|---|---|---|---|
| `GET /person/v2/branchesList` | 1000 (não envia) | — | `addresses` | `code` |
| `POST /person/v2/individuals/search` | 500 (500) | — | `phones`, `addresses`, `emails`, `classifications`, `additionalFields`, `references`, `observations`, `relateds`, `shippingCompany`, `statistics`, `representatives`, `customerObservations`, `familiars`, `individualAdditionals` | `personCode`, `maxChangeFilterDate` |
| `POST /person/v2/legal-entities/search` | 500 (500) | — | `phones`, `addresses`, `emails`, `classifications`, `additionalFields`, `references`, `observations`, `relateds`, `partners`, `shippingCompany`, `contacts`, `statistics`, `paymentMethods`, `preferences`, `socialNetworks`, `representatives`, `customerObservations` | `personCode`, `maxChangeFilterDate` |
| `POST /product/v2/references/search` | 1000 (500) | `option.branchInfoCode` | `observations`, `details`, `barCodes`, `classifications`, `additionalFields`, `composition`, `suppliers`, `manufacturers`, `referenceCodeSequences`, `webData`, `webDetail`, `referenceCategories`, `measurementTableOmni`, `branchesProductBlocked`, `conservationInstructions`, `integrationGroupCodes` | `maxChangeFilterDate` |
| `POST /seller/v2/search` | 500 (100) | — | (a rota não tem `expand`) | (a rota não tem `order`) |
| `POST /purchase-order/v2/search` | 100 (100) | `branchCodeList` | `items`, `requests` | `branchCode`, `orderCode`, `maxChangeFilterDate` |
| `POST /accounts-payable/v2/duplicates/search` | 100 (100) | `branchCodeList` (máx. 900 itens) | `classifications` | `maxChangeFilterDate` (e `productCode`, erro de cópia da doc) |
| `POST /accounts-payable/v2/comissions-paid/search` | 100 (500) | `closingCompanyCode` (uma empresa) | `accountMovements` | `closingDate`, `commissionedCode` |

Em todas as rotas com `change`, o `maxChangeFilterDate` só vem preenchido quando `startDate` e `endDate` são
enviados, como nas notas. Os enums vêm pelo nome na resposta (`"Female"`, `"NotDeclared"`, `"Active"`), exceto o
`status` das duplicatas, que vem pela letra (`"N"`).

### 5.2.2 Filiais (`GetBranchesList`)

* Parâmetros por query string: `StartChangeDate`, `EndChangeDate`, `BranchCodeList`, `BranchCnpjList`,
  `BranchCodePool`, `BranchCNPJPool`, `Page`, `PageSize` (máx. 1000), `Order`, `Expand`.
* **Endereços só vêm com `Expand=addresses`.** O código chama a rota sem parâmetro nenhum: `addresses` vem nulo e
  `TotvsBranchesAddresses` nunca recebe linha (testado).
* **`maxChangeFilterDate` só vem com `StartChangeDate`/`EndChangeDate`** (testado: sem eles vem nulo nas 25
  empresas). Isso resolve o item 8 da §8: a rota tem janela e o método passa a ser incremental como os outros.
* Chave: `doc_company` + `code`. `addresses` não tem chave própria confiável (`sequenceCode` é anulável): a chave da
  filha é `code` + `addressTypeCode` + `sequenceCode` (nulo vira 0 na entidade).
* No teste real, `addressTypeCode` veio **0** com `addressType = "Commercial"` (o swagger lista Comercial como 1). A
  coluna grava o que vier; a chave continua única porque `sequenceCode` não se repetiu (25 empresas, 25 endereços).

### 5.2.3 Vendedores (`PostSearchSellers`)

* A rota **não tem `expand` nem `order`**: `branchInformations` e `customers` vêm sempre (o `expand` que o código
  envia é ignorado).
* **`branchCodeList` filtra também as filhas.** Teste: com as 25 empresas vieram 135 vendedores e o vendedor `1`
  com 14 `branchInformations`; com `branchCodeList = [1]` (como hoje) vieram 86 vendedores e o mesmo vendedor com
  1 `branchInformation`. O código de hoje perde 49 vendedores e as informações das outras lojas.
* `pageSize` aceita 500 (hoje 100). `personCode` veio nulo em 35 dos 135 vendedores (é anulável no schema).
* Teste com as 25 empresas e janela de 5 meses: 135 vendedores numa página, 1.567 `branchInformations`, nenhuma
  loja fora do `branchCodeList`, nenhum `customers` (a lista vem vazia em todos).
* **As datas do filtro são do horário local, e o `Z` é ignorado** (testado: `17:58:00` e `17:58:00Z` trazem o
  mesmo vendedor alterado às `17:58:16`; `20:58:00Z` não traz). A janela vai sem fuso, no mesmo relógio do
  `maxChangeFilterDate` gravado; vale para todas as rotas com `change`.
* Chaves: `TotvsSellers` (`doc_company`, `sellerCode`); `TotvsSellersBranchInformations` (+ `branchCode`);
  `TotvsSellersCustomers` (+ `personCode`).

### 5.2.4 Produtos (`PostSearchReferences`)

* **`option.branchInfoCode` é obrigatório e é uma empresa só** ("empresa base para as informações"). A referência
  é do cadastro geral, não da loja: o multiempresa aqui não é um laço por loja, e sim a escolha da empresa base.
* **Decidido: uma chamada por marca**, com as matrizes como empresa base: `1` (Misha) e `101` (Open Era). Em cada
  chamada:
  * `option.branchInfoCode` e `filter.branchInfo.branchCode` = a matriz da marca (hoje os dois são `1` fixo);
  * `change.branchInfoCodeList` = as lojas da marca em `TotvsBranches` (mesma raiz de CNPJ da matriz: `38367316`
    para a Misha, `42538267` para a Open Era), para trazer também a referência alterada só na informação de uma
    loja;
  * os demais filtros de hoje (`isActive = true`, `isFinishedProduct = true`) continuam.
* As matrizes ficam numa lista do próprio service (não são parâmetro da holding: `TotvsParameters` não tem coluna
  de filiais, §5.1). Uma marca nova entra nessa lista.
* **A empresa base entra na chave.** Os campos por empresa do produto (`isActive`, `isFinishedProduct`,
  `isBlocked`, estoques mínimo/máximo...) vêm da empresa base, então a mesma referência pode vir nas duas
  chamadas com valores diferentes. `TotvsSkus` e todas as filhas ganham `branchInfoCode` na chave.
* **`expand` do código incompleto.** Envia `filter` (não existe) e não envia `manufacturers`,
  `referenceCodeSequences`, `measurementTableOmni`, `branchesProductBlocked`, `conservationInstructions` nem
  `integrationGroupCodes`, embora o repositório tenha tabela para quase todos: `ProductsManufacturers`,
  `ProductsReferenceCodeSequences`, `SkusColorsMeasurementsTableOmni(Medidas)`, `ProductsBranchesBlocked` e
  `ProductsConservationInstructions(Items)` ficam sempre vazias (testado: com o `expand` do código esses grupos vêm
  nulos).
* **`measurementTableOmni` exige `option.measurementTableOmniType`.** Com o `expand` completo e sem esse campo a
  API responde 400 (`Invalid option.MeasurementTableOmniType value null`). O valor é **numérico** (`1` medida
  inicial, `2` aprovada, `3` conferência); o nome do enum (`"MedidaAprovada"`) dá 400. Testado com `2`: 200.
* `integrationGroupCodes` é novo (lista de `code`) e entra como filha `TotvsSkusIntegrationGroupCodes`.
* `pageSize` aceita 1000 (hoje 500). Uma referência com 20 produtos e o `expand` completo teve ~70 KB.
* Chaves: `TotvsSkus` (`doc_company`, `branchInfoCode`, `referenceId`); cores (+ `colorCode`, anulável no schema: nulo vira `''`);
  produtos (`code`, único no cadastro); filhas do produto (+ `code` do produto + a chave do item: `barCodes.code`,
  `classifications.typeCode`+`code`, `additionalFields.code`, `suppliers.code`, `manufacturers.code`,
  `branchesProductBlocked.branchCode`, `conservationInstructions.code`, `items.order`+`item`).
* `observations` da referência e `composition` não têm chave na API (só texto e `material`/`percentage`): a filha
  ganha uma coluna `sequence` com a posição na lista.
* **Testes na implementação (23/10/2026)**, que mudaram o desenho:
  * **Desempenho e instabilidade.** Com o `expand` completo, `pageSize` 1000 levou 105 s numa marca e deu **502**
    (timeout do gateway da Totvs) aos 78 s na outra; `pageSize` 200 respondeu em ~110 s; sem `expand`, 1000 levou
    ~50 s. Também veio um 502 em 13 s e uma resposta truncada no meio do JSON. Por isso: `pageSize` **200**, uma
    nova tentativa em 502/503/504 (aplicada também a filiais e vendedores) e resposta inválida tratada como falha;
  * **Volume da carga inicial.** Em 5 meses o `count` é de ~20 mil referências por marca (com ou sem `inBranchInfo`);
    em 1 dia, ~100. A carga inicial (desde 2000) é lida em **fatias de 30 dias** e as execuções seguintes em fatias
    de 7 dias; cada fatia é gravada ao terminar:
    uma falha interrompe só o resto, e a próxima execução continua do `MAX(maxChangeFilterDate)`;
  * **A página volta com menos itens que o `pageSize`** (675 de 1000, 177 de 200, 88 de 106): o fim da leitura é o
    `totalPages`, não uma página curta;
  * **Sem filtro de marca, as duas chamadas trazem o mesmo catálogo.** Numa janela de 1 dia, as 56 referências da
    matriz 1 vieram também na matriz 101, com `isActive` e `isFinishedProduct` nulos na cópia da Open Era; o filtro
    `branchInfo` (ativo, acabado) não separa as marcas. **Decidido: filtrar pela classificação 6000** em cada
    chamada, `filter.classifications = [{ type: 6000, codeList: [<código da marca>] }]`. A classificação 6000 é o
    **SETOR**, e os códigos conferidos na API são `2` = MISHA e `4` = OPEN ERA (`1` = GERAL, só itens de consumo
    como "ENERGIA ELETRICA", fica de fora). Com `codeList` `"1"` e `"101"` (os códigos das matrizes) a busca volta
    vazia, por isso o service usa o mapa matriz → setor (`1` → `"2"`, `101` → `"4"`). Teste com o filtro: 85 e 54
    referências no dia, **nenhuma nas duas matrizes** e nenhum produto com `isActive` nulo;
  * chaves conferidas em 1.028 referências (amostra das duas marcas): **nenhuma chave repetida** nas 22 tabelas
    com as chaves desta seção (com `details` pela posição e as sequências de código por `referenceId` +
    `parentCode`); `details[].type` vem pelo nome do enum ou pelo próprio código (`"6"`), e a coluna guarda o que vier.

### 5.2.5 Pessoas físicas e jurídicas (`PostSearchCustomers`, `PostSearchLegalEntity`)

* **Estatísticas exigem `option.branchStaticDataList`.** Sem a lista de empresas, `statistics` vem nulo mesmo com
  `statistics` no `expand` (documentado e testado). O código não envia `option`, então `PeopleStatistics` e
  `LegalEntitiesStatistics` ficam sempre vazias. A lista passa a ser as lojas de `TotvsBranches` (máx. 500 itens).
* **PJ: `expand` do código é o do produto** (item 2 da §8), confirmado no teste: `addresses`, `phones`, `emails`,
  `contacts`, `partners` etc. vêm nulos. Com o `expand` da documentação vêm preenchidos.
* **PF:** o código envia `preferences` e `cpf`, que não estão na lista da PF. `preferences` vem mesmo assim
  (testado) e fica; `cpf` é ignorado e sai. Falta `individualAdditionals` (dados complementares: escolaridade,
  documentos, nome social...), que entra no `expand`.
* **Decidido: `individualAdditionals` vira colunas da própria `TotvsPeople`** (60 campos, prefixo
  `individualAdditionals`, §5.2.8). Atenção a `homeTown`: existe na pessoa (`string`, nome da cidade) e em
  `individualAdditionals` (`int32`, código da cidade); o prefixo separa as duas colunas.
* `customerObservations` traz só as 10 últimas por padrão (`option.QuantityOcurrenceCustomerObservation`).
* Tipos que mudam na entidade: `ctps` e `ctpsSerial` são `int64` (hoje `int`); `monthlyIncome` é `double` (hoje
  `int`, perde os centavos); `gender`, `maritalStatus` e `status` vêm pelo nome do enum.
* Volume: 645 PF alteradas em 3 dias (pageSize 500, ~2 páginas por execução de 3 min).
* **Testes na implementação (23/10/2026):**
  * carga inicial: desde 2000 são **188.157 pessoas físicas**, todas com alteração em 2026, e **2.238 jurídicas**;
    500 PF com o `expand` completo levaram 46 s. A carga é lida em **fatias de 30 dias**, cada uma gravada ao
    terminar (como nos produtos), e 30 dias evitam ~1.300 chamadas vazias entre 2000 e 2025;
  * chaves das filhas conferidas em 613 PF e 33 PJ: **nenhuma repetida**, nenhum `sequence` nulo; nenhum texto
    acima do tamanho do swagger;
  * `preferences` vem sempre e `individualAdditionals` em 143 de 500 PF, mas **com todos os campos nulos** na base
    atual: as colunas com prefixo existem e ficam nulas/vazias;
  * as datas das estatísticas vêm só com a data (`"2026-10-20"`); os enums, pelo nome (`"Female"`, `"Married"`,
    `"Active"`, `"Commercial"`).
* Chaves das filhas (todas com `doc_company` + `code` da pessoa): `addresses` (`addressTypeCode` + `sequenceCode`),
  `phones` (`Sequence`, com S maiúsculo na API), `emails` (`sequence`), `observations` e `customerObservations`
  (`sequence`, anulável: nulo vira a posição), `additionalFields` (`code`), `classifications` (`typeCode` + `code`),
  `references` (`sequence`), `relateds` e `partners` (`code`), `contacts` e `socialNetworks` (`sequence`),
  `statistics` (`branchCode` + `customerCode`), `representatives` (`representativeCode`), `familiars` (sem chave na
  API: posição na lista), `paymentMethods` (lista simples de valores: o próprio valor).

### 5.2.6 Pedidos de compra (`PostSearchPurchaseOrders`)

* `expand` aceita `items` e `requests`; o código só envia `items`. `requests` (solicitações de compra ligadas ao
  pedido: `branchCode`, `requestCode`, `requestDate`) passa a ser a filha `TotvsPurchaseOrdersRequests`.
* Chaves: `TotvsPurchaseOrders` (`doc_company`, `branchCode`, `orderCode`, não anuláveis); `Products` (+
  `productCode`); `Requests` (+ `branchCode` e `requestCode` da solicitação).
* `maxChangeFilterDate`, `registrationDate`, as datas de entrega e de pagamento são anuláveis no schema.
* Volume no teste: 321 pedidos alterados em 7 semanas nas 25 empresas.
* **Testes na implementação (23/10/2026):** 294 pedidos de compra (até 51 itens por pedido) e 436 de venda (até 80
  itens), com nenhuma chave repetida nas filhas e nenhum texto acima do tamanho. As chaves das filhas de venda
  ficaram: itens por `productCode`, itens de lote por `sequence`, notas por `code` + `serial`, contagens por
  `branchCode` + `code`, classificações por `typeCode` + `code`, descontos por `typeDiscountCode`, comissionados por
  `representativeCode` e as três observações pela posição (sem chave na API). Os objetos 1:1 `shippingAddress` (no
  pedido) e `electronic` (na nota) viraram colunas com prefixo; os 206 `electronic` da amostra vieram preenchidos.
  `requests` (compra) e `batchItems` (venda) vieram vazios na amostra.

### 5.2.7 Contas a pagar (`PostSearchDuplicates`, `PostSearchComissionsPaid`)

**Duplicatas**

* **A chave não é `duplicateCode`.** O mesmo `duplicateCode` volta uma vez por parcela (`installmentCode` 4, 5...
  no teste) e pode se repetir entre fornecedores e empresas. O código deduplica e compara o cache só por
  `duplicateCode`, então só uma parcela de cada duplicata é gravada. A chave passa a ser `doc_company` +
  `branchCode` + `supplierCode` + `duplicateCode` + `installmentCode` (todos não anuláveis no schema).
* `duplicateCode` é `int64` na API (hoje `int` na entidade).
* `expense` vem sempre, sem `expand` (o `"expense"` que o código envia não está na lista e é ignorado). O único
  `expand` é `classifications`, que passa a ser a filha `TotvsDuplicatesClassifications` (+ `classificationType` +
  `classificationCode`). `expense` tem chave `expenseCode` + `costCenterCode`.
* Volume no teste: 662 parcelas alteradas em 4 dias.
* **Testes na implementação (23/10/2026):** 8.835 parcelas no total (todas com alteração desde dez/2025); 680 em 4
  dias, nenhuma chave repetida com a chave por parcela (5 duplicatas tinham mais de uma parcela na janela, e o código
  antigo gravaria só uma); `status` vem pela letra (`"N"`) e `inclusionType` pelo número; `userInclusionName` sem
  tamanho no swagger (até 40 no teste, coluna de 60); os tamanhos de `classificationTypeDescription` e
  `classificationDescription` no swagger (4) são erro de cópia, e as colunas ficaram com 60.

**Comissões pagas**

* **O código de hoje nunca funcionou.** A rota exige `filter.closingCompanyCode` e o código envia só
  `filter.change`: a API responde **400** (`Invalid ClosingCompanyCode value 0`, testado).
* **O DTO não corresponde à API.** A rota devolve o **fechamento de comissão por comissionado**
  (`ClosingCommissionDataOutModel`: `closingBranchCode`, `closingDate`, `closingCode`, `commissionedCpfCnpj`,
  `commissionedName`, `commissionStatus` e os valores de faturamento, recebimento, IRRF, ISSQN, desconto, líquido,
  saldo...). Os campos do DTO de hoje (`commissionCode`, `representativeCode`, `duplicateCode`, `paymentDate`,
  `maxChangeFilterDate`...) não existem na resposta. DTO, entidade, validador e tabela são refeitos pelo schema.
* **Não há filtro de alteração nem `maxChangeFilterDate`.** A janela é pela data do fechamento
  (`startClosingDate`/`endClosingDate`) e a chamada é **uma por empresa** (`closingCompanyCode`), em laço pelas
  lojas de `TotvsBranches`. Sem data de alteração, o "novo ou alterado" é decidido por hash do registro (cache de
  hash, §3.3 do padrão de cache), e a janela relê os últimos meses a cada execução.
* `expand` `accountMovements` (movimentos da conta do comissionado) vira a filha
  `TotvsCommissionsPaidAccountMovements` (sem chave na API: a posição na lista é a chave).
* Chave: `doc_company` + `closingBranchCode` + `closingCode` + `commissionedCpfCnpj`.
* **Não há dados hoje:** nenhuma das 25 empresas tem fechamento de comissão entre 01/01/2024 e 23/10/2026
  (testado). **Decidido: implementar pelo schema mesmo assim.** Como não há resposta real para conferir, tipos e
  tamanhos vêm do swagger (como `observationNFE` na decisão 13), e o primeiro fechamento real vira o teste.

### 5.2.8 Relacionamentos 1:1: colunas no pai ou tabela própria

Revisão de **todos** os objetos 1:1 (propriedade objeto, não lista) das respostas das rotas usadas, conferidos nos
swaggers. Produtos, vendedores, pedidos de compra, duplicatas, comissões e filiais não têm nenhum.

| Rota | Objeto 1:1 | Campos | Hoje no código | Proposta |
|---|---|---|---|---|
| Notas | `eletronic` | 7 | Colunas da nota (parte) | Colunas de `TotvsInvoices` |
| Notas | `ecf` | 2 | Não gravado (`coupon` falta) | Colunas de `TotvsInvoices` |
| Notas | `sat` | 6 | Colunas (parte; `cnfNumber` falta) | Colunas de `TotvsInvoices` |
| Notas | `shippingCompany` | 29 | Colunas (parte; redespacho falta) | Colunas de `TotvsInvoices` |
| Notas | `person` | 16 | Colunas (parte) | Colunas de `TotvsInvoices` |
| Notas | `productionOrder` | 4 | Não gravado | Colunas de `TotvsInvoices` (zeros viram nulos, §5.2.1) |
| Notas | `items[].rateDifferential` | 7 | Colunas do item | Colunas de `TotvsInvoicesItems` |
| Notas | `payments[].cardInformation` | 7 | Colunas do pagamento (`cardInformationNsu`...) | Colunas de `TotvsInvoicesPayments` |
| Notas | `payments[].digitalWalletInformation` | 3 | Colunas do pagamento (`digitalWalletInformationNsu`...) | Colunas de `TotvsInvoicesPayments` |
| Pedidos de venda | `shippingAddress` | 15 | Colunas do pedido (`shipping*`) | Colunas de `TotvsSaleOrders` |
| Pedidos de venda | `invoices[].electronic` | 4 | Colunas da nota do pedido (parte; `receipt` falta) | Colunas de `TotvsSaleOrdersInvoices` |
| PF e PJ | `shippingCompany` | 6 | Colunas da pessoa | Colunas de `TotvsPeople` / `TotvsLegalEntities` |
| PF e PJ | `preferences` | 13 | Colunas da pessoa | Colunas de `TotvsPeople` / `TotvsLegalEntities` |
| PF | `individualAdditionals` | 60 | Não lido | Colunas de `TotvsPeople` (decidido) |

**Conclusão: todos ficam como colunas do pai; nenhuma tabela nova.** Motivos:

* é um para um com a mesma chave e o mesmo ciclo de vida do pai: chegam juntos no mesmo registro da API;
* tabela própria exigiria mais um par `untreated`/`totvs`, mais uma procedure, mais uma condição de exclusão e um
  `join` em toda consulta, sem nenhum ganho de modelagem;
* quando o grupo não vem (sem `expand`, ou o objeto vem nulo), as colunas ficam nulas, e as entidades já passam a
  aceitar nulos (§3.2);
* o maior caso, `individualAdditionals` com 60 campos, fica bem abaixo do limite de colunas do SQL Server (1.024)
  mesmo somado aos ~60 campos da pessoa.

Uma tabela própria só se justificaria para um objeto com chave própria ou que mudasse sem o pai mudar, e nenhum
dos 14 é assim.

**Prefixo obrigatório: o nome do objeto na API.** Ao achatar, há colisões de nomes, conferidas nos swaggers:

| Rota | Colisão |
|---|---|
| Notas | `ecf.serialMachine` × `sat.serialMachine` |
| Notas | `person.personCode`/`personName` × `personCode`/`personName` da nota |
| Notas | `person` × `shippingCompany`: `cep`, `neighborhood`, `postOfficeBox`, `stateAbbreviation` |
| Notas | `productionOrder.branchCode` × `branchCode` da nota |
| Notas | `cardInformation.nsu` × `digitalWalletInformation.nsu` |
| Pedidos de venda | `invoices[].electronic.accessKey` × `invoices[].accessKey` |
| PF | `individualAdditionals.homeTown` × `homeTown` da pessoa |

Hoje o código resolve só parte delas (`cardInformationNsu`, `shipping*` no pedido) e, nas notas, simplesmente não
grava os campos que colidiriam. Para não haver regra caso a caso, **toda coluna vinda de objeto 1:1 leva o nome do
objeto como prefixo**, em camelCase, como já fazem `cardInformationNsu` e `shippingAddressType`: `eletronicAccessKey`,
`ecfSerialMachine`, `satSerialMachine`, `personCep`, `shippingCompanyCep`, `productionOrderBranchCode`,
`preferencesPaymentConditionCode`, `individualAdditionalsHomeTown`... As tabelas são novas (§5.4), então não há
coluna existente para preservar. Os campos que faltam hoje (marcados como "parte" ou "falta" na tabela acima) entram
completos.

### 5.3 Procedures de sincronização

Geradas por `dbo.sp_Create_Merge_Procedure` no schema `totvs`, uma por tabela, na ordem pai → filhas (hoje as
merges já chamam `totvs`, mas as procedures não existem). Nas filhas, `@delete_condition` apaga o que saiu da API quando o pai veio no lote, pelo
mesmo `integrated_or_updated_at` (igual a `MovideskServicesCategories`):

```sql
declare @delete_by_branch nvarchar(max) = 'exists (select 1 from [totvs].TotvsBranches p where p.doc_company = target.doc_company and p.code = target.branchCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranches', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranchesAddresses', @execute = 1, @delete_condition = @delete_by_branch;
```

(Exemplo com filiais, o primeiro recurso. Nas notas a condição usa a chave da §5.2.1:
`p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.invoiceSequence = target.invoiceSequence and p.invoiceDate = target.invoiceDate`.)

Hoje não há como remover uma filha excluída na Totvs (um telefone apagado continua no banco para sempre).

### 5.4 Scripts DbUp (`Infrastructure.Totvs/Scripts/SQLServer`)

**Decidido:** os scripts `001` e `002` atuais **não rodaram** em nenhum banco, então a numeração é refeita no
formato do Movidesk. O DbUp registra o **nome** do script executado e não roda de novo um script alterado; como a
implementação vai ser um service por vez (§9), cada service traz os **seus** scripts, com os números seguintes, e
nenhum script já executado é editado depois. Os jobs também vão por service, no script de jobs do recurso.

| Script | Conteúdo | Entra com |
|---|---|---|
| `Script001_Create_Totvs_Schema_And_Parameters_Table.sql` | Schema `totvs` + `totvs.TotvsParameters` | Base (fase 1) |
| `Script002_Insert_Totvs_Values_Into_Apps_Table.sql` | App `130 - Totvs` (conteúdo do `Script001` atual) | Base |
| `Script003_Insert_Values_Into_Totvs_Parameters_Table.sql` | A holding Newbloomers (`doc_company = '44065634000106'`) com as credenciais **novas**; fora do git (`.gitignore`), a partir do modelo `.sql.example`, que o DbUp não executa | Base |
| `Script004_Create_Totvs_Branches_Tables.sql` | `TotvsBranches`, `TotvsBranchesAddresses` | Filiais |
| `Script005_Create_Totvs_Branches_Sync_Procedures.sql` | | Filiais |
| `Script006_Insert_Totvs_Branches_Jobs_Into_Jobs_Table.sql` | Job `TotvsGetBranchesList` + procedures (§7) | Filiais |
| `Script007`…`009` | Mesmo trio (tabelas, procedures, jobs) | Próximo service |
| … | Um trio por service, na ordem da §9 | |

O `Script002_Insert_Totvs_Values_Into_Jobs_Table.sql` atual (só comentários) sai.

O `PostgreSQL/migrator.csx` chama `MySqlDatabase` (o mesmo erro do Movidesk e do AfterSale). Fica anotado, fora do
escopo.

### 5.5 Log de requisições (`totvs.TotvsRequests`)

O Movidesk e o AfterSale só gravam em `*Requests` as chamadas **sob demanda** (com `X-User-Name`). O Totvs não
tinha rotas sob demanda; elas entram na fase 3 (§9), e com elas a tabela `totvs.TotvsRequests` (`Script028`), igual
à `MovideskRequests`: só inserção, direto no schema `totvs`, com `request_body` sempre preenchido porque as buscas da
Totvs são `POST` com filtro. A rota gravada nunca leva o token.

---

## 6. Infrastructure.Totvs e host

### 6.1 `DependencyInjection.cs`

* Namespace `Infrastructure.Totvs.DependencyInjection`.
* Um `HttpClient` e um `IApiClient` com a chave `"TotvsAPI"` (`Totvs:BaseUrl`, timeout de 20 min), como o
  `"MovideskAPI"`. Hoje há dois `HttpClient` (`"TotvsAPI"` e `"TotvsHmg"`), mas só o `"TotvsHmg"` tem
  `IApiClient`, e todos os services injetam `[FromKeyedServices("TotvsHmg")]`. Como `BaseUrl` e `BaseUrlHmg`
  são a mesma URL no `appsettings.json`, a produção roda pelo cliente "Hmg". Homologação passa a ser só outro
  `BaseUrl` no `appsettings.Development.json`.
* `AddValidatorsFromAssemblyContaining` é chamado **9 vezes** sobre o mesmo assembly, o que registra cada
  validador 9 vezes. Fica uma chamada, como no Movidesk.
* Registrar `ITotvsService`, os 7 services, os repositórios e o `ITotvsCommandHandler`.

### 6.2 Repositórios

* `ISharedRepository` no lugar de `IIntegrationsSharedRepository` (não compila hoje).
* `BulkInsertIntoTableRaw(table, "untreated")` (a sobrecarga sem schema está marcada `[Obsolete]`) e
  `CallDbProcMerge("totvs", ...)`, que já é o schema usado hoje.
* Lista `(DataTable table, IList rows)` percorrida em dois laços (bulk de todas, depois as merges na ordem), como
  em `MovideskServicesRepository`. Hoje cada repositório repete três blocos de 10 a 20 linhas.
* **Todas as filhas com `SelectMany`.** Em `People`, `LegalEntities`, `Invoices`, `SaleOrders`, `PurchaseOrders` e
  `Sellers` o código usa `registros.Select(y => y.filhas).Where(x => x.Count > 0).Select(z => z.FirstOrDefault())`:
  **só a primeira filha de cada pai é gravada**. Uma nota com 30 itens grava 1 item; dos impostos, produtos e
  códigos de barras, só os do primeiro item. Produtos, filiais e contas a pagar já usam `SelectMany`.
* `GetExisting*` passa a devolver a entidade (§4.4), como `GetTicketsByIds` no Movidesk.

### 6.3 Host Hangfire

| Arquivo | Alteração |
|---|---|
| `Hangfire.csproj` | Referenciar `Application.Totvs` e `Infrastructure.Totvs` |
| `Extensions/ServicesExtensions.cs` | Descomentar com `using Infrastructure.Totvs.DependencyInjection;` + `AddScopedTotvsServices` |
| `appsettings.json` | Manter `Totvs:BaseUrl`; tirar `BaseUrlHmg` (ou levar para o `appsettings.Development.json`) |
| `Controllers/Totvs/TotvsController.cs` | Reescrever (abaixo) |

### 6.4 `TotvsController`

Rota base `TotvsJobs/Totvs` (mantida), `[Authorize]` **comentado** como no Movidesk e no AfterSale (decidido; hoje
está ativo), `[Tags]` por recurso e resposta `Ok("Records integrated successfully.")`.

**O verbo segue a documentação oficial (decidido)**, como os nomes dos métodos (§2.3): `POST` para as buscas
(`.../search` é `POST` na API) e `GET` só para as filiais, porque o `branchesList` é `GET` na API.

| Verbo | Rota | Tag | Método do service |
|---|---|---|---|
| POST | `Customers` | `TotvsPeople` | `PostSearchCustomers` |
| POST | `LegalEntities` | `TotvsPeople` | `PostSearchLegalEntity` |
| GET | `Branches` | `TotvsPeople` | `GetBranchesList` |
| POST | `Products` | `TotvsProducts` | `PostSearchReferences` |
| POST | `Invoices` | `TotvsInvoices` | `PostSearchInvoices` |
| POST | `PurchaseOrders` | `TotvsPurchaseOrders` | `PostSearchPurchaseOrders` |
| POST | `SalesOrders` | `TotvsSalesOrders` | `PostSearchSalesOrders` |
| POST | `Sellers` | `TotvsSellers` | `PostSearchSellers` (hoje a rota é `TotvsSelers`) |
| POST | `Duplicates` | `TotvsAccountsPayable` | `PostSearchDuplicates` |
| POST | `ComissionsPaid` | `TotvsAccountsPayable` | `PostSearchComissionsPaid` |

As rotas comentadas `TotvsOperations`, `TotvsPrices`, `TotvsCosts` e `TotvsBalances` chamam métodos que não
existem (`SearchOperations`, `PostSearchPrices`...) e o `IGeneralService`, que não é do módulo. Elas saem; preços,
custos e saldos voltam na fase 3 como jobs (§9), e operações é descartada (não existe em nenhum swagger da Totvs,
decisão 30).

**Rotas sob demanda (fase 3)**, no desenho do `GET Tickets/{docCompany}/{id}` do Movidesk: cabeçalho `X-User-Name`
obrigatório (até 100), `docCompany` por `^\d{14}$`, resposta `200` gravado, `404` não existe na Totvs e `422` empresa
não cadastrada ou resposta inválida. Como a API da Totvs não tem rota por chave, cada uma usa a busca `POST ...search`
com a lista de chave (`personCodeList`, `branchCodeList` + `orderCodeList`...) e `pageSize` 1:

| Verbo | Rota | Busca na API | Método do service |
|---|---|---|---|
| GET | `Customers/{docCompany}/{code}` | `individuals/search` com `personCodeList` | `GetCustomerByCode` |
| GET | `LegalEntities/{docCompany}/{code}` | `legal-entities/search` com `personCodeList` | `GetLegalEntityByCode` |
| GET | `SalesOrders/{docCompany}/{branchCode}/{orderCode}` | `orders/search` com `branchCodeList` + `orderCodeList` | `GetSalesOrderByKey` |
| GET | `Invoices/{docCompany}/{branchCode}/{invoiceSequence}` | `invoices/search` com `branchCodeList` + `invoiceSequenceList` | `GetInvoiceByKey` |
| GET | `Products/{docCompany}/{code}` | `products/search` com `productCodeList` | `GetProductByCode` |

---

## 7. Auditoria (jobs)

Faixa já reservada no `Script002`: integrações 13000–13499, procedures 13500–13999 (app 130, como o Movidesk usa
30 / 3000 / 3500). Os ids comentados no `EnumJob` (1900–1909) saem.

Intervalos **decididos**: 3 min para documentos, clientes PF e pessoas jurídicas, e 120 min para os demais
cadastros (filiais, vendedores, produtos). As procedures de cada job usam o mesmo
intervalo do job, como no Movidesk).

| Id | Job | Tipo | Intervalo | Hoje |
|---|---|---|---|---|
| 13000 | `TotvsPostSearchReferences` | cadastro | 120 min | `TotvsProducts = 1900` comentado |
| 13001 | `TotvsPostSearchLegalEntity` | cadastro | 3 min | `1901` comentado |
| 13002 | `TotvsPostSearchCustomers` | cadastro | 3 min | `1902` comentado |
| 13003 | `TotvsPostSearchInvoices` | documento | 3 min | `1903` comentado |
| 13004 | `TotvsPostSearchPurchaseOrders` | documento | 3 min | `1904` comentado |
| 13005 | `TotvsPostSearchSalesOrders` | documento | 3 min | `1905` comentado |
| 13006 | `TotvsPostSearchSellers` | cadastro | 120 min | `1906` comentado; o service usa o job de **notas fiscais** |
| 13007 | `TotvsGetBranchesList` | cadastro | 120 min | `1907` comentado |
| 13008 | `TotvsPostSearchDuplicates` | documento | 3 min | `1908` comentado |
| 13009 | `TotvsPostSearchComissionsPaid` | documento | 3 min | `1909` comentado |
| 13010 | `TotvsGetCustomerByCode` | sob demanda | 0 | fase 3 |
| 13011 | `TotvsGetLegalEntityByCode` | sob demanda | 0 | fase 3 |
| 13012 | `TotvsGetSalesOrderByKey` | sob demanda | 0 | fase 3 |
| 13013 | `TotvsGetInvoiceByKey` | sob demanda | 0 | fase 3 |
| 13014 | `TotvsGetProductByCode` | sob demanda | 0 | fase 3 |
| 13015 | `TotvsPostSearchPrices` | cadastro | 3 min | fase 3 |
| 13016 | `TotvsPostSearchCosts` | cadastro | 3 min | fase 3 |
| 13017 | `TotvsPostSearchBalances` | documento | 3 min | fase 3 |
| 13500+ | `sp_Totvs<Tabela>_Sync` | procedure | o do job de origem | não existem (`IdType = 1`, `IdParent` = job de origem) |

Os jobs sob demanda têm `IntervalTimeInMinutes = 0` (não são recorrentes), como no Movidesk.

O nome do job é `Totvs` + o nome do método (mantido, §2.3), como no Movidesk (`MovideskGetTickets`). Os ids
ficam reservados agora, mas cada job só entra no banco junto com o seu service (§5.4).

`EnumJob` passa a ser o de `Domain.Totvs.Enums` (os services hoje fazem `using Domain.Shared.Enums`, que é o motivo
dos erros de compilação).

---

## 8. Erros de comportamento encontrados (corrigir junto)

| # | Onde | Problema |
|---|---|---|
| 1 | 6 repositórios (§6.2) | `FirstOrDefault()` nas filhas: só a primeira filha de cada pai é gravada (itens de nota, telefones, endereços, produtos de pedido...) |
| 2 | `PersonService.PostSearchLegalEntity` | `expand` copiado do produto (`details,composition,barCodes,webData...`): endereços, telefones, e-mails, contatos e o resto das filhas da pessoa jurídica **nunca vêm** da API |
| 3 | `PersonRepository.BulkInsertIntoTableLegalEntitiesRaw` | `LegalEntitiesStatistics` recebe bulk, mas a merge nunca é chamada: os dados ficam em `untreated` |
| 4 | `SellerService.JobName` | Usa `EnumJob.TotvsFiscal`: execuções de vendedores ficam registradas como notas fiscais |
| 5 | `GetExisting*` + `DateTime.Parse(..., en-US)` | Data do banco lida como `string` na cultura do servidor e relida em `en-US` (§4.4): comparação errada ou exceção |
| 6 | `ProductService` | Cache estático nunca atualizado: SKUs regravados em toda execução (§4.4) |
| 7 | `PersonService.PostSearchCustomers`/`PostSearchLegalEntity` | Janela desde `2000-01-01` a cada execução: relê a base inteira de pessoas |
| 8 | `PersonService.GetBranchesList` | Não registra nada em `auditing.Records`; chama a rota sem `StartChangeDate`/`EndChangeDate`, então `maxChangeFilterDate` vem nulo e todas as filiais são regravadas sempre (§5.2.2, testado) |
| 9 | Todos | Resposta da API não tratada; token nulo ou erro HTTP vira `NullReferenceException` (§4.2) |
| 10 | Todos | Log de execução não fecha quando há exceção (sem `finally`) |
| 11 | Entidades | Datas nulas gravadas como `1990-01-01` (ex.: `disableDate` de nota não cancelada) |
| 12 | Filhas de notas e pedidos | Sem `branchCode`: mistura filhas de documentos de filiais diferentes (§3.2) |
| 13 | Merges | Filhas excluídas na Totvs nunca saem do banco (§5.3) |
| 14 | Código | Credenciais de produção no código e no histórico do git (§5.1) |
| 15 | `DependencyInjection` | Validadores registrados 9 vezes; produção usando o cliente "Hmg" (§6.1) |
| 16 | `TotvsBaseSyncService` e os 6 métodos com login | Credenciais do código inválidas: o login responde 400 (§5.1) |
| 17 | `FiscalService.ExpandFields` | `productionOrder` e `referencedTaxInvoice` não são nomes de `expand` aceitos: ordem de produção e notas referenciadas nunca vêm (§5.2.1, testado) |
| 18 | `PersonService.GetBranchesList` | Sem `Expand=addresses`: endereços das filiais nunca vêm (§5.2.2, testado) |
| 19 | `PostSearchCustomers` e `PostSearchLegalEntity` | Sem `option.branchStaticDataList`: estatísticas nunca vêm (§5.2.5, testado) |
| 20 | `SellerService` | `branchCodeList = [1]` filtra vendedores e filhas: 86 de 135 vendedores e só a `branchInformation` da loja 1 (§5.2.3, testado) |
| 21 | `ProductService` | `expand` sem `manufacturers`, `referenceCodeSequences`, `measurementTableOmni`, `branchesProductBlocked` e `conservationInstructions`: 7 tabelas do repositório ficam sempre vazias (§5.2.4, testado) |
| 22 | `AccountsPayableService.PostSearchDuplicates` | Chave e cache só por `duplicateCode`: só uma parcela de cada duplicata é gravada; `duplicateCode` é `int64` na API (§5.2.7, testado) |
| 23 | `AccountsPayableService.PostSearchComissionsPaid` | Sem `closingCompanyCode` a API responde 400 sempre; o DTO não corresponde à resposta da rota (§5.2.7, testado) |
| 24 | `PurchaseOrderService` | `expand` sem `requests`: solicitações de compra do pedido nunca vêm (§5.2.6) |
| 25 | Entidades de PF | `ctps`/`ctpsSerial` (`int64` na API) e `monthlyIncome` (`double` na API) gravados como `int` (§5.2.5) |

---

## 9. Plano em fases

**Decidido:** a implementação é feita **um service de cada vez**. A fase 1 deixa a base pronta; a fase 2 se repete
para cada service, que só é ligado no host e no controller quando está pronto.

**Fase 1 implementada (23/10/2026)** (commit `f615deb`, 26/10/2026):

* `.csproj` como na §2.1 (`Caching.Memory` saiu do Domain e entrou na Application; `Microsoft.Extensions.Http` saiu
  da Infrastructure, que passou a referenciar o Domain);
* o código antigo que não compila e será reescrito na fase 2 ficou **fora do build** (`<Compile Remove>`, com
  comentário no `.csproj`): os 8 arquivos de `Application.Totvs/Services` (os 7 services e a `TotvsBaseSyncService`)
  e os 7 repositórios de `Infrastructure.Totvs/Repositorys`. Cada um volta ao build quando o seu service for
  reescrito. DTOs, entidades, validadores, as interfaces antigas e os command handlers antigos continuam
  compilando;
* novos: `ITotvsService`/`TotvsService` (`GetBatchTime`, `GetCompanys` com cache `Totvs:Parameters`, `GetToken` com
  cache `Totvs:Token:<doc_company>` e `RemoveToken`), `Entities/Parameters/Company`, `ITotvsRepository`/`TotvsRepository`,
  `ITotvsCommandHandler`/`TotvsCommandHandler` (`CreateGetCompanysQuery`);
* `Entities/Token.cs` foi para `Dtos/Reads/Authorization/Token.cs`, e `Entities/Accounts Payable/` virou
  `Entities/AccountsPayable/`; `EnumJob` ficou só com `None` e o comentário novo (os jobs entram com cada service);
* `DependencyInjection` em `Infrastructure.Totvs.DependencyInjection`, com o cliente `"TotvsAPI"`, os registros acima
  e uma chamada só a `AddValidatorsFromAssemblyContaining`;
* scripts: `Script001` (schema + `TotvsParameters`), `Script002` (app 130) e o modelo do `Script003`; os dois scripts
  antigos saíram;
* host: referências no `Hangfire.csproj`, `AddScopedTotvsServices` ligado, `BaseUrlHmg` removido do
  `appsettings.json` e `TotvsController` só com a rota base (`TotvsJobs/Totvs`, `[Authorize]` comentado);
* `dotnet build` do host Hangfire: **0 erros**. Os avisos que sobram são de nulidade nos DTOs antigos, que mudam na
  fase 2 (§3.2). O login e a leitura de `TotvsParameters` ainda não rodaram contra um banco.

**Fase 2, service 1: `GetBranchesList` implementado (23/10/2026)** (commit `f615deb`, 26/10/2026):

* `ITotvsPeopleService`/`TotvsPeopleService.GetBranchesList`, com o fluxo da §4.2 escrito no próprio método: log,
  empresas e token do `ITotvsService`, janela por `MAX(maxChangeFilterDate) - 1 h` (banco vazio: desde 2000),
  `GET person/v2/branchesList` com `StartChangeDate`, `EndChangeDate`, `Expand=addresses`, `Order=code` e
  `PageSize=1000`, uma nova tentativa em 401 (token renovado) e 429 (`Retry-After`), cache de estado
  `Totvs:Branches:<doc_company>`, validação, gravação e `auditing.Records` com o JSON original de cada empresa, e
  `finally` no log. Uma leitura com erro em qualquer página não grava nada daquela empresa, para a janela pelo
  `MAX` não pular páginas;
* DTOs `Dtos/Reads/People/Branches` (`Branch`, `Address`) e o genérico `Dtos/Reads/Base/Page<T>`; entidades
  `Entities/Reads/People/Branches` com `doc_company`, `integrated_or_updated_at` e anuláveis; validadores
  `Validations/People/Branches` com os tamanhos do swagger. O `Root` antigo saiu, e `IPersonRepository` (que
  apontava para as classes movidas) ficou fora do build até as pessoas serem reescritas;
* `ITotvsPeopleRepository`/`TotvsPeopleRepository` (última alteração, filiais por código em lotes de 1000, bulk em
  `untreated` e merges em `totvs`) e as queries no `TotvsCommandHandler`;
* scripts `Script004` (tabelas, PK e FK), `Script005` (procedures com exclusão dos endereços removidos) e
  `Script006` (job `13007 TotvsGetBranchesList` e procedures `13500`/`13501`, 120 min); `EnumJob.TotvsGetBranchesList`;
* rota `GET TotvsJobs/Totvs/Branches` (tag `TotvsPeople`) e registros no DI;
* conferido com um console descartável contra a API real (sem banco): token, 25 empresas numa página, 0 mensagens de
  validação, 25 endereços, nenhuma empresa sem `maxChangeFilterDate`, nenhuma chave de endereço repetida, e as duas
  `DataTable` montadas com as colunas do `Script004`;
* **não verificado:** os scripts e as procedures não rodaram em banco, e o método não rodou pelo host (bulk, merges,
  `auditing` e cache dependem do banco).

**Fase 2, service 2: `PostSearchSellers` implementado (23/10/2026)** (commit `f615deb`, 26/10/2026):

* `ITotvsSellersService`/`TotvsSellersService.PostSearchSellers`, no mesmo desenho do `GetBranchesList`:
  `POST seller/v2/search` com `branchCodeList` = **todas as lojas de `TotvsBranches`** (sem lojas, a empresa é
  pulada com mensagem: o `GetBranchesList` tem de rodar antes), janela por `MAX(maxChangeFilterDate) - 1 h` (banco
  vazio: desde 2000), datas sem fuso, `pageSize` 500, sem `expand` (a rota não tem), 401/429 com uma nova tentativa,
  cache `Totvs:Sellers:<doc_company>`, validação, gravação, `auditing.Records` e `finally`;
* `ITotvsService.GetBranchCodes` + `ITotvsRepository.GetBranchCodes` + `CreateGetBranchCodesQuery`, sem cache, para
  os próximos services usarem as lojas do mesmo jeito;
* DTOs `Dtos/Reads/Sellers`, entidades `Entities/Reads/Sellers` (com `doc_company`, horário do lote, anuláveis e
  filhas sem chave repetida), validadores `Validations/Sellers`, `ITotvsSellersRepository`/`TotvsSellersRepository`;
* scripts `Script007` (tabelas, PK, FK), `Script008` (procedures com exclusão das filhas que saíram) e `Script009`
  (job `13006 TotvsPostSearchSellers` e procedures `13502` a `13504`, 120 min); `EnumJob.TotvsPostSearchSellers`;
  rota `POST TotvsJobs/Totvs/Sellers` (tag `TotvsSellers`);
* o código antigo de vendedores (service, repositório, command handler, interfaces, DTOs, entidades e validadores)
  foi **apagado**, e as duas linhas `<Compile Remove>` dele saíram dos `.csproj`;
* conferido com o console descartável contra a API real (sem banco): 135 vendedores, 0 mensagens de validação,
  1.567 informações por loja, nenhuma loja fora da lista, e as três `DataTable` com as colunas do `Script007`;
* **não verificado:** scripts, procedures e execução pelo host, como no `GetBranchesList`.

**Fase 2, service 3: `PostSearchReferences` implementado (23/10/2026)** (commit `f615deb`, 26/10/2026):

* `ITotvsProductsService`/`TotvsProductsService.PostSearchReferences`: para cada matriz (`1` e `101`, mapa no
  service com o setor da marca), `option.branchInfoCode` e `filter.branchInfo.branchCode` = a matriz,
  `filter.classifications` = classificação 6000 com o setor da marca (`2` Misha, `4` Open Era), `change.branchInfoCodeList` = as
  lojas da marca (`ITotvsService.GetBrandBranchCodes`: mesma raiz de CNPJ da matriz em `TotvsBranches`), `expand`
  completo com `measurementTableOmniType = 2`, `pageSize` 200, janela por `MAX(maxChangeFilterDate)` da matriz - 1 h
  (banco vazio: desde 2000) lida em fatias de 30 dias na carga inicial e de 7 dias depois, cada fatia gravada com o
  seu horário de lote, cache
  `Totvs:Skus:<doc_company>:<matriz>`, validação, `auditing.Records` (chave `matriz|referenceId`) e `finally`;
* 21 DTOs em `Dtos/Reads/Products`, 22 entidades em `Entities/Reads/Products` (uma por tabela, com `doc_company`,
  horário do lote, `branchInfoCode` e anuláveis), 19 validadores em `Validations/Products`;
  `ITotvsProductsRepository`/`TotvsProductsRepository` com as 22 tabelas na ordem dos merges;
* scripts `Script010` (22 pares de tabelas, PKs, FKs com `on delete cascade` a partir das cores), `Script011` (22
  procedures: filhas da referência, cores e produtos apagados pela referência; filhas do produto pelo produto) e
  `Script012` (job `13000 TotvsPostSearchReferences` e procedures `13505` a `13526`, 120 min);
  `EnumJob.TotvsPostSearchReferences`; rota `POST TotvsJobs/Totvs/Products` (tag `TotvsProducts`);
* o código antigo de produtos foi **apagado** (service, repositório, command handler, interfaces, DTOs, entidades e
  validadores), e as linhas `<Compile Remove>` dele saíram dos `.csproj`;
* filiais e vendedores ganharam a mesma nova tentativa em 502/503/504;
* conferido com o console descartável (sem banco): chamada real de 1 dia nas duas matrizes (200, 6 s e 11 s) e 1.028
  referências (amostras das duas marcas): 0 mensagens de validação, nenhuma chave repetida nas 22 tabelas, e as
  colunas das 22 `DataTable` iguais às do `Script010`;
* **não verificado:** scripts, procedures (incluindo as FKs em cascata) e execução pelo host; a carga inicial de 10
  meses não foi rodada (pelo volume medido, deve levar horas na primeira execução).

**Fase 2, service 4: `PostSearchCustomers` e `PostSearchLegalEntity` implementados (23/10/2026)** (commit `f615deb`, 26/10/2026):

* os dois métodos no `TotvsPeopleService` (junto do `GetBranchesList`), no mesmo desenho dos anteriores: `change`
  com todos os `in*`, `option.branchStaticDataList` = todas as lojas de `TotvsBranches` (sem ele as estatísticas
  não vêm), `expand` da documentação (o da PF inclui `preferences`, que vem mesmo fora da lista), `pageSize` 500,
  janela por `MAX(maxChangeFilterDate) - 1 h` (banco vazio: desde 2000) em fatias de 30 dias, nova tentativa em
  401/429/502/503/504, caches `Totvs:Customers:<doc_company>` e `Totvs:LegalEntities:<doc_company>`, validação,
  `auditing.Records` e `finally`;
* DTOs: as filhas e os objetos 1:1 comuns às duas pessoas em `Dtos/Reads/People/Persons` (18 classes) e as raízes
  em `Dtos/Reads/People/Customers` e `LegalEntities`. Entidades: as filhas comuns em `Entities/Reads/People/Persons`
  (16 classes, com `personCode`; a mesma classe grava em `TotvsPeople*` e `TotvsLegalEntities*`) e as raízes com os
  objetos 1:1 achatados com prefixo (`shippingCompany*`, `preferences*`, `individualAdditionals*`, §5.2.8). Os
  tamanhos são os do swagger. Validadores em `Validations/People/Persons`, `Customers` e `LegalEntities`;
* `ITotvsPeopleRepository`/`TotvsPeopleRepository` ganharam última alteração, pessoas por código e bulk + merges das
  13 tabelas da PF e das 16 da PJ;
* scripts `Script013` (29 pares de tabelas, PKs, FKs), `Script014` (29 procedures, filhas apagadas pela pessoa) e
  `Script015` (jobs `13001 TotvsPostSearchLegalEntity` e `13002 TotvsPostSearchCustomers`, 3 min, e procedures
  `13527` a `13555`); `EnumJob`; rotas `POST TotvsJobs/Totvs/Customers` e `POST TotvsJobs/Totvs/LegalEntities`;
* o código antigo de pessoas (service, repositório, command handler, interfaces, DTOs, entidades e validadores) foi
  **apagado**, e as linhas `<Compile Remove>` dele saíram dos `.csproj`;
* conferido com o console descartável (sem banco): chamadas reais com o corpo do service (PF 6 h: 113; PJ 30 dias:
  29) e a amostra (500 PF e 4 PJ): 0 mensagens de validação, nenhuma chave repetida nas 29 tabelas, e as colunas
  das 29 `DataTable` iguais às do `Script013`;
* **não verificado:** scripts, procedures e execução pelo host; a carga inicial das 188 mil PF não foi rodada (pelo
  tempo medido, deve levar algumas horas na primeira execução).

**Fase 2, service 5: `PostSearchPurchaseOrders` e `PostSearchSalesOrders` implementados (23/10/2026)** (commit `f615deb`, 26/10/2026):

* `TotvsPurchaseOrdersService` e `TotvsSalesOrdersService`, no mesmo desenho dos anteriores: `branchCodeList`
  (obrigatório) = todas as lojas de `TotvsBranches`, `expand` da documentação (compra: `items,requests`; venda: os 10
  grupos, que já batiam no código antigo), `pageSize` 100 (compra) e 500 (venda), janela por
  `MAX(maxChangeFilterDate) - 1 h` (banco vazio: 5 meses na compra, desde 01/01/2025 na venda) em fatias de 30 dias,
  nova tentativa em 401/429/502/503/504, cache `Totvs:PurchaseOrders` / `Totvs:SalesOrders` por empresa com chave (loja, número), validação,
  `auditing.Records` (chave `loja|número`) e `finally`;
* DTOs, entidades e validadores em `Dtos/Reads`, `Entities/Reads` e `Validations` de `PurchaseOrders` (3 tabelas) e
  `SalesOrders` (11 tabelas), gerados de uma especificação só, como em pessoas; repositórios
  `TotvsPurchaseOrdersRepository` e `TotvsSalesOrdersRepository`; queries por (loja, número) com `VALUES` em lotes de 1000;
* scripts `Script016`–`018` (compra: tabelas, procedures, job `13004 TotvsPostSearchPurchaseOrders` e procedures
  `13556` a `13558`) e `Script019`–`021` (venda: tabelas, procedures com cascata nos itens de lote, job
  `13005 TotvsPostSearchSalesOrders` e procedures `13559` a `13569`), 3 min; `EnumJob`; rotas
  `POST TotvsJobs/Totvs/PurchaseOrders` e `POST TotvsJobs/Totvs/SalesOrders`;
* o código antigo de pedidos de compra e de venda foi **apagado**, e as linhas `<Compile Remove>` dele saíram dos `.csproj`;
* conferido com o console descartável (sem banco): chamadas reais com o corpo dos services (compra 30 dias: 194 em 2
  páginas; venda 2 dias: 184) e as amostras: 0 mensagens de validação, nenhuma chave repetida nas 14 tabelas, e as
  colunas das 14 `DataTable` iguais às dos scripts;
* **não verificado:** scripts, procedures e execução pelo host.

**Fase 2, service 6: `PostSearchDuplicates` e `PostSearchComissionsPaid` implementados (23/10/2026)** (commit `f615deb`, 26/10/2026):

* `TotvsAccountsPayableService` com os dois métodos. **Duplicatas**, no mesmo desenho dos anteriores:
  `branchCodeList` = todas as lojas, `expand=classifications` (o `expense` vem sempre), `pageSize` 100, janela por
  `MAX(maxChangeFilterDate) - 1 h` (banco vazio: 10 meses) em fatias de 30 dias, chave por parcela (loja +
  fornecedor + duplicata + parcela, `duplicateCode` como `bigint`), cache `Totvs:Duplicates:<doc_company>`.
  **Comissões**: uma chamada por loja (`closingCompanyCode`), janela dos últimos 10 meses pela data do fechamento
  relida a cada execução (a rota não tem filtro de alteração), `expand=accountMovements`, e "novo ou alterado"
  decidido por **cache de hash** do JSON de cada fechamento (`Totvs:CommissionsPaidHash:<doc_company>`, §3.3 do
  padrão de cache); uma loja com erro não impede as outras. Os dois: nova tentativa em 401/429/502/503/504,
  validação, `auditing.Records` e `finally`;
* DTOs, entidades e validadores em `AccountsPayable` (gerados de uma especificação), `TotvsAccountsPayableRepository`
  e a query de duplicatas por chave com `VALUES` em lotes de 1000;
* scripts `Script022` (5 pares de tabelas, PKs, FKs), `Script023` (5 procedures) e `Script024` (jobs
  `13008 TotvsPostSearchDuplicates` e `13009 TotvsPostSearchComissionsPaid`, 3 min, e procedures `13570` a `13574`);
  `EnumJob`; rotas `POST TotvsJobs/Totvs/Duplicates` e `POST TotvsJobs/Totvs/ComissionsPaid`;
* o código antigo de contas a pagar foi **apagado**, e as linhas `<Compile Remove>` dele saíram dos `.csproj`;
* conferido com o console descartável (sem banco): duplicatas reais de 4 dias (680 em 7 páginas, 686 despesas), 0
  mensagens de validação e nenhuma chave repetida; as 25 chamadas de comissão com o corpo do service responderam 200
  (0 fechamentos, como no teste anterior); entidade e colunas das comissões conferidas com **um registro sintético**
  no formato do swagger, porque não há fechamento real; as colunas das 5 `DataTable` iguais às do `Script022`;
* **não verificado:** scripts, procedures e execução pelo host; as comissões nunca foram lidas com dados reais.

**Fase 2, service 7: `PostSearchInvoices` implementado (23/10/2026)** (commit `f615deb`, 26/10/2026). Com ele a fase 2 está completa:

* `TotvsInvoicesService.PostSearchInvoices` no mesmo desenho dos anteriores: `branchCodeList` = todas as lojas,
  `expand` da documentação, `pageSize` 100, janela por `MAX(maxChangeFilterDate) - 1 h` (banco vazio: desde 01/01/2025) em
  fatias de 30 dias, chave loja + sequência + data, cache `Totvs:Invoices:<doc_company>`, nova tentativa em
  401/429/502/503/504, validação, `auditing.Records` e `finally`. `referencedInvoices` fica **comentado** no
  `expand` por causa do `400` da API; o contorno (reler a página em partes, §5.2.1, com as funções locais `Search` e
  `ReadPage`) continua no método para quando ele for religado;
* DTOs, entidades e validadores em `Invoices` (gerados de uma especificação), com os objetos 1:1 como colunas
  prefixadas (`eletronic`, `ecf`, `sat`, `shippingCompany`, `person`, `productionOrder` na nota; `rateDifferential` no
  item; `cardInformation` e `digitalWalletInformation` no pagamento); `productionOrder` zerado vira `null`;
  `TotvsInvoicesRepository` e a query por chave com `VALUES` (data no estilo 126) em lotes de 1000;
* scripts `Script025` (10 pares de tabelas, PKs, FKs com cascata a partir dos itens), `Script026` (10 procedures; as
  filhas que saíram de uma nota do lote são apagadas) e `Script027` (job `13003 TotvsPostSearchInvoices`, 3 min, e
  procedures `13575` a `13584`); `EnumJob`; rota `POST TotvsJobs/Totvs/Invoices`;
* o código antigo de notas (`FiscalService`, `FiscalRepository`, `FiscalCommandHandler`, DTOs, entidades e
  validadores `Fiscal`) e a `TotvsBaseSyncService` foram **apagados**; não sobrou nenhum `<Compile Remove>` nos `.csproj`;
* conferido com o console descartável rodando o **service de verdade** (API real, repositório e log falsos) em 21 a
  23/10/2026, ainda com `referencedInvoices` ligado: 1.272 notas gravadas, 9 delas sem `referencedInvoices` (incluindo a 7/42572), nenhuma chave repetida nas
  10 tabelas (7.745 itens, 35.560 impostos, 1.765 pagamentos) e as colunas das 10 `DataTable` iguais às do `Script025`;
* **não verificado:** scripts, procedures e execução pelo host.

**Fase 3, parte 1: base sob demanda, `GetCustomerByCode` e `GetLegalEntityByCode` implementados (26/10/2026)**, ainda sem commit:

* escopo da fase 3 decidido (decisões 28 a 31 da §10.1): cinco leituras sob demanda (PF, PJ, pedido de venda, nota e
  produto, §6.4), preços, custos e saldos como jobs de 3 min no padrão da fase 2, e operações descartada;
* base: `Entities/Writes/Requests/Request` → `totvs.TotvsRequests` (`Script028`), `ITotvsRepository.InsertIntoTotvsRequests`,
  `ITotvsService.GetCompany(doc_company)` (filtra o `GetCompanys` em cache) e `ITotvsService.SaveRequest` (igual ao
  Movidesk, com `requestBody`);
* `TotvsPeopleService.GetCustomerByCode` e `GetLegalEntityByCode`: `POST .../search` com `personCodeList = [code]`,
  `pageSize` 1, o mesmo `expand` e `option.branchStaticDataList` da busca por janela, e **`filter.change` desde 2000 com
  as mesmas flags**: sem ele o `maxChangeFilterDate` vem nulo e a pessoa seria regravada na próxima execução do job
  (testado; sem as flags a API responde 400). A pessoa é gravada sempre pelo mesmo `SaveCustomers`/`SaveLegalEntities`
  do job (validação, `auditing.Records`) e atualiza o cache do job se ele estiver carregado; `items` vazio é "não
  existe" (`false`); a chamada vai para `TotvsRequests` em todos os caminhos que chegam à API, inclusive exceção;
* `EnumJob` 13010 e 13011, `Script029` (jobs sob demanda, intervalo 0), rotas `GET TotvsJobs/Totvs/Customers/{docCompany}/{code}`
  e `GET .../LegalEntities/{docCompany}/{code}` com `X-User-Name` e `code:int:min(1)`;
* conferido com o console descartável rodando os métodos de verdade (API real, repositório e log falsos): PF 58509 e
  PJ 58631 gravadas com filhas e `maxChangeFilterDate` preenchido, código inexistente devolve `false` sem gravar,
  empresa não cadastrada devolve `null` sem chamar a API, e cada chamada gera um registro de `TotvsRequests`;
* **não verificado:** `Script028`/`029` em banco e as rotas pelo host.

| Fase | Entrega | Resultado |
|---|---|---|
| **1. Base** | §2 (csproj, nomes de classes, namespaces), `TotvsService` comum (§4.1: horário do lote, parâmetros e token em cache), remoção da `TotvsBaseSyncService`, §5.0–5.1 e `Script001`–`003` (schema, parâmetros com a holding, app), §6.1–6.3 (DI, `ISharedRepository`, host). Os services ainda não migrados ficam fora do DI e do controller | Módulo compila no Shared e sobe no host |
| **2. Um service por vez** | Para o service da vez: o fluxo da §4.2 escrito no próprio método (janela pelo banco, lojas de `TotvsBranches`, `SendAsync`, `finally`), DTOs `Reads` e entidades (§3), validadores (§4.3), cache (§4.4), queries (§4.5), repositório (§6.2), trio de scripts (§5.4), job (§7), rota no controller (§6.4) e as correções de §8 que tocam nele | Recurso lido no padrão, auditável e incremental |
| **3. Sob demanda e valores** (decidida em 26/10/2026) | Leituras sob demanda por chave (§6.4: PF, PJ, pedido de venda, nota e produto, com `X-User-Name`, CNPJ por `^\d{14}$` e `totvs.TotvsRequests`); preços, custos e saldos como jobs de 3 min (`prices`, `costs` e `balances/search`, uma chamada por marca como as referências); operações descartada | Registro único gravado sob demanda e auditado; valores por produto incrementais |
| **4. Docs** | `README.md` do módulo no formato do Movidesk (arquitetura, services, fluxo, scripts, endpoints, jobs, configuração) e pasta `Responsed Jsons` com uma resposta de exemplo por rota (dados fictícios) | — |

---

Ordem sugerida dos services (filiais primeiro, porque as outras buscas leem as filiais de `TotvsBranches`):

| Ordem | Service | Observação |
|---|---|---|
| 1 | `GetBranchesList` | **Implementado** (falta rodar em banco). Pré-requisito dos demais (lojas da holding); janela por `StartChangeDate`/`EndChangeDate` e `Expand=addresses` (§5.2.2) |
| 2 | `PostSearchSellers` | **Implementado** (falta rodar em banco). O mais simples das buscas (2 filhas, sem `expand`); `pageSize` 500 (§5.2.3) |
| 3 | `PostSearchReferences` | **Implementado** (falta rodar em banco). Tira o cache estático; uma chamada por marca (matrizes 1 e 101) e `expand` completo com `measurementTableOmniType` (§5.2.4) |
| 4 | `PostSearchCustomers` e `PostSearchLegalEntity` | **Implementados** (falta rodar em banco). Corrige o `expand` das PJ, as estatísticas e as filhas com `FirstOrDefault` (§5.2.5) |
| 5 | `PostSearchPurchaseOrders` e `PostSearchSalesOrders` | **Implementados** (falta rodar em banco). Compra: `requests` no `expand` (§5.2.6). Venda: o `expand` do código bate com o swagger, e o `pageSize` aceita até 500 (hoje 100) |
| 6 | `PostSearchDuplicates` e `PostSearchComissionsPaid` | **Implementados** (falta rodar em banco). Duplicatas com a chave por parcela; comissões refeitas pelo schema (§5.2.7) |
| 7 | `PostSearchInvoices` | **Implementado** (falta rodar em banco). Chave e `expand` do swagger, com `referencedInvoices` comentado por causa do `400` da API (§5.2.1) |

Ordem da fase 3 (a base sob demanda entra com o primeiro):

| Ordem | Entrega | Observação |
|---|---|---|
| 1 | `TotvsRequests` + `GetCustomerByCode` e `GetLegalEntityByCode` | **Implementados** (falta rodar em banco). `Script028`/`029`, jobs 13010 e 13011 |
| 2 | `GetSalesOrderByKey` e `GetInvoiceByKey` | Reusam `SaveSalesOrders`/`SaveInvoices`; jobs 13012 e 13013 |
| 3 | `GetProductByCode` | `products/search` com `productCodeList`, grava em `TotvsSkus` e filhas; job 13014 |
| 4 | `PostSearchPrices` | `prices/search` com `change.inPrice`, `option.prices` por loja; tabelas `TotvsPrices` e filhas; job 13015 |
| 5 | `PostSearchCosts` | `costs/search` com `change.inCost`; `TotvsCosts`; job 13016 |
| 6 | `PostSearchBalances` | `balances/search` com `change.inStock`; `TotvsBalances` e `TotvsBalancesLocations`; job 13017 |

---

## 10. Decisões

### 10.1 Tomadas (23/10/2026)

| # | Pergunta | Decisão | Onde está no texto |
|---|---|---|---|
| 1 | Schema das tabelas | Novo schema **`totvs`** (`totvs.Totvs*`), com bulk em `untreated` | §5.0 |
| 2 | Scripts `001`/`002` já rodaram? | **Não**: numeração refeita no formato do Movidesk, com scripts por service | §5.4 |
| 3 | Carga inicial (banco vazio) | Decidido em 23/10: as janelas de hoje. **Revisado em 26/10/2026**: pessoas, vendedores e produtos desde 2000 (produtos em fatias de 30 dias na carga inicial); notas e pedidos de venda desde 01/01/2025; pedidos de compra 5 meses; duplicatas e comissões 10 meses | §4.2 |
| 4 | Filiais lidas | **Todas as da empresa**, lidas de `totvs.TotvsBranches` (sem coluna de filiais em `TotvsParameters`) | §4.2, §5.1 |
| 5 | Chave das notas fiscais | **`doc_company` + `branchCode` + `invoiceSequence` + `invoiceDate`**, conferida no swagger da API Fiscal | §5.2.1 |
| 6 | `[Authorize]` no controller | **Comentado**, como Movidesk e AfterSale | §6.4 |
| 7 | Intervalo dos jobs | **3 min** para documentos (notas, pedidos, duplicatas), clientes PF e pessoas jurídicas; **120 min** para filiais, vendedores e produtos | §7 |
| 8 | Credenciais | **Trocar na Totvs** ao migrar para `TotvsParameters` | §5.1 |
| 9 | Implementação | **Um service de cada vez** | §9 |
| 10 | Intervalo de comissões pagas | **3 min**, como documento | §7 |
| 11 | Chave de `TotvsInvoicesSalesOrders` | **Chave da nota + `orderBranchCode` + `orderCode`**, conferida no swagger da API de pedidos | §5.2.1 |
| 12 | `expand` de notas fora da lista da documentação | **Testado:** a API ignora em silêncio; o `expand` passa a ser o da documentação | §5.2.1 |
| 13 | `observationNFE` e `digitalWalletInformation` | **Seguir a documentação**: não há notas com esses campos preenchidos, então os tipos e tamanhos vêm do swagger (`observationNFE` string anulável na nota, enviando `observationNFE` no `expand`; `digitalWalletInformation` como `DigitalWalletModel` dentro de `payments`), sem conferência em resposta real | §5.2.1 |
| 14 | `pageSize` das notas | **100** (o máximo da API), mesmo com ~220 KB por nota | §5.2.1 |
| 15 | Classe base dos services | **Nenhum service usa a `TotvsBaseSyncService`** (removida); cada método faz as próprias chamadas à API | §1, §4.1, §4.2 |
| 16 | Credenciais e token | **Em cache**: `Totvs:Parameters` e `Totvs:Token:<doc_company>` | §4.4 |
| 17 | Multiempresa | **Todas as lojas** da holding, lidas de `TotvsBranches`; nada preso à filial 1 | §1, §4.2 |
| 18 | Nomes dos métodos | **Mantidos** (`PostSearch*`, `GetBranchesList`), como na documentação oficial | §2.3, §4.1 |
| 19 | Empresas em `TotvsParameters` | **Uma linha, a holding Newbloomers** (opção A); sem job de cadastro de empresas | §5.1 |
| 20 | Verbo das rotas do controller | **O da documentação oficial**: `POST` nas buscas, `GET` em `Branches` | §6.4 |
| 21 | CNPJ da holding | **`44065634000106`** (empresa `900` na Totvs), único `doc_company` de `TotvsParameters` | §5.1 |
| 22 | Fonte das regras de cada rota | **Swagger oficial de cada API** (Person, Product, Seller, Purchase Order, Accounts Payable, Fiscal, Sales Order), conferido com chamadas reais | §5.2.1 a §5.2.7 |

| 23 | Empresa base dos produtos | **Uma chamada por marca** (matrizes `1` e `101`); `branchInfoCode` entra na chave de `TotvsSkus` | §5.2.4 |
| 24 | Comissões pagas | **Implementar pelo schema** mesmo sem dados hoje | §5.2.7 |
| 25 | `individualAdditionals` da PF | **Colunas na própria `TotvsPeople`** | §5.2.5, §5.2.8 |
| 26 | Objetos 1:1 | **Todos como colunas do pai**, com o nome do objeto como prefixo; nenhuma tabela nova | §5.2.8 |

| 27 | Separação das marcas nos produtos | **Filtro pela classificação 6000 (SETOR)** em cada chamada; códigos conferidos na API: `2` Misha, `4` Open Era (não `1`/`101`) | §5.2.4 |

| 28 | Escopo das leituras sob demanda (fase 3, 26/10/2026) | **PF e PJ por código, pedido de venda por loja + número, nota por loja + sequência e produto por código**, via as buscas `POST ...search` com lista de chave | §6.4, §9 |
| 29 | Preços, custos e saldos | **Os três entram como jobs de 3 min** no padrão da fase 2 (`PostSearchPrices`, `PostSearchCosts`, `PostSearchBalances`) | §7, §9 |
| 30 | Operações (`TotvsOperations`) | **Descartada**: não existe rota de operações em nenhum swagger da Totvs; o `SearchOperations` do código antigo nunca existiu | §6.4 |
| 31 | `filter.change` nas buscas por chave | **Enviado desde 2000 com as flags da busca por janela**, para o `maxChangeFilterDate` vir preenchido (sem ele a pessoa seria regravada pelo job; sem as flags a API responde 400) | §9 |

### 10.2 Pendentes

Conferido em 26/10/2026: a entrega da fase 2 (fluxo da §4.2, DTOs, entidades, validadores, cache, queries,
repositório, trio de scripts, job, rota e correções da §8) está no código para os 10 métodos, no commit `f615deb`.
O que falta é só execução, não implementação:

| # | Pendência | Depende de |
|---|---|---|
| 1 | Rodar os scripts `001`–`027` num banco (DbUp) e conferir as 85 procedures geradas por `sp_Create_Merge_Procedure`, incluindo as FKs em cascata (produtos, pedidos de venda e notas) | Banco com o `Script003` real (credenciais novas, decisão 8) |
| 2 | Rodar cada rota pelo host Hangfire, na ordem da §9 (`Branches` primeiro), e conferir bulk, merges, `auditing.Logs/Messages/Records` e a segunda execução incremental (janela por `MAX(maxChangeFilterDate)`) | 1 |
| 3 | Cargas iniciais que não foram rodadas: 188 mil PF (`PostSearchCustomers`), produtos e vendedores desde 2000, notas e pedidos de venda desde 01/01/2025, 10 meses de duplicatas (observar tempo e tamanho das páginas) | 2 |
| 4 | Comissões pagas nunca foram lidas com dados reais (só o registro sintético do swagger); conferir quando houver fechamento | 2 |
| 5 | `referencedInvoices` comentado no `expand` das notas: `TotvsInvoicesItemsProductsReferenced` fica vazia até a Totvs corrigir o 400 ou o contorno ser religado (§5.2.1) | Totvs |

Fase 3 iniciada em 26/10/2026 (parte 1 implementada; partes 2 a 6 na ordem da §9). Fase 4 (README e `Responsed Jsons`)
continua opcional e não iniciada.
