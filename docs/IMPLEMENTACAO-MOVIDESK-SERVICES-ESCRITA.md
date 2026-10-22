# Movidesk — Escrita de serviços (`POST /services`, `PATCH /services` e `DELETE /services`)

Proposta para implementar as rotas de escrita do catálogo de serviços:
* criar um serviço;
* alterar um serviço;
* excluir um serviço.

Segue o que já foi feito e testado na escrita de tickets e de pessoas
([IMPLEMENTACAO-MOVIDESK-TICKETS-ESCRITA.md](IMPLEMENTACAO-MOVIDESK-TICKETS-ESCRITA.md),
[IMPLEMENTACAO-MOVIDESK-PESSOAS-ESCRITA.md](IMPLEMENTACAO-MOVIDESK-PESSOAS-ESCRITA.md)):
`IApiClient.SendAsync`, o `Result` comum (`Dtos/Writes/Result.cs`), log em `general.MovideskRequests` com o corpo
enviado, validação antes de chamar a API e as rotas sob demanda com `X-User-Name`. A leitura do catálogo está em
[IMPLEMENTACAO-MOVIDESK-SERVICES.md](IMPLEMENTACAO-MOVIDESK-SERVICES.md).

> **Implementado em 22/10/2026**, com as 6 decisões da seção 10, e testado na API real com o roteiro da seção 9
> (serviço `1465796`, excluído ao final). As rotas e regras da seção 1 vêm da
> [documentação de serviços](https://atendimento.movidesk.com/kb/pt-br/article/7440/api-servicos), lida em 22/10/2026.

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`. Uma classe por arquivo `.cs`.

---

## 1. O que a API oferece

| Método | Rota | Parâmetros | Corpo | Retorno documentado |
|---|---|---|---|---|
| **POST** | `/services` | `token`, `returnAllProperties` (padrão `false`) | JSON no layout do serviço | 200 e o id do serviço criado |
| **PATCH** | `/services` | `token`, `id` | JSON só com o que muda | 200 |
| **DELETE** | `/services` | `token`, `id` | nenhum (o exemplo da documentação não envia corpo) | 200 |

Não existe PUT.

### 1.1 Layout do serviço

| Campo | Tipo | Obrigatório | Regra |
|---|---|---|---|
| `id` | int | — | "Somente leitura", mas o exemplo de POST da documentação envia `"id": 5707`. A integração **não envia** `id` (decisão 2): a Movidesk gera, e o service devolve o id criado |
| `name` | string(128) | ✓ | Nome do serviço |
| `description` | string | — | |
| `parentServiceId` | int | — | Serviço pai |
| `serviceForTicketType` | int | ✓ | 0 = Público, 1 = Interno, 2 = Públicos e internos |
| `isVisible` | int | ✓ | 1 = Agente, 2 = Cliente, 3 = Agente e Cliente |
| `allowSelection` | int | ✓ | 1 = Agente, 2 = Cliente, 3 = Agente e Cliente |
| `allowFinishTicket` | bool | ✓ | Falso: o ticket não pode ser concluído com este serviço selecionado |
| `isActive` | bool | ✓ | Serviço desabilitado não aparece para seleção nos tickets |
| `automationMacro` | string(64) | — | Macro executada ao selecionar o serviço |
| `defaultCategory` | string(128) | — | Categoria padrão ao selecionar o serviço |
| `defaultUrgency` | string(128) | — | Urgência padrão ao selecionar o serviço |
| `allowAllCategories` | bool | ✓ | Permite todas as categorias |
| `categories` | array de string(128) | quando `allowAllCategories = false` | Nomes das categorias do serviço |

**Diferenças em relação à leitura:**
* `isVisible`: a leitura já trouxe o valor **0**, não documentado (gravado como "Ninguém (não documentado)" em
  `MovideskServiceAudiences`). A escrita aceita **só os valores documentados** (1, 2 e 3; decisão 6).
* `isActive`: na leitura, um serviço abaixo de um serviço desabilitado vem como desabilitado, mesmo que esteja habilitado
  individualmente. Na escrita, o `isActive` é o do próprio serviço.

### 1.2 Regras do PATCH

* **Alteração parcial:** só os campos enviados mudam.
* **Listas sobrescrevem tudo:** `categories` enviada substitui a lista inteira de categorias.

### 1.3 `DELETE /services`

Exclui o serviço do catálogo. É **irreversível**, e o catálogo classifica os tickets. A documentação não diz o que
acontece com os serviços filhos nem com os tickets que usam o serviço. O teste real só exclui um serviço sem filhos e sem
tickets (seção 9).

### 1.4 Limites

Os mesmos da escrita de tickets e pessoas: 10 requisições por minuto por token e bloqueio de 60 s depois de 3
requisições com erro seguidas. Por isso o corpo é validado antes de chamar a API (seção 5).

---

## 2. Visão geral da proposta

```
MovideskController (X-User-Name, docCompany, id)
   │  valida rota e corpo (FluentValidation) → 400 sem chamar a API
   ▼
MovideskServicesService.PostService / PatchServiceById / DeleteServiceById
   │  fluxo comum (SendService): log do job, empresa, chamada, MovideskRequests
   ▼
IApiClient.SendAsync (já existe)
   ▼
sucesso → POST com returnAllProperties=true: se a resposta trouxer o serviço inteiro, grava com o SaveServices
          PATCH: relê o serviço (GET services?id=) e grava com o SaveServices (decisão 1)
          DELETE: mantém o serviço no banco local (decisão 3)
          sempre: remove o hash do catálogo do cache, para o próximo GetServices regravar
```

| Método do service | Rota da Movidesk | Rota do controller | Job |
|---|---|---|---|
| `PostService` | `POST services?returnAllProperties=true` | `POST Services/{docCompany}` | 3013 `MovideskPostService` |
| `PatchServiceById` | `PATCH services?id={id}` | `PATCH Services/{docCompany}/{id}` | 3014 `MovideskPatchServiceById` |
| `DeleteServiceById` | `DELETE services?id={id}` | `DELETE Services/{docCompany}/{id}` | 3015 `MovideskDeleteServiceById` |

Os três ficam no `MovideskServicesService`, junto das leituras do catálogo.

---

## 3. O que já existe e é reaproveitado

| Item | Situação |
|---|---|
| `IApiClient.SendAsync` / `ApiResponse` (Shared) | Já existe; nada muda |
| `Result` (`Dtos/Writes/Result.cs`) | Já existe; o `id` (texto) leva o id do serviço |
| `MovideskService.SaveRequest(..., requestBody)` | Já existe; nada muda |
| `SaveServices` (privado do `MovideskServicesService`) | Grava o serviço devolvido pelo POST e o relido depois do PATCH (decisão 1) |
| Hash do catálogo no cache (`Movidesk:ServicesHash:<cnpj>`) | Removido depois de toda escrita com sucesso, como o `GetServiceById` já faz |
| Rota de leitura `Services/{docCompany}/{id:int:min(1)}` | Mesmo formato de id nas rotas novas |

O `id` do serviço é número, então não há o problema de formato do Cód. Ref. das pessoas.

---

## 4. DTO de escrita (`Domain.Movidesk/Dtos/Writes/Services/Service.cs`)

Uma classe só, porque o serviço não tem objetos filhos (`categories` é uma lista de textos):

```csharp
namespace Domain.Movidesk.Dtos.Writes.Services
{
    /// <summary>
    /// corpo do POST e do PATCH de serviços: só os campos que podem ser escritos. Tudo anulável e serializado sem nulos:
    /// no PATCH, null = campo não enviado (não muda); categories vazia = categorias apagadas; com itens = substituídas
    /// </summary>
    public class Service
    {
        // sem id: a Movidesk gera o id na criação (decisão 2), e no PATCH o id vai na rota. Um id enviado no JSON é ignorado
        public string? name { get; init; }
        public string? description { get; init; }
        public int? parentServiceId { get; init; }
        public int? serviceForTicketType { get; init; }   // 0 = Público, 1 = Interno, 2 = Públicos e internos
        public int? isVisible { get; init; }              // 1 = Agente, 2 = Cliente, 3 = Agente e Cliente
        public int? allowSelection { get; init; }         // 1 = Agente, 2 = Cliente, 3 = Agente e Cliente
        public bool? allowFinishTicket { get; init; }
        public bool? isActive { get; init; }
        public string? automationMacro { get; init; }
        public string? defaultCategory { get; init; }
        public string? defaultUrgency { get; init; }
        public bool? allowAllCategories { get; init; }
        public List<string>? categories { get; init; }
    }
}
```

---

## 5. Validação antes de chamar a API

Validadores em `Application.Movidesk/Validations/Writes/Services/`, um por arquivo. O controller devolve **400** sem
chamar a Movidesk.

| Validador | Regras |
|---|---|
| `PostServiceValidator` | `name` preenchido; `serviceForTicketType` em {0, 1, 2}; `isVisible` e `allowSelection` em {1, 2, 3} (só os documentados, decisão 6); `allowFinishTicket`, `isActive` e `allowAllCategories` preenchidos; `categories` com ao menos uma categoria quando `allowAllCategories = false` |
| `PatchServiceValidator` | ao menos um campo; os campos numéricos, se vierem, nos valores válidos (`isVisible` e `allowSelection` só 1, 2 e 3); `categories` com ao menos uma categoria quando vier `allowAllCategories = false` |
| `ServiceRulesValidator` (incluído nos dois) | `name` até 128; `automationMacro` até 64; `defaultCategory` e `defaultUrgency` até 128; cada categoria preenchida, até 128 e sem repetir (sem diferenciar maiúsculas); `defaultCategory` dentro de `categories` quando as duas vierem e `allowAllCategories = false`; `parentServiceId > 0` quando vier |

O DELETE não tem corpo; só a rota é validada (`{id:int:min(1)}`).

O que só a Movidesk sabe (serviço pai, macro, categoria ou urgência cadastrados) volta da API como **422**.

---

## 6. Service: fluxo comum das três rotas

Um método privado `SendService` no `MovideskServicesService`, com o mesmo desenho do `SendTicket` e do `SendPerson`:

1. abre o log com o job da rota;
2. busca a empresa pelo CNPJ. Se não existir, devolve resultado sem sucesso sem chamar a API;
3. monta o corpo (JSON sem nulos; nenhum no DELETE) e o `requestBody` do log;
4. chama `_apiClient.SendAsync`. Se a chamada não chegar a ter resposta, grava a tentativa em `MovideskRequests` e
   relança a exceção;
5. grava a chamada em `general.MovideskRequests`. O `field_key_value` é o id do serviço: no POST, o devolvido pela API;
   nas outras, o da rota;
6. depois do sucesso na Movidesk:
   * **remove o hash do catálogo** do cache (`Movidesk:ServicesHash:<cnpj>`), em todas as rotas;
   * **POST:** se a resposta trouxer o serviço inteiro, grava com o `SaveServices` (seção 6.1);
   * **PATCH:** relê o serviço e grava com o `SaveServices` (decisão 1);
   * **DELETE:** nada no banco local; o serviço continua gravado (decisão 3);
7. fecha o log num `finally`.

Se a gravação local falhar depois do sucesso na Movidesk, a resposta continua sendo sucesso, com a falha na `message` e
no log, como em tickets e pessoas.

### 6.1 Banco depois da escrita

O catálogo não tem data de alteração: o `GetServices` roda a cada **2 horas** e só regrava quando o hash da resposta
muda. Por isso o atraso padrão é bem maior que os 3 minutos de tickets e pessoas.

* **POST (decisão 4):** vai com `returnAllProperties=true`. Se a resposta trouxer o serviço inteiro, ele é
  desserializado no DTO de leitura (`Dtos/Reads/Services/Service`) e gravado com o `SaveServices`, sem requisição a mais.
  Se a resposta trouxer só o id, o serviço chega pelo próximo `GetServices`.
* **PATCH (decisão 1):** o PATCH responde 200 sem corpo, então o service **relê** o serviço (`GET services?id={id}`, a
  mesma chamada do `GetServiceById`) e grava com o `SaveServices`. É uma requisição a mais, e o banco fica certo na hora
  em vez de esperar até 2 horas.
  * A releitura usa o mesmo log da escrita, sem abrir outro job.
  * Se a releitura falhar, o PATCH continua valendo: vira mensagem no log e o `GetServices` corrige depois (o hash do
    catálogo já saiu do cache).
* **DELETE (decisão 3):** o serviço **continua no banco local**. A merge de serviços nunca apaga linhas, e a leitura do
  catálogo já decidiu manter os serviços removidos, porque tickets antigos apontam para eles (`serviceFirstLevelId`).
  Um serviço excluído pela integração fica igual a um removido pelo painel da Movidesk.

### 6.2 Esboço

```csharp
public Task<Result> PostService(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.Services.Service service) =>
    SendService(EnumJob.MovideskPostService, user_name, doc_company, null, HttpMethod.Post, "services",
        query: new() { { "returnAllProperties", "true" } },
        body: service,
        // id gerado pela Movidesk e, se a resposta trouxer o serviço inteiro, ele para gravar (formato a conferir)
        readResponse: response => ...);

public Task<Result> PatchServiceById(string user_name, string doc_company, int id, Domain.Movidesk.Dtos.Writes.Services.Service service) =>
    SendService(EnumJob.MovideskPatchServiceById, user_name, doc_company, id, HttpMethod.Patch, "services",
        query: new() { { "id", id.ToString() } },
        body: service,
        readResponse: _ => (id, null));
        // no SendService, com sucesso e HttpMethod.Patch: GET services?id={id} e SaveServices (decisão 1)

public Task<Result> DeleteServiceById(string user_name, string doc_company, int id) =>
    SendService(EnumJob.MovideskDeleteServiceById, user_name, doc_company, id, HttpMethod.Delete, "services",
        query: new() { { "id", id.ToString() } },
        body: null,
        readResponse: _ => (id, null));
```

---

## 7. Controller

Rotas novas no `MovideskController`, com `X-User-Name` (até 100 caracteres), o CNPJ na rota (`^\d{14}$`) e o id no mesmo
formato da leitura (`{id:int:min(1)}`):

| Método | Rota | Corpo | Service |
|---|---|---|---|
| POST | `Services/{docCompany}` | JSON do serviço | `PostService` |
| PATCH | `Services/{docCompany}/{id:int:min(1)}` | JSON só com o que muda | `PatchServiceById` |
| DELETE | `Services/{docCompany}/{id:int:min(1)}` | nenhum (**irreversível**) | `DeleteServiceById` |

Respostas pelo mesmo `WriteResponse` de tickets e pessoas:

| Código | Quando |
|---|---|
| **200** | Escrita aceita |
| **400** | Corpo ou rota inválidos |
| **422** | Empresa não cadastrada ou a Movidesk recusou |
| **429** | Bloqueio por excesso de erros (`Retry-After`) |

---

## 8. Jobs e configuração

* **`EnumJob`:** `MovideskPostService = 3013`, `MovideskPatchServiceById = 3014`, `MovideskDeleteServiceById = 3015`.
* **`Script005`:** três linhas novas no bloco sob demanda, como na escrita de tickets e pessoas. O comentário de faixas
  passa a citar a escrita de serviços (3013–3015). Num banco que já rodou o `Script005`, os jobs são inseridos à mão.
* **Tabelas:** nenhuma nova; o log vai para `general.MovideskRequests`.

---

## 9. Teste na API real (decisão 5)

Um serviço novo aparece no catálogo da empresa. Para não aparecer nos tickets durante o teste, o roteiro proposto usa um
serviço **desabilitado**, só para agentes e só em tickets internos:

1. **Criar** pelo `POST Services/38367316000199` um serviço com:
   * `name` **"Teste - Cayo Kume"**, sem serviço pai;
   * `isActive = false`;
   * `serviceForTicketType = 1` (interno), `isVisible = 1` e `allowSelection = 1` (agente);
   * `allowFinishTicket = true` e `allowAllCategories = true` (sem lista de categorias);
2. **Alterar** o serviço pelo `PATCH Services/38367316000199/{id}` (por exemplo, `description`);
3. **Excluir** o serviço pelo `DELETE Services/38367316000199/{id}`.

Durante o roteiro, conferir:
* **POST:** a Movidesk aceita sem `id` e qual id gera (decisão 2); o que o `returnAllProperties=true` devolve;
* **PATCH e DELETE:** 200 sem corpo? A releitura do PATCH grava a `description` nova no banco local?
* **Depois do DELETE:** o que o `GET services?id={id}` devolve (corpo vazio, como a leitura por id já trata?) e se o
  serviço some do `GET services` (catálogo inteiro). No banco local o serviço continua (decisão 3).

Não provocar erros de propósito, por causa do bloqueio de 60 s.

### 9.1 Resultado do teste (22/10/2026)

Roteiro executado pelas rotas novas do host, na empresa `38367316000199`:

| Passo | Chamada | Resultado |
|---|---|---|
| Criar | `POST Services/38367316000199` (desabilitado, interno, só agentes, "Teste - Cayo Kume") | 200 em 1,0 s; id **1465796** |
| Alterar | `PATCH Services/38367316000199/1465796` (`description`) | 200 em 1,0 s, corpo vazio |
| Excluir | `DELETE Services/38367316000199/1465796` | 200 em 0,3 s, corpo vazio |

Respostas às perguntas da seção 9:

* **POST sem `id`:** a Movidesk aceitou e gerou o id `1465796` (decisão 2). Com `returnAllProperties=true`, a resposta
  traz o **serviço inteiro**, que foi gravado na hora em `general.MovideskServices`. O serviço não tem datas, então não
  há o problema do `Z` dos tickets.
* **PATCH:** 200 sem corpo. A releitura gravou a `description` nova no banco local logo depois (decisão 1).
* **DELETE:** 200 sem corpo. Depois dele, o `GET services?id=` devolve **404 com corpo vazio**, o mesmo formato que o
  `GetServiceById` já trata, e o serviço sumiu do catálogo inteiro (de volta a 65 serviços). No banco local o serviço
  continua, com a última `description` (decisão 3).
* **Validações locais:** `isVisible = 0`, categorias repetidas, categoria padrão fora da lista e `allowSelection = 4`
  voltaram 400; um `id` enviado no POST foi ignorado (a chamada seguiu para o service).

Os 3 registros ficaram em `general.MovideskRequests` (jobs 3013, 3014 e 3015) com o corpo enviado; o do DELETE com
`request_body` nulo.

---

## 10. Decisões (22/10/2026)

| # | Pergunta | Decisão |
|---|---|---|
| 1 | PATCH no banco local | **Reler** o serviço (`GET services?id=`) e gravar na hora com o `SaveServices` (seção 6.1) |
| 2 | `id` no POST | **Não aceitar:** o DTO não tem `id`; a Movidesk gera, e o service devolve o id criado (seções 1.1 e 4) |
| 3 | Serviço excluído no banco local | **Manter**, porque tickets antigos apontam para o serviço e a leitura do catálogo já decidiu mantê-los (seção 6.1) |
| 4 | `returnAllProperties` no POST | **`true`**, gravando direto se a resposta trouxer o serviço inteiro (seção 6.1) |
| 5 | Roteiro de teste | **Serviço desabilitado, interno e só para agentes, "Teste - Cayo Kume"**, alterado e **excluído no final** (seção 9) |
| 6 | `isVisible = 0` | **Só os valores documentados** (1, 2 e 3), também em `allowSelection` (seção 5) |
