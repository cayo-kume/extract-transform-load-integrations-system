# Movidesk — Implementação C# do `GetTickets`

Guia para adaptar o código C# do módulo Movidesk às tabelas de
[MOVIDESK-MODELAGEM-TICKETS.md](MOVIDESK-MODELAGEM-TICKETS.md) (seção 8). Segue o padrão do AfterSale
(`AfterSaleReversesService.GetReverses`) e o [padrão de cache](../../../../../../docs/PADRAO-CACHE-INTEGRACOES.md).

> **Implementado em 19/10/2026.** O módulo e o host `Hangfire` compilam com 0 erros, e o host sobe com o endpoint
> `GET MovideskJobs/Movidesk/Tickets` no Swagger. O `GetTickets` ainda **não foi executado** contra a API nem o banco.
> Os scripts SQL estão em `Infrastructure.Movidesk/Scripts/SQLServer/Script001` a `Script005`.

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`.

---

## 0. Ponto de partida e ordem

Hoje o módulo **não compila** e nem chega a restaurar os pacotes:

| Problema | Onde |
|---|---|
| `PackageReference` sem versão (erro `NU1015`) | os 3 `.csproj` |
| Referências para `newbloomers-core-dlls`, que não existe no repositório | os 3 `.csproj` |
| `using Domain.Core.*` / `Application.Core.*` / `Infrastructure.Core.*` | cerca de 30 arquivos (tickets e pessoas) |
| `Domain.Movidesk.Models.*`, `TicketAntigo` e `Invoice`, que não existem | service, repositório, interface |
| `Entities/Ticket/Ticket.cs` com trechos copiados de `Person` (`emails`, `relationships`, `personDto`) | entidade |
| `ActionValidator` valida `x.customFieldValues`, que a ação não tem | validador |
| `IMovideskRepository` todo comentado | interface |

Ordem sugerida (cada passo compila sobre o anterior):

1. Projetos e namespaces (seções 1 e 2)
2. Empresas e parâmetros (3)
3. DTOs (4)
4. Entidades (5)
5. Validadores (6)
6. Queries, repositório e interface (7)
7. Service (8)
8. Divisão em três services e remoção das faturas (9)
9. Injeção de dependência e host (10)

---

## 1. Projetos (`.csproj`)

Iguais aos do AfterSale: referência ao `Shared` em vez de `newbloomers-core-dlls`, e versões explícitas.

**`Domain.Movidesk/Domain.Movidesk.csproj`**
```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="..\..\..\..\shared\Domain.Shared\Domain.Shared.csproj" />
</ItemGroup>
```
Remova também os `Compile/EmbeddedResource/None Remove` de `Entities\Services\**` e `Entities\Tickets\**`
(pastas que não existem mais).

**`Application.Movidesk/Application.Movidesk.csproj`**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="10.0.8" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="..\..\..\..\shared\Application.Shared\Application.Shared.csproj" />
  <ProjectReference Include="..\Domain.Movidesk\Domain.Movidesk.csproj" />
</ItemGroup>
```
Remova os `Remove` de `Validations\Services\**` e `Validations\Tickets\**`.

**`Infrastructure.Movidesk/Infrastructure.Movidesk.csproj`**
```xml
<ItemGroup>
  <ProjectReference Include="..\..\..\..\Shared\Infrastructure.Shared\Infrastructure.Shared.csproj" />
  <ProjectReference Include="..\Application.Movidesk\Application.Movidesk.csproj" />
  <ProjectReference Include="..\Domain.Movidesk\Domain.Movidesk.csproj" />
</ItemGroup>
```
O `Microsoft.Extensions.Http` sai: o AfterSale não declara e recebe pelo `Infrastructure.Shared`.

---

## 2. Namespaces

Troca mecânica em todos os `.cs` do módulo (inclusive pessoas, para o módulo compilar):

| De | Para |
|---|---|
| `Domain.Core.Extensions` | `Domain.Shared.Extensions` (conversores, `MustBeValid*`, `[SkipProperty]`) |
| `Domain.Core.Abstractions.Api` | `Domain.Shared.Abstractions.Api` |
| `Domain.Core.Abstractions.Repositorys` + `IIntegrationsCoreRepository` | `Domain.Shared.Abstractions.Repositorys` + `ISharedRepository` |
| `Application.Core.Abstractions.Services` | `Application.Shared.Abstractions.Services` |
| `Infrastructure.Core.Api` | `Infrastructure.Shared.Api` |
| `Domain.Core.Enums` (`EnumJob`) | `Domain.Movidesk.Enums` (o enum do próprio módulo) |

E, para ficar igual ao AfterSale, os namespaces das abstrações passam a seguir as pastas:

| Arquivo | Namespace atual | Novo |
|---|---|---|
| `Application.Movidesk/Abstractions/Services/IMovideskService.cs` (vira 3 interfaces, seção 9) | `Application.Movidesk.Interfaces.Services` | `Application.Movidesk.Abstractions.Services` |
| `Application.Movidesk/Abstractions/Handlers/Commands/IMovideskCommandHandler.cs` | `Domain.Movidesk.Interfaces.Handlers.Commands` | `Application.Movidesk.Abstractions.Handlers.Commands` |
| `Domain.Movidesk/Abstractions/Repositorys/IMovideskRepository.cs` | `Domain.Movidesk.Interfaces.Repositorys` | `Domain.Movidesk.Abstractions.Repositorys` |

### Pastas `Reads` (decidido)
Como no AfterSale, as classes das rotas GET ficam em `Reads`:

| De | Para | Namespace |
|---|---|---|
| `Domain.Movidesk/Dtos/Tickets/` | `Domain.Movidesk/Dtos/Reads/Tickets/` | `Domain.Movidesk.Dtos.Reads.Tickets` |
| `Domain.Movidesk/Entities/Ticket/` | `Domain.Movidesk/Entities/Reads/Tickets/` | `Domain.Movidesk.Entities.Reads.Tickets` |

Use `git mv` para manter o histórico. As pastas de pessoas (`Dtos/Person`, `Entities/Person`) ficam como estão
até o service de pessoas.

---

## 3. Empresas e parâmetros

A tabela `movidesk.MovideskParameters (doc_company, token)` já existe no `Script001`.

**Remover** `Domain.Movidesk/Entities/Parameters.cs` e **criar** `Domain.Movidesk/Entities/Parameters/Company.cs`,
igual ao `Company` do AfterSale:
```csharp
using Domain.Shared.Entities.Base;

namespace Domain.Movidesk.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }
    }
}
```

**`IMovideskCommandHandler` / `MovideskCommandHandler`:** `CreateGetTokenQuery()` sai e entram as queries de
`7.1`. A de empresas:
```csharp
public string CreateGetCompanysQuery()
{
    return @$"SELECT DISTINCT
             DOC_COMPANY AS DOC_COMPANY,
             TOKEN AS TOKEN
             FROM MOVIDESK.MOVIDESKPARAMETERS";
}
```

---

## 4. DTOs (`Dtos/Reads/Tickets`)

Continuam com tudo como `string` (padrão do projeto). Mudanças:

| Arquivo | Mudança |
|---|---|
| `Ticket.cs` | `slaResponsetime` → **`slaResponseTime`** (com o nome errado o valor nunca é lido). `asinits` → **`assets`**. Remover `satisfactionSurveyResponsese` e `serviceFull`. Manter `canceledIn` e `isDeleted`. |
| `CustomFieldValueItem.cs` | + `storageFileGuid`, + `fileName` (vêm na resposta real) |
| `Asset.cs` | Remover `categoryFull` |
| `SatisfactionSurveyResponse.cs` | **Remover** o arquivo (API depreciada) |
| `TimeAppointment.cs` | Remover `using static Domain.Movidesk.Dtos.Tickets.Ticket;` (não é usado) |

