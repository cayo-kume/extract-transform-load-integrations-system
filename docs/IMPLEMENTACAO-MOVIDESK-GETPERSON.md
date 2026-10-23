# Movidesk — Implementação do `GetPersonById` (uma pessoa por id)

Guia para implementar `MovideskPeopleService.GetPersonById`: recebe a empresa, o Cód. Ref. de uma pessoa e o usuário que
pediu, busca **só essa pessoa** na API da Movidesk, grava nas mesmas tabelas do `GetPeople` e registra a chamada em
`movidesk.MovideskRequests`. É o equivalente do `GetTicketById` para pessoas
([IMPLEMENTACAO-MOVIDESK-GETTICKET.md](IMPLEMENTACAO-MOVIDESK-GETTICKET.md)).

> **Implementado em 21/10/2026.** O host `Hangfire` compila com 0 erros; o host sobe e o Swagger lista
> `GET MovideskJobs/Movidesk/People/{docCompany}/{id}` com `X-User-Name`. As validações da rota responderam 400 (sem
> cabeçalho, `id` com `-` ou `.`, `id` com 65 caracteres, CNPJ inválido). O `GetPersonById` ainda **não foi executado**
> contra a API nem o banco. Ele parte do código atual do `GetPeople`
> ([IMPLEMENTACAO-MOVIDESK-GETPEOPLE.md](IMPLEMENTACAO-MOVIDESK-GETPEOPLE.md)) e do `GetTicketById`, já implementados.
> As decisões de 21/10/2026 estão na seção 10; uma delas cria o `MovideskService` com os métodos em comum (seção 6.1).

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`. Uma classe por arquivo `.cs`.

---

## 1. Para que serve

| | `GetPeople` (já existe) | `GetPersonById` (novo) |
|---|---|---|
| Quando roda | Recorrente (job), para todas as empresas | Sob demanda, para uma empresa e uma pessoa, pedido por um usuário |
| O que busca | Pessoas criadas ou alteradas desde a última execução (12 meses no banco vazio) | Uma pessoa pelo Cód. Ref. |
| Chamada na API | `GET /persons?$filter=changedDate ge ... or createdDate ge ...` | `GET /persons?token=...&id=...` |
| Cache de estado | Descarta quem não mudou | **Sempre grava** |
| Log de requisição | Não | **Sim**, em `movidesk.MovideskRequests`, com o `X-User-Name` |

**Uso principal:** trazer pessoas que ficaram fora da primeira carga de 12 meses. Nas respostas reais, o agente
responsável pelo ticket 69255 teve a última alteração em 08/2024: o `GetPeople` não o traz, mas os tickets apontam para
ele em `ownerId`. Um `GetPersonById` com o id dele resolve. Também serve para reprocessar uma pessoa específica.

---

## 2. Comportamento da API (verificado)

| Situação | Resposta | Como foi verificado |
|---|---|---|
| Pessoa existe | **200** com um objeto JSON (não uma lista), com todas as listas | Pessoas consultadas em 20/10/2026 (`Responsed Jsons/Person/GetPersonById`) |
| Pessoa não existe | **404 com um JSON de erro** (`application/problem+json`): `{"type": "...", "title": "Not Found", "status": 404, "traceId": "..."}` | `GET /persons?id=nao-existe-999999999` em 21/10/2026 |
| Token inválido, limite (429) etc. | Não testado (testar gera bloqueio da API) | Documentação: 3 requisições com erro seguidas bloqueiam por 60 s, depois 120 s e 300 s |

**Diferença importante em relação aos tickets:** o `GET /tickets?id=` de um ticket inexistente devolve **corpo vazio**;
o de pessoas devolve **um objeto JSON**. Como o `IApiClient.GetAsync` não devolve o status HTTP, o service precisa olhar
o conteúdo:
- corpo com `"status": 404` e sem `"id"` → **não encontrado**;
- corpo vazio → também tratado como não encontrado (por segurança, caso a API mude);
- qualquer outro objeto sem o mesmo `id` pedido → **resposta inválida**.

Se o corpo do 404 fosse simplesmente desserializado como `Person`, viria um objeto com `id = null`, que seria tratado
como resposta inválida (422) em vez de não encontrado (404). Por isso a checagem do `status` vem antes.

---

## 3. Contrato

### 3.1 Service
`Application.Movidesk/Abstractions/Services/IMovideskPeopleService.cs`
```csharp
public interface IMovideskPeopleService
{
    /// <summary>
    /// busca uma pessoa pelo Cód. Ref., grava nas tabelas de pessoas e registra a chamada em movidesk.MovideskRequests.
    /// true: gravada | false: a pessoa não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
    /// </summary>
    public Task<bool?> GetPersonById(string user_name, string doc_company, string id);

