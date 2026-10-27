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
│   ├── Handlers/Commands/      (ITotvsCommandHandler)
│   └── Services/               (ITotvsService e um por recurso)
├── Handlers/Commands/          (TotvsCommandHandler: monta as queries SQL)
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
| `SaveRequest` | Grava uma chamada sob demanda em `totvs.TotvsRequests`, ligada ao `Execution` do log aberto por quem chama. A rota é gravada sem o token, e o filtro enviado vai em `request_body` |

**Leituras recorrentes**

| Service | Método | Rota da API | Tabelas | Job | Intervalo |
|---|---|---|---|---|---|
| `TotvsPeopleService` | `GetBranchesList` | `GET person/v2/branchesList` | `TotvsBranches` e endereços | 13007 | 120 min |
| `TotvsPeopleService` | `PostSearchCustomers` | `POST person/v2/individuals/search` | `TotvsPeople` e 12 filhas | 13002 | 3 min |
| `TotvsPeopleService` | `PostSearchLegalEntity` | `POST person/v2/legal-entities/search` | `TotvsLegalEntities` e 15 filhas | 13001 | 3 min |
| `TotvsSellersService` | `PostSearchSellers` | `POST seller/v2/search` | `TotvsSellers` e 2 filhas | 13006 | 120 min |
| `TotvsProductsService` | `PostSearchReferences` | `POST product/v2/references/search` (uma chamada por marca) | `TotvsSkus` e 21 filhas/netas | 13000 | 120 min |
| `TotvsPurchaseOrdersService` | `PostSearchPurchaseOrders` | `POST purchase-order/v2/search` | `TotvsPurchaseOrders` e 2 filhas | 13004 | 3 min |
| `TotvsSalesOrdersService` | `PostSearchSalesOrders` | `POST sales-order/v2/orders/search` | `TotvsSaleOrders` e 10 filhas/netas | 13005 | 3 min |
| `TotvsAccountsPayableService` | `PostSearchDuplicates` | `POST accounts-payable/v2/duplicates/search` | `TotvsDuplicates` e 2 filhas | 13008 | 3 min |
| `TotvsAccountsPayableService` | `PostSearchComissionsPaid` | `POST accounts-payable/v2/comissions-paid/search` (uma chamada por loja) | `TotvsCommissionsPaid` e movimentos | 13009 | 3 min |
| `TotvsInvoicesService` | `PostSearchInvoices` | `POST fiscal/v2/invoices/search` | `TotvsInvoices` e 9 filhas/netas | 13003 | 3 min |
| `TotvsPersonCatalogsService` | `GetClassifications` | `GET person/v2/classifications` | `TotvsPersonClassifications` | 13032 | 120 min |
| `TotvsPersonCatalogsService` | `GetEmailTypes` | `GET person/v2/email-types` | `TotvsEmailTypes` | 13033 | 120 min |
| `TotvsPersonCatalogsService` | `GetPhoneTypes` | `GET person/v2/phone-types` | `TotvsPhoneTypes` | 13034 | 120 min |
| `TotvsPersonCatalogsService` | `GetContactTypes` | `GET person/v2/contact-types` | `TotvsContactTypes` | 13035 | 120 min |

**Escritas** (sistema externo, com `X-User-Name`)

| Service | Método | Rota da API | Job | Chave de configuração |
|---|---|---|---|---|
| `TotvsAccountsPayableService` | `PostDuplicatesInstallmentsStage` | `POST accounts-payable/v2/duplicates/installments/stage` | 13020 | `Totvs:Writes:AccountsPayable:Enabled` |

**Leituras sob demanda** (um registro por chave)

