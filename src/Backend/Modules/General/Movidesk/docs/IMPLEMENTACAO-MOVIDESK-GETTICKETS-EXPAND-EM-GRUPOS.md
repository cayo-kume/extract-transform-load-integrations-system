# Movidesk — `GetTickets` com o `$expand` dividido em grupos

Proposta para acabar com a lentidão da listagem de tickets (`GET /tickets` com todas as listas expandidas).
Complementa [IMPLEMENTACAO-MOVIDESK-GETTICKETS.md](IMPLEMENTACAO-MOVIDESK-GETTICKETS.md).

> **Implementado em 21/10/2026** no `MovideskTicketsService` (seção 4, tudo dentro do `GetTickets`), com as decisões da seção 7: 5 grupos,
> blocos de **100** tickets e ticket incompleto só como mensagem. As medições da seção 2 foram feitas na API real em
> 21/10/2026, com o token de uma empresa.
>
> **Resultado real (21/10/2026, banco vazio, 1 empresa):**
> * primeira carga de 89 dias: 1.966 tickets em 20 blocos, em 11 min (~33 s por bloco), sem nenhum ticket incompleto;
> * 3 tickets regravados pelo `GetTicketById` ficaram com as mesmas contagens em todas as listas;
> * segunda execução logo depois: 0,5 s, sem nada alterado;
> * versão com tudo dentro do `GetTickets` (seção 4): 157 tickets forçados a voltar para a fase 2 foram regravados em
>   68 s, com `lastUpdate` e contagens de ações, históricos, campos adicionais e clientes idênticos aos de antes.
>
> **Pendente:** o `[DisableConcurrentExecution]` (seção 3.7) entra quando o job recorrente for registrado. Hoje não há
> job da Movidesk no Hangfire, então não há onde aplicar a trava.

Arquivo afetado: `src/Backend/Modules/General/Movidesk/Application.Movidesk/Services/MovideskTicketsService.cs`
(só o `GetTickets`; o `GetTicketById` busca por id, sem `$expand`, e responde em cerca de 1,6 s).

---

## 1. O problema

O `GetTickets` pede, numa única chamada por página, os campos do ticket (`$select`) e **todas** as listas (`$expand`):
pessoas, clientes, ações (com apontamentos, despesas e anexos), históricos, campos adicionais, itens de configuração e
tickets pai/filho.

* Cada página leva **90 a 110 segundos** para responder, com 25 ou com 100 tickets. Reduzir o `$top` não ajudou.
* Algumas chamadas passam de 120 s e voltam **504** (o gateway da Movidesk corta a chamada). Uma resposta inválida
  interrompe a empresa naquela execução, e 3 erros seguidos podem bloquear a API.
* Na primeira carga (89 dias, mais de 1.000 tickets na conta testada), seriam mais de 40 páginas de ~100 s.

---

## 2. Medições (21/10/2026)

### 2.1 O que custa o tempo

| Chamada (`$top=25`) | Tempo |
|---|---|
| Só `$select`, sem `$expand` | 1,5 s |
| `$expand` completo, filtro por `lastUpdate` | 100 s a 109 s |
| `$expand` completo, filtro só do último dia | 106 s |
| `$expand` completo, filtro dos últimos 7 dias | **504** (120 s) |
| `$expand` completo, sem filtro | **504** (120 s) |
| `$expand` completo, faixa de ids (`id ge X and id le Y`) | 90 s a 104 s |

O tamanho da página e o tamanho da janela não mudam o resultado: o custo vem de pedir **todas as listas juntas**.
Cada lista sozinha é rápida, e a soma das partes fica muito abaixo do todo.

### 2.2 Cada item do `$expand` sozinho (filtro por `lastUpdate`)

| `$expand` | Tempo |
|---|---|
| `owner,createdBy,slaSolutionChangedBy` | 1,9 s |
| `parentTickets,childrenTickets` | 2,7 s |
| `statusHistories($expand=changedBy)` | 4,3 s |
| `clients($expand=organization)` | 5,0 s |
| `ownerHistories($expand=owner,changedBy)` | 5,0 s |
| `customFieldValues($expand=items)` | 5,0 s |
| `assets` | 7,1 s |
| `actions(...)` (com apontamentos, despesas e anexos) | 13,2 s |
| Todos menos `actions` | 65,8 s |

