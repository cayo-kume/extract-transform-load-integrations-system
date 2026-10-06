# Padrão de Cache para Integrações (ETL)

Este documento define o **padrão de uso de cache** adotado nas integrações do sistema.
A implementação de referência é o método `GetReverses` de
[`AfterSaleReversesService.cs`](../src/modules/General/AfterSale/Application.AfterSale/Services/AfterSaleReversesService.cs).

Toda nova integração que precise de cache deve seguir este modelo.

---

## 1. Objetivos do cache

O cache serve a **dois propósitos** distintos em uma integração:

1. **Deduplicar o consumo da API** — não rebaixar/reprocessar páginas ou respostas já lidas
   dentro de uma janela de tempo (ex.: paginação com rate limit).
2. **Decidir o que é novo/atualizado** — manter em memória um retrato do que já está no banco
   (`id → estado atual`), para gravar apenas registros novos ou com data mais recente,
   evitando idas repetidas ao banco.

Tecnologia: `IMemoryCache` (cache em memória do processo, `Microsoft.Extensions.Caching.Memory`).

---

## 2. Convenções

### 2.1 Chaves de cache

Declarar as chaves como `private const string`, com prefixo do módulo e nome descritivo:

```csharp
private const string SummarizedReversesKey = "AfterSale:SummarizedReverses"; // cache de páginas da API
private const string ReversesKey           = "AfterSale:Reverses";           // cache de estado (id -> registro do banco)
private const string TransportationsHashKey = "AfterSale:TransportationsHash"; // cache de hash (payload inteiro)
private const string StatusHashKey          = "AfterSale:StatusHash";
```

Formato: `"<Modulo>:<Recurso>"`. Para hash de payload, sufixo `Hash`.

### 2.2 Política de expiração (padrão)

Toda entrada de cache usa expiração **combinada**:

```csharp
entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);   // teto rígido: morre em no máximo 2h
entry.SlidingExpiration               = TimeSpan.FromMinutes(30); // morre se ficar 30 min sem acesso
```

Vale **o que ocorrer primeiro**: 30 min de inatividade **ou** 2h totais.
Mantenha esses valores como padrão, salvo justificativa específica da integração.

---

## 3. Estratégias de cache

Escolha a estratégia conforme o tipo de dado.

### 3.1 Cache de páginas / deduplicação de respostas (`GetOrCreate` de dicionário)

Use quando a API é **paginada** e você quer evitar reprocessar páginas já lidas.

```csharp
var pageCache = _cache.GetOrCreate(SummarizedReversesKey, entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
    entry.SlidingExpiration = TimeSpan.FromMinutes(30);
    return new Dictionary<string, string>(); // chave: "empresa|pagina"  valor: JSON bruto
});

// ...ao processar cada página:
if (!pageCache.ContainsKey($"{company.doc_company}|{page.current_page}") &&
    !pageCache.ContainsValue(response)) // dedup por chave E por conteúdo
{
    pageCache.Add($"{company.doc_company}|{page.current_page}", response);
    simplifiedReverses.AddRange(page.data);
}
```

Pontos-chave:
- `GetOrCreate` retorna o dicionário existente **ou** cria (e cacheia) um novo.
- O dicionário é mantido **por referência**: `pageCache.Add(...)` já altera o objeto no cache,
  não é preciso reinserir.
- Dedup dupla: por **chave** (`ContainsKey`) e por **conteúdo** (`ContainsValue`), para o caso de
  a API retornar o mesmo payload com número de página diferente.

### 3.2 Cache de estado (`GetOrCreateAsync` → `Dictionary<chave, Entity>`)

Use para decidir o que é novo/atualizado sem consultar o banco repetidamente.

```csharp
var reversesCache = await _cache.GetOrCreateAsync(ReversesKey, async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

    var list = await _afterSaleReversesRepository.GetSummarizedReversesByIds(
        simplifiedReverses.Select(x => x.id).ToList()
    );

    return list
        .Where(x => x != null)
        .GroupBy(x => x.id)                        // agrupa por id (proteção contra duplicados)
        .ToDictionary(g => g.Key, g => g.First()); // Dictionary<long, Entity.Reverse>
});
```

Depois de persistir os registros novos, **atualize o dicionário em memória** para que a próxima
execução (dentro da janela de expiração) já os enxergue como atualizados:

```csharp
foreach (var s in completeReverses.Where(x => x.id != null))
    reversesCache[s.id] = s; // mesma referência do cache -> reflete imediatamente
```

