# Movidesk — Implementação do `GetTicketById` (um ticket por id)

Guia para implementar `MovideskTicketsService.GetTicketById`: recebe a empresa, o número de um ticket e o usuário que
pediu, busca **só esse ticket** na API da Movidesk, grava nas mesmas tabelas do `GetTickets` e registra a chamada numa
tabela de log, como o AfterSale faz nas rotas de escrita.

> **Implementado em 20/10/2026.** O host `Hangfire` compila com 0 erros; o `Script006` rodou sem erro num banco
> temporário e as colunas batem com a entidade `Request`; as validações da rota foram testadas com o host rodando
> (seção 13). O `GetTicketById` ainda **não foi executado** contra a API nem o banco. Parte do código atual do `GetTickets`
> ([IMPLEMENTACAO-MOVIDESK-GETTICKETS.md](IMPLEMENTACAO-MOVIDESK-GETTICKETS.md)) e das tabelas de
> [MOVIDESK-MODELAGEM-TICKETS.md](MOVIDESK-MODELAGEM-TICKETS.md). As decisões de 20/10/2026 estão na seção 11.

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`.

---

## 1. Para que serve e como difere do `GetTickets`

| | `GetTickets` (já existe) | `GetTicketById` (novo) |
|---|---|---|
| Quando roda | Recorrente (job), para todas as empresas | Sob demanda, para uma empresa e um ticket, pedido por um usuário |
| O que busca | Tickets com `lastUpdate` desde a última execução, paginados | Um ticket pelo número |
| Chamada na API | `GET /tickets?$select=...&$expand=...&$filter=lastUpdate ge ...` | `GET /tickets?token=...&id=...` |
| `htmlDescription` das ações | Não vem na listagem; passa a **preservar** o que já está gravado (seção 7) | **Vem** |
| Limite de 90 dias | Sim (`/tickets` só lista `lastUpdate` de até 90 dias) | Não: a busca por id devolveu o ticket 1000, de 2021 |
| Cache de estado | Descarta os que não mudaram | **Sempre grava**: quem chamou quer o ticket atualizado |
| Log de requisição (`movidesk.MovideskRequests`) | Não | **Sim**, com o usuário (`X-User-Name`) |

Usos esperados: reprocessar um ticket específico, trazer um ticket antigo (fora da janela de 90 dias), preencher o
`htmlDescription` das ações e, no futuro, ser chamado por um webhook da Movidesk.

---

## 2. Comportamento da API (verificado)

| Situação | Resposta | Como foi verificado |
|---|---|---|
| Ticket existe | **200** com um objeto JSON (não uma lista), com todas as listas expandidas e `htmlDescription` preenchido | Tickets 69255 e 1000 (`Responsed Jsons/Ticket/GetTicketById`) |
| Ticket não existe | **404 com corpo vazio** (sem JSON, sem `content-type`) | `GET /tickets?id=999999999` em 20/10/2026 |
| Token inválido, limite de requisições (429) etc. | Não testado (testar gera bloqueio da API) | Documentação: 3 requisições com erro seguidas bloqueiam por 60 s, depois 120 s e 300 s |

Consequências para o código:
- O `IApiClient.GetAsync` **não devolve o status HTTP**, só o corpo. Um 404 chega como string vazia; é assim que o
  service reconhece "não encontrado".
- Qualquer outro corpo que não seja um objeto de ticket com o mesmo `id` (texto de erro, objeto de erro, 429) é tratado
  como resposta inválida e registrado no log com o conteúdo recebido.
- **Não há nova tentativa** em caso de erro: um 404 pode contar como "requisição com erro" para o bloqueio da API.
- Sem `$select`/`$expand`: a busca por id já devolve tudo.

---

## 3. Contrato

### 3.1 Service
`Application.Movidesk/Abstractions/Services/IMovideskTicketsService.cs`
```csharp
public interface IMovideskTicketsService
{
    /// <summary>
    /// busca um ticket por número, grava nas tabelas de tickets e registra a chamada em movidesk.MovideskRequests.
    /// true: gravado | false: o ticket não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
    /// </summary>
    public Task<bool?> GetTicketById(string user_name, string doc_company, long id);

