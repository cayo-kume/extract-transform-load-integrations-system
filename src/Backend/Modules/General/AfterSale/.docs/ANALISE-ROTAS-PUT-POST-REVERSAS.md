# Análise — rotas PUT/POST de Reversas (API Send4, versão 1.0.8)

Fonte: especificação OpenAPI pública
`https://api.swaggerhub.com/apis/send42/public-api/1.0.8` (mesmo conteúdo da página
`app.swaggerhub.com/apis/send42/public-api/1.0.8`).

Objetivo: entender as cinco rotas de escrita da seção de Reversas e comparar com o que já existe no
código, para definir como implementá-las. **Nada foi implementado ainda.**

---

## 1. Resumo

| # | Rota | O que faz | No nosso código hoje |
|---|---|---|---|
| 1 | `POST /v3/api/reverses/{id}/approve` | Aprova os produtos da reversa | `PostApproveReverseById(long id)` → `NotImplementedException` |
| 2 | `PUT /v3/api/reverses/{id}/receive-products` | Registra o recebimento dos produtos | `PutReciveProductsById(long id)` → `NotImplementedException` |
| 3 | `PUT /v3/api/reverses/{id}/products/returned-invoice` | Informa a nota fiscal de devolução de produtos | `PutReturnedInvoiceById(long id)` → `NotImplementedException` |
| 4 | `POST /v3/api/reverses/{id}/invoice` | Envia os dados da nota para a transportadora e gera o código de postagem | `PostReverseInvoiceById(long id)` → `NotImplementedException` |
| 5 | `POST /v3/api/reverses/{id}/administrative-records` | Cria um registro administrativo (anotação) na reversa | **Não existe** |

Todas usam `Authorization: Bearer <token>` (o token da empresa, de `AfterSaleParameters`),
`Content-Type: application/json` e `Accept: application/json`.

---

## 2. Detalhe de cada rota

### 2.1 `POST /v3/api/reverses/{id}/approve` — aprovar produtos

Corpo (JSON):

```json
{
  "comments": "texto livre",
  "skip_process_step": false,
  "products": [
    { "id": 1, "qty": 3, "product_received_comment": "observação", "reason_id": 30 }
  ]
}
```

| Campo | Tipo | Obrigatório |
|---|---|---|
| `comments` | string | não |
| `skip_process_step` | boolean | não |
| `products[].id` | inteiro (id do produto **na reversa**) | não |
| `products[].qty` | inteiro | não |
| `products[].product_received_comment` | string | não |
| `products[].reason_id` | inteiro | não |

Resposta `200`:

```json
{ "success": true, "status": { "id": 34, "service_code": "reverses", "name": "processing", "description": "processando" } }
```

### 2.2 `PUT /v3/api/reverses/{id}/receive-products` — receber produtos

Corpo (JSON):

```json
{
  "products": [
    {
      "id": 1,
      "qty": 3,
      "product_received_comment": "observação",
      "situation": "repack",
      "fraud": "others",
      "reason_id": 30
    }
  ]
}
```

| Campo | Tipo | Valores aceitos |
|---|---|---|
| `products[].id` | inteiro | — |
| `products[].qty` | inteiro | — |
| `products[].product_received_comment` | string | — |
| `products[].situation` | string | `repack`, `recondition`, `discard`, `sale` |
| `products[].fraud` | string | `free_shipping`, `used_without_defect`, `wrong_package`, `others` |
| `products[].reason_id` | inteiro | — |

Nenhum campo é marcado como obrigatório.

Resposta `200`:

```json
{ "success": true, "message": "Reverse updated.", "status": { "id": 3, "service_code": "reverses", "name": "done", "description": "done" } }
```

### 2.3 `PUT /v3/api/reverses/{id}/products/returned-invoice` — nota de devolução

Corpo (JSON):

```json
{ "id": [1, 2, 3, 4], "returned_invoice": "123456789" }
```

- `id`: lista de ids de produtos **da reversa** que recebem a nota.
- `returned_invoice`: número da nota de devolução (string).

Resposta `200`: `{ "success": true }`.

> Esse valor vai para `AfterSaleReversesProducts.returned_invoice`, que nós já lemos no
> `GetReverse`. Depois da chamada, a próxima execução do `GetReverse` traz a nota gravada.

### 2.4 `POST /v3/api/reverses/{id}/invoice` — dados da nota para a transportadora

Duas formas de envio, conforme a transportadora:

**A) Arquivo XML** (`multipart/form-data`):

