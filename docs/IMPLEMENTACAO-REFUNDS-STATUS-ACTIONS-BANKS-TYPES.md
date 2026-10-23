# Implementação — `GetStatus`, `GetActions`, `GetBanks` e `GetTypes` de reembolsos

Guia para aplicação **manual**. Nada foi alterado no código.

Fonte: especificação OpenAPI 1.0.8 (`api.swaggerhub.com/apis/send42/public-api/1.0.8`) e as
respostas salvas em `Responsed Jsons/Refund/{Actions,Banks,Status,Types}`.

As quatro rotas são **tabelas de domínio** (listas pequenas, sem paginação), então o padrão de
referência é o `AfterSaleReversesService.GetStatus` (status de reversas), e não o `GetReverses`:

- uma chamada por empresa, com o token da empresa;
- o DTO recebe `doc_company` com `[JsonIgnore]` e o service preenche depois de desserializar;
- cache por empresa (`"<chave>:<doc_company>"`) com o que já está no banco;
- grava só o que é novo ou mudou; o CNPJ faz parte da PK.

| Método | Rota da API | Job (`EnumJob`) | Tabela | Procedure de merge |
|---|---|---|---|---|
| `GetStatus` | `GET v3/api/refunds/status` | `AfterSaleStatusRefunds = 5013` | `AfterSaleStatusRefunds` | `sp_AfterSaleStatusRefunds_Sync` |
| `GetActions` | `GET v3/api/refunds/actions` | `AfterSaleActionsRefunds = 5014` | `AfterSaleActionsRefunds` | `sp_AfterSaleActionsRefunds_Sync` |
| `GetBanks` | `GET v3/api/refunds/banks` | `AfterSaleBanksRefunds = 5015` | `AfterSaleBanksRefunds` | `sp_AfterSaleBanksRefunds_Sync` |
| `GetTypes` | `GET v3/api/refunds/types` | `AfterSaleTypesRefunds = 5016` | `AfterSaleTypesRefunds` | `sp_AfterSaleTypesRefunds_Sync` |

Os jobs (`Script007`, linhas 32–35 e 59–62), os valores do `EnumJob` e as procedures de merge
(`Script005`, linhas 19–22) **já existem**. Nada a inserir na auditoria.

---

## 1. As rotas na documentação 1.0.8

Todas: sem parâmetros, `Authorization: Bearer <token>`, `Accept: application/json`, resposta
`{ "success": bool, "data": [ ... ] }`, erro documentado só `401`.

| Rota | Itens de `data` (doc 1.0.8) |
|---|---|
| `/v3/api/refunds/status` | `id` (number), `name`, `description` |
| `/v3/api/refunds/actions` | `id` (number), `name`, `description` |
| `/v3/api/refunds/banks` | `id` (number), `name`, `description` |
| `/v3/api/refunds/types` | `name`, `description` — **sem `id`** |

> **`types` sem `id`:** a doc 1.0.8 não traz `id`, e o DTO atual também não tem. O arquivo
> `Responsed Jsons/Refund/Types/json-returned-by-api-end-point.json` mostra `id`, mas os valores
> (`0`, `"string"`) indicam que é um modelo, não uma resposta real. Hoje a tabela tem um `id`
> "gerado com incremental no C#", o que é frágil: se a API mudar a ordem da lista, o mesmo tipo
> ganha outro id. **Decisão:** seguir o que fizemos em `AfterSaleTransportationsReverses` (que também
> não tem id na API): a coluna `id` sai e a chave passa a ser **`(doc_company, name)`**. Ver passos
> 2.4, 3.4 e 6.5.

## Decisões

| Ponto | Decisão |
|---|---|
| Chave de `types` | PK `(doc_company, name)`, sem `id` |
| `Bank.name` | Cortado em **250** caracteres (tamanho da coluna e do `BankValidation`), não em 50 |
| `AfterSaleErrors` quando `success = false` | **Pendente** — fora deste guia, a decidir depois (ver seção 5) |