    public Task<bool?> GetTickets();
}
```
`user_name` vem primeiro, na mesma ordem dos métodos de escrita do AfterSale (`PostApproveReverseById(user_name,
doc_company, id, ...)`).

### 3.2 Endpoint
```
GET MovideskJobs/Movidesk/Tickets/{docCompany}/{id}
Cabeçalho obrigatório: X-User-Name (até 100 caracteres)
```
- `docCompany` na rota porque o mesmo número de ticket pode existir em duas contas Movidesk (mesma decisão das rotas
  de escrita do AfterSale, `Reverses/{docCompany}/{id}`). Validado com `^\d{14}$`.
- `id` com a restrição `long:min(1)`.
- `X-User-Name` obrigatório, como no AfterSale: sem ele o ASP.NET responde 400 antes de chamar a API.

| Resultado do service | HTTP | Corpo |
|---|---|---|
| `true` | 200 | `Ticket {id} integrated successfully.` |
| `false` | 404 | `Ticket {id} not found in Movidesk.` |
| `null` | 422 | `Company not registered or invalid response from Movidesk. See the execution log.` |
| `X-User-Name` ausente ou com mais de 100 caracteres, `docCompany` fora de `^\d{14}$` | 400 | automático do `[ApiController]` |
| `id` menor que 1 | 404 | a restrição `long:min(1)` faz a rota não casar |

---

## 4. Auditoria (`EnumJob` e `Script005`)

Um job próprio, para o log não se misturar com o `GetTickets` recorrente:

`Domain.Movidesk/Enums/EnumJob.cs`
```csharp
/// <summary>
/// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql
/// </summary>
public enum EnumJob
{
    None = 0,

    MovideskGetTickets = 3000,
    MovideskGetPersons = 3001,
    MovideskGetTicketById = 3002
}
```
O comentário do enum ainda cita `Script002_Insert_Into_Jobs_Table.sql`, que foi renumerado; a correção acima vem junto.

`Infrastructure.Movidesk/Scripts/SQLServer/Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql`, num bloco de
ações sob demanda, como o AfterSale fez com as rotas POST/PUT (`IntervalTimeInMinutes = 0`):
```sql
/*Integrações*/
/*Range 3000 - 3499*/
(30, 3000, 'MovideskGetTickets', 'Integração de dados das tabelas MovideskTickets e filhas', 3, 2),
(30, 3001, 'MovideskGetPersons', 'Integração de dados da tabela MovideskPersons', 3, 2),

