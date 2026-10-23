# Implementação — `AfterSaleRefundsService.GetRefund` (detalhe dos reembolsos em lote)

Guia para aplicação **manual**. Nada foi alterado no código.

`GetRefund` faz para reembolsos o que o `AfterSaleReversesService.GetReverse` faz para reversas:
pega na tabela resumida os itens que **ainda não têm detalhe** gravado, chama a rota de detalhe de
cada um e grava o resultado nas tabelas de detalhe. Ele **não substitui** o `GetRefundById`:

| Método | Disparo | O que lê | Job |
|---|---|---|---|
| `GetRefund` (este guia) | Recorrente, sem parâmetros (`GET AfterSaleJobs/AfterSale/Refund`) | Até 30 reembolsos de `AfterSaleSummarizedRefunds` sem detalhe em `AfterSaleRefunds` | `AfterSaleRefund = 5011` |
| `GetRefundById` (já existe) | Sob demanda, com usuário (`GET AfterSaleJobs/AfterSale/Refunds/{docCompany}/{id}`) | Um reembolso específico | `AfterSaleGetRefundById = 5023` |

Correspondência com reversas:

| | Reversas (referência) | Reembolsos (este guia) |
|---|---|---|
| Lista de pendentes | `GetSummarizedReversesIdsForGetReverse` | `GetSummarizedRefundsIdsForGetRefund` (novo) |
| Query dos pendentes | `CreateGetSummarizedReversesIdsForGetReverseQuery` | `CreateGetSummarizedRefundsIdsForGetRefundQuery` (nova) |
| Cache (o que já está gravado) | `GetReversesByIds` / `CreateGetReversesByIdsQuery` | `GetRefundsByIds` / `CreateGetRefundsByIdsQuery` (novos) |
| Chave de cache | `AfterSale:Reverses:<cnpj>` | `AfterSale:Refunds:<cnpj>` (nova) |
| Rota da API | `GET v3/api/reverses/{id}` | `GET v3/api/refunds/{id}` |
| DTO | `Dtos/Reads/Reverses/GetReverse/Root` | `Dtos/Reads/Refunds/GetRefund/Root` |
| Entidade | `Entities/Reads/Reverses/GetReverse/Reverse` | `Entities/Reads/Refunds/GetRefund/Refund` |
| Gravação | `BulkInsertIntoAfterSaleReverses` | `BulkInsertIntoAfterSaleRefunds` (já existe) |
| Erros | `_afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors` | `_afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors` (já existe) |
| Endpoint | `GET Reverse` | `GET Refund` (novo) |

**Já existem e não mudam:** o DTO, as 8 entidades de detalhe com `doc_company`, as tabelas do
`Script003`, o `BulkInsertIntoAfterSaleRefunds`, o validador `_refundValidator`, o `EnumJob`
`AfterSaleRefund = 5011`, o job 5011 no `Script007` (linha 57) e as procedures de merge das 8
tabelas (jobs 5517–5524 e `Script005`). **Nada a inserir em banco.**

---

## Passo 1 — Queries

### 1.1 Interface

[`Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs),
após a linha 12 (`CreateGetSummarizedRefundsByIdsQuery`):

```csharp
        string CreateGetSummarizedRefundsIdsForGetRefundQuery();
        string CreateGetRefundsByIdsQuery(string doc_company, List<string> refundsIds);
```

### 1.2 Implementação

[`Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs),
logo depois do `CreateGetSummarizedRefundsByIdsQuery` (começa na linha 82):

```csharp
        public string CreateGetSummarizedRefundsIdsForGetRefundQuery()
        {
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM
                        [AFTERSALE].[AFTERSALESUMMARIZEDREFUNDS] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALEREFUNDS] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.ID
                      WHERE
                        B.ID IS NULL";
        }

        public string CreateGetRefundsByIdsQuery(string doc_company, List<string> refundsIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < refundsIds.Count(); j++)
            {
                if (j == refundsIds.Count() - 1)
                    identificadores += $"'{refundsIds[j]}'";
                else
                    identificadores += $"'{refundsIds[j]}', ";
            }

            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALEREFUNDS] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
        }
```

> `TOP 30` é o mesmo limite do `GetReverse`: a API da AfterSale devolve *Too Many Requests* depois
> de cerca de 30 consultas, e cada reembolso pendente é uma chamada.

---

## Passo 2 — Repositório de reembolsos

### 2.1 Interface

[`Domain.AfterSale/Abstractions/Repositorys/IAfterSaleRefundsRepository.cs`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Abstractions/Repositorys/IAfterSaleRefundsRepository.cs),
antes da linha 21 (`BulkInsertIntoAfterSaleRefunds`):