### 3.3 Cache de hash (payload inteiro sem paginação)

Use quando o recurso é pequeno e você só quer saber **se mudou** desde a última execução
(ex.: `GetStatus`, `GetTransportations`).

```csharp
var hash = _logger.ComputeSha256Hash(response);
var cachedHash = _cache.Get<string>(StatusHashKey) ?? String.Empty;

if (!cachedHash.Equals(hash))
{
    // ...processa o payload novo...
    _cache.Set(StatusHashKey, hash, TimeSpan.FromHours(2));
}
```

---

## 4. Comparação DTO (API) × Entity (banco): conversões obrigatórias

**Atenção — erro comum.** Os DTOs de resposta da API costumam ter **todas as propriedades como
`string`**, enquanto as Entities do banco têm tipos fortes (`Int64`, `DateTime`, `decimal`...).
Ao comparar o item da API (`x`, DTO) com o registro do cache (`y`, Entity), é preciso **converter**
os campos do DTO usando os helpers de
[`ConverterValidatorsExtensions`](../src/shared/Domain.Shared/Extensions/ConverterValidatorsExtensions.cs):

| Helper | De | Para | Fallback se inválido |
|--------|----|------|----------------------|
| `ConvertToInt64Validation(string)`    | string | `Int64`    | `0` |
| `ConvertToInt32Validation(string)`    | string | `Int32`    | `0` |
| `ConvertToDateTimeValidation(string)` | string | `DateTime` | `1990-01-01` |
| `ConvertToDecimalValidation(string)`  | string | `decimal`  | `0` |

### Filtro padrão "manter só o que é novo/atualizado"

```csharp
simplifiedReverses.RemoveAll(x =>
   !string.IsNullOrEmpty(x.id) &&                                                  // 1) tem id
   reversesCache.TryGetValue(                                                       // 2) já existe no cache?
       ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&  //    (converte id string -> long)
   ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.updated_at)          // 3) versão da API é
       <= y.updated_at                                                              //    igual/mais antiga? -> remove
);
```

Comportamento do predicado (`RemoveAll` remove quando o predicado é `true`):

| Situação | Resultado |
|----------|-----------|
| `id` vazio/nulo | **Mantido** |
| `id` não está no cache (inédito) | **Mantido** |
| Existe, versão da API mais recente | **Mantido** |
| Existe, versão da API igual ou mais antiga | **Removido** |

Notas:
- A guarda `!string.IsNullOrEmpty(x.id)` evita casar com a chave `0` que
  `ConvertToInt64Validation` devolve para strings vazias/inválidas.
- Prefira `<=` (descarta igual **ou** mais antigo, processa só o estritamente mais novo).
  Use `<` apenas se a regra da integração exigir reprocessar quando a data for igual.
- Como `updated_at` na Entity é `DateTime` **não anulável**, datas ausentes viram o fallback
  (`1990-01-01`); avalie se isso é aceitável para a integração.

---

## 5. Checklist para aplicar em uma nova integração

- [ ] Declarar as chaves como `const string` no formato `"<Modulo>:<Recurso>"`.
- [ ] Injetar `IMemoryCache _cache` no construtor do service.
- [ ] Usar expiração padrão **2h absoluta / 30 min deslizante** em toda entrada.
- [ ] API paginada com rate limit? → **cache de páginas** (§3.1) com dedup por chave e conteúdo.
- [ ] Precisa decidir novo/atualizado sem bater no banco toda hora? → **cache de estado** (§3.2).
- [ ] Recurso pequeno, só saber se mudou? → **cache de hash** (§3.3).
- [ ] Ao comparar DTO (string) com Entity (tipada), **sempre** converter com
      `ConverterValidatorsExtensions` (§4).
- [ ] Após gravar novos registros, **atualizar o dicionário do cache** em memória.

---

## 6. Referências

- Implementação de referência: [`AfterSaleReversesService.GetReverses`](../src/modules/General/AfterSale/Application.AfterSale/Services/AfterSaleReversesService.cs)
- Conversores: [`ConverterValidatorsExtensions`](../src/shared/Domain.Shared/Extensions/ConverterValidatorsExtensions.cs)
- Entity de referência: [`Entities/.../SearchReverses/Reverse.cs`](../src/modules/General/AfterSale/Domain.AfterSale/Entities/Reverses/SearchReverses/Reverse.cs)
- DTO de referência: [`Dtos/.../SearchReverses/Reverse.cs`](../src/modules/General/AfterSale/Domain.AfterSale/Dtos/Reverses/SearchReverses/Reverse.cs)
