# Alterações — discernir por CNPJ (`doc_company`) em `GetReverses`, `GetReverse` e `GetCourierAttributes`

Guia para aplicação **manual**. As linhas indicadas referem-se ao estado atual do working tree
(branch `main`, já com as alterações de `GetStatus` e `GetTransportations` aplicadas — ver
[ALTERACOES-GETSTATUS-REVERSES.md](ALTERACOES-GETSTATUS-REVERSES.md) e
[ALTERACOES-GETTRANSPORTATIONS-REVERSES.md](ALTERACOES-GETTRANSPORTATIONS-REVERSES.md)).
Aplique **de baixo para cima** dentro de cada arquivo para que as linhas de cima não se desloquem.

Escopo: os três métodos restantes de `AfterSaleReversesService`. Os métodos `Put*`/`Post*` desse
service e todo o `AfterSaleRefundsService` ainda lançam `NotImplementedException`, então não há o
que adaptar neles por enquanto.

---

## Decisões de modelagem

1. **Nome da coluna: `doc_company`**, o mesmo de `AfterSaleStatusReverses` e
   `AfterSaleTransportationsReverses`. A coluna `cnpj_emp` que já existe em
   `AfterSaleSummarizedReverses` (e a propriedade da entidade `SearchReverses.Reverse`) é
   **renomeada** para `doc_company`. `AfterSaleParameters` continua com `cnpj_emp` — a query de
   empresas já o expõe como `DOC_COMPANY` — e `AfterSaleErrors` fica fora deste guia (ver "Fora do
   escopo").
2. **`doc_company` entra na chave primária** de todas as tabelas de reversas, porque o mesmo
   `reverse_id` pode existir em dois CNPJs diferentes.
3. **Todas as tabelas filhas recebem `doc_company`**, e as FKs passam a ser compostas
   (`doc_company` + colunas antigas). Sem isso, uma filha não saberia a qual das duas reversas de
   mesmo id pertence.
4. **Mesmo desenho dos métodos já corrigidos**: processamento por empresa dentro do `foreach`,
   chave de cache por empresa (`"<chave>:<doc_company>"`), consultas ao banco filtradas pelo CNPJ
   e contador `totalInseridos` para a mensagem final.

Tabelas afetadas (todas no `Script004`):

| Tabela | PK atual | PK nova | Unique / FK novas |
|---|---|---|---|
| `AfterSaleSummarizedReverses` | `id` | `doc_company, id` | — (coluna `cnpj_emp` renomeada) |
| `AfterSaleReverses` | `id` | `doc_company, id` | — |
| `AfterSaleReversesCustomers` | `reverse_id` (+ `id` unique) | `doc_company, reverse_id` | UQ `doc_company, id`; FK → `AfterSaleReverses` |
| `AfterSaleReversesDestinationsData` | `reverse_id` (+ `id` unique) | `doc_company, reverse_id` | UQ `doc_company, id`; FK → `AfterSaleReverses` |
| `AfterSaleReversesTrackings` | `reverse_id` (+ `id` unique) | `doc_company, reverse_id` | UQ `doc_company, id`; FK → `AfterSaleReverses` |
| `AfterSaleReversesTrackingHistories` | `id, reverse_id, tracking_id` | `doc_company, id, reverse_id, tracking_id` | FK → `AfterSaleReversesTrackings (doc_company, id)` |
| `AfterSaleReversesDestinations` | `reverse_id` (+ `id` unique) | `doc_company, reverse_id` | UQ `doc_company, id`; FK → `AfterSaleReverses` |
| `AfterSaleReversesEcommerceOrders` | `reverse_id` (+ `id` unique) | `doc_company, reverse_id` | UQ `doc_company, id`; FK → `AfterSaleReverses` |
| `AfterSaleReversesEcommerceOrdersPaymentsMethods` | `reverse_id, ecommerce_order_payment_id, ecommerce_order_id` | `doc_company` + as mesmas | FK → `AfterSaleReversesEcommerceOrders (doc_company, id)` |
| `AfterSaleReversesProducts` | `id, reverse_id` | `doc_company, id, reverse_id` | FK → `AfterSaleReverses` |
| `AfterSaleReversesProductsImages` | `id, reverse_id, reverse_product_id` | `doc_company` + as mesmas | FK → `AfterSaleReversesProducts (doc_company, id, reverse_id)` |
| `AfterSaleReversesStatusHistories` | `id, reverse_id` | `doc_company, id, reverse_id` | FK → `AfterSaleReverses` |
| `AfterSaleHistoriesReverses` | `id` | `doc_company, id` | — |
| `AfterSaleCourierAttributes` | `reverse_id` | `doc_company, reverse_id` | — |
| `AfterSaleCourierAttributesFields` | `id, reverse_id` | `doc_company, id, reverse_id` | FK → `AfterSaleCourierAttributes` |

> `AfterSaleHistoriesReverses` não tem entidade nem é usada por nenhum método hoje, mas recebe a
> coluna para já nascer no mesmo padrão.

Resumo do que muda:

| # | Arquivo(s) | O quê |
|---|---------|-------|
| 1 | Entidades de reversa e courier attributes (14 arquivos) | Propriedade `doc_company` + parâmetro no construtor, repassado às filhas |
| 2 | `IAfterSaleCommandHandler.cs` | `doc_company` nas assinaturas de 3 queries |
| 3 | `AfterSaleCommandHandler.cs` | `DOC_COMPANY` em 5 queries (+ 2 bugs de SQL) |
| 4 | `IAfterSaleReversesRepository.cs` | `doc_company` em 3 métodos e novo retorno em 1 |
| 5 | `AfterSaleReversesRepository.cs` | Repassar `doc_company` + não gravar filhas vazias |
| 6 | `AfterSaleReversesService.cs` | Reescrita de `GetReverses`, `GetReverse` e `GetCourierAttributes` |
| 7 | `Script004_Create_Reverses_Tables.sql` | Coluna `doc_company`, PKs, uniques e FKs compostas |
| 8 | Novo script SQL (bancos já existentes) | Migração completa + recriar procedures de merge |

---

## 1. Entidades

Pasta: [`Domain.AfterSale/Entities/Reverses`](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reverses)

### 1.1 Padrão aplicado a cada entidade

Em cada arquivo da tabela 1.2, aplique **de baixo para cima**:

1. **Criação de filhas** (quando houver): passar `doc_company` como primeiro argumento.
2. **Atribuição**: após a linha indicada, adicionar:

   ```csharp
               this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
   ```

3. **Construtor**: adicionar `string doc_company` como **primeiro** parâmetro.
4. **Propriedade**: antes da linha indicada, adicionar:

   ```csharp
           public string? doc_company { get; private set; }

   ```

> A ordem das propriedades não importa para o bulk insert: o `SqlBulkCopy` mapeia as colunas por
> nome (`SharedRepository.cs:45`).

### 1.2 Onde aplicar

| Arquivo | Propriedade (antes da linha) | Construtor (linha) | Atribuição (após a linha) | Criação de filhas (linha) |
|---|---|---|---|---|
| `GetReverse/Customer.cs` | 10 | 68 | 70 | — |
| `GetReverse/DestinationData.cs` | 10 | 40 | 42 | — |
| `GetReverse/EcommerceOrder.cs` | 10 | 42 | 44 | 73 |
| `GetReverse/EcommerceOrderPaymentMethods.cs` | 9 | 18 | 21 | — |
| `GetReverse/Image.cs` | 10 | 22 | 24 | — |
| `GetReverse/Product.cs` | 10 | 70 | 73 | 130 |
| `GetReverse/ReverseDestination.cs` | 10 | 34 | 36 | — |
| `GetReverse/StatusHistories.cs` | 10 | 35 | 38 | — |
| `GetReverse/Tracking.cs` | 10 | 44 | 47 | 79 |
| `GetReverse/TrackingHistory.cs` | 10 | 25 | 27 | — |
| `GetReverse/Reverse.cs` | 10 | 104 | 105 (`{`) | 178–191 |
| `CourierAttributes/Field.cs` | 10 | 23 | 27 | — |
| `CourierAttributes/CourierAttributes.cs` | 12 | 21 | 23 | 31 |

Construtores resultantes:

```csharp
public Customer(string doc_company, Dtos.Reverses.GetReverse.Customer customerDto, Int64 reverse_id)
public DestinationData(string doc_company, Dtos.Reverses.GetReverse.DestinationData destinationDataDto, Int64 reverse_id)
public EcommerceOrder(string doc_company, Dtos.Reverses.GetReverse.EcommerceOrder ecommerceOrderDto, Int64 reverse_id)
public EcommerceOrderPaymentMethods(string doc_company, Int64 reverse_id, Int64 ecommerce_order_id, string payment_method)
public Image(string doc_company, Dtos.Reverses.GetReverse.Image imageDto, Int64 reverse_id, Int64 reverse_product_id)
public Product(string doc_company, Dtos.Reverses.GetReverse.Product productDto)
public ReverseDestination(string doc_company, Dtos.Reverses.GetReverse.ReverseDestination reverseDestinationDto, Int64 reverse_id)
public StatusHistories(string doc_company, Dtos.Reverses.GetReverse.StatusHistories statusHistoriesDto)
public Tracking(string doc_company, Dtos.Reverses.GetReverse.Tracking trackingDto, List<Dtos.Reverses.GetReverse.TrackingHistory> trackingHistoriesDto)
public TrackingHistory(string doc_company, Dtos.Reverses.GetReverse.TrackingHistory trackingHistoryDto, Int64 reverse_id)
public Reverse(string doc_company, Dtos.Reverses.GetReverse.Reverse reverseDto, Dtos.Reverses.GetReverse.Customer customerDto, List<Dtos.Reverses.GetReverse.TrackingHistory> trackingHistoryDto)
public Field(string doc_company, Dtos.Reverses.CourierAttributes.Field fieldDto, Int64 reverse_id)
public CourierAttributes(string doc_company, Dtos.Reverses.CourierAttributes.CourierAttribute courierAttributeDto)
```

Criação de filhas:

- **`EcommerceOrder.cs`, linha 73:**

  ```csharp
                ecommerce_order_payment_methods.Add(new EcommerceOrderPaymentMethods(doc_company, reverse_id, ecommerce_order_id, x));
  ```

- **`Product.cs`, linha 130:**

  ```csharp
                images.Add(new Image(doc_company, x, reverse_id, id));
  ```

- **`Tracking.cs`, linha 79:**

  ```csharp
                    tracking_history.Add(new TrackingHistory(doc_company, x, reverse_id));
  ```

- **`CourierAttributes.cs`, linha 31:**

  ```csharp
                fields.Add(new Field(doc_company, x, reverse_id));
  ```

- **`Reverse.cs`, linhas 178–191** — substituir por:

  ```csharp
            destination_data = reverseDto.destination_data != null ? new DestinationData(doc_company, reverseDto.destination_data, id) : new DestinationData();
            ecommerce_order = reverseDto.ecommerce_order != null ? new EcommerceOrder(doc_company, reverseDto.ecommerce_order, id) : new EcommerceOrder();
            tracking = reverseDto.tracking != null ? new Tracking(doc_company, reverseDto.tracking, trackingHistoryDto) : new Tracking();
            customer = customerDto != null ? new Customer(doc_company, customerDto, id) : new Customer();
            reverse_destination = reverseDto.reverse_destination != null ? new ReverseDestination(doc_company, reverseDto.reverse_destination, id) : new ReverseDestination();

            reverseDto.products?.ForEach(x =>
            {
                products.Add(new Product(doc_company, x));
            });

            reverseDto.status_histories?.ForEach(x =>
            {
                status_histories.Add(new StatusHistories(doc_company, x));
            });
  ```

  > As filhas recebem o **parâmetro** `doc_company` sem truncar, porque cada uma já trunca no
  > próprio construtor. Os objetos vazios (`new Customer()` etc.) ficam com `doc_company` nulo
  > e são descartados no repositório (passo 5.5).

### 1.3 `SearchReverses/Reverse.cs` — renomear `cnpj_emp` → `doc_company`

Arquivo: [Reverse.cs](../src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reverses/SearchReverses/Reverse.cs)

- **Linha 42:** `this.doc_company = doc_company;`
- **Linha 39:** `public Reverse(string doc_company, Dtos.Reverses.SearchReverses.Reverse reverseDto)`
- **Linha 25:** `public string doc_company { get; private set; }`

---

## 2. Interface de queries — `Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs`

Arquivo: [IAfterSaleCommandHandler.cs](../Application.AfterSale/Abstractions/Handlers/Commands/IAfterSaleCommandHandler.cs)

**Linhas 8, 9 e 11** — trocar:

```csharp
        string CreateGetReversesByIdsQuery(List<string> reversesIds);
        string CreateGetCourierAttributesByReverseIdQuery(List<Int64> reversesIds);

        string CreateGetSummarizedReversesByIdsQuery(List<string> reversesIds);
```

por:

```csharp
        string CreateGetReversesByIdsQuery(string doc_company, List<string> reversesIds);
        string CreateGetCourierAttributesByReverseIdQuery(string doc_company, List<Int64> reversesIds);

        string CreateGetSummarizedReversesByIdsQuery(string doc_company, List<string> reversesIds);
```

---

## 3. Queries — `Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs`

Arquivo: [AfterSaleCommandHandler.cs](../Application.AfterSale/Handlers/Commands/AfterSaleCommandHandler.cs)

### 3.1 `CreateGetCourierAttributesByReverseIdQuery` — linhas 91 e 103

Além do CNPJ, a query atual seleciona `ID, UPDATED_AT`, colunas que **não existem** em
`AFTERSALECOURIERATTRIBUTES` (a tabela tem `reverse_id` e `fill_form_from_file`).

Linha 91:

```csharp
        public string CreateGetCourierAttributesByReverseIdQuery(string doc_company, List<Int64> reversesIds)
```

Linha 103:

```csharp
            return $"SELECT REVERSE_ID, DOC_COMPANY, FILL_FORM_FROM_FILE FROM [AFTERSALE].[AFTERSALECOURIERATTRIBUTES] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND REVERSE_ID IN ({identificadores})";
```

### 3.2 `CreateGetSummarizedReversesByIdsQuery` — linhas 66 e 78

Linha 66:

```csharp
        public string CreateGetSummarizedReversesByIdsQuery(string doc_company, List<string> reversesIds)
```

Linha 78:

```csharp
            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALESUMMARIZEDREVERSES] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
```

### 3.3 `CreateGetSummarizedReversesIdsForGetCourierAttributesQuery` — linhas 54–64

A query atual tem dois erros que a impedem de rodar: `WEHRE` em vez de `WHERE`, e `B.ID`, coluna
que não existe em `AFTERSALECOURIERATTRIBUTES`. Além disso, só devolve o `ID`, então o service não
sabe a qual empresa a reversa pertence e acaba chamando a API com o token de todas.

Substituir o método inteiro por:

```csharp
        public string CreateGetSummarizedReversesIdsForGetCourierAttributesQuery()
        {
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM 
                        [AFTERSALE].[AFTERSALESUMMARIZEDREVERSES] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALECOURIERATTRIBUTES] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.REVERSE_ID
                      WHERE
                        B.REVERSE_ID IS NULL";
        }
```

> `TOP 30` segue o mesmo limite de `CreateGetSummarizedReversesIdsForGetReverseQuery`: a API da
> AfterSale devolve *Too Many Requests* depois de cerca de 30 consultas
> (`AfterSaleReversesService.cs:140`), e sem o limite o job faria uma chamada para cada reversa
> sem courier attributes. Se preferir manter sem limite, remova o `TOP 30`.

### 3.4 `CreateGetSummarizedReversesIdsForGetReverseQuery` — linhas 41–52

Substituir o método inteiro por:

```csharp
        public string CreateGetSummarizedReversesIdsForGetReverseQuery()
        {
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM 
                        [AFTERSALE].[AFTERSALESUMMARIZEDREVERSES] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALEREVERSES] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.ID
                      WHERE
                        B.ID IS NULL";
        }
```

### 3.5 `CreateGetReversesByIdsQuery` — linhas 26 e 38

Linha 26:

```csharp
        public string CreateGetReversesByIdsQuery(string doc_company, List<string> reversesIds)
```

Linha 38:

```csharp
            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALEREVERSES] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
```

---

## 4. Interface do repositório — `Domain.AfterSale/Abstractions/Repositorys/IAfterSaleReversesRepository.cs`

Arquivo: [IAfterSaleReversesRepository.cs](../Domain.AfterSale/Abstractions/Repositorys/IAfterSaleReversesRepository.cs)

**Linhas 24–25** — trocar:

```csharp
        public Task<IEnumerable<Int64>> GetSummarizedReversesIdsForGetCourierAttributes();
        public Task<List<Entities.Reverses.CourierAttributes.CourierAttributes?>> GetCourierAttributesByReverseId(List<Int64> reversesIds);
```

por:

```csharp
        public Task<IEnumerable<Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetCourierAttributes();
        public Task<List<Entities.Reverses.CourierAttributes.CourierAttributes?>> GetCourierAttributesByReverseId(string doc_company, List<Int64> reversesIds);
```

**Linha 15** — trocar:

```csharp
        public Task<List<Entities.Reverses.GetReverse.Reverse?>> GetReversesByIds(List<string> reversesIds);
```

por:

```csharp
        public Task<List<Entities.Reverses.GetReverse.Reverse?>> GetReversesByIds(string doc_company, List<string> reversesIds);
```

**Linha 12** — trocar:

```csharp
        public Task<List<Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesByIds(List<string> reversesIds);
```

por:

```csharp
        public Task<List<Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesByIds(string doc_company, List<string> reversesIds);
```

---

## 5. Repositório — `Infrastructure.AfterSale/Repositorys/AfterSaleReversesRepository.cs`

Arquivo: [AfterSaleReversesRepository.cs](../Infrastructure.AfterSale/Repositorys/AfterSaleReversesRepository.cs)

### 5.1 `GetCourierAttributesByReverseId` — linhas 201, 212 e 221

```csharp
        public async Task<List<Domain.AfterSale.Entities.Reverses.CourierAttributes.CourierAttributes?>> GetCourierAttributesByReverseId(string doc_company, List<Int64> reversesIds)
```

```csharp
                    string sql = _afterSaleCommandHandler.CreateGetCourierAttributesByReverseIdQuery(doc_company, top1000List);
```

```csharp
                string sql = _afterSaleCommandHandler.CreateGetCourierAttributesByReverseIdQuery(doc_company, reversesIds);
```

### 5.2 `BulkInsertIntoAfterSaleReverses` — linhas 119, 120, 121, 123 e 124

O construtor de `GetReverse.Reverse` cria objetos **vazios** (`new Customer()`, `new Tracking()`...)
quando a API não manda aquele bloco. Hoje eles já iriam para o banco com `reverse_id = 0`; com
`doc_company` na PK e `not null`, o merge falharia. Descarte-os antes do bulk insert:

```csharp
            _coreRepository.FillSystemDataTable(customersTable, data.Select(x => x.customer).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(destinationsDataTable, data.Select(x => x.destination_data).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(trackingsTable, data.Select(x => x.tracking).Where(x => x.doc_company != null).ToList());
```

(linha 122, `trackingsHistoriesTable`, não muda)

```csharp
            _coreRepository.FillSystemDataTable(destinationsTable, data.Select(x => x.reverse_destination).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(ecommerceOrdersTable, data.Select(x => x.ecommerce_order).Where(x => x.doc_company != null).ToList());
```

> As listas de histórico, pagamentos, produtos e imagens vêm vazias nesses casos, então não
> precisam de filtro.

### 5.3 `GetReversesByIds` — linhas 76, 87 e 96

```csharp
        public async Task<List<Domain.AfterSale.Entities.Reverses.GetReverse.Reverse?>> GetReversesByIds(string doc_company, List<string> reversesIds)
```

```csharp
                    string sql = _afterSaleCommandHandler.CreateGetReversesByIdsQuery(doc_company, top1000List);
```

```csharp
                string sql = _afterSaleCommandHandler.CreateGetReversesByIdsQuery(doc_company, reversesIds);
```

### 5.4 `GetSummarizedReversesByIds` — linhas 35, 46 e 55

```csharp
        public async Task<List<Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesByIds(string doc_company, List<string> reversesIds)
```

```csharp
                    string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesByIdsQuery(doc_company, top1000List);
```

```csharp
                string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesByIdsQuery(doc_company, reversesIds);
```

### 5.5 `GetSummarizedReversesIdsForGetCourierAttributes` — linhas 28–33

Passa a devolver `DOC_COMPANY` + `ID`, igual a `GetSummarizedReversesIdsForGetReverse` (linhas 21–26):

```csharp
        public async Task<IEnumerable<Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetCourierAttributes()
        {
            string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesIdsForGetCourierAttributesQuery();

            return await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse>(sql);
        }
```

---

## 6. Service — `Application.AfterSale/Services/AfterSaleReversesService.cs`

Arquivo: [AfterSaleReversesService.cs](../Application.AfterSale/Services/AfterSaleReversesService.cs)

Substitua **de baixo para cima**: primeiro 6.1 (`GetCourierAttributes`), depois 6.2
(`GetReverse`) e por último 6.3 (`GetReverses`).

### 6.1 `GetCourierAttributes` — substituir as linhas 388–486

Do `/// <summary>` do `GetCourierAttributes` até o `}` que fecha o método, imediatamente antes do
`/// <summary>` do `GetStatus`:

```csharp
        /// <summary>
        /// get courier data fields needed to process the reverse, for each company
        /// </summary>
        public async Task<bool?> GetCourierAttributes()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleCourierAttributes);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var summarizedReverses = await _afterSaleReversesRepository.GetSummarizedReversesIdsForGetCourierAttributes();

            foreach (var reversesByCompany in summarizedReverses.Where(x => x != null).GroupBy(x => x!.doc_company))
            {
                var company = companys.FirstOrDefault(x => x.doc_company == reversesByCompany.Key);

                if (company is null)
                {
                    _logger.AddMessage($"CNPJ {reversesByCompany.Key} não encontrado em AfterSaleParameters: {reversesByCompany.Count()} reversa(s) ignorada(s).\n");
                    continue;
                }

                var courierAttributes = new List<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute>();

                foreach (var summarizedReverse in reversesByCompany)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/reverses/{summarizedReverse!.id}/get-courier-attributes",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var courierAttribute = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute>(response);

                    if (courierAttribute?.reverse is null)
                        continue;

                    courierAttributes.Add(courierAttribute);
                }

                if (courierAttributes.Count == 0)
                    continue;

                var courierAttributesCache = await _cache.GetOrCreateAsync($"{CourierAttributesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetCourierAttributesByReverseId(
                        company.doc_company,
                        reversesByCompany.Select(x => x!.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.reverse_id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...CourierAttributes>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.CourierAttributes.CourierAttributes>();

                foreach (var courierAttribute in courierAttributes)
                {
                    var validations = _courierAttributeValidator.Validate(courierAttribute);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.CourierAttributes.CourierAttributes(company.doc_company, courierAttribute));
                }

                // mantém só as reversas que ainda não têm courier attributes gravados para a empresa
                _listSomenteNovos.RemoveAll(x => courierAttributesCache.ContainsKey(x.reverse_id));

                if (_listSomenteNovos.Count() > 0)
                {
                    //await _afterSaleReversesRepository.BulkInsertIntoAfterSaleCourierAttributes(_listSomenteNovos, _logger.GetExecutionGuid());

                    //foreach (var record in _listSomenteNovos)
                    //{
                    //    _logger.AddRecord(record.reverse_id.ToString(), ...);
                    //    courierAttributesCache[record.reverse_id] = record;
                    //}

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

| Linha(s) antiga(s) | Antes | Depois | Motivo |
|---|---|---|---|
| 397–401 | Listas e `listResponses` fora de qualquer laço | Listas por empresa; `listResponses` removido (não era usado) | Processamento por empresa. |
| 403–406 | Para **cada** reversa, um laço por **todas** as empresas | Agrupa as reversas por `doc_company` e usa só o token da empresa dona | Antes, cada reversa era consultada com o token de todas as empresas: chamadas em dobro e respostas de erro tratadas como dados válidos. |
| 407–415 | Rota `/v3/api/reverses?id=123/get-courier-attributes` | `v3/api/reverses/{id}/get-courier-attributes` | A rota montada era inválida (o `id` ia na query string, com o sufixo colado nela). **Confirme a rota na documentação da API Send4.** |
| 422–424 | Adicionava o DTO mesmo se a desserialização falhasse | Ignora resposta sem `reverse` | O construtor da entidade acessa `courierAttributeDto.reverse.id`. |
| 429 | Chave de cache única | `$"{CourierAttributesKey}:{company.doc_company}"` | Cache por empresa. Como o mesmo `reverse_id` pode existir nas duas empresas, um cache único confundiria as duas. |
| 434–436 | `GetCourierAttributesByReverseId(ids)` | `GetCourierAttributesByReverseId(company.doc_company, ids)` | Consulta filtrada pela empresa. |
| 444–449 | Filtro de "só novos" comentado | `RemoveAll` por `reverse_id` já gravado | Courier attributes não têm `updated_at`; a regra é "existe ou não existe" para a reversa. |
| 465 | `new CourierAttributes(courierAttribute)` | `new CourierAttributes(company.doc_company, courierAttribute)` | Grava a empresa (e repassa para os `Field`). |
| 470–475 | Insert comentado | **Continua comentado** | Não existe `BulkInsertIntoAfterSaleCourierAttributes` no repositório. Ver "Fora do escopo". |

### 6.2 `GetReverse` — substituir as linhas 264–386

Do `/// <summary>` do `GetReverse` até o `}` que fecha o método, imediatamente antes do
`/// <summary>` do `GetCourierAttributes`:

```csharp
        /// <summary>
        /// get the details of the summarized reverses not yet integrated, for each company
        /// </summary>
        public async Task<bool?> GetReverse()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleReverse);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var summarizedReverses = await _afterSaleReversesRepository.GetSummarizedReversesIdsForGetReverse();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var reversesByCompany in summarizedReverses.Where(x => x != null).GroupBy(x => x!.doc_company))
            {
                var company = companys.FirstOrDefault(x => x.doc_company == reversesByCompany.Key);

                if (company is null)
                {
                    _logger.AddMessage($"CNPJ {reversesByCompany.Key} não encontrado em AfterSaleParameters: {reversesByCompany.Count()} reversa(s) ignorada(s).\n");
                    continue;
                }

                var reversesDtosList = new List<Domain.AfterSale.Dtos.Reverses.GetReverse.Root>();
                var reversesJson = new Dictionary<long, string>();

                foreach (var summarizedReverse in reversesByCompany)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/reverses/{summarizedReverse!.id}",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.GetReverse.Root>(response);

                    if (root is null || !root.success || root.data?.reverse is null)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleReverse,
                            _logger.GetExecutionGuid(),
                            summarizedReverse.id,
                            root?.error,
                            root?.error_code,
                            root?.message,
                            root?.message_translated
                        ));
                        continue;
                    }

                    reversesDtosList.Add(root);
                    reversesJson[summarizedReverse.id] = JObject.Parse(response)["data"]?["reverse"]?.ToString() ?? response;
                }

                if (reversesDtosList.Count == 0)
                    continue;

                var reversesCache = await _cache.GetOrCreateAsync($"{ReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetReversesByIds(
                        company.doc_company,
                        reversesDtosList.Select(x => x.data.reverse.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...Reverse>
                });

                reversesDtosList.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.data.reverse.id) &&
                   reversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.data.reverse.id), out var y) &&
                       ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.data.reverse.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.GetReverse.Reverse>();

                foreach (var reversesDto in reversesDtosList)
                {
                    var validations = _reversesValidator.Validate(reversesDto.data.reverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.GetReverse.Reverse(company.doc_company, reversesDto.data.reverse, reversesDto.data.customer, reversesDto.data.tracking_history));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (reversesJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        reversesCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
```

| Linha(s) antiga(s) | Antes | Depois | Motivo |
|---|---|---|---|
| 274–279 | Listas únicas para todas as empresas | Agrupa as reversas por `doc_company`; listas por empresa | Processamento por empresa. O dicionário `reversesJson` e o cache são indexados pelo `id`, o que só é seguro **dentro** de uma empresa. |
| 286 | Rota `v3/api/reverses/{summarizedReverse}` | `v3/api/reverses/{summarizedReverse!.id}` | `summarizedReverse` é a **entidade** inteira; interpolada, vira o nome do tipo (`Domain.AfterSale.Entities...Reverse`) e a rota fica errada. |
| 287, 298 | `companys.Where(...).First()` a cada chamada, comparando com `cnpj_emp` | `company` resolvido uma vez por grupo, com tratamento se não existir | `First()` lançava exceção se o CNPJ não estivesse em `AfterSaleParameters`, derrubando o job inteiro. |
| 295 | Só checava `success` | Checa também `root` nulo e `data.reverse` nulo | Evita `NullReferenceException` no cache e no construtor. |
| 310 | `listResponses.Add(response)` | `reversesJson[id] = <json da reversa>` | Ver linhas 362–367. |
| 314 | Chave de cache única | `$"{ReversesKey}:{company.doc_company}"` | Cache por empresa. |
| 319–321 | `GetReversesByIds(ids)` | `GetReversesByIds(company.doc_company, ids)` + guarda para lista vazia | Consulta filtrada; com lista vazia a query viraria `IN ()`, que é SQL inválido. |
| 329–330 | `x != null` | `!string.IsNullOrEmpty(x.data.reverse.id)` | Mesma guarda do padrão de cache (§4 de `PADRAO-CACHE-INTEGRACOES.md`). |
| 350 | Construtor sem CNPJ | `new Reverse(company.doc_company, ...)` | Grava a empresa na reversa e em todas as filhas. |
| 356 | Erros só eram gravados se houvesse registro novo | Erros gravados ao final, sempre que existirem | Antes, uma execução em que todas as reversas falhavam não registrava nenhum erro. |
| 362–367 | `data?.FirstOrDefault(r => (long?)r["id"] == ...)` sobre `["data"]["reverse"]` | Busca no dicionário `reversesJson` | `["data"]["reverse"]` é um **objeto**, não um array: iterar sobre ele devolve `JProperty`, e `r["id"]` lança `InvalidOperationException`. |
| 369–373 | Cache só atualizado se o JSON fosse encontrado | Cache sempre atualizado após o insert | O registro foi gravado; o cache tem que refletir isso. |
| 379 | `_listSomenteNovos.Count` | `totalInseridos` | Mensagem final soma todas as empresas. |

### 6.3 `GetReverses` — substituir as linhas 80–262

Do `/// <summary>` do `GetReverses` até o `}` que fecha o método, imediatamente antes do
`/// <summary>` do `GetReverse`:

```csharp
        /// <summary>
        /// returns all reverses summarized, for each company
        /// </summary>
        public async Task<bool?> GetReverses()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleReverses);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var company in companys)
            {
                var summarizedReverses = new List<Domain.AfterSale.Dtos.Reverses.SearchReverses.Reverse>();
                var listResponses = new List<string>();

                var parameters = new Dictionary<string, string>
                {
                    //{ "start_date", $"{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}" },
                    //{ "end_date", $"{DateTime.Now.ToString("yyyy-MM-dd")}" }

                    { "start_date", $"{DateTime.Now.AddMonths(-1).ToString()}" },
                    { "end_date", $"{DateTime.Now.ToString()}" }
                };

                var encodedParameters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

                var response = await _apiClient.GetAsync(
                    route: $"v3/api/reverses?{encodedParameters}",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var page = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.SearchReverses.Root>(response);

                if (page is null || !page.success)
                {
                    _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                        company.doc_company,
                        EnumJob.AfterSaleReverses,
                        _logger.GetExecutionGuid(),
                        page?.error,
                        page?.error_code,
                        page?.message,
                        page?.message_translated
                    ));
                    continue;
                }

                int contador = 0;
                string? rote = page.next_page_url;
                summarizedReverses.AddRange(page.data ?? []);
                listResponses.Add(response);

                while (page.last_page > 1 && !String.IsNullOrEmpty(rote))
                {
                    //API da AfterSale lança Too Many Requests após 30 consultas, por isso caso tenha dado erro na consuta das páginas aguardamos por 1 minuto
                    if (contador > 14)
                    {
                        Thread.Sleep(60 * 1000);
                        contador = 0;
                    }

                    var responseByPage = await _apiClient.GetAsync(
                        route: rote.Replace("https://api.send4.com.br/", ""),
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    var nextPage = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.SearchReverses.Root>(responseByPage);

                    if (nextPage is null || !nextPage.success)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleReverses,
                            _logger.GetExecutionGuid(),
                            nextPage?.error,
                            nextPage?.error_code,
                            nextPage?.message,
                            nextPage?.message_translated
                        ));
                        break;
                    }

                    contador++;
                    rote = nextPage.next_page_url;
                    summarizedReverses.AddRange(nextPage.data ?? []);
                    listResponses.Add(responseByPage);
                    Thread.Sleep(2 * 1000);
                }

                if (summarizedReverses.Count == 0)
                    continue;

                var summarizedReversesCache = await _cache.GetOrCreateAsync($"{SummarizedReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetSummarizedReversesByIds(
                        company.doc_company,
                        summarizedReverses.Select(x => x.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...SearchReverses.Reverse>
                });

                summarizedReverses.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.id) &&
                   summarizedReversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
                       ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse>();

                foreach (var summarizedReverse in summarizedReverses)
                {
                    var validations = _summarizedReversesValidator.Validate(summarizedReverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse(company.doc_company, summarizedReverse));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleSummarizedReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    // índice id -> json da reversa, montado só com as páginas desta empresa
                    var reversesJson = new Dictionary<long, string>();

                    foreach (var pageResponse in listResponses)
                    {
                        foreach (var item in (JObject.Parse(pageResponse)["data"] as JArray) ?? new JArray())
                        {
                            var id = (long?)item["id"];

                            if (id != null)
                                reversesJson[id.Value] = item.ToString();
                        }
                    }

                    foreach (var record in _listSomenteNovos)
                    {
                        if (reversesJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        summarizedReversesCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
```

| Linha(s) antiga(s) | Antes | Depois | Motivo |
|---|---|---|---|
| 89–93 | `listResponses`, `dictionarysDocCompanyReverses` e `_listSomenteNovos` compartilhados entre empresas | Tudo por empresa, dentro do `foreach`; o dicionário intermediário deixa de existir | Processamento por empresa. Também elimina o `Dictionary.Add` da linha 178, que lançava exceção se `AfterSaleParameters` tivesse o mesmo CNPJ com dois tokens (a query usa `DISTINCT CNPJ_EMP, TOKEN`). |
| 117 | `!page.success` | `page is null \|\| !page.success` | Resposta vazia ou inválida derrubava o job com `NullReferenceException`. |
| 136–138 | `if (last_page > 1) { while (...) }` | `while (last_page > 1 && ...)` | Mesmo comportamento, um nível a menos. |
| 154–166 | Página com erro: `continue` | `break` | **Laço infinito**: o `continue` voltava ao `while` sem mudar `rote`, pedindo a mesma página para sempre (e registrando um erro por volta). |
| 133, 170 | `AddRange(page.data)` | `AddRange(page.data ?? [])` | `data` nulo lançava exceção. |
| 181 | Chave de cache única, carregada **uma vez** com as reversas de todas as empresas | `$"{SummarizedReversesKey}:{company.doc_company}"`, carregada por empresa | Cache por empresa: com o mesmo `id` podendo existir nas duas empresas, um dicionário único por `id` misturaria as duas. |
| 186–189 | `GetSummarizedReversesByIds(ids de todas as empresas)` | `GetSummarizedReversesByIds(company.doc_company, ids da empresa)` + guarda para lista vazia | Consulta filtrada; com lista vazia a query viraria `IN ()`, que é SQL inválido. |
| 187–188 | Comentário "reverse id não replica entre os cnpjs" | Removido | A premissa não vale: o mesmo id pode existir nos dois CNPJs. |
| 211–225 | `simplifiedReverse` / `cnpj_emp` vindo da chave do dicionário | `summarizedReverse` / `company.doc_company` | Mesmo valor, sem o dicionário intermediário. |
| 233 | Erros só eram gravados se houvesse registro novo | Erros gravados ao final, sempre que existirem | Antes, se a API de uma empresa falhasse e não houvesse nada novo nas outras, o erro se perdia. |
| 235–251 | Para cada registro, parse de **todas** as páginas de **todas** as empresas | Índice `id → json` montado uma vez, só com as páginas da empresa | Antes era O(registros × páginas) parses de JSON, e com ids repetidos entre empresas o log poderia pegar o JSON da empresa errada. O cache também passa a ser atualizado sempre após o insert. |
| 255 | `_listSomenteNovos.Count` | `totalInseridos` | Mensagem final soma todas as empresas. |

---

## 7. Script de criação — `Script004_Create_Reverses_Tables.sql`

Arquivo: [Script004_Create_Reverses_Tables.sql](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script004_Create_Reverses_Tables.sql)

Aplique **de baixo para cima**. Em todas as tabelas `[untreated]`, basta adicionar a coluna
(anulável, como as demais colunas dessas tabelas). Nas tabelas `[general]`, a coluna é
`not null` e entra na PK. As constraints passam a ter nome explícito (`PK_`, `UQ_`), o que
facilita futuras migrações.

### 7.1 FKs — linhas 810–820

Substituir o bloco inteiro por:

```sql
alter table [aftersale].AfterSaleCourierAttributesFields add constraint [FK_AfterSaleCourierAttributesFields] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleCourierAttributes] (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesCustomers] add constraint [FK_AfterSaleReversesCustomers] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesDestinationsData] add constraint [FK_AfterSaleReversesDestinationsData] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesTrackings] add constraint [FK_AfterSaleReversesTrackings] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesTrackingHistories] add constraint [FK_AfterSaleReversesTrackingHistories] foreign key (doc_company, tracking_id) references [aftersale].[AfterSaleReversesTrackings] (doc_company, id);
alter table [aftersale].[AfterSaleReversesDestinations] add constraint [FK_AfterSaleReversesDestinations] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesEcommerceOrders] add constraint [FK_AfterSaleReversesEcommerceOrders] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] add constraint [FK_AfterSaleReversesEcommerceOrdersPaymentsMethods] foreign key (doc_company, ecommerce_order_id) references [aftersale].[AfterSaleReversesEcommerceOrders] (doc_company, id);
alter table [aftersale].[AfterSaleReversesProducts] add constraint [FK_AfterSaleReversesProducts] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesProductsImages] add constraint [FK_AfterSaleReversesProductsImages] foreign key (doc_company, reverse_product_id, reverse_id) references [aftersale].[AfterSaleReversesProducts] (doc_company, id, reverse_id);
alter table [aftersale].[AfterSaleReversesStatusHistories] add constraint [FK_AfterSaleReversesStatusHistories] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
```

> As FKs que apontam para `id` de Trackings e EcommerceOrders dependem das uniques
> `UQ_..._(doc_company, id)` criadas abaixo.

### 7.2 `[aftersale].[AfterSaleReversesStatusHistories]` — linhas 785–808

- Após a linha 785 (`integrated_or_updated_at datetime,`): `[doc_company] nvarchar(14) not null,`
- Linhas 805–808 → substituir por:

  ```sql
  constraint [PK_AfterSaleReversesStatusHistories] primary key (
  	doc_company asc,
  	id asc,
  	reverse_id asc
  ));
  ```

`[untreated]` (após a linha 760): `[doc_company] nvarchar(14),`

### 7.3 `[aftersale].[AfterSaleReversesProductsImages]` — linhas 745–757

- Após a linha 746: `[doc_company] nvarchar(14) not null,`
- Linha 752 (`image_url nvarchar(300)`) → `image_url nvarchar(300),` (falta a vírgula antes da constraint)
- Linhas 753–757 → substituir por:

  ```sql
  constraint [PK_AfterSaleReversesProductsImages] primary key (
  	doc_company asc,
  	id asc,
  	reverse_id asc,
  	reverse_product_id asc
  ));
  ```

`[untreated]` (após a linha 736): `[doc_company] nvarchar(14),`

### 7.4 `[aftersale].[AfterSaleReversesProducts]` — linhas 680–733

- Após a linha 681: `[doc_company] nvarchar(14) not null,`
- Linha 729 (`original_qty int`) → `original_qty int,` (falta a vírgula antes da constraint)
- Linhas 730–733 → substituir por:

  ```sql
  constraint [PK_AfterSaleReversesProducts] primary key (
  	doc_company asc,
  	id asc,
  	reverse_id asc
  ));
  ```

`[untreated]` (após a linha 629): `[doc_company] nvarchar(14),`

### 7.5 `[aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods]` — linhas 616–626

- Após a linha 617: `[doc_company] nvarchar(14) not null,`
- Linhas 622–626 → substituir por:

  ```sql
  constraint [PK_AfterSaleReversesEcommerceOrdersPaymentsMethods] primary key (
  	doc_company asc,
  	reverse_id asc,
  	ecommerce_order_payment_id asc,
  	ecommerce_order_id asc
  ));
  ```

`[untreated]` (após a linha 609): `[doc_company] nvarchar(14),`

### 7.6 Tabelas 1:1 com a reversa (PK em `reverse_id` + `id` unique)

Vale para `AfterSaleReversesEcommerceOrders`, `AfterSaleReversesDestinations`,
`AfterSaleReversesTrackings`, `AfterSaleReversesDestinationsData` e `AfterSaleReversesCustomers`.
Em cada `[general]`:

1. Após `integrated_or_updated_at datetime,`: `[doc_company] nvarchar(14) not null,`
2. `reverse_id bigint not null primary key,` → `reverse_id bigint not null,`
3. `id bigint not null unique,` → `id bigint not null,`
4. Na última coluna, adicionar a vírgula e, antes do `);`, as constraints:

   ```sql
   constraint [PK_<Tabela>] primary key (
   	doc_company asc,
   	reverse_id asc
   ),
   constraint [UQ_<Tabela>] unique (
   	doc_company asc,
   	id asc
   ));
   ```

| Tabela `[general]` | `integrated_or_updated_at` | `reverse_id ... primary key` | `id ... unique` | Última coluna | `);` | `[untreated]`: `doc_company` após a linha |
|---|---|---|---|---|---|---|
| `AfterSaleReversesEcommerceOrders` | 586 | 587 | 589 | 605 | 606 | 563 |
| `AfterSaleReversesDestinations` | 541 | 542 | 543 | 559 | 560 | 519 |
| `AfterSaleReversesTrackings` | 462 | 463 | 464 | 485 | 486 | 435 |
| `AfterSaleReversesDestinationsData` | 408 | 409 | 410 | 431 | 432 | 381 |
| `AfterSaleReversesCustomers` | 325 | 326 | 327 | 377 | 378 | 269 |

Exemplo completo do fechamento de `AfterSaleReversesTrackings` (linhas 485–486):

```sql
	deleted_at datetime,
constraint [PK_AfterSaleReversesTrackings] primary key (
	doc_company asc,
	reverse_id asc
),
constraint [UQ_AfterSaleReversesTrackings] unique (
	doc_company asc,
	id asc
));
```

### 7.7 `[aftersale].[AfterSaleReversesTrackingHistories]` — linhas 501–516

- Após a linha 502: `[doc_company] nvarchar(14) not null,`
- Linha 511 (`deleted_at datetime`) → `deleted_at datetime,` (falta a vírgula antes da constraint)
- Linhas 512–516 → substituir por:

  ```sql
  constraint [PK_AfterSaleReversesTrackingHistories] primary key (
  	doc_company asc,
  	id asc,
  	reverse_id asc,
  	tracking_id asc
  ));
  ```

`[untreated]` (após a linha 489): `[doc_company] nvarchar(14),`

### 7.8 `[aftersale].[AfterSaleReverses]` — linhas 200–266

- Após a linha 201: `[doc_company] nvarchar(14) not null,`
- Linha 202: `id bigint not null primary key,` → `id bigint not null,`
- Linha 265 (`retry_options_can_retry bit`) → `retry_options_can_retry bit,`
- Linha 266 (`);`) → substituir por:

  ```sql
  constraint [PK_AfterSaleReverses] primary key (
  	doc_company asc,
  	id asc
  ));
  ```

`[untreated]` (após a linha 133): `[doc_company] nvarchar(14),`

### 7.9 `AfterSaleSummarizedReverses` — linhas 84–130

`[general]`:

- Linha 110: `cnpj_emp nvarchar(14),` → `[doc_company] nvarchar(14) not null,`
- Linha 111: `id bigint not null primary key,` → `id bigint not null,`
- Linha 129 (`tracking_code nvarchar(50)`) → `tracking_code nvarchar(50),`
- Linha 130 (`);`) → substituir por:

  ```sql
  constraint [PK_AfterSaleSummarizedReverses] primary key (
  	doc_company asc,
  	id asc
  ));
  ```

`[untreated]` — linha 86: `cnpj_emp nvarchar(14),` → `[doc_company] nvarchar(14),`

### 7.10 `AfterSaleHistoriesReverses` — linhas 51–63

`[general]` (linhas 58–63) → substituir por:

```sql
create table [aftersale].AfterSaleHistoriesReverses(
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	id bigint not null,
	reverse_id bigint not null,
	link nvarchar(250),
constraint [PK_AfterSaleHistoriesReverses] primary key (
	doc_company asc,
	id asc
));
```

`[untreated]` (após a linha 52): `[doc_company] nvarchar(14),`

### 7.11 `AfterSaleCourierAttributesFields` — linhas 13–34

`[general]` (linhas 23–34) → substituir por:

```sql
create table [aftersale].AfterSaleCourierAttributesFields (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	[required] bit,
	id nvarchar(50) not null,
	[name] nvarchar(250),
	[type] nvarchar(50),
	mime nvarchar(50),
	reverse_id bigint not null,
constraint [PK_AfterSaleCourierAttributesFields] primary key (
	doc_company asc,
	id asc,
	reverse_id asc
)); 
```

`[untreated]` (após a linha 14): `[doc_company] nvarchar(14),`

### 7.12 `AfterSaleCourierAttributes` — linhas 1–11

Substituir por:

```sql
create table [untreated].AfterSaleCourierAttributes (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	reverse_id bigint,
	fill_form_from_file nvarchar(50)
);

create table [aftersale].AfterSaleCourierAttributes (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	reverse_id bigint not null,
	fill_form_from_file nvarchar(50),
constraint [PK_AfterSaleCourierAttributes] primary key (
	doc_company asc,
	reverse_id asc
));
```

---

## 8. Bancos já existentes — novo script SQL

Os scripts `ScriptNNN_*.sql` são executados pelo DbUp (`migrator.csx`) uma única vez cada; alterar
o `Script004` não muda um banco onde ele já foi executado. Crie um novo script com o próximo
número livre (por exemplo, `Script011_Add_DocCompany_To_Reverses_Tables.sql`, se o `Script009` e
o `Script010` dos outros guias já existirem).

O `go` é necessário: o SQL Server não deixa um mesmo lote criar uma coluna e já referenciá-la
num `update`. O DbUp divide o script em lotes a cada `go`.

> **Faça backup antes.** O script remove e recria todas as PKs, uniques e FKs das tabelas de
> reversas. Rode primeiro em homologação.

```sql
-- 0) remover FKs e depois PKs/uniques das tabelas de reversas
--    (as constraints antigas foram criadas sem nome, então o nome muda de banco para banco)
declare @tables table (name sysname);
insert into @tables values
	('AfterSaleSummarizedReverses'), ('AfterSaleReverses'), ('AfterSaleReversesCustomers'),
	('AfterSaleReversesDestinationsData'), ('AfterSaleReversesTrackings'), ('AfterSaleReversesTrackingHistories'),
	('AfterSaleReversesDestinations'), ('AfterSaleReversesEcommerceOrders'), ('AfterSaleReversesEcommerceOrdersPaymentsMethods'),
	('AfterSaleReversesProducts'), ('AfterSaleReversesProductsImages'), ('AfterSaleReversesStatusHistories'),
	('AfterSaleHistoriesReverses'), ('AfterSaleCourierAttributes'), ('AfterSaleCourierAttributesFields');

declare @sql nvarchar(max) = N'';

select @sql += N'alter table [general].' + quotename(object_name(fk.parent_object_id)) + N' drop constraint ' + quotename(fk.name) + N';' + char(10)
from sys.foreign_keys fk
where schema_name(fk.schema_id) = 'general'
	and object_name(fk.parent_object_id) in (select name from @tables);

exec sp_executesql @sql;

set @sql = N'';

select @sql += N'alter table [general].' + quotename(object_name(kc.parent_object_id)) + N' drop constraint ' + quotename(kc.name) + N';' + char(10)
from sys.key_constraints kc
where kc.type in ('PK', 'UQ')
	and schema_name(kc.schema_id) = 'general'
	and object_name(kc.parent_object_id) in (select name from @tables);

exec sp_executesql @sql;
go

-- 1) renomear cnpj_emp -> doc_company na tabela de reversas resumidas
exec sp_rename 'untreated.AfterSaleSummarizedReverses.cnpj_emp', 'doc_company', 'COLUMN';
exec sp_rename 'aftersale.AfterSaleSummarizedReverses.cnpj_emp', 'doc_company', 'COLUMN';
go

-- 2) nova coluna nas demais tabelas (untreated e general)
alter table [untreated].[AfterSaleReverses] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReverses] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesCustomers] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesCustomers] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesDestinationsData] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesDestinationsData] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesTrackings] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesTrackings] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesTrackingHistories] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesTrackingHistories] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesDestinations] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesDestinations] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesEcommerceOrders] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesEcommerceOrders] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesEcommerceOrdersPaymentsMethods] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesProducts] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesProducts] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesProductsImages] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesProductsImages] add doc_company nvarchar(14);
alter table [untreated].[AfterSaleReversesStatusHistories] add doc_company nvarchar(14);
alter table [aftersale].[AfterSaleReversesStatusHistories] add doc_company nvarchar(14);
alter table [untreated].AfterSaleHistoriesReverses add doc_company nvarchar(14);
alter table [aftersale].AfterSaleHistoriesReverses add doc_company nvarchar(14);
alter table [untreated].AfterSaleCourierAttributes add doc_company nvarchar(14);
alter table [aftersale].AfterSaleCourierAttributes add doc_company nvarchar(14);
alter table [untreated].AfterSaleCourierAttributesFields add doc_company nvarchar(14);
alter table [aftersale].AfterSaleCourierAttributesFields add doc_company nvarchar(14);
go

-- 3) backfill: reversas e courier attributes a partir das resumidas; filhas a partir das reversas
--    (até aqui o id era único na tabela resumida, então o join por id é seguro)
update r set r.doc_company = s.doc_company
from [aftersale].[AfterSaleReverses] r
join [aftersale].AfterSaleSummarizedReverses s on s.id = r.id
where r.doc_company is null;

update c set c.doc_company = s.doc_company
from [aftersale].AfterSaleCourierAttributes c
join [aftersale].AfterSaleSummarizedReverses s on s.id = c.reverse_id
where c.doc_company is null;

update f set f.doc_company = c.doc_company
from [aftersale].AfterSaleCourierAttributesFields f
join [aftersale].AfterSaleCourierAttributes c on c.reverse_id = f.reverse_id
where f.doc_company is null;

update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesCustomers] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesDestinationsData] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesTrackings] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesTrackingHistories] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesDestinations] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesEcommerceOrders] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesProducts] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesProductsImages] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].[AfterSaleReversesStatusHistories] x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
update x set x.doc_company = r.doc_company from [aftersale].AfterSaleHistoriesReverses x join [aftersale].[AfterSaleReverses] r on r.id = x.reverse_id where x.doc_company is null;
go

-- 4) registros que ficaram sem empresa não têm como entrar na nova PK: conferir e remover
--    (filhas primeiro, por causa das FKs que serão recriadas)
delete from [aftersale].[AfterSaleReversesProductsImages] where doc_company is null;
delete from [aftersale].[AfterSaleReversesProducts] where doc_company is null;
delete from [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] where doc_company is null;
delete from [aftersale].[AfterSaleReversesEcommerceOrders] where doc_company is null;
delete from [aftersale].[AfterSaleReversesTrackingHistories] where doc_company is null;
delete from [aftersale].[AfterSaleReversesTrackings] where doc_company is null;
delete from [aftersale].[AfterSaleReversesDestinations] where doc_company is null;
delete from [aftersale].[AfterSaleReversesDestinationsData] where doc_company is null;
delete from [aftersale].[AfterSaleReversesCustomers] where doc_company is null;
delete from [aftersale].[AfterSaleReversesStatusHistories] where doc_company is null;
delete from [aftersale].AfterSaleHistoriesReverses where doc_company is null;
delete from [aftersale].[AfterSaleReverses] where doc_company is null;
delete from [aftersale].AfterSaleCourierAttributesFields where doc_company is null;
delete from [aftersale].AfterSaleCourierAttributes where doc_company is null;
delete from [aftersale].AfterSaleSummarizedReverses where doc_company is null;
go

-- 5) coluna obrigatória
alter table [aftersale].AfterSaleSummarizedReverses alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReverses] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesCustomers] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesDestinationsData] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesTrackings] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesTrackingHistories] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesDestinations] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesEcommerceOrders] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesProducts] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesProductsImages] alter column doc_company nvarchar(14) not null;
alter table [aftersale].[AfterSaleReversesStatusHistories] alter column doc_company nvarchar(14) not null;
alter table [aftersale].AfterSaleHistoriesReverses alter column doc_company nvarchar(14) not null;
alter table [aftersale].AfterSaleCourierAttributes alter column doc_company nvarchar(14) not null;
alter table [aftersale].AfterSaleCourierAttributesFields alter column doc_company nvarchar(14) not null;
alter table [aftersale].AfterSaleCourierAttributesFields alter column id nvarchar(50) not null;
alter table [aftersale].AfterSaleCourierAttributesFields alter column reverse_id bigint not null;
go

-- 6) novas PKs e uniques
alter table [aftersale].AfterSaleSummarizedReverses add constraint [PK_AfterSaleSummarizedReverses] primary key (doc_company, id);
alter table [aftersale].[AfterSaleReverses] add constraint [PK_AfterSaleReverses] primary key (doc_company, id);

alter table [aftersale].[AfterSaleReversesCustomers] add constraint [PK_AfterSaleReversesCustomers] primary key (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesCustomers] add constraint [UQ_AfterSaleReversesCustomers] unique (doc_company, id);
alter table [aftersale].[AfterSaleReversesDestinationsData] add constraint [PK_AfterSaleReversesDestinationsData] primary key (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesDestinationsData] add constraint [UQ_AfterSaleReversesDestinationsData] unique (doc_company, id);
alter table [aftersale].[AfterSaleReversesTrackings] add constraint [PK_AfterSaleReversesTrackings] primary key (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesTrackings] add constraint [UQ_AfterSaleReversesTrackings] unique (doc_company, id);
alter table [aftersale].[AfterSaleReversesDestinations] add constraint [PK_AfterSaleReversesDestinations] primary key (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesDestinations] add constraint [UQ_AfterSaleReversesDestinations] unique (doc_company, id);
alter table [aftersale].[AfterSaleReversesEcommerceOrders] add constraint [PK_AfterSaleReversesEcommerceOrders] primary key (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesEcommerceOrders] add constraint [UQ_AfterSaleReversesEcommerceOrders] unique (doc_company, id);

alter table [aftersale].[AfterSaleReversesTrackingHistories] add constraint [PK_AfterSaleReversesTrackingHistories] primary key (doc_company, id, reverse_id, tracking_id);
alter table [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] add constraint [PK_AfterSaleReversesEcommerceOrdersPaymentsMethods] primary key (doc_company, reverse_id, ecommerce_order_payment_id, ecommerce_order_id);
alter table [aftersale].[AfterSaleReversesProducts] add constraint [PK_AfterSaleReversesProducts] primary key (doc_company, id, reverse_id);
alter table [aftersale].[AfterSaleReversesProductsImages] add constraint [PK_AfterSaleReversesProductsImages] primary key (doc_company, id, reverse_id, reverse_product_id);
alter table [aftersale].[AfterSaleReversesStatusHistories] add constraint [PK_AfterSaleReversesStatusHistories] primary key (doc_company, id, reverse_id);
alter table [aftersale].AfterSaleHistoriesReverses add constraint [PK_AfterSaleHistoriesReverses] primary key (doc_company, id);
alter table [aftersale].AfterSaleCourierAttributes add constraint [PK_AfterSaleCourierAttributes] primary key (doc_company, reverse_id);
alter table [aftersale].AfterSaleCourierAttributesFields add constraint [PK_AfterSaleCourierAttributesFields] primary key (doc_company, id, reverse_id);
go

-- 7) novas FKs (mesmo bloco do passo 7.1)
alter table [aftersale].AfterSaleCourierAttributesFields add constraint [FK_AfterSaleCourierAttributesFields] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleCourierAttributes] (doc_company, reverse_id);
alter table [aftersale].[AfterSaleReversesCustomers] add constraint [FK_AfterSaleReversesCustomers] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesDestinationsData] add constraint [FK_AfterSaleReversesDestinationsData] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesTrackings] add constraint [FK_AfterSaleReversesTrackings] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesTrackingHistories] add constraint [FK_AfterSaleReversesTrackingHistories] foreign key (doc_company, tracking_id) references [aftersale].[AfterSaleReversesTrackings] (doc_company, id);
alter table [aftersale].[AfterSaleReversesDestinations] add constraint [FK_AfterSaleReversesDestinations] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesEcommerceOrders] add constraint [FK_AfterSaleReversesEcommerceOrders] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesEcommerceOrdersPaymentsMethods] add constraint [FK_AfterSaleReversesEcommerceOrdersPaymentsMethods] foreign key (doc_company, ecommerce_order_id) references [aftersale].[AfterSaleReversesEcommerceOrders] (doc_company, id);
alter table [aftersale].[AfterSaleReversesProducts] add constraint [FK_AfterSaleReversesProducts] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
alter table [aftersale].[AfterSaleReversesProductsImages] add constraint [FK_AfterSaleReversesProductsImages] foreign key (doc_company, reverse_product_id, reverse_id) references [aftersale].[AfterSaleReversesProducts] (doc_company, id, reverse_id);
alter table [aftersale].[AfterSaleReversesStatusHistories] add constraint [FK_AfterSaleReversesStatusHistories] foreign key (doc_company, reverse_id) references [aftersale].[AfterSaleReverses] (doc_company, id);
go

-- 8) a procedure de merge é gerada a partir da PK e da lista de colunas; recriar todas
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleSummarizedReverses', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReverses', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesCustomers', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesDestinationsData', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesTrackings', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesTrackingHistories', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesDestinations', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesEcommerceOrders', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesEcommerceOrdersPaymentsMethods', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesProducts', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesProductsImages', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesStatusHistories', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleHistoriesReverses', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleCourierAttributes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleCourierAttributesFields', @execute = 1;
```

> O `sp_Create_Merge_Procedure` gera a procedure com `create or alter proc`
> (`Script003_Create_Merge_Procedure.sql:179`), então reexecutá-lo basta para atualizar a
> procedure existente. Isso também responde à ressalva deixada nos guias de `GetStatus` e
> `GetTransportations`: lá também não é preciso `drop procedure`.
>
> Sem o passo 8, o merge continuaria casando os registros só pelas colunas antigas e copiando a
> lista antiga de colunas, então `doc_company` ficaria nulo na trusted, o que viola a PK.
>
> O passo 4 apaga o que não pôde ser associado a uma empresa. Se preferir não perder nada,
> rode até o passo 3, confira os registros com `doc_company is null` e preencha-os à mão antes
> de seguir.

---

## Checklist

- [ ] 1. Entidades: `doc_company` nas 13 entidades da tabela 1.2 e rename em `SearchReverses.Reverse`
- [ ] 2. `IAfterSaleCommandHandler`: `doc_company` nas assinaturas (linhas 8, 9 e 11)
- [ ] 3. `AfterSaleCommandHandler`: 5 queries (3.1 a 3.5)
- [ ] 4. `IAfterSaleReversesRepository`: linhas 12, 15, 24 e 25
- [ ] 5. `AfterSaleReversesRepository`: 5 alterações (5.1 a 5.5)
- [ ] 6. Service: `GetCourierAttributes`, `GetReverse` e `GetReverses` substituídos, nessa ordem
- [ ] 7. `Script004`: coluna, PKs, uniques e FKs (7.1 a 7.12)
- [ ] 8. Novo script de migração + recriação das procedures de merge
- [ ] Compilar a solução
- [ ] Rodar `AfterSaleReverses` → `AfterSaleReverse` e conferir `doc_company` preenchido em `[aftersale].[AfterSaleReverses]` e nas filhas, para as duas empresas

---

## Fora do escopo, mas visto no caminho

- **`AfterSaleErrors` continua com `cnpj_emp`.** Para padronizar de vez, renomeie a coluna nas
  duas tabelas (`Script001`, linhas 17 e 29) e a propriedade/parâmetros de `Entities/Errors/Error.cs`
  (linhas 16, 24, 30, 39 e 45). Atenção também à PK dessa tabela: é só `execution`, então dois
  erros na mesma execução colidem no merge.
- **`GetCourierAttributes` não grava nada.** Não existe `BulkInsertIntoAfterSaleCourierAttributes`
  no repositório (o insert está comentado no service e a assinatura está comentada na interface,
  linha 26). Para o job funcionar, é preciso criar esse método, gravando `AfterSaleCourierAttributes`
  e `AfterSaleCourierAttributesFields` no mesmo molde de `BulkInsertIntoAfterSaleReverses`.
- **Rota de courier attributes:** confirme `v3/api/reverses/{id}/get-courier-attributes` na
  documentação da API Send4 antes de publicar.
- **`Thread.Sleep` em método `async`** (`GetReverses`): bloqueia a thread do pool durante a espera
  de 1 minuto. Trocar por `await Task.Delay(...)` é o equivalente não bloqueante.
- **`GetReverse.Reverse.total_amount`** (linha 141 da entidade) usa `ConvertToInt32Validation`
  num campo `decimal`, então perde os centavos. Deveria ser `ConvertToDecimalValidation`.
- **Paginação em lotes de 1000 nos repositórios** (`GetSummarizedReversesByIds`,
  `GetReversesByIds`, `GetCourierAttributesByReverseId`): a condição `indice > 1` só entra no laço a
  partir de 2000 ids. Entre 1000 e 1999 tudo vai numa query só. Funciona no SQL Server, mas a
  intenção parece ser `indice >= 1`.
- **Queries por concatenação de string:** CNPJs e ids vêm do próprio banco/API, então o risco é
  baixo, mas o ideal é migrar para parâmetros (`@doc_company`) quando o `GetRecords` aceitar.