---

## 2. O que precisa mudar (resumo)

| # | Onde | O quê |
|---|---|---|
| 1 | DTOs (`Dtos/Reads/Refunds/{Status,Actions,Banks,Types}`) | `doc_company` com `[JsonIgnore]` no item; `Rootobject` → `Root` em `Actions` |
| 2 | Entidades (`Entities/Reads/Refunds/{Status,Actions,Banks,Types}`) | `doc_company`; `Types` sem `id`; `Bank.name` com 250 caracteres |
| 3 | `Script003` | `doc_company` nas 8 tabelas e PK com CNPJ |
| 4 | `IAfterSaleCommandHandler` / `AfterSaleCommandHandler` | 4 queries filtradas por CNPJ |
| 5 | `IAfterSaleRefundsRepository` / `AfterSaleRefundsRepository` | 4 leituras + 4 bulk inserts (no lugar dos stubs `InsertIntoAfterSaleRefunds{Status,Actions,Banks,Types}`) |
| 6 | `AfterSaleRefundsService` | 4 validadores e 4 chaves de cache no construtor; os 4 métodos |
| 7 | `AfterSaleController` | `GET Refunds/Status`, `Refunds/Actions`, `Refunds/Banks`, `Refunds/Types` |

---

## 3. Passo a passo

Aplique de baixo para cima dentro de cada arquivo.

### Passo 1 — DTOs

Mesmo padrão do DTO de status de reversas
([`Dtos/Reads/Reverses/Status/Status.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Reverses/Status/Status.cs)).

**1.1** Em cada um dos 4 arquivos de item, adicionar no topo `using Newtonsoft.Json;` e, como
última propriedade da classe:

```csharp

        [JsonIgnore] // não vem da API; preenchido no service após a desserialização
        public string? doc_company { get; set; }
```

| Arquivo | Após a linha |
|---|---|
| [`Status/Status.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Refunds/Status/Status.cs) | 11 (`description`) |
| [`Actions/Action.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Refunds/Actions/Action.cs) | 11 (`description`) |
| [`Banks/Bank.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Refunds/Banks/Bank.cs) | 11 (`description`) |
| [`Types/Type.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Refunds/Types/Type.cs) | 10 (`description`) |

**1.2** [`Actions/Root.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Refunds/Actions/Root.cs),
linha 7: `public class Rootobject` → `public class Root` (é o único `Root` com outro nome; nada usa
`Rootobject` hoje).

### Passo 2 — Entidades

Mesmo padrão da entidade de status de reversas: `doc_company` como **segundo** parâmetro do
construtor (`Status(statusDto, doc_company)`).

**2.1** [`Entities/Reads/Refunds/Status/Status.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reads/Refunds/Status/Status.cs)
— conteúdo da classe:

```csharp
    public class Status
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public int id { get; private set; }
        public string? doc_company { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Status() { }

        public Status(Dtos.Reads.Refunds.Status.Status statusDto, string doc_company)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(statusDto.id);

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
            name = statusDto.name?.Length > 50 ? statusDto.name?.Substring(0, 50) : statusDto.name;
            description = statusDto.description?.Length > 250 ? statusDto.description?.Substring(0, 250) : statusDto.description;
        }
    }
```

**2.2** [`Entities/Reads/Refunds/Actions/Action.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reads/Refunds/Actions/Action.cs)
— igual ao 2.1, trocando `Status`→`Action`, `statusDto`→`actionDto` e o tipo do DTO para
`Dtos.Reads.Refunds.Actions.Action`. O `using System.Xml.Linq;` (linha 5) não é usado e pode sair.