    public Task<bool?> GetPeople();
}
```
Mesma ordem de parâmetros do `GetTicketById(user_name, doc_company, id)`; o `id` é `string`, porque o Cód. Ref. é
alfanumérico (`string(64)`).

### 3.2 Endpoint
```
GET MovideskJobs/Movidesk/People/{docCompany}/{id}
Cabeçalho obrigatório: X-User-Name (até 100 caracteres)
```
- `docCompany` validado com `^\d{14}$`, como no `GetTicketById`.
- `id` com `[StringLength(64, MinimumLength = 1)]` e `[RegularExpression(@"^[A-Za-z0-9]+$")]` (decidido): **só letras e
  números**, como diz a documentação ("Alfanumérico"). Qualquer outro caractere (`.`, `-`, `/`, espaço, aspas...) responde
  400 antes de chamar a API.

| Resultado do service | HTTP | Corpo |
|---|---|---|
| `true` | 200 | `Person {id} integrated successfully.` |
| `false` | 404 | `Person {id} not found in Movidesk.` |
| `null` | 422 | `Company not registered or invalid response from Movidesk. See the execution log.` |
| `X-User-Name`, `docCompany` ou `id` ausentes/inválidos | 400 | automático do `[ApiController]` |

---

## 4. Auditoria (`EnumJob` e `Script005`)

Job próprio, no mesmo bloco sob demanda do `MovideskGetTicketById`:

`Domain.Movidesk/Enums/EnumJob.cs`
```csharp
/*Sob demanda*/
MovideskGetTicketById = 3002,
MovideskGetPersonById = 3003
```

`Infrastructure.Movidesk/Scripts/SQLServer/Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql`, no bloco
"Sob demanda":
```sql
(30, 3002, 'MovideskGetTicketById', 'Integração de um ticket por número (GET Tickets/{docCompany}/{id})', 0, 2),
(30, 3003, 'MovideskGetPersonById', 'Integração de uma pessoa por Cód. Ref. (GET People/{docCompany}/{id})', 0, 2);
```
As procedures de merge (3516–3525) não mudam. **Banco local/existente:** inserir a linha do 3003 à mão (o `Script005`
não roda de novo onde já rodou).

---

## 5. Log de requisições

Reaproveita tudo o que o `GetTicketById` criou: a tabela `movidesk.MovideskRequests` (`Script006`), a entidade
`Entities/Writes/Requests/Request` e o `IMovideskRepository.InsertIntoMovideskRequests`. Nada novo no banco.

| Coluna | Valor |
|---|---|
| `user_name` | cabeçalho `X-User-Name` |
| `http_method` | `GET` |
| `route` | `persons?id={id}` — **sem o token** |
| `doc_company` / `field_key_value` | CNPJ da rota / Cód. Ref. da pessoa |
| `request_body` | `null` |
| `response_body` | o JSON devolvido pela API (inclusive o JSON de erro do 404); a mensagem da exceção se não houver resposta |
| `is_success` | `true` só quando a pessoa foi gravada |

Como no `GetTicketById`, a resposta inteira (nome, CPF/CNPJ, e-mails, telefones, endereços) vai para `response_body`.
Empresa não cadastrada não gera registro (nenhuma chamada à API).

---

## 6. Services

### 6.1 `MovideskService`: métodos em comum (decidido)
Um service novo do módulo, injetado nos outros, com o que hoje se repete entre `MovideskTicketsService` e
`MovideskPeopleService`:

| Método | O que faz | Usado por |
|---|---|---|
| `GetBatchTime()` | Horário do lote no fuso `E. South America Standard Time` (o mesmo valor para o registro e todas as filhas) | `GetTickets`, `GetTicketById`, `GetPeople`, `GetPersonById` |
| `SaveRequest(...)` | Grava uma chamada sob demanda em `movidesk.MovideskRequests`, com a chave como texto | `GetTicketById`, `GetPersonById` |

`Application.Movidesk/Abstractions/Services/IMovideskService.cs`
```csharp
using Domain.Movidesk.Enums;