`Client : Person` continua igual. Os campos de endereço que vêm em `clients[]` são ignorados, porque não são
gravados (modelagem 4.2). `PersonWithAddress` continua sendo o tipo de `expenses[].createdBy`.

`Ticket.cs` fica assim no trecho das listas:
```csharp
public Person owner { get; init; }
public Person createdBy { get; init; }
public Person slaSolutionChangedBy { get; init; }

public List<string> tags { get; init; }
public List<CustomFieldValue> customFieldValues { get; init; }
public List<Asset> assets { get; init; }
public List<ParentChildTicket> childrenTickets { get; init; }
public List<ParentChildTicket> parentTickets { get; init; }
public List<Client> clients { get; init; }
public List<Action> actions { get; init; }
public List<OwnerHistory> ownerHistories { get; init; }
public List<StatusHistory> statusHistories { get; init; }
```

---

## 5. Entidades (`Entities/Reads/Tickets`)

### 5.1 Regras para todas
- **Nome e tipo de cada propriedade = coluna do `Script002`**, inclusive maiúsculas/minúsculas. O bulk insert mapeia
  por nome; uma propriedade sem coluna dá o erro `ColumnMapping does not match`.
- `integrated_or_updated_at` e `doc_company` em todas, recebidos pelo construtor:
  **o mesmo `integrated_or_updated_at` para o ticket e todas as filhas e netas do lote** (modelagem 4.8). Nenhuma
  entidade calcula o próprio horário.
- Listas filhas com `[SkipProperty]` e inicializadas com `= []`.
- Construtor vazio (usado pelo `CreateSystemDataTable`) + construtor que recebe o DTO.
- Textos com tamanho na coluna passam por `ConverterValidatorsExtensions.StringLengthValidation(valor, tamanho)`,
  para nunca dar erro de truncamento no bulk insert. Colunas `nvarchar(max)` recebem o valor direto.

### 5.2 Datas, inteiros e horas que podem vir nulos (decidido: tipos anuláveis)
A API manda `null` em muitas datas de evento (`resolvedIn`, `closedIn`, `canceledIn`, `reopenedIn`, `slaSolutionDate`,
`slaResponseDate`...) e em tempos (`lifeTimeWorkingTime`, `chatTalkTime`, `permanencyTime*`).
`ConvertToDateTimeValidation` devolve `1990-01-01` e `ConvertToInt32Validation` devolve `0` nesses casos, então um ticket
aberto ficaria com "resolvido em 01/01/1990".

**Decisão:** usar tipos anuláveis (`DateTime?`, `int?`, `decimal?`, `TimeSpan?`) nessas colunas, que já aceitam
nulo. O `CreateSystemDataTable` já trata `Nullable<>` e o `FillSystemDataTable` grava `DBNull`. Basta um helper,
que fica no `Domain.Shared` para outros módulos poderem usar:

`src/Backend/Shared/Domain.Shared/Extensions/NullableConverters.cs`
```csharp
using Domain.Shared.Extensions;
using System.Globalization;

namespace Domain.Shared.Extensions
{
    public static class NullableConverters
    {
        public static DateTime? ToNullableDateTime(string? value) =>
            String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToDateTimeValidation(value);

        public static int? ToNullableInt32(string? value) =>
            String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToInt32Validation(value);

        public static decimal? ToNullableDecimal(string? value) =>
            String.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);

        public static TimeSpan? ToNullableTimeSpan(string? value) =>
            TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var t) ? t : null;
    }
}
```
- `decimal` com `InvariantCulture`, porque o Newtonsoft entrega `63089.171960499996` com ponto e a máquina está
  em pt-BR (o mesmo problema do `[Range]` no AfterSale).
- `TimeSpan` e não `TimeOnly` nas colunas `time`: o `SqlBulkCopy` converte `TimeSpan` para `time`.

A convenção do AfterSale (`1990-01-01` e `0`) **não** é usada no Movidesk.

As datas da Movidesk são UTC (documentação) e chegam sem `Z`; são gravadas como vêm.

