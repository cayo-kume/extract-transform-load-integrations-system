# Proposta — padronização do módulo Oto (padrão Movidesk / AfterSale)

Documento para revisão. **Nada foi alterado no código ainda.**

Objetivo: listar tudo o que precisa mudar em `src/Backend/Modules/General/Oto` (e nos pontos do host que o
referenciam) para que o módulo siga o mesmo padrão dos módulos **Movidesk** e **AfterSale**, já
considerando a documentação atual da API ([Oto Data API 1.3.5](https://data-api.otocrm.com.br/docs)).

Diferença de natureza que orienta todo o documento: Movidesk e AfterSale **leem** uma API e gravam no
banco. O Oto faz o contrário. Ele **lê o banco** (tabelas `LINX_MICROVIX_ERP`, `LINX_MICROVIX_COMMERCE` e
`LINX_COMMERCE`) e **escreve** na API do Oto, por upsert e em lote. Por isso o padrão é aplicado nas mesmas
camadas e convenções, mas a parte de "escrita" do Movidesk/AfterSale (DTOs `Writes`, `SendAsync`, log de
requisições) é a referência principal, não a de leitura.

---

## 1. Situação atual (inventário)

| Item | Hoje |
|---|---|
| Projetos | `Domain.Oto`, `Application.Oto`, `Infrastructure.Oto` (estão no `.slnx`) |
| Referências | `..\..\..\..\..\newbloomers-core-dlls\src\*.Core` — **pasta que não existe mais** (os outros módulos usam `src/Backend/Shared`) |
| Host | `WebApi.csproj` não referencia o Oto. `AddScopedOttoServices` está comentado em `ServicesExtensions.cs` e o `OttoController` está inteiro comentado |
| Service | `OtoService` com `SendCustomers`, `SendCustomersCashBack`, `SendProducts`, `SendOrders`, `SendStores`, `SendSellers` |
| Montagem do JSON | `Factories/BuildObjects.cs`, com `JObject` montado à mão e todos os valores interpolados como string |
| Controle de envio | `general.OttoSentRecordsLog` (grava o que já foi enviado; a query exclui o que já está lá) |
| Credenciais | `general.PARAMETROS_OTTO` (`USERNAME`, `PASSWORD`), com um login JWT por chamada |
| Multimarca | Separação por `parameter.username == "misha"` e por `Contains` em textos (`"MI-"`, `"MISHA"`, `"MNR"`, `"NEWFIT"`, `"Open Era"`) |
| Scripts | Só o insert em `auditing.Apps`. O insert de jobs está **comentado**. Não há script para `OttoSentRecordsLog` nem para `PARAMETROS_OTTO` |
| Documentação | Sem `README.md` e sem pasta de JSONs de referência |

Conclusão: o módulo **não compila** contra o Shared atual. As entidades herdam bases (`ClientBase`,
`OrderBase`, `ProductBase`, `SellerBase`, `CompanyBase`) cujas propriedades antigas (`Code`, `Reason`, `Doc`,
`Phone`, `Number`, `Sku`, `Description`, `UnitaryValue`...) não existem nas bases de `Domain.Shared`. A
migração é, na prática, uma reescrita das entidades. As queries e as regras de negócio podem ser
reaproveitadas.

---

## 2. Projetos, referências e nomes

### 2.1 `.csproj`

| Projeto | Hoje | Padrão (Movidesk) |
|---|---|---|
| `Domain.Oto` | `Newtonsoft.Json` sem versão + `newbloomers-core-dlls\...\Domain.Core` | `Newtonsoft.Json` **13.0.4** + `..\..\..\..\shared\Domain.Shared\Domain.Shared.csproj` |
| `Application.Oto` | `...\Application.Core` | `Microsoft.Extensions.Caching.Memory` **10.0.8** + `..\..\..\..\shared\Application.Shared\Application.Shared.csproj` |
| `Infrastructure.Oto` | `Microsoft.Extensions.Http` + `...\Infrastructure.Core` | sem pacote, `..\..\..\..\Shared\Infrastructure.Shared\Infrastructure.Shared.csproj` |

### 2.2 Grafia "Oto" × "Otto"

A marca é **Oto**. Hoje as duas grafias se misturam. Tudo passa a ser `Oto`:

| Hoje | Passa a ser |
|---|---|
| `AddScopedOttoServices` | `AddScopedOtoServices` |
| chave/`HttpClient` `"OttoAPI"` | `"OtoAPI"` |
| `OttoController`, rota `OttoJobs/Otto` | `OtoController`, rota `OtoJobs/Oto` |
| `general.OttoSentRecordsLog` | `general.OtoSentRecords` (ver §5.2) |
| `general.PARAMETROS_OTTO` | `general.OtoParameters` (ver §5.1) |
| campo `name_otto` enviado à API | `name_oto` (o nome do campo na API; hoje o valor **é ignorado**) |

### 2.3 Namespaces e pastas

Os namespaces hoje não batem com as pastas. O padrão é namespace = caminho da pasta:

| Arquivo | Namespace hoje | Padrão |
|---|---|---|
| `Abstractions/Handlers/Commands/IOtoCommandHandler.cs` | `Application.Oto.Interfaces.Handlers.Commands` | `Application.Oto.Abstractions.Handlers.Commands` |
| `Abstractions/Services/IOtoService.cs` | `Application.Oto.Interfaces.Services` | `Application.Oto.Abstractions.Services` |
| `Abstractions/Repositorys/IOtoRepository.cs` | `Domain.Oto.Interfaces.Repositorys` | `Domain.Oto.Abstractions.Repositorys` |
| `Entities/*.cs` | `Domain.Oto.Models` (e alguns `Domain.Oto.Entities`) | `Domain.Oto.Entities.<Pasta>` |
| `DependencyInjection.cs` | `Infrastructure.Oto` | `Infrastructure.Oto.DependencyInjection` |

Também saem os `using` sem uso (`System.Linq`, `System.Text` etc. em `Response.cs` e `SentRecordLog.cs`) e o
`using Application.Oto.Factories` do `DependencyInjection.cs`.

---

## 3. Domain.Oto

### 3.1 Estrutura proposta

```
Domain.Oto/
├── Abstractions/Repositorys/
│   ├── IOtoRepository.cs           (empresas/marcas, controle de envio, log de requisições)
│   ├── IOtoCustomersRepository.cs  (origem de clientes e cashback)
│   ├── IOtoOrdersRepository.cs     (origem de pedidos e itens)
│   └── IOtoCatalogRepository.cs    (origem de produtos, lojas e vendedores)
├── Dtos/
│   └── Writes/                     ← corpo das rotas da Oto Data API (um arquivo por classe)
│       ├── Base/
│       │   ├── DataRequest.cs      (DataRequest<T> { data: List<T> })
│       │   └── SuccessResponse.cs  (success, requestId)
│       ├── Auth/                   (LoginRequest, TokenResponse)
│       ├── Customers/Customer.cs
│       ├── Orders/Order.cs
│       ├── OrderItems/OrderItem.cs
│       ├── Products/Product.cs
│       ├── Stores/Store.cs
│       ├── Sellers/Seller.cs
│       ├── Cashback/Cashback.cs
│       └── Errors/                 (HTTPValidationError, ValidationError: corpo do 422)
├── Entities/
│   ├── Parameters/Company.cs       (→ general.OtoParameters)
│   ├── Sources/                    ← linhas lidas do ERP pelas queries (nomes = colunas do SELECT)
│   │   ├── Customer.cs, Order.cs, OrderItem.cs, Product.cs, Store.cs, Seller.cs, Cashback.cs
│   └── Writes/
│       ├── SentRecords/SentRecord.cs   (→ general.OtoSentRecords)
│       └── Requests/Request.cs         (→ general.OtoRequests)
└── Enums/EnumJob.cs
```

### 3.2 Convenções que mudam

* **DTOs tipados no lugar do `BuildObjects`.** Cada rota tem uma classe em `Dtos/Writes`, com os nomes da API
  (`snake_case`) e **os tipos da API**: `bool?`, `decimal`, `int?`, `DateOnly`/`DateTime`. Hoje tudo vai como
  string (`$"{order.canceled}"` vira `"True"`, `$"{item.quantity_product}"` vira `"2"`), o que depende da
  coerção do servidor e pode dar 422.
* **`null` em vez de `""`.** Toda interpolação `$"{x}"` transforma `null` em `""`. Como o upsert da Oto
  **substitui o registro inteiro**, isso grava strings vazias por cima de dados bons. Os DTOs ficam anuláveis
  e a serialização usa `NullValueHandling.Ignore`, como o `WriteSettings` de `MovideskServicesService`.
* **Entidades de origem** (`Entities/Sources`) com os mesmos nomes das colunas do `SELECT`, sem herdar as
  bases antigas, e com os campos opcionais anuláveis (`DateTime?`, `decimal?`), como em Movidesk/AfterSale.
  Hoje `birthday_date`, `created_date` e `date` são `DateTime` não anuláveis e viram `0001-01-01` quando a
  coluna é nula.
* **Conversão Entity → DTO no construtor do DTO** (ou num método `From(entity)`), no lugar do `BuildObjects`
  estático, cortando os textos quando a API tiver limite.
* **`Parameters/Company.cs`** herda `CompanyBase` (`doc_company`), como nos outros módulos (ver §5.1).
* **`EnumJob`**: o comentário aponta para `Script002_Insert_Into_Jobs_Table.sql`, que não existe. Ele precisa
  citar o script real e ganhar os jobs que faltam (ver §7).

---

## 4. Application.Oto

### 4.1 Services

O `OtoService` faz tudo. Ele passa a ser dividido como no Movidesk (um service comum mais um por recurso):

| Service | Métodos | Observação |
|---|---|---|
| `OtoService` (comum) | `GetBatchTime`, `GetToken(company)`, `SaveRequest(...)`, `Send<T>(...)` | Fluxo comum de envio (ver §4.2), igual ao papel do `SendService`/`SendTicket` |
| `OtoCustomersService` | `SendCustomers`, `SendCashback` | O cashback passa a usar `POST /v1/cashback` (ver §8) |
| `OtoOrdersService` | `SendOrders` | Pedidos, itens e clientes do pedido |
| `OtoCatalogService` | `SendProducts`, `SendStores`, `SendSellers` | |

### 4.2 Fluxo comum de envio (`Send<T>`)

Mesmo desenho do `SendService` do Movidesk, adaptado a lotes:

1. abre um log em `auditing.Logs` com o job do método;
2. para cada marca de `general.OtoParameters`, obtém o token (com cache, ver §4.4);
3. lê da origem só o que é novo ou mudou (ver §5.2) e valida cada DTO (FluentValidation). Os inválidos viram
   mensagem em `auditing.Messages` e **não entram no lote**, porque um registro inválido faz a API recusar o
   lote inteiro com 422;
4. quebra em lotes de **até 10.000** registros (o limite da API; acima disso a resposta é 413). Hoje só o
   `SendOrders` respeita esse limite: os outros contam com o `TOP 10000` da query;
5. chama a API com `IApiClient.SendAsync`, que devolve status, corpo e cabeçalhos. Hoje o `PostAsync`
   devolve só o corpo, e o código assume sucesso: um 401, 413, 422 ou 429 vira `result.success == false`
   (ou `NullReferenceException` quando o corpo vem vazio), sem nenhuma mensagem no log;
6. grava a chamada em `general.OtoRequests` (rota, marca, status, `requestId`, quantidade, corpo da resposta);
7. com sucesso, grava os registros enviados em `general.OtoSentRecords` (com o hash de cada um) e em
   `auditing.Records`;
8. em erro, registra em `auditing.Messages` a mensagem da API (o `detail[]` do 422 é legível) e segue para o
   próximo lote ou a próxima marca. Num 429, espera o tempo indicado e tenta de novo uma vez;
9. fecha o log (`SetLogEndDate` + `CommitAllChanges`) num `finally`, como o Movidesk. Hoje uma exceção deixa
   o log aberto.

Os `try { ... } catch { throw; }` e `catch (Exception ex) { throw; }` sem efeito saem.

### 4.3 Validações (FluentValidation)

Pasta `Validations/Writes/<Recurso>`, um validador por DTO, registrados com
`AddValidatorsFromAssemblyContaining<...>()`. Regras tiradas do schema da API:

| DTO | Regras |
|---|---|
| `Customer` | `customer_id` e `create_date` obrigatórios; `gender` ∈ {M, F, O}; `type_person` ∈ {P, J}; `mobile_number` com 13 dígitos (DDI + DDD + 9 + 8) quando informado |
| `Order` | `customer_id`, `date`, `id`, `store` e `total` obrigatórios; `store_type` ∈ {E, L}; `payment_type` ∈ {V, P} |
| `OrderItem` | `date`, `id`, `store`, `sku`, `seller_id`, `quantity` e `price` obrigatórios |
| `Product` | `sku`, `id` e `name` obrigatórios; `image` e `url` como URI de até 2083 caracteres |
| `Store` | `id` e `name` obrigatórios; `type` ∈ {E, L}; `email` válido; `identity` ∈ {F, P} |
| `Seller` | `id` e `name` obrigatórios |
| `Cashback` | `customer_id`, `order_date`, `order_id`, `order_store`, `credit` e `valid_until` obrigatórios |
| Todos com código de loja | `store`, `order_store` e `id` da loja **sem** `|`, `,` ou `;` |

### 4.4 Cache (padrão `docs/PADRAO-CACHE-INTEGRACOES.md`)

| Chave | Conteúdo | Expiração |
|---|---|---|
| `Oto:Token:<doc_company>` | JWT da marca | `expires_in` da resposta menos 5 min (hoje é feito um login por chamada) |
| `Oto:SentRecords:<doc_company>:<tipo>` | Cache de estado (§3.2 do padrão): chave do registro → hash do último envio | 2 h absoluto / 30 min deslizante |

### 4.5 Command handler (queries)

* As queries ficam no `OtoCommandHandler`, como hoje, mas **recebem a marca como parâmetro** (ver §5.1) em
  vez de o service filtrar a lista com `Contains`.
* Ficam fora das queries os valores fixos: o CNPJ `38367316000199`, os depósitos e a lista de lojas excluídas
  (`'Margaux - Matriz'`, `'New Bloomers'`, lojas fechadas). Eles vão para colunas de parâmetro ou para uma tabela
  de lojas por marca (decisão em §10).
* O `GetOrders` usa o multi-mapping do Dapper com `splitOn: "Code, Code, Code"` e quatro colunas chamadas
  `CODE`. Isso é frágil e quebra com a mudança de nomes. A proposta é ler uma linha plana
  (`Entities/Sources/OrderItem`) e agrupar por pedido no service.

---

## 5. Banco de dados

### 5.1 `general.OtoParameters` (no lugar de `PARAMETROS_OTTO`)

Mesmo formato de `MovideskParameters`/`AfterSaleParameters`, com a chave da marca:

```sql
create table [general].OtoParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key,
	[doc_company] nvarchar(14) not null,  --CNPJ da marca (chave em todas as tabelas do módulo)
	brand nvarchar(50) not null,          --marca usada nos filtros das queries (ex.: MISHA, OPEN ERA)
	username nvarchar(100) null,
	[password] nvarchar(200) null,
	token nvarchar(400) null              --chave privada fixa, se a Oto fornecer (ver §10)
);
```

A documentação da API aceita os dois modos: **chave privada fixa** no `Authorization: Bearer` ou **login**
(`POST /auth/login`, JWT de 60 min). Se a Oto fornecer a chave fixa, o modelo fica igual ao do Movidesk
(`token`) e o login sai.

### 5.2 `general.OtoSentRecords` (no lugar de `OttoSentRecordsLog`)

A tabela atual é usada nas queries, mas **não tem script de criação** no módulo. Além disso, tem três problemas:

* **Um registro enviado nunca é reenviado.** A query exclui tudo o que está no log, então uma alteração de
  cadastro (e-mail, telefone, endereço, status da loja, vendedor desligado) nunca chega ao Oto. Como a API é
  upsert, reenviar é seguro.
* **A marca não entra no filtro.** Um cliente enviado para uma marca fica marcado como enviado para as duas
  (`C.DOCUMENTO = A.COD_CLIENTE AND C.TIPO = 'C'`, sem `branch`).
* **O payload inteiro é gravado por registro**, o que cresce sem limite.

Proposta: guardar a chave e o **hash** do último envio, por marca, e reenviar quando o hash mudar (é o cache de
estado do padrão, com a tabela como fonte):

```sql
create table [general].OtoSentRecords (
	integrated_or_updated_at datetime not null,
	[doc_company] nvarchar(14) not null,
	record_type varchar(3) not null,      --C, CB, P, PE, PI, L, S
	record_key nvarchar(200) not null,    --chave de deduplicação da API (ex.: date|store|id|sku)
	payload_hash char(64) not null,       --SHA-256 do JSON enviado
	request_id uniqueidentifier null,     --requestId devolvido pela Oto
constraint [PK_OtoSentRecords] primary key ([doc_company], record_type, record_key));
```

Segue o fluxo `untreated` → `general.sp_OtoSentRecords_Sync` (gerada por `dbo.sp_Create_Merge_Procedure`), como
as tabelas de dados do Movidesk, porque cada registro é atualizado a cada reenvio.

A migração da tabela antiga precisa de um script que copie as chaves já enviadas (com um hash vazio, para
forçar um reenvio único e corrigir os dados mandados com `""`) ou que comece do zero. Decisão em §10.

### 5.3 `general.OtoRequests` (log das chamadas)

Igual a `general.MovideskRequests`/`AfterSaleRequests`: só recebe inserts e vai direto para `general`. Há duas
diferenças:

* `user_name` vale `'Hangfire'` nos jobs recorrentes (ou o `X-User-Name` nas rotas sob demanda, §6);
* `request_body` pode ter 10.000 registros. A proposta é gravar só `record_count` e o corpo da **resposta**,
  porque o conteúdo de cada registro já fica em `auditing.Records`.

### 5.4 Scripts DbUp (`Infrastructure.Oto/Scripts/SQLServer`)

Os scripts atuais ainda não rodaram em nenhum banco (o módulo nem compila), então podem ser renumerados:

| Script | Conteúdo |
|---|---|
| `Script001_Create_Oto_Parameters_Table.sql` | `general.OtoParameters` |
| `Script002_Create_Oto_SentRecords_Tables.sql` | `untreated`/`general.OtoSentRecords` + índices |
| `Script003_Create_Oto_Sync_Procedures.sql` | `sp_OtoSentRecords_Sync` |
| `Script004_Insert_Oto_Values_Into_Apps_Table.sql` | App `40 - Oto` (o atual `Script001`) |
| `Script005_Insert_Oto_Values_Into_Jobs_Table.sql` | Jobs **ativos** (hoje estão comentados), ver §7 |
| `Script006_Create_Oto_Requests_Table.sql` | `general.OtoRequests` |
| `Script007_Insert_Values_Into_Oto_Parameters_Table.sql` | Marcas e credenciais |
| `Script008_Migrate_Otto_Sent_Records_Log.sql` | Opcional: cópia de `OttoSentRecordsLog` (§10) |

O `PostgreSQL/migrator.csx` chama `MySqlDatabase` (é o mesmo erro do Movidesk e do AfterSale). Fica anotado,
mas fora do escopo.

---

## 6. Infrastructure.Oto e host

### 6.1 `DependencyInjection.cs`

* Namespace `Infrastructure.Oto.DependencyInjection` e método `AddScopedOtoServices`.
* `IApiClient` com a chave `"OtoAPI"` e o `HttpClient` `"OtoAPI"` (`Oto:BaseUrl`, timeout de 20 min).
* O `HttpClient` `"OttoAPIHmg"` hoje é registrado e nunca usado. Proposta: trocar o `BaseUrl` por ambiente
  (`appsettings.Development.json` → homologação) em vez de manter dois clientes.
* Registrar os quatro services, os quatro repositórios, o command handler e os validadores
  (`AddValidatorsFromAssemblyContaining<...>()`).

### 6.2 Repositórios

* `ISharedRepository` no lugar de `IIntegrationsCoreRepository` e `ISQLServerConnection`
  (`GetRecords`, `GetRecord`, `CreateSystemDataTable`, `FillSystemDataTable`, `BulkInsertIntoTableRaw` e as
  merges).
* O `BulkInsertIntoTableRaw` atual não é `async`, cria uma lista que não usa (`sentRecordLogs`) e devolve
  `true` sempre.

### 6.3 Host WebApi

| Arquivo | Alteração |
|---|---|
| `WebApi.csproj` | Referenciar `Application.Oto` e `Infrastructure.Oto` |
| `Extensions/ServicesExtensions.cs` | Descomentar e trocar para `using Infrastructure.Oto.DependencyInjection;` + `AddScopedOtoServices` |
| `appsettings.json` | Manter `Oto:BaseUrl`; tirar `BaseUrlHmg` (ou levar para o `appsettings.Development.json`) |
| `Controllers/General/OttoController.cs` | Renomear para `OtoController.cs` e reescrever (abaixo) |

### 6.4 `OtoController`

Rota base `OtoJobs/Oto`, `[Authorize]` comentado como no Movidesk e no AfterSale, `[Tags]` por recurso.

| Método | Rota | Service |
|---|---|---|
| POST | `Customers` | `SendCustomers` |
| POST | `Cashback` | `SendCashback` |
| POST | `Orders` | `SendOrders` |
| POST | `Products` | `SendProducts` |
| POST | `Stores` | `SendStores` |
| POST | `Sellers` | `SendSellers` |

Os outros módulos usam `GET` para disparar os jobs de leitura. Aqui o verbo é `POST` porque a rota escreve num
sistema externo. Fica como decisão em §10.

As rotas por registro que estavam comentadas (`Customer`, `Order`, `Product`...) podem voltar no padrão das
leituras sob demanda do Movidesk: `POST Customers/{docCompany}/{id}`, com o cabeçalho `X-User-Name`, o CNPJ
validado por `^\d{14}$` e a chamada registrada em `OtoRequests`. Respostas: 200 enviado, 404 não encontrado na
origem, 422 recusado pela Oto. Isso é opcional (fase 3).

---

## 7. Auditoria (jobs)

| Id | Job | Hoje | Proposta |
|---|---|---|---|
| 4000 | `OtoSendOrders` | comentado | ativo, 3 min |
| 4001 | `OtoSendCustomers` | comentado; também usado pelo cashback | ativo, 3 min, só clientes |
| 4002 | `OtoSendProducts` | comentado | ativo (intervalo a definir) |
| 4003 | `OtoSendOrdersItems` | comentado e nunca usado (os itens vão junto com o pedido) | remover, ou manter reservado |
| 4004 | `OtoSendStores` | comentado | ativo (intervalo a definir) |
| 4005 | `OtoSendSellers` | comentado | ativo (intervalo a definir) |
| 4006 | `OtoSendCashback` | não existe | novo |
| 4007+ | sob demanda (`OtoSendCustomerById`...) | não existe | se a fase 3 entrar |
| 4500+ | `sp_OtoSentRecords_Sync` | não existe | procedure (`IdType = 1`, `IdParent` = job de origem) |

---

## 8. Erros de comportamento encontrados (corrigir junto)

| # | Onde | Problema |
|---|---|---|
| 1 | `OtoService.SendSellers` | O laço percorre `sellers` em vez de `groupedSellers`: **todos os vendedores vão para as duas marcas** |
| 2 | `OtoService.SendCustomers` | Não separa por marca: todos os clientes vão para as duas contas |
| 3 | `OtoService.SendCustomersCashBack` | Manda o saldo em `custom_field_03` de `/v1/customers` em vez de `/v1/cashback`; usa o job `OtoSendCustomers`; e manda `EXPIRACAO_SALDO` (data de expiração) no campo do **valor** |
| 4 | `BuildObjects.BuildCustomerObject` | Campo `address_n` não existe na API (o certo é `address_number`): **o número do endereço nunca chega** |
| 5 | `BuildObjects.BuildStoreObject`/`BuildSellerObject` | `name_otto` em vez de `name_oto`; `neighborhood` não existe em lojas; `status` vai como `0/1` e a API espera booleano |
| 6 | `BuildObjects.BuildCustomerObject` | `whatsapp_permission` é sempre `true`, sem consentimento na origem (risco de LGPD) |
| 7 | `BuildObjects.BuildCustomerObject` | `mobile_number` e `phone_number` recebem o mesmo valor; o celular não é validado nos 13 dígitos |
| 8 | `BuildObjects.BuildCustomerObject` | `ZipCode.Insert(5, "-")` lança exceção com CEP nulo ou com menos de 5 caracteres, e derruba o job |
| 9 | `CreateGetOrdersQuery` (e-commerce) | Alias `SIPCODE` em vez de `ZIPCODE`: o CEP dos clientes do e-commerce vai vazio |
| 10 | `CreateGetOrdersQuery` (POS, VF, VD) | O filtro `A.CANCELADO = 'N'` faz o `CANCELED` ser sempre 0: **cancelamentos nunca são enviados** |
| 11 | `OtoService.SendOrders` | O mesmo cliente entra várias vezes no lote (um por pedido); falta deduplicar por `customer_id` |
| 12 | `OtoService.SendOrders` | Envia itens antes dos pedidos e dos clientes; a ordem natural é clientes → pedidos → itens |
| 13 | Todos os `Send*` | Resposta não tratada: 401/413/422/429 passam sem mensagem; corpo vazio dá `NullReferenceException` |
| 14 | Todos os `Send*` | Log de execução não fecha quando há exceção (sem `finally`) |
| 15 | `OttoSentRecordsLog` | Sem reenvio de alterações e sem marca no filtro (§5.2) |

---

## 9. Plano em fases

| Fase | Entrega | Resultado |
|---|---|---|
| **1. Base** | §2 (csproj, nomes, namespaces), §3 (DTOs e entidades), §5 (scripts), §6.1–6.3 (DI e host) | Módulo compila no Shared e sobe no host |
| **2. Fluxo** | §4 (services divididos, `Send<T>`, validações, cache, log de requisições), §6.4 (controller), §7 (jobs) e as correções de §8 | Envio recorrente no padrão, auditável |
| **3. Opcional** | Rotas sob demanda por registro (§6.4); novos endpoints da API (`/v1/nps`, `/v1/blocked`, `tag_*`), se houver origem | — |
| **4. Docs** | `README.md` do módulo no formato do Movidesk e pasta `Requests Jsons` com um corpo de exemplo por rota (dados fictícios) | — |

---

## 10. Decisões pendentes

| # | Pergunta | Opções |
|---|---|---|
| 1 | Autenticação | Chave privada fixa (como o token do Movidesk) **ou** login + JWT em cache |
| 2 | Separação por marca | Coluna `brand` em `OtoParameters` usada nas queries **ou** tabela de lojas por marca |
| 3 | Histórico de `OttoSentRecordsLog` | Migrar as chaves (forçando um reenvio único para corrigir os `""` e o `address_number`) **ou** começar do zero |
| 4 | Verbo das rotas que disparam jobs | `POST` (escreve no Oto) **ou** `GET` (igual aos outros módulos) |
| 5 | `whatsapp_permission` | De onde vem o consentimento; até lá, enviar `null` |
| 6 | Intervalo dos jobs de produtos, lojas e vendedores | Ex.: 3 min (como hoje) **ou** 120 min (catálogo, como serviços do Movidesk) |
| 7 | Cancelamentos | Tirar o filtro `CANCELADO = 'N'` e mandar `canceled = true` (e `order_item_canceled`) |
