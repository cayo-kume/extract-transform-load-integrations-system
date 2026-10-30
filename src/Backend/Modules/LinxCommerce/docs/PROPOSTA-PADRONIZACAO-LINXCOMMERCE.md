# Proposta — padronização do módulo LinxCommerce (padrão Movidesk / AfterSale)

Documento para revisão. **Fases 1, 2 e 4 e as leituras sob demanda da fase 3 implementadas em 29/10/2026** (§9); as escritas
continuam fora. O que está marcado como **(proposto)** segue
o que já foi decidido no Totvs (`src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md`) e no LinxMicrovix
(`src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md`) e foi implementado com as recomendações da §10.2, que continuam
pendentes de confirmação por quem usa o módulo. Nada foi conferido com a API real: não há credenciais (§9, "Não
verificado").

Objetivo: listar tudo o que precisa mudar em `src/Backend/Modules/LinxCommerce` (e nos pontos do host que o
referenciam) para que o módulo siga o mesmo padrão dos módulos **Movidesk** e **AfterSale**, como foi feito no Totvs
e no LinxMicrovix.

Como o Totvs, o LinxCommerce **lê** uma API REST com JSON (as APIs `Sales`, `Profile`, `Catalog` e `Queue` do Linx
Commerce, em `/v1/<Área>/API.svc/web/<Método>`) e **grava** no banco, com respostas **aninhadas** (pedido → itens,
pagamentos, entregas, pacotes...). Ele também tem **duas escritas** (rastreio do pedido e carteira de vendedores,
§4.6). A referência principal é a leitura recorrente do Movidesk (`GetTickets`, `GetPeople`, `GetServices`), o Totvs
já migrado (`TotvsService`, `TotvsPeopleService`) e o padrão de cache de `docs/PADRAO-CACHE-INTEGRACOES.md`.

**Fontes conferidas em 28/10/2026: só o código.** Diferente do Totvs e do Microvix, **não há credenciais** do Linx
Commerce disponíveis, então **nenhuma chamada real** foi feita e nenhuma documentação oficial foi usada. Foram lidos:

* todo o código do módulo (125 arquivos, ~6.700 linhas): 7 services, 4 repositórios, 54 entidades e respostas,
  38 validadores e um helper, a injeção de dependência e os scripts;
* o `LinxCommerceController` e o `LinxCommerceRecurringJobsExtensions` do host Hangfire (hoje WebApi) (os dois inteiros
  comentados), o `ServicesExtensions.cs` e o `appsettings.json`;
* os módulos que **leem** as tabelas do LinxCommerce: `Oto` (`OtoCommandHandler`) e `Stone`
  (`StoneLogisticaCommandHandler`), que fazem `join` com `LINX_COMMERCE.[ORDER]`, `OrderDeliveryMethod` e
  `OrderProperties` (§5.0).

Por isso, tudo o que depende da API (nomes e tipos dos campos, paginação, filtros aceitos no `Where`, fila de
integração, chaves naturais) está descrito **como o código usa hoje** e marcado com **"a conferir"**. O que é
convenção, banco, fluxo, host e auditoria não depende da API e pode ser feito já (fase 1, §9).

---

## 1. Situação atual (inventário)

| Item | Hoje | Passa a ser |
|---|---|---|
| Projetos | `Domain.LinxCommerce`, `Application.LinxCommerce`, `Infrastructure.LinxCommerce` (no `.slnx`) | Os mesmos, nas mesmas pastas (§2) |
| Referências | `..\..\..\..\newbloomers-core-dlls\src\*.Core`, **que não existe no repositório**; `PackageReference` sem versão (`Caching.Memory`, `Newtonsoft.Json`, `FluentValidation.DependencyInjectionExtensions`, `Dapper`, `Microsoft.Extensions.Http`, `System.Data.SqlClient`) | `..\..\..\Shared\*.Shared` e versões explícitas (§2.1) |
| Host | `Hangfire.csproj` não referencia o módulo. `AddScopedLinxCommerceServices` comentado em `ServicesExtensions.cs`; o `LinxCommerceController` está comentado linha a linha (só sobram a classe, a rota e o `[Authorize]`) e o `LinxCommerceRecurringJobsExtensions` também | Ligado no host, um service por vez (§6.3, §9) |
| Métodos | 7 interfaces com **70 métodos** da API; **48** são `throw new NotImplementedException()`. `PaymentTermService`, `RemarksService` e `ShipmentService` são classes vazias que nem implementam a interface (e não estão no DI) | 4 services por assunto com o prefixo do módulo, só com o que é usado, mais um `LinxCommerceService` comum (§2.2) |
| Leituras | 6 recorrentes (pedidos por data e por fila, status de pedido, clientes por data e por fila, produtos e SKUs por data e por fila, vendedores) e 4 sob demanda (`GetOrderByNumber`, `GetCustomer`, `GetSKU`, `GetSalesRepresentative`) | Uma leitura recorrente por recurso, pela data de alteração (§4.2); sob demanda na fase 3 |
| Escritas | `UpdateTrackingNumberOrder` (envia o rastreio da JadLog ao pedido) e `SaveSalesRepresentative` (regrava **todos** os vendedores e zera a carteira, §8 item 1) | Fora das fases 1 e 2; voltam só depois de decididas e testadas (§4.6, §10.2) |
| Credenciais | `LinxCommerceJobParameter` montado no controller a partir do `appsettings` (`LinxCommerce:Key`, `Authentication`, `DocMainCompany`, `ParametersTableName`, `Methods`), mas o `appsettings` de hoje só tem `"BaseUrl": "https://misha.layer.core.dcg.com.br"` | `linxcommerce.LinxCommerceParameters`, uma linha por cliente, com a URL do ambiente, fora do git, com cache (§5.1) |
| Janela de leitura | Cada método de um jeito: pedidos `ModifiedDate` de hoje−2; clientes e SKUs `CreatedDate` de hoje−1; produtos `CreatedDate` de hoje−7. Alterações em clientes, produtos e SKUs **nunca** são relidas | Data de alteração a partir do banco (`MAX(ModifiedDate)` − folga), como o `GetTicketsLastUpdate` (§4.2) |
| Paginação | Só os pedidos paginam. Clientes, SKUs e fila leem a página 0 com 1000; produtos a página 0 com 10000; status e vendedores com `PageSize = 0` | Todas as buscas paginadas pelo `Page.PageCount` (a conferir, §4.2) |
| Banco | Nenhum script de tabela ou procedure. Bulk em `untreated.<Tabela>` (sobrecarga `[Obsolete]`), merges em `jobParameter.schema` (`"linx_commerce"`, fixo no controller) e consultas em `[linx_commerce]` | Schema `linxcommerce` com scripts por service (§5) |
| Scripts | `Script001` (app 70) e `Script002` (jobs, **todo comentado**, ids repetidos) | Numeração refeita no formato do Totvs (§5.4, §7) |
| Jobs | `EnumJob` do módulo com 7 ids, mas os services usam o `EnumJob` de `Domain.Core.Enums`, inclusive um `LinxCommerceOrdersStatus` que o enum do módulo não tem | Um job por recurso, nas faixas do próprio `Script002` (§7) |
| Dados sensíveis | `Customer.Password` e `CustomerHash` gravados; `OrderPaymentInfo` grava portador, dica do número e do código de segurança do cartão e chave PIX | Fora das tabelas (§3.2, §10.2) |
| Documentação | Sem `README.md` e sem pasta de respostas de referência | README e `Responsed Jsons` quando houver respostas reais (§9) |

Conclusão: o módulo **não compila** nem restaura. Como no Microvix, faltam os três projetos `*.Core` e as versões
dos pacotes; trocando por `*.Shared`, faltam tipos que o Shared não tem: `IIntegrationsCoreRepository` (hoje
`ISharedRepository`), `ParameterBase`, `SQLCommandException`, `SkipProperty` de `Domain.Core.Extensions` e o
`EnumJob` de `Domain.Core.Enums`, que todos os services importam no lugar do `EnumJob` do próprio módulo.

Diferente do Microvix, a migração aqui é menor em quantidade (6 recursos, não 260) e maior em **forma**: hoje as
entidades são ao mesmo tempo o alvo da desserialização (`JsonConvert.DeserializeObject<Order.Root>`) e a linha da
tabela; não há DTO, a chave não tem empresa e cada busca chama um `Get` por registro (§4.2).

---

## 2. Projetos, referências e nomes

### 2.1 `.csproj`

| Projeto | Hoje | Padrão (Totvs) |
|---|---|---|
| `Domain.LinxCommerce` | `newbloomers-core-dlls\...\Domain.Core`; `FluentValidation.DependencyInjectionExtensions` sem versão | `..\..\..\Shared\Domain.Shared\Domain.Shared.csproj`; sem pacote (os validadores saem do Domain, §2.4) |
| `Application.LinxCommerce` | `Application.Core`; `Caching.Memory` e `Newtonsoft.Json` sem versão; `<Folder Include>` de `Handlers` vazias | `Application.Shared`; `Microsoft.Extensions.Caching.Memory` **10.0.8**; `Newtonsoft.Json` com a versão do Shared |
| `Infrastructure.LinxCommerce` | `Infrastructure.Core`; `Dapper`, `Microsoft.Extensions.Http`, `System.Data.SqlClient` sem versão | `Infrastructure.Shared`; sem pacote próprio (o `SalesRepresentativeRepository` abre `SqlConnection` direto com Dapper e passa a usar o `ISharedRepository`, §6.2) |

O LinxCommerce fica no mesmo nível do Totvs (`Modules/LinxCommerce/<projeto>`), então o caminho do Shared tem
**3 níveis**, como no Totvs (o Microvix e o Movidesk têm 4).

### 2.2 Services por assunto, com o prefixo do módulo (proposto)

No Movidesk e no Totvs um service agrupa os métodos de um assunto (`MovideskTicketsService`, `TotvsPeopleService`).
Hoje há um service por entidade da API, sem prefixo (`OrderService`, `SKUService`...), com 48 métodos vazios. Passa a
ser um service por assunto, com o prefixo, **só com os métodos que o módulo usa**:

| Service | Leitura recorrente | Sob demanda (fase 3) | Rotas da API usadas |
|---|---|---|---|
| `LinxCommerceService` (comum, novo) | — | — | Lote, clientes, credenciais e última data de alteração (§4.1) |
| `LinxCommerceOrdersService` | `SearchOrders`, `SearchOrderStatus` | `GetOrder`, `GetOrderByNumber` | `Sales/SearchOrders`, `Sales/GetOrder`, `Sales/GetOrderByNumber`, `Sales/SearchOrderStatus`, `Sales/GetOrderStatus` |
| `LinxCommerceCustomersService` | `SearchCustomer` | `GetCustomer` | `Profile/SearchCustomer`, `Profile/GetPerson`, `Profile/GetCompany` |
| `LinxCommerceCatalogService` | `SearchProduct`, `SearchSKU` | `GetProduct`, `GetSKU` | `Catalog/SearchProduct`, `Catalog/GetProduct`, `Catalog/SearchSKU`, `Catalog/GetSKU` |
| `LinxCommerceSalesRepresentativesService` | `SearchSalesRepresentative` | `GetSalesRepresentative` | `Sales/SearchSalesRepresentative`, `Sales/GetSalesRepresentative` |

Notas sobre a lista:

* **Os 48 métodos vazios saem** das interfaces (`AddCustomerRelation`, `ApproveCustomerFile`, `GetOrderGroup`,
  `MakeOrderTransition`, `RunOrderWorkflow`, `SearchPaymentTerm`, `UpdateRemarks`, `GetShipment`...), junto com
  `IPaymentTermService`, `IRemarksService`, `IShipmentService` e as três classes vazias. Eles servem hoje só como lista
  dos nomes da API; essa lista fica registrada no histórico do git e na §10.2, pendência 6. Um método novo entra
  quando houver credencial para conferir a resposta.
* **Fila de integração (`Queue/SearchQueueItems`, `GetQueueOrders`, `GetQueueCustomers`, `DequeueQueueItems`)**: as
  leituras por fila saem das fases 1 e 2 (proposto, §4.2 e §10.2, pendência 2). A leitura por data de alteração cobre
  o mesmo dado sem retirar itens de uma fila que outro sistema pode estar consumindo.
* **Escritas** (`Sales/UpdateOrder`, `Sales/SaveSalesRepresentative`) ficam fora dos services de leitura; o desenho
  está na §4.6.
* `IntegrityLockOrdersRegisters` (relê pedidos gravados sem item, endereço, entrega ou pagamento) sai: com a merge
  por lote e a deduplicação (§4.2), um pedido só é gravado inteiro; a consulta tem data fixa (`'2025-01-01'`) e
  status fixos da Newbloomers (`6, 20, 22, 32, 33`).

Repositórios com o mesmo nome (`ILinxCommerceOrdersRepository`...), um `ILinxCommerceRepository` comum
(parâmetros e última data de alteração) e **um** `LinxCommerceCommandHandler` com todas as queries, como o
`MovideskCommandHandler`. Hoje não há command handler: as queries estão em texto dentro dos repositórios
(`//Refatorar Aqui (Adicionar Command Handler ao projeto application)`, 4 vezes).

### 2.3 Nomes dos métodos e das tabelas (proposto)

**O nome do método é o nome da rota de busca da API**, como no Totvs e no Microvix (os nomes seguem a API):
`SearchOrders()`, `SearchCustomer()`, `SearchSKU()`, e não `SearchOrdersByDateInterval()`. O sufixo `ByDateInterval`
/ `ByQueue` some porque cada recurso passa a ter uma leitura só (§4.2).

As tabelas levam o prefixo do módulo e o recurso no plural, como no Totvs (`TotvsSaleOrders`,
`TotvsSaleOrdersProducts`), porque os nomes da API (`Order`, `Customer`, `Product`) colidiriam com qualquer outro
módulo no `untreated` compartilhado: `linxcommerce.LinxCommerceOrders`, `linxcommerce.LinxCommerceOrdersItems`. O job
e a rota têm o nome do método (`SearchOrders`), e a descrição do job diz o recurso.

### 2.4 Namespaces e pastas

| Item | Hoje | Padrão |
|---|---|---|
| Namespaces | `Domain.LinxCommerce.Interfaces.Repositorys` na pasta `Abstractions/Repositorys`; `Infrastructure.LinxCommerce.Repository` (singular) no `CustomerRepository` e `...Repositorys` nos outros | O namespace segue a pasta: `Domain.LinxCommerce.Abstractions.Repositorys`, `Infrastructure.LinxCommerce.Repositorys` |
| `Entities/Responses` | Respostas de busca e de fila misturadas com `Responses` de `Get` (`GetOrderPayments`, `GetSalesRepresentative`) | `Dtos/<Assunto>/Search*.cs` (resposta da busca) e `Dtos/<Assunto>/<Recurso>.cs` (resposta do `Get`) |
| `CustomValidations/**` (Domain) | 38 validadores FluentValidation **no Domain** sobre as entidades, e um `ConvertToDateTimeValidation` próprio | `Application.LinxCommerce/Validations/<Assunto>`, sobre os DTOs, como no Totvs; o helper sai (o Shared já tem `FluentValidatorsExtension` e `NullableConverters`) |
| `Entities/Parameters/LinxCommerceJobParameter.cs`, `Page.cs` | Parâmetro de execução herdando `ParameterBase` (schema, banco, tabela, intervalo); `Page`/`Root` soltos | Removidos. `Entities/Parameters/Company.cs` com as colunas de `LinxCommerceParameters` (§5.1); `Page` vai para os DTOs de busca |
| `Entities/Queue/QueueItem.cs`, `SearchQueued*` | Itens da fila | Saem com a leitura por fila (§2.2) |
| `Entities/Order/OrderTrackingNumber.cs`, `OrderSeller`, `OrderMultiSiteTenant`, `OrderType` | O primeiro é a linha da escrita de rastreio; os outros três são objetos 1:1 do pedido | `OrderTrackingNumber` fica com a escrita (§4.6); os 1:1 viram colunas do pedido (§5.2.1) |

Saem também os `using` sem uso (`System`, `System.Text` em `EnumJob.cs`), os `try { ... } catch { throw; }` sem
efeito e as variáveis declaradas e nunca usadas (`pageIndex`, `pageCount` e `salesRepresentativeResults` nos dois
métodos de vendedores, `route` nos laços de produto e SKU, `orderStatus` no `SearchOrderStatus`, `dequeueResponse`).

---

## 3. Domain.LinxCommerce

### 3.1 Estrutura proposta

```
Domain.LinxCommerce/
├── Abstractions/Repositorys/
│   ├── ILinxCommerceRepository.cs             (parâmetros, última data de alteração)
│   └── ILinxCommerce<Assunto>Repository.cs    (um por service da §2.2)
├── Dtos/                                       ← o JSON da API ({ get; init; })
│   ├── Orders/      SearchOrders, Order (+ filhas), SearchOrderStatus, OrderStatus
│   ├── Customers/   SearchCustomer, Customer (+ filhas)
│   ├── Catalog/     SearchProduct, Product (+ filhas), SearchSKU, Sku (+ filhas)
│   ├── SalesRepresentatives/  SearchSalesRepresentative, SalesRepresentative (+ filhas)
│   └── Page.cs
├── Entities/
│   ├── Parameters/Company.cs                   (→ linxcommerce.LinxCommerceParameters)
│   └── <mesmas pastas dos DTOs>                (→ linxcommerce.LinxCommerce<Tabela>)
└── Enums/EnumJob.cs
```

### 3.2 Convenções que mudam

* **DTO separado da entidade.** Hoje a mesma classe recebe o JSON (`DeserializeObject<Person>`) e vira a linha da
  tabela pelo construtor `Person(Person customer, string getCustomerResponse)`, que copia campo a campo. Passa a ser
  como no Totvs: DTO com `{ get; init; }` e sem construtor, entidade com as colunas da tabela, construída de
  `(doc_company, batchTime, dto)`.
* **DTOs montados a partir da resposta real** (a conferir). Os campos de hoje são a única fonte; quando houver
  credencial, cada DTO é conferido com uma resposta real antes do script de tabela, como no Microvix (§3.2 de lá). Até
  lá, os DTOs novos copiam os nomes e tipos das entidades de hoje.
* **Entidades com `doc_company` e `integrated_or_updated_at`.** Nenhuma entidade tem as duas colunas; todas têm
  `lastupdateon` calculado **por registro** com `TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard
  Time")` (identificador só do Windows). Passa a ser como no Movidesk e no Totvs: `doc_company` entra na chave
  primária e `integrated_or_updated_at` é o horário do lote (`GetBatchTime()`), igual para todo o lote.
* **Filhas recebem a chave do pai no código.** Hoje cada construtor de filha copia o `OrderID`/`CustomerID`/`ProductID`
  do pai; com `doc_company` na chave, a filha recebe os dois.
* **Datas e números anuláveis.** As entidades usam `DateTimeOffset?`, `Int32?`, `decimal?`, mas várias chaves são
  `Int32` não anulável (`CustomerID`, `SalesRepresentativeID`, `OrderStatusID`) e o pedido tem `Guid? OrderID` como
  chave. Padrão da `src/Backend/Modules/General/AfterSale/docs/PROPOSTA-DATAS-NULAS-AFTERSALE.md`: dados anuláveis, chaves não anuláveis, e o registro sem
  chave é rejeitado na validação com mensagem.
* **`DateTimeOffset` gravado como `datetime`?** (a conferir). O Linx Commerce devolve as datas com fuso; o código de
  hoje grava `DateTimeOffset` direto no `SqlBulkCopy`, sem script para dizer o tipo da coluna. Proposto:
  `datetimeoffset` nas colunas de data da API, para não perder o fuso, e `datetime` só em `integrated_or_updated_at`.
* **Corte de texto com `StringLengthValidation`**, com o tamanho do script, como no Movidesk. Hoje não há script e um
  texto maior que a coluna derruba o `SqlBulkCopy` do lote inteiro.
* **Sem `Responses` na entidade.** Hoje cada entidade guarda um `Dictionary<id, json>` marcado `[SkipProperty]` só
  para o `AddRecord`, e o service procura o JSON com `Responses.Where(pair => pair.Key == ...)` em cada registro. Como no
  Movidesk (`SaveServices`), o método monta um `Dictionary<chave, json>` e passa para a gravação.
* **Listas e ids concatenados viram filhas ou ficam como texto** (a conferir). Hoje `Customer.CustomerGroupID` é
  `String.Join(", ", Groups...)`, e `Product` grava `SkusIDs`, `FlagsIDs`, `MediasIDs`, `CategoriesIDs`, `TagsIDs`,
  `ShippingRegionsIDs` como estão no JSON. Proposto: os grupos do cliente viram a filha
  `LinxCommerceCustomersGroups`; as listas de ids do produto ficam como texto, porque os objetos que elas apontam
  (categorias, marcas, flags) não são lidos pelo módulo.
* **Sem dados sensíveis** (proposto, §10.2 pendência 4). `Password` e `CustomerHash` do cliente e, no pagamento,
  `Holder`, `NumberHint`, `SecurityCodeHint`, `PixQRCode` e `PixKey` **não entram** na entidade. O DTO pode tê-los
  para a desserialização, mas nada é gravado nem vai para `auditing.Records` (o JSON original é gravado sem esses
  campos).
* **`EnumJob`** do próprio módulo, com os ids conferidos com o script (§7).

---

## 4. Application.LinxCommerce

### 4.1 Services

| Service | Observação |
|---|---|
| `LinxCommerceService` (comum) | Mesmo papel do `MovideskService`/`TotvsService`: `GetBatchTime`, `GetCompanys` (cache), `GetAuthHeaders(company)` e `GetLastModifiedDate(company, recurso)`. Substitui o `BuildAuthHeaders` copiado nos 4 services |
| Os 4 services da §2.2 | Cada método escreve o próprio fluxo (§4.2), sem classe base |

**Tudo à mão em cada método (proposto, como no Totvs e no Microvix).** O comum fica no `ILinxCommerceService`,
injetado nos services como o `IMovideskService`: horário do lote, clientes, cabeçalho de autenticação e última data
de alteração. A busca paginada, os `Get` por registro, a validação, a conversão e a gravação ficam no corpo de cada
método; partes de gravação podem virar métodos privados do próprio service, como o `SaveServices` do Movidesk.

**Autenticação.** Hoje é `Basic` com `"{UserAuthentication}:{KeyAuthorization}"` em Base64, montado a cada chamada
(4 cópias de `BuildAuthHeaders`). Fica igual, montado uma vez por cliente no `LinxCommerceService` a partir de
`LinxCommerceParameters` (§5.1).

**Sob demanda.** `GetOrderByNumber`, `GetCustomer`, `GetSKU` e `GetSalesRepresentative` existem hoje, mas não abrem
o log, passam `new Guid()` como execução (`GetCustomer`) ou nem gravam (`GetSKU` e `GetSalesRepresentative` têm a
gravação comentada). Não entram nas fases 1 e 2. Entram depois (fase 3, §9), no desenho do
`GET Tickets/{docCompany}/{id}` do Movidesk e das rotas sob demanda do Totvs: cabeçalho `X-User-Name`, `docCompany`
por `^\d{14}$` e registro em `linxcommerce.LinxCommerceRequests`.

### 4.2 Fluxo de leitura (escrito em cada método)

Cada método segue o desenho do `GetTickets` do Movidesk e das buscas do Totvs:

1. abre o log em `auditing.Logs` com o job do método;
2. para cada cliente de `linxcommerce.LinxCommerceParameters` (§5.1), obtém URL e credenciais pelo
   `ILinxCommerceService.GetCompanys`;
3. calcula a janela **a partir do banco**: `MAX(ModifiedDate)` do recurso para o cliente, menos uma folga de
   1 hora, como o `GetTicketsLastUpdate`. Com o banco vazio, a carga inicial vem de uma data fixa por recurso
   (§10.2, pendência 3). O filtro é o `Where` que a API já aceita nos pedidos:

   | Recurso | Filtro proposto | Hoje |
   |---|---|---|
   | Pedidos | `ModifiedDate>="<início>" && ModifiedDate<="<agora>"` | `ModifiedDate` de hoje−2 até hoje 23:59:59 |
   | Status de pedido | sem filtro (tabela pequena, relida inteira; a conferir) | sem filtro, `PageSize = 0`, com hash da resposta |
   | Clientes | `ModifiedDate` (**a conferir**: o código só usa `CreatedDate`, e a entidade não tem `ModifiedDate`) | `CreatedDate` de hoje−1 |
   | Produtos | `ModifiedDate` (a busca devolve `ModifiedDate`) | `CreatedDate` de hoje−7 |
   | SKUs | `ModifiedDate` (a busca devolve `ModifiedDate`) | `CreatedDate` de hoje−1 |
   | Vendedores | sem filtro (a conferir se a busca aceita data) | sem filtro, `PageSize = 0` |

   O `Where` é montado só com datas formatadas pelo próprio código (`yyyy-MM-ddTHH:mm:ss`), nunca com texto vindo de
   fora;
4. **pagina a busca** pelo `Page.PageCount` da resposta, como o `SearchOrdersByDateInterval` já faz. Hoje só os
   pedidos paginam e só o DTO de pedidos tem `Page`: clientes e SKUs perdem o que passa de 1000 por execução, e
   produtos o que passa de 10000. O tamanho da página (500 nos pedidos) e o significado de `PageSize = 0` ficam a
   conferir;
5. deduplica os ids da busca e chama o `Get` de cada um (`GetOrder`, `GetPerson`/`GetCompany`, `GetProduct`,
   `GetSKU`, `GetSalesRepresentative`). **A conferir:** o `SearchOrders` já devolve itens, endereços, pagamentos,
   entregas e nota (o DTO de hoje tem essas listas), e o código ignora e chama o `GetOrder` de novo para cada pedido.
   Se a busca trouxer o pedido completo, o `GetOrder` por registro sai e a leitura cai de N+1 chamadas para uma por
   página;
6. chama a API com `IApiClient.SendAsync` e trata o `ApiResponse`: erro HTTP vira mensagem em `auditing.Messages` e
   o registro (ou a página) segue para o próximo. Hoje o `PostAsync` devolve só o corpo e o código assume sucesso:
   um erro vira `NullReferenceException` em `result.Page.PageCount` ou no `DeserializeObject`. A desserialização
   passa a tratar `JsonException` com mensagem e `continue`, como no `GetServices`;
7. compara com o cache de estado (§4.4) e fica só com o novo/alterado;
8. valida cada DTO (FluentValidation), registra as mensagens e converte para entidade com o `batchTime` do lote;
9. grava (bulk em `untreated` + merges em `linxcommerce`, pai → filhas), atualiza o cache e registra cada registro
   em `auditing.Records` com o JSON original (sem os campos sensíveis, §3.2);
10. fecha o log (`SetLogEndDate` + `CommitAllChanges`) num `finally`. Hoje nenhum método tem `try/finally`.

Uma leitura com erro em qualquer página não grava nada daquele cliente naquele recurso, para o `MAX(ModifiedDate)`
não pular registros (como no `GetBranchesList` do Totvs).

**Fila × data de alteração (proposto: só data).** Hoje cada recurso tem as duas leituras, com o mesmo job. A fila
tem problemas que a leitura por data não tem:

* o `QueueID = 21` é fixo no código, igual para pedidos, clientes, produtos e SKUs;
* lê só a página 0 (1000 itens) e não pagina;
* **retira da fila todos os itens lidos** (`DequeueQueueItems`), inclusive os que falharam na desserialização ou na
  validação: um erro perde o aviso de alteração para sempre;
* a fila é do Linx Commerce, e outro sistema pode depender dela.

A leitura por data é idempotente: uma execução que falha é repetida na próxima, sem perder nada. A fila pode voltar
depois, se a leitura por data não for suficiente (§10.2, pendência 2).

### 4.3 Validações (FluentValidation)

Os 38 validadores de `Domain/CustomValidations` passam para `Application.LinxCommerce/Validations/<Assunto>`, sobre os
DTOs. Mudanças:

* acompanham a renomeação de pastas e DTOs (§2.4); os validadores de objetos 1:1 que viram colunas (§5.2) viram
  regras do validador do pai;
* os tamanhos passam a citar o script de tabela, como o comentário de `Movidesk.Validations.Services.ServiceValidator`;
  hoje não há script, e os limites não têm fonte;
* a mensagem de erro deixa de ser montada com `msg.Replace("[0]", ...)` e procurada depois com
  `_errorMessagesList.Where(x => x.Contains(...))` (texto livre, que falha se o nome tiver o texto de outro registro):
  a mensagem vai para o log junto com o registro, como no Totvs;
* registro com **uma** chamada `AddValidatorsFromAssemblyContaining<...>()` (§6.1). Hoje são **36 chamadas** sobre o
  mesmo assembly: cada validador fica registrado 36 vezes.

### 4.4 Cache (padrão `docs/PADRAO-CACHE-INTEGRACOES.md`)

| Chave | Conteúdo | Expiração |
|---|---|---|
| `LinxCommerce:Parameters` | Linhas de `linxcommerce.LinxCommerceParameters` | 2 h absoluto / 30 min deslizante |
| `LinxCommerce:{doc_company}:Orders` | `OrderID` → `ModifiedDate` | 2 h absoluto / 30 min deslizante |
| `LinxCommerce:{doc_company}:Customers` | `CustomerID` → data de alteração (a conferir) | idem |
| `LinxCommerce:{doc_company}:Products` | `ProductID` → `ModifiedDate` | idem |
| `LinxCommerce:{doc_company}:Skus` | `ProductID` do SKU → `ModifiedDate` | idem |
| (sem cache) | Status de pedido e vendedores | Relidos inteiros; a merge não regrava o que não mudou |

Aqui o cache de estado **faz sentido**, ao contrário do Microvix: a janela relê a última hora (folga do item 3 da
§4.2), e cada registro relido custaria um `Get` a mais. Com o cache, um registro com a mesma data de alteração do
banco não é buscado de novo. O cache é carregado do banco pelos ids da página (em lotes, não num `IN` único) e
atualizado depois da gravação. O de hoje erra:

* as chaves não levam a empresa (`"LinxCommerce:Orders"`, `"LinxCommerce:CustomerHash"`);
* o cache de pedidos é criado com os ids da **primeira** execução (`GetOrCreateAsync`) e nunca recarregado: um
  pedido que não estava nela nunca é encontrado;
* o filtro é `x.ModifiedDate < y.ModifiedDate`: um pedido **sem alteração** (mesma data) é buscado de novo;
* clientes e status guardam o **hash da resposta inteira** da busca: qualquer mudança em um registro relê todos, e
  nenhuma mudança relê nenhum, mesmo que a gravação anterior tenha falhado.

### 4.5 Command handler (queries)

* Um `LinxCommerceCommandHandler` com as queries de todos os métodos.
* As queries passam a ler `linxcommerce.LinxCommerce<Tabela>` filtrando por `doc_company` (hoje `[linx_commerce]`
  fixo no texto, mesmo quando a merge usa `jobParameter.schema`).
* `CreateGetCompanysQuery`, `CreateGetLastModifiedDateQuery(doc_company, recurso)` e
  `CreateGetRegistersExistsQuery(doc_company, recurso, ids)`. O nome da tabela vem de uma lista fechada, nunca de
  texto livre.
* Valores interpolados seguem a regra do Movidesk: só valores já validados (CNPJ por `^\d{14}$`, `Guid` e números
  convertidos pelo código). Hoje `GetRegistersExists` monta o `IN (...)` com todos os ids da resposta, sem lotes.

### 4.6 Escritas

As duas escritas de hoje **não entram** nas fases 1 e 2 (proposto). Entram só depois de decididas (§10.2,
pendência 1) e de testadas num ambiente de homologação, como as escritas do Totvs
(`src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md`): log em `linxcommerce.LinxCommerceRequests` com o `status_code`, trava de
repetição e corpo sem dados sensíveis.

| Escrita | O que faz hoje | Problemas |
|---|---|---|
| `UpdateTrackingNumberOrder` (`Sales/UpdateOrder`) | Lê os pedidos dos últimos 7 dias com nota e com rastreio da **JadLog** (`general.JadLogPedidos`/`JadLogTracking`) ainda não enviado, e manda `WorkflowType = "SHIPPED"` com nota e rastreio, de 80 em 80 com `Thread.Sleep(5s)` | Depende de tabelas de outro módulo (só JadLog; Stone, TotalExpress e FlashCourier não entram); considera sucesso qualquer resposta não nula (o `PostAsync` devolve o corpo também no erro); grava o envio em `[linx_commerce].[{tableName}]` direto na tabela final e chama a merge de **`Customer`** (§8 item 4); `Thread.Sleep` bloqueia a thread do Hangfire |
| `SaveSalesRepresentative` (`Sales/SaveSalesRepresentative`) | Lê **todos** os vendedores e regrava cada um com `RemovePhoto = true`, `RemoveNotInformedAddresses/Customers/Users = true` e `Portfolio.Customers` **vazio** | Apaga a foto, os endereços, os usuários e **toda a carteira de clientes** de todos os vendedores a cada execução; manda `Commission` em `PortfolioCommission` e em `GeneralCommission` (§8 item 1). O job no script diz "Atualização das carteiras de clientes das vendedoras", mas nenhuma carteira é montada |

Se a escrita de rastreio continuar, ela passa a ler o rastreio de uma origem comum às transportadoras (a definir com
quem usa) e fica no `LinxCommerceOrdersService`. A escrita de vendedores só volta com a regra de montagem da carteira
escrita aqui.

---

## 5. Banco de dados

### 5.0 Schema `linxcommerce` (proposto)

Todas as tabelas do módulo ficam no schema próprio **`linxcommerce`**, como `movidesk`, `aftersale`, `totvs` e
`linxmicrovix` (sem `_`). Hoje o código aponta para `linx_commerce`, que nenhum script deste repositório cria.

* o bulk vai para `untreated.LinxCommerce<Tabela>` (`BulkInsertIntoTableRaw(table, "untreated")`, sem a sobrecarga
  `[Obsolete]`);
* as merges ficam em `linxcommerce.sp_LinxCommerce<Tabela>_Sync`, geradas por `dbo.sp_Create_Merge_Procedure`;
* o primeiro script cria o schema
  (`if not exists (select 1 from sys.schemas where name = 'linxcommerce') exec('create schema linxcommerce')`).

**Consumidores do schema legado.** O `OtoCommandHandler` e o `StoneLogisticaCommandHandler` fazem `join` com
`LINX_COMMERCE.[ORDER]` (por `ORDERNUMBER`), `OrderDeliveryMethod` e `OrderProperties`. Mudar o schema e os nomes
quebra os dois. Proposto, como no Microvix com o Oto: **o legado não é tocado** nesta refatoração, e a adaptação dos
consumidores (ou uma view de compatibilidade) é decidida à parte (§10.2, pendência 5).

### 5.1 `linxcommerce.LinxCommerceParameters` (novo; tira as credenciais do `appsettings`)

**Uma linha por cliente** (proposto: nada fixo da Newbloomers). No Linx Commerce, cada cliente tem o **seu
endereço** (hoje `https://misha.layer.core.dcg.com.br`, fixo no `appsettings`), então a URL também vai para a tabela,
e o `HttpClient` do módulo fica sem `BaseAddress` (o `ApiClient` do Shared aceita rota absoluta).

```sql
create table [linxcommerce].LinxCommerceParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key,
	[doc_company] nvarchar(14) not null,   --CNPJ do cliente (chave em todas as tabelas do módulo)
	base_url nvarchar(200) not null,       --endereço do ambiente do cliente (ex.: https://<loja>.layer.core.dcg.com.br)
	user_name nvarchar(100) not null,      --usuário da API (primeira parte do Basic)
	api_key nvarchar(200) not null         --chave da API (segunda parte do Basic)
);
```

* `doc_company` é o CNPJ que o cliente escolher para identificá-lo. Na Newbloomers, o CNPJ da holding
  (`44065634000106`), como no Totvs e no Microvix.
* O script de valores fica **fora do git** (`Script003_..._Table.sql.example` no repositório e o `.sql` no
  `.gitignore`). **Hoje não há valores para ele**: as credenciais não estão disponíveis, e o `appsettings` não tem
  mais `Key` nem `Authentication`. Enquanto não houver, a tabela fica vazia e todos os jobs registram que não há
  cliente e terminam.
* Somem do `appsettings` as chaves que o controller lia (`DocMainCompany`, `ProjectName`, `ParametersTableName`,
  `Key`, `Authentication`, `Methods`) e o `LinxMicrovix:ParametersDateInterval`, que o controller do LinxCommerce lia
  da seção de outro módulo.

### 5.2 Tabelas de dados

Mesma convenção do Movidesk e do Totvs: par `untreated.LinxCommerce<Tabela>` (sem chave, recebe o bulk) e
`linxcommerce.LinxCommerce<Tabela>` (com chave primária), com `integrated_or_updated_at` e `doc_company` em todas.
Chaves tiradas do código de hoje; todas **a conferir** com uma resposta real antes do script.

| Recurso | Tabela pai (chave, além de `doc_company`) | Filhas | Hoje |
|---|---|---|---|
| Pedidos | `LinxCommerceOrders` (`OrderID`) | `Items` (`OrderItemID`), `Tags` (`OrderID`, `TagID`), `Addresses` (`OrderAddressID`), `PaymentMethods` (`OrderPaymentMethodID`), `DeliveryMethods` (`OrderDeliveryMethodID`), `Discounts` (`OrderID`, `DiscountID`), `Shipments` (`OrderShipmentID`) → `Packages` (`OrderPackageID`), `Invoices` (`OrderInvoiceID`), `Properties` (`OrderPropertyID`) | `Order`, `OrderItem`, `OrderTag`, `OrderAddress`, `OrderPaymentMethod`, `OrderPaymentInfo`, `OrderDeliveryMethod`, `OrderDiscount`, `OrderShipment`, `OrderPackage`, `OrderInvoice`, `OrderProperties` |
| Status de pedido | `LinxCommerceOrderStatus` (`OrderStatusID`) | — | `OrderStatus` |
| Clientes | `LinxCommerceCustomers` (`CustomerID`) | `Addresses` (`ID`), `Groups` (`CustomerID`, `CustomerGroupID`) | `Customer`, `CustomerContact`, `CustomerAddress`, `CustomerEmailConfirmation` |
| Produtos | `LinxCommerceProducts` (`ProductID`) | `MetadataValues` (`ProductID`, `PropertyMetadataID`), `Medias` (`MediaID`), `MediaAssociations` (`ID`) | `Product`, `ProductMetadataValues`, `ProductMedia`, `ProductMediaImage`, `ProductMediaVideo`, `ProductMediaAssociations` |
| SKUs | `LinxCommerceSkus` (`ProductID`) | `MetadataValues` (`ProductID`, `PropertyMetadataID`), `ParentRelations` (`ProductID`, `ParentID`) | `SKU`, `SKUMetadataValue`, `SKUParentRelation` |
| Vendedores | `LinxCommerceSalesRepresentatives` (`SalesRepresentativeID`) | `Addresses` (`SalesRepresentativeID`, `Name`?), `Customers` (`SalesRepresentativeID`, `CustomerID`) | `SalesRepresentative` + 9 tabelas, uma por objeto |

Os nomes completos das filhas levam o nome do pai (`LinxCommerceOrdersItems`, `LinxCommerceCustomersAddresses`),
como no Totvs. Pontos a conferir na resposta real: se `OrderAddressID`, `OrderItemID` e os outros ids das filhas são
únicos no ambiente ou só dentro do pai (se forem só no pai, a chave leva o id do pai); a chave do endereço do vendedor,
que no código não tem id.

### 5.2.1 Relacionamentos 1:1: colunas no pai

Mesma regra do Totvs (§5.2.8 de lá): objeto 1:1 que chega junto com o pai e não tem ciclo de vida próprio vira
**colunas do pai**, com o nome do objeto como prefixo, na grafia da API (PascalCase: `PaymentInfoAlias`, `ContactPhone`,
`ImageRelativePath`), como as colunas `Seller*` e `OrderType*` que o pedido já tinha.

| Recurso | Objeto 1:1 | Hoje | Proposta |
|---|---|---|---|
| Pedidos | `Seller`, `MultiSiteTenant`, `OrderType`, `Customer` | Já são colunas do pedido (`SellerName`, `MultiSiteTenantBrandId`, `OrderTypeName`, `CustomerCPF`...) | Ficam, já com o prefixo |
| Pedidos | `PaymentMethods[].PaymentInfo` | Tabela `OrderPaymentInfo`, **sem** a chave do pagamento na primeira coluna (é a última) | Colunas de `LinxCommerceOrdersPaymentMethods` (`paymentInfoAlias`, `paymentInfoAuthorizationCode`...), sem os campos sensíveis (§3.2) |
| Pedidos | `OrderInvoice` | Tabela `OrderInvoice` | **Tabela própria** `LinxCommerceOrdersInvoices`: tem chave própria (`OrderInvoiceID`), o `XML` e o PDF da nota, e muda depois do pedido (é o que a escrita de rastreio envia) |
| Clientes | `Contact`, `EmailConfirmation` | Tabelas `CustomerContact` e `CustomerEmailConfirmation`, chave `CustomerID` | Colunas de `LinxCommerceCustomers` (`contactPhone`, `emailConfirmationStatus`...) |
| Produtos | `Medias[].Image`, `Medias[].Video` | Tabelas `ProductMediaImage` e `ProductMediaVideo`, as duas preenchidas para **toda** mídia (§8 item 9) | Colunas de `LinxCommerceProductsMedias` (`imageRelativePath`, `videoUrl`...), nulas quando a mídia não é daquele tipo |
| Vendedores | `Contact`, `Identification`, `Commission`, `MaxDiscount`, `WebSiteSettings`, `Portfolio`, `ShippingRegion` | 7 tabelas com chave `SalesRepresentativeID` | Colunas de `LinxCommerceSalesRepresentatives` (`contactEmail`, `identificationDocumentNumber`, `commissionTotalCommission`, `portfolioHasPortfolio`...); as listas de dentro (`Portfolio.Customers`, `ShippingRegion.PointOfSales`, `WebSiteSettings.WebSites`) viram filhas ou texto (a conferir) |

### 5.3 Procedures de sincronização

Geradas por `dbo.sp_Create_Merge_Procedure` no schema `linxcommerce`, uma por tabela, na ordem pai → filhas. Nas
filhas, `@delete_condition` apaga o que saiu da API quando o pai veio no lote, pelo mesmo `integrated_or_updated_at`
(igual a `MovideskServicesCategories` e às filhas do Totvs). Hoje um item removido do pedido, um endereço apagado do
cliente ou uma mídia retirada do produto ficam no banco para sempre.

### 5.4 Scripts DbUp (`Infrastructure.LinxCommerce/Scripts/SQLServer`)

Os scripts `001` e `002` atuais não criam nada (o `002` é só comentário; o `001` insere o app 70). A numeração é
refeita no formato do Totvs: cada service traz o seu **trio** de scripts (tabelas, procedures, jobs), e nenhum script
executado é editado depois.

| Script | Conteúdo | Entra com |
|---|---|---|
| `Script001_Create_LinxCommerce_Schema_And_Parameters_Table.sql` | Schema `linxcommerce` + `LinxCommerceParameters` | Base (fase 1) |
| `Script002_Insert_LinxCommerce_Values_Into_Apps_Table.sql` | App `70 - LinxCommerce` (o `Script001` atual) | Base |
| `Script003_Insert_Values_Into_LinxCommerce_Parameters_Table.sql` | Cliente, URL e credenciais; fora do git, a partir do `.sql.example` | Base (o `.sql` real só quando houver credenciais) |
| `Script004_Create_LinxCommerce_Orders_Tables.sql` | Pedidos, filhas e status | Service 1 |
| `Script005_Create_LinxCommerce_Orders_Sync_Procedures.sql` | | Service 1 |
| `Script006_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql` | Jobs e procedures (§7) | Service 1 |
| `Script007`… | Mesmo trio por service, na ordem da §9 | |

O `Script002_Insert_LinxCommerce_Values_Into_Jobs_Table.sql` atual sai. Os três `migrator.csx` têm o mesmo erro do
Movidesk e do AfterSale (`PostgreSQL/migrator.csx` chama `MySqlDatabase`); fica anotado, fora do
escopo.

### 5.5 Log de requisições (`linxcommerce.LinxCommerceRequests`)

Entra com as rotas sob demanda (fase 3, §9), igual à `MovideskRequests`/`TotvsRequests`: só inserção, com
`request_body` preenchido (as buscas são `POST` com filtro) e **sem** o cabeçalho de autenticação. Com as escritas
(§4.6), registra também cada escrita, com o `status_code`.

---

## 6. Infrastructure.LinxCommerce e host

### 6.1 `DependencyInjection.cs`

* Namespace `Infrastructure.LinxCommerce.DependencyInjection` (mantido).
* **Um** `HttpClient`/`IApiClient` com a chave `"LinxCommerceAPI"`, timeout de 20 min (mantido), **sem `BaseAddress`**:
  a URL de cada cliente vem de `LinxCommerceParameters` (§5.1). Sai o `configuration["LinxCommerce:BaseUrl"]!`, que
  derruba o host na inicialização se a chave não existir.
* Uma chamada a `AddValidatorsFromAssemblyContaining` (hoje 36, §4.3).
* Registrar `ILinxCommerceService`, os 4 services, os repositórios e o `ILinxCommerceCommandHandler`.

### 6.2 Repositórios

* `ISharedRepository` no lugar de `IIntegrationsCoreRepository`.
* `BulkInsertIntoTableRaw(table, "untreated")` e `CallDbProcMerge("linxcommerce", tabela, execução)`, **com `await`**
  (hoje o `SKURepository` chama as 9 merges sem `await`, §8 item 3).
* Um método de gravação por recurso, com o bulk e a merge do pai e de cada filha, preenchendo o `DataTable` pelas
  propriedades da entidade (hoje `Rows.Add` posicional com 40 a 70 argumentos por linha, que quebra em silêncio se a
  ordem das colunas mudar).
* Saem os `InsertRecord` (um por registro, com `{jobParameter.tableName}_raw` **sem schema** no SKU), a segunda
  sobrecarga de `OrderRepository.BulkInsertIntoTableRaw` (cópia da primeira sem `OrderProperties` e sem execução),
  o `GetLostOrdersRegisters` e a `SqlConnection` aberta à mão no `SalesRepresentativeRepository`.

### 6.3 Host WebApi

| Arquivo | Alteração |
|---|---|
| `WebApi.csproj` | Referenciar `Application.LinxCommerce` e `Infrastructure.LinxCommerce` |
| `Extensions/ServicesExtensions.cs` | `using Infrastructure.LinxCommerce.DependencyInjection;` + `AddScopedLinxCommerceServices(builder.Configuration)` |
| `Extensions/RecurringJobs/LinxCommerceRecurringJobsExtensions.cs` | Apagado (todo comentado, com `using` do Microvix). A recorrência passa a ser a do Movidesk e do Totvs: o intervalo de cada job em `auditing.Jobs.IntervalTimeInMinutes` (§7) |
| `appsettings.json` | Sai a seção `LinxCommerce` (a URL vai para a tabela de parâmetros) |
| `Controllers/LinxCommerce/LinxCommerceController.cs` | Reescrito, `partial` por service (abaixo) |

### 6.4 `LinxCommerceController`

Rota base `LinxCommerceJobs/LinxCommerce` (mantida), `[Authorize]` **comentado** e `IgnoreApi` em `RELEASE` no nível
da classe, como no Movidesk, no Totvs e no Microvix (hoje o `[Authorize]` está ativo e o `IgnoreApi` é por ação),
`[Tags]` com o nome do service e resposta `Ok("Records integrated successfully.")`.

Uma rota `POST` por leitura recorrente, com o nome do método: `POST LinxCommerceJobs/LinxCommerce/SearchOrders`,
`.../SearchOrderStatus`, `.../SearchCustomer`, `.../SearchProduct`, `.../SearchSKU`, `.../SearchSalesRepresentative`.
Um arquivo por service (`LinxCommerceController.Orders.cs`, `LinxCommerceController.Catalog.cs`...), cada um injetando
só o seu service.

Saem as rotas `*ByQueue`, `IntegrityLockOrderItem`, as duas escritas (voltam com a §4.6) e o
`LinxCommerceJobParameter` montado no construtor a partir do `appsettings`. As rotas sob demanda (`GetOrderByNumber`,
`GetCustomer`, `GetSKU`) voltam na fase 3 no formato novo.

---

## 7. Auditoria (jobs)

Hoje o `EnumJob` do módulo, o `Script002` (comentado) e os services **não batem**:

| Item | `EnumJob` do módulo | `Script002` | Service usa |
|---|---|---|---|
| Pedidos | `LinxCommerceOrders` = 7000 | 7000 `Order` | `Domain.Core.Enums.EnumJob.LinxCommerceOrders` (por data **e** por fila) |
| Status de pedido | — | — | `LinxCommerceOrdersStatus`, que o enum do módulo não tem |
| Produtos | 7009 | 7009 `Product` | idem, por data e por fila |
| Vendedores | 7012 | 7012 | |
| SKUs | 7013 | 7013 `Sku` | |
| Clientes | 7016 | 7016 `Customers` | |
| Rastreio (escrita) | 7017 | 7017 | |
| Carteira de vendedores (escrita) | 7018 | 7018 | |

No script, as procedures têm `IdParent` **7000** (o job de pedidos) mesmo quando são de clientes, produtos ou
vendedores; 47 das 59 estão sem `IdParent` (a linha começa com `(, 7504, ...`); e há ids repetidos para a mesma
tabela (`OrderItem` em 7503 e 7533, `Product` em 7510 e 7539, `OrderStatus` em 7508 e 7536, `OrderPaymentMethod` em
7506 e 7535).

**App 70 (mantido).** Os jobs ficam nas faixas que o próprio `Script002` reservava:

| Faixa | Uso |
|---|---|
| 7000–7499 | Integrações, um id por método: `SearchOrders` = 7000, `SearchOrderStatus` = 7001, `SearchCustomer` = 7002, `SearchProduct` = 7003, `SearchSKU` = 7004, `SearchSalesRepresentative` = 7005; escritas a partir de 7100 |
| 7500–7999 | Procedures (`sp_LinxCommerce<Tabela>_Sync`, `IdType = 1`, `IdParent` = job de origem) |

Os ids de hoje saem do `EnumJob` (nenhum foi gravado em `auditing.Jobs`: o script nunca rodou descomentado). Cada
job só entra no banco junto com o seu service.

**Intervalos (proposto).**

| Intervalo | Jobs |
|---|---|
| 3 min | `SearchOrders` (como hoje) |
| 60 min | `SearchCustomer`, `SearchProduct`, `SearchSKU` |
| 1440 min (1 dia) | `SearchOrderStatus`, `SearchSalesRepresentative` |

As procedures de cada job usam o mesmo intervalo do job, como no Movidesk. Os intervalos são ajustáveis depois por
um `update` em `auditing.Jobs`, sem mudar o código.

---

## 8. Erros de comportamento encontrados (corrigir junto)

Os caminhos são relativos à pasta do módulo; `S` = service, `R` = repositório, `E` = entidade.

| # | Onde | Problema |
|---|---|---|
| 1 | `SalesRepresentativeService.SaveSalesRepresentative` S:58-151 | Regrava **todos** os vendedores com `RemoveNotInformedCustomers = true` e `Portfolio.Customers` vazio, `RemovePhoto`, `RemoveNotInformedAddresses` e `RemoveNotInformedUsers` ligados: apaga carteira, foto, endereços e usuários de todos a cada execução. `PortfolioCommission` e `GeneralCommission` recebem o mesmo `Commission` (§4.6) |
| 2 | Projetos | Não compilam: `newbloomers-core-dlls` não existe; pacotes sem versão; tipos ausentes no Shared (§1) |
| 3 | `SKURepository` R:45-53, R:101-123 | As 9 chamadas a `CallDbProcMerge` **sem `await`**: as merges rodam em paralelo com o bulk seguinte, e uma falha é perdida. O `catch` do produto faz `throw new Exception()`, que joga fora a mensagem e a pilha |
| 4 | `OrderRepository.InsertIntoUpdateTrackingNumberOrderTable` R:424-436 | Grava em `[linx_commerce].[{tableName}]` (a tabela final, não o `untreated`) e chama a merge de **`Customer`** |
| 5 | `OrderService.UpdateTrackingNumberOrder` S:473-487 | Considera enviado qualquer retorno não nulo: o `PostAsync` devolve o corpo também no erro, então um pedido recusado é marcado como enviado e nunca é reenviado. `Thread.Sleep(5000)` bloqueia a thread |
| 6 | `CustomerRepository.BulkInsertIntoTableRaw` R:23-57 | `catch { continue; }` depois de já ter adicionado a linha do cliente: um erro numa filha grava o cliente pela metade, sem mensagem |
| 7 | `CustomerRepository`, `Person` E:20 | `Password` e `CustomerHash` do cliente gravados no banco (§3.2) |
| 8 | `OrderRepository` R:95, R:289 | `PaymentInfo.lastupdateon` sem checar nulo: um pagamento sem `PaymentInfo` derruba o lote inteiro com `NullReferenceException`. Grava portador, dica do cartão e chave PIX (§3.2) |
| 9 | `SKURepository` R:90-92, `Midia` E:38-39 | Cada mídia gera uma linha em `ProductMediaImage` **e** em `ProductMediaVideo`; o construtor faz `new Video(midia.Video, ...)` e `new Image(midia.Image, ...)` sem checar nulo (a conferir se lança exceção) |
| 10 | `SearchCustomerByDateInterval`, `SearchSKUByDateInterval`, `SearchProductByDateInterval` | Filtram por `CreatedDate`: uma alteração em cliente, SKU ou produto já existente **nunca** é lida (§4.2) |
| 11 | Mesmos métodos, filas, `SearchOrderStatus`, `SearchSalesRepresentative` | Sem paginação: leem só a página 0 (1000 ou 10000); o DTO nem tem `Page` (§4.2, item 4) |
| 12 | `SearchCustomerByDateInterval` S:228-283, `SearchOrderStatus` S:376-425 | Cache do **hash da resposta inteira**, sem a empresa na chave: nenhuma mudança na resposta não regrava nada, mesmo que a gravação anterior tenha falhado (§4.4) |
| 13 | `SearchOrdersByDateInterval` S:175-194 | Cache criado com os ids da primeira execução e nunca recarregado; `<` no lugar de `<=` rebusca pedidos sem alteração; `IN` sem lotes (§4.4) |
| 14 | Leituras por fila (`*ByQueue`) | Retiram da fila todos os itens lidos, inclusive os que falharam; `QueueID = 21` fixo para tudo; uma página só (§4.2) |
| 15 | `SearchSKUByQueue` S:302-320 | Filtra a fila por `EntityKeyName == "CatalogItemID"` no código, sem o `EntityTypeName` do `Where`: pode trazer itens de outro tipo de catálogo |
| 16 | `SearchCustomerByQueue` S:326 × `SearchCustomerByDateInterval` S:239 | O tipo do cliente é `80` num e `"P"` no outro (o código ASCII de `P`): a conferir qual a fila devolve |
| 17 | `SearchOrderStatus` S:402-410 | Chama `GetOrderStatus` para cada status e descarta a resposta (`orderStatus` não é usado) |
| 18 | `GetCustomer` S:114 | Grava com `new Guid()` (vazio) como execução, sem abrir o log; só o primeiro cliente da busca |
| 19 | `GetSKU`, `GetSalesRepresentative` | Chamam a API e não gravam (gravação comentada) |
| 20 | `SKUService` S:23, `SalesRepresentativeService` S:21 | `_errorMessagesList` **estática**: compartilhada entre execuções simultâneas |
| 21 | Todos os métodos | Sem `try/finally`: uma exceção deixa o log aberto. Erro HTTP vira `NullReferenceException` (§4.2, item 6) |
| 22 | `OrderRepository.GetTrackingNumbersToUpdate` R:462-487, `GetLostOrdersRegisters` R:489-503 | Regras fixas: só JadLog, data `'2025-01-01'`, status `6, 20, 22, 32, 33` da Newbloomers |
| 23 | `OrderService.SearchOrdersByDateInterval` S:235 | `AddRecord` do pedido usa o `CustomerID` como chave do registro, não o `OrderID` |
| 24 | `DependencyInjection` | Validadores registrados 36 vezes; `BaseUrl` com `!` derruba a inicialização sem a chave (§6.1) |
| 25 | `EnumJob` × `Script002` × services | Enum do módulo sem uso; ids sem `IdParent` e repetidos no script (§7) |
| 26 | Entidades | `lastupdateon` por registro com fuso só do Windows (`E. South America Standard Time`): em Linux, `TimeZoneNotFoundException` |
| 27 | Controller (comentado) | Lia `LinxMicrovix:ParametersDateInterval`, de outro módulo (§5.1) |

---

## 9. Plano em fases

Como no Totvs e no Microvix: **um service de cada vez**. A fase 1 deixa a base pronta; a fase 2 se repete para cada
um dos 4 services, que só é ligado no host e no controller quando está pronto.

**Sem credenciais.** A fase 1 não depende da API. A fase 2 foi **escrita** a partir do código antigo e testada contra
uma API simulada (abaixo), mas os pontos "a conferir" (campos do DTO, chave das tabelas, filtro por `ModifiedDate`,
paginação e o ponto 5 da §4.2) só se fecham com uma resposta real. `auditing.Jobs` não tem coluna de ativação (e o
host não registra job recorrente): o que impede a leitura sem credencial é `linxcommerce.LinxCommerceParameters` vazia,
com a qual cada job só registra "Nenhum cliente" e termina.

**Fase 1 — base:**

* `.csproj` como na §2.1 (Shared, versões);
* código antigo que não compila fica **fora do build** (`<Compile Remove>` com comentário, como no Totvs) e volta
  service a service; os 48 métodos vazios, as interfaces e classes vazias (`PaymentTerm`, `Remarks`, `Shipment`),
  `LinxCommerceJobParameter`, as leituras por fila e o `IntegrityLock` são apagados;
* `ILinxCommerceService`/`LinxCommerceService` (§4.1: lote, clientes, autenticação e última data),
  `Entities/Parameters/Company`, `ILinxCommerceRepository`, `LinxCommerceCommandHandler`;
* `DependencyInjection` com o cliente `"LinxCommerceAPI"` sem `BaseAddress` e uma chamada de validadores;
* scripts `Script001` a `Script003` (o `003` só como `.sql.example`); `EnumJob` só com `None` e o comentário novo;
* host: referências, `AddScopedLinxCommerceServices`, seção `LinxCommerce` fora do `appsettings`, controller `partial`
  só com a rota base; `LinxCommerceRecurringJobsExtensions` apagado;
* `dotnet build` do host Hangfire com 0 erros.

**Fase 2 — um service por vez**, na ordem:

1. `LinxCommerceOrdersService` (`SearchOrders`, `SearchOrderStatus`): é o recurso que o Oto e a Stone leem, e o único
   que já pagina e filtra por `ModifiedDate`;
2. `LinxCommerceCustomersService` (`SearchCustomer`);
3. `LinxCommerceCatalogService` (`SearchProduct`, `SearchSKU`);
4. `LinxCommerceSalesRepresentativesService` (`SearchSalesRepresentative`, só leitura).

Cada um: DTOs, entidades, validadores, repositório, queries, o trio de scripts, as rotas no controller e o
`dotnet build`; com credencial, a conferência com a resposta real e a cópia anonimizada das respostas em
`Responsed Jsons`, como no Totvs.

**Fases 1 e 2 implementadas (29/10/2026), os 4 services de uma vez:**

* **Código antigo apagado** (recuperável pelo histórico do git), sem `<Compile Remove>`: os 7 services e as 7 interfaces
  (com os 48 métodos vazios), os 4 repositórios, as 54 entidades e respostas, os 39 arquivos de `Domain/CustomValidations`,
  `LinxCommerceJobParameter`, as leituras por fila, o `IntegrityLock`, as duas escritas, os scripts `001`/`002` antigos, o
  `LinxCommerceController` comentado e o `LinxCommerceRecurringJobsExtensions`.
* **Projetos:** `.csproj` com `..\..\..\shared\*.Shared` e `Newtonsoft.Json` 13.0.4 / `Caching.Memory` 10.0.8, como no Totvs.
* **Domain:** DTOs em `Dtos/Reads/<Assunto>` (texto, `{ get; init; }`, sem os campos sensíveis), `Dtos/Reads/Base/SearchResult`
  (Page + Result), 25 entidades em `Entities/Reads/<Assunto>` construídas de `(doc_company, batchTime, dto)`, `Company`,
  `EnumJob` (7000–7005) e `Extensions/LinxCommerceConverters` (data com fuso ou `/Date(...)/` do WCF, `Int64`, `Guid` e listas
  de ids como texto).
* **Application:** `LinxCommerceService` (lote, clientes com CNPJ validado por `^\d{14}$`, filtro da janela, busca
  paginada e `Get` com o Basic do cliente, uma nova tentativa em 429/502/503/504, e o JSON de `auditing.Records` sem os
  campos sensíveis); `LinxCommerceOrdersService` (`SearchOrders`, `SearchOrderStatus`), `LinxCommerceCustomersService`
  (`SearchCustomer`), `LinxCommerceCatalogService` (`SearchProduct`, `SearchSKU`) e `LinxCommerceSalesRepresentativesService`
  (`SearchSalesRepresentative`); `LinxCommerceCommandHandler`; 25 validadores em `Validations/<Assunto>`, com os tipos e
  tamanhos dos scripts. A busca paginada e o `Get` ficaram no comum (como o `ReadOnDemand` do Totvs); a janela, o cache,
  os `Get` de cada registro, a validação e a gravação ficam em cada método.
* **Infrastructure:** `DependencyInjection` com o cliente `"LinxCommerceAPI"` sem `BaseAddress` e uma chamada de
  validadores; 5 repositórios com bulk em `untreated` e merge em `linxcommerce`, pai antes das filhas.
* **Scripts:** `Script001` (schema + parâmetros), `Script002` (app 70), `Script003_..._Table.sql.example` (o `.sql` no
  `.gitignore`) e os trios `004`–`006` (pedidos e status), `007`–`009` (clientes), `010`–`012` (produtos e SKUs) e
  `013`–`015` (vendedores): 25 pares de tabelas, 25 procedures (filhas com `@delete_condition` pelo pai) e 6 jobs + 25
  procedures em `auditing.Jobs` (7000–7005 e 7500–7524).
* **Host:** referências no `Hangfire.csproj`, `AddScopedLinxCommerceServices` ligado, seção `LinxCommerce` fora do
  `appsettings.json` e `LinxCommerceController` `partial` com as 6 rotas `POST` (`LinxCommerceController.Orders.cs`,
  `.Customers.cs`, `.Catalog.cs`, `.SalesRepresentatives.cs`).
* **Diferenças do desenho acima, decididas na implementação:** a janela dos clientes sai de
  `MAX(integrated_or_updated_at)` (o cliente não tem data de alteração no código antigo); o `SearchOrderStatus` usa a
  própria busca e não chama mais o `GetOrderStatus`; o endereço do vendedor, sem id na API, tem a posição na lista
  (`AddressIndex`) como chave; os pacotes levam o `OrderID` na chave para o merge apagar pelo pedido; `base_url` precisa
  ser `https`, salvo `http://localhost` (a API simulada dos testes).

**Verificado (29/10/2026):**

* `dotnet build` do host Hangfire: **0 erros**; nenhum aviso de código nos arquivos do módulo (só os NU1902/NU1903 que
  chegam pelo `Infrastructure.Shared`, como no Totvs).
* **Banco:** no SQL Server local em docker (o mesmo da validação do Totvs), os 14 scripts rodaram sem erro; ficaram
  26 tabelas em `linxcommerce` e 25 em `untreated`; as 25 procedures executam sem erro com as tabelas vazias; os 6 jobs têm
  o título do `EnumJob` e todo job de procedure tem a sua procedure.
* **Host contra uma API simulada** (`http://localhost`, no formato do código antigo, com uma linha de teste em
  `LinxCommerceParameters`), as 6 rotas responderam 200 e gravaram pai e filhas: paginação (duas páginas no
  `SearchOrders`), `GetPerson`/`GetCompany` pelo tipo, datas ISO com fuso e `/Date(...)/`, objetos 1:1 em colunas, listas
  de ids como texto, nenhum campo sensível nas tabelas nem em `auditing.Records`, e todos os logs fechados. Na segunda
  rodada: sem alteração, nenhum `GetOrder` (cache); um pedido alterado com um item a menos teve o item apagado pelo merge,
  sem tocar no outro pedido; um erro 500 no `GetCompany` foi para o log e **nada** daquele cliente foi gravado.
* Depois do teste, o banco local voltou ao estado anterior (schema, tabelas, procedures, jobs, app e registros de
  auditoria do módulo apagados): os scripts rodaram pelo `sqlcmd`, fora do journal do DbUp, e ficariam duplicados.

**Não verificado:** tudo o que depende da API real: nomes e tipos dos campos, envelope e `PageCount` das buscas, o
filtro por `ModifiedDate` no `SearchCustomer` e no `SearchSalesRepresentative`, o formato das datas, o corpo dos `Get`,
as chaves das filhas e se o `SearchOrders` já traz o pedido completo (§4.2). A carga inicial de pedidos desde
`2025-01-01` faz um `GetOrder` por pedido numa execução só, sem fatias: o tempo e a memória dela também só se medem com a
API real.

**Fase 3 — sob demanda e escritas:** `GetOrder`/`GetOrderByNumber`, `GetCustomer`, `GetProduct`/`GetSKU`,
`GetSalesRepresentative` com `X-User-Name` e `LinxCommerceRequests` (§5.5); as escritas só depois da pendência 1 da
§10.2 e de teste em homologação.

**Fase 4 — README** do módulo no formato do Totvs e do Microvix.

**Fases 3 (só as leituras sob demanda) e 4 implementadas (29/10/2026):**

* **Rotas `GET` com `X-User-Name`** (até 100 caracteres) e `docCompany` por `^\d{14}$`, no desenho do Totvs:
  `Orders/{docCompany}/{orderId:guid}`, `Orders/{docCompany}/Number/{orderNumber}` (`^[A-Za-z0-9._-]{1,50}$`),
  `Customers/{docCompany}/{customerId}`, `Products/{docCompany}/{productId}`, `Skus/{docCompany}/{productId}` e
  `SalesRepresentatives/{docCompany}/{salesRepresentativeId}` (ids `int:min(1)`). Respostas: 200 gravado, 404 não existe, 422
  cliente não cadastrado ou resposta inválida, 400 de validação.
* **Services:** `GetOrder`, `GetOrderByNumber`, `GetCustomer` (a busca por `CustomerID` diz o tipo; depois `GetPerson` ou
  `GetCompany`), `GetProduct`, `GetSKU` e `GetSalesRepresentative`, com o contrato `true`/`false`/`null` do Totvs. Gravam sempre,
  pelo mesmo método do job (as gravações de produto, SKU e vendedor saíram do corpo do job para métodos privados), e atualizam o cache
  do job quando ele está carregado. "Não existe" é o 404 ou a resposta 2xx sem o registro pedido (a conferir).
* **Comum:** `LinxCommerceService.GetCompany` e `ReadOnDemand`, que devolve status, DTO e JSON e grava cada chamada em
  `linxcommerce.LinxCommerceRequests`: rota `v1/...` sem o endereço do cliente nem a autenticação, corpo enviado, resposta sem os
  campos sensíveis, `is_success` (2xx com JSON válido) e `status_code` (0 na exceção). O envio HTTP com nova tentativa ficou num
  `Send` privado, usado pelas leituras recorrentes e pelas sob demanda.
* **Scripts:** `Script016_Create_LinxCommerce_Requests_Table.sql` (já com `status_code`) e
  `Script017_Insert_LinxCommerce_On_Demand_Jobs_Into_Jobs_Table.sql` (jobs 7010–7015, `IntervalTimeInMinutes = 0`); `EnumJob` com os
  6 valores novos.
* **README** em `src/Backend/Modules/LinxCommerce/README.md`.
* **As escritas continuam fora** (pendência 1 da §10.2).

**Verificado (29/10/2026):** `dotnet build` do host com 0 erros e nenhum aviso de código no módulo; os 16 scripts rodaram sem erro no
SQL Server local (37 jobs em `auditing.Jobs`); contra a API simulada, 14 de 14 chamadas com a resposta esperada: as 6 rotas com
registro existente (200, gravado pai e filhas), pedido por id e por número, cliente e o 404 dos inexistentes, sem `X-User-Name` (400),
CNPJ com 13 dígitos (400), número de pedido com aspas (400), cliente não cadastrado (422, sem chamada à API) e id zero (404 da rota).
`LinxCommerceRequests` registrou as 10 chamadas à API com a chave, o status e nenhum campo sensível; os logs ficaram fechados. Depois do
teste, o banco local voltou ao estado anterior. **Não verificado:** como o Linx Commerce responde um id inexistente e o corpo real do
`GetOrderByNumber`.

---

## 10. Decisões

### 10.1 Tomadas

Nenhuma confirmada por quem usa o módulo. A implementação de 29/10/2026 (§9) seguiu as recomendações das pendências 1
a 6 abaixo: escritas não migradas, leitura só por data de alteração, cargas iniciais de `2025-01-01` (pedidos) e
`2000-01-01` (o resto), sem campos sensíveis, schema `linxcommerce` com o legado intocado e os métodos vazios apagados.
Mudar qualquer uma delas é uma alteração pequena e localizada.

### 10.2 Pendentes

| # | Pendência | Recomendação |
|---|---|---|
| 1 | Escritas: `SaveSalesRepresentative` apaga a carteira de todos os vendedores; `UpdateTrackingNumberOrder` depende da JadLog | Desligar as duas (já estão, o host não tem o módulo) e não migrar até alguém dizer qual é a regra da carteira e de onde vem o rastreio. Confirmar se o efeito de zerar a carteira era intencional |
| 2 | Leitura por fila (`Queue`) | Só por data de alteração (§4.2). A fila volta se a leitura por data não cobrir algum caso, com paginação e retirada só do que foi gravado |
| 3 | Carga inicial (banco vazio) por recurso | Pedidos desde `2025-01-01`, como as notas e pedidos do Totvs; clientes, produtos, SKUs, status e vendedores, tudo (desde `2000-01-01`), em fatias de 30 dias se a busca limitar |
| 4 | Campos sensíveis (`Password`, `CustomerHash`, dados de cartão e PIX) | Não gravar, nem em `auditing.Records` (§3.2) |
| 5 | Schema novo × consumidores do legado (Oto, Stone) | Schema `linxcommerce` com os nomes novos, legado intocado; a adaptação do Oto e da Stone é decidida à parte (§5.0) |
| 6 | Métodos da API que o módulo declara e não usa (48) | Apagar; cada um entra quando houver uso e credencial para conferir |
| 7 | Credenciais | Pedir à Linx ou a quem administra o ambiente da Misha um usuário e chave (de preferência de homologação) para conferir os pontos "a conferir" deste documento antes de ligar qualquer job |
| 8 | O Linx Commerce ainda é usado na Newbloomers? | Confirmar. Se foi descontinuado como o Microvix, o módulo segue a decisão 1 do Microvix (recorrente, para qualquer cliente), e a falta de credenciais deixa a fase 2 só escrita, sem ligar |