### 5.3 `Ticket`
```csharp
using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Ticket
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long id { get; private set; }
        public string protocol { get; private set; }
        public int? type { get; private set; }
        public string subject { get; private set; }
        public string category { get; private set; }
        public string urgency { get; private set; }
        public string status { get; private set; }
        public string baseStatus { get; private set; }
        public string justification { get; private set; }
        public int? origin { get; private set; }
        public DateTime? createdDate { get; private set; }
        public string originEmailAccount { get; private set; }
        public string? ownerId { get; private set; }
        public string ownerTeam { get; private set; }
        public string? createdById { get; private set; }
        public int? serviceFirstLevelId { get; private set; }
        public string serviceFirstLevel { get; private set; }
        public string serviceSecondLevel { get; private set; }
        public string serviceThirdLevel { get; private set; }
        public string contactForm { get; private set; }
        public string cc { get; private set; }
        public DateTime? resolvedIn { get; private set; }
        public DateTime? reopenedIn { get; private set; }
        public DateTime? closedIn { get; private set; }
        public DateTime? canceledIn { get; private set; }
        public bool isDeleted { get; private set; }
        public DateTime? lastActionDate { get; private set; }
        public int? actionCount { get; private set; }
        public DateTime? lastUpdate { get; private set; }
        public int? lifetimeWorkingTime { get; private set; }
        public int? stoppedTime { get; private set; }
        public int? stoppedTimeWorkingTime { get; private set; }
        public bool resolvedInFirstCall { get; private set; }
        public string chatWidget { get; private set; }
        public string chatGroup { get; private set; }
        public int? chatTalkTime { get; private set; }
        public int? chatWaitingTime { get; private set; }
        public int? sequence { get; private set; }
        public string slaAgreement { get; private set; }
        public string slaAgreementRule { get; private set; }
        public int? slaSolutionTime { get; private set; }
        public int? slaResponseTime { get; private set; }
        public bool slaSolutionChangedByUser { get; private set; }
        public string? slaSolutionChangedById { get; private set; }
        public DateTime? slaSolutionDate { get; private set; }
        public bool slaSolutionDateIsPaused { get; private set; }
        public DateTime? slaResponseDate { get; private set; }
        public DateTime? slaRealResponseDate { get; private set; }
        public string jiraIssueKey { get; private set; }
        public int? redmineIssueId { get; private set; }

        [SkipProperty] public List<TicketTag> tags { get; private set; } = [];
        [SkipProperty] public List<Client> clients { get; private set; } = [];
        [SkipProperty] public List<Relationship> relationships { get; private set; } = [];
        [SkipProperty] public List<OwnerHistory> ownerHistories { get; private set; } = [];
        [SkipProperty] public List<StatusHistory> statusHistories { get; private set; } = [];
        [SkipProperty] public List<CustomFieldValue> customFieldValues { get; private set; } = [];
        [SkipProperty] public List<Asset> assets { get; private set; } = [];
        [SkipProperty] public List<Action> actions { get; private set; } = [];

        public Ticket() { }

        public Ticket(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Tickets.Ticket dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.id);
            protocol = ConverterValidatorsExtensions.StringLengthValidation(dto.protocol, 30);
            type = NullableConverters.ToNullableInt32(dto.type);
            subject = ConverterValidatorsExtensions.StringLengthValidation(dto.subject, 350);
            category = ConverterValidatorsExtensions.StringLengthValidation(dto.category, 128);
            urgency = ConverterValidatorsExtensions.StringLengthValidation(dto.urgency, 128);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 128);
            baseStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.baseStatus, 128);
            justification = ConverterValidatorsExtensions.StringLengthValidation(dto.justification, 128);
            origin = NullableConverters.ToNullableInt32(dto.origin);
            createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
            originEmailAccount = ConverterValidatorsExtensions.StringLengthValidation(dto.originEmailAccount, 128);
            ownerId = dto.owner?.id;
            ownerTeam = ConverterValidatorsExtensions.StringLengthValidation(dto.ownerTeam, 128);
            createdById = dto.createdBy?.id;
            serviceFirstLevelId = NullableConverters.ToNullableInt32(dto.serviceFirstLevelId);
            serviceFirstLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceFirstLevel, 1024);
            serviceSecondLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceSecondLevel, 1024);
            serviceThirdLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceThirdLevel, 1024);
            contactForm = ConverterValidatorsExtensions.StringLengthValidation(dto.contactForm, 128);
            cc = ConverterValidatorsExtensions.StringLengthValidation(dto.cc, 1024);
            resolvedIn = NullableConverters.ToNullableDateTime(dto.resolvedIn);
            reopenedIn = NullableConverters.ToNullableDateTime(dto.reopenedIn);
            closedIn = NullableConverters.ToNullableDateTime(dto.closedIn);
            canceledIn = NullableConverters.ToNullableDateTime(dto.canceledIn);
            isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
            lastActionDate = NullableConverters.ToNullableDateTime(dto.lastActionDate);
            actionCount = NullableConverters.ToNullableInt32(dto.actionCount);
            lastUpdate = NullableConverters.ToNullableDateTime(dto.lastUpdate);
            lifetimeWorkingTime = NullableConverters.ToNullableInt32(dto.lifeTimeWorkingTime);
            stoppedTime = NullableConverters.ToNullableInt32(dto.stoppedTime);
            stoppedTimeWorkingTime = NullableConverters.ToNullableInt32(dto.stoppedTimeWorkingTime);
            resolvedInFirstCall = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.resolvedInFirstCall);
            chatWidget = ConverterValidatorsExtensions.StringLengthValidation(dto.chatWidget, 128);
            chatGroup = ConverterValidatorsExtensions.StringLengthValidation(dto.chatGroup, 128);
            chatTalkTime = NullableConverters.ToNullableInt32(dto.chatTalkTime);
            chatWaitingTime = NullableConverters.ToNullableInt32(dto.chatWaitingTime);
            sequence = NullableConverters.ToNullableInt32(dto.sequence);
            slaAgreement = ConverterValidatorsExtensions.StringLengthValidation(dto.slaAgreement, 128);
            slaAgreementRule = ConverterValidatorsExtensions.StringLengthValidation(dto.slaAgreementRule, 128);
            slaSolutionTime = NullableConverters.ToNullableInt32(dto.slaSolutionTime);
            slaResponseTime = NullableConverters.ToNullableInt32(dto.slaResponseTime);
            slaSolutionChangedByUser = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.slaSolutionChangedByUser);
            slaSolutionChangedById = dto.slaSolutionChangedBy?.id;
            slaSolutionDate = NullableConverters.ToNullableDateTime(dto.slaSolutionDate);
            slaSolutionDateIsPaused = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.slaSolutionDateIsPaused);
            slaResponseDate = NullableConverters.ToNullableDateTime(dto.slaResponseDate);
            slaRealResponseDate = NullableConverters.ToNullableDateTime(dto.slaRealResponseDate);
            jiraIssueKey = ConverterValidatorsExtensions.StringLengthValidation(dto.jiraIssueKey, 64);
            redmineIssueId = NullableConverters.ToNullableInt32(dto.redmineIssueId);

            foreach (var tag in (dto.tags ?? []).Where(t => !String.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
                tags.Add(new TicketTag(doc_company, integrated_or_updated_at, id, tag));

            foreach (var client in (dto.clients ?? []).Where(c => !String.IsNullOrEmpty(c.id)))
                clients.Add(new Client(doc_company, integrated_or_updated_at, id, client));

            foreach (var parent in (dto.parentTickets ?? []).Where(p => !String.IsNullOrEmpty(p.id)))
                relationships.Add(new Relationship(doc_company, integrated_or_updated_at, id, "Parent", parent));

            foreach (var child in (dto.childrenTickets ?? []).Where(c => !String.IsNullOrEmpty(c.id)))
                relationships.Add(new Relationship(doc_company, integrated_or_updated_at, id, "Child", child));

            // históricos não têm id na API: numerados na ordem de changedDate (estável, o histórico só cresce)
            var ownerHistoriesOrdered = (dto.ownerHistories ?? []).OrderBy(h => NullableConverters.ToNullableDateTime(h.changedDate)).ToList();
            for (int i = 0; i < ownerHistoriesOrdered.Count; i++)
                ownerHistories.Add(new OwnerHistory(doc_company, integrated_or_updated_at, id, i + 1, ownerHistoriesOrdered[i]));

            var statusHistoriesOrdered = (dto.statusHistories ?? []).OrderBy(h => NullableConverters.ToNullableDateTime(h.changedDate)).ToList();
            for (int i = 0; i < statusHistoriesOrdered.Count; i++)
                statusHistories.Add(new StatusHistory(doc_company, integrated_or_updated_at, id, i + 1, statusHistoriesOrdered[i]));

            foreach (var customFieldValue in dto.customFieldValues ?? [])
                customFieldValues.Add(new CustomFieldValue(doc_company, integrated_or_updated_at, id, customFieldValue));

            foreach (var asset in (dto.assets ?? []).Where(a => !String.IsNullOrEmpty(a.id)))
                assets.Add(new Asset(doc_company, integrated_or_updated_at, id, asset));

            foreach (var action in dto.actions ?? [])
                actions.Add(new Action(doc_company, integrated_or_updated_at, id, action));
        }
    }
}
```
- Filtrar itens sem chave (`id` vazio) evita linhas que o merge descartaria como "withou pk".
- Tags repetidas são descartadas aqui (`Distinct` sem diferenciar maiúsculas/minúsculas, como o SQL Server), senão
  violam a PK.
- O `for (int i = 0; i <= Count; i++)` do código atual lia um item além do fim da lista; aqui não há mais índice
  manual nas listas.

### 5.4 Filhas do ticket

Todas seguem o mesmo molde: `integrated_or_updated_at`, `doc_company`, `ticketId`, e os campos da coluna.

```csharp
public class TicketTag
{
    public DateTime integrated_or_updated_at { get; private set; }
    public string doc_company { get; private set; }
    public long ticketId { get; private set; }
    public string tag { get; private set; }

    public TicketTag() { }

    public TicketTag(string doc_company, DateTime integrated_or_updated_at, long ticketId, string tag)
    {
        this.integrated_or_updated_at = integrated_or_updated_at;
        this.doc_company = doc_company;
        this.ticketId = ticketId;
        this.tag = ConverterValidatorsExtensions.StringLengthValidation(tag, 128);
    }
}

public class Client
{
    public DateTime integrated_or_updated_at { get; private set; }
    public string doc_company { get; private set; }
    public long ticketId { get; private set; }
    public string clientId { get; private set; }
    public string? organizationId { get; private set; }
    public bool isDeleted { get; private set; }

    public Client() { }

    public Client(string doc_company, DateTime integrated_or_updated_at, long ticketId, Dtos.Reads.Tickets.Client dto)
    {
        this.integrated_or_updated_at = integrated_or_updated_at;
        this.doc_company = doc_company;
        this.ticketId = ticketId;
        clientId = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
        organizationId = dto.organization?.id;
        isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
    }
}

public class Relationship
{
    public DateTime integrated_or_updated_at { get; private set; }
    public string doc_company { get; private set; }
    public long ticketId { get; private set; }
    public long relatedTicketId { get; private set; }
    public string relationType { get; private set; }
    public string subject { get; private set; }
    public bool isDeleted { get; private set; }

    public Relationship() { }

    public Relationship(string doc_company, DateTime integrated_or_updated_at, long ticketId, string relationType, Dtos.Reads.Tickets.ParentChildTicket dto)
    {
        this.integrated_or_updated_at = integrated_or_updated_at;
        this.doc_company = doc_company;
        this.ticketId = ticketId;
        this.relationType = relationType;
        relatedTicketId = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.id);
        subject = ConverterValidatorsExtensions.StringLengthValidation(dto.subject, 350);
        isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
    }
}
```