| Campo | Tipo |
|---|---|
| `id` | inteiro (id da reversa) |
| `xml` | arquivo (XML da NF-e) |

**B) Campos da nota** (`application/json`):

```json
{
  "id": 4927,
  "nf_access_key": "<44 caracteres>",
  "nf_number": "7",
  "nf_serial": "1",
  "nf_amount": 100,
  "nf_cfop": "5000",
  "weight": "1"
}
```

Resposta `200`: `{ "success": true }`.

> **Ligação com `GetCourierAttributes`:** é a rota de courier attributes que diz qual das duas
> formas a transportadora daquela reversa exige.
> - Se `fields` traz `{ "id": "xml", "type": "file", "validation": { "mime": "xml" } }` → forma **A**.
> - Se traz `nf_access_key`, `nf_number` etc. → forma **B**, com `validation.mask` indicando o
>   formato (ex.: 44 caracteres para a chave).
> - `fill_form_from_file` mapeia os caminhos do XML da NF-e para esses mesmos campos, ou seja:
>   com o XML em mãos dá para montar a forma **B** automaticamente.
>
> Por isso os dados de `AfterSaleCourierAttributes`/`Fields` que acabamos de passar a gravar são o
> pré-requisito desta rota.

### 2.5 `POST /v3/api/reverses/{id}/administrative-records` — registro administrativo

Corpo (JSON):

```json
{ "message": "texto", "inserted_from_type": "reverse", "inserted_from_id": "100" }
```

`inserted_from_type` e `inserted_from_id` são opcionais (o exemplo sugere `reverse` ou `refund`).

Resposta `201`:

```json
{
  "success": true,
  "data": {
    "id": "1",
    "message": "Some Message XPTO",
    "reverse_id": "100",
    "username": "API TF",
    "inserted_from": "Reversa, id 100",
    "created_at": "2023-06-30T22:55:39.000000Z"
  }
}
```

### 2.6 Inconsistências da própria especificação

- Nenhuma rota documenta respostas de erro além de `401`. Na prática a API devolve
  `{ "success": false, "error", "error_code", "message", "message_translated" }` (é o formato que
  já tratamos em `GetReverses`/`GetReverse`), então vale tratar `success = false` do mesmo jeito.
- O `id` da rota aparece como `integer` em umas e `string` em outras; em todas é o id da reversa.
- As descrições de `invoice`, `returned-invoice` e `administrative-records` foram copiadas de
  outras rotas ("get a reverse", "Set reverse courier data…").

---

## 3. O que falta no nosso código

### 3.1 Assinaturas dos métodos

Os stubs recebem só `long id`, mas todas as rotas precisam de um **corpo** e de saber **qual
empresa** é dona da reversa (para escolher o token). Proposta:

```csharp
public Task<bool?> PostApproveReverseById(string doc_company, long id, ApproveReverseRequest request);
public Task<bool?> PutReciveProductsById(string doc_company, long id, ReceiveProductsRequest request);
public Task<bool?> PutReturnedInvoiceById(string doc_company, long id, ReturnedInvoiceRequest request);
public Task<bool?> PostReverseInvoiceById(string doc_company, long id, ReverseInvoiceRequest request);   // forma B (JSON)
public Task<bool?> PostReverseInvoiceById(string doc_company, long id, byte[] xml, string fileName);   // forma A (arquivo)
public Task<bool?> PostAdministrativeRecordById(string doc_company, long id, AdministrativeRecordRequest request);
```

> O `doc_company` também poderia ser descoberto pelo `id` consultando
> `AfterSaleSummarizedReverses`, mas como o mesmo id pode existir nos dois CNPJs, é mais seguro
> recebê-lo de quem chama.

### 3.2 DTOs novos

Em `Domain.AfterSale/Dtos/Reverses/` (uma pasta por rota, como as demais):

| DTO | Propriedades |
|---|---|
| `Approve/ApproveReverseRequest` | `comments`, `skip_process_step`, `products: List<ApproveProduct>` (`id`, `qty`, `product_received_comment`, `reason_id`) |
| `ReceiveProducts/ReceiveProductsRequest` | `products: List<ReceiveProduct>` (`id`, `qty`, `product_received_comment`, `situation`, `fraud`, `reason_id`) |
| `ReturnedInvoice/ReturnedInvoiceRequest` | `id: List<long>`, `returned_invoice` |
| `Invoice/ReverseInvoiceRequest` | `id`, `nf_access_key`, `nf_number`, `nf_serial`, `nf_amount`, `nf_cfop`, `weight` |
| `AdministrativeRecords/AdministrativeRecordRequest` | `message`, `inserted_from_type`, `inserted_from_id` |
| `Actions/ActionResponse` (resposta comum) | `success`, `message`, `status` (`id`, `service_code`, `name`, `description`), `error`, `error_code`, `message_translated` |
| `AdministrativeRecords/AdministrativeRecordResponse` | `success`, `data` (`id`, `message`, `reverse_id`, `username`, `inserted_from`, `created_at`) |