### 2.3 Grupos propostos, com filtro por faixa de ids

| Grupo | `$select` | `$expand` | Tempo |
|---|---|---|---|
| 1 | campos do ticket (`TicketsSelect`) | `owner,createdBy,slaSolutionChangedBy,parentTickets,childrenTickets` | 4,3 s |
| 2 | `id` | `actions($expand=createdBy,timeAppointments($expand=createdBy,createdByTeam),expenses($expand=createdBy),attachments($expand=createdBy))` | 5,8 s |
| 3 | `id` | `clients($expand=organization),assets` | 4,4 s |
| 4 | `id` | `ownerHistories($expand=owner,changedBy),statusHistories($expand=changedBy)` | 1,5 s |
| 5 | `id` | `customFieldValues($expand=items)` | 1,3 s |
| | | **Total** | **≈ 17 s** (contra ~100 s numa chamada só) |

### 2.4 Blocos de 100 tickets (decisão 2)

Mesmos 5 grupos, faixa de 100 ids da fase 1 e `$top=150`. Para comparar, um bloco de 25 medido na mesma hora:

| Grupo | Bloco de 100 (ids 53951–67159) | Bloco de 100 (ids 67160–67279) | Bloco de 25 (ids 67280–67309) |
|---|---|---|---|
| 1 campos + pessoas + pai/filho | 6,3 s | 6,1 s | 7,6 s |
| 2 ações | 5,2 s | 7,1 s | 5,0 s |
| 3 clientes + itens | 6,5 s | 4,8 s | 4,6 s |
| 4 históricos | 3,3 s | 3,1 s | 2,0 s |
| 5 campos adicionais | 2,2 s | 3,3 s | 2,1 s |
| **Total** | **23,5 s** | **24,4 s** | **21,3 s** |

Todas as respostas vieram completas (100 e 25 tickets por grupo). Um bloco de 100 custa praticamente o mesmo que um de
25, então a primeira carga fica cerca de 4 vezes mais rápida. Nenhuma chamada passou de 8 s, longe do corte de 120 s.

### 2.5 Outros comportamentos verificados

| Teste | Resultado |
|---|---|
| Lista só de ids (`$select=id`) com `$top=1000` | 0,9 s |
| `$top=2000` | A API devolve no máximo **1.000** registros |
| Filtro com a lista de ids (`id eq 1 or id eq 2 ...`, 25 ids) | **400**: a API limita o filtro a 100 nós OData (`MaxNodeCount`). Por isso a proposta usa faixa (`id ge X and id le Y`) |
| `expenses($expand=createdByTeam)` | **500** (já corrigido: nas despesas só `createdBy` é expandido) |

---

## 3. Proposta

O `GetTickets` passa a trabalhar em duas fases por empresa:

```
fase 1  GET tickets  $select=id,lastUpdate  $filter=lastUpdate ge {since}  $top=1000   (~1 s por 1.000 tickets)
        └─ compara com o cache e separa só os tickets novos ou alterados

fase 2  para cada bloco de até 100 tickets (faixa de ids):
          grupo 1  campos + pessoas + pai/filho  ┐
          grupo 2  ações                         │  mesma faixa: id ge {primeiro} and id le {último}
          grupo 3  clientes + itens              │               and lastUpdate ge {since}
          grupo 4  históricos                    │
          grupo 5  campos adicionais             ┘
        └─ junta as 5 respostas por id, valida e grava o bloco
```

### 3.1 Fase 1: lista dos ids alterados

* Uma chamada leve por 1.000 tickets: `$select=id,lastUpdate`, o mesmo filtro de hoje (`lastUpdate ge {since}`),
  `$orderby=id asc`, `$top=1000` e `$skip` de 1.000 em 1.000.
* O cache de estado (`Movidesk:Tickets:<cnpj>`) é montado com esses ids, e os tickets cujo `lastUpdate` não mudou são
  descartados **antes** das chamadas caras. Hoje o descarte só acontece depois de baixar tudo.
* Ganho na execução a cada 3 minutos: a folga de 1 hora traz cerca de 18 tickets por execução na conta testada, mas
  quase todos já estão gravados. A fase 2 só busca os que mudaram de verdade, em geral 0 a 2 tickets.

### 3.2 Fase 2: blocos por faixa de ids