As demais, no mesmo molde (propriedade → origem no DTO):

| Entidade (arquivo) | Construtor | Propriedades |
|---|---|---|
| `OwnerHistory` | `(doc_company, integrated_or_updated_at, ticketId, int id, Dtos...OwnerHistory dto)` | `id`; `ownerTeam` (128); `ownerId = dto.owner?.id`; `permanencyTimeFullTime`, `permanencyTimeWorkingTime` → `decimal?` (`ToNullableDecimal`); `changedById = dto.changedBy?.id`; `changedDate` → `DateTime?` |
| `StatusHistory` | `(doc_company, integrated_or_updated_at, ticketId, int id, Dtos...StatusHistory dto)` | `id`; `status`, `justification` (128); `permanencyTime*` → `decimal?`; `changedById`; `changedDate` → `DateTime?`. O arquivo atual tem `ticketdId` (com "d" a mais); corrija para `ticketId`. |
| `CustomFieldValue` | `(doc_company, integrated_or_updated_at, ticketId, Dtos...CustomFieldValue dto)` | `customFieldId`, `customFieldRuleId`, `line` → `int` (`ConvertToInt32Validation`); `value` (sem corte, `nvarchar(max)`); `[SkipProperty] List<CustomFieldValueItem> items = []`, criada com `id = i + 1` |
| `CustomFieldValueItem` | `(doc_company, integrated_or_updated_at, ticketId, customFieldId, customFieldRuleId, line, int id, Dtos...CustomFieldValueItem dto)` | `personId`, `clientId` → **`string`** (64; a API manda texto, não `int`); `team` (128); `customFieldItem` (256); `storageFileGuid` (36); `fileName` (255) |
| `Asset` | `(doc_company, integrated_or_updated_at, ticketId, Dtos...Asset dto)` | `id` (64); `name`, `label`, `serialNumber`, `categoryFirstLevel/Second/Third` (128); `isDeleted` → `bool`. Sai `List<AssetCategoryFull> categoryFull`. |

> O `CustomFieldValue` atual nunca inicializa `items` e quebraria com `NullReferenceException` no primeiro item.
> Com `= []` isso some.

### 5.5 Ações e netas
```csharp
public class Action
{
    public DateTime integrated_or_updated_at { get; private set; }
    public string doc_company { get; private set; }
    public long ticketId { get; private set; }
    public int id { get; private set; }
    public int? type { get; private set; }
    public int? origin { get; private set; }
    public string? description { get; private set; }
    public string? htmlDescription { get; private set; }
    public string status { get; private set; }
    public string justification { get; private set; }
    public DateTime? createdDate { get; private set; }
    public string? createdById { get; private set; }
    public bool isDeleted { get; private set; }

    [SkipProperty] public List<ActionTag> tags { get; private set; } = [];
    [SkipProperty] public List<TimeAppointment> timeAppointments { get; private set; } = [];
    [SkipProperty] public List<Expense> expenses { get; private set; } = [];
    [SkipProperty] public List<Attachment> attachments { get; private set; } = [];

    public Action() { }

    public Action(string doc_company, DateTime integrated_or_updated_at, long ticketId, Dtos.Reads.Tickets.Action dto)
    {
        this.integrated_or_updated_at = integrated_or_updated_at;
        this.doc_company = doc_company;
        this.ticketId = ticketId;

        id = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.id);
        type = NullableConverters.ToNullableInt32(dto.type);
        origin = NullableConverters.ToNullableInt32(dto.origin);
        description = dto.description;          // nvarchar(max)
        htmlDescription = dto.htmlDescription;  // só vem na busca por id; na listagem fica nulo
        status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 128);
        justification = ConverterValidatorsExtensions.StringLengthValidation(dto.justification, 128);
        createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
        createdById = dto.createdBy?.id;
        isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);

        foreach (var tag in (dto.tags ?? []).Where(t => !String.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
            tags.Add(new ActionTag(doc_company, integrated_or_updated_at, ticketId, id, tag));

        foreach (var appointment in (dto.timeAppointments ?? []).Where(a => !String.IsNullOrEmpty(a.id)))
            timeAppointments.Add(new TimeAppointment(doc_company, integrated_or_updated_at, ticketId, id, appointment));

        foreach (var expense in (dto.expenses ?? []).Where(e => !String.IsNullOrEmpty(e.id)))
            expenses.Add(new Expense(doc_company, integrated_or_updated_at, ticketId, id, expense));

        foreach (var attachment in (dto.attachments ?? []).Where(a => !String.IsNullOrEmpty(a.path)))
            attachments.Add(new Attachment(doc_company, integrated_or_updated_at, ticketId, id, attachment));
    }
}
```

| Entidade | Construtor | Propriedades |
|---|---|---|
| `ActionTag` | `(doc_company, integrated_or_updated_at, ticketId, int actionId, string tag)` | `actionId`, `tag` (128). Saem `id` e `ticketId string`. |
| `TimeAppointment` | `(doc_company, integrated_or_updated_at, ticketId, int actionId, Dtos...TimeAppointment dto)` | `id` → **`long`**; `activity` (128); `date` → `DateTime?`; `periodStart`, `periodEnd`, `workTime` → **`TimeSpan?`** (`ToNullableTimeSpan`); `accountedTime` → `decimal?`; `workTypeName` (128); `createdById = dto.createdBy?.id`; `createdByTeamId = ToNullableInt32(dto.createdByTeam?.id)`; `createdByTeamName` (128) |
| `Expense` | `(doc_company, integrated_or_updated_at, ticketId, int actionId, Dtos...Expense dto)` | `id` → **`long`**; `type`, `serviceReport` (128); `createdById`; `createdByTeamId`, `createdByTeamName`; `date` → `DateTime?`; `quantity` → `int?`; `value` → `decimal?` |
| `Attachment` | `(doc_company, integrated_or_updated_at, ticketId, int actionId, Dtos...Attachment dto)` | `path` (255, chave); `fileName` (255); `createdById`; `createdDate` → `DateTime?`. Sai `id`. |

### 5.6 Arquivos que saem
`TicketServiceFull.cs`, `AssetCategoryFull.cs`, `SatisfactionSurveyResponse.cs`, `CreatedByTeam.cs`,
`ParentChildTicket.cs` (substituído por `Relationship.cs`).

---

## 6. Validadores (`Application.Movidesk/Validations/Ticket`)

Os validadores só **registram mensagens** no log (o service não descarta o ticket), então devem refletir os tamanhos
do `Script002`. Hoje o `TicketValidator` tem regras contraditórias para o mesmo campo (`subject` com no máximo 350 **e**
no máximo 60; `protocol` 30 **e** 60) e limites que não batem com a tabela (`cc` 300, `ownerTeam` 50...).

- **`TicketValidator`:** uma regra por campo, com os tamanhos da tabela: `protocol` 30, `subject` 350, `category`/
  `urgency`/`status`/`baseStatus`/`justification`/`originEmailAccount`/`ownerTeam`/`contactForm`/`chatWidget`/
  `chatGroup`/`slaAgreement`/`slaAgreementRule` 128, `serviceFirstLevel/Second/Third` e `cc` 1024, `jiraIssueKey` 64.
  `id` com `MustBeValidInt64()` em vez de `MaximumLength(10)`. Remover as regras de `serviceFull` e de `tags`
  (somavam a lista inteira numa string). Manter `MustBeValid*` dos números, datas e booleanos.
