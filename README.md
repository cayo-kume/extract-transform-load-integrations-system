# extract-transform-load-integrations-system
A conceptual ETL, develop completely with .NET

O sistema de integrações da **Newbloomers**: lê os dados dos ERPs e SaaS usados pelas marcas (Totvs Moda, Linx Microvix,
Linx Commerce, Movidesk e AfterSale), grava tudo num banco SQL Server com histórico de auditoria, oferece leituras e
escritas sob demanda para outros sistemas e alimenta um **data warehouse** com dimensões, fatos e agregados.

Cada integração é um pacote independente (`Domain`, `Application` e `Infrastructure`), e um único host web (`WebApi`)
registra todos e expõe as rotas. Este documento dá o panorama; os detalhes de cada módulo estão no README dele.

---

## Sumário

1. [Tecnologias](#tecnologias)
2. [Estrutura do repositório](#estrutura-do-repositório)
3. [Módulos](#módulos)
4. [Como uma integração funciona](#como-uma-integração-funciona)
5. [Endpoints](#endpoints)
6. [Banco de dados e scripts](#banco-de-dados-e-scripts)
7. [Configuração](#configuração)
8. [Como executar](#como-executar)
9. [Auditoria e relatórios](#auditoria-e-relatórios)
10. [Data warehouse](#data-warehouse)
11. [Padrões do código](#padrões-do-código)
12. [Fora do host e pendências](#fora-do-host-e-pendências)
13. [Documentação](#documentação)

---

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Linguagem e runtime | C# / **.NET 10** (`net10.0`) |
| Host | ASP.NET Core (`WebApplication.CreateSlimBuilder`), controllers e **Swagger** (Swashbuckle) |
| Agendamento | **Hangfire** 1.8 (SQL Server storage), opcional e hoje desligado (`Hangfire:Enabled`): os jobs são disparados pelas rotas |
| Acesso a dados | **Dapper** e bulk insert (`ISharedRepository`), SQL montado nos *command handlers* |
| Banco | **SQL Server** (dados, auditoria e DW); scripts para MySQL e PostgreSQL existem só como `migrator.csx` |
| Migrações | **DbUp** executado por `dotnet-script` (`migrator.csx` em cada módulo) |
| Validação | **FluentValidation** (um validador por DTO) |
| HTTP e JSON | `IHttpClientFactory` (`IApiClient` nomeado por módulo), **Newtonsoft.Json**, XML (`System.Xml.Linq`) no Linx Microvix |
| Cache | `IMemoryCache` ([docs/PADRAO-CACHE-INTEGRACOES.md](docs/PADRAO-CACHE-INTEGRACOES.md)) |
| Front-end (em construção) | Blazor Web App (interativo no servidor) com **MudBlazor** (`src/Frontend/WebApp`) e .NET MAUI (`src/Mobile/MobileApp`) |

---

## Estrutura do repositório

```
extract-transform-load-integrations-system/
├── etl-integrations-system.slnx          solução
├── docs/                                 documentos comuns a todos os módulos: padrões, migrator e revisão
├── src/
│   ├── Backend/
│   │   ├── Hosts/
│   │   │   ├── WebApi/                   host web: controllers, Swagger, registro dos módulos (o único em uso)
│   │   │   ├── Quartz/                   worker alternativo (não usado)
│   │   │   └── AzureWebJob/              worker alternativo (não usado)
│   │   ├── Shared/                       auditoria, cliente HTTP, repositório comum, middlewares e scripts base
│   │   └── Modules/
│   │       ├── Totvs/
│   │       ├── LinxMicrovix/             LinxMicrovix - Outbound Web Service (leituras) e Inbound (vazio)
│   │       ├── LinxCommerce/
│   │       ├── General/                  AfterSale, Movidesk (ativos); Dootax, Mobsim, Oto, Pagarme (fora do host)
│   │       ├── Shippment/                FlashCourier, Jadlog, Stone, TotalExpress (fora do host)
│   │       └── DataWarehouse/
│   ├── Frontend/WebApp/                  painel Blazor (aplicações, jobs e integrações)
│   └── Mobile/MobileApp/                 app MAUI
└── tests/                                (vazio)
```

Cada módulo segue o mesmo desenho:

```
<Módulo>/
├── Domain.<Módulo>/          DTOs (Dtos/Reads e Dtos/Writes), entidades, enums (EnumJob) e interfaces dos repositórios
├── Application.<Módulo>/     services (regras de leitura e escrita), command handlers (SQL) e validadores
├── Infrastructure.<Módulo>/  repositórios, injeção de dependência e Scripts/SQLServer (DbUp)
├── docs/                     propostas, planos, implementações e modelagens do módulo, e as respostas de
│                             exemplo da API (Responsed Jsons ou Responsed XMLs, com dados fictícios)
└── README.md                 documentação do módulo
```

---

## Módulos

Registrados no host (`src/Backend/Hosts/WebApi/Extensions/ServicesExtensions.cs`):

| Módulo | Sistema | App em `auditing.Apps` | Schema | Rota base | Rotas | README |
|---|---|---|---|---|---|---|
| **Totvs** | Totvs Moda (ERP: pessoas, produtos, pedidos, notas, financeiro) | 130 | `totvs` | `TotvsJobs/Totvs` | 122 | [README](src/Backend/Modules/Totvs/README.md) |
| **LinxMicrovix** (Outbound) | Linx Microvix (ERP e B2C, web services XML) | 80 | `linxmicrovix` | `LinxMicrovixJobs/LinxMicrovix` | 284 | [README](src/Backend/Modules/LinxMicrovix/LinxMicrovix%20-%20Outbound%20Web%20Service/README.md) |
| **LinxCommerce** | Linx Commerce (e-commerce: pedidos, clientes, catálogo) | 70 | `linxcommerce` | `LinxCommerceJobs/LinxCommerce` | 12 | [README](src/Backend/Modules/LinxCommerce/README.md) |
| **Movidesk** | Movidesk (help desk: tickets, pessoas, serviços) | 30 | `movidesk` | `MovideskJobs/Movidesk` | 16 | [README](src/Backend/Modules/General/Movidesk/README.md) |
| **AfterSale** | Troque Fácil (pós-venda: reversas e reembolsos) | 50 | `aftersale` | `AfterSaleJobs/AfterSale` | 24 | [README](src/Backend/Modules/General/AfterSale/README.md) |
| **DataWarehouse** | DW do NEWBLOOMERS (lê as tabelas dos módulos) | 90 | `dw_*`, `datawarehouse`, `datamart` | `DataWarehouseJobs/DataWarehouse` | 5 | [guia do DW](src/Backend/Modules/DataWarehouse/docs/00-ordem-de-leitura.md) |
| **Shared** | Base comum: auditoria, `IApiClient`, `ISharedRepository` | — | `auditing`, `untreated`, `dbo` | — | — | — |

---

## Como uma integração funciona

### Leitura recorrente (carga do banco)

1. Uma rota (`POST` ou `GET` sem parâmetros) dispara o job; ele abre um log em `auditing.Logs` com o `IdJob` do `EnumJob`.
2. A service lê as empresas cadastradas na tabela de parâmetros do módulo (`<schema>.<Módulo>Parameters`) e, para cada uma,
   chama a API com paginação, nova tentativa em 401/429/502/503/504 e a janela de alteração (data ou `timestamp`).
3. Cada registro é validado (FluentValidation; erro vira mensagem no log, não interrompe), convertido em entidade com o
   **mesmo horário de lote** e o CNPJ da empresa (`doc_company`) e filtrado pelo cache/estado já gravado (só novos ou
   alterados).
4. O repositório faz o **bulk insert** em `untreated.<Tabela>` e a procedure `<schema>.sp_<Tabela>_Sync` (gerada pelo
   `dbo.sp_Create_Merge_Procedure` do Shared) faz o `MERGE` na tabela final `<schema>.<Tabela>`, com PK.
5. O JSON/XML de cada registro vai para `auditing.Records`, as mensagens para `auditing.Messages`, e o log é fechado com o
   total gravado.

### Leitura sob demanda (um registro por chave)

Rotas `GET .../{docCompany}/{chave}` chamadas por sistemas externos: exigem o cabeçalho **`X-User-Name`** (até 100
caracteres) e o CNPJ da empresa na rota; leem só aquele registro, gravam pelo mesmo caminho da leitura recorrente e
registram a chamada em `<schema>.<Módulo>Requests`. Códigos: **200** gravado, **404** a API não tem o registro, **422**
empresa não cadastrada ou erro da API (detalhe no log), **400** parâmetros inválidos.

### Escrita (Totvs, Movidesk, AfterSale)

Rotas `POST`/`PUT`/`PATCH`/`DELETE` com corpo validado **antes** de chamar a API (400 com os erros, sem chamada). Cada
chamada fica em `<Módulo>Requests` com usuário, rota, corpo, resposta e `status_code`. Na Totvs, cada grupo de escrita só
roda com `Totvs:Writes:<grupo>:Enabled = true` (senão 422) e há trava contra a repetição do mesmo corpo em 10 minutos.

---

## Endpoints

O Swagger do host lista todas as rotas, agrupadas pela tag de cada service: **`http://localhost:5252/swagger`** (perfil de
desenvolvimento). Resumo:

| Módulo | Leituras recorrentes | Leituras sob demanda | Escritas e ações |
|---|---|---|---|
| Totvs | 43 (pessoas, filiais, vendedores, produtos, saldos, preços, pedidos, notas, duplicatas, catálogos) | 25 | 54 (pessoas, produtos, preços, pedidos de compra e de venda, duplicatas, bônus, fiscal) |
| LinxMicrovix | 260 (`POST <Método>`, um por método da API: 191 do ERP e 69 do B2C) | 24 | — |
| LinxCommerce | 6 (`SearchOrders`, `SearchOrderStatus`, `SearchCustomer`, `SearchProduct`, `SearchSKU`, `SearchSalesRepresentative`) | 6 | — |
| Movidesk | 3 (`Tickets`, `People`, `Services`) | 3 | 10 (tickets, anexos, pessoas, serviços) |
| AfterSale | 12 (e-commerce, reversas, reembolsos e domínios) | 2 (`Reverses/{docCompany}/{id}`, `Refunds/{docCompany}/{id}`) | 10 (reversas: aprovar, receber produtos, nota de devolução, nota e XML, registro administrativo; reembolsos: aprovar, cancelar, pagar, alterar valor) |
| DataWarehouse | `RunHourly`, `RunDaily`, `RunShortCycle`, `RunMonthlyStock?positionDate=`, `RunLinxLegacy` | — | — |

Exemplos:

```http
POST http://localhost:5252/TotvsJobs/Totvs/Customers
GET  http://localhost:5252/TotvsJobs/Totvs/Customers/44065634000106/123
X-User-Name: fulano

POST http://localhost:5252/LinxMicrovixJobs/LinxMicrovix/LinxProdutos
POST http://localhost:5252/DataWarehouseJobs/DataWarehouse/RunHourly
```

> O `[Authorize]` dos controllers está comentado: o host ainda não tem um esquema de autenticação configurado
> (`Api/Auth/token` existe, mas não protege as rotas).

---

## Banco de dados e scripts

### Schemas

| Schema | Conteúdo |
|---|---|
| `auditing` | `Apps`, `Jobs`, `Types`, `Logs`, `Messages`, `Records` e as procedures `sp_Log_Start/Message/Error/End` (Shared) |
| `untreated` | Carga bruta de cada tabela de dados, sem chave (recebe o bulk insert) |
| `totvs`, `linxmicrovix`, `linxcommerce`, `movidesk`, `aftersale` | Tabelas finais (com PK), procedures `sp_<Tabela>_Sync`, parâmetros e `<Módulo>Requests` |
| `dbo` | `sp_Create_Merge_Procedure` (gera os merges), limpeza de logs e manutenção (Shared) |
| `dw_landing`, `dw_history`, `dw_intermediate`, `dw_personal_data`, `datawarehouse`, `datamart`, `dw_outbound`, `dw_reference`, `dw_control` | Data warehouse |

Toda tabela de dados tem `doc_company` (CNPJ da empresa) e `integrated_or_updated_at` (horário da gravação, em Brasília).

### Scripts (DbUp)

Cada módulo tem `Infrastructure.<Módulo>/Scripts/SQLServer` com um `migrator.csx` e os scripts em subpastas:

```
SQLServer/
├── migrator.csx
├── Parameters/        schema, tabela de parâmetros, app, credenciais e a tabela de requests
└── <Service>/         um trio por service: _Tables, _Sync_Procedures e _Jobs_Into_Jobs_Table
```

O `Shared`, base dos outros, não tem services e mantém os 5 scripts direto em `SQLServer/`.

A numeração é única e sequencial no módulo e define a ordem de execução (o `migrator.csx` lê as subpastas e ordena pelo
número). No DW as pastas são por camada (`Parameters`, `Landing`, `History`, `PersonalData`, `Dimensions`,
`Intermediate`, `Facts`, `Datamart`, `Outbound`, `Checks`). Uma correção de tabela, procedure ou view vai no script de
criação, não num `ALTER` novo.

**Ordem entre módulos:** `Shared` primeiro (auditoria e `sp_Create_Merge_Procedure`), depois os módulos, e o
`DataWarehouse` por último (ele lê as tabelas dos módulos).

> **Aviso:** Renomear, dividir ou renumerar scripts: em bancos que já rodaram as migrações, o DbUp vê os scripts como novos e executa de novo.

---

## Configuração

`src/Backend/Hosts/WebApi/appsettings.json`:

| Chave | Uso |
|---|---|
| `ConnectionStrings:Connection` | SQL Server (dados, auditoria e DW) |
| `<Módulo>:BaseUrl` | Endereço da API de cada módulo (`Totvs`, `LinxMicrovix`, `Movidesk`, `AfterSale`...); homologação é outro valor no ambiente |
| `Totvs:Writes:<grupo>:Enabled` | Liga as escritas da Totvs por grupo (`AccountsPayable`, `Bonus`, `Invoices`, `People`, `Products`, `PurchaseOrders`, `SalesOrders`) |
| `Totvs:Writes:Products:AllowedValueCodes`, `AllowedValueBranches`, `MaxValueVariationPercent` | Limites da escrita de preço e custo |
| `DataWarehouse:Pepper` | Pepper do hash de CPF/CNPJ no DW; sem ele, as etapas com `doc_hash` são puladas |

As empresas e credenciais de cada API ficam no banco, na tabela de parâmetros do módulo (`TotvsParameters`,
`LinxMicrovixParameters`, `LinxCommerceParameters`, `MovideskParameters`, `AfterSaleParameters`), gravadas pelo script
`Parameters/Script003` de cada um. O Linx Commerce não tem `BaseUrl`: o endereço é de cada cliente, na tabela de
parâmetros. Credenciais nunca são gravadas em logs nem nas tabelas de requests.

---

## Como executar

**Pré-requisitos:** .NET 10 SDK, SQL Server e o `dotnet-script` (`dotnet tool install -g dotnet-script`).

1. **Banco:** rode os migrators na ordem (Shared, módulos, DataWarehouse), de dentro da pasta `Scripts/SQLServer` de cada
   um, passando a connection string:

   ```bash
   dotnet script migrator.csx -- "Server=localhost;Database=NEWBLOOMERS;User Id=sa;Password=...;TrustServerCertificate=True;"
   ```

   No Windows, o caminho dos módulos passa de 260 caracteres: mapeie a pasta com `subst X: "<pasta SQLServer>"` e rode o
   migrator a partir de `X:` ([docs/MIGRATOR-CSX.md](docs/MIGRATOR-CSX.md)). O migrator do DW exige a connection string
   como argumento e não imprime usuário nem senha.

2. **Parâmetros:** confira o `Parameters/Script003` de cada módulo (empresas e credenciais) antes de rodar o migrator.

3. **Host:**

   ```bash
   dotnet run --project src/Backend/Hosts/WebApi
   ```

   O Swagger abre em `http://localhost:5252/swagger`.

4. **Build da solução:** o host compila sozinho (`dotnet build src/Backend/Hosts/WebApi`); a solução inteira hoje falha
   no restore de módulos fora do host (pacotes sem versão em Dootax, Oto, Stone e TotalExpress).

---

## Auditoria e relatórios

| Onde | O que fica registrado |
|---|---|
| `auditing.Logs` | Cada execução de job: `IdJob`, `IdType`, `Execution` (GUID), execução pai, início e fim |
| `auditing.Messages` | Mensagens da execução: erros de validação, respostas de erro da API, avisos |
| `auditing.Records` | O registro integrado (JSON ou XML) pela chave (`FieldKeyValue`), para conferir o que veio da origem |
| `<schema>.<Módulo>Requests` | Cada leitura sob demanda e escrita: usuário (`X-User-Name`), rota, chave, corpo, resposta, `is_success`, `status_code` |
| `auditing.Jobs` / `auditing.Apps` | Catálogo dos jobs (id, título, intervalo, tipo: integração ou procedure) por app |
| `dw_control.CheckResult` | Resultado das checagens do DW em cada carga |
| `dw_control.Watermark` | Até onde cada origem já foi lida pelo DW |

**Relatórios gerados pelo DW:**

* **Fatos e dimensões** (`datawarehouse`): vendas por item (`FactSalesItem`), pedidos (`FactOrderItem`), pagamentos de
  notas, estoque mensal (`FactMonthlyStock`), compras, contas a pagar, comissões, recepção de loja, chamadas de API
  (`FactApiRequest`), tickets e reembolsos; dimensões de data, loja, produto, cliente, vendedor, fornecedor, forma de
  pagamento, operação, canal, marca, representante, região e promoção.
* **Agregados** (`datamart.DailySales`, atualizado por `sp_DailySales_Refresh`).
* **Visões de saída** (`dw_outbound.WmsOrders`, `dw_outbound.LogisticsShipments`) para os sistemas de logística.
* **Checagens** (9 views `vw_Check_*`: grão do fato, membro desconhecido, soma dos produtos, pedido sem tipo, filhas
  apagadas, conciliação do faturado, estoque negativo ou sem custo, sobreposição de ERP e de/para de produto), com o
  resultado gravado em `CheckResult`.
* **Dado pessoal** separado em `dw_personal_data`; nas tabelas do DW o cliente aparece só pelo hash do documento.

---

## Data warehouse

O DW lê as tabelas finais dos módulos (Totvs, legado Linx Microvix, Movidesk, AfterSale e, quando houver carga real,
Linx Commerce) e monta as camadas: **landing** (cópia incremental, com detecção de exclusão), **histórico** (SCD2 e
snapshots), **intermediate** (views de regra de negócio), **dimensões**, **fatos**, **datamart** e **saída**. As cargas são
disparadas pelas rotas do `DataWarehouseController`:

| Rota | Job | O que faz |
|---|---|---|
| `RunHourly` | 9000 | Landing, snapshots de notas e pedidos, dimensões, fatos, agregados e checagens |
| `RunDaily` | 9001 | A horária mais os snapshots de cadastros, custos e preços, a detecção de exclusões e, no dia 1, o estoque do mês |
| `RunShortCycle` | 9002 | Ciclo curto da logística: landing de pedidos, notas e clientes e o endereço de entrega |
| `RunMonthlyStock` | 9003 | Recarga manual da posição de estoque de um fim de mês |
| `RunLinxLegacy` | 9004 | Carga única do legado Linx Microvix |

O guia completo (premissas, arquitetura, modelo, DDL, procedures, operação, acesso e riscos) está em
[src/Backend/Modules/DataWarehouse/docs](src/Backend/Modules/DataWarehouse/docs/00-ordem-de-leitura.md), junto das regras de prefixo dos pedidos e do funcionamento do DW.

---

## Padrões do código

As regras de organização valem para todos os módulos e estão em
[docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md):

* refatoração de estilo não muda o comportamento;
* método privado só existe quando é usado por mais de um método; senão, vai para dentro do pai (código direto ou função local);
* nas services, públicos primeiro e privados no fim, depois de uma linha `// ----`;
* `Validations` espelha os DTOs (`Reads/` e `Writes/`);
* um command handler por service, usado só pelo repositório dele;
* scripts em `Parameters/` e uma pasta por service, numeração sequencial, correção no script de criação.

O cache segue [docs/PADRAO-CACHE-INTEGRACOES.md](docs/PADRAO-CACHE-INTEGRACOES.md).

---

## Fora do host e pendências

* **Módulos fora do host:** Dootax, Mobsim, Oto, Pagarme (General) e FlashCourier, Jadlog, Stone, TotalExpress
  (Shippment) têm código do sistema antigo, mas o registro no host e os controllers estão comentados. O
  `LinxMicrovix - Inbound Web Service` (escritas do B2C) está vazio.
* **Agendamento:** o Hangfire é opcional no host `WebApi` (`Hangfire:Enabled`, desligado por padrão): ligado, sobem o
  storage, o servidor, o dashboard com usuário e senha e a agenda montada a partir de `auditing.Jobs` (só os jobs com
  `IsActive = 1`, que nascem 0), com uma trava que pula a execução quando a anterior do mesmo job ainda está rodando
  (`src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md`, passos 3 e 4). Hoje o Hangfire está desligado e os jobs rodam
  pelas rotas, chamadas por um agendador externo. Os hosts `Quartz` e `AzureWebJob` não são usados.
* **Autenticação:** o `[Authorize]` dos controllers está comentado.
* **Testes:** a pasta `tests` está vazia.

---

## Documentação

| Documento | Conteúdo |
|---|---|
| README de cada módulo | Conceitos da API, services e métodos, fluxos, endpoints, scripts, jobs, configuração e pendências |
| [src/Backend/Modules/DataWarehouse/docs](src/Backend/Modules/DataWarehouse/docs/00-ordem-de-leitura.md) | Guia do data warehouse |
| [docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md) | Padrão de código e scripts |
| [docs/PADRAO-CACHE-INTEGRACOES.md](docs/PADRAO-CACHE-INTEGRACOES.md) | Padrão de cache |
| [docs/MIGRATOR-CSX.md](docs/MIGRATOR-CSX.md) | Como rodar o `migrator.csx` |
| [docs/REVISAO-PADRAO-CODIGO-E-SCRIPTS.md](docs/REVISAO-PADRAO-CODIGO-E-SCRIPTS.md) | Revisão dos módulos contra o padrão (todos os módulos do host e o Shared já seguem) |
| `docs/` de cada módulo | Propostas, planos, implementações e modelagens do módulo (ex.: `src/Backend/Modules/Totvs/docs`); os do Shared ficam em `src/Backend/Shared/docs` |