* Os blocos são montados a partir da lista **completa** da fase 1, ordenada por id:
  * cada bloco começa num ticket alterado;
  * tem no máximo 100 ids da fase 1;
  * termina no último ticket alterado dentro desses 100.
  
  Assim a faixa `id ge {primeiro} and id le {último}` nunca contém mais de 100 tickets do filtro, mesmo que a faixa
  inclua tickets que não mudaram (eles vêm na resposta e são ignorados).
* O `$top` de cada grupo é **150**: os 100 do bloco e uma folga para tickets que entraram no filtro entre a fase 1 e a
  fase 2. Se algum grupo devolver 150 registros, o bloco é tratado como incompleto (ver 3.4).
* Os 5 grupos usam o mesmo filtro, a mesma ordenação e o mesmo `$top`.

### 3.3 Junção das respostas

* O grupo 1 traz o objeto base de cada ticket (os campos e `lastUpdate`). Os grupos 2 a 5 trazem só `id` e as suas
  listas.
* As respostas são juntadas por id com `JObject.Merge` (Newtonsoft, `MergeArrayHandling.Replace`). O JSON junto é:
  * desserializado no DTO `Ticket` atual, sem nenhuma mudança no DTO, nas entidades ou no repositório;
  * gravado em `auditing.Records`, como hoje.
* Só entram os ids que a fase 1 marcou como alterados. Ids extras (tickets que entraram no filtro durante a leitura)
  são ignorados e chegam na próxima execução, porque o `lastUpdate` deles é mais recente que o gravado.

### 3.4 Ticket incompleto não é gravado

Um ticket só é gravado se apareceu nas **5** respostas. O motivo é o "um horário por lote" das tabelas filhas: ao
regravar o ticket, o merge apaga as filhas que não vieram no lote. Gravar um ticket sem as ações, por exemplo, apagaria
as ações do banco.

| Situação | Comportamento |
|---|---|
| Um grupo devolve erro (500, 504, JSON inválido) | Interrompe a empresa nesta execução, como hoje, sem gravar o bloco atual. Os blocos anteriores já foram gravados (3.5) |
| Um ticket alterado falta em algum grupo | O ticket não é gravado e vira só uma mensagem em `auditing.Messages` (decisão 4: sem tabela de erros). Ele volta na próxima execução, porque o `lastUpdate` gravado continua antigo |
| Um grupo devolve 150 registros (`$top` cheio) | O bloco não é gravado e vira mensagem; na próxima execução os blocos são remontados |

**Consistência entre os grupos:** as 5 chamadas acontecem em segundos diferentes. Se um ticket mudar no meio delas,
algumas listas podem vir da versão anterior. Como o `lastUpdate` gravado é o do grupo 1 (o mais antigo), a próxima
execução encontra um `lastUpdate` mais novo na API e regrava o ticket. A diferença se corrige sozinha em até 3 minutos.

### 3.5 Gravação por bloco

Hoje cada empresa acumula todos os tickets e grava no fim. Com a proposta, o `SaveTickets` é chamado **a cada bloco**:

* o progresso não se perde se uma chamada falhar no meio da primeira carga;
* a memória fica limitada a 100 tickets por vez;
* o cache de estado é atualizado bloco a bloco (o `SaveTickets` já faz isso).

O horário de lote (`GetBatchTime()`) passa a ser um por bloco. A condição de exclusão das filhas continua valendo,
porque compara o pai com as suas próprias filhas.

### 3.6 Limite de requisições

Cada bloco faz 5 chamadas. Com o limite de 10 por minuto, o intervalo de 6 s precisa valer entre **todas** as chamadas,
não só entre páginas.

* **Proposta:** trocar o `Thread.Sleep(6000)` depois de cada página por uma espera que conta a partir do **início** da
  chamada anterior (`await Task.Delay` só do que falta para completar 6 s).
  * Como os grupos respondem em 2 a 8 s, cada chamada custa ~6 a 8 s no total, e não 6 s mais o tempo da chamada.
  * `Task.Delay` também não bloqueia a thread, ao contrário do `Thread.Sleep` atual.

| Cenário | Hoje | Proposta |
|---|---|---|
| Página/bloco | 25 tickets em ~100 s (com risco de 504) | 100 tickets em ~32 s (5 chamadas de ~6 a 7 s) |
| Execução a cada 3 min (0 a 2 tickets alterados) | ~100 s (baixa os ~18 da folga de 1 hora) | ~1 s (fase 1) + ~32 s se houver alteração |
| Primeira carga (1.000 tickets) | 40 × ~100 s ≈ 70 min, com 504s | 10 × ~32 s ≈ 6 min |