- **`ActionValidator`:** remover `RuleForEach(x => x.customFieldValues)` (a ação não tem essa lista; não compila).
  `description` sem `MaximumLength(300)`: a coluna é `nvarchar(max)` e uma ação real já tem 3.744 caracteres.
  `status`/`justification` 128.
- **`CreatedbyValidator`, `ClientValidator`, `AttachmentValidator`, `CustomfieldValueValidator`, `ItemValidator`:**
  só a troca de namespace; conferir os tamanhos (`id` 64, `fileName`/`path` 255, `customFieldItem` 256).

O `TicketValidator` passa a ser registrado na injeção de dependência (seção 10); hoje só os de pessoas são.

---

## 7. Queries, interface e repositório

### 7.1 Queries (`IMovideskCommandHandler` / `MovideskCommandHandler`)
```csharp
public string CreateGetCompanysQuery();                                   // seção 3
public string CreateGetTicketsLastUpdateQuery(string doc_company);
public string CreateGetTicketsByIdsQuery(string doc_company, List<long> ids);
```
```csharp
public string CreateGetTicketsLastUpdateQuery(string doc_company)
{
    return @$"SELECT MAX(LASTUPDATE)
             FROM MOVIDESK.MOVIDESKTICKETS
             WHERE DOC_COMPANY = '{doc_company}'";
}

public string CreateGetTicketsByIdsQuery(string doc_company, List<long> ids)
{
    // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
    return @$"SELECT DOC_COMPANY, ID, LASTUPDATE
             FROM MOVIDESK.MOVIDESKTICKETS
             WHERE DOC_COMPANY = '{doc_company}'
             AND ID IN ({String.Join(", ", ids)})";
}
```

### 7.2 Interfaces

Mesma divisão do AfterSale (`IAfterSaleRepository` para o que é do módulo + um repositório por recurso):

| Interface | Implementação | Métodos | Usada por |
|---|---|---|---|
| `IMovideskRepository` | `MovideskRepository` | `GetCompanys` | os três services |
| `IMovideskTicketsRepository` (nova) | `MovideskTicketsRepository` (nova) | `GetTicketsLastUpdate`, `GetTicketsByIds`, `BulkInsertIntoMovideskTickets` | `MovideskTicketsService` |
| `IMovideskPeopleRepository` (nova) | `MovideskPeopleRepository` (nova) | nenhum ainda (7.4) | `MovideskPeopleService` |
| `IMovideskServicesRepository` (nova) | `MovideskServicesRepository` (nova) | nenhum ainda (7.4) | `MovideskServicesService` |

`Domain.Movidesk/Abstractions/Repositorys/IMovideskRepository.cs`
```csharp
using Domain.Movidesk.Entities.Parameters;

namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
    }
}
```

`Domain.Movidesk/Abstractions/Repositorys/IMovideskTicketsRepository.cs`
```csharp
namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskTicketsRepository
    {
        public Task<DateTime?> GetTicketsLastUpdate(string doc_company);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Ticket?>> GetTicketsByIds(string doc_company, List<long> ids);
        public Task<bool> BulkInsertIntoMovideskTickets(List<Domain.Movidesk.Entities.Reads.Tickets.Ticket> data, Guid? parentExecutionGUID);
    }
}
```
Os métodos antigos (`GetBlankTicketsFromDatabase`, `InsertInvoice`, `UpdateTicketFromDatabase`...) e o
`BulkInsertIntoTableRaw` de pessoas saem (seção 9).

### 7.3 Repositórios

`Infrastructure.Movidesk/Repositorys/MovideskRepository.cs` (fica só com as empresas)
```csharp
using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Parameters;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskRepository : IMovideskRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _movideskCommandHandler.CreateGetCompanysQuery();
            return await _coreRepository.GetRecords<Company>(sql);
        }
    }
}
```

`Infrastructure.Movidesk/Repositorys/MovideskTicketsRepository.cs` (novo)
```csharp
using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Reads.Tickets;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskTicketsRepository : IMovideskTicketsRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskTicketsRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<DateTime?> GetTicketsLastUpdate(string doc_company)
        {
            string sql = _movideskCommandHandler.CreateGetTicketsLastUpdateQuery(doc_company);
            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Ticket?>> GetTicketsByIds(string doc_company, List<long> ids)
        {
            if (ids.Count == 0)
                return [];

            string sql = _movideskCommandHandler.CreateGetTicketsByIdsQuery(doc_company, ids);
            return await _coreRepository.GetRecords<Ticket>(sql);
        }

        public async Task<bool> BulkInsertIntoMovideskTickets(List<Ticket> data, Guid? parentExecutionGUID)
        {
            var actions = data.SelectMany(x => x.actions).ToList();
            var customFieldValues = data.SelectMany(x => x.customFieldValues).ToList();

            // ordem = ordem dos merges: pai, nível do meio, depois filhas e netas (FKs e condição de exclusão)
            var tables = new List<(System.Data.DataTable table, System.Collections.IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Ticket(), tableName: "MovideskTickets"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Entities.Reads.Tickets.Action(), tableName: "MovideskTicketsActions"), actions),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValue(), tableName: "MovideskTicketsCustomFieldValues"), customFieldValues),
                (_coreRepository.CreateSystemDataTable(entity: new TicketTag(), tableName: "MovideskTicketsTags"), data.SelectMany(x => x.tags).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Client(), tableName: "MovideskTicketsClients"), data.SelectMany(x => x.clients).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Relationship(), tableName: "MovideskTicketsRelationships"), data.SelectMany(x => x.relationships).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OwnerHistory(), tableName: "MovideskTicketsOwnerHistories"), data.SelectMany(x => x.ownerHistories).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new StatusHistory(), tableName: "MovideskTicketsStatusHistories"), data.SelectMany(x => x.statusHistories).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Asset(), tableName: "MovideskTicketsAssets"), data.SelectMany(x => x.assets).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValueItem(), tableName: "MovideskTicketsCustomFieldValuesItems"), customFieldValues.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ActionTag(), tableName: "MovideskTicketsActionsTags"), actions.SelectMany(x => x.tags).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new TimeAppointment(), tableName: "MovideskTicketsActionsTimeAppointments"), actions.SelectMany(x => x.timeAppointments).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Expense(), tableName: "MovideskTicketsActionsExpenses"), actions.SelectMany(x => x.expenses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Attachment(), tableName: "MovideskTicketsActionsAttachments"), actions.SelectMany(x => x.attachments).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                FillTable(table, rows);
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge roda mesmo para tabelas sem linhas no lote: é ele que apaga as filhas removidas
            // (ex.: o ticket ficou sem tags -> nenhuma linha em untreated.MovideskTicketsTags, mas as tags antigas saem)
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("movidesk", table.TableName, parentExecutionGUID);

            return true;
        }

        private void FillTable(System.Data.DataTable table, System.Collections.IList rows)
        {
            // FillSystemDataTable é genérico em List<TEntity>; o cast mantém o tipo concreto de cada lista
            _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
        }
    }
}
```

**Atenção ao `FillTable`:** `FillSystemDataTable` lê as propriedades de `data.FirstOrDefault().GetType()`, então
uma `List<object>` funciona (o tipo real vem do primeiro item). Se preferir não depender disso, troque a lista de
tuplas por 14 pares explícitos de `FillSystemDataTable` + `BulkInsertIntoTableRaw`, como no
`BulkInsertIntoAfterSaleReverses`; o importante é **manter a ordem dos merges** acima.

