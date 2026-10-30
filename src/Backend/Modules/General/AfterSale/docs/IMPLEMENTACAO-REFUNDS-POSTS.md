# Implementação — POSTs de reembolsos (approve, cancel, pay, change-total-amount)

Guia para aplicação **manual**. Nada foi alterado no código.

Fonte: especificação OpenAPI 1.0.8 (`api.swaggerhub.com/apis/send42/public-api/1.0.8`).

O padrão de referência são as rotas de escrita de reversas, já implementadas em
`AfterSaleReversesService` (`PostApproveReverseById`, `PutReciveProductsById` etc.):

- endpoint no `AfterSaleController` com o CNPJ na rota e o cabeçalho obrigatório `X-User-Name`;
- DTOs em `Dtos/Writes/<recurso>/<rota>/{Request,Response}`, com `Root` em arquivo próprio;
- um fluxo comum (`SendReverseAction`) que pega o token da empresa, chama a API, grava a chamada em
  `aftersale.AfterSaleRequests` (usuário, rota, request e response), grava um erro em
  `AfterSaleErrors` quando a API não devolve sucesso e registra o log da auditoria;
- resposta 200 quando `success = true`, 422 quando não.

| Método | Rota da API | Corpo | Job (`EnumJob`) | Endpoint nosso |
|---|---|---|---|---|
| `PostApproveRefundById` | `POST v3/api/refunds/{id}/approve` | nenhum | `AfterSaleApproveRefund = 5017` | `POST Refunds/{docCompany}/{id}/Approve` |
| `PostCancelRefundById` | `POST v3/api/refunds/{id}/cancel` | nenhum | `AfterSaleCancelRefund = 5018` | `POST Refunds/{docCompany}/{id}/Cancel` |
| `PostPayRefundById` | `POST v3/api/refunds/{id}/pay` | `multipart/form-data` (comprovante obrigatório no nosso endpoint) | `AfterSalePayRefund = 5019` | `POST Refunds/{docCompany}/{id}/Pay` |
| `PostChangeTotalAmountRefundById` | `POST v3/api/refunds/{id}/change-total-amount` | JSON | `AfterSaleChangeTotalAmountRefund = 5020` | `POST Refunds/{docCompany}/{id}/ChangeTotalAmount` |

Os quatro jobs já estão no bloco de ações do `Script007` (`IntervalTimeInMinutes = 0`, `IdType = 2`)
e os valores do `EnumJob` já existem. A tabela `aftersale.AfterSaleRequests` (`Script009`) já serve
para reembolsos: `field_key_value` recebe o id do reembolso. **Nada a inserir em banco.**

---

## 1. As rotas na documentação 1.0.8

Todas: `Authorization: Bearer <token>`, `Accept` e `Content-Type`, `id` do reembolso na rota.
A resposta 200 é a mesma nas quatro:

```json
{ "success": true, "error": "string", "message": "string" }
```

(`success` é o único campo obrigatório; erro documentado só `401`.)

| Rota | Corpo da requisição |
|---|---|
| `approve` | nenhum |
| `cancel` | nenhum |
| `pay` | `multipart/form-data`, todos opcionais: `voucher_redemption_code` (string), `observation` (string), `file` (arquivo, "comprovante de pagamento") |
| `change-total-amount` | `application/json`: `total_amount` (number, ex.: `1234.56`) |

> O resumo das rotas `approve` e `cancel` na doc diz "reverse", mas o caminho e o parâmetro são de
> reembolso ("Number of refund").

---

## 2. Decisões

| # | Ponto | Decisão |
|---|---|---|
| 1 | `ResponseRoot` hoje está em `Dtos/Writes/Reverses/Base` | **Decidido:** mover para `Dtos/Writes/Base` (comum a reversas e reembolsos), atualizando 7 referências (passo 1). |
| 2 | `PostMultipartAsync` exige arquivo, mas no `pay` da API o arquivo é opcional | **Decidido: opção C** — o comprovante passa a ser **obrigatório** no nosso endpoint `Pay`, e o `ApiClient` (Shared) não muda. Ver 2.1. |
| 3 | O que gravar em `request_body` no `pay` quando há arquivo | **Decidido:** gravar o arquivo **inteiro**, como no `InvoiceXml`. Como o comprovante pode não ser XML (PDF, imagem), o conteúdo vai como texto quando o arquivo é XML/texto e em base64 quando é binário (passo 5). |
| 4 | Corpo enviado no `approve`/`cancel` | **Decidido:** `{}` (JSON vazio). |