| Service | Método | Busca na API | Job |
|---|---|---|---|
| `TotvsPeopleService` | `GetCustomerByCode` | `individuals/search` com `personCodeList` | 13010 |
| `TotvsPeopleService` | `GetLegalEntityByCode` | `legal-entities/search` com `personCodeList` | 13011 |
| `TotvsSalesOrdersService` | `GetSalesOrderByKey` | `orders/search` com `branchCodeList` + `orderCodeList` | 13012 |
| `TotvsInvoicesService` | `GetInvoiceByKey` | `invoices/search` com `branchCodeList` + `invoiceSequenceList` | 13013 |
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
    ├── SQLServer/              (scripts DbUp + migrator.csx)
    ├── MySQL/                  (migrator.csx)
    └── PostgreSQL/             (migrator.csx)
```

### Injeção de dependência

`AddScopedTotvsServices(IServiceCollection, IConfiguration)` registra:

* `IApiClient` com a chave `"TotvsAPI"` e o `HttpClient` nomeado `"TotvsAPI"` (base `Totvs:BaseUrl`, timeout de 20 minutos);
* os services, os repositórios e o `ITotvsCommandHandler` (`Scoped`);
* todos os validadores do assembly numa chamada só (`AddValidatorsFromAssemblyContaining<SellerValidator>()`).

O host chama o método em `Hosts/Hangfire/Extensions/ServicesExtensions.cs`.

### Scripts de banco (SQL Server)

O DbUp executa cada script uma única vez, em ordem alfabética. Alterar um script que já rodou não muda um banco existente: para isso é preciso um script novo. Cada recurso traz um trio de scripts (tabelas, procedures e jobs).

| Script | Conteúdo |
|---|---|
| `Script001` | Schema `totvs` e `totvs.TotvsParameters` |
| `Script002` | App `130 - Totvs` em `auditing.Apps` |
| `Script003` | A holding em `TotvsParameters`, com as credenciais. **Fica fora do git** (`.gitignore`); o modelo é o `Script003_...sql.example` |
| `Script004`–`006` | Filiais |
| `Script007`–`009` | Vendedores |
| `Script010`–`012` | Produtos |
| `Script013`–`015` | Pessoas físicas e jurídicas |
| `Script016`–`018` | Pedidos de compra |
| `Script019`–`021` | Pedidos de venda |
| `Script022`–`024` | Duplicatas e comissões pagas |
| `Script025`–`027` | Notas fiscais |
| `Script028` | `totvs.TotvsRequests` (log das chamadas sob demanda) |
| `Script029`–`031` | Jobs das leituras sob demanda |
| `Script032` | Jobs das escritas de contas a pagar (13018–13020) |
| `Script033`–`035` | Catálogos da API Person (classificações e tipos de e-mail, telefone e contato) |

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
| POST | `Products` | `TotvsProductsService.PostSearchReferences` |
| POST | `PurchaseOrders` | `TotvsPurchaseOrdersService.PostSearchPurchaseOrders` |
| POST | `SalesOrders` | `TotvsSalesOrdersService.PostSearchSalesOrders` |
| POST | `Duplicates` | `TotvsAccountsPayableService.PostSearchDuplicates` |
| POST | `ComissionsPaid` | `TotvsAccountsPayableService.PostSearchComissionsPaid` |
| POST | `Invoices` | `TotvsInvoicesService.PostSearchInvoices` |
| GET | `People/Classifications` | `TotvsPersonCatalogsService.GetClassifications` |
| GET | `People/EmailTypes` | `TotvsPersonCatalogsService.GetEmailTypes` |
| GET | `People/PhoneTypes` | `TotvsPersonCatalogsService.GetPhoneTypes` |
| GET | `People/ContactTypes` | `TotvsPersonCatalogsService.GetContactTypes` |

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
| GET | `Invoices/{docCompany}/{branchCode}/{invoiceSequence}` | `GetInvoiceByKey` |
| GET | `Products/{docCompany}/{code}` | `GetProductByCode` |

| Código | Quando |
|---|---|
| **200** | Registro gravado |
| **404** | O registro não existe na Totvs |
| **422** | Empresa não cadastrada ou resposta inválida da API (detalhes no log da execução) |

### Escritas (chamadas por sistema externo)

Mesmas exigências das leituras sob demanda (`X-User-Name` e `{docCompany}`), com o corpo validado **antes** de chamar a Totvs.

| Método | Rota | Corpo | Service |
|---|---|---|---|
| POST | `Duplicates/{docCompany}/Installments/Stage` | `branchCnpj` (loja da empresa), `supplierCpfCnpj` **ou** `supplierCode`, `duplicateCode`, `installmentCode`, `newStage` (1 título não conferido, 2 liberado para pagamento, 5 título aceito) | `PostDuplicatesInstallmentsStage` |

| Código | Quando |
|---|---|
| **200** | Escrita aceita pela Totvs (chave e resposta da Totvs no corpo) |
| **400** | Corpo inválido (lista de campos e mensagens), sem chamar a Totvs |
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
| 13500–13999 | 1 (Procedure) | Procedures `sp_<Tabela>_Sync` (13500–13588), com `IdParent` = job da leitura e o mesmo intervalo |

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
| 13032 | `TotvsPersonGetClassifications` | 120 min |
| 13033 | `TotvsGetEmailTypes` | 120 min |
| 13034 | `TotvsGetPhoneTypes` | 120 min |
| 13035 | `TotvsGetContactTypes` | 120 min |
| 13018 | `TotvsPostDuplicates` | escrita (reservado, fase AP-2) |
| 13019 | `TotvsPostGroupDuplicates` | escrita (reservado, fase AP-2) |
| 13020 | `TotvsPostDuplicatesInstallmentsStage` | escrita |

Toda execução grava `auditing.Logs` e, conforme o caso:
* `auditing.Messages`, com mensagens e erros de validação;
* `auditing.Records`, com os registros integrados e o JSON recebido.

---

## Configuração

| Onde | Chave | Uso |
|---|---|---|
| `appsettings.json` do host | `Totvs:BaseUrl` | Endereço da API; homologação é só outro valor no `appsettings` do ambiente |
| `appsettings.json` do host | `Totvs:Writes:<grupo>:Enabled` | Liga as escritas de um grupo (`AccountsPayable`...). Sem a chave, ou com `false`, a rota devolve 422 sem chamar a Totvs |
| `appsettings.json` do host | `ConnectionStrings` | Conexão com o SQL Server (dados e auditoria) |
| Banco | `totvs.TotvsParameters` | CNPJ (`doc_company`) e credenciais (`client_id`, `client_secret`, `username`, `password`) de cada empresa |

As credenciais ficam só no banco (via `Script003`, fora do git) e nunca são gravadas em logs.

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

---

## Pendências

* **Preços, custos e saldos** (`PostSearchPrices`, `PostSearchCosts`, `PostSearchBalances`, jobs 13015–13017): decididos, ainda não implementados (fase 3, partes 4 a 6).
* **Execução em banco:** os scripts e as rotas ainda não rodaram pelo host; a modelagem foi conferida com chamadas reais à API, sem banco.
* **`referencedInvoices`:** comentado no `expand` das notas até a Totvs corrigir o erro 400.
* **Escritas:** só a troca de estágio da parcela está implementada; as demais seguem as fases do plano (`docs/PLANO-TOTVS-METODOS-FALTANTES.md`, §9.4). A primeira chamada real em homologação ainda não foi feita.

---

## Documentação complementar

Na pasta `docs/` da raiz do repositório:

| Documento | Assunto |
|---|---|
| `PROPOSTA-PADRONIZACAO-TOTVS.md` | Proposta, decisões e plano em fases da padronização do módulo, com o que foi conferido em cada API |
| `PADRAO-CACHE-INTEGRACOES.md` | Padrão de cache das leituras |
| `ALTERACOES-MERGE-PROCEDURE-AUDITORIA.md` | Procedure de merge e auditoria |