**Por que chamar o merge de todas as 14 tabelas sempre:** o `when not matched by source` da filha só roda dentro do
merge dela. Se o repositório pulasse a tabela de tags por não ter linhas no lote, as tags removidas de um ticket que
ficou sem nenhuma tag nunca sairiam.

Use a sobrecarga `BulkInsertIntoTableRaw(table, "untreated")`; a sem schema está marcada como `[Obsolete]`.

### 7.4 Repositórios de pessoas e serviços (estrutura para o futuro)

Criados já agora, vazios, para que pessoas e serviços sejam implementados depois sem mexer na injeção de dependência
nem nos construtores. Seguem o mesmo molde do `MovideskTicketsRepository`.

`Domain.Movidesk/Abstractions/Repositorys/IMovideskPeopleRepository.cs`
```csharp
namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskPeopleRepository
    {
        // GetPeopleByIds, BulkInsertIntoMovideskPeople... entram quando as tabelas de pessoas forem modeladas
    }
}
```

`Domain.Movidesk/Abstractions/Repositorys/IMovideskServicesRepository.cs`
```csharp
namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskServicesRepository
    {
        // GetServicesByIds, BulkInsertIntoMovideskServices... entram quando as tabelas de serviços forem modeladas
    }
}
```

`Infrastructure.Movidesk/Repositorys/MovideskPeopleRepository.cs`
```csharp
using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskPeopleRepository : IMovideskPeopleRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskPeopleRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);
    }
}
```

`Infrastructure.Movidesk/Repositorys/MovideskServicesRepository.cs`
```csharp
using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskServicesRepository : IMovideskServicesRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskServicesRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);
    }
}
```
Os campos `_movideskCommandHandler` e `_coreRepository` ainda não são usados; o compilador só avisa (warning), não dá
erro. Eles ficam para os métodos futuros, como nos outros repositórios.

---

## 8. Service (`MovideskTicketsService.GetTickets`)

> **Atualizado em 21/10/2026:** a busca em uma chamada por página (`TicketsExpand`, seções 8.2 e 8.4) foi substituída
> pela busca em duas fases, com o `$expand` dividido em grupos. Ver
> [IMPLEMENTACAO-MOVIDESK-GETTICKETS-EXPAND-EM-GRUPOS.md](IMPLEMENTACAO-MOVIDESK-GETTICKETS-EXPAND-EM-GRUPOS.md).

`Application.Movidesk/Services/MovideskService.cs` passa a ser `MovideskTicketsService.cs` (`git mv`), com a interface
`IMovideskTicketsService`. A divisão dos outros métodos está na seção 9.

### 8.1 Construtor
```csharp
private readonly IApiClient _apiClient;
private readonly ILoggerService _logger;
private readonly IMemoryCache _cache;
private readonly IMovideskRepository _movideskRepository;
private readonly IMovideskTicketsRepository _movideskTicketsRepository;
private readonly IValidator<Domain.Movidesk.Dtos.Reads.Tickets.Ticket> _ticketValidator;

private const string TicketsKey = "Movidesk:Tickets";

// limite da API: 10 requisições por minuto -> 1 requisição a cada 6 s
private const int RequestIntervalInMilliseconds = 6 * 1000;
private const int PageSize = 25;

public MovideskTicketsService(
    [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
    ILoggerService logger,
    IMovideskRepository movideskRepository,
    IMovideskTicketsRepository movideskTicketsRepository,
    IValidator<Domain.Movidesk.Dtos.Reads.Tickets.Ticket> ticketValidator,
    IMemoryCache cache
) =>
    (_apiClient, _logger, _movideskRepository, _movideskTicketsRepository, _ticketValidator, _cache) =
    (apiClient, logger, movideskRepository, movideskTicketsRepository, ticketValidator, cache);
```
O validador de pessoas vai para o `MovideskPeopleService` (seção 9).

### 8.2 `$select` e `$expand`
`$select` é obrigatório na listagem. Os campos são os do ticket (sem listas); as listas vêm pelo `$expand`.
```csharp
private const string TicketsSelect =
    "id,protocol,type,subject,category,urgency,status,baseStatus,justification,origin,createdDate," +
    "isDeleted,originEmailAccount,ownerTeam,serviceFirstLevelId,serviceFirstLevel,serviceSecondLevel," +
    "serviceThirdLevel,contactForm,tags,cc,resolvedIn,closedIn,canceledIn,actionCount,lifeTimeWorkingTime," +
    "stoppedTime,stoppedTimeWorkingTime,resolvedInFirstCall,chatWidget,chatGroup,chatTalkTime,chatWaitingTime," +
    "sequence,slaAgreement,slaAgreementRule,slaSolutionTime,slaResponseTime,slaSolutionChangedByUser," +
    "slaSolutionDate,slaSolutionDateIsPaused,jiraIssueKey,redmineIssueId,reopenedIn,lastActionDate,lastUpdate," +
    "slaResponseDate,slaRealResponseDate";

private const string TicketsExpand =
    "owner,createdBy,slaSolutionChangedBy," +
    "clients($expand=organization)," +
    "actions($expand=createdBy,timeAppointments($expand=createdBy,createdByTeam),expenses($expand=createdBy),attachments($expand=createdBy))," +
    "parentTickets,childrenTickets," +
    "ownerHistories($expand=owner,changedBy),statusHistories($expand=changedBy)," +
    "customFieldValues($expand=items),assets";
```
> **Validar na primeira chamada:** a documentação só mostra `$expand` aninhado de um nível
> (`actions($expand=timeAppointments($expand=createdBy))`). Se a API recusar algum trecho, compare com a busca por
> id (`/tickets?id=`), que traz tudo expandido sem `$expand` (ver `Responsed Jsons/Ticket/GetTicketById`).
> `htmlDescription` não vem na listagem.

### 8.3 Janela de busca (`lastUpdate`)
O filtro atual (`createdDate` dos últimos 7 dias) nunca relê um ticket antigo que foi atualizado. Para sincronizar,
o filtro é por `lastUpdate`:
- **Com dados:** `lastUpdate ge (MAX(lastUpdate) da empresa − 1 hora)`. A hora de folga cobre o atraso de replicação
  que a documentação avisa ("podem levar alguns minutos"). Os tickets repetidos na folga são descartados pelo cache
  (8.4) ou apenas reprocessados pelo merge.
- **Banco vazio:** últimos 89 dias, porque `/tickets` só devolve `lastUpdate` de até 90 dias. A carga dos mais antigos
  é por `/tickets/past` e fica fora deste guia.
- `lastUpdate` é UTC na API e é gravado assim; o filtro também é montado em UTC (`...z`).

