# Implementação — `AfterSaleRefundsService.GetRefunds` (GET `/v3/api/refunds`)

Guia para aplicação **manual**. Nada foi alterado no código.

Fonte: especificação OpenAPI 1.0.8 (`api.swaggerhub.com/apis/send42/public-api/1.0.8`) e a resposta
real salva em
[`Responsed Jsons/Refund/SearchRefunds/json-returned-by-api-end-point.json`](../src/Backend/Modules/General/AfterSale/Responsed%20Jsons/Refund/SearchRefunds/json-returned-by-api-end-point.json).

O desenho é o mesmo do `AfterSaleReversesService.GetReverses`, que lê a lista resumida de
reversas e grava em `general.AfterSaleSummarizedReverses`. Aqui a lista resumida de reembolsos vai
para `general.AfterSaleSummarizedRefunds`:

| | Reversas (referência) | Reembolsos (este guia) |
|---|---|---|
| Rota da API | `GET v3/api/reverses` | `GET v3/api/refunds` |
| Método do service | `GetReverses` | `GetRefunds` |
| Job (`EnumJob`) | `AfterSaleReverses = 5002` | `AfterSaleRefunds = 5012` |
| DTOs | `Dtos/Reads/Reverses/SearchReverses` | `Dtos/Reads/Refunds/SearchRefunds` |
| Entidade | `Entities/Reads/Reverses/SearchReverses/Reverse` | `Entities/Reads/Refunds/SearchRefunds/Refund` |
| Tabela | `AfterSaleSummarizedReverses` | `AfterSaleSummarizedRefunds` |
| Procedure de merge | `sp_AfterSaleSummarizedReverses_Sync` | `sp_AfterSaleSummarizedRefunds_Sync` (já existe no `Script005`, linha 18) |
| Endpoint nosso | `GET AfterSaleJobs/AfterSale/Reverses` | `GET AfterSaleJobs/AfterSale/Refunds` (novo) |

---

## 1. A rota na documentação 1.0.8

`GET /v3/api/refunds` — "returns all refunds". Autenticação `Bearer` (token da empresa) e
cabeçalho `Accept: application/json`.

**Filtros (query string), todos opcionais:**

| Parâmetro | Formato | Uso neste job |
|---|---|---|
| `start_date` | `yyyy-MM-dd` | **sim**: hoje − 1 mês, igual ao `GetReverses` |
| `end_date` | `yyyy-MM-dd` | **sim**: hoje |
| `search`, `status`, `banks`, `actions`, `type` | texto / lista separada por vírgula | não |

**Resposta 200:** paginada, no mesmo formato da de reversas (`current_page`, `last_page`,
`next_page_url`, `per_page`, `total`, ..., `success`, `data[]`).

### Documentação × resposta real

A documentação descreve menos campos do que a API realmente devolve. A resposta real (arquivo
`json-returned-by-api-end-point.json`) é a que o DTO atual já segue:

| Campo | Na doc 1.0.8 | Na resposta real | No DTO atual | Na tabela | Na entidade |
|---|---|---|---|---|---|
| `id`, `reverse_id`, `order_id`, `type`, `action` | sim | sim | sim | sim | sim |
| `requested_amount`, `requested_total_amount`, `total_amount`, `bonus_amount_percent` | sim | sim | sim | sim | sim |
| `customer` (`id`, `first_name`, `last_name`) | sim | sim | sim | sim | sim |
| `status` (`id`, `name`, `description`) | sim | sim | sim | sim | sim |
| `created_at`, `updated_at` | sim | sim | sim | sim | sim |
| `ecommerce_order_id`, `bonus_amount`, `requested_shipping_amount`, `refund_attachment`, `refunded_shipping_type`, `last_status_history_date`, `reverse.partner_store`, `product_price_type` | não | sim | sim | sim | sim |
| `shipping_method`, `requested_raw_amount` | não | sim | sim | sim | **não** (passo 2) |
| `brands` (lista) | não | sim (vazia no exemplo) | não | não | não |
| `links` (paginação) | não | sim | sim (`Root.links`) | — | — |
| `error`, `error_code`, `message`, `message_translated` (quando `success = false`) | não | — | **não** (passo 1) | — | — |

