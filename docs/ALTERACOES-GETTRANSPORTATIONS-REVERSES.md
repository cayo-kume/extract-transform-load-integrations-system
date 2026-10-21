# Alterações — `AfterSaleReversesService.GetTransportations` (doc_company por empresa + correções)

Guia para aplicação **manual**. As linhas indicadas referem-se ao estado atual do working tree
(branch `main`, já com as alterações do `GetStatus` aplicadas — ver
[ALTERACOES-GETSTATUS-REVERSES.md](ALTERACOES-GETSTATUS-REVERSES.md)). Aplique na ordem abaixo — de
baixo para cima dentro de cada arquivo, se preferir, para que as linhas de cima não se desloquem.

Resumo do que muda:

| # | Arquivo | O quê |
|---|---------|-------|
| 1 | `Entities/Reverses/Transportations/Transportation.cs` | Truncar `transportation` em 50 caracteres (tamanho da coluna) |
| 2 | `AfterSaleReversesService.cs` | Reescrita do `GetTransportations` (doc_company por empresa, duplicatas, cache, filtro, log e `break`) |
| 3 | `IAfterSaleReversesRepository.cs` | Adicionar o parâmetro `doc_company` na interface |
| 4 | `AfterSaleCommandHandler.cs` | Retornar `DOC_COMPANY` e filtrar a query por ela |
| 5 | `Script004_Create_Reverses_Tables.sql` | Remover `id` e criar a PK composta `(doc_company, transportation)` |
| 6 | Novo script SQL (bancos já existentes) | `ALTER` da tabela + recriar a procedure de merge |

> **Por que aqui não existe o passo "doc_company no DTO"?** No `GetStatus` o item da API é um
> objeto (`Dtos.Reverses.Status.Status`), então dava para pendurar o `doc_company` nele. Aqui a API
> devolve só uma lista de strings (`Dtos.Reverses.Transportations.Root.data` é `List<string>`),
> então não há onde colocar a propriedade. Criar um DTO só para isso seria exagero: com a lista
> declarada **dentro** do `foreach`, todos os itens pertencem a `company`, e o `doc_company` vai
> direto para o construtor da entidade.

---

## 1. Entity — `Domain.AfterSale/Entities/Reverses/Transportations/Transportation.cs`

Arquivo: [Transportation.cs](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reverses/Transportations/Transportation.cs)

O validator (`TransportationValidation`) só **registra** no log quando o texto passa de 50
caracteres, mas a entidade grava o valor inteiro numa coluna `nvarchar(50)`, o que quebra o bulk
insert. Mesmo tratamento já usado em `Status` (`name`/`description`).

**Linha 19** — trocar:

```csharp
            this.transportation = transportation;
```

por:

```csharp
            this.transportation = transportation?.Length > 50 ? transportation.Substring(0, 50) : transportation;
```

---

## 2. Service — `Application.AfterSale/Services/AfterSaleReversesService.cs`

Arquivo: [AfterSaleReversesService.cs](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Services/AfterSaleReversesService.cs)

**Substituir as linhas 583–683** (do `/// <summary>` do `GetTransportations` até o `}` que fecha o
método, imediatamente antes do `/// <summary>` de `PutReciveProductsById`) por:

```csharp
        /// <summary>
        /// returns the possible transportations for each company.
        /// </summary>
        public async Task<bool?> GetTransportations()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleTransportationsReverses);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                        route: "v3/api/transportations",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.Transportations.Root>(response);
                var transportationsReverses = (root?.data ?? [])
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var transportationsReversesCache = await _cache.GetOrCreateAsync($"{TransportationsReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetTransportations(company.doc_company);

                    return list
                        .Where(x => x != null && !string.IsNullOrEmpty(x.transportation))
                        .Select(x => x!.transportation)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase); // HashSet<string> com as transportadoras já gravadas da empresa
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.Transportations.Transportation>();

                foreach (var transportationReverse in transportationsReverses)
                {
                    var validations = _transportationsValidator.Validate(transportationReverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.Transportations.Transportation(transportationReverse, company.doc_company));
                }

                // mantém só o que é novo (remove quando a transportadora já existe no cache da empresa)
                _listSomenteNovos.RemoveAll(x => transportationsReversesCache.Contains(x.transportation));

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleTransportationsReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        _logger.AddRecord(record.transportation, record.transportation);
                        transportationsReversesCache.Add(record.transportation);
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
```

### O que mudou e por quê (referência às linhas antigas)

