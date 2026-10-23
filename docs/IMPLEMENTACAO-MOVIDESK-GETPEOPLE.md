# Movidesk — Implementação C# do `GetPeople`

Guia para adaptar o código C# de pessoas às tabelas de [MOVIDESK-MODELAGEM-PESSOAS.md](MOVIDESK-MODELAGEM-PESSOAS.md)
(seção 8), no mesmo padrão já implementado para tickets ([IMPLEMENTACAO-MOVIDESK-GETTICKETS.md](IMPLEMENTACAO-MOVIDESK-GETTICKETS.md)).

> **Implementado em 21/10/2026.** O host `Hangfire` compila com 0 erros; as 10 entidades batem com o `Script007` em nome e
> tipo; o host sobe e o Swagger lista `GET MovideskJobs/Movidesk/People`. O `GetPeople` ainda **não foi executado** contra a
> API nem o banco. O `PageSize` ficou em 100 (a seção 6.3 traz a opção de 1.000). Os scripts SQL já existem (`Script007`, `Script008`
> e os jobs 3516–3525 no `Script005`). As decisões de 21/10/2026 estão na seção 9.

**Um arquivo por classe:** cada classe deste guia vai no seu próprio `.cs`, com o nome da classe (`Team.cs`,
`Relationship.cs`, `NullableConverters.cs`...). Os blocos de código abaixo já estão separados por arquivo.

Raiz do módulo: `src/Backend/Modules/General/Movidesk/`. **Nomenclatura:** `Person` só para uma pessoa; `People` para o
plural.

---

## 0. Ordem

1. Renomeações (seção 1)
2. `NullableConverters.ToNullableBoolean` no `Domain.Shared` (3.0)
3. DTOs (2)
4. Entidades (3)
5. Validadores (4)
6. Queries, interface e repositório (5)
7. Service (6)
8. Controller (7)

Cada passo compila sobre o anterior; o módulo já compila hoje, então dá para compilar a cada passo.

---

## 1. Renomeações

| Hoje | Passa a ser | Como |
|---|---|---|
| `Domain.Movidesk/Dtos/Reads/Person/` | `Domain.Movidesk/Dtos/Reads/People/` | `git mv`; namespace `Domain.Movidesk.Dtos.Reads.People` |
| `Domain.Movidesk/Entities/Reads/Person/` | `Domain.Movidesk/Entities/Reads/People/` | `git mv`; namespace `Domain.Movidesk.Entities.Reads.People` |
| `Application.Movidesk/Validations/Person/` | `Application.Movidesk/Validations/People/` | `git mv`; namespace `Application.Movidesk.Validations.People` |
| `EnumJob.MovideskGetPersons = 3001` | `EnumJob.MovideskGetPeople = 3001` | mesmo valor; o `Script005` já usa o título novo |
| `IMovideskPeopleService.GetPersons()` | `GetPeople()` | `GetPersonById()` continua (é de uma pessoa) |

As classes continuam no singular (`Person`, `Address`, `Email`...): cada uma representa um item.

Com a pasta `People`, o namespace deixa de ter o mesmo nome da classe `Person` (`...Reads.Person.Person`), o que hoje
obriga a escrever o nome completo em alguns lugares.

---

## 2. DTOs (`Dtos/Reads/People`)

Continuam `string`. Mudanças:

| Arquivo | Mudança | Motivo |
|---|---|---|
| `Person.cs` | Remover `password` | Só de gravação; a API nunca devolve (modelagem 4.3). |
| `Relationship.cs` | Remover `isGetMethod` | Não documentado; vem sempre `true` (modelagem 4.3). |
| `CustomFieldValueItem.cs` | + `storageFileGuid`, + `fileName` | Como nos tickets (campos do tipo arquivo). |
| `Address.cs` | Manter `countryId` | Veio preenchido nas respostas reais. |

`accountEmail` (não documentado, sempre `null` até agora) **não** entra.

---

## 3. Entidades (`Entities/Reads/People`)

### 3.0 `NullableConverters.ToNullableBoolean` (Shared)
`src/Backend/Shared/Domain.Shared/Extensions/NullableConverters.cs` ganha um método, ao lado dos que já existem
(decidido: no Shared, não como método privado da entidade):
```csharp
// bool? para campos que a API manda null (ex.: includeInParents); bool comum gravaria false, que é outra informação
public static bool? ToNullableBoolean(string? value) =>
    String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToBooleanValidation(value);
```