### 2.1 Sobre o `ApiClient` (decisão 2)

Sim, o `ApiClient` é do Shared:

- a interface `IApiClient` fica em `src/Backend/Shared/Domain.Shared/Abstractions/Api/IApiClient.cs`;
- a implementação `ApiClient` fica em `src/Backend/Shared/Infrastructure.Shared/Api/ApiClient.cs`;
- **9 módulos** criam uma instância dela no próprio `DependencyInjection.cs` (AfterSale, Movidesk,
  Oto, LinxCommerce, LinxMicrovix, FlashCourier, Jadlog, Stone e Totvs), cada um com o seu nome de
  `HttpClient` (o AfterSale usa `"AfterSaleAPI"`).

O método `PostMultipartAsync` em si foi adicionado **nesta sessão**, para o `InvoiceXml` de
reversas, e ainda não foi commitado. Hoje só o AfterSale o usa. Ou seja, o Shared já está alterado
no seu working copy por causa do módulo.

Opções avaliadas para o arquivo opcional do `pay`:

| Opção | Como | Resultado |
|---|---|---|
| A. Ajustar no Shared | Tornar os parâmetros do arquivo opcionais no `PostMultipartAsync` | Descartada: mexe no Shared |
| B. Manter no módulo | Cliente HTTP próprio em `Infrastructure.AfterSale` | Descartada: duplica o `ApiClient` inteiro |
| **C. Exigir o arquivo no `pay`** | Manter o `PostMultipartAsync` como está e tornar o comprovante obrigatório no nosso endpoint | **Escolhida** |

Consequências da opção C:

- nenhuma alteração no `IApiClient`/`ApiClient` (o passo 2 deixa de existir);
- o endpoint `Pay` devolve **400** quando o arquivo não vem, antes de chamar a API;
- o nosso endpoint fica mais restritivo que a API: um pagamento sem comprovante não pode ser
  registrado por aqui. Se isso for necessário no futuro, a saída é a opção A ou B.

---

## 3. Passo a passo

### Passo 1 — Mover `ResponseRoot` para `Dtos/Writes/Base`

1. Mover o arquivo
   `Domain.AfterSale/Dtos/Writes/Reverses/Base/ResponseRoot.cs` →
   `Domain.AfterSale/Dtos/Writes/Base/ResponseRoot.cs`.
2. No arquivo movido, linha 1:
   `namespace Domain.AfterSale.Dtos.Writes.Reverses.Base` → `namespace Domain.AfterSale.Dtos.Writes.Base`.
   Na descrição da classe, trocar "reverse write routes" por "write routes".
3. Nos 5 `Response/Root.cs` de reversas, linha 1:
   `using Domain.AfterSale.Dtos.Writes.Reverses.Base;` → `using Domain.AfterSale.Dtos.Writes.Base;`
   - `Dtos/Writes/Reverses/AdministrativeRecords/Response/Root.cs`
   - `Dtos/Writes/Reverses/Approve/Response/Root.cs`
   - `Dtos/Writes/Reverses/Invoice/Response/Root.cs`
   - `Dtos/Writes/Reverses/ReceiveProducts/Response/Root.cs`
   - `Dtos/Writes/Reverses/ReturnedInvoice/Response/Root.cs`
4. `Application.AfterSale/Services/AfterSaleReversesService.cs`, linha 783:
   `where TResponse : Domain.AfterSale.Dtos.Writes.Reverses.Base.ResponseRoot, new()` →
   `where TResponse : Domain.AfterSale.Dtos.Writes.Base.ResponseRoot, new()`
5. `Hosts/Hangfire/Controllers/General/AfterSaleController.cs`, linha 243:
   `where TResponse : Domain.AfterSale.Dtos.Writes.Reverses.Base.ResponseRoot` →
   `where TResponse : Domain.AfterSale.Dtos.Writes.Base.ResponseRoot`

### Passo 2 — `ApiClient`: nenhuma alteração

Com a opção C (decisão 2), o `PostMultipartAsync` continua exigindo o arquivo, que agora é
obrigatório no nosso endpoint. Nada a mudar em `IApiClient` nem em `ApiClient` (Shared).

### Passo 3 — DTOs em `Dtos/Writes/Refunds`

Pasta: `Domain.AfterSale/Dtos/Writes/Refunds/`. Mesmo formato dos DTOs de escrita de reversas.

```
Dtos/Writes/Refunds/
├── Approve/Response/Root.cs
├── Cancel/Response/Root.cs
├── Pay/
│   ├── Request/Root.cs
│   └── Response/Root.cs
└── ChangeTotalAmount/
    ├── Request/Root.cs
    └── Response/Root.cs
```