### 8.4 Código
```csharp
/// <summary>
/// sincroniza os tickets atualizados desde a última execução, para cada empresa
/// </summary>
public async Task<bool?> GetTickets()
{
    _logger
        .Clear()
        .AddLog(EnumJob.MovideskGetTickets);

    var totalInseridos = 0;
    var companys = await _movideskRepository.GetCompanys();

    foreach (var company in companys)
    {
        if (company?.doc_company is null)
            continue;

        // um horário por lote: ticket, filhas e netas gravam o mesmo valor (condição de exclusão do merge)
        var batchTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        var lastUpdate = await _movideskTicketsRepository.GetTicketsLastUpdate(company.doc_company);
        var since = lastUpdate.HasValue ? lastUpdate.Value.AddHours(-1) : DateTime.UtcNow.AddDays(-89);

        var tickets = new List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>();
        var ticketsJson = new Dictionary<long, string>();
        var skip = 0;

        while (true)
        {
            var response = await _apiClient.GetAsync(
                route: "tickets",
                queryParams: new Dictionary<string, string>
                {
                    { "token", company.Token.ToString() },
                    { "$select", TicketsSelect },
                    { "$expand", TicketsExpand },
                    { "$filter", $"lastUpdate ge {since:yyyy-MM-ddTHH:mm:ss}.00z" },
                    { "$orderby", "id asc" },
                    { "$top", PageSize.ToString() },
                    { "$skip", skip.ToString() }
                }
            );

            List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>? page;

            try
            {
                page = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>>(response ?? String.Empty);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                // a API devolve um objeto de erro (ou 429 Too many failed requests) em vez da lista
                _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, skip {skip}): {response}");
                break;
            }

            if (page is null || page.Count == 0)
                break;

            tickets.AddRange(page);

            foreach (var item in Newtonsoft.Json.Linq.JArray.Parse(response!))
            {
                var id = ConverterValidatorsExtensions.ConvertToInt64Validation((string?)item["id"]);
                if (id != 0)
                    ticketsJson[id] = item.ToString();
            }

            if (page.Count < PageSize)
                break;

            skip += PageSize;
            Thread.Sleep(RequestIntervalInMilliseconds);
        }

        if (tickets.Count == 0)
            continue;

        // cache de estado: id -> ticket gravado (com lastUpdate), por empresa
        var ticketsCache = await _cache.GetOrCreateAsync($"{TicketsKey}:{company.doc_company}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            var list = await _movideskTicketsRepository.GetTicketsByIds(
                company.doc_company,
                tickets.Select(x => ConverterValidatorsExtensions.ConvertToInt64Validation(x.id)).Distinct().ToList()
            );

            return list
                .Where(x => x != null)
                .GroupBy(x => x!.id)
                .ToDictionary(g => g.Key, g => g.First()!);
        });

        // mantém só os novos ou com lastUpdate mais recente que o gravado
        tickets.RemoveAll(x =>
            !string.IsNullOrEmpty(x.id) &&
            ticketsCache.TryGetValue(ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
            NullableConverters.ToNullableDateTime(x.lastUpdate) <= y.lastUpdate
        );

        var _listSomenteNovos = new List<Domain.Movidesk.Entities.Reads.Tickets.Ticket>();

        foreach (var ticket in tickets.GroupBy(x => x.id).Select(g => g.Last()))
        {
            var validations = _ticketValidator.Validate(ticket);

            if (validations.Errors.Count > 0)
            {
                var message = String.Empty;
                validations.Errors.ForEach(s => message += $"ticket {ticket.id}: {s.ErrorMessage}\n");
                _logger.AddMessage(message);
            }

            _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.Tickets.Ticket(company.doc_company, batchTime, ticket));
        }

        if (_listSomenteNovos.Count == 0)
            continue;

        await _movideskTicketsRepository.BulkInsertIntoMovideskTickets(_listSomenteNovos, _logger.GetExecutionGuid());

        foreach (var record in _listSomenteNovos)
        {
            if (ticketsJson.TryGetValue(record.id, out var json))
                _logger.AddRecord(record.id.ToString(), json);

            ticketsCache[record.id] = record;
        }

        totalInseridos += _listSomenteNovos.Count;
    }

    _logger.AddMessage(
        $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
    );

    _logger.SetLogEndDate();
    await _logger.CommitAllChanges();

    return true;
}
```
Pontos do código:
- **`queryParams`** do `IApiClient` faz o encode da URL (espaços e `$` do OData). O código atual montava a URL na mão.
- **Paginação por `$skip` com `$orderby=id`**: estável enquanto a página é lida. Um ticket atualizado no meio da
  leitura pode mudar de posição; ele volta na próxima execução, porque o filtro é por `lastUpdate` com folga.
- **6 s entre páginas** para respeitar 10 requisições por minuto. Três erros seguidos bloqueiam a API por 60 s
  (depois 120 s e 300 s); por isso, num erro, a empresa é interrompida em vez de tentar de novo no laço.
- **Cache (padrão §3.2):** carregado com os ids do lote. A comparação usa `lastUpdate` convertido, como o
  `updated_at` do `GetReverses`.
- **`GroupBy(id).Last()`**: se o mesmo ticket vier em duas páginas, fica a última versão.
- **Retorno `Task<bool?>`**, como no AfterSale (interface `IMovideskTicketsService`, seção 9).

`using`s do service:
```csharp
using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Enums;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
```

---

## 9. Divisão em três services

O `MovideskService` atual mistura tickets, pessoas, serviços e faturas. Ele vira três services, um por recurso da API,
como o AfterSale (`AfterSaleMeService`, `AfterSaleReversesService`, `AfterSaleRefundsService`):

| Service (arquivo em `Application.Movidesk/Services/`) | Interface (`Application.Movidesk/Abstractions/Services/`) | Métodos | Estado |
|---|---|---|---|
| `MovideskTicketsService.cs` (`git mv` do `MovideskService.cs`) | `IMovideskTicketsService` | `GetTicketById`, `GetTickets` | `GetTickets` implementado (seção 8); `GetTicketById` continua stub |
| `MovideskPeopleService.cs` (novo) | `IMovideskPeopleService` | `GetPersonById`, `GetPersons` | Os dois como stub |
| `MovideskServicesService.cs` (novo) | `IMovideskServicesService` | `GetServiceById`, `GetServices` | Os dois como stub |

`IMovideskService.cs` é renomeado com `git mv` para `IMovideskTicketsService.cs`, e as outras duas interfaces são
arquivos novos.

### 9.1 Interfaces
```csharp
namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskTicketsService
    {
        public Task<bool?> GetTicketById();
        public Task<bool?> GetTickets();
    }

    public interface IMovideskPeopleService
    {
        public Task<bool?> GetPersonById();
        public Task<bool?> GetPersons();
    }

    public interface IMovideskServicesService
    {
        public Task<bool?> GetServiceById();
        public Task<bool?> GetServices();
    }
}
```
Uma interface por arquivo, com o nome da interface.

### 9.2 Por que pessoas e serviços ficam como stub
- **`GetPersons`:** grava em `MovideskPersons*`, cujas tabelas vinham do `people.sql`, removido. Usa `Models.Persons`,
  que não existe, e `GetTokenAsync`, que sai (seção 3). Volta quando o service de pessoas for modelado, como foi feito
  com tickets.
- **`GetServices`:** hoje só lê a API e descarta o resultado (não grava nada), e também depende do `GetTokenAsync`.
  Fica como stub até existir tabela de serviços.

Os dois services nascem com os métodos ainda como stub, mas **já recebendo as dependências** (API, logger, empresas e
o próprio repositório da seção 7.4), para que a implementação futura só preencha os métodos:
```csharp
using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Movidesk.Services
{
    public class MovideskPeopleService : IMovideskPeopleService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMovideskRepository _movideskRepository;
        private readonly IMovideskPeopleRepository _movideskPeopleRepository;

        public MovideskPeopleService(
            [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMovideskRepository movideskRepository,
            IMovideskPeopleRepository movideskPeopleRepository
        ) =>
            (_apiClient, _logger, _movideskRepository, _movideskPeopleRepository) =
            (apiClient, logger, movideskRepository, movideskPeopleRepository);

        public Task<bool?> GetPersonById() => throw new NotImplementedException();

        public Task<bool?> GetPersons() => throw new NotImplementedException();
    }
}
```
```csharp
using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Movidesk.Services
{
    public class MovideskServicesService : IMovideskServicesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMovideskRepository _movideskRepository;
        private readonly IMovideskServicesRepository _movideskServicesRepository;

        public MovideskServicesService(
            [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMovideskRepository movideskRepository,
            IMovideskServicesRepository movideskServicesRepository
        ) =>
            (_apiClient, _logger, _movideskRepository, _movideskServicesRepository) =
            (apiClient, logger, movideskRepository, movideskServicesRepository);

        public Task<bool?> GetServiceById() => throw new NotImplementedException();

        public Task<bool?> GetServices() => throw new NotImplementedException();
    }
}
```
Quando pessoas forem implementadas, o `MovideskPeopleService` recebe também o
`IValidator<Domain.Movidesk.Dtos.Person.Person>` e, se usar cache, o `IMemoryCache`, como o `MovideskTicketsService`.