### 3.1 Regras (as mesmas dos tickets)
- Nome e tipo de cada propriedade = coluna do `Script007` (o bulk insert mapeia por nome).
- `integrated_or_updated_at` e `doc_company` em todas, recebidos pelo construtor; **um horário por lote**.
- Listas filhas com `[SkipProperty]` e `= []`; construtor vazio + construtor com o DTO.
- Textos com tamanho passam por `StringLengthValidation`; `nvarchar(max)` recebe o valor direto.
- Valores ausentes com `NullableConverters` (`Domain.Shared.Extensions`), inclusive `ToNullableBoolean`.
- **Uma classe por arquivo**, com o nome da classe.
- A coluna que liga à pessoa é `personId`.

### 3.2 `Person`
`Entities/Reads/People/Person.cs`
```csharp
using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Person
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string id { get; private set; }
        public string codeReferenceAdditional { get; private set; }
        public bool isActive { get; private set; }
        public int? personType { get; private set; }
        public int? profileType { get; private set; }
        public string accessProfile { get; private set; }
        public string corporateName { get; private set; }
        public string businessName { get; private set; }
        public string businessBranch { get; private set; }
        public string cpfCnpj { get; private set; }
        public string userName { get; private set; }
        public string role { get; private set; }
        public string? bossId { get; private set; }
        public string bossName { get; private set; }
        public string classification { get; private set; }
        public string cultureId { get; private set; }
        public string timeZoneId { get; private set; }
        public string? authenticateOn { get; private set; }
        public DateTime? createdDate { get; private set; }
        public string? createdById { get; private set; }
        public DateTime? changedDate { get; private set; }
        public string? changedById { get; private set; }
        public string? observations { get; private set; }

        [SkipProperty]
        public List<Address> addresses { get; private set; } = [];

        [SkipProperty]
        public List<Contact> contacts { get; private set; } = [];

        [SkipProperty]
        public List<Email> emails { get; private set; } = [];

        [SkipProperty]
        public List<Team> teams { get; private set; } = [];

        [SkipProperty]
        public List<Relationship> relationships { get; private set; } = [];

        [SkipProperty]
        public List<CustomFieldValue> customFieldValues { get; private set; } = [];

        [SkipProperty]
        public List<Asset> atAssets { get; private set; } = [];

        public Person() { }

        public Person(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Person dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            codeReferenceAdditional = ConverterValidatorsExtensions.StringLengthValidation(dto.codeReferenceAdditional, 64);
            isActive = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isActive);
            personType = NullableConverters.ToNullableInt32(dto.personType);
            profileType = NullableConverters.ToNullableInt32(dto.profileType);
            accessProfile = ConverterValidatorsExtensions.StringLengthValidation(dto.accessProfile, 128);
            corporateName = ConverterValidatorsExtensions.StringLengthValidation(dto.corporateName, 128);
            businessName = ConverterValidatorsExtensions.StringLengthValidation(dto.businessName, 128);
            businessBranch = ConverterValidatorsExtensions.StringLengthValidation(dto.businessBranch, 128);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
            userName = ConverterValidatorsExtensions.StringLengthValidation(dto.userName, 64);
            role = ConverterValidatorsExtensions.StringLengthValidation(dto.role, 128);
            bossId = dto.bossId;
            bossName = ConverterValidatorsExtensions.StringLengthValidation(dto.bossName, 128);
            classification = ConverterValidatorsExtensions.StringLengthValidation(dto.classification, 128);
            cultureId = ConverterValidatorsExtensions.StringLengthValidation(dto.cultureId, 32);
            timeZoneId = ConverterValidatorsExtensions.StringLengthValidation(dto.timeZoneId, 64);
            authenticateOn = dto.authenticateOn;   // nvarchar(max)
            createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
            createdById = dto.createdBy;
            changedDate = NullableConverters.ToNullableDateTime(dto.changedDate);   // nulo para quem nunca foi alterado
            changedById = dto.changedBy;
            observations = dto.observations;       // nvarchar(max)

            // endereços, contatos e e-mails não têm chave na API: posição na lista (a lista inteira é regravada a cada lote)
            var dtoAddresses = dto.addresses ?? [];
            for (int i = 0; i < dtoAddresses.Count; i++)
                addresses.Add(new Address(doc_company, integrated_or_updated_at, id, i + 1, dtoAddresses[i]));

            var dtoContacts = dto.contacts ?? [];
            for (int i = 0; i < dtoContacts.Count; i++)
                contacts.Add(new Contact(doc_company, integrated_or_updated_at, id, i + 1, dtoContacts[i]));

            var dtoEmails = dto.emails ?? [];
            for (int i = 0; i < dtoEmails.Count; i++)
                emails.Add(new Email(doc_company, integrated_or_updated_at, id, i + 1, dtoEmails[i]));

            // equipes repetidas violariam a PK (o SQL Server não diferencia maiúsculas de minúsculas)
            foreach (var team in (dto.teams ?? []).Where(t => !String.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
                teams.Add(new Team(doc_company, integrated_or_updated_at, id, team));

            // um único relacionamento por organização; o sem organização (id null -> '') também só pode haver um
            foreach (var relationship in (dto.relationships ?? []).GroupBy(r => r.id ?? String.Empty).Select(g => g.First()))
                relationships.Add(new Relationship(doc_company, integrated_or_updated_at, id, relationship));

            foreach (var customFieldValue in dto.customFieldValues ?? [])
                customFieldValues.Add(new CustomFieldValue(doc_company, integrated_or_updated_at, id, customFieldValue));

            foreach (var asset in (dto.atAssets ?? []).Where(a => !String.IsNullOrEmpty(a.id)))
                atAssets.Add(new Asset(doc_company, integrated_or_updated_at, id, asset));
        }
    }
}
```
O `for (int i = 0; i <= personDto.teams?.Count(); i++)` do código atual lê um item além do fim da lista; aqui não há
mais índice manual nas listas sem chave.

