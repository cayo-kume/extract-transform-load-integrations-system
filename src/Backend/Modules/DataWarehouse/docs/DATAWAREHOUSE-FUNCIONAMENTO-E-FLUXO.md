# Data warehouse do NEWBLOOMERS · Funcionamento e fluxo de execução

Este documento explica, com diagramas, como o módulo `DataWarehouse` funciona hoje: de onde vêm os dados, quem dispara cada carga, em que ordem as procedures rodam e como o resultado é registrado. As decisões de modelagem, o DDL e as justificativas estão no guia em [src/Backend/Modules/DataWarehouse/docs](00-ordem-de-leitura.md); os números de seção citados aqui (por exemplo, "10.1") são os desse guia.

Código de referência:

| Peça | Arquivo |
|---|---|
| Rotas de disparo | [DataWarehouseController.cs](../../../Hosts/WebApi/Controllers/DataWarehouse/DataWarehouseController.cs) |
| Orquestração (ordem dos passos, trava, log, checagens) | [DataWarehouseService.cs](../Application.DataWarehouse/Services/DataWarehouseService.cs) |
| Acesso ao banco (procedures, catálogo, checagens) | [DataWarehouseRepository.cs](../Infrastructure.DataWarehouse/Repositorys/DataWarehouseRepository.cs) |
| Jobs registrados em `auditing` | [EnumJob.cs](../Domain.DataWarehouse/Enums/EnumJob.cs) |
| Objetos do banco (DbUp) | [Scripts/SQLServer](../Infrastructure.DataWarehouse/Scripts/SQLServer) e [migrator.csx](../Infrastructure.DataWarehouse/Scripts/SQLServer/migrator.csx) |

---

## 1. A ideia em uma frase

O DW mora **no mesmo banco** das integrações. As integrações gravam as tabelas confiáveis (`totvs.*`, `linxmicrovix.*`, `movidesk.*`, `aftersale.*`); o DW lê essas tabelas por **procedures T-SQL** chamadas em ordem fixa por um job .NET, e o trabalho pesado (MERGE, snapshot, dimensões, fatos) acontece todo dentro do SQL Server. O código C# só decide **a ordem**, **registra o log** e **impede duas cargas ao mesmo tempo**.

---

## 2. Camadas de dados

Cada seta é uma família de procedures (ou de views). Os dados só andam para a direita.

```mermaid
flowchart LR
    subgraph ORIGEM["Integrações (outros módulos)"]
        API["APIs: Totvs, Linx Microvix,<br/>Linx Commerce, Movidesk, AfterSale"]
        UNT["untreated.*<br/>(carga bruta)"]
        TRUST["totvs.* · linxmicrovix.* · linxcommerce.*<br/>movidesk.* · aftersale.*<br/>(tabelas confiáveis,<br/>integrated_or_updated_at)"]
        API --> UNT -->|"sp_*_Sync (MERGE)"| TRUST
    end

    subgraph DW["Data warehouse (mesmo banco)"]
        LAND["dw_landing.*<br/>estado que o DW viu<br/>(sem dado pessoal)"]
        HIST["dw_history.*<br/>versões SCD2"]
        INTER["dw_intermediate.vw_*<br/>unificação Totvs + Linx,<br/>regras, canal, tipo do pedido"]
        STAR["datawarehouse.*<br/>Dim* e Fact*<br/>(estrela Kimball)"]
        MART["datamart.*<br/>agregados (DailySales)"]
        PII["dw_personal_data.*<br/>dado pessoal em claro<br/>(acesso restrito dw_pii)"]
        OUT["dw_outbound.*<br/>views por consumidor"]
    end

    TRUST -->|"dw_landing.sp_#lt;Tabela#gt;_Load"| LAND
    LAND -->|"dw_history.sp_#lt;Tabela#gt;_Snapshot"| HIST
    TRUST -->|"dw_personal_data.sp_*_Load"| PII
    LAND --> INTER
    HIST --> INTER
    INTER -->|"datawarehouse.sp_Dim*/Fact*_Load"| STAR
    PII -->|"doc_hash, doc_masked (com pepper)"| STAR
    STAR -->|"datamart.sp_*_Refresh"| MART
    INTER --> OUT
    STAR --> OUT
    PII -.->|"só OtoCustomers e<br/>LogisticsShipments"| OUT

    MART --> BI["Ferramentas de BI"]
    OUT --> SAIDA["Módulos de saída: Oto, Pagarme,<br/>Dootax, Mobsim, logística"]

    subgraph TRANS["Transversais"]
        CTRL["dw_control.*<br/>Watermark, LandingSource,<br/>CheckResult"]
        REF["dw_reference.*<br/>feriados, ChannelRules,<br/>BrandSeries, ValueCodes..."]
        AUD["auditing.*<br/>Logs e Messages"]
    end
```