**2.3** [`Entities/Reads/Refunds/Banks/Bank.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reads/Refunds/Banks/Bank.cs)
— igual ao 2.1, trocando `Status`→`Bank`, `statusDto`→`bankDto`, DTO `Dtos.Reads.Refunds.Banks.Bank`
e **`name` cortado em 250** (não 50):

```csharp
            name = bankDto.name?.Length > 250 ? bankDto.name?.Substring(0, 250) : bankDto.name;
```

> Hoje a entidade corta `name` em 50, mas a coluna `AfterSaleBanksRefunds.name` e o
> `BankValidation` usam 250. Nomes de banco longos estavam sendo truncados sem necessidade.

**2.4** [`Entities/Reads/Refunds/Types/Type.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reads/Refunds/Types/Type.cs)
— sem `id`, com `name` como chave:

```csharp
    public class Type
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public string? doc_company { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Type() { }

        public Type(Dtos.Reads.Refunds.Types.Type typeDto, string doc_company)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
            name = typeDto.name?.Length > 50 ? typeDto.name?.Substring(0, 50) : typeDto.name;
            description = typeDto.description?.Length > 250 ? typeDto.description?.Substring(0, 250) : typeDto.description;
        }
    }
```

O `using System.Xml.Linq;` (linha 4) não é usado e pode sair.

### Passo 3 — Tabelas (`Script003`)

Arquivo: [`Infrastructure.AfterSale/Scripts/SQLServer/Script003_Create_Refunds_Tables.sql`](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script003_Create_Refunds_Tables.sql)

Mesmo desenho de `AfterSaleStatusReverses` no `Script004`. Aplique de baixo para cima.

**3.4 `AfterSaleTypesRefunds` — linhas 503–515** → substituir por:

```sql
create table [untreated].AfterSaleTypesRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	[name] nvarchar(50),
	[description] nvarchar(250)
);

create table [aftersale].AfterSaleTypesRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(50) not null,
	[description] nvarchar(250),
constraint [PK_AfterSaleTypesRefunds] primary key (
	doc_company asc,
	[name] asc
));
```

**3.3 `AfterSaleBanksRefunds` — linhas 489–501** → substituir por:

```sql
create table [untreated].AfterSaleBanksRefunds (
	integrated_or_updated_at datetime,
	id int,
	[doc_company] nvarchar(14),
	[name] nvarchar(250),
	[description] nvarchar(250)
);

create table [aftersale].AfterSaleBanksRefunds (
	integrated_or_updated_at datetime,
	id int not null,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(250),
	[description] nvarchar(250),
constraint [PK_AfterSaleBanksRefunds] primary key (
	id asc,
	doc_company asc
));
```

**3.2 `AfterSaleActionsRefunds` — linhas 475–487** e **3.1 `AfterSaleStatusRefunds` — linhas
461–473**: mesmo formato do 3.3, com `[name] nvarchar(50)` e a constraint
`[PK_AfterSaleActionsRefunds]` / `[PK_AfterSaleStatusRefunds]`.

> A ordem `(id, doc_company)` na PK segue `AfterSaleStatusReverses`. As procedures de merge são
> geradas no `Script005`, que roda depois, então num banco novo já nascem com a PK nova.

### Passo 4 — Queries

**Interface** [`IAfterSaleCommandHandler.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs),
após a linha 12 (`CreateGetSummarizedRefundsByIdsQuery`):

```csharp
        string CreateGetStatusRefundsQuery(string doc_company);
        string CreateGetActionsRefundsQuery(string doc_company);
        string CreateGetBanksRefundsQuery(string doc_company);
        string CreateGetTypesRefundsQuery(string doc_company);
```

**Implementação** [`AfterSaleCommandHandler.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs),
após o `CreateGetSummarizedRefundsByIdsQuery` (termina na linha 95):

```csharp
        public string CreateGetStatusRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALESTATUSREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetActionsRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALEACTIONSREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBanksRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALEBANKSREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTypesRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALETYPESREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }
```

### Passo 5 — Repositório de reembolsos