### 3.3 Filhas sem chave na API (posição na lista)
`Entities/Reads/People/Address.cs`
```csharp
using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Address
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public int id { get; private set; }
        public string addressType { get; private set; }
        public string country { get; private set; }
        public int? countryId { get; private set; }
        public string postalCode { get; private set; }
        public string state { get; private set; }
        public string city { get; private set; }
        public string district { get; private set; }
        public string street { get; private set; }
        public string number { get; private set; }
        public string complement { get; private set; }
        public string reference { get; private set; }
        public bool isDefault { get; private set; }

        public Address() { }

        public Address(string doc_company, DateTime integrated_or_updated_at, string personId, int id, Dtos.Reads.People.Address dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.id = id;

            addressType = ConverterValidatorsExtensions.StringLengthValidation(dto.addressType, 128);
            country = ConverterValidatorsExtensions.StringLengthValidation(dto.country, 128);
            countryId = NullableConverters.ToNullableInt32(dto.countryId);
            postalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.postalCode, 32);
            state = ConverterValidatorsExtensions.StringLengthValidation(dto.state, 128);
            city = ConverterValidatorsExtensions.StringLengthValidation(dto.city, 128);
            district = ConverterValidatorsExtensions.StringLengthValidation(dto.district, 128);
            street = ConverterValidatorsExtensions.StringLengthValidation(dto.street, 128);
            number = ConverterValidatorsExtensions.StringLengthValidation(dto.number, 32);
            complement = ConverterValidatorsExtensions.StringLengthValidation(dto.complement, 128);
            reference = ConverterValidatorsExtensions.StringLengthValidation(dto.reference, 128);
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDefault);
        }
    }
}
```
`Contact` e `Email` seguem o mesmo molde, cada um no seu arquivo (`Contact.cs`, `Email.cs`):

| Entidade | Construtor | Propriedades (além de `integrated_or_updated_at`, `doc_company`, `personId`, `id`) |
|---|---|---|
| `Contact` | `(doc_company, integrated_or_updated_at, personId, int id, Dtos...Contact dto)` | `contactType` (128), `contact` (128), `isDefault` (`bool`) |
| `Email` | `(doc_company, integrated_or_updated_at, personId, int id, Dtos...Email dto)` | `emailType` (128), `email` (128), `isDefault` (`bool`) |

### 3.4 Filhas com chave natural
`Entities/Reads/People/Team.cs`
```csharp
using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Team
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public string team { get; private set; }

        public Team() { }

        public Team(string doc_company, DateTime integrated_or_updated_at, string personId, string team)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.team = ConverterValidatorsExtensions.StringLengthValidation(team, 128);
        }
    }
}
```

