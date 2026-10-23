# Movidesk — Implementação de `GetServices` e `GetServiceById`

Guia para implementar o catálogo de serviços da Movidesk no `MovideskServicesService`: tabelas, scripts, entidades,
repositório, service e endpoints. Segue as regras e decisões já aplicadas em tickets e pessoas
([MOVIDESK-MODELAGEM-TICKETS.md](MOVIDESK-MODELAGEM-TICKETS.md), [MOVIDESK-MODELAGEM-PESSOAS.md](MOVIDESK-MODELAGEM-PESSOAS.md),
[IMPLEMENTACAO-MOVIDESK-GETPERSON.md](IMPLEMENTACAO-MOVIDESK-GETPERSON.md)).

> **Implementado em 21/10/2026.** Scripts 009/010, jobs no Script005, DTO, entidades, validador, repositório, service e
> rotas da controller. Compilado sem erros; scripts validados num banco temporário (dois lotes). A modelagem foi
> conferida com respostas reais da API em 21/10/2026 (seção 2). As decisões de 21/10/2026 estão na seção 7.

Base: [documentação da API de Serviços](https://atendimento.movidesk.com/kb/pt-br/article/7440/api-servicos)
(atualizada em 27/04/2026). Raiz do módulo: `src/Backend/Modules/General/Movidesk/`. Uma classe por arquivo `.cs`.

---

## 1. O que a API oferece

| | Detalhe |
|---|---|
| Rota | `GET /services` (também `POST`, `PATCH`, `DELETE`, fora deste guia) |
| Um serviço | `GET /services?token=...&id=...` |
| Lista | `GET /services?token=...`, com OData (`$filter`, `$orderby`, `$top`, `$skip`, `$select`) |
| Campos | `id`, `name`, `description`, `parentServiceId`, `serviceForTicketType`, `isVisible`, `allowSelection`, `allowFinishTicket`, `isActive`, `automationMacro`, `defaultCategory`, `defaultUrgency`, `allowAllCategories`, `categories[]` |
| Data de alteração | **Não há.** Diferente de tickets (`lastUpdate`) e pessoas (`changedDate`), não dá para buscar só o que mudou. |
| Hierarquia | `parentServiceId` aponta para o serviço pai; um serviço filho de um desabilitado volta como desabilitado (`isActive = false`), mesmo se estiver habilitado individualmente. |

---

## 2. Comportamento real da API (verificado em 21/10/2026)

| Chamada | Resultado |
|---|---|
| `GET /services` (sem `$top`) | **HTTP 200, 65 serviços numa única resposta (23 KB)**: o catálogo inteiro vem sem paginação. |
| `GET /services?id=<existente>` | HTTP 200, um **objeto** (não lista) com os 14 campos. |
| `GET /services?id=999999999` | **HTTP 404 com corpo vazio**, como em tickets (pessoas devolve um JSON de erro). |

O que a lista mostrou:

| Ponto | Valor real | Consequência |
|---|---|---|
| Tipos | `id` e `parentServiceId` `int`; `serviceForTicketType`, `isVisible`, `allowSelection` `int`; booleanos `bool`; `categories` lista de texto | Batem com a documentação. |
| `allowSelection` | Valores `0`, `1` e `3` | **`0` não está na documentação** (que lista 1, 2 e 3); provavelmente "ninguém". A coluna aceita qualquer `int`. |
| `isVisible` | `1` e `3` | Documentados. |
| `serviceForTicketType` | Só `2` | Documentado (Públicos e internos). |
| Ativos | 40 de 65 | Os inativos também vêm na lista; não filtrar por `isActive`. |
| Raízes | 10 serviços sem pai | — |
| **Profundidade** | **até 4 níveis** (4 serviços no 4º nível) | Ver 3.4: o ticket só tem colunas para 3 níveis. |
| `description` | até 268 caracteres | A documentação não dá tamanho: `nvarchar(max)`. |
| `name` | até 48 caracteres | Documentado: 128. |
| `categories` | Só 2 serviços têm categorias (1 cada) | Tabela filha pequena. |
| Pais | Todo `parentServiceId` aponta para um serviço da própria lista | Dá para montar a árvore inteira só com a tabela. |

As respostas estão, com dados mockados (nomes, descrições, categorias, macros e ids trocados, mantendo os vínculos de pai e
filho), em `Responsed Jsons/Services/GetServices/json-returned-by-api-end-point.json` (lista) e
`Responsed Jsons/Service/GetServiceById/json-returned-by-api-end-point.json` (um serviço).

---

## 3. Modelagem

### 3.1 Tabelas
Mesmas regras de tickets e pessoas: `untreated` + `movidesk` com merge gerado, `integrated_or_updated_at` e
`doc_company` (primeira coluna da PK), `nvarchar`, nomes iguais às propriedades das entidades.

| Tabela | Origem na API | Chave primária |
|---|---|---|
| `MovideskServices` | `service` | `doc_company, id` |
| `MovideskServicesCategories` | `categories[]` | `doc_company, serviceId, category` |

```sql
create table [movidesk].MovideskServices (
	integrated_or_updated_at	datetime,
	doc_company					nvarchar(14) not null,
	id							int not null,
	[name]						nvarchar(128),
	[description]				nvarchar(max),
	parentServiceId				int,			-- serviço pai (nulo na raiz)
	serviceForTicketType		int,			-- 0 Público, 1 Interno, 2 Públicos e internos
	isVisible					int,			-- 1 Agente, 2 Cliente, 3 Agente e Cliente
	allowSelection				int,			-- 1 Agente, 2 Cliente, 3 Agente e Cliente (0 visto na resposta real, não documentado)
	allowFinishTicket			bit,
	isActive					bit,			-- já vem false para filhos de um serviço desabilitado
	automationMacro				nvarchar(64),
	defaultCategory				nvarchar(128),
	defaultUrgency				nvarchar(128),
	allowAllCategories			bit,
constraint [PK_MovideskServices] primary key (doc_company asc, id asc));

create table [movidesk].MovideskServicesCategories (
	integrated_or_updated_at	datetime,
	doc_company					nvarchar(14) not null,
	serviceId					int not null,
	category					nvarchar(128) not null,
constraint [PK_MovideskServicesCategories] primary key (doc_company asc, serviceId asc, category asc));

alter table [movidesk].MovideskServicesCategories add constraint [FK_MovideskServicesCategories] foreign key (doc_company, serviceId) references [movidesk].MovideskServices (doc_company, id);
```
- **Sem FK em `parentServiceId`:** o pai e o filho chegam no mesmo lote, e a ordem de inserção dentro do merge não é
  garantida. A hierarquia fica por junção (`parentServiceId = id`).
- **Categorias:** a chave é o próprio nome (como as tags dos tickets); repetidos são descartados na entidade.

### 3.2 Tabelas de valores (decidido: criar)
Para leitura dos códigos, no mesmo padrão das origens de ticket e tipos de pessoa (sem FK, sem `doc_company`):
```sql
create table [movidesk].MovideskServiceTicketTypes (id int not null primary key, [description] nvarchar(60) not null);
insert into [movidesk].MovideskServiceTicketTypes (id, [description]) values (0, N'Público'), (1, N'Interno'), (2, N'Públicos e internos');

create table [movidesk].MovideskServiceAudiences (id int not null primary key, [description] nvarchar(60) not null);
insert into [movidesk].MovideskServiceAudiences (id, [description]) values (0, N'Ninguém (não documentado)'), (1, N'Agente'), (2, N'Cliente'), (3, N'Agente e Cliente');
```
`MovideskServiceAudiences` serve para `isVisible` e `allowSelection`, que usam os mesmos códigos.

### 3.3 Serviços apagados e itens removidos (decidido: manter)
- **Categorias removidas** de um serviço: mesma condição de exclusão dos tickets e pessoas, pelo serviço pai:
  ```sql
  exists (select 1 from [movidesk].MovideskServices p where p.doc_company = target.doc_company and p.id = target.serviceId and p.integrated_or_updated_at > target.integrated_or_updated_at)
  ```
- **Serviços apagados** na Movidesk (`DELETE /services`): **ficam** em `MovideskServices`. Os tickets antigos continuam
  apontando para eles (`serviceFirstLevelId`), e apagar quebraria a junção. Um serviço que não deve mais ser usado
  normalmente é desabilitado (`isActive = false`), e isso a sincronização pega.

### 3.4 Relação com os tickets (verificado com tickets reais)
Conferido em 21/10/2026 com os tickets 69255 (serviço de 3 níveis) e 1000 (2 níveis) e com 3 tickets de
`/tickets/past` cujo serviço está no **4º nível**, comparando com o catálogo real:

| Campo do ticket | O que guarda |
|---|---|
| `serviceFirstLevelId` | **id do serviço selecionado** (o mais fundo da árvore, sem filhos no catálogo) |
| `serviceFirstLevel` | nome do serviço selecionado |
| `serviceSecondLevel` | nome do **pai** do selecionado |
| `serviceThirdLevel` | nome do **avô** do selecionado |
| `serviceFull` | todos os nomes, **da raiz até o selecionado** |

Ou seja, a API numera os "níveis" de baixo para cima, a partir do serviço selecionado. Num serviço de 4 níveis, **a
raiz não tem coluna no ticket**: só aparecia no `serviceFull`, que a modelagem de tickets deixou de gravar.

Com `MovideskServices` carregada, o caminho completo sai da hierarquia a partir de `serviceFirstLevelId`:
```sql
with caminho as (
	select s.doc_company, s.id, s.parentServiceId, s.[name], 1 as nivel_a_partir_do_selecionado
	from [movidesk].MovideskServices s
	join [movidesk].MovideskTickets t on t.doc_company = s.doc_company and t.serviceFirstLevelId = s.id
	where t.id = @ticketId
	union all
	select p.doc_company, p.id, p.parentServiceId, p.[name], c.nivel_a_partir_do_selecionado + 1
	from [movidesk].MovideskServices p
	join caminho c on c.doc_company = p.doc_company and c.parentServiceId = p.id
)
select * from caminho order by nivel_a_partir_do_selecionado desc;   -- da raiz até o selecionado
```
Por isso a tabela de serviços também cobre o que o `serviceFull` guardaria, sem precisar voltar a gravá-lo. A
modelagem de tickets foi corrigida com essa leitura dos níveis.

---

## 4. Scripts e jobs

| Arquivo | Conteúdo |
|---|---|
| `Script009_Create_Movidesk_Services_Tables.sql` | 3.1 (untreated + movidesk + FK) e as tabelas de valores de 3.2 |
| `Script010_Create_Movidesk_Services_Sync_Procedures.sql` | `sp_Create_Merge_Procedure` de `MovideskServices` (sem `@delete_condition`) e de `MovideskServicesCategories` (com a condição de 3.3) |
| `Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql` | acrescentar os jobs abaixo (mesma decisão de pessoas) |

| IdJob | Título | IdParent | IntervalTimeInMinutes | IdType |
|---|---|---|---|---|
| 3004 | `MovideskGetServices` | 30 | **120** (a cada 2 horas) | 2 |
| 3005 | `MovideskGetServiceById` (sob demanda) | 30 | 0 | 2 |
| 3526 | `sp_MovideskServices_Sync` | 3004 | 120 | 1 |
| 3527 | `sp_MovideskServicesCategories_Sync` | 3004 | 120 | 1 |

Banco local/existente: inserir as quatro linhas à mão (o `Script005` não roda de novo onde já rodou).

**Frequência (decidido):** o catálogo muda pouco e o cache de hash evita gravar uma resposta igual, então o
`GetServices` roda **a cada 2 horas** (menos que os 3 minutos dos outros jobs). O `IntervalTimeInMinutes` é só
informativo hoje (nenhum código o lê); o agendamento de fato fica no Hangfire, com a mesma frequência. Se um serviço novo
precisar estar na tabela antes da próxima execução, o `GetServiceById` (5.7) traz um serviço específico na hora.

---

## 5. Código

### 5.1 `EnumJob`
```csharp
MovideskGetTickets = 3000,
MovideskGetPeople = 3001,
MovideskGetServices = 3004,

/*Sob demanda*/
MovideskGetTicketById = 3002,
MovideskGetPersonById = 3003,
MovideskGetServiceById = 3005
```

### 5.2 DTO (`Dtos/Reads/Services/Service.cs`)
O DTO atual tem todos os campos; muda só o padrão:
- `{ get; set; }` → `{ get; init; }`, como os DTOs de tickets e pessoas;
- `string[] categories` → `List<string> categories`.

### 5.3 Entidades (`Entities/Reads/Services/`)
`Service.cs`
```csharp
using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Services
{
    public class Service
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public int id { get; private set; }
        public string name { get; private set; }
        public string? description { get; private set; }
        public int? parentServiceId { get; private set; }
        public int? serviceForTicketType { get; private set; }
        public int? isVisible { get; private set; }
        public int? allowSelection { get; private set; }
        public bool? allowFinishTicket { get; private set; }
        public bool? isActive { get; private set; }
        public string automationMacro { get; private set; }
        public string defaultCategory { get; private set; }
        public string defaultUrgency { get; private set; }
        public bool? allowAllCategories { get; private set; }

        [SkipProperty]
        public List<ServiceCategory> categories { get; private set; } = [];

        public Service() { }

        public Service(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Services.Service dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.id);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128);
            description = dto.description;   // nvarchar(max)
            parentServiceId = NullableConverters.ToNullableInt32(dto.parentServiceId);
            serviceForTicketType = NullableConverters.ToNullableInt32(dto.serviceForTicketType);
            isVisible = NullableConverters.ToNullableInt32(dto.isVisible);
            allowSelection = NullableConverters.ToNullableInt32(dto.allowSelection);
            allowFinishTicket = NullableConverters.ToNullableBoolean(dto.allowFinishTicket);
            isActive = NullableConverters.ToNullableBoolean(dto.isActive);
            automationMacro = ConverterValidatorsExtensions.StringLengthValidation(dto.automationMacro, 64);
            defaultCategory = ConverterValidatorsExtensions.StringLengthValidation(dto.defaultCategory, 128);
            defaultUrgency = ConverterValidatorsExtensions.StringLengthValidation(dto.defaultUrgency, 128);
            allowAllCategories = NullableConverters.ToNullableBoolean(dto.allowAllCategories);

            // categorias repetidas violariam a PK (o SQL Server não diferencia maiúsculas de minúsculas)
            foreach (var category in (dto.categories ?? []).Where(c => !String.IsNullOrWhiteSpace(c)).Distinct(StringComparer.OrdinalIgnoreCase))
                categories.Add(new ServiceCategory(doc_company, integrated_or_updated_at, id, category));
        }
    }
}
```
`ServiceCategory.cs`: `integrated_or_updated_at`, `doc_company`, `serviceId` (`int`), `category` (128); construtor
`(doc_company, integrated_or_updated_at, int serviceId, string category)` e construtor vazio, no molde do `TicketTag`.

Nome da classe: `Service` já existe em `Entities/Reads/People` (serviço permitido numa organização); os namespaces
diferentes (`...Reads.Services` e `...Reads.People`) evitam o conflito, como acontece com os DTOs.

### 5.4 Validador (`Validations/Services/ServiceValidator.cs`)
Só registra mensagens: `id` com `MustBeValidInt32`; `name` 128; `automationMacro` 64; `defaultCategory`,
`defaultUrgency` 128; `parentServiceId`, `serviceForTicketType`, `isVisible`, `allowSelection` com `MustBeValidInt32`;
os três booleanos com `MustBeValidBoolean`; cada categoria até 128. Registrado automaticamente pelo
`AddValidatorsFromAssemblyContaining<TicketValidator>`.

### 5.5 Repositório
`IMovideskServicesRepository` (hoje vazio):
```csharp
public Task<bool> BulkInsertIntoMovideskServices(List<Domain.Movidesk.Entities.Reads.Services.Service> data, Guid? parentExecutionGUID);
```
`MovideskServicesRepository`: mesmo molde do `BulkInsertIntoMovideskPeople`, com duas tabelas e a ordem dos merges
`MovideskServices` → `MovideskServicesCategories`, os dois sempre (o merge das categorias apaga as removidas). Não há
consulta por ids: o catálogo inteiro vem a cada vez (5.6).

### 5.6 `GetServices`
O catálogo é pequeno e vem inteiro numa resposta, sem data de alteração, então o padrão é o **cache de hash** do
[PADRAO-CACHE-INTEGRACOES.md](PADRAO-CACHE-INTEGRACOES.md) (§3.3), como o `GetStatus` do AfterSale: se a resposta for
igual à da última execução, nada é gravado.
```csharp
private const string ServicesHashKey = "Movidesk:ServicesHash";

/// <summary>
/// sincroniza o catálogo de serviços (inteiro, numa chamada) de cada empresa; só grava quando a resposta mudou
/// </summary>
public async Task<bool?> GetServices()
{
    _logger
        .Clear()
        .AddLog(EnumJob.MovideskGetServices);

    var totalInseridos = 0;
    var companys = await _movideskRepository.GetCompanys();

    foreach (var company in companys)
    {
        if (company?.doc_company is null)
            continue;

        var response = await _apiClient.GetAsync(
            route: "services",
            queryParams: new Dictionary<string, string> { { "token", company.Token.ToString() } }
        );

        List<Domain.Movidesk.Dtos.Reads.Services.Service>? services;

        try
        {
            services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.Services.Service>>(response ?? String.Empty);
        }
        catch (Newtonsoft.Json.JsonException)
        {
            _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}): {response}");
            continue;
        }

        if (services is null || services.Count == 0)
            continue;

        // cache de hash por empresa: catálogo igual ao da última execução não é regravado
        var hash = _logger.ComputeSha256Hash(response!);
        var hashKey = $"{ServicesHashKey}:{company.doc_company}";

        if (hash.Equals(_cache.Get<string>(hashKey)))
            continue;

        var servicesJson = JArray.Parse(response!)
            .Where(x => x["id"] is not null)
            .ToDictionary(x => ConverterValidatorsExtensions.ConvertToInt32Validation((string?)x["id"]), x => x.ToString());

        totalInseridos += await SaveServices(company.doc_company, _movideskService.GetBatchTime(), services, servicesJson);

        // 3 horas: precisa durar mais que o intervalo do job (2 horas) para a próxima execução ainda encontrar o hash
        _cache.Set(hashKey, hash, TimeSpan.FromHours(3));
    }

    _logger.AddMessage($"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!");

    _logger.SetLogEndDate();
    await _logger.CommitAllChanges();

    return true;
}
```
- **Uma chamada por empresa**, sem paginação e sem `Thread.Sleep`: a lista real veio inteira. Se um dia o catálogo
  passar de algumas centenas, a paginação do `GetPeople` serve de modelo.
- **Sem `$filter` por `isActive`:** os desabilitados também são gravados (os tickets antigos apontam para eles).
- **Hash depois de gravar:** se a gravação falhar, o hash não é salvo e a próxima execução tenta de novo.
- **Hash com duração de 3 horas**, não as 2 horas padrão do projeto: com o job a cada 2 horas, um hash de 2 horas
  expiraria no mesmo instante da execução seguinte e o catálogo seria regravado toda vez. Com 3 horas, a execução
  seguinte ainda encontra o hash e só grava se a resposta mudou. Sem expiração deslizante (`SlidingExpiration`): ela
  mataria a entrada antes da próxima execução, porque ninguém consulta o hash no intervalo.
- `SaveServices` é o equivalente do `SavePeople`: valida, monta as entidades com o horário do lote, grava, registra o
  JSON de cada serviço (`_logger.AddRecord(id.ToString(), json)`) e devolve a quantidade.

### 5.7 `GetServiceById` (um serviço por id)
Mesmo desenho do `GetTicketById`: `X-User-Name` obrigatório, registro em `movidesk.MovideskRequests` pelo
`_movideskService.SaveRequest`, job 3005.
```csharp
public Task<bool?> GetServiceById(string user_name, string doc_company, int id);
```
| Situação | Como o service reconhece | Resultado |
|---|---|---|
| Serviço existe | objeto com o mesmo `id` | grava (`SaveServices` com uma lista de um) e devolve `true` |
| Não existe | **corpo vazio** (404, como tickets) | `false` |
| Empresa não cadastrada | `GetCompany` nulo | `null`, sem registro em `MovideskRequests` |
| Resposta inválida | não desserializa ou `id` diferente | `null` |

- Rota registrada no log: `services?id={id}` (sem o token).
- Depois de gravar, **apaga o hash** da empresa (`_cache.Remove(hashKey)`), para o `GetServices` seguinte não pular uma
  resposta que mudou por causa dessa gravação.

### 5.8 `MovideskServicesService`
- Construtor: hoje recebe `IApiClient`, `ILoggerService`, `IMovideskRepository` e `IMovideskServicesRepository`; passa a
  receber também `IMovideskService`, `IValidator<Domain.Movidesk.Dtos.Reads.Services.Service>` e `IMemoryCache`.
- `IMovideskServicesService`: `GetServices()` e `GetServiceById(string user_name, string doc_company, int id)`.

### 5.9 Controller
`MovideskController` injeta `IMovideskServicesService` e troca o endpoint comentado por:
```csharp
[HttpGet("Services")]
[Tags("MovideskServices")]
public async Task<ActionResult> Services()
{
    var result = await _movideskServicesService.GetServices();

    return Ok($"Records integrated successfully.");
}

/// <summary>
/// get one service by id (GET services?id={id}) and save it; the call is logged in movidesk.MovideskRequests
/// </summary>
[HttpGet("Services/{docCompany}/{id:int:min(1)}")]
[Tags("MovideskServices")]
public async Task<ActionResult> Service(
    [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
    [RegularExpression(@"^\d{14}$")] string docCompany,
    int id
)
{
    var result = await _movideskServicesService.GetServiceById(userName, docCompany, id);

    return result switch
    {
        true => Ok($"Service {id} integrated successfully."),
        false => NotFound($"Service {id} not found in Movidesk."),
        null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
    };
}
```

---

## 6. Checklist

- [ ] `Script009` (tabelas e as duas tabelas de valores) e `Script010` (merges); jobs 3004 (120 minutos), 3005, 3526 e 3527 no `Script005`
- [ ] `EnumJob.MovideskGetServices` e `MovideskGetServiceById`
- [ ] DTO com `init` e `List<string> categories`
- [ ] Entidades `Service` e `ServiceCategory` em `Entities/Reads/Services`, uma por arquivo
- [ ] `ServiceValidator` em `Validations/Services`
- [ ] `IMovideskServicesRepository` / `MovideskServicesRepository` com os 2 merges sempre
- [ ] `MovideskServicesService`: `GetServices` (cache de hash), `GetServiceById` (404 vazio, `SaveRequest`), `SaveServices`
- [ ] Endpoints `GET Services` e `GET Services/{docCompany}/{id}`
- [ ] Compilar o host; comparar as entidades com o `Script009`; validar os scripts num banco temporário
- [ ] Banco local: rodar `Script009`/`Script010` e inserir os 4 jobs à mão

## 7. Decisões (aplicadas em 21/10/2026)

| # | Decisão | Onde |
|---|---|---|
| 1 | Criar `MovideskServiceTicketTypes` e `MovideskServiceAudiences` | 3.2, 4 |
| 2 | Serviços apagados na Movidesk ficam em `MovideskServices` | 3.3 |
| 3 | `GetServices` a cada 2 horas (`IntervalTimeInMinutes = 120`), com o hash durando 3 horas | 4, 5.6 |
| 4 | Conferido com tickets reais: `serviceFirstLevelId` é o serviço selecionado; o caminho completo sai de `MovideskServices` | 3.4 |

Não há decisões pendentes neste guia.