**3.1 Respostas** — as quatro iguais, mudando só o namespace e o comentário. Exemplo do `approve`
(`Approve/Response/Root.cs`):

```csharp
using Domain.AfterSale.Dtos.Writes.Base;

namespace Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response
{
    /// <summary>
    /// response of POST v3/api/refunds/{id}/approve
    /// </summary>
    public class Root : ResponseRoot
    {
    }
}
```

| Arquivo | Namespace | Comentário |
|---|---|---|
| `Approve/Response/Root.cs` | `...Writes.Refunds.Approve.Response` | `POST v3/api/refunds/{id}/approve` |
| `Cancel/Response/Root.cs` | `...Writes.Refunds.Cancel.Response` | `POST v3/api/refunds/{id}/cancel` |
| `Pay/Response/Root.cs` | `...Writes.Refunds.Pay.Response` | `POST v3/api/refunds/{id}/pay` |
| `ChangeTotalAmount/Response/Root.cs` | `...Writes.Refunds.ChangeTotalAmount.Response` | `POST v3/api/refunds/{id}/change-total-amount` |

> `success`, `error` e `message` já vêm do `ResponseRoot`, então as classes ficam vazias.

**3.2 `Pay/Request/Root.cs`** — campos de texto do formulário (o arquivo vem à parte, como
`IFormFile`, no controller):

```csharp
namespace Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request
{
    /// <summary>
    /// text fields of POST v3/api/refunds/{id}/pay (multipart/form-data). the payment attachment is sent apart, as a required file.
    /// </summary>
    public class Root
    {
        public string? voucher_redemption_code { get; init; }
        public string? observation { get; init; }
    }
}
```

**3.3 `ChangeTotalAmount/Request/Root.cs`:**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request
{
    /// <summary>
    /// body of POST v3/api/refunds/{id}/change-total-amount
    /// </summary>
    public class Root
    {
        //limites lidos com ponto decimal independente da cultura da máquina (pt-BR usa vírgula)
        [Required, Range(typeof(decimal), "0", "99999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public decimal? total_amount { get; init; }
    }
}
```

> O `[Required]` e o `[Range]` fazem o controller devolver 400 antes de chamar a API quando o valor
> não vem ou é negativo. O limite segue o `decimal(10,2)` das colunas de valor dos reembolsos.

### Passo 4 — Interface do service

[`Application.AfterSale/Abstractions/Services/IAfterSaleRefundsService.cs`](../Application.AfterSale/Abstractions/Services/IAfterSaleRefundsService.cs),
linhas 15–18 → substituir por:

```csharp
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root> PostApproveRefundById(string user_name, string doc_company, long id);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root> PostCancelRefundById(string user_name, string doc_company, long id);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root> PostPayRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request.Root request, byte[] file, string fileName, string fileContentType);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root> PostChangeTotalAmountRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request.Root request);
```

### Passo 5 — Service

[`Application.AfterSale/Services/AfterSaleRefundsService.cs`](../Application.AfterSale/Services/AfterSaleRefundsService.cs)
— substituir os quatro stubs (linhas 619–651, do `/// <summary> cancel reverse` até o fim do
`PostApproveRefundById`) por:

```csharp
        /// <summary>
        /// approve refund by ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root> PostApproveRefundById(string user_name, string doc_company, long id)
        {
            var body = new JObject();
            var route = $"v3/api/refunds/{id}/approve";

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root>(EnumJob.AfterSaleApproveRefund, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// cancel refund by ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root> PostCancelRefundById(string user_name, string doc_company, long id)
        {
            var body = new JObject();
            var route = $"v3/api/refunds/{id}/cancel";

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root>(EnumJob.AfterSaleCancelRefund, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// pay refund by ID (multipart/form-data), with the payment attachment (required in our endpoint).
        /// the whole file is logged in the request body: as text when it is XML/text, as base64 when it is binary (PDF, image).
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root> PostPayRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request.Root request, byte[] file, string fileName, string fileContentType)
        {
            var route = $"v3/api/refunds/{id}/pay";

            var fields = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(request.voucher_redemption_code))
                fields["voucher_redemption_code"] = request.voucher_redemption_code;

            if (!string.IsNullOrEmpty(request.observation))
                fields["observation"] = request.observation;

            //arquivo gravado inteiro, como no InvoiceXml: texto quando é XML/texto, base64 quando é binário
            var isText = fileContentType.Contains("xml", StringComparison.OrdinalIgnoreCase) ||
                fileContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);

            var requestLog = JObject.FromObject(fields);
            requestLog["file"] = new JObject
            {
                ["name"] = fileName,
                ["content_type"] = fileContentType,
                ["size"] = file.Length,
                ["encoding"] = isText ? "text" : "base64",
                ["content"] = isText ? Encoding.UTF8.GetString(file) : Convert.ToBase64String(file)
            };

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root>(EnumJob.AfterSalePayRefund, user_name, doc_company, id, "POST", route, requestLog.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostMultipartAsync(
                    route: route,
                    fields: fields,
                    fileField: "file",
                    fileContent: file,
                    fileName: fileName,
                    fileContentType: string.IsNullOrEmpty(fileContentType) ? "application/octet-stream" : fileContentType,
                    headers: headers
                ));
        }

        /// <summary>
        /// change total amount of refund by ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root> PostChangeTotalAmountRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request.Root request)
        {
            var body = ToJsonBody(request);
            var route = $"v3/api/refunds/{id}/change-total-amount";

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root>(EnumJob.AfterSaleChangeTotalAmountRefund, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// serializes the request ignoring null properties, so the API does not receive fields the caller did not fill
        /// </summary>
        private static JObject ToJsonBody(object request) =>
            JObject.FromObject(request, Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            }));

        /// <summary>
        /// common flow of the refund write routes: resolves the company token, calls the API,
        /// logs the call in aftersale.AfterSaleRequests (user, request and response) and saves an
        /// AfterSaleErrors record when the API does not return success
        /// </summary>
        private async Task<TResponse> SendRefundAction<TResponse>(EnumJob job, string user_name, string doc_company, long id, string httpMethod, string route, string requestBody, Func<Dictionary<string, string>, Task<string?>> send)
            where TResponse : Domain.AfterSale.Dtos.Writes.Base.ResponseRoot, new()
        {
            _logger
               .Clear()
               .AddLog(job);

            TResponse result;
            string? response = null;

            try
            {
                var company = (await _afterSaleRepository.GetCompanys()).FirstOrDefault(x => x?.doc_company == doc_company);

                if (company is null)
                {
                    result = new TResponse { success = false, message = $"CNPJ {doc_company} não encontrado em AfterSaleParameters." };
                    _logger.AddMessage($"Reembolso {id}: {result.message}");

                    return result;
                }

                try
                {
                    response = await send(new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } });
                }
                catch (Exception ex)
                {
                    //registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    SaveRequest(job, user_name, httpMethod, route, doc_company, id, requestBody, $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                JToken? responseJson = null;

                try
                {
                    responseJson = String.IsNullOrEmpty(response) ? null : JToken.Parse(response);
                }
                catch (Newtonsoft.Json.JsonReaderException) { }

                result = responseJson is JObject responseObject
                    ? responseObject.ToObject<TResponse>() ?? new TResponse()
                    : new TResponse
                    {
                        success = false,
                        message = "Resposta inválida da API.",
                        error = response?.Length > 800 ? response.Substring(0, 800) : response
                    };

                SaveRequest(job, user_name, httpMethod, route, doc_company, id, requestBody, response, result.success);

                if (result.success)
                {
                    _logger.AddMessage($"Reembolso {id}: concluído com sucesso. {result.message}".Trim());
                }
                else
                {
                    _logger.AddMessage($"Reembolso {id}: {result.message_translated ?? result.message ?? result.error ?? "a API não retornou sucesso."}");

                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors(
                        [
                            new Domain.AfterSale.Entities.Errors.Error(
                                company.doc_company,
                                job,
                                _logger.GetExecutionGuid(),
                                id,
                                result.error,
                                result.error_code,
                                result.message,
                                result.message_translated
                            )
                        ],
                        _logger.GetExecutionGuid()
                    );
                }

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        private void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, long id, string requestBody, string? responseBody, bool isSuccess) =>
            _afterSaleRepository.InsertIntoAfterSaleRequests(
                new Domain.AfterSale.Entities.Writes.Requests.Request(
                    execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: id.ToString(),
                    request_body: requestBody,
                    response_body: responseBody,
                    is_success: isSuccess
                )
            );
```

Diferenças em relação ao `SendReverseAction` (de resto, linha a linha igual):

| Trecho | Reversas | Reembolsos |
|---|---|---|
| Nome | `SendReverseAction` | `SendRefundAction` |
| Mensagens do log | `Reversa {id}: ...` / "concluída" | `Reembolso {id}: ...` / "concluído" |
| Erros | `_afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors` | `_afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors` (já existe, criado no `GetRefunds`) |
| Restrição genérica | `Writes.Base.ResponseRoot` (após o passo 1) | igual |