### 9.3 Métodos de fatura: removidos
`BuildInvoicesFromTickets` (público e privado), `InsertTicket` e `UpdateTicket` saem da interface e do service, sem ir
para nenhum dos três. Eles usam `TicketAntigo`, `Invoice` e as tabelas `MovideskTicketAprovaFatura`/`MovideskAprovaFatura`,
e leem os campos adicionais do formulário antigo (modelagem 11). Uma rotina de fatura nova deve partir das tabelas novas.

O `MovideskRepository` perde junto `BulkInsertIntoTableRaw` (pessoas e tickets antigos) e os 6 métodos de fatura, e
fica só com `GetCompanys` (seção 7).

Os validadores de pessoas (`Validations/Person`) e as entidades de pessoas (`Entities/Person`) ficam, só com a troca
de namespace da seção 2.

---

## 10. Injeção de dependência e host

**`Infrastructure.Movidesk/DependencyInjection.cs`**
```csharp
using Application.Movidesk.Abstractions.Handlers.Commands;
using Application.Movidesk.Abstractions.Services;
using Application.Movidesk.Handlers.Commands;
using Application.Movidesk.Services;
using Application.Movidesk.Validations.Ticket;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Infrastructure.Movidesk.Repositorys;
using Infrastructure.Shared.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Movidesk.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedMovideskServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<IApiClient>("MovideskAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "MovideskAPI"));
            services.AddHttpClient("MovideskAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Movidesk:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IMovideskTicketsService, MovideskTicketsService>();
            services.AddScoped<IMovideskTicketsRepository, MovideskTicketsRepository>();

            services.AddScoped<IMovideskPeopleService, MovideskPeopleService>();
            services.AddScoped<IMovideskPeopleRepository, MovideskPeopleRepository>();

            services.AddScoped<IMovideskServicesService, MovideskServicesService>();
            services.AddScoped<IMovideskServicesRepository, MovideskServicesRepository>();

            services.AddScoped<IMovideskRepository, MovideskRepository>();

            services.AddScoped<IMovideskCommandHandler, MovideskCommandHandler>();

            // registra todos os validadores do assembly (tickets e pessoas), como o AfterSale
            services.AddValidatorsFromAssemblyContaining<TicketValidator>();

            return services;
        }
    }
}
```
Os 5 `AddValidatorsFromAssemblyContaining` atuais registram o mesmo assembly 5 vezes; uma chamada basta.

**Host (`src/Backend/Hosts/Hangfire`)**
1. `Hangfire.csproj`: + `ProjectReference` para `Application.Movidesk` e `Infrastructure.Movidesk`, como os do AfterSale.
2. `Extensions/ServicesExtensions.cs`: descomentar `using Infrastructure.Movidesk.DependencyInjection;` e
   `builder.Services.AddScopedMovideskServices(builder.Configuration);`.
3. `Controllers/General/MovideskController.cs`: injetar só o `IMovideskTicketsService` e ativar o endpoint de tickets,
   como **GET** (mesma decisão tomada para as consultas do AfterSale):
   ```csharp
   using Application.Movidesk.Abstractions.Services;
   using Microsoft.AspNetCore.Authorization;
   using Microsoft.AspNetCore.Mvc;

   namespace Hangfire.Controllers.General
   {
       [ApiController]
       [Authorize]
   #if RELEASE
       [ApiExplorerSettings(IgnoreApi = true)]
   #endif
       [Route("MovideskJobs/Movidesk")]
       public class MovideskController : Controller
       {
           private readonly IMovideskTicketsService _movideskTicketsService;

           public MovideskController(IMovideskTicketsService movideskTicketsService) =>
               _movideskTicketsService = movideskTicketsService;

           [HttpGet("Tickets")]
           [Tags("MovideskTickets")]
           public async Task<ActionResult> Tickets()
           {
               var result = await _movideskTicketsService.GetTickets();

               return Ok($"Records integrated successfully.");
           }
       }
   }
   ```
   - Os endpoints `Persons` e `Services` continuam comentados; quando voltarem, injetam `IMovideskPeopleService` e
     `IMovideskServicesService`.
   - Os endpoints `Ticket` (webhook), `UpdateTicket` e `BuildInvoicesFromTickets` são **removidos** junto com os
     métodos de fatura (9.3).

`appsettings.json` já tem `Movidesk:BaseUrl = https://api.movidesk.com/public/v1/`.

**`EnumJob`** não muda: `MovideskGetTickets = 3000` e `MovideskGetPersons = 3001` já batem com o `Script005`.

---

## 11. Checklist

- [ ] 1. `.csproj` apontando para `Shared`, com versões
- [ ] 2. Namespaces trocados; DTOs e entidades em `Reads` (`git mv`): `Dtos/Reads/{Tickets,Person,Services}` e
      `Entities/Reads/{Tickets,Person}`
- [ ] 3. `Entities/Parameters/Company.cs` + `CreateGetCompanysQuery`
- [ ] 4. DTOs: `slaResponseTime`, `assets`, `storageFileGuid`/`fileName`; arquivos removidos
- [ ] 5. `NullableConverters` + 14 entidades com tipos anuláveis e `integrated_or_updated_at`/`doc_company` recebidos de fora
- [ ] 6. Validadores sem regras contraditórias; `ActionValidator` sem `customFieldValues`
- [ ] 7. Queries; `IMovideskRepository` só com empresas; `IMovideskTicketsRepository` com os 14 merges sempre, na ordem;
      `IMovideskPeopleRepository` e `IMovideskServicesRepository` vazios (7.4)
- [ ] 8. `MovideskTicketsService.GetTickets` com laço por empresa, `lastUpdate`, paginação de 6 s e cache
- [ ] 9. `MovideskPeopleService` e `MovideskServicesService` com as dependências no construtor e métodos stub;
      métodos de fatura removidos
- [ ] 10. DI, host e endpoint `GET MovideskJobs/Movidesk/Tickets`
- [ ] Compilar o host
- [ ] Inserir uma empresa em `movidesk.MovideskParameters` (o token não vai para o git; o `Script008` do AfterSale tem
      os tokens commitados e não deve ser seguido)
- [ ] Primeira execução: conferir o `$expand` (8.2) e comparar um ticket gravado com a busca por id

## 12. Decisões (aplicadas em 19/10/2026)

| # | Decisão | Onde |
|---|---|---|
| 1 | Tipos anuláveis (`DateTime?`, `int?`, `decimal?`, `TimeSpan?`) para valores que a API manda `null` | 5.2 |
| 2 | DTOs e entidades de ticket em `Dtos/Reads/Tickets` e `Entities/Reads/Tickets` | 2 |
| 3 | Métodos de fatura removidos (service, interface, repositório e controller) | 9.3, 10 |
| 4 | `MovideskService` renomeado para `MovideskTicketsService` | 8, 9 |
| 5 | Novos `MovideskPeopleService` e `MovideskServicesService`, com os métodos de pessoas e de serviços | 9 |
| 6 | `MovideskPeopleRepository` e `MovideskServicesRepository` (e interfaces) criados vazios, já injetados nos services | 7.4, 9.2, 10 |

Consequência da divisão: o repositório também se divide, como no AfterSale (`IMovideskRepository` para empresas +
um repositório por recurso: tickets, pessoas e serviços), seção 7.

Não há decisões pendentes neste guia.