/*Sob demanda - executadas pelo MovideskController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(30, 3002, 'MovideskGetTicketById', 'Integração de um ticket por número (GET Tickets/{docCompany}/{id})', 0, 2);
```
As procedures de merge (3500–3513) não mudam: são as mesmas tabelas. Elas ficam registradas como filhas do job 3000,
mas a execução é ligada ao log do 3002 pelo `parentExecutionGUID`, como já acontece hoje.

**Banco local/existente:** o `Script005` não roda de novo num banco onde já rodou; insira a linha do 3002 à mão.
Sem ela, o insert do log falha na chave estrangeira para `auditing.Jobs`.

---

## 5. Log de requisições (`movidesk.MovideskRequests`)

Cópia do `aftersale.AfterSaleRequests` (AfterSale `Script009`): mesmas colunas, mesmos índices, mesma entidade e mesma
forma de gravar. Tabela só de inserção: gravada direto no `movidesk`, sem `untreated` nem procedure de merge.

### 5.1 Script
Novo arquivo `Infrastructure.Movidesk/Scripts/SQLServer/Script006_Create_Movidesk_Requests_Table.sql` (o DbUp roda em
ordem alfabética, então ele entra depois do `Script005` sem renumerar nada):
```sql
--log das chamadas sob demanda feitas pelo MovideskController na API da Movidesk.
--tabela somente de inserção: gravada direto no schema movidesk, sem untreated nem procedure de merge.
create table [movidesk].MovideskRequests (
	id bigint identity(1,1) not null,
	request_date datetime not null,
	execution uniqueidentifier not null, --auditing.Logs.Execution da chamada
	id_job int not null,                 --auditing.Jobs.IdJob (IdType = 2)
	[user_name] nvarchar(100) not null,  --cabeçalho X-User-Name
	http_method varchar(10) not null,
	[route] nvarchar(400) not null,      --rota chamada na API da Movidesk (sem o token)
	[doc_company] nvarchar(14) not null,
	field_key_value nvarchar(400) null,  --chave do registro (ex.: número do ticket)
	request_body nvarchar(max) null,     --corpo enviado; nulo nas chamadas GET
	response_body nvarchar(max) null,
	is_success bit not null,
constraint [PK_MovideskRequests] primary key (
	id asc
));

create nonclustered index [IX_MovideskRequests_Company_Key] on [movidesk].MovideskRequests ([doc_company], field_key_value);
create nonclustered index [IX_MovideskRequests_User_Date] on [movidesk].MovideskRequests ([user_name], request_date);
```

### 5.2 Entidade
`Domain.Movidesk/Entities/Writes/Requests/Request.cs` (pasta `Writes`, como no AfterSale: é algo que o sistema
registra, não uma leitura da API). Igual ao `Domain.AfterSale.Entities.Writes.Requests.Request`, só com o namespace e o
comentário trocados:
```csharp
namespace Domain.Movidesk.Entities.Writes.Requests
{
    /// <summary>
    /// log of an on-demand call made to the Movidesk API, table movidesk.MovideskRequests
    /// </summary>
    public class Request
    {
        public DateTime request_date { get; private set; }
        public Guid execution { get; private set; }
        public int id_job { get; private set; }

        public string user_name { get; private set; }
        public string http_method { get; private set; }
        public string route { get; private set; }
        public string doc_company { get; private set; }
        public string? field_key_value { get; private set; }

        public string? request_body { get; private set; }
        public string? response_body { get; private set; }

        public bool is_success { get; private set; }

        public Request() { }

        public Request(Guid execution, Enum job, string user_name, string http_method, string route, string doc_company, string? field_key_value, string? request_body, string? response_body, bool is_success)
        {
            request_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.execution = execution;
            id_job = Convert.ToInt32(job);

            this.user_name = user_name?.Length > 100 ? user_name.Substring(0, 100) : user_name ?? string.Empty;
            this.http_method = http_method;
            this.route = route?.Length > 400 ? route.Substring(0, 400) : route ?? string.Empty;
            this.doc_company = doc_company?.Length > 14 ? doc_company.Substring(0, 14) : doc_company ?? string.Empty;
            this.field_key_value = field_key_value?.Length > 400 ? field_key_value.Substring(0, 400) : field_key_value;

            this.request_body = request_body;
            this.response_body = response_body;

            this.is_success = is_success;
        }
    }
}
```

### 5.3 Repositório
No `IMovideskRepository` / `MovideskRepository` (o repositório do módulo, como o `IAfterSaleRepository` tem o
`InsertIntoAfterSaleRequests`):
```csharp
public bool InsertIntoMovideskRequests(Domain.Movidesk.Entities.Writes.Requests.Request request);
```
```csharp
/// <summary>
/// log table of the on-demand calls: insert-only, so it goes straight to the movidesk schema (no untreated/merge)
/// </summary>
public bool InsertIntoMovideskRequests(Domain.Movidesk.Entities.Writes.Requests.Request request)
{
    var requestsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Entities.Writes.Requests.Request(), tableName: "MovideskRequests");

    _coreRepository.FillSystemDataTable(requestsTable, [request]);

    return _coreRepository.BulkInsertIntoTableRaw(requestsTable, "movidesk");
}
```

### 5.4 O que é gravado no `GetTicketById`
| Coluna | Valor |
|---|---|
| `user_name` | cabeçalho `X-User-Name` |
| `http_method` | `GET` |
| `route` | `tickets?id={id}` — **sem o token**, que nunca vai para o log |
| `doc_company` / `field_key_value` | CNPJ da rota / número do ticket |
| `request_body` | `null` (GET não tem corpo) |
| `response_body` | o JSON devolvido pela API; vazio no 404; a mensagem da exceção se a chamada não tiver resposta (timeout, rede) |
| `is_success` | `true` só quando o ticket foi gravado |

**Atenção:** como no AfterSale, a resposta inteira vai para `response_body`. O JSON do ticket tem nomes, e-mails e
telefones de clientes e o texto das ações, então o acesso a essa tabela deve ser restrito como o de `auditing.Records`,
que já guarda o mesmo JSON.

Se a empresa não estiver cadastrada, **não** há registro em `MovideskRequests` (nenhuma chamada foi feita à API),
igual ao AfterSale; o motivo fica no log de auditoria.

---

## 6. Empresa por documento

Hoje só existe `GetCompanys()` (todas). Entra uma busca de uma empresa, como o `GetCompany(doc_company)` do AfterSale.

`Application.Movidesk/Abstractions/Handlers/Commands/IMovideskCommandHandler.cs`
```csharp
public string CreateGetCompanyQuery(string doc_company);
```
`Application.Movidesk/Handlers/Commands/MovideskCommandHandler.cs`
```csharp
public string CreateGetCompanyQuery(string doc_company)
{
    return @$"SELECT DISTINCT
             DOC_COMPANY AS DOC_COMPANY,
             TOKEN AS TOKEN
             FROM MOVIDESK.MOVIDESKPARAMETERS
             WHERE DOC_COMPANY = '{doc_company}'";
}
```
O valor entra na query como texto, mas só depois de passar pelo `^\d{14}$` do controller, então não há como injetar
SQL por ele. A versão do AfterSale monta `DOC_COMPANY = {doc_company}` sem aspas; aqui as aspas ficam, porque a coluna é
`nvarchar` e um CNPJ que começa com zero perderia o zero se fosse comparado como número.

`IMovideskRepository` / `MovideskRepository`
```csharp
public Task<Company?> GetCompany(string doc_company);
```
```csharp
public async Task<Company?> GetCompany(string doc_company)
{
    string sql = _movideskCommandHandler.CreateGetCompanyQuery(doc_company);

    return await _coreRepository.GetRecord<Company>(sql);
}
```

---

## 7. `htmlDescription`: preservar o que já foi gravado

**Problema:** a listagem (`GetTickets`) não devolve `htmlDescription`. Quando ela regrava um ticket que já tinha o HTML
gravado (por um `GetTicketById`), o merge faz `update` de todas as colunas e o HTML volta a ficar nulo.

**Solução:** antes de gravar, o service completa o `htmlDescription` que veio vazio com o que já está no banco. Não
custa nenhuma requisição à API (uma consulta ao banco por lote), e vale para os dois métodos:
- **`GetTicketById`:** as ações chegam com o HTML da API, então nada é trocado (só completa uma ação que a API mandar sem HTML).
- **`GetTickets`:** as ações já conhecidas mantêm o HTML gravado. **Ações novas** que chegam pela listagem ficam sem
  HTML até alguém chamar o `GetTicketById` daquele ticket; buscar o HTML de cada ação nova na listagem custaria uma
  requisição por ticket (limite de 10 por minuto) e fica de fora.

### 7.1 Query
`IMovideskCommandHandler` / `MovideskCommandHandler`
```csharp
public string CreateGetActionsHtmlDescriptionsQuery(string doc_company, List<long> ticketIds);
```
```csharp
public string CreateGetActionsHtmlDescriptionsQuery(string doc_company, List<long> ticketIds)
{
    // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
    return @$"SELECT DOC_COMPANY, TICKETID, ID, HTMLDESCRIPTION
             FROM MOVIDESK.MOVIDESKTICKETSACTIONS
             WHERE DOC_COMPANY = '{doc_company}'
             AND TICKETID IN ({String.Join(", ", ticketIds)})
             AND HTMLDESCRIPTION IS NOT NULL";
}
```

### 7.2 Repositório de tickets
`IMovideskTicketsRepository` / `MovideskTicketsRepository`
```csharp
public Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Action?>> GetActionsHtmlDescriptions(string doc_company, List<long> ticketIds);
```
```csharp
public async Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Action?>> GetActionsHtmlDescriptions(string doc_company, List<long> ticketIds)
{
    var result = new List<Domain.Movidesk.Entities.Reads.Tickets.Action?>();

    // lotes de 1000 ids, para o IN não crescer sem limite num GetTickets com muitos tickets
    foreach (var chunk in ticketIds.Distinct().Chunk(1000))
    {
        string sql = _movideskCommandHandler.CreateGetActionsHtmlDescriptionsQuery(doc_company, chunk.ToList());
        result.AddRange(await _coreRepository.GetRecords<Domain.Movidesk.Entities.Reads.Tickets.Action>(sql));
    }

    return result;
}
```

### 7.3 Entidade `Action`
Um método para completar o HTML sem abrir o `set` da propriedade:
```csharp
/// <summary>
/// a listagem da API não traz htmlDescription: mantém o que já estava gravado em vez de sobrescrever com null
/// </summary>
public void KeepHtmlDescription(string? storedHtmlDescription)
{
    if (String.IsNullOrEmpty(htmlDescription))
        htmlDescription = storedHtmlDescription;
}
```

### 7.4 No `SaveTickets` (seção 8.1)
Antes do `BulkInsertIntoMovideskTickets`:
```csharp
var stored = (await _movideskTicketsRepository.GetActionsHtmlDescriptions(doc_company, _listSomenteNovos.Select(x => x.id).ToList()))
    .Where(x => x != null)
    .ToDictionary(x => (x!.ticketId, x.id), x => x!.htmlDescription);

foreach (var action in _listSomenteNovos.SelectMany(x => x.actions))
    if (stored.TryGetValue((action.ticketId, action.id), out var html))
        action.KeepHtmlDescription(html);
```

---

## 8. Service

### 8.1 Extrair a gravação comum (`SaveTickets`)
O `GetTickets` e o `GetTicketById` fazem a mesma coisa depois de ter os DTOs: validar, montar as entidades com o horário
do lote, preservar o `htmlDescription`, gravar, registrar o JSON no log e atualizar o cache. Para não duplicar, esse
trecho vira um método privado.

```csharp
/// <summary>
/// valida, monta as entidades (com o mesmo horário de lote), preserva o htmlDescription já gravado,
/// grava e atualiza o cache da empresa. Devolve a quantidade de tickets gravados.
/// </summary>
private async Task<int> SaveTickets(
    string doc_company,
    DateTime batchTime,
    IEnumerable<Domain.Movidesk.Dtos.Reads.Tickets.Ticket> tickets,
    IReadOnlyDictionary<long, string> ticketsJson,
    Dictionary<long, Domain.Movidesk.Entities.Reads.Tickets.Ticket>? ticketsCache)
{
    var _listSomenteNovos = new List<Domain.Movidesk.Entities.Reads.Tickets.Ticket>();

    // o mesmo ticket pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
    foreach (var ticket in tickets.GroupBy(x => x.id).Select(g => g.Last()))
    {
        var validations = _ticketValidator.Validate(ticket);

        if (validations.Errors.Count > 0)
        {
            var message = String.Empty;
            validations.Errors.ForEach(s => message += $"ticket {ticket.id}: {s.ErrorMessage}\n");
            _logger.AddMessage(message);
        }

        _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.Tickets.Ticket(doc_company, batchTime, ticket));
    }

    if (_listSomenteNovos.Count == 0)
        return 0;

    // a listagem não traz htmlDescription: mantém o que já está gravado em vez de sobrescrever com null
    var stored = (await _movideskTicketsRepository.GetActionsHtmlDescriptions(doc_company, _listSomenteNovos.Select(x => x.id).ToList()))
        .Where(x => x != null)
        .ToDictionary(x => (x!.ticketId, x.id), x => x!.htmlDescription);

    foreach (var action in _listSomenteNovos.SelectMany(x => x.actions))
        if (stored.TryGetValue((action.ticketId, action.id), out var html))
            action.KeepHtmlDescription(html);

    await _movideskTicketsRepository.BulkInsertIntoMovideskTickets(_listSomenteNovos, _logger.GetExecutionGuid());

    foreach (var record in _listSomenteNovos)
    {
        if (ticketsJson.TryGetValue(record.id, out var json))
            _logger.AddRecord(record.id.ToString(), json);

        if (ticketsCache is not null)
            ticketsCache[record.id] = record;
    }

    return _listSomenteNovos.Count;
}
```

No `GetTickets`, o trecho que hoje vai de `var _listSomenteNovos = ...` até `totalInseridos += _listSomenteNovos.Count;`
passa a ser:
```csharp
totalInseridos += await SaveTickets(company.doc_company, batchTime, tickets, ticketsJson, ticketsCache);
```
A única mudança de comportamento no `GetTickets` é a preservação do `htmlDescription` (seção 7).

### 8.2 `GetTicketById`
```csharp
/// <summary>
/// busca um ticket por número, grava nas tabelas de tickets (sempre, sem consultar o cache)
/// e registra a chamada em movidesk.MovideskRequests
/// </summary>
public async Task<bool?> GetTicketById(string user_name, string doc_company, long id)
{
    _logger
        .Clear()
        .AddLog(EnumJob.MovideskGetTicketById);

    // rota registrada no log de requisições: sem o token
    var route = $"tickets?id={id}";
    bool? result;

    try
    {
        var company = await _movideskRepository.GetCompany(doc_company);

        if (company is null)
        {
            // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao AfterSale)
            _logger.AddMessage($"Empresa {doc_company} não cadastrada em movidesk.MovideskParameters.");
            return null;
        }

        // mesmo horário de lote para o ticket e as filhas (condição de exclusão do merge)
        var batchTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        string? response;

        try
        {
            // por id a API já devolve todas as listas e o htmlDescription: sem $select/$expand
            response = await _apiClient.GetAsync(
                route: "tickets",
                queryParams: new Dictionary<string, string>
                {
                    { "token", company.Token.ToString() },
                    { "id", id.ToString() }
                }
            );
        }
        catch (Exception ex)
        {
            // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
            SaveRequest(EnumJob.MovideskGetTicketById, user_name, "GET", route, doc_company, id, $"Exceção ao chamar a API: {ex.Message}", false);
            throw;
        }

        if (String.IsNullOrWhiteSpace(response))
        {
            // a API responde 404 com corpo vazio quando o ticket não existe
            _logger.AddMessage($"Ticket {id} não encontrado na Movidesk (empresa {doc_company}).");
            result = false;
        }
        else
        {
            Domain.Movidesk.Dtos.Reads.Tickets.Ticket? ticket = null;

            try
            {
                ticket = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>(response);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                // ticket continua null: tratado abaixo como resposta inválida
            }

            if (ticket is null || ConverterValidatorsExtensions.ConvertToInt64Validation(ticket.id) != id)
            {
                _logger.AddMessage($"Resposta inválida da API Movidesk para o ticket {id} (empresa {doc_company}): {response}");
                result = null;
            }
            else
            {
                // atualiza o cache do GetTickets, se ele estiver carregado, para a próxima execução não regravar o ticket
                _cache.TryGetValue($"{TicketsKey}:{doc_company}", out Dictionary<long, Domain.Movidesk.Entities.Reads.Tickets.Ticket>? ticketsCache);

                await SaveTickets(doc_company, batchTime, [ticket], new Dictionary<long, string> { { id, response } }, ticketsCache);

                _logger.AddMessage($"Concluída com sucesso: ticket {id} inserido ou atualizado!");
                result = true;
            }
        }

        SaveRequest(EnumJob.MovideskGetTicketById, user_name, "GET", route, doc_company, id, response, result == true);

        return result;
    }
    finally
    {
        _logger.SetLogEndDate();
        await _logger.CommitAllChanges();
    }
}

private void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, long id, string? responseBody, bool isSuccess) =>
    _movideskRepository.InsertIntoMovideskRequests(
        new Domain.Movidesk.Entities.Writes.Requests.Request(
            execution: _logger.GetExecutionGuid() ?? Guid.Empty,
            job: job,
            user_name: user_name,
            http_method: httpMethod,
            route: route,
            doc_company: doc_company,
            field_key_value: id.ToString(),
            request_body: null,
            response_body: responseBody,
            is_success: isSuccess
        )
    );
```
Pontos do código:
- **Mesma estrutura do `SendReverseAction` do AfterSale:** `try/finally` para o log de auditoria ser gravado em todos os
  casos, e o registro em `MovideskRequests` também quando a chamada lança exceção (depois disso a exceção segue, e o
  endpoint responde 500).
- **`SaveRequest` depois de gravar o ticket:** se o `BulkInsertIntoMovideskTickets` falhar, a exceção sobe antes do
  registro; o erro fica no log de auditoria. Para registrar também esse caso como `is_success = false`, o bloco do
  `SaveTickets` pode ganhar um `try/catch` que chama o `SaveRequest` e relança.
- **Conferência do `id` devolvido:** se a API devolver outro objeto (de erro, por exemplo), o `id` não bate e a resposta
  é tratada como inválida, em vez de gravar algo errado.
- **Sem consulta ao cache antes de gravar:** a chamada é explícita; mesmo um ticket sem mudanças é regravado (e ganha
  o `htmlDescription`).
- **Cache depois de gravar:** só é atualizado se já existir (`TryGetValue`). O `GetTicketById` não cria o cache da empresa,
  porque o `GetTickets` o carrega com os ids do lote dele.
- `using`s: os mesmos que o `MovideskTicketsService` já tem.

---

## 9. Controller

`src/Backend/Hosts/WebApi/Controllers/General/MovideskController.cs`, depois do endpoint `Tickets`, com o cabeçalho
`X-User-Name` igual ao `AfterSaleController`:
```csharp
/// <summary>
/// get one ticket by number (GET tickets?id={id}) and save it; the call is logged in movidesk.MovideskRequests
/// </summary>
[HttpGet("Tickets/{docCompany}/{id:long:min(1)}")]
[Tags("MovideskTickets")]
public async Task<ActionResult> Ticket(
    [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
    [RegularExpression(@"^\d{14}$")] string docCompany,
    long id
)
{
    var result = await _movideskTicketsService.GetTicketById(userName, docCompany, id);

    return result switch
    {
        true => Ok($"Ticket {id} integrated successfully."),
        false => NotFound($"Ticket {id} not found in Movidesk."),
        null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
    };
}
```
`using System.ComponentModel.DataAnnotations;` no controller, para `[Required]`, `[StringLength]` e
`[RegularExpression]`. O Swagger passa a mostrar o campo `X-User-Name` nessa rota.

O endpoint `Tickets` (o job recorrente) continua sem `X-User-Name`, como os GETs de integração do AfterSale.

A rota `Tickets` e a nova `Tickets/{docCompany}/{id}` não conflitam: o ASP.NET escolhe pela quantidade de segmentos.

---

## 10. Riscos e limitações

- **Execução simultânea com o `GetTickets`:** as procedures de merge usam `sp_TryApplock`. Se um `GetTicketById` rodar
  enquanto o `GetTickets` está no merge da mesma tabela, a procedure falha com "The process is already running", e o
  `CallDbProcMerge` lança erro: a chamada devolve 500 e o ticket pode ter ficado só na `untreated`. Ele entra no
  próximo merge (o merge processa tudo que está na `untreated`). Vale reexecutar o `GetTicketById` nesse caso.
- **Bloqueio da API:** três chamadas com erro seguidas (inclusive ids que não existem, se a Movidesk contar o 404 como
  erro) bloqueiam o token por 60 s. Um sistema externo que chame o endpoint em sequência deve respeitar isso.
- **Ticket de outra conta:** se o número existir em outra conta Movidesk, mas não na do `docCompany`, a resposta é 404;
  o endpoint responde 404, corretamente.
- **Ações novas sem HTML:** vindas pelo `GetTickets`, ficam sem `htmlDescription` até um `GetTicketById` daquele ticket
  (seção 7).
- **Dados pessoais no log:** `MovideskRequests.response_body` guarda o JSON completo do ticket (seção 5.4).

---

## 11. Decisões (aplicadas em 20/10/2026)

| # | Decisão | Onde |
|---|---|---|
| 1 | Rota com `docCompany`: `GET Tickets/{docCompany}/{id}` | 3.2, 9 |
| 2 | Job próprio `MovideskGetTicketById = 3002`, sob demanda (`IntervalTimeInMinutes = 0`) | 4 |
| 3 | `htmlDescription` tratado já: o `GetTickets` preserva o HTML gravado em vez de sobrescrever com `null` | 7, 8.1 |
| 4 | Usuário obrigatório pelo cabeçalho `X-User-Name` e log das chamadas em `movidesk.MovideskRequests`, como no AfterSale | 5, 8.2, 9 |

Não há decisões pendentes neste guia.

---

## 12. Checklist

- [ ] `EnumJob.MovideskGetTicketById = 3002` e comentário apontando para o `Script005`
- [ ] Linha do job 3002 no `Script005` (e no banco local, à mão)
- [ ] `Script006_Create_Movidesk_Requests_Table.sql` (e rodar no banco local)
- [ ] Entidade `Entities/Writes/Requests/Request.cs`
- [ ] `IMovideskRepository`: `GetCompany` e `InsertIntoMovideskRequests` + queries e implementação
- [ ] `IMovideskTicketsRepository.GetActionsHtmlDescriptions` + query (em lotes de 1000)
- [ ] `Action.KeepHtmlDescription`
- [ ] `IMovideskTicketsService.GetTicketById(string user_name, string doc_company, long id)`
- [ ] `SaveTickets` privado (com a preservação do HTML), usado pelo `GetTickets` e pelo `GetTicketById`
- [ ] `GetTicketById` + `SaveRequest`, com o log de auditoria gravado em todos os casos
- [ ] Endpoint `GET Tickets/{docCompany}/{id}` com `X-User-Name` e 200/404/422
- [ ] Compilar o host
- [ ] Teste: um ticket existente (grava, inclusive `htmlDescription`, e registra em `MovideskRequests`), um inexistente
      (404 e registro com `is_success = 0`), um CNPJ não cadastrado (422, sem registro) e uma chamada sem `X-User-Name` (400)

---

## 13. O que mudou na implementação

- **`[Authorize]` comentado no `MovideskController`**, como já está no `AfterSaleController`. O host não tem esquema de
  autenticação configurado: com o atributo, **qualquer** rota da Movidesk (inclusive o `GET Tickets` que já existia)
  falhava com 500 antes de validar os parâmetros. O 500 ainda vinha mascarado por um `NullReferenceException` no
  `ExceptionHandler` do Shared (linha 26 lê `exception.InnerException.Message` sem checar nulo), que é um problema à
  parte, anterior a esta mudança.
- **`id` menor que 1 responde 404**, não 400: a restrição `long:min(1)` impede a rota de casar.

Testes feitos com o host rodando (sem chamar a API nem o banco):

| Chamada | Resultado |
|---|---|
| Swagger | `GET /MovideskJobs/Movidesk/Tickets/{docCompany}/{id}` com `X-User-Name` (header, obrigatório), `docCompany` e `id` |
| Sem `X-User-Name` | 400, "The userName field is required." |
| `X-User-Name` com 101 caracteres | 400 |
| `docCompany` = `123` | 400, "must match the regular expression '^\d{14}$'" |
| `id` = 0 | 404 |