| Linha(s) antiga(s) | Antes | Depois | Motivo |
|---|---|---|---|
| 583–585 | `returns the possible transportations` | `... for each company.` | O método agora percorre todas as empresas. |
| 592–595 | `listResponses`, `_listSomenteNovos` e `transportationsReverses` declarados **fora** do `foreach` | `transportationsReverses` e `_listSomenteNovos` declarados **dentro** do `foreach`; `listResponses` removido; novo contador `totalInseridos` | Mesmo problema do `GetStatus`: na 2ª empresa, as transportadoras da 1ª seriam inseridas de novo, com o CNPJ da 2ª. |
| 604–606 | `AddRange(root.data)` (quebra com `NullReferenceException` se `data` vier `null`) | Fallback para lista vazia, descarta vazios/brancos e remove repetidos ignorando maiúsculas/minúsculas | Itens vazios virariam PK nula e repetidos dentro da mesma resposta violariam a PK no merge. |
| 608 | Chave de cache única `TransportationsReversesKey` | `$"{TransportationsReversesKey}:{company.doc_company}"` | Cada empresa passa a ter seu cache, em vez de reaproveitar o da 1ª. |
| 615–618 | `GroupBy(x => x.doc_company).ToDictionary(...)` → `Dictionary<string, Transportation>` | `HashSet<string>` com os nomes das transportadoras | O dicionário agrupado por empresa guardava **uma única** transportadora por empresa (`g.First()`), e a chave era o CNPJ, não a transportadora. O que se precisa saber é só "essa transportadora já existe para a empresa?", e um `HashSet` responde isso direto. |
| 621–627 | `transportationsReversesCache.RemoveAll(...)` | `_listSomenteNovos.RemoveAll(x => cache.Contains(...))`, **depois** de montar a lista | Não compilava: `Dictionary` não tem `RemoveAll`, e o `x` da lambda seria um `KeyValuePair`, sem `doc_company`. Além disso, aplicava o filtro no cache em vez da lista da API, e com `!=`/`&&` (lógica invertida, igual ao `GetStatus`). |
| 629, 631, 643 | Variável `transportationRoot` | `transportationReverse` | Só nome: o item é uma transportadora, não o `Root`. |
| 643 | `company.doc_company` | Mantido | Correto agora que a lista é por empresa (ver nota no topo). |
| 650–666 | Laço sobre `listResponses` que sempre terminava com o `data` da **última** resposta; log e cache só eram atualizados se esse `data` existisse | `AddRecord(record.transportation, record.transportation)` e `cache.Add(...)` direto | O item da API **é** a própria string; não há JSON a procurar. O laço antigo era inútil (o `continue` não fazia nada) e com várias empresas buscaria na resposta errada. `transportationsReversesCache.Add(string)` também não compilava sobre um `Dictionary`. |
| 669–672 | `break` + comentário | **Removidos** | O `break` fazia só a 1ª empresa ser processada. |
| 676 | `_listSomenteNovos.Count` | `totalInseridos` | A mensagem final passa a somar todas as empresas. |

> **Comparação sem diferenciar maiúsculas/minúsculas.** O SQL Server normalmente usa collation
> case-insensitive; com PK em `transportation`, `"Correios"` e `"CORREIOS"` seriam a mesma chave
> no banco. Por isso o `Distinct` e o `HashSet` usam `StringComparer.OrdinalIgnoreCase`. Se a
> collation do banco for case-sensitive, troque por `StringComparer.Ordinal` nos dois lugares.

---

## 3. Interface — `Domain.AfterSale/Abstractions/Repositorys/IAfterSaleReversesRepository.cs`

Arquivo: [IAfterSaleReversesRepository.cs](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Abstractions/Repositorys/IAfterSaleReversesRepository.cs)

A implementação (`AfterSaleReversesRepository.cs`, linha 179) e o service já usam
`GetTransportations(string doc_company)`, mas a interface não tem o parâmetro — hoje não compila.

**Linha 21** — trocar:

```csharp
        public Task<List<Entities.Reverses.Transportations.Transportation?>> GetTransportations();
```

por:

```csharp
        public Task<List<Entities.Reverses.Transportations.Transportation?>> GetTransportations(string doc_company);
```

> O repository (`AfterSaleReversesRepository.cs`, linhas 179–186) já está correto; nada a mudar lá.

---

## 4. Query — `Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs`

Arquivo: [AfterSaleCommandHandler.cs](../src/Backend/Modules/General/AfterSale/Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs)

**Linhas 89–90** — trocar a query sem filtro pela filtrada. Assim como no `GetStatus`, a versão
comentada tem um `--` antes do `WHERE`; remova-o. Inclua também `DOC_COMPANY` no `SELECT` para a
entidade vir completa.

Antes:

```csharp
            return @$"SELECT TRANSPORTATION FROM [GENERAL].[AFTERSALETRANSPORTATIONSREVERSES] A (NOLOCK)";
            //return @$"SELECT TRANSPORTATION FROM [GENERAL].[AFTERSALETRANSPORTATIONSREVERSES] A (NOLOCK) --WHERE DOC_COMPANY = '{doc_company}'";
```