namespace Application.Movidesk.Abstractions.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo (tickets, pessoas, serviços)
    /// </summary>
    public interface IMovideskService
    {
        /// <summary>
        /// horário do lote: o registro e todas as filhas gravam o mesmo integrated_or_updated_at (condição de exclusão do merge)
        /// </summary>
        public DateTime GetBatchTime();

        /// <summary>
        /// registra uma chamada sob demanda à API da Movidesk em movidesk.MovideskRequests (a rota nunca leva o token)
        /// </summary>
        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess);
    }
}
```

`Application.Movidesk/Services/MovideskService.cs`
```csharp
using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Enums;

namespace Application.Movidesk.Services
{
    public class MovideskService : IMovideskService
    {
        private readonly ILoggerService _logger;
        private readonly IMovideskRepository _movideskRepository;

        public MovideskService(ILoggerService logger, IMovideskRepository movideskRepository) =>
            (_logger, _movideskRepository) = (logger, movideskRepository);

        public DateTime GetBatchTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess) =>
            _movideskRepository.InsertIntoMovideskRequests(
                new Domain.Movidesk.Entities.Writes.Requests.Request(
                    execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: field_key_value,
                    request_body: null,
                    response_body: responseBody,
                    is_success: isSuccess
                )
            );
    }
}
```
- **Mesmo log de auditoria:** o `ILoggerService` é registrado como `Scoped` no Shared
  (`Infrastructure.Shared/DependencyInjection.cs`), então dentro de uma requisição o `MovideskService` recebe a
  **mesma instância** que o service que o chamou. O `GetExecutionGuid()` devolve a execução aberta pelo `GetTicketById` ou
  `GetPersonById`, e o registro em `MovideskRequests` continua ligado a ela.
- **`request_body` fixo em `null`:** as duas chamadas sob demanda são GET. Quando houver POST/PATCH (escrita na
  Movidesk), o método ganha um parâmetro `requestBody`, como o `SaveRequest` do AfterSale.
- **Injeção de dependência** (`Infrastructure.Movidesk/DependencyInjection.cs`):
  ```csharp
  services.AddScoped<IMovideskService, MovideskService>();
  ```
  O nome `IMovideskService` era o da interface antiga, renomeada para `IMovideskTicketsService`; não há conflito.

**Mudanças no `MovideskTicketsService`** (já implementado):
- O construtor recebe `IMovideskService movideskService` (campo `_movideskService`).
- O `SaveRequest` privado (com `long id`) **sai**; as duas chamadas no `GetTicketById` passam a ser
  `_movideskService.SaveRequest(EnumJob.MovideskGetTicketById, user_name, "GET", route, doc_company, id.ToString(), ..., ...)`.
- `GetTickets` e `GetTicketById` trocam o `TimeZoneInfo.ConvertTimeFromUtc(...)` por `_movideskService.GetBatchTime()`.

**Mudanças no `MovideskPeopleService`:** o construtor recebe `IMovideskService`; `GetPeople` usa
`_movideskService.GetBatchTime()`; o `GetPersonById` (6.3) usa os dois métodos. O `MovideskServicesService` recebe o
`IMovideskService` quando serviços forem implementados.

### 6.2 Extrair a gravação comum (`SavePeople`)
Como foi feito com `SaveTickets`: o trecho do `GetPeople` que valida, monta as entidades, grava, registra o JSON e
atualiza o cache vira um método privado, usado pelos dois.

```csharp
/// <summary>
/// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
/// Devolve a quantidade de pessoas gravadas.
/// </summary>
private async Task<int> SavePeople(
    string doc_company,
    DateTime batchTime,
    IEnumerable<Domain.Movidesk.Dtos.Reads.People.Person> people,
    IReadOnlyDictionary<string, string> peopleJson,
    Dictionary<string, Domain.Movidesk.Entities.Reads.People.Person>? peopleCache)
{
    var _listSomenteNovos = new List<Domain.Movidesk.Entities.Reads.People.Person>();

    // a mesma pessoa pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
    foreach (var person in people.Where(x => !String.IsNullOrEmpty(x.id)).GroupBy(x => x.id).Select(g => g.Last()))
    {
        var validations = _personValidator.Validate(person);

        if (validations.Errors.Count > 0)
        {
            var message = String.Empty;
            validations.Errors.ForEach(s => message += $"pessoa {person.id}: {s.ErrorMessage}\n");
            _logger.AddMessage(message);
        }

        _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.People.Person(doc_company, batchTime, person));
    }

    if (_listSomenteNovos.Count == 0)
        return 0;

    await _movideskPeopleRepository.BulkInsertIntoMovideskPeople(_listSomenteNovos, _logger.GetExecutionGuid());

    foreach (var record in _listSomenteNovos)
    {
        if (peopleJson.TryGetValue(record.id, out var json))
            _logger.AddRecord(record.id, json);

        if (peopleCache is not null)
            peopleCache[record.id] = record;
    }

    return _listSomenteNovos.Count;
}
```
No `GetPeople`, o trecho que hoje vai de `var _listSomenteNovos = ...` até `totalInseridos += _listSomenteNovos.Count;`
passa a ser:
```csharp
totalInseridos += await SavePeople(company.doc_company, batchTime, people, peopleJson, peopleCache);
```
O comportamento do `GetPeople` não muda; a linha do `batchTime` passa a ser `var batchTime = _movideskService.GetBatchTime();`.

### 6.3 `GetPersonById`
```csharp
/// <summary>
/// busca uma pessoa pelo Cód. Ref., grava nas tabelas de pessoas (sempre, sem consultar o cache)
/// e registra a chamada em movidesk.MovideskRequests
/// </summary>
public async Task<bool?> GetPersonById(string user_name, string doc_company, string id)
{
    _logger
        .Clear()
        .AddLog(EnumJob.MovideskGetPersonById);

    // rota registrada no log de requisições: sem o token
    var route = $"persons?id={id}";
    bool? result;

    try
    {
        var company = await _movideskRepository.GetCompany(doc_company);

        if (company is null)
        {
            // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetTicketById)
            _logger.AddMessage($"Empresa {doc_company} não cadastrada em movidesk.MovideskParameters.");
            return null;
        }

        // mesmo horário de lote para a pessoa e as filhas (condição de exclusão do merge)
        var batchTime = _movideskService.GetBatchTime();

        string? response;

        try
        {
            response = await _apiClient.GetAsync(
                route: "persons",
                queryParams: new Dictionary<string, string>
                {
                    { "token", company.Token.ToString() },
                    { "id", id }
                }
            );
        }
        catch (Exception ex)
        {
            // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
            _movideskService.SaveRequest(EnumJob.MovideskGetPersonById, user_name, "GET", route, doc_company, id, $"Exceção ao chamar a API: {ex.Message}", false);
            throw;
        }

        JObject? json = null;

        try
        {
            json = String.IsNullOrWhiteSpace(response) ? null : JToken.Parse(response) as JObject;
        }
        catch (Newtonsoft.Json.JsonException)
        {
            // json continua null: tratado abaixo como resposta inválida
        }

        if (String.IsNullOrWhiteSpace(response) || (json?["status"]?.Value<int?>() == 404 && json["id"] is null))
        {
            // a API responde 404 com um JSON de erro (application/problem+json) quando a pessoa não existe
            _logger.AddMessage($"Pessoa {id} não encontrada na Movidesk (empresa {doc_company}).");
            result = false;
        }
        else
        {
            var person = json?.ToObject<Domain.Movidesk.Dtos.Reads.People.Person>();

            if (person is null || !String.Equals(person.id, id, StringComparison.Ordinal))
            {
                _logger.AddMessage($"Resposta inválida da API Movidesk para a pessoa {id} (empresa {doc_company}): {response}");
                result = null;
            }
            else
            {
                // atualiza o cache do GetPeople, se ele estiver carregado, para a próxima execução não regravar a pessoa
                _cache.TryGetValue($"{PeopleKey}:{doc_company}", out Dictionary<string, Domain.Movidesk.Entities.Reads.People.Person>? peopleCache);

                await SavePeople(doc_company, batchTime, [person], new Dictionary<string, string> { { id, response! } }, peopleCache);

                _logger.AddMessage($"Concluída com sucesso: pessoa {id} inserida ou atualizada!");
                result = true;
            }
        }

        _movideskService.SaveRequest(EnumJob.MovideskGetPersonById, user_name, "GET", route, doc_company, id, response, result == true);

        return result;
    }
    finally
    {
        _logger.SetLogEndDate();
        await _logger.CommitAllChanges();
    }
}
```
Pontos do código:
- **Mesma estrutura do `GetTicketById`:** `try/finally` para o log de auditoria em todos os casos e registro em
  `MovideskRequests` também quando a chamada lança exceção.
- **Leitura em dois passos:** primeiro como `JObject`, para reconhecer o 404 pelo `status`; depois `ToObject<Person>()`.
  O `ToObject` usa o mesmo conversor do `JsonConvert.DeserializeObject`, então o DTO recebe os valores do mesmo jeito.
- **Conferência do `id` devolvido:** comparação exata (`Ordinal`). Se a Movidesk devolver o Cód. Ref. com outra caixa
  (`abc` pedido, `ABC` devolvido), a resposta vira "inválida"; nos dados reais os ids são numéricos, então isso não
  apareceu.
- **`SaveRequest` e `GetBatchTime` do `MovideskService` (6.1):** um só método para tickets e pessoas, com a chave como
  texto (o `GetTicketById` passa `id.ToString()`).
- `using`s a mais no `MovideskPeopleService`: nenhum; `Newtonsoft.Json.Linq` (para `JObject`/`JToken`) já está lá.

---

## 7. Controller

`src/Backend/Hosts/Hangfire/Controllers/General/MovideskController.cs`, depois do endpoint `People`:
```csharp
/// <summary>
/// get one person by Cód. Ref. (GET persons?id={id}) and save it; the call is logged in movidesk.MovideskRequests
/// </summary>
[HttpGet("People/{docCompany}/{id}")]
[Tags("MovideskPeople")]
public async Task<ActionResult> Person(
    [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
    [RegularExpression(@"^\d{14}$")] string docCompany,
    [StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")] string id
)
{
    var result = await _movideskPeopleService.GetPersonById(userName, docCompany, id);

    return result switch
    {
        true => Ok($"Person {id} integrated successfully."),
        false => NotFound($"Person {id} not found in Movidesk."),
        null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
    };
}
```
`People` e `People/{docCompany}/{id}` não conflitam (quantidade de segmentos), como em `Tickets`.

---

## 8. Riscos e limitações

- **Execução simultânea com o `GetPeople`:** as procedures de merge usam `sp_TryApplock`; se as duas rodarem o merge da
  mesma tabela ao mesmo tempo, o `GetPersonById` falha com 500 e a pessoa pode ficar só na `untreated` até o próximo merge.
  Reexecutar resolve.
- **Bloqueio da API:** três chamadas com erro seguidas (inclusive ids inexistentes, se o 404 contar como erro)
  bloqueiam o token por 60 s.
- **Pessoa apagada na Movidesk:** o `GetPersonById` devolve 404, mas **não apaga** a pessoa de `MovideskPeople`; o 404 só fica
  registrado no log. Apagar exigiria uma regra de exclusão que a modelagem deixou de fora (modelagem de pessoas, 4.8).
- **Dados pessoais no log:** `MovideskRequests.response_body` guarda a pessoa inteira.

---

## 9. Checklist

- [ ] `EnumJob.MovideskGetPersonById = 3003`
- [ ] Linha do job 3003 no `Script005` (e no banco local, à mão)
- [ ] `IMovideskService` + `MovideskService` (`GetBatchTime`, `SaveRequest`) e registro na injeção de dependência
- [ ] `MovideskTicketsService`: injetar `IMovideskService`, remover o `SaveRequest` privado, usar `GetBatchTime`
- [ ] `IMovideskPeopleService.GetPersonById(string user_name, string doc_company, string id)`
- [ ] `SavePeople` privado, usado pelo `GetPeople` (sem mudar o comportamento) e pelo `GetPersonById`
- [ ] `GetPersonById` com o 404 reconhecido pelo JSON de erro, usando `_movideskService.SaveRequest` e `GetBatchTime`
- [ ] Endpoint `GET People/{docCompany}/{id}` com `X-User-Name` e 200/404/422
- [ ] Compilar o host
- [ ] Teste: uma pessoa existente (grava e registra em `MovideskRequests`), uma inexistente (404 e registro com
      `is_success = 0`), um CNPJ não cadastrado (422, sem registro), sem `X-User-Name` (400) e um `id` com `-` ou `.` (400)
- [ ] Teste de regressão do `GetTicketById`: continua registrando em `MovideskRequests` pelo `MovideskService`

## 10. Decisões (aplicadas em 21/10/2026)

| # | Decisão | Onde |
|---|---|---|
| 1 | Rota `GET People/{docCompany}/{id}`, no formato do `GetTicketById` | 3.2, 7 |
| 2 | `id` só com letras e números (`^[A-Za-z0-9]+$`), como diz a documentação | 3.2, 7 |
| 3 | `SaveRequest` unificado num novo `MovideskService`, com os métodos em comum aos outros services (também o `GetBatchTime`) | 6.1 |

Não há decisões pendentes neste guia.