> `brands` fica de fora: o exemplo vem vazio e não há como saber o formato dos itens. O
> Newtonsoft ignora campos que não existem no DTO, então isso não quebra a leitura.

---

## 2. Passo a passo

Aplique de baixo para cima dentro de cada arquivo.

### Passo 1 — DTO `Root`: campos de erro

Arquivo: [`Domain.AfterSale/Dtos/Reads/Refunds/SearchRefunds/Root.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reads/Refunds/SearchRefunds/Root.cs)

Após a linha 20 (`public bool success { get; init; }`), adicionar — igual ao `Root` de
`SearchReverses`:

```csharp
        public string error { get; init; }
        public string error_code { get; init; }
        public string message { get; init; }
        public string message_translated { get; init; }
```

Sem isso, quando a API devolve `success = false`, não há como gravar o motivo em
`AfterSaleErrors`.

### Passo 2 — Entidade `Refund`: `doc_company` e os dois campos que faltam

Arquivo: [`Domain.AfterSale/Entities/Reads/Refunds/SearchRefunds/Refund.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reads/Refunds/SearchRefunds/Refund.cs)

**Após a linha 81** (última atribuição, `refunded_shipping_type = ...`):

```csharp
            requested_raw_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(refundDto.requested_raw_amount);
            shipping_method = refundDto.shipping_method?.Length > 50 ? refundDto.shipping_method?.Substring(0, 50) : refundDto.shipping_method;
```

**Após a linha 51** (`integrated_or_updated_at = ...`):

```csharp
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
```

**Linha 49** — construtor recebe a empresa como primeiro parâmetro (mesmo padrão de
`SearchReverses.Reverse`):

```csharp
        public Refund(string doc_company, Dtos.Reads.Refunds.SearchRefunds.Refund refundDto)
```

**Após a linha 45** (`public string? refunded_shipping_type ...`):

```csharp
        public string? shipping_method { get; private set; }
```

**Junto dos outros `decimal`** (por exemplo após `requested_shipping_amount`):

```csharp
        public decimal requested_raw_amount { get; private set; }
```

**Antes da linha 13** (`public Int64 id ...`):

```csharp
        public string doc_company { get; private set; }

```

> `shipping_method` e `requested_raw_amount` já existem no DTO e nas duas tabelas, mas a entidade
> não os tinha, então chegavam sempre nulos no banco.

### Passo 3 — Tabelas: `doc_company` e chave composta

Arquivo: [`Infrastructure.AfterSale/Scripts/SQLServer/Script003_Create_Refunds_Tables.sql`](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script003_Create_Refunds_Tables.sql)

Mesma regra das reversas: o mesmo id pode existir nos dois CNPJs, então o CNPJ entra na PK.

**`[general].AfterSaleSummarizedRefunds` — linhas 424–454:**

- Após a linha 425 (`integrated_or_updated_at datetime,`): `[doc_company] nvarchar(14) not null,`
- Linha 426: `id bigint not null primary key,` → `id bigint not null,`
- Linha 453: `last_status_history_date datetime` → `last_status_history_date datetime,`
- Linha 454 (`);`) → substituir por:

  ```sql
  constraint [PK_AfterSaleSummarizedRefunds] primary key (
  	doc_company asc,
  	id asc
  ));
  ```

**`[untreated].AfterSaleSummarizedRefunds` — após a linha 393:** `[doc_company] nvarchar(14),`

> A procedure `sp_AfterSaleSummarizedRefunds_Sync` é gerada no `Script005` (linha 18), que roda
> depois do `Script003`, então num banco novo ela já nasce com a PK composta. Num banco que já
> existe, seria preciso alterar a tabela e reexecutar o `sp_Create_Merge_Procedure` dela.

### Passo 4 — Query

**Interface** [`Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs),
após a linha 11:

```csharp
        string CreateGetSummarizedRefundsByIdsQuery(string doc_company, List<string> refundsIds);
```

**Implementação** [`Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs),
logo após o `CreateGetSummarizedReversesByIdsQuery` (que termina na linha 80):