**Interface** [`IAfterSaleRefundsRepository.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Abstractions/Repositorys/IAfterSaleRefundsRepository.cs)
— **substituir as linhas 9–12** (os stubs `InsertIntoAfterSaleRefunds{Status,Actions,Banks,Types}`,
sem parâmetro e sem uso) por:

```csharp
        public Task<List<Entities.Reads.Refunds.Status.Status?>> GetStatus(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleStatusRefunds(List<Entities.Reads.Refunds.Status.Status> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Refunds.Actions.Action?>> GetActions(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleActionsRefunds(List<Entities.Reads.Refunds.Actions.Action> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Refunds.Banks.Bank?>> GetBanks(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleBanksRefunds(List<Entities.Reads.Refunds.Banks.Bank> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Refunds.Types.Type?>> GetTypes(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleTypesRefunds(List<Entities.Reads.Refunds.Types.Type> data, Guid? parentExecutionGUID);
```

**Implementação** [`AfterSaleRefundsRepository.cs`](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Repositorys/AfterSaleRefundsRepository.cs)
— **remover os 4 stubs** correspondentes (linhas 23–41: `InsertIntoAfterSaleRefundsActions`,
`...Banks`, `...Status`, `...Types`) e adicionar, no mesmo molde de `GetStatus` /
`BulkInsertIntoAfterSaleStatusReverses` do repositório de reversas:

```csharp
        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status?>> GetStatus(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetStatusRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Status.Status>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleStatusRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status> data, Guid? parentExecutionGUID)
        {
            var statusRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Status.Status(), tableName: "AfterSaleStatusRefunds");

            _coreRepository.FillSystemDataTable(statusRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(statusRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", statusRefundsTable.TableName, parentExecutionGUID);

            return true;
        }
```

Repetir os dois métodos para as outras três, trocando:

| | Status | Actions | Banks | Types |
|---|---|---|---|---|
| Entidade | `...Refunds.Status.Status` | `...Refunds.Actions.Action` | `...Refunds.Banks.Bank` | `...Refunds.Types.Type` |
| Query | `CreateGetStatusRefundsQuery` | `CreateGetActionsRefundsQuery` | `CreateGetBanksRefundsQuery` | `CreateGetTypesRefundsQuery` |
| Tabela (`tableName`) | `AfterSaleStatusRefunds` | `AfterSaleActionsRefunds` | `AfterSaleBanksRefunds` | `AfterSaleTypesRefunds` |
| Métodos | `GetStatus` / `BulkInsertIntoAfterSaleStatusRefunds` | `GetActions` / `BulkInsertIntoAfterSaleActionsRefunds` | `GetBanks` / `BulkInsertIntoAfterSaleBanksRefunds` | `GetTypes` / `BulkInsertIntoAfterSaleTypesRefunds` |

### Passo 6 — Service

Arquivo: [`AfterSaleRefundsService.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Services/AfterSaleRefundsService.cs)

**6.1 Campos, chaves e construtor** — substituir o bloco das linhas 18–39 (dos campos até o fim do
construtor) por:

```csharp
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleRepository _afterSaleRepository;
        private readonly IAfterSaleRefundsRepository _afterSaleRefundsRepository;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Refund> _refundValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund> _summarizedRefundsValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Status.Status> _statusValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Actions.Action> _actionValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Banks.Bank> _bankValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Types.Type> _typeValidator;

        private const string GetRefundHashesKey = "AfterSale:GetRefundHashes";
        private const string SummarizedRefundsKey = "AfterSale:SummarizedRefunds";
        private const string StatusRefundsKey = "AfterSale:StatusRefunds";
        private const string ActionsRefundsKey = "AfterSale:ActionsRefunds";
        private const string BanksRefundsKey = "AfterSale:BanksRefunds";
        private const string TypesRefundsKey = "AfterSale:TypesRefunds";

        public AfterSaleRefundsService(
            [FromKeyedServices("AfterSaleAPI")] IApiClient apiClient,
            ILoggerService logger,
            IAfterSaleRepository afterSaleRepository,
            IAfterSaleRefundsRepository afterSaleRefundsRepository,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Refund> refundValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund> summarizedRefundsValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Status.Status> statusValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Actions.Action> actionValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Banks.Bank> bankValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Types.Type> typeValidator,
            IMemoryCache cache
        ) =>
        (_apiClient, _logger, _afterSaleRepository, _afterSaleRefundsRepository, _refundValidator, _summarizedRefundsValidator, _statusValidator, _actionValidator, _bankValidator, _typeValidator, _cache) =
        (apiClient, logger, afterSaleRepository, afterSaleRefundsRepository, refundValidator, summarizedRefundsValidator, statusValidator, actionValidator, bankValidator, typeValidator, cache);
```

> **Injeção de dependência:** nada a registrar. Os validadores
> (`Validations/Refunds/{Status,Actions,Banks,Types}`) já são registrados pelo
> `AddValidatorsFromAssemblyContaining<EcommerceValidation>()`.

**6.2 `GetStatus`** — substituir o stub atual (linhas 243–249) por:

```csharp
        /// <summary>
        /// returns the possible status of a refund for each company.
        /// </summary>
        public async Task<bool?> GetStatus()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleStatusRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/refunds/status",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.Status.Root>(response);
                var statusRefunds = root?.data ?? [];

                foreach (var statusRefund in statusRefunds)
                    statusRefund.doc_company = company.doc_company;

                var statusRefundsCache = await _cache.GetOrCreateAsync($"{StatusRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetStatus(company.doc_company);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<int, Entities...Refunds.Status.Status>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status>();

                foreach (var statusRefund in statusRefunds)
                {
                    var validations = _statusValidator.Validate(statusRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.Status.Status(statusRefund, statusRefund.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    statusRefundsCache.TryGetValue(x.id, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.name == y.name &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleStatusRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var item = data?.FirstOrDefault(r => (int?)r["id"] == record.id);

                        if (item != null)
                            _logger.AddRecord(record.id.ToString(), item.ToString());

                        statusRefundsCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
```

**6.3 `GetActions`** (substitui o stub das linhas 219–225) e **6.4 `GetBanks`** (linhas 227–233):
**idênticos ao 6.2**, trocando:

| | `GetStatus` (6.2) | `GetActions` (6.3) | `GetBanks` (6.4) |
|---|---|---|---|
| `EnumJob` | `AfterSaleStatusRefunds` | `AfterSaleActionsRefunds` | `AfterSaleBanksRefunds` |
| Rota | `v3/api/refunds/status` | `v3/api/refunds/actions` | `v3/api/refunds/banks` |
| DTO `Root` | `...Refunds.Status.Root` | `...Refunds.Actions.Root` (após o passo 1.2) | `...Refunds.Banks.Root` |
| Entidade | `...Refunds.Status.Status` | `...Refunds.Actions.Action` | `...Refunds.Banks.Bank` |
| Chave de cache | `StatusRefundsKey` | `ActionsRefundsKey` | `BanksRefundsKey` |
| Repositório | `GetStatus` / `BulkInsertIntoAfterSaleStatusRefunds` | `GetActions` / `BulkInsertIntoAfterSaleActionsRefunds` | `GetBanks` / `BulkInsertIntoAfterSaleBanksRefunds` |
| Validador | `_statusValidator` | `_actionValidator` | `_bankValidator` |
| Variáveis | `statusRefunds` / `statusRefund` | `actionsRefunds` / `actionRefund` | `banksRefunds` / `bankRefund` |

**6.5 `GetTypes`** — substitui o stub das linhas 251–257. Mesmo fluxo, mas com o **`name` como
chave** (não há `id`):

```csharp
        /// <summary>
        /// returns the possible types of a refund for each company. the API does not return an id, so the name is the key.
        /// </summary>
        public async Task<bool?> GetTypes()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleTypesRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/refunds/types",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.Types.Root>(response);
                var typesRefunds = (root?.data ?? [])
                    .Where(x => !string.IsNullOrWhiteSpace(x?.name))
                    .ToList();

                foreach (var typeRefund in typesRefunds)
                    typeRefund.doc_company = company.doc_company;

                var typesRefundsCache = await _cache.GetOrCreateAsync($"{TypesRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetTypes(company.doc_company);

                    return list
                        .Where(x => x != null && !string.IsNullOrEmpty(x.name))
                        .GroupBy(x => x!.name!, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.First()!, StringComparer.OrdinalIgnoreCase); // Dictionary<string name, Entities...Refunds.Types.Type>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.Types.Type>();

                foreach (var typeRefund in typesRefunds)
                {
                    var validations = _typeValidator.Validate(typeRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.Types.Type(typeRefund, typeRefund.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    typesRefundsCache.TryGetValue(x.name!, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleTypesRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var item = data?.FirstOrDefault(r => string.Equals((string?)r["name"], record.name, StringComparison.OrdinalIgnoreCase));

                        if (item != null)
                            _logger.AddRecord(record.name!, item.ToString());

                        typesRefundsCache[record.name!] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
```

> O descarte de `name` vazio e a comparação sem diferenciar maiúsculas/minúsculas seguem o
> `GetTransportations`: `name` passa a fazer parte da PK, e o SQL Server normalmente compara texto
> sem diferenciar maiúsculas.

### Passo 7 — Endpoints

Arquivo: [`AfterSaleController.cs`](../src/Backend/Hosts/Hangfire/Controllers/General/AfterSaleController.cs)

Após a ação `Refunds` (termina na linha 99):

```csharp

        [HttpGet("Refunds/Status")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsStatus()
        {
            var result = await _afterSaleRefundsService.GetStatus();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Actions")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsActions()
        {
            var result = await _afterSaleRefundsService.GetActions();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Banks")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsBanks()
        {
            var result = await _afterSaleRefundsService.GetBanks();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Types")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsTypes()
        {
            var result = await _afterSaleRefundsService.GetTypes();

            return Ok($"Records integrated successfully.");
        }
```

> As rotas ficam sob `Refunds/` para não colidir com o `GET Status` de reversas, que já existe.

---

## 4. Checklist

- [ ] 1. DTOs: `doc_company` com `[JsonIgnore]` nos 4 itens; `Rootobject` → `Root` em `Actions`
- [ ] 2. Entidades: `doc_company` nas 4; `Types` sem `id`; `Bank.name` com 250
- [ ] 3. `Script003`: `doc_company` nas 8 tabelas; PKs `(id, doc_company)` e `(doc_company, name)` em `Types`
- [ ] 4. 4 queries no `IAfterSaleCommandHandler` / `AfterSaleCommandHandler`
- [ ] 5. `IAfterSaleRefundsRepository` / `AfterSaleRefundsRepository`: 8 métodos no lugar dos 4 stubs
- [ ] 6. `AfterSaleRefundsService`: construtor e os 4 métodos
- [ ] 7. `AfterSaleController`: 4 endpoints
- [ ] Compilar
- [ ] Recriar as 4 tabelas e reexecutar o `sp_Create_Merge_Procedure` delas no banco local
- [ ] Chamar os 4 endpoints e conferir as tabelas com `doc_company` preenchido para as duas empresas

## 5. Pontos de atenção

- **Respostas com `success = false` (decisão pendente):** o `GetStatus` de reversas não trata esse
  caso (quando a API falha, `data` vem nulo e nada é gravado, sem registrar o motivo), e este guia
  segue igual. O que gravar em `AfterSaleErrors` fica para depois. Quando for decidido, o caminho
  é o mesmo do `GetRefunds`: adicionar `error`, `error_code`, `message` e `message_translated` nos
  quatro `Root` e o bloco `if (root is null || !root.success)` antes de processar `data`.
- **`id` de `types`:** se a API passar a devolver `id` (como o modelo em `Responsed Jsons`
  sugere), vale voltar para o desenho de `Status` (PK `(id, doc_company)`).