```csharp
        public Task<IEnumerable<Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsIdsForGetRefund();
        public Task<List<Entities.Reads.Refunds.GetRefund.Refund?>> GetRefundsByIds(string doc_company, List<string> refundsIds);
```

### 2.2 Implementação

[`Infrastructure.AfterSale/Repositorys/AfterSaleRefundsRepository.cs`](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Repositorys/AfterSaleRefundsRepository.cs),
antes do `BulkInsertIntoAfterSaleRefunds` (linha 18). Mesmo molde de
`GetSummarizedReversesIdsForGetReverse` e `GetReversesByIds` do repositório de reversas:

```csharp
        public async Task<IEnumerable<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsIdsForGetRefund()
        {
            string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsIdsForGetRefundQuery();

            return await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund?>> GetRefundsByIds(string doc_company, List<string> refundsIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund?>();
            int indice = refundsIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = refundsIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetRefundsByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetRefundsByIdsQuery(doc_company, refundsIds);
                var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>(sql);
                list.AddRange(result);

                return list;
            }
        }

```

> A lista de pendentes usa a entidade resumida (`SearchRefunds.Refund`), que já tem `doc_company`
> e `id`, como o `GetReverse` usa `SearchReverses.Reverse`.

---

## Passo 3 — Interface do service

[`Application.AfterSale/Abstractions/Services/IAfterSaleRefundsService.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Abstractions/Services/IAfterSaleRefundsService.cs),
após a linha 13 (`GetRefunds`):

```csharp
        public Task<bool?> GetRefund();
```

O `GetRefundById` (linha 14) continua como está.

---

## Passo 4 — Service

[`Application.AfterSale/Services/AfterSaleRefundsService.cs`](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Services/AfterSaleRefundsService.cs)

### 4.1 Chave de cache

Após a linha 33 (`SummarizedRefundsKey`):

```csharp
        private const string RefundsKey = "AfterSale:Refunds";
```

### 4.2 Método

Antes do `GetRefundById` (o bloco `/// <summary>` da linha 423):

```csharp
        /// <summary>
        /// get the details of the summarized refunds not yet integrated, for each company
        /// </summary>
        public async Task<bool?> GetRefund()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleRefund);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var summarizedRefunds = await _afterSaleRefundsRepository.GetSummarizedRefundsIdsForGetRefund();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var refundsByCompany in summarizedRefunds.Where(x => x != null).GroupBy(x => x!.doc_company))
            {
                var company = companys.FirstOrDefault(x => x?.doc_company == refundsByCompany.Key);

                if (company is null)
                {
                    _logger.AddMessage($"CNPJ {refundsByCompany.Key} não encontrado em AfterSaleParameters: {refundsByCompany.Count()} reembolso(s) ignorado(s).\n");
                    continue;
                }

                var refundsDtosList = new List<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Root>();
                var refundsJson = new Dictionary<long, string>();

                foreach (var summarizedRefund in refundsByCompany)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/refunds/{summarizedRefund!.id}",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Root>(response);

                    if (root is null || !root.success || root.data is null)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleRefund,
                            _logger.GetExecutionGuid(),
                            summarizedRefund.id,
                            root?.error,
                            root?.error_code,
                            root?.message,
                            root?.message_translated
                        ));
                        continue;
                    }

                    refundsDtosList.Add(root);
                    refundsJson[summarizedRefund.id] = JObject.Parse(response)["data"]?.ToString() ?? response;
                }

                if (refundsDtosList.Count == 0)
                    continue;

                var refundsCache = await _cache.GetOrCreateAsync($"{RefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetRefundsByIds(
                        company.doc_company,
                        refundsDtosList.Select(x => x.data.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...GetRefund.Refund>
                });

                refundsDtosList.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.data.id) &&
                   refundsCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.data.id), out var y) &&
                       ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.data.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>();

                foreach (var refundsDto in refundsDtosList)
                {
                    var validations = _refundValidator.Validate(refundsDto.data);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund(company.doc_company, refundsDto.data));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (refundsJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        refundsCache[record.id] = record;
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

Diferenças em relação ao `GetReverse` (de resto, linha a linha igual):

| Trecho | `GetReverse` | `GetRefund` |
|---|---|---|
| Job | `EnumJob.AfterSaleReverse` | `EnumJob.AfterSaleRefund` |
| Pendentes | `_afterSaleReversesRepository.GetSummarizedReversesIdsForGetReverse()` | `_afterSaleRefundsRepository.GetSummarizedRefundsIdsForGetRefund()` |
| Rota | `v3/api/reverses/{id}` | `v3/api/refunds/{id}` |
| Objeto principal no DTO | `root.data.reverse` (o JSON traz `data.reverse`, `data.customer`, `data.tracking_history`) | `root.data` (o JSON traz o reembolso direto em `data`) |
| JSON gravado em `auditing.Records` | `["data"]["reverse"]` | `["data"]` |
| Cache | `ReversesKey` + `GetReversesByIds` | `RefundsKey` + `GetRefundsByIds` |
| Construtor da entidade | `Reverse(doc_company, reverse, customer, tracking_history)` | `Refund(doc_company, data)` |
| Gravação e erros | `_afterSaleReversesRepository` | `_afterSaleRefundsRepository` |
| Busca da empresa | `companys.FirstOrDefault(x => x.doc_company == ...)` | `companys.FirstOrDefault(x => x?.doc_company == ...)` (o `GetCompanys` devolve `Company?`) |

> **Dependências:** nada novo no construtor. O service de reembolsos já recebe `_apiClient`,
> `_logger`, `_cache`, `_afterSaleRepository`, `_afterSaleRefundsRepository` e `_refundValidator`,
> e o arquivo já tem os `using` de `Domain.AfterSale.Enums`, `Domain.Shared.Extensions` e
> `Newtonsoft.Json.Linq`.

### 4.3 (Opcional) `GetRefundById` atualizando o cache do `GetRefund`

O `GetReverseById` atualiza o cache do `GetReverse` quando ele está carregado, para a próxima
execução não regravar a reversa. Para manter o mesmo comportamento, no `GetRefundById`, logo depois
de `BulkInsertIntoAfterSaleRefunds([refund], ...)`:

```csharp
                    // atualiza o cache do GetRefund, se ele estiver carregado, para a próxima execução não regravar o reembolso
                    if (_cache.TryGetValue($"{RefundsKey}:{doc_company}", out Dictionary<long, Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>? refundsCache) && refundsCache is not null)
                        refundsCache[id] = refund;
```

---

## Passo 5 — Endpoint

[`Hosts/Hangfire/Controllers/General/AfterSaleController.cs`](../src/Backend/Hosts/Hangfire/Controllers/General/AfterSaleController.cs),
logo depois da ação `Refunds` (`[HttpGet("Refunds")]`, linha 113), no mesmo formato do
`GET Reverse`:

```csharp

        [HttpGet("Refund")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsDetails()
        {
            var result = await _afterSaleRefundsService.GetRefund();

            return Ok($"Records integrated successfully.");
        }
```

> O nome do método C# não pode ser `Refund`, porque essa ação já existe (é o `GET` por id). As
> rotas não colidem: `Refund` (sem parâmetros) e `Refunds/{docCompany}/{id}`.

---

## Passo 6 — Auditoria

Nada a inserir. O job 5011 já está no `Script007`, no bloco de integrações:

```sql
(50, 5011, 'AfterSaleRefund', 'Integração de dados das tabelas AfterSaleRefunds, AfterSaleRefundsCouldChangeStatusFor, ...', 3, 2),
```

> A descrição termina com `, ` (vírgula e espaço) sobrando depois de
> `AfterSaleRefundsProductsImagesUrls`. Se quiser, troque por
> `... AfterSaleRefundsProductsImages e AfterSaleRefundsProductsImagesUrls'`, como nas descrições
> das reversas.

---

## Checklist

- [ ] 1. Queries `CreateGetSummarizedRefundsIdsForGetRefundQuery` e `CreateGetRefundsByIdsQuery` (interface e implementação)
- [ ] 2. Repositório: `GetSummarizedRefundsIdsForGetRefund` e `GetRefundsByIds` (interface e implementação)
- [ ] 3. `IAfterSaleRefundsService`: `GetRefund()`
- [ ] 4. `AfterSaleRefundsService`: `RefundsKey` e `GetRefund`; opcional: cache no `GetRefundById`
- [ ] 5. `AfterSaleController`: `GET Refund`
- [ ] Compilar
- [ ] Rodar `GET Refunds` (resumidos) e depois `GET Refund` e conferir `AfterSaleRefunds` e filhas com `doc_company` preenchido

## Pontos de atenção

- **Ordem dos jobs:** o `GetRefund` só encontra pendentes depois que o `GetRefunds` gravou os
  resumidos, como o `GetReverse` depende do `GetReverses`.
- **Reembolsos já detalhados não são atualizados por aqui:** a query só traz quem ainda não está em
  `AfterSaleRefunds` (mesmo comportamento do `GetReverse`). Mudanças posteriores num reembolso já
  gravado chegam pelo `GetRefundById`, ou exigiriam outro critério na query (por exemplo,
  `A.UPDATED_AT > B.UPDATED_AT`).
- **Pausa entre chamadas:** o `GetReverse` não pausa entre as até 30 chamadas de detalhe; o
  `GetRefund` segue igual. Se a API começar a responder *Too Many Requests*, a saída é a mesma
  pausa usada nas leituras paginadas.