`Entities/Reads/People/Relationship.cs`
```csharp
using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Relationship
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string slaAgreement { get; private set; }
        public bool forceChildrenToHaveSomeAgreement { get; private set; }
        public bool? allowAllServices { get; private set; }
        public bool? includeInParents { get; private set; }
        public bool? loadChildOrganizations { get; private set; }

        [SkipProperty]
        public List<Service> services { get; private set; } = [];

        public Relationship() { }

        public Relationship(string doc_company, DateTime integrated_or_updated_at, string personId, Dtos.Reads.People.Relationship dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;

            // relacionamento sem organização: a API manda id null; '' porque a coluna faz parte da PK (modelagem 4.6)
            id = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128);
            slaAgreement = ConverterValidatorsExtensions.StringLengthValidation(dto.slaAgreement, 128);
            forceChildrenToHaveSomeAgreement = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.forceChildrenToHaveSomeAgreement);
            allowAllServices = NullableConverters.ToNullableBoolean(dto.allowAllServices);
            includeInParents = NullableConverters.ToNullableBoolean(dto.includeInParents);
            loadChildOrganizations = NullableConverters.ToNullableBoolean(dto.loadChildOrganizations);

            foreach (var service in (dto.services ?? []).Where(s => !String.IsNullOrEmpty(s.id)))
                services.Add(new Service(doc_company, integrated_or_updated_at, personId, id, service));
        }
    }
}
```
- `StringLengthValidation(null, 64)` já devolve `String.Empty`, então o `id` nulo vira `''` sem código extra; o
  comentário fica para quem ler depois.
- `allowAllServices`, `includeInParents` e `loadChildOrganizations` usam `NullableConverters.ToNullableBoolean` (3.0):
  `includeInParents` e `loadChildOrganizations` vieram `null` nas respostas reais, e com `bool` comum seriam gravados
  como `false`, que é uma informação diferente.

| Entidade | Construtor | Propriedades |
|---|---|---|
| `Service` (serviço da organização) | `(doc_company, integrated_or_updated_at, personId, string relationshipId, Dtos...Service dto)` | `relationshipId`; `id` (`int`, `ConvertToInt32Validation`); `name` (128); `copyToChildren` (`bool?`, `NullableConverters.ToNullableBoolean`) |
| `Asset` | `(doc_company, integrated_or_updated_at, personId, Dtos...Asset dto)` | `id` (64), `name` (64), `label` (64) |
| `CustomFieldValue` | `(doc_company, integrated_or_updated_at, personId, Dtos...CustomFieldValue dto)` | `customFieldId`, `customFieldRuleId`, `line` (`int`); `value` (sem corte); `[SkipProperty] List<CustomFieldValueItem> items`, criados com `id = i + 1` |
| `CustomFieldValueItem` | `(doc_company, integrated_or_updated_at, personId, customFieldId, customFieldRuleId, line, int id, Dtos...CustomFieldValueItem dto)` | **`itemPersonId`** = `dto.personId` (64), **`itemClientId`** = `dto.clientId` (64), `team` (128), `customFieldItem` (256), `storageFileGuid` (36), `fileName` (255) |

Cada uma no seu arquivo: `Service.cs`, `Asset.cs`, `CustomFieldValue.cs`, `CustomFieldValueItem.cs`.
`CustomFieldValue` e `CustomFieldValueItem` são cópias das de tickets, trocando `ticketId` (`long`) por `personId`
(`string`) e `personId`/`clientId` do item por `itemPersonId`/`itemClientId`.

### 3.5 O que sai das entidades atuais
- `Person.password`, `Person.createdBy`/`changedBy` (viram `createdById`/`changedById`).
- `Relationship.isGetMethod`.
- `Team.id` (a chave passa a ser o `team`).
- `CustomFieldValueItem.personId` e `clientId` como `int` (viram `itemPersonId`/`itemClientId` como texto).

---

## 4. Validadores (`Validations/People`)

Os validadores de pessoas existem desde o código antigo; os ajustes são poucos:

| Arquivo | Mudança |
|---|---|
| `PersonValidator.cs` | Remover a regra de `password`. Conferir os tamanhos com o `Script007` (`id`/`bossId`/`codeReferenceAdditional`/`userName` 64, `cpfCnpj` 14, `cultureId` 32, `timeZoneId` 64, textos 128). Adicionar `RuleForEach(x => x.customFieldValues).SetValidator(new CustomFieldValueValidator())` se não existir, e `RuleForEach(x => x.atAssets).SetValidator(new AssetValidator())`. |
| `RelationshipValidator.cs` | Remover a regra de `isGetMethod`. |
| `CustomFieldValueItemValidator.cs` | `personId` e `clientId` estão com `MustBeValidInt32()`; trocar por `MaximumLength(64)`, porque são Cód. Ref. em texto (mesma correção feita nos tickets). Adicionar `fileName` (255). |
| `AddressValidator.cs` | `countryId` com `MustBeValidInt32()` continua. |

Todos registram só mensagens no log; o service não descarta a pessoa. O registro na injeção de dependência não muda
(`AddValidatorsFromAssemblyContaining<TicketValidator>` registra o assembly inteiro).

---

## 5. Queries, interface e repositório

### 5.1 Queries (`IMovideskCommandHandler` / `MovideskCommandHandler`)
```csharp
public string CreateGetPeopleLastChangeQuery(string doc_company);
public string CreateGetPeopleByIdsQuery(string doc_company, List<string> ids);
```
```csharp
public string CreateGetPeopleLastChangeQuery(string doc_company)
{
    // quem nunca foi alterado tem changedDate nulo: a última mudança é a criação
    return @$"SELECT MAX(COALESCE(CHANGEDDATE, CREATEDDATE))
             FROM MOVIDESK.MOVIDESKPEOPLE
             WHERE DOC_COMPANY = '{doc_company}'";
}

public string CreateGetPeopleByIdsQuery(string doc_company, List<string> ids)
{
    // o Cód. Ref. é alfanumérico: entra entre aspas, com as aspas internas dobradas
    // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
    return @$"SELECT DOC_COMPANY, ID, CREATEDDATE, CHANGEDDATE
             FROM MOVIDESK.MOVIDESKPEOPLE
             WHERE DOC_COMPANY = '{doc_company}'
             AND ID IN ({String.Join(", ", ids.Select(id => $"'{id.Replace("'", "''")}'"))})";
}
```
Diferente dos tickets, o id é texto vindo da API, então as aspas são dobradas para não quebrar (nem injetar) SQL.

### 5.2 Interface (`Domain.Movidesk/Abstractions/Repositorys/IMovideskPeopleRepository.cs`)
```csharp
namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskPeopleRepository
    {
        public Task<DateTime?> GetPeopleLastChange(string doc_company);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.People.Person?>> GetPeopleByIds(string doc_company, List<string> ids);
        public Task<bool> BulkInsertIntoMovideskPeople(List<Domain.Movidesk.Entities.Reads.People.Person> data, Guid? parentExecutionGUID);
    }
}
```

### 5.3 Repositório (`Infrastructure.Movidesk/Repositorys/MovideskPeopleRepository.cs`)
```csharp
using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Reads.People;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskPeopleRepository : IMovideskPeopleRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskPeopleRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<DateTime?> GetPeopleLastChange(string doc_company)
        {
            string sql = _movideskCommandHandler.CreateGetPeopleLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Person?>> GetPeopleByIds(string doc_company, List<string> ids)
        {
            var result = new List<Person?>();

            // lotes de 1000 ids, como no GetActionsHtmlDescriptions dos tickets
            foreach (var chunk in ids.Distinct().Chunk(1000))
            {
                string sql = _movideskCommandHandler.CreateGetPeopleByIdsQuery(doc_company, chunk.ToList());
                result.AddRange(await _coreRepository.GetRecords<Person>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoMovideskPeople(List<Person> data, Guid? parentExecutionGUID)
        {
            var relationships = data.SelectMany(x => x.relationships).ToList();
            var customFieldValues = data.SelectMany(x => x.customFieldValues).ToList();

            // ordem = ordem dos merges: pessoa, nível do meio, depois filhas e netas (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Person(), tableName: "MovideskPeople"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Relationship(), tableName: "MovideskPeopleRelationships"), relationships),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValue(), tableName: "MovideskPeopleCustomFieldValues"), customFieldValues),
                (_coreRepository.CreateSystemDataTable(entity: new Address(), tableName: "MovideskPeopleAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Contact(), tableName: "MovideskPeopleContacts"), data.SelectMany(x => x.contacts).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Email(), tableName: "MovideskPeopleEmails"), data.SelectMany(x => x.emails).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Team(), tableName: "MovideskPeopleTeams"), data.SelectMany(x => x.teams).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Service(), tableName: "MovideskPeopleRelationshipsServices"), relationships.SelectMany(x => x.services).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValueItem(), tableName: "MovideskPeopleCustomFieldValuesItems"), customFieldValues.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Asset(), tableName: "MovideskPeopleAssets"), data.SelectMany(x => x.atAssets).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge roda mesmo para tabelas sem linhas no lote: é ele que apaga as filhas removidas
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("movidesk", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
```
É o mesmo molde do `MovideskTicketsRepository.BulkInsertIntoMovideskTickets`.

