# Pacote de DLLs para Integração com a Movidesk

## O que é a Movidesk?
A **Movidesk** é uma plataforma de help desk e service desk usada para organizar o atendimento ao cliente: pedidos de suporte, dúvidas e reclamações viram **tickets**, atendidos por agentes e equipes.

### Conceitos usados pela integração
* **Tickets**: cada atendimento, com assunto, status, responsável, ações (mensagens e anotações), histórico de status e de responsáveis, apontamentos de horas, despesas, anexos e campos adicionais.
* **Pessoas**: os cadastros da conta (agentes, clientes pessoa física e empresas), com endereços, contatos, e-mails, equipes, relacionamentos e campos adicionais.
* **Serviços**: o catálogo de serviços (até 4 níveis) que classifica os tickets, com as categorias permitidas em cada serviço.

Para conhecer a solução, visite o [**site da Movidesk**](https://www.movidesk.com/).

---

## Sobre o pacote

Módulo do sistema de integrações (ETL) responsável por **ler** os dados da API pública da Movidesk (tickets, pessoas e serviços) e gravá-los no banco SQL Server, empresa por empresa:

* de forma **recorrente**, pelos jobs do Hangfire (leituras incrementais ou do catálogo inteiro);
* **sob demanda**, a partir de um sistema externo, buscando um único registro por id e registrando quem pediu cada chamada.

O módulo só lê da Movidesk: nenhuma rota altera dados na plataforma.

| Item | Valor |
|---|---|
| Plataforma | .NET 10 (`net10.0`) |
| Arquitetura | Clean Architecture em três assemblies: Domain, Application e Infrastructure |
| API consumida | Movidesk API pública v1 — `https://api.movidesk.com/public/v1/` |
| Documentação da API | [Tickets](https://atendimento.movidesk.com/kb/article/256/movidesk-ticket-api), [Pessoas](https://atendimento.movidesk.com/kb/article/189/movidesk-person-api), [Serviços](https://atendimento.movidesk.com/kb/article/7440/api-servicos) |
| Banco de dados | SQL Server (schemas `untreated`, `general` e `auditing`) |
| Multiempresa | Sim: cada CNPJ cadastrado em `general.MovideskParameters` tem o seu token; o CNPJ (`doc_company`) é a primeira coluna da chave de todas as tabelas |
| Host | `src/Backend/Hosts/Hangfire` (controller `MovideskController`) |

### Limites da API
* **10 requisições por minuto** por token: as leituras esperam **6 segundos** entre uma chamada e outra.
* **3 respostas de erro seguidas bloqueiam a API** por um tempo. Por isso, quando uma página volta inválida, a empresa é interrompida naquela execução em vez de tentar de novo.
* O `GET /tickets` (lista) só devolve tickets atualizados nos **últimos 90 dias**; tickets mais antigos só pelo `GET Tickets/{docCompany}/{id}`.

### Disclaimer sobre a documentação da API
A modelagem foi conferida com respostas reais da API, porque a documentação não descreve tudo (por exemplo, a numeração dos níveis de serviço no ticket, `changedDate` nulo em pessoas nunca alteradas e o formato da resposta 404 de cada rota). Os JSONs usados estão na pasta [`Responsed Jsons`](#pasta-responsed-jsons), com os dados reais trocados por dados fictícios.

---

## Arquitetura

```
Movidesk/
├── Domain.Movidesk          ← contratos, DTOs, entidades e enums (sem dependência de infraestrutura)
├── Application.Movidesk     ← services (regras de integração), queries SQL e validações
├── Infrastructure.Movidesk  ← repositórios, injeção de dependência e scripts de banco
└── Responsed Jsons          ← JSONs de referência (dados fictícios) usados para montar DTOs e entidades
```

### Referências entre projetos

```
Infrastructure.Movidesk ──► Application.Movidesk ──► Domain.Movidesk
        │                           │                       │
        ▼                           ▼                       ▼
Infrastructure.Shared       Application.Shared        Domain.Shared
```

| Assembly | Pacotes NuGet | Referências |
|---|---|---|
| `Domain.Movidesk` | `Newtonsoft.Json` 13.0.4 | `Domain.Shared` |
| `Application.Movidesk` | `Microsoft.Extensions.Caching.Memory` 10.0.8 | `Application.Shared`, `Domain.Movidesk` |
| `Infrastructure.Movidesk` | — | `Infrastructure.Shared`, `Application.Movidesk`, `Domain.Movidesk` |

Do projeto Shared o módulo usa:
* `IApiClient`/`ApiClient` (cliente HTTP);
* `ILoggerService` (auditoria);
* `ISharedRepository` (consultas, bulk insert e chamada das procedures de merge);
* `ConverterValidatorsExtensions` e `NullableConverters` (conversões de string para os tipos das entidades, mantendo `null` quando a API não manda o valor).

---

## Domain.Movidesk

```
Domain.Movidesk/
├── Abstractions/Repositorys/   ← interfaces dos repositórios
├── Dtos/
│   └── Reads/                  ← respostas das rotas GET da API
│       ├── People/             (Person, Address, Contact, Email, Relationship, Service, CustomFieldValue, CustomFieldValueItem, Asset)
│       ├── Services/           (Service)
│       └── Tickets/            (Ticket, Action, Client, Person, OwnerHistory, StatusHistory, TimeAppointment, Expense, Attachment, Asset...)
├── Entities/
│   ├── Parameters/             (Company → general.MovideskParameters)
│   ├── Reads/                  ← o que é gravado a partir das rotas GET
│   │   ├── People/
│   │   ├── Services/
│   │   └── Tickets/
│   └── Writes/
│       └── Requests/           (Request → general.MovideskRequests)
└── Enums/                      (EnumJob)
```

### Convenções

* **DTOs** espelham o JSON da API: propriedades `string` com os nomes da API (`camelCase`). É **uma classe por arquivo**.
* **Entidades** têm os tipos das colunas e os **mesmos nomes das colunas**, porque o bulk insert mapeia as colunas pelo nome da propriedade.
  * Convertem os campos do DTO no construtor e cortam os textos no tamanho da coluna.
  * Campos opcionais ficam anuláveis (`int?`, `bool?`, `DateTime?`), para gravar `null` quando a API não manda o valor, em vez de `0` ou `false`.
* Toda entidade tem `doc_company` (CNPJ) e `integrated_or_updated_at`. Um registro e todas as suas filhas recebem **o mesmo horário de lote** (`MovideskService.GetBatchTime()`), que é a base da exclusão das filhas no merge (ver [Scripts](#scripts-de-banco-sql-server)).
* Listas filhas usam `[SkipProperty]` para não virarem coluna no bulk insert.
* Nomes: **Person** no singular e **People** no plural (`MovideskPeople`, `GetPeople`, `GetPersonById`).
* `EnumJob` deve ficar em sincronia com o `Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql`.

### Repositórios (interfaces)

| Interface | Responsabilidade |
|---|---|
| `IMovideskRepository` | Empresas (`GetCompanys`, `GetCompany`) e log das chamadas sob demanda (`InsertIntoMovideskRequests`) |
| `IMovideskTicketsRepository` | Tickets e filhas (bulk insert + merges), última atualização por empresa e consulta por ids para o cache |
| `IMovideskPeopleRepository` | Pessoas e filhas (bulk insert + merges), última alteração por empresa e consulta por ids para o cache |
| `IMovideskServicesRepository` | Serviços e categorias (bulk insert + merges) |

---

## Application.Movidesk

```
Application.Movidesk/
├── Abstractions/
│   ├── Handlers/Commands/      (IMovideskCommandHandler)
│   └── Services/               (IMovideskService, IMovideskTicketsService, IMovideskPeopleService, IMovideskServicesService)
├── Handlers/Commands/          (MovideskCommandHandler: monta as queries SQL)
├── Services/                   (MovideskService, MovideskTicketsService, MovideskPeopleService, MovideskServicesService)
└── Validations/                (FluentValidation por DTO: People/*, Services/*, Ticket/*)
```

### Services e métodos

**`MovideskService`**: operações comuns aos outros services

| Método | O que faz |
|---|---|
| `GetBatchTime` | Horário do lote (horário de Brasília), gravado igual no registro e em todas as filhas |
| `SaveRequest` | Grava uma chamada sob demanda em `general.MovideskRequests`, ligada ao `Execution` do log aberto por quem chama. A rota é gravada **sem o token** |

**`MovideskTicketsService`**

| Método | Rota da API | Tabelas | Job | Execução |
|---|---|---|---|---|
| `GetTickets` | `GET tickets` (lista de ids por `lastUpdate`, depois blocos de 100 com o `$expand` em 5 chamadas) | `MovideskTickets` e 13 tabelas filhas/netas | 3000 | A cada 3 minutos |
| `GetTicketById` | `GET tickets?id={id}` | as mesmas + `MovideskRequests` (log) | 3002 | Sob demanda |

**`MovideskPeopleService`**

| Método | Rota da API | Tabelas | Job | Execução |
|---|---|---|---|---|
| `GetPeople` | `GET persons` (`$filter` por `changedDate` **ou** `createdDate`, paginado de 100 em 100) | `MovideskPeople` e 9 tabelas filhas/netas | 3001 | A cada 3 minutos |
| `GetPersonById` | `GET persons?id={id}` | as mesmas + `MovideskRequests` (log) | 3003 | Sob demanda |

**`MovideskServicesService`**

| Método | Rota da API | Tabelas | Job | Execução |
|---|---|---|---|---|
| `GetServices` | `GET services` (catálogo inteiro numa resposta, sem paginação) | `MovideskServices`, `MovideskServicesCategories` | 3004 | A cada 2 horas |
| `GetServiceById` | `GET services?id={id}` | as mesmas + `MovideskRequests` (log) | 3005 | Sob demanda |

### Fluxo das leituras recorrentes (tickets e pessoas)

Seguem o padrão de [`docs/PADRAO-CACHE-INTEGRACOES.md`](../../../../../docs/PADRAO-CACHE-INTEGRACOES.md):

1. abre um log em `auditing.Logs` com o job do método;
2. para cada empresa de `MovideskParameters`, calcula a janela de busca:
   * **Tickets:** desde o maior `lastUpdate` gravado, menos 1 hora de folga. Com o banco vazio, os últimos 89 dias.
   * **Pessoas:** desde a maior data de alteração/criação gravada, menos 1 hora de folga. Com o banco vazio, os últimos 12 meses.
3. busca as páginas com `$skip` (`$top=100` nas pessoas), esperando 6 segundos entre as chamadas. Os tickets seguem um fluxo próprio, descrito logo abaixo;
4. monta um cache em memória **por empresa** (`IMemoryCache`, 2 h absoluto / 30 min deslizante) com o que já está gravado e descarta o que não mudou;
5. valida os DTOs (FluentValidation) e registra as inconsistências em `auditing.Messages`, sem barrar a gravação;
6. converte para entidades, faz o bulk insert nas tabelas `untreated` e chama as procedures de merge `general.sp_<Tabela>_Sync`, do pai para os filhos;
7. grava cada registro integrado (com o JSON recebido) em `auditing.Records`.

**Tickets** com todas as listas expandidas numa chamada levam ~100 s por página e chegam a dar 504. Por isso o `GetTickets` trabalha em duas fases ([detalhes](../../../../../docs/IMPLEMENTACAO-MOVIDESK-GETTICKETS-EXPAND-EM-GRUPOS.md)):

* **Fase 1:** lista só `id` e `lastUpdate` da janela (1.000 por chamada) e usa o cache para separar os tickets novos ou alterados.
* **Fase 2:** busca os alterados em blocos de até 100, por faixa de ids, com o `$expand` dividido em **5 chamadas** por bloco (campos, ações, clientes e itens, históricos, campos adicionais).
  * As respostas são juntadas por id e o bloco é gravado em seguida.
  * Um ticket que falte em algum grupo não é gravado (o merge apagaria as filhas que faltaram): vira mensagem e volta na próxima execução.

**Serviços** usam outro desenho, porque o catálogo é pequeno e vem inteiro numa chamada: o service calcula o hash SHA-256 da resposta de cada empresa e só regrava quando o catálogo mudou desde a última execução.

| Chave de cache | Conteúdo | Usada por |
|---|---|---|
| `Movidesk:Tickets:<cnpj>` | Tickets gravados (id → `lastUpdate`) | `GetTickets` |
| `Movidesk:People:<cnpj>` | Pessoas gravadas (id → datas) | `GetPeople` |
| `Movidesk:ServicesHash:<cnpj>` | Hash do último catálogo gravado (3 h, mais que o intervalo de 2 h do job) | `GetServices`; o `GetServiceById` remove a chave ao gravar |

### Fluxo das leituras sob demanda (um registro por id)

`GetTicketById`, `GetPersonById` e `GetServiceById` seguem o mesmo fluxo:

1. abre um log em `auditing.Logs` com o job do método;
2. busca a empresa pelo CNPJ recebido. Se ela não estiver cadastrada, devolve `null` sem chamar a API;
3. chama a API com o token da empresa e grava o registro como nas leituras recorrentes (mesmas tabelas e merges);
4. grava a chamada em `general.MovideskRequests` (via `MovideskService.SaveRequest`). Isso inclui:
   * usuário (`X-User-Name`), método, rota sem o token, CNPJ, id, resposta e se houve sucesso;
   * também quando a chamada falha antes de ter resposta (timeout, rede);
5. devolve `true` (gravado), `false` (não existe na Movidesk) ou `null` (empresa não cadastrada ou resposta inválida).

Como a API indica "não encontrado":

| Rota | Resposta para id inexistente |
|---|---|
| `tickets?id=` | 404 com corpo vazio |
| `persons?id=` | 404 com `application/problem+json` (`"status": 404`) |
| `services?id=` | 404 com corpo vazio |

---

## Infrastructure.Movidesk

```
Infrastructure.Movidesk/
├── DependencyInjection.cs      (AddScopedMovideskServices)
├── Repositorys/                (MovideskRepository, MovideskTicketsRepository, MovideskPeopleRepository, MovideskServicesRepository)
└── Scripts/
    ├── SQLServer/              (scripts DbUp + migrator.csx)
    ├── MySQL/                  (migrator.csx)
    └── PostgreSQL/             (migrator.csx)
```

### Injeção de dependência

`AddScopedMovideskServices(IServiceCollection, IConfiguration)` registra:

* `IApiClient` com a chave `"MovideskAPI"` e o `HttpClient` nomeado `"MovideskAPI"` (base `Movidesk:BaseUrl`, timeout de 20 minutos);
* os quatro services e os quatro repositórios (`Scoped`);
* `IMovideskCommandHandler`;
* todos os validadores do assembly (`AddValidatorsFromAssemblyContaining<TicketValidator>()`).

O host chama o método em `Hosts/Hangfire/Extensions/ServicesExtensions.cs`.

### Scripts de banco (SQL Server)

O DbUp (`Scripts/SQLServer/migrator.csx`) executa cada script uma única vez, em ordem alfabética. Alterar um script que já rodou não muda um banco existente: para isso é preciso um script novo.

| Script | Conteúdo |
|---|---|
| `Script001_Create_Movidesk_Parameters_Table.sql` | `general.MovideskParameters` (CNPJ + token por empresa) |
| `Script002_Create_Movidesk_Tickets_Tables.sql` | `MovideskTickets`, as tabelas filhas e netas (tags, clientes, relacionamentos, históricos, campos adicionais, itens de configuração, ações e seus tags/apontamentos/despesas/anexos), mais os domínios `MovideskTicketsOrigins` e `MovideskTicketsTypes` |
| `Script003_Create_Movidesk_Sync_Procedures.sql` | Procedures de merge dos tickets |
| `Script004_Insert_Movidesk_Values_Into_Apps_Table.sql` | App `30 - Movidesk` em `auditing.Apps` |
| `Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql` | Jobs em `auditing.Jobs` (ver [Auditoria](#auditoria-jobs)) |
| `Script006_Create_Movidesk_Requests_Table.sql` | `general.MovideskRequests` (log das chamadas sob demanda) |
| `Script007_Create_Movidesk_People_Tables.sql` | `MovideskPeople`, as filhas e netas (endereços, contatos, e-mails, equipes, relacionamentos e seus serviços, campos adicionais, itens de configuração), mais os domínios `MovideskPersonTypes` e `MovideskProfileTypes` |
| `Script008_Create_Movidesk_People_Sync_Procedures.sql` | Procedures de merge das pessoas |
| `Script009_Create_Movidesk_Services_Tables.sql` | `MovideskServices`, `MovideskServicesCategories` e os domínios `MovideskServiceTicketTypes` e `MovideskServiceAudiences` |
| `Script010_Create_Movidesk_Services_Sync_Procedures.sql` | Procedures de merge dos serviços |
| `Script011_Insert_Values_Into_Movidesk_Parameters_Table.sql` | Empresa e token em `MovideskParameters` |
| `Script012_Rename_Movidesk_On_Demand_Jobs.sql` | Renomeia os jobs sob demanda 3002, 3003 e 3005 para `MovideskGetTicketById`, `MovideskGetPersonById` e `MovideskGetServiceById` |

**Regras gerais das tabelas e dos merges:**
* Toda tabela de dados existe em dois schemas: `untreated` (recebe o bulk insert, sem chave) e `general` (tabela final, com PK). As exceções são `general.MovideskRequests`, que só recebe inserts, e as tabelas de domínio, preenchidas pelos próprios scripts.
* As procedures `general.sp_<Tabela>_Sync` são geradas por `dbo.sp_Create_Merge_Procedure`, do projeto Shared.
* Nas tabelas filhas, o merge **apaga** as linhas que não vieram no lote quando o pai foi regravado (o `integrated_or_updated_at` do pai é mais recente que o da filha). Assim, um item removido na Movidesk também sai do banco.
* As netas têm FK com `on delete cascade`.
* Os registros principais (tickets, pessoas e serviços) **nunca são apagados**: um serviço removido do catálogo continua no banco.

---

## Endpoints expostos (`MovideskController`)

Rota base: `MovideskJobs/Movidesk`.

### Leituras recorrentes (disparam a integração de todas as empresas)

| Método | Rota | Service |
|---|---|---|
| GET | `Tickets` | `MovideskTicketsService.GetTickets` |
| GET | `People` | `MovideskPeopleService.GetPeople` |
| GET | `Services` | `MovideskServicesService.GetServices` |

### Leituras sob demanda (chamadas por sistema externo)

Todas exigem:
* o cabeçalho **`X-User-Name`** (até 100 caracteres);
* o CNPJ da empresa na rota (`{docCompany}`, 14 dígitos).

Sem o cabeçalho, com o CNPJ inválido ou com um id fora do formato, a resposta é **400** (ou **404** pela restrição de rota), sem chamar a API.

| Método | Rota | Formato do id | Service |
|---|---|---|---|
| GET | `Tickets/{docCompany}/{id}` | número ≥ 1 | `GetTicketById` |
| GET | `People/{docCompany}/{id}` | Cód. Ref., só letras e números, até 64 caracteres | `GetPersonById` |
| GET | `Services/{docCompany}/{id}` | número ≥ 1 | `GetServiceById` |

Respostas:

| Código | Quando |
|---|---|
| **200** | Registro gravado |
| **404** | O registro não existe na Movidesk |
| **422** | Empresa não cadastrada ou resposta inválida da API (detalhes no log da execução) |

> O `[Authorize]` do controller está comentado, como no AfterSale, porque o host ainda não tem um esquema de autenticação configurado.

---

## Auditoria (jobs)

Os jobs ficam em `auditing.Jobs` (`Script005`), com os mesmos ids em `EnumJob`.

| Faixa | Tipo (`IdType`) | Conteúdo |
|---|---|---|
| 3000–3499 | 2 (Integração) | Leituras, com `IdParent = 30` (app Movidesk) |
| 3500–3999 | 1 (Procedure) | Procedures `sp_<Tabela>_Sync`, com `IdParent` = job da leitura (tickets 3500–3515, pessoas 3516–3525, serviços 3526–3527) |

| Id | Job | Intervalo |
|---|---|---|
| 3000 | `MovideskGetTickets` | 3 min |
| 3001 | `MovideskGetPeople` | 3 min |
| 3002 | `MovideskGetTicketById` | sob demanda |
| 3003 | `MovideskGetPersonById` | sob demanda |
| 3004 | `MovideskGetServices` | 120 min |
| 3005 | `MovideskGetServiceById` | sob demanda |

Toda execução grava `auditing.Logs` e, conforme o caso:
* `auditing.Messages`, com mensagens e erros de validação;
* `auditing.Records`, com os registros integrados e o JSON recebido.

---

## Configuração

| Onde | Chave | Uso |
|---|---|---|
| `appsettings.json` do host | `Movidesk:BaseUrl` | Endereço da API (`https://api.movidesk.com/public/v1/`) |
| `appsettings.json` do host | `ConnectionStrings` | Conexão com o SQL Server (dados e auditoria) |
| Banco | `general.MovideskParameters` | CNPJ (`doc_company`) e token de cada empresa |

Os tokens ficam só no banco e nunca são gravados em logs: a rota registrada em `MovideskRequests` não inclui o token.

---

## Pasta `Responsed Jsons`

Respostas reais da API, com os dados trocados por **dados fictícios** para não expor informações no histórico do git:

| Pasta | Conteúdo |
|---|---|
| `Ticket/GetTicketById/` | Um ticket no formulário novo e um no formulário antigo |
| `People/GetPeople/` | Página da lista de pessoas |
| `Person/GetPersonById/` | Uma pessoa de cada perfil: agente, cliente e empresa |
| `Services/GetServices/` | Catálogo de serviços |
| `Service/GetServiceById/` | Um serviço por id |

---

## Documentação complementar

Guias de modelagem, implementação e decisões tomadas, na pasta `docs/` da raiz do repositório:

| Documento | Assunto |
|---|---|
| `MOVIDESK-MODELAGEM-TICKETS.md` | Tabelas e relações dos tickets |
| `IMPLEMENTACAO-MOVIDESK-GETTICKETS.md`, `IMPLEMENTACAO-MOVIDESK-GETTICKET.md` | Leitura dos tickets (lista e por número) |
| `MOVIDESK-MODELAGEM-PESSOAS.md` | Tabelas e relações das pessoas |
| `IMPLEMENTACAO-MOVIDESK-GETPEOPLE.md`, `IMPLEMENTACAO-MOVIDESK-GETPERSON.md` | Leitura das pessoas (lista e por Cód. Ref.) |
| `IMPLEMENTACAO-MOVIDESK-SERVICES.md` | Tabelas e leitura do catálogo de serviços |
| `PADRAO-CACHE-INTEGRACOES.md` | Padrão de cache das leituras |
| `ALTERACOES-MERGE-PROCEDURE-AUDITORIA.md` | Procedure de merge e auditoria |