| Schema | Papel | Se for perdido |
|---|---|---|
| `dw_landing` | Espelho incremental das tabelas confiáveis, com `row_hash`, `loaded_at` e `missing_since` | Recarrega da origem zerando o watermark |
| `dw_history` | Versões com `valid_from`, `valid_to`, `is_current` | **Não recupera**: a origem só guarda o estado atual. Depende do backup |
| `dw_intermediate` | Views com as regras de negócio e o formato unificado dos ERPs | Recria pelo script |
| `datawarehouse` | Dimensões e fatos | Recarrega de landing e history |
| `datamart` | Agregados para BI | Recalcula |
| `dw_personal_data` | Nome, documento, endereço, e-mail, telefone: só a versão atual | Recria da origem |
| `dw_outbound` | Views que os módulos de saída varrem por `updated_at` | Recria pelo script |
| `dw_control` | Watermarks, catálogo de origens e resultado das checagens | Zerar watermark = refazer pela janela inicial |

---

## 3. Componentes de código

```mermaid
flowchart TB
    CALLER["Disparo manual (Swagger / HTTP)<br/>ou o agendador, quando existir"]

    subgraph HOST["Host WebApi (ASP.NET)"]
        CTRL["DataWarehouseController<br/>POST DataWarehouseJobs/DataWarehouse/*"]
        DI["ServicesExtensions<br/>AddScopedDataWarehouseServices"]
    end

    subgraph APP["Application.DataWarehouse"]
        SVC["DataWarehouseService<br/>ordem dos passos · RunLock · checagens"]
    end

    subgraph SHARED["Shared"]
        LOG["ILoggerService<br/>(auditing.Logs / Messages)"]
        SREPO["ISharedRepository<br/>(conexão, GetRecords, ExecuteCommand)"]
    end

    subgraph INFRA["Infrastructure.DataWarehouse"]
        REPO["DataWarehouseRepository<br/>ExecuteProcedure · CountRows ·<br/>GetLandingSources · InsertCheckResult"]
    end

    CONF["IConfiguration<br/>DataWarehouse:Pepper"]
    DB[("Azure SQL Database<br/>procedures e views do DW")]

    CALLER --> CTRL
    DI -.registra.-> SVC
    DI -.registra.-> REPO
    CTRL --> SVC
    SVC --> LOG
    SVC --> CONF
    SVC --> REPO
    REPO --> SREPO
    REPO -->|"Dapper, CommandType.StoredProcedure,<br/>timeout 7200 s, @rows OUTPUT"| DB
    LOG --> DB
```

Pontos importantes do código:

- **O service não tem SQL de negócio.** Cada passo é `Step("[schema].sp_Nome")`, que chama `ExecuteProcedure`, mede o tempo e grava no log `"<procedure>: N linha(s) em X s"`. Toda procedure devolve as linhas afetadas no parâmetro de saída `@rows`.
- **Os nomes das procedures de landing e snapshot vêm do catálogo** `dw_control.LandingSource` (só as linhas com `is_enabled = 1`): `sp_<source_table>_Load`, `sp_<source_table>_DetectMissing`, `sp_<source_table>_Snapshot`. Ligar uma origem nova é inserir no catálogo, sem mexer no C#.
- **Proteção contra injeção:** como os nomes são montados como texto, o repositório só aceita `schema.nome` com letras, números e sublinhado (regex `ObjectName`), tanto para procedure quanto para view.
- **Timeout de 2 horas** por procedure, porque a carga inicial dos fatos passa dos 45 minutos padrão do Shared.
- **Watermark não aparece no C#:** ler e avançar o `dw_control.Watermark` é responsabilidade de cada procedure de carga.

---

## 4. Os cinco jobs