### 3.7 Execuções sobrepostas (decisão 3)

Mesmo com ~6 minutos, a primeira carga passa do intervalo de 3 minutos do job. Uma segunda execução não pode começar
enquanto a primeira roda, porque as duas buscariam os mesmos tickets e dividiriam o limite de 10 requisições por minuto.

* Hoje nenhum job recorrente da Movidesk está registrado: `GeneralIntegrationsRecurringJobsExtensions.AddRecurringJobs`
  está todo comentado, e o `GetTickets` só roda quando a rota `MovideskJobs/Movidesk/Tickets` é chamada.
* Quando o job for registrado (`RecurringJob.AddOrUpdate<IMovideskTicketsService>(..., s => s.GetTickets(), ...)`), o
  método recebe `[DisableConcurrentExecution]`.
  * Como o job é registrado pela interface, o Hangfire lê os filtros do método **da interface**. É lá que o atributo
    fica, não na classe `MovideskTicketsService`.
  * `timeoutInSeconds` é quanto a nova execução espera pela trava antes de desistir. Com `AutomaticRetry` em 0
    tentativas (configuração atual do host), ela falha e a próxima tentativa fica para o próximo intervalo.

```csharp
public interface IMovideskTicketsService
{
    // a primeira carga passa do intervalo de 3 min do job: uma execução de cada vez
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    public Task<bool?> GetTickets();
    ...
}
```

  * O atributo é do pacote `Hangfire.Core`. O `Application.Movidesk` ainda não referencia esse pacote, então é preciso
    adicioná-lo, ou registrar o job por um método da camada do host que tenha o atributo e chame o service.
* A trava vale só para as execuções do Hangfire. Uma chamada manual à rota `Tickets` enquanto o job roda continua
  possível.

---

## 4. Código implementado

Tudo fica dentro do `GetTickets()`: os passos que antes eram métodos privados (lista de ids, cache, montagem dos blocos,
busca de um bloco, leitura do `JArray` e espera entre chamadas) viraram blocos do próprio método, cada um com o
comentário que explicava o método. O único método privado do service é o `SaveTickets`, porque o `GetTicketById` também o
usa, e ele não mudou.

### 4.1 Constantes

```csharp
// limite da API: 10 requisições por minuto -> 1 requisição a cada 6 s (contados do início da chamada anterior)
private const int RequestIntervalInMilliseconds = 6 * 1000;
// 100 tickets por bloco (faixa de ids): com o $expand em grupos, cada chamada responde em 2 a 8 s (medido em 21/10/2026);
// $top com folga para tickets que entram no filtro durante a leitura
private const int PageSize = 100;
private const int BlockTop = 150;
// a fase 1 lista só id e lastUpdate; a API devolve no máximo 1.000 por chamada
private const int IdsPageSize = 1000;

// $select é obrigatório na listagem: campos do ticket, sem as listas
private const string TicketsSelect =
    "id,protocol,type,subject,category,urgency,status,baseStatus,justification,origin,createdDate," +
    "isDeleted,originEmailAccount,ownerTeam,serviceFirstLevelId,serviceFirstLevel,serviceSecondLevel," +
    "serviceThirdLevel,contactForm,tags,cc,resolvedIn,closedIn,canceledIn,actionCount,lifeTimeWorkingTime," +
    "stoppedTime,stoppedTimeWorkingTime,resolvedInFirstCall,chatWidget,chatGroup,chatTalkTime,chatWaitingTime," +
    "sequence,slaAgreement,slaAgreementRule,slaSolutionTime,slaResponseTime,slaSolutionChangedByUser," +
    "slaSolutionDate,slaSolutionDateIsPaused,jiraIssueKey,redmineIssueId,reopenedIn,lastActionDate,lastUpdate," +
    "slaResponseDate,slaRealResponseDate";

// as listas vêm pelo $expand, dividido em grupos: tudo numa chamada leva ~100 s e chega a dar 504;
// em grupos, ~24 s por bloco de 100 (medido em 21/10/2026). O grupo 1 traz os campos; os outros, só id e as listas.
// expenses($expand=createdByTeam) faz a API devolver 500: nas despesas só createdBy é expandido
private static readonly (string select, string expand)[] TicketsExpandGroups =
[
    (TicketsSelect, "owner,createdBy,slaSolutionChangedBy,parentTickets,childrenTickets"),
    ("id", "actions($expand=createdBy,timeAppointments($expand=createdBy,createdByTeam),expenses($expand=createdBy),attachments($expand=createdBy))"),
    ("id", "clients($expand=organization),assets"),
    ("id", "ownerHistories($expand=owner,changedBy),statusHistories($expand=changedBy)"),
    ("id", "customFieldValues($expand=items)"),
];
```