Depois:

```csharp
            return @$"SELECT DOC_COMPANY, TRANSPORTATION FROM [GENERAL].[AFTERSALETRANSPORTATIONSREVERSES] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
```

> A assinatura em `IAfterSaleCommandHandler` (linha 18) já recebe `doc_company`; nada a mudar lá.

---

## 5. Script de criação — `Script004_Create_Reverses_Tables.sql`

Arquivo: [Script004_Create_Reverses_Tables.sql](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script004_Create_Reverses_Tables.sql)

Hoje as tabelas têm uma coluna `id bigint` (comentário "gerar com c-sharp") que é a PK da trusted,
mas a entidade `Transportation` **não tem** `id`: a tabela é montada a partir da entidade
(`CreateSystemDataTable`), então o `id` chega sempre nulo e o merge (que casa pela PK) não
funciona. A identidade natural do registro é a transportadora dentro da empresa, então a PK passa
a ser `(doc_company, transportation)` e o `id` sai.

**Linhas 36–48** — substituir por (mesmo estilo de PK composta usado em
`AfterSaleCourierAttributesFields`, linhas 31–34):

```sql
create table [untreated].AfterSaleTransportationsReverses (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	transportation nvarchar(50)
);

create table [general].AfterSaleTransportationsReverses (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	transportation nvarchar(50) not null,
constraint [PK_AfterSaleTransportationsReverses] primary key (
	doc_company asc,
	transportation asc
));
```

---

## 6. Bancos já existentes — novo script SQL

Os scripts `ScriptNNN_*.sql` só rodam uma vez; alterar o `Script004` não muda um banco onde ele
já foi executado. Se a tabela já existe em algum ambiente, crie um novo script na mesma pasta. Se
você já criou o `Script009` do `GetStatus`, use o próximo número, por exemplo
`Script010_Alter_AfterSaleTransportationsReverses_PK.sql`:

```sql
-- registros antigos foram gravados sem doc_company e/ou sem id válido
delete from [general].AfterSaleTransportationsReverses where doc_company is null or transportation is null;
delete from [untreated].AfterSaleTransportationsReverses where doc_company is null or transportation is null;

declare @pk_name nvarchar(128) = (
	select name from sys.key_constraints
	where type = 'PK' and parent_object_id = object_id('[general].AfterSaleTransportationsReverses')
);

if @pk_name is not null
	exec('alter table [general].AfterSaleTransportationsReverses drop constraint [' + @pk_name + ']');

alter table [general].AfterSaleTransportationsReverses drop column id;
alter table [untreated].AfterSaleTransportationsReverses drop column id;

alter table [general].AfterSaleTransportationsReverses alter column [doc_company] nvarchar(14) not null;
alter table [general].AfterSaleTransportationsReverses alter column transportation nvarchar(50) not null;

alter table [general].AfterSaleTransportationsReverses
	add constraint [PK_AfterSaleTransportationsReverses] primary key (doc_company asc, transportation asc);

-- a procedure de merge é gerada a partir da PK; recriar para que passe a casar por (doc_company, transportation)
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'AfterSaleTransportationsReverses', @execute = 1;
```

> Assim como no `GetStatus`, confira antes se o `sp_Create_Merge_Procedure` recria a procedure
> quando ela já existe. Se não recriar, faça o `drop procedure` da
> `sp_AfterSaleTransportationsReverses_Sync` antes da última linha.
>
> Se a trusted tiver transportadoras repetidas para a mesma empresa (possível, já que o `id` vinha
> nulo/sem controle), o `add constraint` vai falhar. Nesse caso, remova as duplicatas antes, por
> exemplo com um `;with cte as (select *, row_number() over (partition by doc_company, transportation order by integrated_or_updated_at desc) rn from [general].AfterSaleTransportationsReverses) delete from cte where rn > 1;`.

---

## Checklist

- [ ] 1. Entity: truncar `transportation` em 50 caracteres (linha 19)
- [ ] 2. Service: método `GetTransportations` substituído (linhas 583–683)
- [ ] 3. Interface: `GetTransportations(string doc_company)` (linha 21)
- [ ] 4. Command handler: query com `DOC_COMPANY` no `SELECT` e no `WHERE` (linhas 89–90)
- [ ] 5. `Script004`: remover `id` e criar PK composta (linhas 36–48)
- [ ] 6. Novo script de `ALTER` para bancos existentes + recriação da procedure de merge
- [ ] Compilar a solução
- [ ] Rodar o job `AfterSaleTransportationsReverses` e conferir na tabela trusted as transportadoras de cada empresa, sem repetição