| Rota (`POST DataWarehouseJobs/DataWarehouse/...`) | Método do service | Job (`auditing`) | Quando roda | O que faz |
|---|---|---|---|---|
| `RunHourly` | `RunHourly()` | 9000 `DataWarehouseHourly` | A cada hora | Sequência 10.1 sem os passos diários |
| `RunDaily` | `RunDaily()` | 9001 `DataWarehouseDaily` | Uma vez por dia, no fim do dia | Sequência 10.1 completa: + exclusões de filhas, snapshots diários, `DimDate` e, no dia 1, o estoque do mês |
| `RunShortCycle` | `RunShortCycle()` | 9002 `DataWarehouseShortCycle` | A cada 3 minutos | Só landing de pedidos, notas e clientes + dado pessoal, para a logística (10.3) |
| `RunMonthlyStock?positionDate=yyyy-MM-dd` | `RunMonthlyStock(date)` | 9003 `DataWarehouseMonthlyStock` | Manual | Refaz a posição de estoque de um fim de mês (sem data: último dia do mês anterior) |
| `RunLinxLegacy` | `RunLinxLegacy()` | 9004 `DataWarehouseLinxLegacy` | Uma vez (fase 3), reexecutável | Carga única do legado Linx Microvix (10.2) |

> Hoje **não há agendamento recorrente** do DW no Hangfire: as rotas são o disparo manual e o ponto que o agendador vai chamar. Quando ele for codificado, precisa de `DisableConcurrentExecution` (ver seção 9). O `[Authorize]` do controller está comentado e, em `RELEASE`, as rotas ficam fora do Swagger.

---

## 5. Fluxo de uma requisição

```mermaid
sequenceDiagram
    autonumber
    actor C as Chamador
    participant CT as DataWarehouseController
    participant S as DataWarehouseService
    participant L as ILoggerService
    participant R as DataWarehouseRepository
    participant DB as SQL Server

    C->>CT: POST RunHourly
    CT->>S: RunHourly()
    S->>S: RunLock.WaitAsync(0)
    alt já existe carga rodando no processo
        S-->>CT: null
        CT-->>C: 409 Conflict
    else trava obtida
        S->>L: Clear().AddLog(job 9000)
        S->>R: GetLandingSources()
        R->>DB: select ... from dw_control.LandingSource where is_enabled = 1
        DB-->>R: origens
        loop cada passo da sequência
            S->>R: ExecuteProcedure("[schema].sp_...", params)
            R->>DB: EXEC procedure (@rows OUTPUT)
            Note over DB: a procedure lê e avança<br/>o próprio watermark
            DB-->>R: @rows
            R-->>S: linhas
            S->>L: AddMessage("procedure: N linha(s) em X s")
        end
        loop cada view de checagem
            S->>R: CountRows(view)
            R->>DB: select count(*) from view
            S->>R: InsertCheckResult(nome, erros)
            R->>DB: insert dw_control.CheckResult
        end
        S->>L: SetLogEndDate() + CommitAllChanges()
        S->>S: RunLock.Release() (no finally)
        alt sucesso e nenhuma checagem bloqueante falhou
            S-->>CT: true
            CT-->>C: 200 OK
        else exceção em um passo ou checagem bloqueante
            S-->>CT: false
            CT-->>C: 500 (detalhe no log e em CheckResult)
        end
    end
```

Uma exceção em qualquer passo **interrompe a carga** ali mesmo: os passos seguintes não rodam, a mensagem e o stack trace vão para o log e a resposta é 500. Como cada procedure só avança o watermark depois do próprio sucesso, a próxima execução retoma de onde parou.

---

## 6. Sequência horária e diária (10.1)

`RunHourly` e `RunDaily` usam o mesmo método `Run(job, daily)`. Os losangos marcam o que só entra no ciclo diário.