A constante `TicketsExpand` deixou de existir.

### 4.2 `GetTickets`

```csharp
/// <summary>
/// sincroniza os tickets atualizados desde a última execução, para cada empresa.
/// Fase 1: lista id e lastUpdate da janela. Fase 2: busca só os alterados, em blocos por faixa de ids,
/// com o $expand dividido em grupos, e grava bloco a bloco.
/// </summary>
public async Task<bool?> GetTickets()
{
    _logger
        .Clear()
        .AddLog(EnumJob.MovideskGetTickets);

    var totalInseridos = 0;
    var companys = await _movideskRepository.GetCompanys();

    // início da última chamada à API: os 6 s do limite contam a partir dele
    var lastRequestAt = DateTime.MinValue;

    foreach (var company in companys)
    {
        if (company?.doc_company is null)
            continue;

        // lastUpdate é UTC na API; 1 hora de folga para o atraso de replicação. Banco vazio: /tickets só traz até 90 dias
        var lastUpdate = await _movideskTicketsRepository.GetTicketsLastUpdate(company.doc_company);
        var since = lastUpdate.HasValue ? lastUpdate.Value.AddHours(-1) : DateTime.UtcNow.AddDays(-89);

        // fase 1: id e lastUpdate dos tickets da janela, em ordem de id (1.000 por chamada, o máximo da API)
        var listed = new List<(long id, DateTime? lastUpdate)>();
        var listFailed = false;
        var skip = 0;

        while (true)
        {
            // espera o que falta para 6 s desde o início da chamada anterior (limite de 10 requisições por minuto)
            var wait = lastRequestAt.AddMilliseconds(RequestIntervalInMilliseconds) - DateTime.UtcNow;

            if (wait > TimeSpan.Zero)
                await Task.Delay(wait);

            lastRequestAt = DateTime.UtcNow;

            var response = await _apiClient.GetAsync(
                route: "tickets",
                queryParams: new Dictionary<string, string>
                {
                    { "token", company.Token.ToString() },
                    { "$select", "id,lastUpdate" },
                    { "$filter", $"lastUpdate ge {since:yyyy-MM-ddTHH:mm:ss}.00z" },
                    { "$orderby", "id asc" },
                    { "$top", IdsPageSize.ToString() },
                    { "$skip", skip.ToString() }
                }
            );

            List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>? page;

            try
            {
                page = JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>>(response ?? String.Empty);
            }
            catch (JsonException)
            {
                // a API devolve um objeto de erro (ou 429 Too many failed requests) em vez da lista
                _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, lista de ids, skip {skip}): {response}");
                listFailed = true;
                break;
            }

            if (page is null || page.Count == 0)
                break;

            listed.AddRange(page
                .Select(x => (id: ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), lastUpdate: NullableConverters.ToNullableDateTime(x.lastUpdate)))
                .Where(x => x.id != 0));

            if (page.Count < IdsPageSize)
                break;

            skip += IdsPageSize;
        }

        // erro na lista de ids: nada foi buscado ainda, a empresa fica para a próxima execução
        if (listFailed || listed.Count == 0)
            continue;

        // um ticket alterado durante a leitura pode aparecer em duas páginas
        listed = listed
            .GroupBy(x => x.id)
            .Select(g => g.Last())
            .OrderBy(x => x.id)
            .ToList();

        // cache de estado: id -> ticket gravado (com lastUpdate), por empresa
        var ticketsCache = (await _cache.GetOrCreateAsync($"{TicketsKey}:{company.doc_company}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            var list = await _movideskTicketsRepository.GetTicketsByIds(company.doc_company, listed.Select(x => x.id).ToList());

            return list
                .Where(x => x != null)
                .GroupBy(x => x!.id)
                .ToDictionary(g => g.Key, g => g.First()!);
        }))!;

        // só os novos ou com lastUpdate mais recente que o gravado vão para as chamadas caras
        var changed = listed
            .Where(x => !(ticketsCache.TryGetValue(x.id, out var y) && x.lastUpdate <= y.lastUpdate))
            .Select(x => x.id)
            .ToHashSet();

        // monta os blocos da fase 2 a partir de todos os ids da fase 1 (em ordem). Cada bloco começa num ticket
        // alterado, tem no máximo PageSize ids e termina no último alterado da janela; assim a faixa
        // id ge {primeiro} and id le {último} nunca tem mais de PageSize tickets do filtro.
        var ids = listed.Select(x => x.id).ToList();
        var blocks = new List<List<long>>();
        var i = 0;

        while (i < ids.Count)
        {
            if (!changed.Contains(ids[i]))
            {
                i++;
                continue;
            }

            var window = ids.Skip(i).Take(PageSize).ToList();
            var lastChanged = window.FindLastIndex(changed.Contains);

            blocks.Add(window.Take(lastChanged + 1).ToList());
            i += lastChanged + 1;
        }

        var apiFailed = false;

        foreach (var block in blocks)
        {
            // fase 2: busca o bloco com uma chamada por grupo do $expand e junta as respostas por id
            var filter = $"id ge {block[0]} and id le {block[^1]} and lastUpdate ge {since:yyyy-MM-ddTHH:mm:ss}.00z";
            var merged = new Dictionary<long, JObject>();
            var seen = new Dictionary<long, int>();
            var blockFull = false;

            foreach (var (select, expand) in TicketsExpandGroups)
            {
                // espera o que falta para 6 s desde o início da chamada anterior (limite de 10 requisições por minuto)
                var wait = lastRequestAt.AddMilliseconds(RequestIntervalInMilliseconds) - DateTime.UtcNow;

                if (wait > TimeSpan.Zero)
                    await Task.Delay(wait);

                lastRequestAt = DateTime.UtcNow;

                var response = await _apiClient.GetAsync(
                    route: "tickets",
                    queryParams: new Dictionary<string, string>
                    {
                        { "token", company.Token.ToString() },
                        { "$select", select },
                        { "$expand", expand },
                        { "$filter", filter },
                        { "$orderby", "id asc" },
                        { "$top", BlockTop.ToString() }
                    }
                );

                JArray page;

                try
                {
                    // lê a resposta mantendo datas e decimais como vieram (o DTO é todo string); o JArray.Parse padrão
                    // converteria as datas e mudaria o formato. Lança JsonReaderException quando a resposta não é uma lista.
                    using var reader = new JsonTextReader(new StringReader(response ?? String.Empty))
                    {
                        DateParseHandling = DateParseHandling.None,
                        FloatParseHandling = FloatParseHandling.Decimal
                    };

                    page = JArray.Load(reader);
                }
                catch (JsonReaderException)
                {
                    _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, tickets {block[0]} a {block[^1]}, $expand {expand}): {response}");
                    apiFailed = true;
                    break;
                }

                // $top cheio: pode ter ficado ticket de fora; o bloco é remontado na próxima execução
                if (page.Count >= BlockTop)
                {
                    _logger.AddMessage($"Bloco de tickets {block[0]} a {block[^1]} (empresa {company.doc_company}) com {page.Count} registros: não gravado, será remontado na próxima execução.");
                    blockFull = true;
                    break;
                }

                foreach (var item in page.OfType<JObject>())
                {
                    var id = ConverterValidatorsExtensions.ConvertToInt64Validation((string?)item["id"]);

                    // tickets que não mudaram, ou que entraram no filtro depois da fase 1 (chegam na próxima execução)
                    if (!changed.Contains(id))
                        continue;

                    if (merged.TryGetValue(id, out var ticket))
                        ticket.Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
                    else
                        merged[id] = item;

                    seen[id] = seen.GetValueOrDefault(id) + 1;
                }
            }

            // a API devolveu erro: interrompe a empresa em vez de tentar de novo (3 erros seguidos bloqueiam a API).
            // Os blocos anteriores já foram gravados.
            if (apiFailed)
                break;

            if (blockFull)
                continue;

            // ticket sem algum grupo não é gravado: o merge apagaria as filhas que faltaram. Volta na próxima execução,
            // porque o lastUpdate gravado continua antigo
            foreach (var id in merged.Keys.Where(id => seen[id] < TicketsExpandGroups.Length).ToList())
            {
                _logger.AddMessage($"Ticket {id} (empresa {company.doc_company}) não veio em todos os grupos do $expand: não gravado nesta execução.");
                merged.Remove(id);
            }

            if (merged.Count == 0)
                continue;

            var tickets = merged.Values.Select(x => x.ToObject<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>()!).ToList();
            var ticketsJson = merged.ToDictionary(x => x.Key, x => x.Value.ToString());

            // um horário de lote por bloco: ticket, filhas e netas gravam o mesmo valor (condição de exclusão do merge)
            totalInseridos += await SaveTickets(company.doc_company, _movideskService.GetBatchTime(), tickets, ticketsJson, ticketsCache);
        }
    }

    _logger.AddMessage(
        $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
    );

    _logger.SetLogEndDate();
    await _logger.CommitAllChanges();

    return true;
}
```