`situation` e `fraud` podem ser enums C# serializados como texto
(`[JsonConverter(typeof(StringEnumConverter))]`) para não aceitar valores fora da lista.

Para não enviar campos nulos (a API pode interpretar `null` como "limpar"), serializar com
`NullValueHandling.Ignore`.

### 3.3 `ApiClient`

[ApiClient.cs](../../../../Shared/Infrastructure.Shared/Api/ApiClient.cs):

| Lacuna | Impacto | Proposta |
|---|---|---|
| Não existe envio `multipart/form-data` com arquivo (`PostFormAsync` é `application/x-www-form-urlencoded`) | A forma **A** do `/invoice` (XML) não tem como ser enviada | Novo `PostMultipartAsync(route, Dictionary<string,string> fields, (string name, byte[] content, string fileName, string contentType) file, headers)` |
| Os métodos devolvem só o corpo e descartam o status HTTP | Uma resposta `401`/`404`/`500` sem JSON vira `null` ou HTML e é difícil distinguir de sucesso | Ou retornar `(int statusCode, string? body)`, ou registrar o status no log de erro. Para as rotas de escrita é importante saber se a alteração foi aceita. |
| `PostAsync`/`PutAsync` serializam com as configurações padrão do Newtonsoft | Campos nulos são enviados | Aceitar `JsonSerializerSettings` opcional ou serializar no service com `NullValueHandling.Ignore` |

### 3.4 Jobs e log

| Ação | `EnumJob` | Job em `Script007` |
|---|---|---|
| Aprovar | `AfterSaleApproveReverse = 5005` | já existe |
| Receber produtos | `AfterSaleReciveProducts = 5006` | já existe |
| Nota de devolução | `AfterSaleReturnedInvoice = 5007` | já existe |
| Nota para a transportadora | `AfterSaleReverseInvoice = 5008` | já existe |
| Registro administrativo | **não existe** — próximo livre: `5021` | **adicionar** `(50, 5021, 'AfterSaleAdministrativeRecordsReverse', ..., 3, 2)` |

Cada chamada deveria gravar no log: o corpo enviado e a resposta (`AddRecord(id, json)`), e em
caso de `success = false` um `AfterSaleErrors` com `error`/`error_code`/`message`, como já fazemos
em `GetReverse`.

### 3.5 Exposição

O `AfterSaleController` do host Hangfire só tem ações sem parâmetros, que disparam jobs de leitura.
Estas rotas precisam de dados de entrada, então a forma de disparo é uma decisão (ver seção 4).

---

## 4. Decisões necessárias antes de implementar

1. **De onde vêm os dados?** Quem decide aprovar uma reversa, com quais quantidades e motivos, qual
   a situação de cada produto no recebimento e qual a nota de devolução? As opções mais comuns:
   - **a)** um sistema externo (ERP, tela web) chama um endpoint nosso com o corpo pronto;
   - **b)** uma tabela de pendências no nosso banco, preenchida por outro processo, e um job
     recorrente que lê essa tabela e chama a API.
2. **Como disparar?** Ligado à resposta 1: endpoint HTTP com corpo no `AfterSaleController` (opção
   a) ou job sem parâmetro que processa pendências (opção b).
3. **Nota para a transportadora (`/invoice`):** vamos ter o XML da NF-e (forma A, ou montar a forma
   B a partir dele com `fill_form_from_file`) ou só os campos digitados (forma B)?
4. **Registro administrativo:** implementar agora? Não há stub nem job para ele.

## 5. Ordem sugerida de implementação

1. `ApiClient`: status HTTP + `PostMultipartAsync`.
2. DTOs de request/response e resposta de erro comum.
3. `PutReturnedInvoiceById` (corpo mais simples; bom para validar o fluxo de escrita + log).
4. `PostApproveReverseById` e `PutReciveProductsById`.
5. `PostReverseInvoiceById` (as duas formas), usando os courier attributes já gravados.
6. `PostAdministrativeRecordById` + `EnumJob` 5021 + job no `Script007`, se decidido.
7. Endpoints/jobs conforme as decisões da seção 4.
