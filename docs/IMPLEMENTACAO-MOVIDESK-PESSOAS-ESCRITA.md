# Movidesk — Escrita de pessoas (`POST /persons`, `PATCH /persons`, `POST /persons/relationships` e `DELETE /persons`)

Proposta para implementar as rotas de escrita da API de pessoas:
* criar uma pessoa;
* alterar uma pessoa;
* incluir organizações (relacionamentos) numa pessoa que já existe;
* excluir uma pessoa, incluída pela decisão 5 para encerrar o teste real.

Segue o que já foi feito e testado na escrita de tickets ([IMPLEMENTACAO-MOVIDESK-TICKETS-ESCRITA.md](IMPLEMENTACAO-MOVIDESK-TICKETS-ESCRITA.md)):
`IApiClient.SendAsync`, `Result`, log em `general.MovideskRequests` com o corpo enviado, validação antes de chamar a
API e as rotas sob demanda com `X-User-Name`.

> **Implementado em 22/10/2026**, com as 7 decisões da seção 10, e testado na API real com o roteiro da seção 9 (pessoa
> `9a3b9d8a-376e-4317-`, excluída ao final). As rotas e regras da seção 1 vêm da
> [documentação de pessoas](https://atendimento.movidesk.com/kb/article/189/movidesk-person-api), lida em 22/10/2026.
>
> Diferença em relação ao texto abaixo, vinda do teste real (seção 9.1, decisão 8): o **`id` (Cód. Ref.) passou a ser
> obrigatório no POST**, só com letras e números. Sem ele, a Movidesk gera um Cód. Ref. com hífen, que as rotas
> `People/{docCompany}/{id}` não aceitam.

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`. Uma classe por arquivo `.cs`. **Person** no singular e **People**
no plural.

---

## 1. O que a API oferece

| Método | Rota | Parâmetros | Corpo | Retorno documentado |
|---|---|---|---|---|
| **POST** | `/persons` | `token`, `returnAllProperties` (padrão `false`) | JSON no layout da pessoa | 200 e o id (Cód. Ref.) da pessoa criada |
| **PATCH** | `/persons` | `token`, `id` | JSON só com o que muda | 200 |
| **POST** | `/persons/relationships` | `token`, `id` (da pessoa) | `{ "relationships": [ ... ] }` | 200 |
| **DELETE** | `/persons` | `token`, `id` | nenhum (o exemplo da documentação não envia corpo) | 200 |

Não existe PUT.

### 1.1 Campos obrigatórios na criação (`POST /persons`)

| Objeto | Obrigatórios |
|---|---|
| `person` | `isActive`, `personType` (1 = Pessoa, 2 = Empresa, 4 = Departamento), `profileType` (1 = Agente, 2 = Cliente, 3 = Agente e Cliente), `businessName` |
| `person.addresses[n]` | `addressType`, `isDefault` |
| `person.contacts[n]` | `contactType`, `contact`, `isDefault` |
| `person.emails[n]` | `emailType`, `email` (válido), `isDefault` |
| `person.relationships[n]` | `forceChildrenToHaveSomeAgreement`; `services` com ao menos um item quando `allowAllServices = false` |
| `person.relationships[n].services[n]` | `id` |
| `person.customFieldValues[n]` | `customFieldId`, `customFieldRuleId`, `line`; `value` ou `items` conforme o tipo do campo |

**Obrigatórios que dependem de outros campos:**
* `corporateName`: empresa (`personType = 2`) sem `businessName`;
* `accessProfile` (um perfil já cadastrado): agente. Num cliente, se não vier, a Movidesk usa o perfil padrão de cliente;
* `teams` (ao menos uma equipe): agente;
* `userName`: quando a pessoa se autentica num domínio diferente do Movidesk (`authenticateOn`);
* `relationships`: pode ser exigido pela configuração da conta, para pessoa (`personType = 1`) com perfil cliente.

**Regras de formato:**
* `cpfCnpj`: 11 dígitos (CPF) para pessoa, 14 (CNPJ) para empresa, nenhum para departamento;
* `addresses`, `contacts` e `emails`: só um item com `isDefault = true` em cada lista;
* `relationships`: um único relacionamento por organização. Uma hierarquia segue o padrão `IdPai/IdFilho`.

### 1.2 Campos que podem ser escritos pela integração

`id` (só na criação), `codeReferenceAdditional`, `isActive`, `personType`, `profileType`, `accessProfile`,
`corporateName`, `businessName`, `businessBranch`, `cpfCnpj`, `userName`, `role`, `bossId`, `classification`,
`cultureId`, `timeZoneId`, `authenticateOn`, `observations`, `addresses`, `contacts`, `emails`, `teams`,
`relationships`, `customFieldValues` e `atAssets`.

* **`password` fica de fora (decisão 3):** a API aceita senha na escrita, mas a integração não recebe nem repassa senhas.
  Quem precisar de acesso define a senha pela Movidesk.
* **Somente leitura:** `bossName`, `createdDate`, `createdBy`, `changedDate`, `changedBy`, e o `id` depois da criação.

### 1.3 Regras do PATCH

* **Alteração parcial:** só os campos enviados mudam.
* **Listas sobrescrevem tudo:** `addresses`, `contacts`, `emails`, `teams`, `relationships`, `customFieldValues` e
  `atAssets` enviados substituem a lista inteira. `{ "emails": [] }` apaga todos os e-mails da pessoa.
* **Campos adicionais:** os que estiverem gravados e não forem enviados no corpo são excluídos. Para alterar o valor de
  um campo, informe a linha (`line`) em que ele está.

### 1.4 `POST /persons/relationships` × `relationships` no PATCH

* **`POST /persons/relationships`** ("Inserindo organizações para uma pessoa") **inclui** organizações sem mexer nas que
  a pessoa já tem.
* **`relationships` no PATCH** **substitui** a lista inteira.

Para só acrescentar uma organização, a rota certa é o `POST /persons/relationships`.

### 1.5 `DELETE /persons`

Exclui a pessoa na Movidesk. É **irreversível**, e a rota fica disponível para o sistema externo como as outras.
* Não há validação de corpo: o único dado é o Cód. Ref. da rota.
* Depois da exclusão na Movidesk, a pessoa e as filhas também são apagadas do banco local (decisão 7, seção 6.2).

### 1.6 Limites

Os mesmos da escrita de tickets: 10 requisições por minuto por token e bloqueio de 60 s depois de 3 requisições com
erro seguidas. Por isso o corpo é validado antes de chamar a API (seção 5).

---

## 2. Visão geral da proposta

```
MovideskController (X-User-Name, docCompany, id)
   │  valida rota e corpo (FluentValidation) → 400 sem chamar a API
   ▼
MovideskPeopleService.PostPerson / PatchPersonById / PostPersonRelationshipsById / DeletePersonById
   │  fluxo comum (SendPerson): log do job, empresa, chamada, MovideskRequests
   ▼
IApiClient.SendAsync (já existe: status HTTP, PATCH, DELETE, query string)
   ▼
sucesso → POST com returnAllProperties=true: se a resposta trouxer a pessoa inteira, grava com o SavePeople
          PATCH e relacionamentos: o GetPeople traz a alteração na próxima execução (até 3 minutos)
          DELETE: apaga a pessoa e as filhas do banco local (decisão 7)
```

| Método do service | Rota da Movidesk | Rota do controller | Job |
|---|---|---|---|
| `PostPerson` | `POST persons?returnAllProperties=true` | `POST People/{docCompany}` | 3009 `MovideskPostPerson` |
| `PatchPersonById` | `PATCH persons?id={id}` | `PATCH People/{docCompany}/{id}` | 3010 `MovideskPatchPersonById` |
| `PostPersonRelationshipsById` | `POST persons/relationships?id={id}` | `POST People/{docCompany}/{id}/Relationships` | 3011 `MovideskPostPersonRelationshipsById` |
| `DeletePersonById` | `DELETE persons?id={id}` | `DELETE People/{docCompany}/{id}` | 3012 `MovideskDeletePersonById` |

Os quatro ficam no `MovideskPeopleService`, junto das leituras de pessoas, como a escrita de tickets ficou no
`MovideskTicketsService`.

---

## 3. O que já existe e o que muda na escrita de tickets

| Item | Situação |
|---|---|
| `IApiClient.SendAsync` / `ApiResponse` (Shared) | Já existe; aceita `HttpMethod.Delete` sem corpo. Nada muda |
| `MovideskService.SaveRequest(..., requestBody)` | Já existe; nada muda |
| `SavePeople` (privado do `MovideskPeopleService`) | Usado pelo `PostPerson` quando a resposta traz a pessoa inteira |
| `ISharedRepository.ExecuteCommand(sql, entity)` | Já existe; executa o `DELETE` local com parâmetros (seção 6.2) |
| Remoção do `Z` das datas no retorno do POST | Mesmo tratamento do `PostTicket`, se a resposta de pessoas vier igual (a conferir na seção 9) |
| **`Result`** | **Muda (decisão 1)**: sai de `Dtos/Writes/Tickets/Result.cs` para `Dtos/Writes/Result.cs` |

### 3.1 `Result` comum às escritas (decisão 1)

`Domain.Movidesk/Dtos/Writes/Result.cs`, namespace `Domain.Movidesk.Dtos.Writes`:

```csharp
public class Result
{
    public bool success { get; init; }
    public string? id { get; init; }            // número do ticket ou Cód. Ref. da pessoa (no POST, o criado pela API)
    public int statusCode { get; init; }        // status devolvido pela Movidesk; 0 quando a API nem foi chamada
    public string? message { get; init; }
    public string? response { get; init; }
    public int? retryAfterSeconds { get; init; }
}
```

Ajustes na escrita de tickets, sem mudar o comportamento:
* `Dtos/Writes/Tickets/Result.cs` é removido;
* `IMovideskTicketsService`, `MovideskTicketsService` (`PostTicket`, `PatchTicketById`, `PostTicketFileUploadById`,
  `SendTicket`) e o `WriteResponse` do controller passam a usar `Domain.Movidesk.Dtos.Writes.Result`;
* `ticketId` (`long?`) vira `id` (`string?`). O JSON das respostas de tickets troca `"ticketId": 69369` por
  `"id": "69369"`, e quem consome as rotas de tickets precisa saber disso;
* o guia de escrita de tickets e o README passam a citar o `Result` novo.

---

## 4. DTOs de escrita (`Domain.Movidesk/Dtos/Writes/People`)

Mesmas regras dos DTOs de escrita de tickets:
* só campos graváveis (seção 1.2, **sem `password`**), com os tipos reais;
* tudo anulável, inclusive as listas;
* serialização sem nulos. No PATCH: `null` = não muda; `[]` = apaga a lista; lista com itens = substitui.

```
Dtos/Writes/People/
├── Person.cs                 (id, codeReferenceAdditional, isActive, personType, profileType, accessProfile,
│                              corporateName, businessName, businessBranch, cpfCnpj, userName, role, bossId,
│                              classification, cultureId, timeZoneId, authenticateOn, observations, addresses,
│                              contacts, emails, teams, relationships, customFieldValues, atAssets)
├── Address.cs                (addressType, country, postalCode, state, city, district, street, number, complement,
│                              reference, isDefault)
├── Contact.cs                (contactType, contact, isDefault)
├── Email.cs                  (emailType, email, isDefault)
├── Relationship.cs           (id, slaAgreement, forceChildrenToHaveSomeAgreement, allowAllServices,
│                              includeInParents, loadChildOrganizations, services)
├── RelationshipService.cs    (id, copyToChildren)
├── Relationships.cs          (relationships: corpo do POST /persons/relationships)
├── CustomFieldValue.cs       (customFieldId, customFieldRuleId, line, value, items)
├── CustomFieldValueItem.cs   (personId, clientId, team, customFieldItem)
└── Asset.cs                  (Id, name, label; a documentação usa "Id" com maiúscula em atAssets)
```

* `Relationship.allowAllServices` fica anulável: quando não vem, a Movidesk assume `true`.
* Como o `Person` não tem `password`, uma senha enviada no JSON é ignorada na desserialização. Ela nunca chega ao
  service, ao log nem à Movidesk.

---

## 5. Validação antes de chamar a API

Validadores em `Application.Movidesk/Validations/Writes/People/`, um por arquivo. O controller devolve **400** sem chamar
a Movidesk.

| Validador | Regras |
|---|---|
| `PostPersonValidator` | `id` **obrigatório**, só letras e números (até 64, o mesmo formato da rota `People/{docCompany}/{id}`; decisão 8); `isActive` preenchido; `personType` em {1, 2, 4}; `profileType` em {1, 2, 3}; `businessName` preenchido |
| `PatchPersonValidator` | ao menos um campo; `id` **não** pode vir no corpo (somente leitura depois da criação; o id vai na rota); `personType`/`profileType`, se vierem, nos valores válidos |
| `PersonRulesValidator` (incluído nos dois) | agente (`profileType` 1 ou 3) com `accessProfile` e ao menos uma equipe em `teams`, na criação; `cpfCnpj` só com dígitos, 11 para pessoa, 14 para empresa, ausente para departamento; um único `isDefault = true` em `addresses`, `contacts` e `emails`; organizações sem repetir em `relationships` |
| `AddressValidator` | `addressType` e `isDefault` preenchidos |
| `ContactValidator` | `contactType`, `contact` e `isDefault` preenchidos |
| `EmailValidator` | `emailType`, `isDefault` e `email` preenchidos; `email` em formato válido |
| `RelationshipValidator` | `forceChildrenToHaveSomeAgreement` preenchido; ao menos um serviço quando `allowAllServices = false`; cada serviço com `id > 0` |
| `RelationshipsValidator` | corpo do `POST /persons/relationships`: ao menos um relacionamento, sem organização repetida |
| `CustomFieldValueValidator` | `customFieldId`, `customFieldRuleId` e `line` maiores que zero |

O `DELETE` não tem corpo, então só a rota é validada (id com letras e números, até 64).

O que só a Movidesk sabe (perfil de acesso, equipe, contrato de SLA ou serviço cadastrados, `userName` único) volta da API
como **422**.

---

## 6. Service: fluxo comum das quatro rotas (decisão 2)

Um método privado `SendPerson` no `MovideskPeopleService`, usado pelas quatro rotas. Ele repete o desenho do `SendTicket`,
como o AfterSale faz com `SendReverseAction` e `SendRefundAction`:

1. abre o log com o job da rota;
2. busca a empresa pelo CNPJ. Se não existir, devolve resultado sem sucesso sem chamar a API;
3. monta o corpo (JSON sem nulos; nenhum no DELETE) e o `requestBody` do log;
4. chama `_apiClient.SendAsync`. Se a chamada não chegar a ter resposta, grava a tentativa em `MovideskRequests` e
   relança a exceção;
5. grava a chamada em `general.MovideskRequests`. O `field_key_value` é o Cód. Ref. da pessoa: no POST, o devolvido pela
   API; nas outras, o da rota;
6. depois do sucesso na Movidesk:
   * **POST:** se a resposta trouxer a pessoa inteira, grava com o `SavePeople` (seção 6.1);
   * **DELETE:** apaga a pessoa e as filhas do banco local e tira a pessoa do cache (seção 6.2);
7. fecha o log num `finally`.

O passo 6 só roda quando a Movidesk responde com sucesso. Se a exclusão local falhar depois de a Movidesk já ter
excluído, a resposta continua sendo sucesso (a pessoa não existe mais lá), com a falha local na `message` e no log.

### 6.1 Banco depois da escrita

* **POST (decisão 6):** vai com `returnAllProperties=true`. Se a resposta trouxer a pessoa inteira, ela é desserializada
  no DTO de leitura (`Dtos/Reads/People/Person`) e gravada com o `SavePeople`, sem requisição a mais. Se as datas vierem
  com `Z`, como no POST de tickets, o sufixo é retirado antes.
* **PATCH e relacionamentos:** sem releitura. A alteração chega pela próxima execução do `GetPeople` (até 3 minutos),
  cuja janela olha `changedDate` ou `createdDate`.
  * Falta conferir se o `POST /persons/relationships` também atualiza o `changedDate` (seção 9). Se não atualizar, o
    relacionamento novo só aparece quando a pessoa mudar de novo, ou pelo `GET People/{docCompany}/{id}`.
* **DELETE (decisão 7):** a merge de pessoas nunca apaga linhas, então o `GetPeople` não removeria a pessoa excluída.
  Por isso o `DeletePersonById` apaga a pessoa e as filhas do banco local logo depois do sucesso na Movidesk (seção 6.2).

### 6.2 Exclusão no banco local (decisão 7)

As filhas de `MovideskPeople` têm FK para a pessoa **sem** `on delete cascade`. Só as netas caem em cascata com o nível
do meio: `MovideskPeopleRelationshipsServices` com `MovideskPeopleRelationships`, e `MovideskPeopleCustomFieldValuesItems`
com `MovideskPeopleCustomFieldValues`. Por isso o comando apaga as filhas antes da pessoa, numa transação só.

`IMovideskCommandHandler` / `MovideskCommandHandler`
```csharp
// filhas antes da pessoa (as FKs das filhas não têm cascade); as netas caem em cascata com o nível do meio
public string CreateDeletePersonQuery() =>
    @"set xact_abort on;
      begin transaction;
      delete from [general].MovideskPeopleAddresses where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeopleContacts where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeopleEmails where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeopleTeams where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeopleRelationships where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeopleCustomFieldValues where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeopleAssets where doc_company = @doc_company and personId = @id;
      delete from [general].MovideskPeople where doc_company = @doc_company and id = @id;
      commit transaction;";
```

`IMovideskPeopleRepository` / `MovideskPeopleRepository`
```csharp
public Task<bool> DeletePerson(string doc_company, string id) =>
    _coreRepository.ExecuteCommand(_movideskCommandHandler.CreateDeletePersonQuery(), new { doc_company, id });
```

* Os valores vão como **parâmetros** (`@doc_company`, `@id`), não concatenados no SQL.
* Depois de apagar, o service tira a pessoa do cache de estado (`Movidesk:People:<cnpj>`). Se a mesma pessoa for criada
  de novo com o mesmo Cód. Ref., ela não pode ser descartada como "já gravada".
* As tabelas de tickets guardam o Cód. Ref. das pessoas sem FK para `MovideskPeople`. Tickets antigos continuam com o
  Cód. Ref., mas ele deixa de ter cadastro correspondente no banco local.

### 6.3 Esboço

```csharp
public Task<Result> PostPerson(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.People.Person person) =>
    SendPerson(EnumJob.MovideskPostPerson, user_name, doc_company, person.id, HttpMethod.Post, "persons",
        query: new() { { "returnAllProperties", "true" } },
        body: person,
        // Cód. Ref. criado e, se a resposta trouxer a pessoa inteira, ela para gravar (formato a conferir)
        readResponse: response => ...);

public Task<Result> PatchPersonById(string user_name, string doc_company, string id, Domain.Movidesk.Dtos.Writes.People.Person person) =>
    SendPerson(EnumJob.MovideskPatchPersonById, user_name, doc_company, id, HttpMethod.Patch, "persons",
        query: new() { { "id", id } },
        body: person,
        readResponse: _ => (id, null));

public Task<Result> PostPersonRelationshipsById(string user_name, string doc_company, string id, Domain.Movidesk.Dtos.Writes.People.Relationships relationships) =>
    SendPerson(EnumJob.MovideskPostPersonRelationshipsById, user_name, doc_company, id, HttpMethod.Post, "persons/relationships",
        query: new() { { "id", id } },
        body: relationships,
        readResponse: _ => (id, null));

public Task<Result> DeletePersonById(string user_name, string doc_company, string id) =>
    SendPerson(EnumJob.MovideskDeletePersonById, user_name, doc_company, id, HttpMethod.Delete, "persons",
        query: new() { { "id", id } },
        body: null,                       // sem corpo: o request_body do log fica nulo
        readResponse: _ => (id, null));
        // no SendPerson, com sucesso e HttpMethod.Delete: _movideskPeopleRepository.DeletePerson(doc_company, id)
        // e remoção da pessoa do cache (seção 6.2)
```

`Result` aqui é o `Domain.Movidesk.Dtos.Writes.Result` comum (seção 3.1).

---

## 7. Controller

Rotas novas no `MovideskController`, com `X-User-Name` (até 100 caracteres), o CNPJ na rota (`^\d{14}$`) e o id no mesmo
formato da leitura (`[StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")]`):

| Método | Rota | Corpo | Service |
|---|---|---|---|
| POST | `People/{docCompany}` | JSON da pessoa | `PostPerson` |
| PATCH | `People/{docCompany}/{id}` | JSON só com o que muda | `PatchPersonById` |
| POST | `People/{docCompany}/{id}/Relationships` | `{ "relationships": [ ... ] }` | `PostPersonRelationshipsById` |
| DELETE | `People/{docCompany}/{id}` | nenhum | `DeletePersonById` |

Respostas iguais às da escrita de tickets, pelo mesmo `WriteResponse` (agora com o `Result` comum):

| Código | Quando |
|---|---|
| **200** | Escrita aceita |
| **400** | Corpo ou rota inválidos |
| **422** | Empresa não cadastrada ou a Movidesk recusou |
| **429** | Bloqueio por excesso de erros (`Retry-After`) |

---

## 8. Jobs e configuração

* **`EnumJob`:** `MovideskPostPerson = 3009`, `MovideskPatchPersonById = 3010`,
  `MovideskPostPersonRelationshipsById = 3011`, `MovideskDeletePersonById = 3012`.
* **`Script005`:** quatro linhas novas no bloco sob demanda, como foi feito com os jobs de escrita de tickets.
  * O comentário de faixas passa a citar a escrita de pessoas (3009–3012).
  * Num banco que já rodou o `Script005`, os jobs são inseridos à mão (no banco de desenvolvimento, como foi feito com
    3006–3008).
* **Tabelas:** nenhuma nova; o log vai para `general.MovideskRequests`.

---

## 9. Teste na API real (decisão 4)

O roteiro usa uma pessoa desabilitada, cliente e sem e-mail, para não disparar convite de acesso nem ocupar licença de
agente:

1. **Criar** pelo `POST People/38367316000199` uma pessoa com:
   * `isActive = false`;
   * `personType = 1` (pessoa) e `profileType = 2` (cliente);
   * `businessName` **"Teste - Cayo Kume"**;
   * sem e-mail e sem usuário;
2. **Alterar** a pessoa criada pelo `PATCH People/38367316000199/{id}` (por exemplo, `observations`);
3. **Incluir** o relacionamento com a organização **MISHA** (Cód. Ref. `1812557193`, empresa, a mesma do cadastro Cayo
   Kume) pelo `POST People/38367316000199/{id}/Relationships`, com `allowAllServices = true` e
   `forceChildrenToHaveSomeAgreement = false`;
4. **Excluir** a pessoa pelo `DELETE People/38367316000199/{id}` (decisão 5).

Durante o roteiro, conferir:
* **Retorno do POST com `returnAllProperties=true`:** a pessoa inteira? As datas vêm com `Z`?
* **Retorno do PATCH, do relacionamento e do DELETE:** 200 sem corpo?
* **Se o `POST /persons/relationships` atualiza o `changedDate`** (seção 6.1). Um `GET persons?id={id}` antes do DELETE
  mostra se o relacionamento entrou e se a data mudou.
* **Depois do DELETE:** o que o `GET persons?id={id}` devolve (404, como a leitura por id já trata?), e se a pessoa e as
  filhas sumiram de `general.MovideskPeople`, `MovideskPeopleRelationships` e `MovideskPeopleRelationshipsServices` no
  banco de desenvolvimento.

Não provocar erros de propósito, por causa do bloqueio de 60 s.

### 9.1 Resultado do teste (22/10/2026)

Roteiro executado pelas rotas novas do host, na empresa `38367316000199`:

| Passo | Chamada | Resultado |
|---|---|---|
| Criar | `POST People/38367316000199` (desabilitada, cliente, sem e-mail, "Teste - Cayo Kume") | 200 em 1,5 s; Cód. Ref. **`9a3b9d8a-376e-4317-`** |
| Alterar | `PATCH People/38367316000199/9a3b9d8a-376e-4317-` (`observations`) | 200 em 1,5 s, corpo vazio |
| Relacionar | `POST .../9a3b9d8a-376e-4317-/Relationships` (MISHA, `1812557193`) | 200 em 0,7 s, corpo vazio |
| Excluir | `DELETE People/38367316000199/9a3b9d8a-376e-4317-` | 200 em 0,6 s, corpo vazio |

Respostas às perguntas da seção 9:

* **POST com `returnAllProperties=true`:** devolve a **pessoa inteira**, que foi gravada na hora. Diferente de tickets,
  as datas vêm **sem `Z`** (`2026-10-22T20:34:14.1058548`), e a gravação ficou certa (20:34). A remoção do `Z` continua
  no código, sem efeito nesse formato.
  * A resposta também traz o perfil de acesso padrão (`accessProfile: "Clientes"`) e um relacionamento sem
    organização (`id: null`, `allowAllServices: true`), criados pela Movidesk.
* **Cód. Ref. gerado pela Movidesk:** sem `id` no POST, a Movidesk gera um Cód. Ref. **com hífen**
  (`9a3b9d8a-376e-4317-`). Nenhuma das 1.313 pessoas já integradas tinha hífen.
  * As rotas `People/{docCompany}/{id}` só aceitam letras e números, então a pessoa criada não poderia ser alterada
    nem excluída por elas. Para concluir o roteiro, o hífen foi aceito temporariamente nas rotas; depois do teste,
    as rotas voltaram a só letras e números e o `id` passou a ser **obrigatório no POST** (decisão 8). As
    validações locais conferiram: POST sem `id` e `id` com hífen voltam 400, e as quatro rotas recusam id com hífen.
* **PATCH:** 200 sem corpo; atualiza o `changedDate`.
* **`POST /persons/relationships`:** 200 sem corpo. Ele **atualiza o `changedDate`**, então o `GetPeople` traz o
  relacionamento na execução seguinte (confirmado: os 2 relacionamentos entraram no banco local).
* **DELETE:** 200 sem corpo. Depois dele, o `GET persons?id=` devolve **404** em `application/problem+json`, o mesmo
  formato que o `GetPersonById` já trata. A pessoa, os 2 relacionamentos e as demais filhas sumiram do banco local.
* **Senha:** o DTO `Person` não tem `password`, então um `password` no JSON é descartado na desserialização e não chega ao
  service. Nas validações locais, um POST com `password` passou normalmente para o service (respondeu 422 só por usar uma
  empresa não cadastrada). O roteiro real não enviou senha.

Os 4 registros ficaram em `general.MovideskRequests` (jobs 3009 a 3012) com o corpo enviado; o do DELETE com
`request_body` nulo.

---

## 10. Decisões (22/10/2026)

| # | Pergunta | Decisão |
|---|---|---|
| 1 | `Result` para tickets e pessoas | **Mover para `Dtos/Writes/Result.cs`** (namespace `Domain.Movidesk.Dtos.Writes`), trocando `ticketId` por `id` (`string?`), e ajustar a escrita de tickets (seção 3.1) |
| 2 | Fluxo comum | **Repetir o desenho do `SendTicket`** num `SendPerson` no `MovideskPeopleService` (seção 6) |
| 3 | Senha (`password`) | **Não aceitar pela integração:** o campo sai do DTO (seções 1.2 e 4) |
| 4 | Roteiro de teste | **Pessoa desabilitada, cliente, sem e-mail, "Teste - Cayo Kume"**, relacionada à organização **MISHA** (seção 9) |
| 5 | Pessoa de teste ao final | **Excluir pela Movidesk.** A documentação tem a rota `DELETE /persons`, que entra no planejamento como `DeletePersonById` (job 3012) |
| 6 | `returnAllProperties` no POST | **`true`**, gravando direto se a resposta trouxer a pessoa inteira (seção 6.1) |
| 7 | Pessoa excluída no banco local | **Apagar a linha da pessoa (e as filhas)** depois do DELETE na Movidesk (seção 6.2) |
| 8 | Cód. Ref. com hífen gerado pela Movidesk (seção 9.1) | **Rotas só com letras e números, e `id` obrigatório no POST** de pessoas |