```mermaid
flowchart TD
    START(["Run(job, daily)"]) --> LOCK{"RunLock livre?"}
    LOCK -- não --> R409(["null → 409"])
    LOCK -- sim --> CAT["Lê dw_control.LandingSource<br/>(is_enabled = 1)"]

    CAT --> P2["<b>Passo 2 · Landing</b><br/>para cada origem:<br/>dw_landing.sp_#lt;Tabela#gt;_Load"]

    P2 --> D3{"daily?"}
    D3 -- sim --> P3["<b>Passo 3 · Exclusões</b><br/>origens com detect_missing:<br/>dw_landing.sp_#lt;Tabela#gt;_DetectMissing"]
    D3 -- não --> P4
    P3 --> P4["<b>Passo 4 · Snapshots SCD2</b><br/>history_frequency = hourly<br/>+ daily (só no diário)<br/>dw_history.sp_#lt;Tabela#gt;_Snapshot"]

    P4 --> P5["<b>Passo 5 · Dado pessoal</b><br/>sp_Customer_Load<br/>sp_OrderShippingAddress_Load<br/>sp_ServiceCustomer_Load"]

    P5 --> D6{"daily?"}
    D6 -- sim --> DATE["sp_DimDate_Load"]
    D6 -- não --> DIMS
    DATE --> DIMS["<b>Passo 6a · Dimensões</b><br/>Store, Product, Customer*, Seller,<br/>Supplier, PaymentMethod, Operation,<br/>Channel, Brand, SalesRepresentative,<br/>Region, Promotion"]

    DIMS --> FACTS["<b>Passo 6b · Fatos</b><br/>SalesItem, OrderItem, InvoicePayment,<br/>Ticket*, Refund*, PurchaseOrderItem,<br/>AccountsPayable, Commission*,<br/>StoreReception, ApiRequest"]

    FACTS --> D1{"daily e hoje é dia 1<br/>(horário de Brasília)?"}
    D1 -- sim --> STOCK["sp_FactMonthlyStock_Load<br/>(mês que fechou)"]
    D1 -- não --> MART
    STOCK --> MART["<b>Passo 6c · Agregado</b><br/>datamart.sp_DailySales_Refresh"]

    MART --> P7["<b>Passo 7 · Checagens</b><br/>RunChecks()"]
    P7 --> BLK{"alguma checagem<br/>bloqueante com erro?"}
    BLK -- não --> OK(["true → 200"])
    BLK -- sim --> FAIL(["false → 500"])

    P2 -. exceção em qualquer passo .-> ERR(["false → 500<br/>carga interrompida"])
```

`*` = **passo com pepper** (`PepperStep`): `sp_DimCustomer_Load`, `sp_FactTicket_Load`, `sp_FactRefund_Load` e `sp_FactCommission_Load` calculam o `doc_hash` (SHA-256 do documento com um segredo) e recebem `@pepper` de `DataWarehouse:Pepper`. Se a configuração estiver vazia, **essas procedures são puladas** com um aviso no log e o resto da carga segue; a tabela fica como estava.

A ordem importa porque cada passo lê o que o anterior gravou: as dimensões precisam do dado pessoal (hash do cliente), os fatos precisam das dimensões (chaves substitutas) e o agregado precisa dos fatos.

---

## 7. Dentro de uma origem: landing e snapshot

As procedures de landing e de snapshot não são escritas à mão: são **geradas a partir do catálogo** na migração, por `dw_landing.sp_Create_Landing` (Script010) e `dw_history.sp_Create_History` (Script020).

### 7.1 Na migração: o catálogo gera os objetos

```mermaid
flowchart LR
    CAT[("dw_control.LandingSource<br/>source_schema, source_table,<br/>included/excluded_columns,<br/>source_updated_column,<br/>detect_missing, history_frequency,<br/>is_enabled")]
    SRC[("Tabela confiável<br/>ex.: totvs.TotvsInvoices")]

    GENL["dw_landing.sp_Create_Landing<br/>(Script010)"]
    GENH["dw_history.sp_Create_History<br/>(Script020)"]

    CAT --> GENL
    SRC -->|"lê colunas, tipos e PK<br/>em sys.columns"| GENL
    GENL --> T1["Tabela dw_landing.#lt;Tabela#gt;<br/>+ source_updated_at, row_hash,<br/>loaded_at, missing_since"]
    GENL --> P1["sp_#lt;Tabela#gt;_Load"]
    GENL -->|"se detect_missing = 1"| P2["sp_#lt;Tabela#gt;_DetectMissing"]

    CAT --> GENH
    T1 --> GENH
    GENH -->|"se history_frequency<br/>não é nulo"| T2["Tabela dw_history.#lt;Tabela#gt;<br/>+ valid_from, valid_to, is_current"]
    GENH --> P3["sp_#lt;Tabela#gt;_Snapshot"]
```

As colunas de dado pessoal ficam fora da landing pelo catálogo (`excluded_columns`), e por isso também ficam fora do histórico. Elas vão direto da origem para `dw_personal_data`.