### 4.3 Onde ficou cada passo

| Passo | Bloco do `GetTickets` |
|---|---|
| Lista de ids e `lastUpdate` (fase 1) | `while (true)` com `$select=id,lastUpdate`; um erro marca `listFailed` e a empresa fica para a próxima execução |
| Cache de estado | `_cache.GetOrCreateAsync` montado com os ids da fase 1 |
| Montagem dos blocos | `while (i < ids.Count)`, que preenche `blocks` |
| Busca de um bloco (fase 2) | `foreach (var (select, expand) in TicketsExpandGroups)`; erro marca `apiFailed` (interrompe a empresa) e `$top` cheio marca `blockFull` (pula o bloco) |
| Leitura do `JArray` sem converter datas | `JsonTextReader` com `DateParseHandling.None`, dentro do `try` da fase 2 |
| Espera entre chamadas | as 4 linhas com `lastRequestAt`, antes de cada chamada da fase 1 e de cada grupo; `lastRequestAt` é variável local do método |

## 5. O que não muda

* DTOs, entidades, validadores, repositório, tabelas e procedures de merge.
* O `GetTicketById` (por número), que usa `tickets?id=` e já traz o ticket inteiro em ~1,6 s.
* A janela de busca: o maior `lastUpdate` gravado menos 1 hora, ou 89 dias com o banco vazio.
* O `htmlDescription`: continua preservado a partir do que já está gravado, porque a listagem não o traz.

