# Alterações — `AfterSaleReversesService.GetStatus` (doc_company no DTO + correções)

Guia para aplicação **manual**. As linhas indicadas referem-se ao estado atual do working tree
(branch `main`, com as alterações ainda não commitadas). Aplique na ordem abaixo — de baixo para
cima dentro de cada arquivo, se preferir, para que as linhas de cima não se desloquem.

Resumo do que muda:

| # | Arquivo | O quê |
|---|---------|-------|
| 1 | `Dtos/Reverses/Status/Status.cs` | Nova propriedade `doc_company` com `[JsonIgnore]` |
| 2 | `AfterSaleReversesService.cs` | Reescrita do `GetStatus` (preenche `doc_company`, corrige duplicatas, filtro, cache e `break`) |
| 3 | `AfterSaleReversesRepository.cs` | Descomentar o parâmetro `doc_company` |
| 4 | `AfterSaleCommandHandler.cs` | Filtrar a query por `DOC_COMPANY` |
| 5 | `Script004_Create_Reverses_Tables.sql` | PK composta `(id, doc_company)` em `[aftersale].AfterSaleStatusReverses` |
| 6 | Novo script SQL (bancos já existentes) | `ALTER` da PK + recriar a procedure de merge |

---

## 1. DTO — `Domain.AfterSale/Dtos/Reverses/Status/Status.cs`

Arquivo: [Status.cs](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Dtos/Reverses/Status/Status.cs)

**Linha 1–3** — adicionar o using do Newtonsoft (o service desserializa com `Newtonsoft.Json`,
então o atributo tem que ser o do Newtonsoft, não o de `System.Text.Json`):

```csharp
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
```

**Após a linha 11** (`description`) — adicionar:

```csharp
        [JsonIgnore] // não vem da API; preenchido no service após a desserialização
        public string? doc_company { get; set; }
```

Resultado final da classe:

```csharp
    public class Status
    {
        public string id { get; init; }
        public string name { get; init; }
        public string description { get; init; }

        [JsonIgnore] // não vem da API; preenchido no service após a desserialização
        public string? doc_company { get; set; }
    }
```

> `set` (e não `init`) porque a atribuição acontece depois que o Newtonsoft já criou o objeto.

---

## 2. Service — `Application.AfterSale/Services/AfterSaleReversesService.cs`

Arquivo: [AfterSaleReversesService.cs](../Application.AfterSale/Services/AfterSaleReversesService.cs)

**Substituir as linhas 487–590** (do `/// <summary>` do `GetStatus` até o `}` que fecha o método,
imediatamente antes do `/// <summary>` de `GetTransportations`) por:

