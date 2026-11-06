# Proposta — datas anuláveis no AfterSale (padrão Movidesk)

Guia para aplicação **manual**. Nada foi alterado no código.

## Situação atual

Todas as datas que vêm da API AfterSale passam por
`ConverterValidatorsExtensions.ConvertToDateTimeValidation`. Quando o valor é nulo, vazio, inválido
ou anterior a 1753, o método devolve `new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar)`.
Assim, um `deleted_at` nulo (registro **não** excluído) é gravado como "excluído em 01/01/1990", e não
dá para distinguir "sem data" de uma data real.

## Como o Movidesk faz

| Camada | Movidesk |
|---|---|
| DTO | data como `string` (igual ao AfterSale) |
| Entidade | propriedade `DateTime?` (ex.: `Ticket.closedIn`, `Person.changedDate`) |
| Conversão | `NullableConverters.ToNullableDateTime(dto.campo)` |
| Coluna | `datetime` sem `not null` |
| Comparação de cache | `NullableConverters.ToNullableDateTime(x.changedDate) <= y.changedDate` |

`NullableConverters.ToNullableDateTime` ([NullableConverters.cs](../../../../Shared/Domain.Shared/Extensions/NullableConverters.cs)) já existe no `Domain.Shared`:

```csharp
public static DateTime? ToNullableDateTime(string? value) =>
    String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToDateTimeValidation(value);
```

- nulo / vazio / só espaços → `null`
- data válida → a data
- texto preenchido mas inválido, ou anterior a 1753 → continua `01/01/1990` (cai no conversor antigo). Isso protege o `SqlBulkCopy`, porque o `datetime` do SQL Server não aceita datas antes de 1753.

**Proposta:** usar exatamente o mesmo padrão no AfterSale. Não é preciso criar conversor novo nem mexer no `Domain.Shared`.

---

## Passo 1 — Entidades: `DateTime` → `DateTime?`

Para cada propriedade da tabela abaixo:

```csharp
// antes
public DateTime deleted_at { get; private set; }
...
deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(dto.deleted_at);

// depois
public DateTime? deleted_at { get; private set; }
...
deleted_at = NullableConverters.ToNullableDateTime(dto.deleted_at);
```

`NullableConverters` fica no mesmo namespace (`Domain.Shared.Extensions`) do `ConverterValidatorsExtensions`, então os `using` não mudam.

Caminhos relativos a `src/Backend/Modules/General/AfterSale/Domain.AfterSale/Entities/Reads/`:

| Entidade | Propriedades |
|---|---|
| `Me/Ecommerce.cs` | `last_order_report_date`, `address_updated_at`, `address_deleted_at` |
| `Me/Reason.cs` | `created_at`, `updated_at`, `deleted_at` |
| `Me/SubReason.cs` | `created_at`, `updated_at`, `deleted_at` |
| `Refunds/GetRefund/Refund.cs` | `created_at`, `updated_at`, `voucher_expiring_date`, `reverse_updated_at`, `reverse_created_at` |
| `Refunds/GetRefund/Product.cs` | `reason_created_at`, `reason_updated_at`, `reason_deleted_at`, `subreason_created_at`, `subreason_updated_at`, `subreason_deleted_at` |
| `Refunds/GetRefund/TotalAmountHistories.cs` | `date` |
| `Refunds/SearchRefunds/Refund.cs` | `created_at`, `updated_at`, `last_status_history_date` |
| `Reverses/GetReverse/Reverse.cs` | `created_at`, `updated_at`, `deleted_at`, `ecommerce_last_order_report_date`, `billing_date` |
| `Reverses/GetReverse/Customer.cs` | `address_updated_at`, `address_deleted_at`, `shipping_address_updated_at`, `shipping_address_deleted_at` |
| `Reverses/GetReverse/DestinationData.cs` | `address_deleted_at`, `address_updated_at`, `updated_at` |
| `Reverses/GetReverse/ReverseDestination.cs` | `address_updated_at`, `address_deleted_at` |
| `Reverses/GetReverse/EcommerceOrder.cs` | `ordered_at`, `invoiced_at`, `delivered_at`, `updated_at`, `deleted_at` |
| `Reverses/GetReverse/Product.cs` | `reason_created_at`, `reason_updated_at`, `reason_deleted_at`, `subreason_created_at`, `subreason_updated_at`, `subreason_deleted_at` |
| `Reverses/GetReverse/StatusHistories.cs` | `date` |
| `Reverses/GetReverse/Tracking.cs` | `updated_at`, `deleted_at`, `status_updated_at`, `delivery_deadline`, `requested_collect_date`, `expire_date`, `collect_date` |
| `Reverses/GetReverse/TrackingHistory.cs` | `status_updated_at`, `created_at`, `updated_at`, `deleted_at` |
| `Reverses/SearchReverses/Reverse.cs` | `created_at`, `updated_at`, `tracking_delivery_deadline` |

### Ficam como estão (`DateTime` com valor)

| Propriedade | Motivo |
|---|---|
| `integrated_or_updated_at` (todas as entidades, inclusive `Errors/Error.cs`) | Data gerada pelo sistema (horário de Brasília), nunca nula. Igual ao Movidesk. |
| `Writes/Requests/Request.request_date` | Data gerada pelo sistema. |
| `Refunds/GetRefund/StatusHistories.date` | Faz parte da chave primária de `aftersale.AfterSaleRefundsStatusHistories` (`[date] datetime not null`, `Script003`). Uma PK não aceita `NULL`, então o fallback `01/01/1990` continua necessário. |