## 6. Checklist de implementação

1. Constantes `PageSize = 100` (hoje 25), `BlockTop = 150`, `IdsPageSize` e `TicketsExpandGroups`; remover `TicketsExpand`.
2. Espera de 6 s contada do início da chamada anterior, no lugar do `Thread.Sleep`.
3. Fase 1 (lista de ids) e cache montado com os ids da fase 1.
4. Montagem dos blocos e fase 2 (5 grupos por bloco).
5. Gravação por bloco. Os passos 2 a 5 ficam dentro do `GetTickets`, sem métodos privados novos.
6. Ao registrar o job recorrente, aplicar `[DisableConcurrentExecution]` (seção 3.7).
7. Atualizar o README do módulo e o [IMPLEMENTACAO-MOVIDESK-GETTICKETS.md](IMPLEMENTACAO-MOVIDESK-GETTICKETS.md).
8. Compilar e medir uma execução real: tempo por bloco, total de chamadas e se os tickets gravados batem com o
   `GetTicketById` de alguns ids (mesmas ações, históricos e campos adicionais).

---

## 7. Decisões (21/10/2026)

| # | Pergunta | Decisão |
|---|---|---|
| 1 | Manter os 5 grupos ou juntar os dois mais leves? | **Manter os 5 grupos** (seção 2.3) |
| 2 | Tamanho do bloco | **Testar 100.** Medido na seção 2.4: ~24 s por bloco de 100, praticamente o mesmo que um de 25. Adotado: blocos de 100 com `$top=150` |
| 3 | Execuções sobrepostas | **Impedir ao registrar o job**, com `[DisableConcurrentExecution]` (seção 3.7) |
| 4 | Ticket incompleto | **Só mensagem** em `auditing.Messages`; o ticket volta na próxima execução (seção 3.4) |