---

## 6. Service (`MovideskPeopleService.GetPeople`)

### 6.1 Construtor
```csharp
private readonly IApiClient _apiClient;
private readonly ILoggerService _logger;
private readonly IMemoryCache _cache;
private readonly IMovideskRepository _movideskRepository;
private readonly IMovideskPeopleRepository _movideskPeopleRepository;
private readonly IValidator<Domain.Movidesk.Dtos.Reads.People.Person> _personValidator;

private const string PeopleKey = "Movidesk:People";

// limite da API: 10 requisições por minuto -> 1 requisição a cada 6 s
private const int RequestIntervalInMilliseconds = 6 * 1000;
private const int PageSize = 100;

public MovideskPeopleService(
    [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
    ILoggerService logger,
    IMovideskRepository movideskRepository,
    IMovideskPeopleRepository movideskPeopleRepository,
    IValidator<Domain.Movidesk.Dtos.Reads.People.Person> personValidator,
    IMemoryCache cache
) =>
    (_apiClient, _logger, _movideskRepository, _movideskPeopleRepository, _personValidator, _cache) =
    (apiClient, logger, movideskRepository, movideskPeopleRepository, personValidator, cache);
```
Ganha `IValidator` e `IMemoryCache` em relação ao construtor atual; a injeção de dependência já registra os dois.

### 6.2 Chamada à API
- **Sem `$select` e sem `$expand`:** a listagem devolveu todas as listas sem eles (modelagem 10).
- **Todos os perfis** (decidido): sem filtro de `profileType`.
- **Janela:** `changedDate ge {desde} or createdDate ge {desde}`, porque `changedDate` vem nulo para quem nunca foi
  alterado. `desde = MAX(coalesce(changedDate, createdDate)) − 1 hora`.
- **Banco vazio (decidido):** a primeira carga traz as pessoas **criadas ou alteradas nos últimos 12 meses**
  (`desde = agora − 12 meses`, mesmo filtro). Consequência: pessoas sem nenhuma mudança há mais de 12 meses não entram
  até serem alteradas. Nas respostas reais, o agente responsável pelo ticket 69255 teve a última alteração em 08/2024 e
  ficaria de fora; os tickets continuam com o id dele (sem FK), mas a junção com `MovideskPeople` não encontra a pessoa.
  Para trazer alguém específico, a saída é uma busca por id (`GetPersonById`, ainda stub), como o `GetTicketById`.
- `$orderby=id asc`, `$top=100`, `$skip`, 6 s entre páginas.