### 7.2 Na execução: o que cada procedure gerada faz

```mermaid
flowchart TD
    subgraph LOAD["dw_landing.sp_#lt;Tabela#gt;_Load (passo 2)"]
        W1["Lê Watermark<br/>(source = schema, entity = tabela)"] --> SEL["Seleciona na origem<br/>integrated_or_updated_at #gt; watermark − 3 dias"]
        SEL --> HASH["Calcula row_hash (SHA-256)<br/>das colunas de negócio"]
        HASH --> MERGE{"MERGE pela PK"}
        MERGE -- "chave nova" --> INS["INSERT, loaded_at = agora"]
        MERGE -- "hash mudou" --> UPD["UPDATE, loaded_at = agora"]
        MERGE -- "hash igual" --> NOP["nada (releitura idempotente)"]
        INS --> WSET["Avança o watermark"]
        UPD --> WSET
        NOP --> WSET
    end

    subgraph SNAP["dw_history.sp_#lt;Tabela#gt;_Snapshot (passo 4)"]
        W2["Lê o próprio watermark<br/>(source = dw_history)"] --> NEW["Linhas da landing com<br/>loaded_at #gt; watermark"]
        NEW --> CMP{"row_hash ≠ versão vigente?"}
        CMP -- sim --> CLOSE["Fecha a versão vigente<br/>valid_to = source_updated_at,<br/>is_current = 0"]
        CLOSE --> OPEN["Abre versão nova<br/>valid_from = source_updated_at,<br/>is_current = 1"]
        CMP -- não --> SKIP["mantém"]
        OPEN --> W3["Avança o watermark"]
        SKIP --> W3
    end

    subgraph DEL["dw_landing.sp_#lt;Tabela#gt;_DetectMissing (passo 3, diário)"]
        AJ["Anti-join: chaves da landing<br/>que não existem mais na origem"] --> MS["missing_since = agora"]
    end

    LOAD --> SNAP
```

- A **sobreposição de 3 dias** cobre linhas que ficaram visíveis tarde (commit atrasado na origem); o MERGE por chave garante que reler não duplica.
- O hash **ignora colunas de controle** (`integrated_or_updated_at`, `maxChangeFilterDate`, `lastChangeDate`, `json_hash`...): só mudança de negócio gera versão nova.
- O snapshot trabalha **só sobre o que a carga acabou de gravar** (`loaded_at`), por isso cabe a cada hora sem crescer com o tamanho do histórico.
- A **data da versão vem da origem** (`source_updated_at`), não da hora em que o snapshot rodou.
- Notas e pedidos têm snapshot `hourly`; cadastros, saldos, custos e preços, `daily`; lojas e produtos do Linx, `once` (só na carga do legado).

---

## 8. Ciclos especiais

### 8.1 Ciclo curto da logística (10.3) — a cada 3 minutos

A logística precisa do pedido e do endereço logo depois que os jobs da Totvs os trazem. O ciclo curto roda só dois passos da sequência, para sete origens fixas (`ShortCycleTables` no service).

```mermaid
flowchart LR
    START(["RunShortCycle"]) --> LAND["Passo 2 só para:<br/>TotvsSaleOrders, TotvsSaleOrdersProducts,<br/>TotvsInvoices, TotvsInvoicesItems,<br/>TotvsInvoicesSalesOrders,<br/>TotvsPeople, TotvsLegalEntities"]
    LAND --> PII["Passo 5:<br/>sp_Customer_Load<br/>sp_OrderShippingAddress_Load"]
    PII --> FIM(["true → 200<br/>(sem checagens)"])

    LAND -.-> VOP["dw_intermediate.vw_OrderParts<br/>(view sobre a landing +<br/>dw_reference.ChannelRules)"]
    PII -.-> LS
    VOP -.-> LS["dw_outbound.LogisticsShipments"]
    LS -.-> LOGI["Jadlog, TotalExpress,<br/>Stone, FlashCourier"]
```

As views da logística não dependem de dimensões nem fatos, então ficam frescas sem o passo 6. Quando o ciclo horário roda depois, o passo 2 encontra a landing já atualizada e carrega só o que restou; o `loaded_at` garante que tudo que o ciclo curto gravou entra no snapshot seguinte.

### 8.2 Carga única do legado Linx (10.2)