> O `[date]` de `AfterSaleReversesStatusHistories` (`Script004`) não está na PK e aceita nulo, por isso entra na lista acima.

---

## Passo 2 — Comparações de cache nos services

Os quatro filtros `RemoveAll` comparam a data da API com o `updated_at` gravado. Com `y.updated_at` como `DateTime?`, o código continua compilando: `<=` entre `DateTime` e `DateTime?` vira `DateTime?`, e qualquer lado nulo dá `false`, então o registro é **regravado** (comportamento seguro). Para ficar igual ao Movidesk, a troca é só do conversor:

```csharp
// antes
ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.updated_at) <= y.updated_at

// depois
NullableConverters.ToNullableDateTime(x.updated_at) <= y.updated_at
```

| Arquivo (`Application.AfterSale/Services/`) | Método | Campo da API |
|---|---|---|
| `AfterSaleRefundsService.cs` | `GetRefunds` | `x.updated_at` |
| `AfterSaleRefundsService.cs` | `GetRefund` | `x.data.updated_at` |
| `AfterSaleReversesService.cs` | `GetReverses` | `x.updated_at` |
| `AfterSaleReversesService.cs` | `GetReverse` | `x.data.reverse.updated_at` |

> **Efeito colateral aceito:** um registro com `updated_at` nulo na API será regravado em toda execução, porque `null <= qualquer coisa` é `false`. Hoje ele seria comparado como `01/01/1990 <= 01/01/1990` e ignorado. Em reversas e reembolsos o `updated_at` sempre vem preenchido, então na prática não muda nada. Se um dia isso pesar, dá para usar o mesmo fallback do Movidesk para pessoas (`updated_at ?? created_at`).

---

## Passo 3 — Banco de dados

**Nenhuma alteração de estrutura.** Todas as colunas de data do AfterSale, em `untreated` e em `general`, já são `datetime` sem `not null`. A única exceção é a coluna da PK citada acima, que continua com valor.

O `SqlBulkCopy` já lida com `DateTime?`: o `CreateSystemDataTable` / `FillSystemDataTable` do `Shared` é o mesmo usado pelo Movidesk, que grava `DateTime?` nas tabelas de tickets e pessoas.

As procedures de merge (`sp_<Tabela>_Sync`, geradas por `dbo.sp_Create_Merge_Procedure`) fazem `when matched then update set` sem comparar colunas, porque as tabelas do AfterSale não têm coluna `timestamp`. Um `NULL` vindo do `untreated` sobrescreve o valor do `general` normalmente.

### (Opcional) Limpar o legado

Os registros já gravados continuam com `1990-01-01`. Eles serão corrigidos aos poucos, conforme forem reintegrados. Para corrigir tudo de uma vez, um script avulso (fora do DbUp, já que a decisão de não alterar o banco existente continua valendo) por tabela/coluna:

```sql
update [aftersale].[AfterSaleReverses]
set deleted_at = null
where deleted_at = '1990-01-01';
-- repetir para cada coluna da tabela do Passo 1
```

> Só faça isso se não houver datas reais em 01/01/1990. Para colunas `*_deleted_at`, `*_updated_at`, `ordered_at` etc. do AfterSale isso é praticamente garantido.

---

## Passo 4 — O que não muda

- **DTOs:** todas as datas já são `string`, então o JSON com `null` já desserializa.
- **Validações (FluentValidation):** as regras de data já usam `.When(x => !string.IsNullOrEmpty(x.campo))`, ou seja, já aceitam nulo.
- **Repositórios:** o `GetRecords<T>` do cache lê `ID, DOC_COMPANY, UPDATED_AT`. Com a propriedade `DateTime?`, ele passa a aceitar `NULL` do banco em vez de falhar.

## Pontos de atenção

- **Consumidores dos dados:** relatórios, views ou procedures fora deste módulo que filtrem `= '1990-01-01'` (ou `<> '1990-01-01'`) para dizer "sem data" precisam passar a usar `is null` / `is not null`.
- **Datas inválidas continuam como 1990:** um texto preenchido mas que não é data (ou é anterior a 1753) ainda vira `01/01/1990`, exatamente como no Movidesk. Isso é proposital: sem isso, o `SqlBulkCopy` falharia com *SqlDateTime overflow*.
- **Avisos de nulidade:** a troca não deve gerar erros de compilação. Os únicos usos das propriedades fora das entidades são as quatro comparações do Passo 2.

## Checklist

- [ ] 1. Trocar as propriedades da tabela do Passo 1 para `DateTime?` e usar `NullableConverters.ToNullableDateTime`
- [ ] 2. Trocar o conversor nas 4 comparações de cache
- [ ] 3. Compilar
- [ ] 4. Rodar `GET Reverses`, `GET Reverse`, `GET Refunds`, `GET Refund` e `GET Me`, e conferir no `general` que os campos que a API manda nulos (ex.: `deleted_at`) ficaram `NULL`
- [ ] 5. (Opcional) Script avulso de limpeza do legado `1990-01-01`
- [ ] 6. Revisar consumidores externos que dependem de `1990-01-01`