```csharp
        /// <summary>
        /// returns the possible status of a reverse for each company.
        /// </summary>
        public async Task<bool?> GetStatus()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleStatusReverses);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/reverses/status",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.Status.Root>(response);
                var statusReverses = root?.data ?? new List<Domain.AfterSale.Dtos.Reverses.Status.Status>();

                foreach (var statusReverse in statusReverses)
                    statusReverse.doc_company = company.doc_company;

                var statusReversesCache = await _cache.GetOrCreateAsync($"{StatusReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetStatus(company.doc_company);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<int, Entities...Status>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.Status.Status>();

                foreach (var statusReverse in statusReverses)
                {
                    var validations = _statusValidator.Validate(statusReverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.Status.Status(statusReverse, statusReverse.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    statusReversesCache.TryGetValue(x.id, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.name == y.name &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleStatusReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var reverse = data?.FirstOrDefault(r => (int?)r["id"] == record.id);

                        if (reverse != null)
                            _logger.AddRecord(record.id.ToString(), reverse.ToString());

                        statusReversesCache[record.id] = record;
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
| 487–491 | Comentário dizendo que só uma empresa é consultada | Comentário atualizado | O método agora percorre todas as empresas. |
| 498–501 | `listResponses`, `_listSomenteNovos` e `statusReverses` declarados **fora** do `foreach` | `_listSomenteNovos` e `statusReverses` declarados **dentro** do `foreach`; `listResponses` removido; novo contador `totalInseridos` | Antes, na 2ª empresa, os status da 1ª eram validados e inseridos de novo (duplicatas) e gravados com o CNPJ errado. |
| 510–512 | `AddRange(status.data)` sem CNPJ | Desserializa, faz fallback para lista vazia se vier `null`, e preenche `doc_company` em cada item | **Solução 1**: o DTO passa a carregar a empresa de origem. |
| 514 | Chave de cache única `StatusReversesKey` | `$"{StatusReversesKey}:{company.doc_company}"` | Antes, o cache era carregado com os dados da 1ª empresa e reaproveitado pelas demais; agora cada empresa tem o seu. |
| 523–524 | `GroupBy(x => x.id)` / `g.First()` | `GroupBy(x => x!.id)` / `g.First()!` | Só ajuste de nulidade: o dicionário fica `Dictionary<int, Status>` (sem `?`), e `y` não precisa de checagem de null. |
| 527–534 | `RemoveAll` sobre o **DTO** com `!=` e `&&` | `RemoveAll` sobre a **entidade** com `==` e `&&` | O predicado antigo só removia quando *todos* os campos eram diferentes (lógica invertida). Agora remove quando já existe e está igual. Comparar a entidade, e não o DTO, evita falsos "alterados" quando `name`/`description` são truncados no construtor (50/250 chars) e dispensa converter o `id`. |
| 550 | `new Status(statusReverse, company.doc_company)` | `new Status(statusReverse, statusReverse.doc_company)` | Usa o CNPJ que veio com o próprio item. |
| 559–566 | Procurava o JSON do registro em **todas** as respostas acumuladas (`listResponses`) | Procura só na resposta da empresa atual (`response`) | Com várias empresas, o mesmo `id` existiria em mais de uma resposta e o log pegaria o JSON da empresa errada. Também troca o cast `(long?)` por `(int?)`, que é o tipo de `record.id`. |
| 568–572 | Cache só era atualizado se o JSON fosse encontrado | Cache sempre atualizado após o insert | O registro foi gravado, então o cache tem que refletir isso independentemente do log. |
| 576–579 | `break` + comentário | **Removidos** | O `break` fazia só a 1ª empresa ser processada. Os pré-requisitos do comentário (coluna `doc_company` na tabela e na entidade) já estão feitos. |
| 583 | `_listSomenteNovos.Count` (só a última lista) | `totalInseridos` | A mensagem final passa a somar todas as empresas. |

---

## 3. Repository — `Infrastructure.AfterSale/Repositorys/AfterSaleReversesRepository.cs`

Arquivo: [AfterSaleReversesRepository.cs](../Infrastructure.AfterSale/Repositorys/AfterSaleReversesRepository.cs)

**Linhas 157 e 159** — descomentar o parâmetro (hoje a assinatura não bate com a interface
`IAfterSaleReversesRepository.GetStatus(string doc_company)`, linha 18, e não compila):

Antes:

```csharp
        public async Task<List<Status?>> GetStatus(/*string doc_company*/)
        {
            string sql = _afterSaleCommandHandler.CreateGetStatusQuery(/*doc_company*/);
```

Depois:

```csharp
        public async Task<List<Status?>> GetStatus(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetStatusQuery(doc_company);
```

---

## 4. Query — `Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs`

Arquivo: [AfterSaleCommandHandler.cs](../Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs)

**Linhas 83–84** — trocar a query sem filtro pela filtrada. Atenção: a versão comentada tem um
`--` antes do `WHERE`, o que comentaria o filtro dentro do SQL; remova-o.

Antes:

```csharp
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALESTATUSREVERSES] A (NOLOCK)";
            //return @$"SELECT * FROM [AFTERSALE].[AFTERSALESTATUSREVERSES] A (NOLOCK) --WHERE DOC_COMPANY = '{doc_company}'";
```

Depois:

```csharp
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALESTATUSREVERSES] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
```

> A assinatura em `IAfterSaleCommandHandler` (linha 16) já recebe `doc_company`; nada a mudar lá.

---

## 5. Script de criação — `Script004_Create_Reverses_Tables.sql`

Arquivo: [Script004_Create_Reverses_Tables.sql](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script004_Create_Reverses_Tables.sql)

Hoje a tabela trusted tem PK só em `id`. Como cada empresa grava seus próprios status, o mesmo `id`
vai aparecer para empresas diferentes e a procedure de merge (que casa os registros pela PK —
ver `Script003_Create_Merge_Procedure.sql`, linha 76) sobrescreveria um com o outro.

**Linhas 72–78** — substituir por (mesmo estilo de PK composta usado em
`AfterSaleCourierAttributesFields`, linhas 31–34):

```sql
create table [aftersale].AfterSaleStatusReverses (
	integrated_or_updated_at datetime,
	id int not null,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(50),
	[description] nvarchar(250),
constraint [PK_AfterSaleStatusReverses] primary key (
	id asc,
	doc_company asc
));
```

A tabela `[untreated]` (linhas 64–70) não precisa mudar.

---

## 6. Bancos já existentes — novo script SQL

Os scripts `ScriptNNN_*.sql` só rodam uma vez; alterar o `Script004` não muda um banco onde ele
já foi executado. Se a tabela já existe em algum ambiente, crie um novo script, por exemplo
`Script009_Alter_AfterSaleStatusReverses_PK.sql`, na mesma pasta:

```sql
-- registros antigos foram gravados sem doc_company (o service só processava a 1ª empresa)
delete from [aftersale].AfterSaleStatusReverses where doc_company is null;
delete from [untreated].AfterSaleStatusReverses where doc_company is null;