### 6.3 Código
```csharp
/// <summary>
/// sincroniza as pessoas criadas ou alteradas desde a última execução (agentes e clientes), para cada empresa
/// </summary>
public async Task<bool?> GetPeople()
{
    _logger
        .Clear()
        .AddLog(EnumJob.MovideskGetPeople);

    var totalInseridos = 0;
    var companys = await _movideskRepository.GetCompanys();

    foreach (var company in companys)
    {
        if (company?.doc_company is null)
            continue;

        // um horário por lote: pessoa, filhas e netas gravam o mesmo valor (condição de exclusão do merge)
        var batchTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        // changedDate é nulo para quem nunca foi alterado: a janela olha as duas datas.
        // banco vazio: primeira carga com as pessoas criadas ou alteradas nos últimos 12 meses
        var lastChange = await _movideskPeopleRepository.GetPeopleLastChange(company.doc_company);
        var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : DateTime.UtcNow.AddMonths(-12);
        var filter = $"changedDate ge {since:yyyy-MM-ddTHH:mm:ss}.00z or createdDate ge {since:yyyy-MM-ddTHH:mm:ss}.00z";

        var people = new List<Domain.Movidesk.Dtos.Reads.People.Person>();
        var peopleJson = new Dictionary<string, string>();
        var skip = 0;

        while (true)
        {
            var response = await _apiClient.GetAsync(
                route: "persons",
                queryParams: new Dictionary<string, string>
                {
                    { "token", company.Token.ToString() },
                    { "$filter", filter },
                    { "$orderby", "id asc" },
                    { "$top", PageSize.ToString() },
                    { "$skip", skip.ToString() }
                }
            );

            List<Domain.Movidesk.Dtos.Reads.People.Person>? page;

            try
            {
                page = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.People.Person>>(response ?? String.Empty);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                // objeto de erro (ou 429) em vez da lista: 3 erros seguidos bloqueiam a API, então a empresa é interrompida
                _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, skip {skip}): {response}");
                break;
            }

            if (page is null || page.Count == 0)
                break;

            people.AddRange(page);

            foreach (var item in JArray.Parse(response!))
            {
                var id = (string?)item["id"];

                if (!String.IsNullOrEmpty(id))
                    peopleJson[id] = item.ToString();
            }

            if (page.Count < PageSize)
                break;

            skip += PageSize;
            Thread.Sleep(RequestIntervalInMilliseconds);
        }

        if (people.Count == 0)
            continue;

        // cache de estado: id -> pessoa gravada (com as datas), por empresa
        var peopleCache = await _cache.GetOrCreateAsync($"{PeopleKey}:{company.doc_company}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            var list = await _movideskPeopleRepository.GetPeopleByIds(
                company.doc_company,
                people.Where(x => !String.IsNullOrEmpty(x.id)).Select(x => x.id).Distinct().ToList()
            );

            return list
                .Where(x => x != null)
                .GroupBy(x => x!.id)
                .ToDictionary(g => g.Key, g => g.First()!);
        });

        // mantém só as novas ou com última mudança mais recente que a gravada
        people.RemoveAll(x =>
            !string.IsNullOrEmpty(x.id) &&
            peopleCache!.TryGetValue(x.id, out var y) &&
            (NullableConverters.ToNullableDateTime(x.changedDate) ?? NullableConverters.ToNullableDateTime(x.createdDate))
                <= (y.changedDate ?? y.createdDate)
        );

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

            _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.People.Person(company.doc_company, batchTime, person));
        }

        if (_listSomenteNovos.Count == 0)
            continue;

        await _movideskPeopleRepository.BulkInsertIntoMovideskPeople(_listSomenteNovos, _logger.GetExecutionGuid());

        foreach (var record in _listSomenteNovos)
        {
            if (peopleJson.TryGetValue(record.id, out var json))
                _logger.AddRecord(record.id, json);

            peopleCache![record.id] = record;
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
- Mesmo fluxo do `GetTickets`; as diferenças são o `id` em texto, a janela pelas duas datas e a primeira carga de
  12 meses.
- **Primeira carga:** com muitas pessoas alteradas no ano, ela leva tempo (100 por página, 6 s entre páginas: 1.000
  pessoas por minuto). O endpoint espera até o fim; o `HttpClient` da Movidesk tem 20 minutos de timeout.
- **`$filter` com `or` (verificado em 21/10/2026):** a API aceitou `changedDate ge X or createdDate ge X` no formato
  `yyyy-MM-ddTHH:mm:ss.00z` e devolveu exatamente a união dos dois filtros isolados (detalhes na seção 10).
- **Volume:** só desde 01/09/2026 já há mais de 1.000 pessoas criadas, quase todas com `changedDate` nulo; a primeira
  carga de 12 meses deve ter vários milhares. A API aceitou `$top=1000` (devolveu 1.000 numa página), então o
  `PageSize` pode subir de 100 para 1.000 e reduzir as requisições em 10 vezes; fica a seu critério.
- **Comparação no cache** pela última mudança (`changedDate`, ou `createdDate` quando nulo), como na janela.
- `GetPersonById()` continua como stub; uma busca por id pode seguir o modelo do `GetTicketById` depois.

`using`s do service: os mesmos do `MovideskTicketsService` (`Application.Shared.Abstractions.Services`,
`Domain.Movidesk.Abstractions.Repositorys`, `Domain.Movidesk.Enums`, `Domain.Shared.Abstractions.Api`,
`Domain.Shared.Extensions`, `FluentValidation`, `Microsoft.Extensions.Caching.Memory`,
`Microsoft.Extensions.DependencyInjection`, `Newtonsoft.Json.Linq`).

---

## 7. Controller

`src/Backend/Hosts/Hangfire/Controllers/General/MovideskController.cs`: injetar também o `IMovideskPeopleService` e
trocar o endpoint comentado `Persons` por:
```csharp
private readonly IMovideskTicketsService _movideskTicketsService;
private readonly IMovideskPeopleService _movideskPeopleService;