```mermaid
flowchart TD
    START(["RunLinxLegacy"]) --> CAT["LandingSource where<br/>source_schema = 'linxmicrovix'<br/>(inclusive desligadas)"]
    CAT --> L["Passo 2: sp_Linx*_Load<br/>(histórico inteiro, a origem não recebe mais linhas)"]
    L --> S["Passo 4: snapshot 'once'<br/>LinxLojas, LinxProdutos"]
    S --> P["Passo 5: dw_personal_data.sp_LinxCustomer_Load"]
    P --> D["Passo 6: dimensões Store, Product, Customer*,<br/>Seller, Operation, Brand<br/>(refazem Totvs + Linx juntos,<br/>de/para em vw_ProductMatch)"]
    D --> F["dw_intermediate.sp_LinxLegacy_Load<br/>linhas Linx em FactSalesItem e FactOrderItem"]
    F --> M["datamart.sp_DailySales_Refresh"]
    M --> C["Passo 7: checagens<br/>(vw_Check_ErpOverlap bloqueia se faltar<br/>período em dw_reference.StoreErpMap)"]
```

### 8.3 Posição de estoque de um mês (8.5)

```mermaid
flowchart LR
    A(["RunMonthlyStock(positionDate?)"]) --> B["datawarehouse.sp_FactMonthlyStock_Load<br/>@position_date<br/>(nulo = último dia do mês anterior)"]
    B --> C["usa dw_intermediate.fn_StockPosition:<br/>saldo, loja, produto e custo da época,<br/>lidos de dw_history"]
    C --> D["Passo 7: checagens"]
```

O ciclo diário do dia 1 já faz isso automaticamente para o mês que fechou; a rota serve para refazer um mês.

---

## 9. Concorrência

Os ciclos horário, diário e curto escrevem nas mesmas landings, então só uma carga roda por vez.

```mermaid
stateDiagram-v2
    [*] --> Livre
    Livre --> Rodando: WaitAsync(0) obtém a trava
    Rodando --> Rodando: outra chamada chega → recebe null (409), sem esperar
    Rodando --> GravandoLog: fim, erro ou checagem bloqueante
    GravandoLog --> Livre: Release() no finally, mesmo se gravar o log falhar
```

- A trava é um `SemaphoreSlim(1, 1)` **estático**, então vale para o processo inteiro, entre todos os cinco jobs.
- `WaitAsync(0)` não enfileira: se o ciclo curto cair durante o horário, ele simplesmente devolve 409 e roda na próxima janela de 3 minutos.
- A trava **não vale entre processos** (duas instâncias do host). Por isso o agendador, quando existir, precisa de `DisableConcurrentExecution` no Hangfire.

---

## 10. Checagens de qualidade (passo 7)

Cada checagem é uma view que devolve as linhas com erro. O service conta as linhas, grava em `dw_control.CheckResult` (que não é apagada pela limpeza de 15 dias do `auditing`) e decide o resultado.

```mermaid
flowchart TD
    START(["RunChecks()"]) --> LOOP["para cada view da lista"]
    LOOP --> COUNT["select count(*) from view"]
    COUNT --> SAVE["insert dw_control.CheckResult<br/>(check_name, executed_at, error_rows,<br/>passed = error_rows = 0)"]
    SAVE --> ERR{"error_rows #gt; 0?"}
    ERR -- não --> NEXT
    ERR -- sim --> MSG["AddMessage no log"]
    MSG --> BL{"bloqueante?"}
    BL -- sim --> MARK["blocked = true"]
    BL -- não --> NEXT
    MARK --> NEXT{"mais views?"}
    NEXT -- sim --> LOOP
    NEXT -- não --> RET(["devolve blocked"])
```

| View | Bloqueia | O que pega |
|---|---|---|
| `datawarehouse.vw_Check_FactSalesItemGrain` | Sim | Grão duplicado em `FactSalesItem` |
| `datawarehouse.vw_Check_ErpOverlap` | Sim | Mesma loja (CNPJ) e dia vindo da Totvs e do Linx |
| `datawarehouse.vw_Check_ProductMatch` | Não | Produto Linx sem de/para |
| `datawarehouse.vw_Check_UnknownMembers` | Não | Fatos apontando para membro desconhecido |
| `datawarehouse.vw_Check_ItemProducts` | Não | Soma dos produtos de um item ≠ valor do item |
| `datawarehouse.vw_Check_OrderTypeUnknown` | Não | Pedido sem tipo reconhecido |
| `datawarehouse.vw_Check_MissingChildren` | Não | Filhas apagadas na origem (`missing_since`) |
| `datamart.vw_Check_InvoicedReconciliation` | Não | Total mensal por loja diferente do fechamento de referência |
| `datawarehouse.vw_Check_StockNegativeOrNoCost` | Não | Posição de estoque negativa ou sem custo |

