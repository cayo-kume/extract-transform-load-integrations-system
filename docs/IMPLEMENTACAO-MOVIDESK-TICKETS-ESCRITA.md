# Movidesk — Escrita de tickets (`POST /tickets`, `PATCH /tickets` e `POST /ticketFileUpload`)

Proposta para implementar as três rotas de escrita da API de tickets: criar um ticket, alterar um ticket e enviar
anexos para uma ação. Segue o padrão já usado nas rotas de escrita do AfterSale (`SendReverseAction`, log em
`AfterSaleRequests`, cabeçalho `X-User-Name`) e nas leituras sob demanda da Movidesk (`GetTicketById`,
`movidesk.MovideskRequests`).

> **Implementado em 22/10/2026**, com as decisões da seção 10, e testado na API real com o roteiro da seção 9
> (ticket 69369, cancelado ao final). As rotas e regras da seção 1 vêm da
> [documentação de tickets](https://atendimento.movidesk.com/kb/article/256/movidesk-ticket-api), lida em 22/10/2026.
>
> **Atualização de 22/10/2026 (escrita de pessoas):** o `Result` saiu de `Dtos/Writes/Tickets` para `Dtos/Writes/Result.cs`
> (namespace `Domain.Movidesk.Dtos.Writes`), comum a tickets e pessoas, e o `ticketId` (`long?`) virou `id` (`string?`). As
> respostas das rotas de tickets passaram de `"ticketId": 69369` para `"id": "69369"`. As seções abaixo mostram o
> desenho original.
>
> Diferenças em relação ao texto abaixo, todas vindas do teste real:
> * **Sem `FileUploadResult.cs`:** a resposta do `ticketFileUpload` (`[{ "fileName", "path", "error" }]`) volta como texto em
>   `Result.response`, sem DTO próprio;
> * **Datas com `Z` no retorno do POST:** o `PostTicket` tira o sufixo antes de desserializar (seção 9.1), para o
>   ticket ficar igual ao que o `GetTickets` grava.

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`. Uma classe por arquivo `.cs`.

---

## 1. O que a API oferece

| Método | Rota | Parâmetros | Corpo | Retorno documentado |
|---|---|---|---|---|
| **POST** | `/tickets` | `token`, `returnAllProperties` (padrão `false`) | JSON (`application/json`) no layout do ticket | 200 e o id (número) do ticket criado |
| **PATCH** | `/tickets` | `token`, `id` | JSON só com o que muda | 200 |
| **POST** | `/ticketFileUpload` | `token`, `id` (ticket), `actionId` (ação) | Arquivos (`multipart/form-data`) | 200 e nome, hash e possíveis erros de cada anexo |

Não existe PUT: toda alteração é PATCH.

### 1.1 Campos obrigatórios na criação (`POST`)

| Objeto | Obrigatórios |
|---|---|
| `ticket` | `type` (1 = Interno, 2 = Público), `createdBy`, `clients`, `actions` |
| `ticket.actions[n]` | `type` (1 = Interna, 2 = Pública), `description` (interpretada como **HTML**) |
| `ticket.actions[n].timeAppointments[n]` | `activity`, `date` (com horas zeradas), `workTypeName`, `createdBy` |
| `ticket.actions[n].expenses[n]` | `type`, `createdBy`, `date` (UTC) |
| `ticket.customFieldValues[n]` | `customFieldId`, `customFieldRuleId`, `line`; `value` conforme o tipo do campo |
| `ticket.parentTickets[n]` / `childrenTickets[n]` | `id` |

### 1.2 Campos que podem ser escritos no ticket

`type`, `subject`, `category`, `urgency`, `status`, `justification`, `origin`, `owner`, `ownerTeam`, `createdBy`,
`serviceFirstLevelId`, `tags`, `resolvedIn`, `closedIn`, `resolvedInFirstCall`, `sequence`, `slaSolutionDate`,
`clients`, `actions`, `parentTickets`, `childrenTickets`, `customFieldValues` e `assets`.

Todos os outros (`id`, `protocol`, `baseStatus`, `lastUpdate`, `serviceFull`, os campos de SLA calculados, os
históricos etc.) são somente leitura e ficam fora dos DTOs de escrita.

**Campos que andam em par:**
* `status` precisa vir com a `justification` (e vice-versa, quando o status exige justificativa);
* `owner` precisa vir com o `ownerTeam`, e o `ownerTeam` com o `owner`;
* `category`, `urgency`, `status` e `justification` precisam ser nomes já cadastrados e relacionados ao tipo do ticket.

### 1.3 Regras do PATCH (o ponto mais perigoso)

* **Alteração parcial:** só os campos enviados mudam. Um campo ausente fica como está.
* **Listas sobrescrevem tudo:** `tags`, `clients`, `customFieldValues`, `assets`, `parentTickets` e `childrenTickets`
  enviados substituem a lista inteira. `{ "tags": [] }` apaga todas as tags.
* **Ações e apontamentos:** com `id`, o item é alterado; sem `id` (ou `id = 0`), é **inserido** como novo.
* **Clientes e apontamentos não enviados são excluídos** do ticket.

### 1.4 Limites que afetam a escrita

* **10 requisições por minuto** por token, as mesmas do `GetTickets`.
* **3 requisições com erro seguidas** na API de tickets bloqueiam novas chamadas por **60 segundos**. Esse bloqueio
  também derruba o `GetTickets`, por isso a proposta valida tudo o que der localmente antes de chamar a API (seção 5).
* **Anexos:** no máximo **25 MB por anexo**, segundo as
  [dúvidas frequentes sobre tickets](https://atendimento.movidesk.com/kb/pt-br/article/559662/duvidas-frequentes-sobre-tickets)
  (a página da API não traz limite de tamanho).

---

## 2. Visão geral da proposta

```
MovideskController (X-User-Name, docCompany, id)
   │  valida o formato da rota e o corpo (FluentValidation) → 400 sem chamar a API
   ▼
MovideskTicketsService.PostTicket / PatchTicketById / PostTicketFileUploadById
   │  fluxo comum (SendTicket): log do job, empresa, chamada, MovideskRequests
   ▼
IApiClient.SendAsync (novo: devolve status HTTP + corpo; aceita PATCH, query string e vários arquivos)
   ▼
sucesso → POST com returnAllProperties=true: se a resposta trouxer o ticket inteiro, grava com o SaveTickets
          PATCH e anexos: nada a gravar; o GetTickets traz a alteração na próxima execução (até 3 minutos)
```

| Método do service | Rota da Movidesk | Rota do controller | Job |
|---|---|---|---|
| `PostTicket` | `POST tickets` | `POST Tickets/{docCompany}` | 3006 `MovideskPostTicket` |
| `PatchTicketById` | `PATCH tickets?id={id}` | `PATCH Tickets/{docCompany}/{id}` | 3007 `MovideskPatchTicketById` |
| `PostTicketFileUploadById` | `POST ticketFileUpload?id={id}&actionId={actionId}` | `POST Tickets/{docCompany}/{id}/Actions/{actionId}/Files` | 3008 `MovideskPostTicketFileUploadById` |

Os três ficam no `MovideskTicketsService`, junto das leituras de tickets, como o AfterSale fez com reversas.

---

## 3. `IApiClient`: status HTTP, PATCH e vários arquivos

O `ApiClient` do Shared devolve só o corpo (`Task<string?>`). Para a escrita isso não basta:

* o PATCH da Movidesk responde **200 sem corpo** no sucesso, então não dá para saber se deu certo só pelo corpo;
* não existe método `PATCH`;
* `PostAsync` não aceita `queryParams`, e a Movidesk recebe o `token` e o `id` na query string;
* `PostMultipartAsync` envia **um** arquivo, e o `ticketFileUpload` aceita vários na mesma chamada (uma chamada só
  economiza requisições no limite de 10 por minuto).

**Proposta:** um método novo no `IApiClient`, sem mexer nos existentes (AfterSale, Linx e os outros continuam iguais):

`Domain.Shared/Abstractions/Api/ApiResponse.cs`
```csharp
namespace Domain.Shared.Abstractions.Api
{
    /// <summary>
    /// resposta de uma chamada em que o status HTTP importa (rotas de escrita)
    /// </summary>
    public record ApiResponse(int StatusCode, string? Content, IReadOnlyDictionary<string, string> Headers)
    {
        public bool IsSuccess => StatusCode is >= 200 and < 300;
    }
}
```

`Domain.Shared/Abstractions/Api/IApiClient.cs` (método novo)
```csharp
/// <summary>
/// envia a requisição e devolve status, corpo e cabeçalhos (ex.: retry-after). Aceita qualquer método (PATCH incluído).
/// </summary>
Task<ApiResponse> SendAsync(HttpMethod method, string route, HttpContent? content = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null);
```

O `HttpContent` deixa quem chama montar o corpo: `StringContent` com JSON para o POST e o PATCH, e
`MultipartFormDataContent` com vários `ByteArrayContent` para os anexos. O `ApiClient` reaproveita o `BuildUrl` e o
`CreateClient` que já existem.

---

## 4. DTOs de escrita (`Domain.Movidesk/Dtos/Writes/Tickets`)

Os DTOs de leitura (`Dtos/Reads/Tickets`) são todos `string` e trazem campos somente leitura, então não servem para a
escrita. Os de escrita têm:

* **só os campos que podem ser escritos** (seção 1.2), com os tipos reais (`int?`, `bool?`, `DateTime?`, `long?`);
* **tudo anulável**, inclusive as listas. A serialização ignora nulos (`NullValueHandling.Ignore`, como o
  `ToJsonBody` do AfterSale). Assim, no PATCH:
  * propriedade `null` = campo **não enviado** = não muda na Movidesk;
  * lista vazia `[]` = lista **apagada** na Movidesk;
  * lista com itens = lista **substituída**.

```
Dtos/Writes/Tickets/
├── Ticket.cs               (type, subject, category, urgency, status, justification, origin, owner, ownerTeam,
│                            createdBy, serviceFirstLevelId, tags, resolvedIn, closedIn, resolvedInFirstCall,
│                            sequence, slaSolutionDate, clients, actions, parentTickets, childrenTickets,
│                            customFieldValues, assets)
├── PersonReference.cs      (id: Cód. Ref. da pessoa; usado em createdBy, owner e nas ações)
├── Client.cs               (id, organization: OrganizationReference)
├── OrganizationReference.cs (id)
├── Action.cs               (id, type, origin, description, createdDate, createdBy, timeAppointments, expenses, tags)
├── TimeAppointment.cs      (id, activity, date, periodStart, periodEnd, workTime, workTypeName, createdBy, createdByTeam)
├── TeamReference.cs        (id)
├── Expense.cs              (type, createdBy, date, quantity, value)
├── TicketReference.cs      (id: tickets pai e filhos)
├── CustomFieldValue.cs     (customFieldId, customFieldRuleId, line, value, items)
├── CustomFieldValueItem.cs (personId, clientId, team, customFieldItem, storageFileGuid, fileName)
├── AssetReference.cs       (id)
├── (sem FileUploadResult.cs: a resposta do ticketFileUpload volta como texto em Result.response)
└── Result.cs               (resultado devolvido pelo service ao controller, seção 6.2)
```

Datas em UTC (`createdDate` das ações, `resolvedIn`, `closedIn`, `slaSolutionDate`, `expenses[].date`) são
serializadas em ISO 8601 (`yyyy-MM-ddTHH:mm:ss`), e o `date` dos apontamentos vai com as horas zeradas.

O mesmo `Ticket` serve para o POST e para o PATCH. O que muda é a validação (seção 5).

---

## 5. Validação antes de chamar a API

Cada chamada com erro conta para o bloqueio de 60 s (seção 1.4), então o controller valida o corpo e devolve
**400** sem chamar a Movidesk. Validadores em `Application.Movidesk/Validations/Writes/Tickets/`, um por arquivo:

| Validador | Regras |
|---|---|
| `PostTicketValidator` | `type` em {1, 2}; `createdBy.id` preenchido; `clients` com ao menos um item, cada um com `id`; `actions` com ao menos uma ação; nenhuma ação com `id` (na criação todas são novas) |
| `PatchTicketValidator` | ao menos um campo preenchido; `status` ↔ `justification` e `owner` ↔ `ownerTeam` juntos; `type`, se vier, em {1, 2} |
| `ActionValidator` | `type` em {1, 2}; `description` preenchida; `createdBy` obrigatório quando houver apontamentos |
| `TimeAppointmentValidator` | `activity`, `date` (sem horas), `workTypeName` e `createdBy` preenchidos |
| `ExpenseValidator` | `type`, `createdBy` e `date` preenchidos; `date` não pode ser futura |
| `CustomFieldValueValidator` | `customFieldId`, `customFieldRuleId` e `line` maiores que zero |
| Anexos (no controller) | ao menos um arquivo; nenhum vazio; nenhum acima de **25 MB** (limite da Movidesk por anexo) |

Ao contrário das leituras, aqui a validação **barra** a chamada: o corpo vem de outro sistema, não da Movidesk.

O que só a Movidesk sabe (nome de categoria, status ou justificativa cadastrados, Cód. Ref. de pessoa que existe) não
dá para validar localmente. Esses erros voltam da API como **422** para quem chamou.

---

## 6. Service: fluxo comum das três rotas

Um método privado `SendTicket`, usado pelas três rotas (como o `SendReverseAction` do AfterSale). Ele segue a
regra já combinada para o `MovideskTicketsService`: só vira método privado o que é usado por mais de um método
público, como o `SaveTickets`.

1. abre o log com o job da rota (`_logger.Clear().AddLog(job)`);
2. busca a empresa pelo CNPJ (`_movideskRepository.GetCompany`). Se não existir, devolve resultado sem sucesso sem
   chamar a API, como o `GetTicketById`;
3. monta o corpo e o `requestBody` que vai para o log (JSON sem nulos; nos anexos, só os metadados e o hash de cada
   arquivo, seção 6.3);
4. chama `_apiClient.SendAsync`. Se a chamada não chegar a ter resposta (timeout, rede), grava a tentativa em
   `MovideskRequests` e relança a exceção, como as leituras sob demanda;
5. grava a chamada em `movidesk.MovideskRequests` com `MovideskService.SaveRequest`:
   * usuário (`X-User-Name`), método (`POST`/`PATCH`), rota **sem o token**, CNPJ;
   * `field_key_value`: o número do ticket (no POST, o id devolvido pela API; vazio se a criação falhar);
   * `request_body` (corpo enviado), `response_body` e se houve sucesso pelo **status HTTP**;
6. no sucesso do POST, grava o ticket devolvido pela API, se ela devolver o ticket inteiro (seção 6.1);
7. fecha o log (`SetLogEndDate` + `CommitAllChanges`) num `finally`.

O `SaveRequest` hoje grava `request_body: null` fixo. Ele ganha o parâmetro `string? requestBody = null`, sem mudar as
chamadas das leituras.

### 6.1 Banco depois da escrita (sem releitura)

Nenhuma das três rotas lê o ticket de novo depois de escrever (decisão 1). A alteração chega ao banco pela próxima
execução do `GetTickets` (até 3 minutos), porque toda escrita muda o `lastUpdate` do ticket e a fase 1 o encontra.
Assim cada escrita custa uma requisição só.

A exceção é o POST, que vai com **`returnAllProperties=true`** (decisão 7):

* se a resposta trouxer o ticket inteiro, ele é desserializado no DTO de leitura (`Dtos/Reads/Tickets/Ticket`) e
  gravado com o `SaveTickets` que já existe, sem nenhuma requisição a mais;
* se a resposta trouxer só o número, nada é gravado e o `GetTickets` traz o ticket novo na próxima execução;
* se a gravação falhar, a criação continua valendo: vira mensagem no log e o `GetTickets` corrige depois.

O que o `returnAllProperties=true` devolve é o primeiro item a conferir na API real (seção 9).

### 6.2 Resultado devolvido ao controller

`Domain.Movidesk/Dtos/Writes/Tickets/Result.cs`
```csharp
public class Result
{
    public bool success { get; init; }
    public long? ticketId { get; init; }        // no POST, o número criado
    public int statusCode { get; init; }        // status devolvido pela Movidesk (0 quando nem chamou)
    public string? message { get; init; }       // mensagem da API ou do service
    public string? response { get; init; }      // corpo devolvido pela Movidesk (ex.: hashes dos anexos)
    public int? retryAfterSeconds { get; init; } // quando a Movidesk bloqueou por excesso de erros
}
```

### 6.3 Esboço

```csharp
public Task<Result> PostTicket(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket)
{
    var body = JsonConvert.SerializeObject(ticket, WriteSettings);

    return SendTicket(EnumJob.MovideskPostTicket, user_name, doc_company, null, HttpMethod.Post, "tickets",
        // com o ticket inteiro na resposta, ele é gravado sem releitura (seção 6.1)
        query: new() { { "returnAllProperties", "true" } },
        requestBody: body,
        content: () => new StringContent(body, Encoding.UTF8, "application/json"),
        // número criado: vem no corpo (formato a conferir: ticket inteiro, número puro ou { "id": ... })
        readTicketId: response => ParseCreatedId(response));
}

public Task<Result> PatchTicketById(string user_name, string doc_company, long id, Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket)
{
    var body = JsonConvert.SerializeObject(ticket, WriteSettings);

    return SendTicket(EnumJob.MovideskPatchTicketById, user_name, doc_company, id, HttpMethod.Patch, "tickets",
        query: new() { { "id", id.ToString() } },
        requestBody: body,
        content: () => new StringContent(body, Encoding.UTF8, "application/json"),
        readTicketId: _ => id);
}

public Task<Result> PostTicketFileUploadById(string user_name, string doc_company, long id, int actionId, IReadOnlyList<(string fileName, string contentType, byte[] content)> files)
{
    // no log vão só os metadados e o hash de cada arquivo (decisão 4): a Movidesk guarda o arquivo
    var requestBody = JsonConvert.SerializeObject(files.Select(x => new
    {
        x.fileName,
        x.contentType,
        size = x.content.Length,
        sha256 = Convert.ToHexString(SHA256.HashData(x.content)).ToLowerInvariant()
    }));

    return SendTicket(EnumJob.MovideskPostTicketFileUploadById, user_name, doc_company, id, HttpMethod.Post, "ticketFileUpload",
        query: new() { { "id", id.ToString() }, { "actionId", actionId.ToString() } },
        requestBody: requestBody,
        content: () =>
        {
            var multipart = new MultipartFormDataContent();

            foreach (var (fileName, contentType, content) in files)
            {
                var file = new ByteArrayContent(content);
                file.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                multipart.Add(file, "file", fileName);
            }

            return multipart;
        },
        readTicketId: _ => id);
}

// null nos campos não enviados: no PATCH, campo ausente = campo que não muda
private static readonly JsonSerializerSettings WriteSettings = new()
{
    NullValueHandling = NullValueHandling.Ignore,
    DateFormatString = "yyyy-MM-ddTHH:mm:ss"
};
```

* `ParseCreatedId` é usado só pelo `PostTicket`, então fica dentro dele (regra da seção 6); aparece separado aqui só
  para o esboço ficar legível. Ele também decide se a resposta é o ticket inteiro, para o `SendTicket` gravar com
  o `SaveTickets` (seção 6.1).
* No `PostTicket`, a chave do log (`field_key_value`) é o número devolvido pela API; o PATCH e os anexos usam o `id` da
  rota.

---

## 7. Controller

Rotas novas no `MovideskController`, todas com o cabeçalho **`X-User-Name`** (até 100 caracteres) e o CNPJ na rota
(`^\d{14}$`), como as leituras sob demanda:

```csharp
[HttpPost("Tickets/{docCompany}")]
[Tags("MovideskTickets")]
public async Task<ActionResult> PostTicket(
    [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
    [RegularExpression(@"^\d{14}$")] string docCompany,
    [FromBody] Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket
)

[HttpPatch("Tickets/{docCompany}/{id:long:min(1)}")]
[Tags("MovideskTickets")]
public async Task<ActionResult> PatchTicket(... long id, [FromBody] Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket)

[HttpPost("Tickets/{docCompany}/{id:long:min(1)}/Actions/{actionId:int:min(1)}/Files")]
[Tags("MovideskTickets")]
public async Task<ActionResult> PostTicketFiles(... long id, int actionId, [Required] List<IFormFile> files)
```

| Resultado | Resposta |
|---|---|
| Corpo ou rota inválidos (validadores, seção 5) | **400**, sem chamar a API |
| Escrita aceita | **200** (`POST` devolve o `ticketId` criado; anexos devolvem o retorno da Movidesk) |
| Empresa não cadastrada, ou a Movidesk recusou (categoria inexistente, ticket não encontrado etc.) | **422** com a mensagem da API |
| Movidesk bloqueou por excesso de erros (429) | **429** com `Retry-After` |

Na rota de anexos, a resposta **400** também cobre arquivo acima de 25 MB (seção 8).

---

## 8. Jobs, scripts e configuração

* **`EnumJob`:** `MovideskPostTicket = 3006`, `MovideskPatchTicketById = 3007`, `MovideskPostTicketFileUploadById = 3008`.
* **`Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql`** recebe as três linhas novas no bloco das rotas sob
  demanda (decisão 8). O comentário de faixas do script passa a citar os jobs de escrita.

```sql
/*Sob demanda - executadas pelo MovideskController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(30, 3002, 'MovideskGetTicketById', ...),
(30, 3003, 'MovideskGetPersonById', ...),
(30, 3005, 'MovideskGetServiceById', ...),
(30, 3006, 'MovideskPostTicket', 'Criação de um ticket (POST Tickets/{docCompany})', 0, 2),
(30, 3007, 'MovideskPatchTicketById', 'Alteração de um ticket por número (PATCH Tickets/{docCompany}/{id})', 0, 2),
(30, 3008, 'MovideskPostTicketFileUploadById', 'Envio de anexos para uma ação (POST Tickets/{docCompany}/{id}/Actions/{actionId}/Files)', 0, 2);
```

  O DbUp não executa de novo um script que já rodou: as linhas entram nos bancos criados a partir de agora. Num banco
  que já rodou o `Script005`, os três jobs precisam ser inseridos à mão.
* **Tabelas:** nenhuma nova. O log das escritas vai para `movidesk.MovideskRequests`, que já tem `request_body`.
* **Tamanho dos anexos (decisão 5):** o limite é o da Movidesk, **25 MB por anexo**.
  * O controller recusa com **400** qualquer arquivo acima de 25 MB (26.214.400 bytes), sem chamar a API.
  * Como a rota aceita vários arquivos numa requisição, o limite da requisição inteira sobe acima dos 30 MB padrão do
    Kestrel, com `[RequestSizeLimit]` e `[RequestFormLimits(MultipartBodyLengthLimit = ...)]` na rota de anexos. O valor
    fica como uma constante ao lado do limite por arquivo.

---

## 9. Teste na API real (decisão 6)

Os testes criam e alteram um ticket de verdade na conta da empresa, então seguem um roteiro fechado:

1. **Criar** um ticket **interno** (`type = 1`) com o assunto **"Teste - Cayo Kume"**, pelo `POST Tickets/{docCompany}`;
2. **Alterar** o ticket criado pelo `PATCH Tickets/{docCompany}/{id}` (por exemplo, o assunto ou uma tag);
3. **Anexar** um arquivo pequeno a uma ação do ticket pelo `POST Tickets/{docCompany}/{id}/Actions/{actionId}/Files`;
4. **Cancelar** o ticket no final, com um PATCH que muda o `status` para cancelado (com a justificativa, se o status
   exigir).

Durante o roteiro, conferir e registrar no documento:

* **Retorno do POST com `returnAllProperties=true`:** o ticket inteiro, o número puro ou `{ "id": ... }`? Define se a
  criação grava direto no banco (seção 6.1).
* **Retorno do PATCH:** 200 sem corpo, como a documentação indica?
* **Resposta do `ticketFileUpload`:** formato da lista de anexos com os hashes, e se o campo do arquivo no multipart
  precisa ter um nome específico.
* **Erros**, só se aparecerem naturalmente no roteiro: status e corpo de uma recusa da Movidesk. Não provocar erros de
  propósito, porque 3 erros seguidos bloqueiam a API de tickets por 60 s (seção 1.4).

---

### 9.1 Resultado do teste (22/10/2026)

Roteiro executado pelas rotas novas do host, com o cadastro **Cayo Kume** (Cód. Ref. `1641082435`) como criador e
cliente, na empresa `38367316000199`:

| Passo | Chamada | Resultado |
|---|---|---|
| Criar | `POST Tickets/38367316000199` | 200 em 2,9 s; ticket **69369**, interno, status "Novo" |
| Alterar | `PATCH Tickets/38367316000199/69369` (assunto) | 200 em 1,5 s, corpo vazio |
| Anexar | `POST .../69369/Actions/1/Files` (arquivo de 56 bytes) | 200 em 0,7 s |
| Cancelar | `PATCH .../69369` (assunto original, `status` "Cancelado", `justification` vazia) | 200 em 0,6 s, corpo vazio |

Respostas às perguntas da seção 9:

* **POST com `returnAllProperties=true`:** devolve o **ticket inteiro**, que foi gravado na hora. Só que as datas vêm em
  UTC **com `Z`** (`2026-10-22T15:58:42.83Z`), enquanto a listagem e o GET por id devolvem UTC sem sufixo. Com o `Z`, a
  conversão passou as datas para o horário local (12:58 em vez de 15:58). Corrigido: o `PostTicket` tira o `Z` antes de
  desserializar, e o `GetTickets` seguinte confirmou os valores em UTC.
* **PATCH:** 200 com corpo vazio, como a documentação indica. `justification` vazia foi aceita junto do status
  "Cancelado".
* **`ticketFileUpload`:** o campo `file` no multipart funciona. A resposta é uma lista
  `[{ "fileName": "teste-integracao.txt", "path": "C2C67E8C...", "error": null }]`, e o `path` é o mesmo hash que o
  `GetTickets` grava em `MovideskTicketsActionsAttachments`.
* **Erros:** nenhum aconteceu no roteiro, então o formato das recusas da Movidesk continua sem verificação. O service
  usa o `message` do JSON quando existir e, senão, o corpo como veio.

Depois do roteiro, o `GetTickets` trouxe o ticket cancelado com o assunto original e o anexo, e os 4 registros ficaram
em `movidesk.MovideskRequests` (jobs 3006, 3007, 3008 e 3007) com o corpo enviado. No anexo, o log tem só nome, tipo,
tamanho e SHA-256.

---

## 10. Decisões (22/10/2026)

| # | Pergunta | Decisão |
|---|---|---|
| 1 | Reler o ticket depois da escrita (6.1)? | **Não.** A alteração chega pela próxima execução do `GetTickets` (até 3 minutos) |
| 2 | Listas no PATCH (1.3) | **Repassar como veio.** Quem chama manda a lista inteira; no Swagger fica documentado que listas substituem tudo |
| 3 | `ApiClient` (seção 3) | **Criar o `SendAsync` genérico no Shared** |
| 4 | Anexos no log (`request_body`) | **Só os metadados e o hash** (nome, tipo, tamanho e SHA-256 de cada arquivo) |
| 5 | Tamanho máximo dos anexos | **O limite da documentação oficial: 25 MB por anexo** |
| 6 | Como testar na API real | **Ticket interno (`type = 1`) com o assunto "Teste - Cayo Kume"**: criar, alterar, anexar um arquivo pequeno e, ao final, mudar o status para cancelado (seção 9) |
| 7 | `returnAllProperties` no POST | **`true`.** Se a resposta trouxer o ticket inteiro, grava direto sem releitura (seção 6.1) |
| 8 | Script dos jobs | **Linhas novas no `Script005`**, sem script novo (seção 8) |
| 9 | Nome do resultado do service | **`Result`** (`Dtos/Writes/Tickets/Result.cs`), no lugar de `WriteResult` |