public MovideskController(IMovideskTicketsService movideskTicketsService, IMovideskPeopleService movideskPeopleService) =>
    (_movideskTicketsService, _movideskPeopleService) = (movideskTicketsService, movideskPeopleService);

[HttpGet("People")]
[Tags("MovideskPeople")]
public async Task<ActionResult> People()
{
    var result = await _movideskPeopleService.GetPeople();

    return Ok($"Records integrated successfully.");
}
```
O endpoint de serviços (`Services`) continua comentado.

---

## 8. Checklist

- [ ] 1. `git mv` das pastas `Person` → `People` (DTOs, entidades, validadores) e namespaces
- [ ] 1. `EnumJob.MovideskGetPeople`; `IMovideskPeopleService.GetPeople`
- [ ] 2. `NullableConverters.ToNullableBoolean` no `Domain.Shared`
- [ ] 2. DTOs: sem `password` e `isGetMethod`; item com `storageFileGuid`/`fileName`
- [ ] 3. 10 entidades, **cada uma no seu arquivo**, com `doc_company`, `integrated_or_updated_at`, tipos anuláveis; `Address.city` e construtor vazio;
      ids gerados em endereços, contatos e e-mails; `id = ''` no relacionamento sem organização; `bool?` com `ToNullableBoolean`
- [ ] 4. Validadores ajustados
- [ ] 5. Queries (`LastChange` com `COALESCE`, `ByIds` com aspas) e `MovideskPeopleRepository` com os 10 merges sempre
- [ ] 6. `GetPeople` com laço por empresa, janela pelas duas datas (12 meses no banco vazio), paginação de 6 s e cache
- [ ] 7. Endpoint `GET MovideskJobs/Movidesk/People`
- [ ] Compilar o host
- [ ] Comparar as 10 entidades com o `Script007` (nome e tipo de cada coluna), como foi feito com tickets
- [ ] Banco local: rodar `Script007`, `Script008` e inserir os jobs 3516–3525 (e o título novo do 3001) à mão

## 9. Decisões (aplicadas em 21/10/2026)

| # | Decisão | Onde |
|---|---|---|
| 1 | Primeira carga (banco vazio) só com pessoas criadas ou alteradas nos últimos 12 meses | 6.2, 6.3 |
| 2 | `allowAllServices`, `includeInParents`, `loadChildOrganizations` e `copyToChildren` como `bool?`, com `NullableConverters.ToNullableBoolean` no `Domain.Shared` | 3.0, 3.4 |
| 3 | Uma classe por arquivo `.cs` | cabeçalho, 3, 8 |

Não há decisões pendentes neste guia.

---

## 10. Teste do filtro na API (21/10/2026)

Chamadas a `GET /persons` com `$select=id,createdDate,changedDate`, `$orderby=id asc` e `$top=1000`:

| Filtro | Resultado |
|---|---|
| Janela larga, desde 01/09/2026, com `or` | HTTP 200, 1.000 pessoas (bateu no `$top`); todas casam com uma das datas |
| A mesma, só `changedDate ge` | 19 pessoas |
| A mesma, só `createdDate ge` | 1.000 pessoas (bateu no `$top`) |
| Janela curta, desde 19/10/2026: `or` / só `changedDate` / só `createdDate` | 5 / 5 / 1 pessoas; o `or` é igual à união dos dois |

Na janela larga, comparando só a faixa de ids que coube nas 1.000 do `or` (até o id `9106437`):
- as 19 alteradas e as 989 criadas da faixa estão **todas** no resultado do `or`, e nada além delas;
- **981** pessoas do `or` têm `changedDate` nulo, e só entram pelo `createdDate` (o caso que motivou o `or`);
- **11** foram criadas antes da data e só entram pelo `changedDate`.

Também confirmado: o `$select` funciona na listagem de pessoas (vieram só os três campos pedidos), e o `$orderby=id`
ordena o id como texto (o último da página foi `9106437`, depois de ids com mais dígitos), o que não atrapalha a
paginação, porque a ordem é estável.