declare @pk_name nvarchar(128) = (
	select name from sys.key_constraints
	where type = 'PK' and parent_object_id = object_id('[aftersale].AfterSaleStatusReverses')
);

if @pk_name is not null
	exec('alter table [aftersale].AfterSaleStatusReverses drop constraint [' + @pk_name + ']');

alter table [aftersale].AfterSaleStatusReverses alter column [doc_company] nvarchar(14) not null;

alter table [aftersale].AfterSaleStatusReverses
	add constraint [PK_AfterSaleStatusReverses] primary key (id asc, doc_company asc);

-- a procedure de merge é gerada a partir da PK; recriar para que passe a casar por (id, doc_company)
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleStatusReverses', @execute = 1;
```

> Antes de rodar, confira se o `sp_Create_Merge_Procedure` recria (`create or alter` / `drop`) a
> procedure quando ela já existe. Se não recriar, faça o `drop procedure` da
> `sp_AfterSaleStatusReverses_Sync` antes da última linha.
>
> Os `delete` são opcionais: se preferir manter os registros antigos, troque-os por um
> `update ... set doc_company = '<cnpj da empresa que era consultada>' where doc_company is null`.

---

## Checklist

- [ ] 1. DTO: `using Newtonsoft.Json;` + propriedade `doc_company` com `[JsonIgnore]`
- [ ] 2. Service: método `GetStatus` substituído (linhas 487–590)
- [ ] 3. Repository: parâmetro `doc_company` descomentado (linhas 157 e 159)
- [ ] 4. Command handler: query com `WHERE DOC_COMPANY` (linhas 83–84)
- [ ] 5. `Script004`: PK composta em `[aftersale].AfterSaleStatusReverses` (linhas 72–78)
- [ ] 6. Novo script de `ALTER` para bancos existentes + recriação da procedure de merge
- [ ] Compilar a solução
- [ ] Rodar o job `AfterSaleStatusReverses` e conferir na tabela trusted um conjunto de status por empresa

## Fora do escopo, mas visto no caminho

`GetTransportations` tem o mesmo desenho. Ele chama
`_afterSaleReversesRepository.GetTransportations(company.doc_company)` (service, linha 622),
mas a interface declara `GetTransportations()` sem parâmetro (linha 21). Também acumula listas
fora do `foreach` e tem PK só em `id` (`Script004`, linha 45). Vale aplicar as mesmas correções
depois.