> **Dependências:** nada novo no construtor. O service de reembolsos já recebe `_apiClient`,
> `_logger`, `_afterSaleRepository` (com `GetCompanys` e `InsertIntoAfterSaleRequests`) e
> `_afterSaleRefundsRepository`. O `using Newtonsoft.Json.Linq;` também já está no arquivo.

### Passo 6 — Endpoints

[`Hosts/Hangfire/Controllers/General/AfterSaleController.cs`](../../../../Hosts/Hangfire/Controllers/General/AfterSaleController.cs)
— antes do `ToActionResult` (linha 243):

```csharp
        /// <summary>
        /// approve refund by ID (POST v3/api/refunds/{id}/approve)
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/Approve")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root>> ApproveRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id
        )
        {
            var result = await _afterSaleRefundsService.PostApproveRefundById(userName, docCompany, id);

            return ToActionResult(result);
        }

        /// <summary>
        /// cancel refund by ID (POST v3/api/refunds/{id}/cancel)
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/Cancel")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root>> CancelRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id
        )
        {
            var result = await _afterSaleRefundsService.PostCancelRefundById(userName, docCompany, id);

            return ToActionResult(result);
        }

        /// <summary>
        /// pay refund by ID, with the payment attachment (POST v3/api/refunds/{id}/pay, multipart). the file is required here.
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/Pay")]
        [Tags("AfterSaleRefunds")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root>> PayRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromForm] Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request.Root request,
            [Required] IFormFile file
        )
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var result = await _afterSaleRefundsService.PostPayRefundById(userName, docCompany, id, request, stream.ToArray(), file.FileName, file.ContentType);

            return ToActionResult(result);
        }

        /// <summary>
        /// change total amount of refund by ID (POST v3/api/refunds/{id}/change-total-amount)
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/ChangeTotalAmount")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root>> ChangeTotalAmountRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request.Root request
        )
        {
            var result = await _afterSaleRefundsService.PostChangeTotalAmountRefundById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

```

> O `ToActionResult` (200 com sucesso, 422 sem sucesso) já existe e serve para essas rotas depois do
> passo 1. O Swagger já usa o nome completo das classes como id de schema, então os vários `Root`
> não conflitam.

---

## 4. Checklist

- [ ] 1. `ResponseRoot` movido para `Dtos/Writes/Base` e 7 referências atualizadas
- [ ] 2. `ApiClient`: nada a fazer (opção C — comprovante obrigatório no `Pay`)
- [ ] 3. DTOs em `Dtos/Writes/Refunds` (4 respostas + 2 requests)
- [ ] 4. `IAfterSaleRefundsService`: 4 assinaturas
- [ ] 5. `AfterSaleRefundsService`: 4 métodos + `ToJsonBody`, `SendRefundAction` e `SaveRequest`
- [ ] 6. `AfterSaleController`: 4 endpoints
- [ ] Compilar e conferir no Swagger as 4 rotas com o cabeçalho `X-User-Name`
- [ ] Testar uma rota sem o cabeçalho (deve voltar 400)
- [ ] Testar o `Pay` sem arquivo (deve voltar 400)

## 5. Pontos de atenção

- **Ações irreversíveis:** `approve`, `cancel` e principalmente `pay` mudam o estado do reembolso na
  AfterSale (o `pay` registra o pagamento ao cliente). Vale testar primeiro com um reembolso de
  teste, se a AfterSale oferecer ambiente de homologação.
- **`total_amount`:** o Newtonsoft serializa `decimal` com ponto (`1234.56`), que é o formato da
  doc. Um valor inteiro sai como `100.0`; se a API recusar, é o mesmo ponto já anotado para o
  `nf_amount` das reversas.
- **Arquivo do `pay`:** a doc não diz quais tipos são aceitos nem o tamanho máximo. Como o arquivo é
  gravado inteiro em `request_body`, um comprovante binário (PDF, imagem) ocupa cerca de 4/3 do
  tamanho original em base64; XML e texto vão sem conversão. Cabe no `nvarchar(max)`, mas vale
  acompanhar o tamanho da tabela `AfterSaleRequests`.
- **Duplicação de `ToJsonBody`/`SendRefundAction`/`SaveRequest`:** ficam iguais aos de reversas,
  seguindo o padrão do módulo (cada service com o seu fluxo). Se um dia valer extrair para uma
  classe comum, dá para fazer de uma vez para os dois services.