Uma checagem bloqueante **não desfaz** a carga (os dados já foram gravados): ela só faz o job terminar como falha (500), para chamar atenção. O ciclo curto não roda checagens.

---

## 11. Quem consome o DW

```mermaid
flowchart LR
    subgraph DW
        INTER["dw_intermediate.vw_OrderParts<br/>vw_SalesItem, vw_OrderItem..."]
        STAR["datawarehouse.Dim* / Fact*"]
        MART["datamart.DailySales"]
        PII["dw_personal_data"]
        OUT["dw_outbound<br/>OtoOrders, OtoCustomers, WmsOrders,<br/>WmsReceipts, LogisticsShipments,<br/>PagarmePayments, MarketplaceProducts"]
        WM[("dw_control.Watermark<br/>source = módulo, entity = view")]
    end

    INTER --> OUT
    STAR --> OUT
    PII -.-> OUT

    OUT -->|"updated_at #gt; watermark − 3 dias"| MOD["Módulo de saída<br/>(Oto, Pagarme, Dootax,<br/>Mobsim, logística)"]
    MOD <-->|"sp_Watermark_Get / Set"| WM
    MOD -->|"próprios services,<br/>log em *Requests"| SAAS["SaaS contratados"]
    MART --> BI["BI"]
```

O DW **não envia nada** para fora: cada módulo de saída varre a sua view em `dw_outbound` pelo `updated_at`, com o próprio watermark, e continua enviando pelos próprios services. A regra de canal, tipo e número do pedido fica num lugar só (`vw_OrderParts`), e cada view monta o formato que o SaaS espera.

---

## 12. Criação dos objetos (migração)

Os objetos do DW são criados pelo DbUp, via `dotnet script migrator.csx "<connection string>"`, que executa os scripts em ordem numérica, uma vez cada. O migrator do DW roda **por último**, depois dos migrators dos módulos de origem, porque as views referenciam tabelas de origem e o SQL Server as valida na criação.

```mermaid
flowchart LR
    M["Migrators dos módulos<br/>(Shared, Totvs, Linx,<br/>Movidesk, AfterSale)"] --> DW["migrator.csx do DW"]
    DW --> S0["001–009<br/>schemas, Watermark,<br/>CheckResult, LandingSource,<br/>dw_reference, jobs"]
    S0 --> S1["010–019<br/>gerador e catálogo<br/>da landing"]
    S1 --> S2["020–029<br/>gerador de histórico,<br/>views de dw_intermediate"]
    S2 --> S3["030–039<br/>dado pessoal<br/>e dimensões"]
    S3 --> S4["040–049<br/>fatos"]
    S4 --> S5["050–059<br/>datamart e<br/>dw_outbound"]
    S5 --> S6["060–069<br/>checagens"]
    S6 --> S7["070+<br/>seeds e jobs<br/>das fases 2 e 3"]
```

Detalhes por script em [06-procedures-e-projeto.md](06-procedures-e-projeto.md), seção 9.

---

## 13. Resumo para operar

1. **Primeira vez:** rodar os migrators dos módulos, depois o do DW; cadastrar `DataWarehouse:Pepper` na configuração; ligar no catálogo as origens que já têm carga (`TotvsBalances` nasce desligada).
2. **Rotina:** `RunShortCycle` a cada 3 min, `RunHourly` a cada hora, `RunDaily` no fim do dia. Uma chamada que encontra outra rodando recebe 409 e não faz nada.
3. **Diagnóstico:** cada execução fica em `auditing.Logs` / `auditing.Messages` com o tempo e as linhas de cada procedure; o histórico das checagens fica em `dw_control.CheckResult`.
4. **Refazer:** `RunMonthlyStock?positionDate=...` para um mês de estoque; `RunLinxLegacy` para o legado; para recarregar uma origem do zero, zerar o watermark dela em `dw_control.Watermark` (volta à janela inicial da 5.2, regra 5).