```csharp
        public string CreateGetSummarizedRefundsByIdsQuery(string doc_company, List<string> refundsIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < refundsIds.Count(); j++)
            {
                if (j == refundsIds.Count() - 1)
                    identificadores += $"'{refundsIds[j]}'";
                else
                    identificadores += $"'{refundsIds[j]}', ";
            }

            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [GENERAL].[AFTERSALESUMMARIZEDREFUNDS] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
        }
```

### Passo 5 — Repositório de reembolsos

**Interface** [`Domain.AfterSale/Abstractions/Repositorys/IAfterSaleRefundsRepository.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Abstractions/Repositorys/IAfterSaleRefundsRepository.cs).
Hoje ela só tem stubs (`InsertIntoAfterSaleRefunds...`) sem parâmetro e sem uso. Após a linha 13,
adicionar:

```csharp
        public Task<List<Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsByIds(string doc_company, List<string> refundsIds);
        public Task<bool> BulkInsertIntoAfterSaleSummarizedRefunds(List<Entities.Reads.Refunds.SearchRefunds.Refund> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoAfterSaleErrors(List<Entities.Errors.Error> data, Guid? parentExecutionGUID);
```

> O `InsertIntoAfterSaleRefunds()` (linha 13) fica sem uso com isso. Pode ser removido daqui e da
> implementação, ou mantido para o `GetRefundById` futuro.

**Implementação** [`Infrastructure.AfterSale/Repositorys/AfterSaleRefundsRepository.cs`](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Repositorys/AfterSaleRefundsRepository.cs).
Os três métodos são cópias dos equivalentes de `AfterSaleReversesRepository`
(`GetSummarizedReversesByIds`, `BulkInsertIntoAfterSaleSummarizedReverses` e
`BulkInsertIntoAfterSaleErrors`), trocando entidade, query e tabela:

```csharp
        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsByIds(string doc_company, List<string> refundsIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>();
            int indice = refundsIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = refundsIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsByIdsQuery(doc_company, refundsIds);
                var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
                list.AddRange(result);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleSummarizedRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund> data, Guid? parentExecutionGUID)
        {
            var summarizedRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund(), tableName: "AfterSaleSummarizedRefunds");

            _coreRepository.FillSystemDataTable(summarizedRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(summarizedRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", summarizedRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoAfterSaleErrors(List<Domain.AfterSale.Entities.Errors.Error> data, Guid? parentExecutionGUID)
        {
            var errosTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Errors.Error(), tableName: "AfterSaleErrors");

            _coreRepository.FillSystemDataTable(errosTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(errosTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", errosTable.TableName, parentExecutionGUID);

            return true;
        }
```

> Uso `BulkInsertIntoTableRaw(tabela, "untreated")` com o schema explícito porque a sobrecarga sem
> schema está marcada como `[Obsolete]` em `IBulkInsertRepository`. O código de reversas ainda usa
> a sobrecarga antiga; o efeito é o mesmo (o padrão configurado é `untreated`).
>
> **Alternativa ao `BulkInsertIntoAfterSaleErrors` duplicado:** movê-lo para o
> `IAfterSaleRepository` (comum ao módulo) e fazer reversas e reembolsos usarem de lá. Fica mais
> limpo, mas mexe também no service de reversas (3 chamadas). Se preferir esse caminho, é uma
> mudança separada.

### Passo 6 — Service

Arquivo: [`Application.AfterSale/Services/AfterSaleRefundsService.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Services/AfterSaleRefundsService.cs)

**6.1 Usings** (topo do arquivo):

```csharp
using Domain.AfterSale.Enums;
using Domain.Shared.Extensions;
using Newtonsoft.Json.Linq;
```

**6.2 Campos, chave de cache e construtor** — substituir as linhas 16–31 (dos campos até o fim do
construtor) por:

```csharp
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleRepository _afterSaleRepository;
        private readonly IAfterSaleRefundsRepository _afterSaleRefundsRepository;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Refund> _refundValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund> _summarizedRefundsValidator;

        private const string GetRefundHashesKey = "AfterSale:GetRefundHashes";
        private const string SummarizedRefundsKey = "AfterSale:SummarizedRefunds";

        public AfterSaleRefundsService(
            [FromKeyedServices("AfterSaleAPI")] IApiClient apiClient,
            ILoggerService logger,
            IAfterSaleRepository afterSaleRepository,
            IAfterSaleRefundsRepository afterSaleRefundsRepository,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Refund> refundValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund> summarizedRefundsValidator,
            IMemoryCache cache
        ) =>
        (_apiClient, _logger, _afterSaleRepository, _afterSaleRefundsRepository, _refundValidator, _summarizedRefundsValidator, _cache) =
        (apiClient, logger, afterSaleRepository, afterSaleRefundsRepository, refundValidator, summarizedRefundsValidator, cache);
```

> **Injeção de dependência:** nada a registrar. `IAfterSaleRefundsRepository` já está registrado
> em `Infrastructure.AfterSale/DependencyInjection.cs` (linha 34), e o validador de
> `SearchRefunds.Refund` (`Validations/Refunds/SearchRefunds/RefundValidation.cs`) já é registrado
> pelo `AddValidatorsFromAssemblyContaining<EcommerceValidation>()` (linha 43).

**6.3 Método** — substituir o `GetRefunds` atual (linhas 33–39, que lança
`NotImplementedException`) por:

```csharp
        /// <summary>
        /// returns all refunds summarized, for each company
        /// </summary>
        public async Task<bool?> GetRefunds()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var company in companys)
            {
                var summarizedRefunds = new List<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund>();
                var listResponses = new List<string>();

                var parameters = new Dictionary<string, string>
                {
                    { "start_date", $"{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}" },
                    { "end_date", $"{DateTime.Now.ToString("yyyy-MM-dd")}" }
                };

                var encodedParameters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

                var response = await _apiClient.GetAsync(
                    route: $"v3/api/refunds?{encodedParameters}",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var page = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Root>(response);

                if (page is null || !page.success)
                {
                    _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                        company.doc_company,
                        EnumJob.AfterSaleRefunds,
                        _logger.GetExecutionGuid(),
                        page?.error,
                        page?.error_code,
                        page?.message,
                        page?.message_translated
                    ));
                    continue;
                }

                int contador = 0;
                string? rote = page.next_page_url;
                summarizedRefunds.AddRange(page.data ?? []);
                listResponses.Add(response);

                while (page.last_page > 1 && !String.IsNullOrEmpty(rote))
                {
                    //API da AfterSale lança Too Many Requests após 30 consultas, por isso caso tenha dado erro na consuta das páginas aguardamos por 1 minuto
                    if (contador > 14)
                    {
                        Thread.Sleep(60 * 1000);
                        contador = 0;
                    }

                    var responseByPage = await _apiClient.GetAsync(
                        route: rote.Replace("https://api.send4.com.br/", ""),
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    var nextPage = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Root>(responseByPage);

                    if (nextPage is null || !nextPage.success)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleRefunds,
                            _logger.GetExecutionGuid(),
                            nextPage?.error,
                            nextPage?.error_code,
                            nextPage?.message,
                            nextPage?.message_translated
                        ));
                        break;
                    }

                    contador++;
                    rote = nextPage.next_page_url;
                    summarizedRefunds.AddRange(nextPage.data ?? []);
                    listResponses.Add(responseByPage);
                    Thread.Sleep(2 * 1000);
                }

                if (summarizedRefunds.Count == 0)
                    continue;

                var summarizedRefundsCache = await _cache.GetOrCreateAsync($"{SummarizedRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetSummarizedRefundsByIds(
                        company.doc_company,
                        summarizedRefunds.Select(x => x.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...SearchRefunds.Refund>
                });

                summarizedRefunds.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.id) &&
                   summarizedRefundsCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
                       ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>();

                foreach (var summarizedRefund in summarizedRefunds)
                {
                    var validations = _summarizedRefundsValidator.Validate(summarizedRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund(company.doc_company, summarizedRefund));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleSummarizedRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    // índice id -> json do reembolso, montado só com as páginas desta empresa
                    var refundsJson = new Dictionary<long, string>();

                    foreach (var pageResponse in listResponses)
                    {
                        foreach (var item in (JObject.Parse(pageResponse)["data"] as JArray) ?? [])
                        {
                            var id = (long?)item["id"];

                            if (id != null)
                                refundsJson[id.Value] = item.ToString();
                        }
                    }

                    foreach (var record in _listSomenteNovos)
                    {
                        if (refundsJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        summarizedRefundsCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
```

Diferenças em relação ao `GetReverses` (de resto, linha a linha igual):

| Trecho | Reversas | Reembolsos |
|---|---|---|
| Job | `EnumJob.AfterSaleReverses` | `EnumJob.AfterSaleRefunds` |
| Rota | `v3/api/reverses?...` | `v3/api/refunds?...` |
| DTOs | `SearchReverses.Root` / `.Reverse` | `SearchRefunds.Root` / `.Refund` |
| Chave de cache | `AfterSale:SummarizedReverses:<cnpj>` | `AfterSale:SummarizedRefunds:<cnpj>` |
| Repositório | `_afterSaleReversesRepository` | `_afterSaleRefundsRepository` |
| Validador | `_summarizedReversesValidator` | `_summarizedRefundsValidator` |

### Passo 7 — Endpoint

Arquivo: [`Hosts/Hangfire/Controllers/General/AfterSaleController.cs`](../src/Backend/Hosts/Hangfire/Controllers/General/AfterSaleController.cs)

Após a ação `CourierAttributes` (termina na linha 90), adicionar:

```csharp

        [HttpGet("Refunds")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> Refunds()
        {
            var result = await _afterSaleRefundsService.GetRefunds();

            return Ok($"Records integrated successfully.");
        }
```

O `_afterSaleRefundsService` já é injetado no controller (linhas 19, 24 e 29).

### Passo 8 — Auditoria e jobs

Nada a inserir: o job da integração e o da procedure de sincronização já estão no
[`Script007`](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script007_Insert_AfterSale_Values_Into_Jobs_Table.sql):

- linha 58: `(50, 5012, 'AfterSaleRefunds', 'Integração de dados da tabela AfterSaleSummarizedRefunds', 3, 2)`
- linha 31: `(5012, 5525, 'sp_AfterSaleSummarizedRefunds_Sync', ..., 3, 1)`

`EnumJob.AfterSaleRefunds = 5012` também já existe.

---

## 3. Checklist

- [ ] 1. `SearchRefunds/Root.cs`: campos de erro
- [ ] 2. `SearchRefunds/Refund.cs` (entidade): `doc_company`, `shipping_method`, `requested_raw_amount`
- [ ] 3. `Script003`: `doc_company` nas duas tabelas e PK `(doc_company, id)`
- [ ] 4. `IAfterSaleCommandHandler` / `AfterSaleCommandHandler`: `CreateGetSummarizedRefundsByIdsQuery`
- [ ] 5. `IAfterSaleRefundsRepository` / `AfterSaleRefundsRepository`: 3 métodos
- [ ] 6. `AfterSaleRefundsService`: usings, construtor e `GetRefunds`
- [ ] 7. `AfterSaleController`: `GET Refunds`
- [ ] Compilar
- [ ] Recriar as tabelas `AfterSaleSummarizedRefunds` e a procedure `sp_AfterSaleSummarizedRefunds_Sync` no banco local
- [ ] Chamar `GET AfterSaleJobs/AfterSale/Refunds` e conferir `general.AfterSaleSummarizedRefunds` com `doc_company` preenchido para as duas empresas

## 4. Pontos de atenção

- **`total_amount` na doc é `string`** e na resposta real é número. O DTO lê tudo como `string` e
  a entidade converte com `ConvertToDecimalValidation`, então os dois formatos funcionam.
- **Paginação:** a doc não descreve parâmetros `page`/`per_page`; o código segue o `next_page_url`
  devolvido pela API, igual às reversas.
- **`Thread.Sleep`** em método `async`, herdado do `GetReverses`: bloqueia a thread durante a
  espera. `await Task.Delay(...)` é o equivalente não bloqueante; se trocar aqui, vale trocar lá
  também.
