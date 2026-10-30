# Revisão: AfterSale, Movidesk, LinxMicrovix e Totvs contra o padrão

Levantamento dos pontos em que os módulos não seguiam o
[`PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md`](PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md). Cada item cita a seção do padrão e
o que mudou.

> **Situação (30/10/2026): todos os itens foram aplicados**, com as decisões abaixo. Commits na `main`:
> AfterSale `9823566`, `4db66a6` e `4dd25ac`; Movidesk `dad9074` e `1272c5d`; LinxMicrovix `0a3b10b`. Depois de cada
> passo, o host Hangfire compilou sem erros e com os mesmos avisos; nos scripts, as tabelas, índices, FKs, procedures e
> jobs foram conferidos contra a versão anterior.
>
> Decisões:
> 1. `AfterSaleErrors`, a sua procedure e o job 5536 ficaram em `Parameters/`.
> 2. `AfterSaleHistoriesReverses` (5010) saiu do `EnumJob` e, junto com o job da sua procedure (5516), do script de
>    jobs de `Reverses/`. A tabela, a procedure de merge, os DTOs e a entidade ficam para o método que ainda falta.
> 3. Os jobs sob demanda do Movidesk já nascem com os títulos `*ById`; o `Script012` saiu.

Resumo:

| Seção do padrão | AfterSale | Movidesk | LinxMicrovix | Totvs |
|---|---|---|---|---|
| §2 `Save*` só se reutilizado | ok | ok | ok | ok |
| §2.1 Demais métodos privados | feito (1 método) | ok | ok | ok |
| §2.2 Ordem dos membros e separador | feito (2 services) | feito (3 services) | feito (14 services) | ok |
| §2.3 `Validations` espelha os DTOs | feito | feito | feito | ok |
| §2.4 Um command handler por service | feito (3 handlers) | feito (3 handlers) | ok | ok |
| §3 Scripts DbUp | feito (15 scripts) | feito (13 scripts) | ok | ok |

O `LinxMicrovix - Inbound Web Service` não tem nenhum arquivo `.cs` e ficou fora da revisão.

---

## 1. AfterSale (`src/Backend/Modules/General/AfterSale`)

### 1.1 §2.1 Demais métodos privados

| Service | Método | Pai | Proposta |
|---|---|---|---|
| `AfterSaleRefundsService` | `ToJsonBody` (5 linhas, 1 chamada) | `PostChangeTotalAmountRefundById` | Código direto no pai |

O `ToJsonBody` da `AfterSaleReversesService` (5 pais) e os `SendRefundAction`/`SendReverseAction` (4 e 6 pais) ficam.

### 1.2 §2.2 Ordem dos membros

`AfterSaleRefundsService` e `AfterSaleReversesService` já têm os privados no fim, mas **sem a linha separadora**
`// ------...`. Só falta inserir o separador.

### 1.3 §2.3 `Validations` espelha os DTOs

Os DTOs estão em `Dtos/Reads/{Me,Refunds,Reverses}` e `Dtos/Writes/...`, mas os 91 validadores estão na raiz:
`Validations/Me`, `Validations/Refunds` e `Validations/Reverses`. Proposta: mover para `Validations/Reads/...`, com o
namespace acompanhando (`Application.AfterSale.Validations.Reads.*`), e ajustar os `using` que os citam. Não há
validadores de escrita.

> Observação fora do padrão: os validadores do AfterSale terminam em `Validation` (`AddressValidation`), e os dos
> outros módulos em `Validator`. O padrão não define o sufixo; não entra nesta revisão, a menos que você queira.

### 1.4 §2.4 Um command handler por service

O `AfterSaleCommandHandler` tem 16 queries:

| Handler novo | Repositório | Queries |
|---|---|---|
| `AfterSaleRefundsCommandHandler` | `AfterSaleRefundsRepository` | 7 |
| `AfterSaleReversesCommandHandler` | `AfterSaleReversesRepository` | 7 |
| `AfterSaleCommandHandler` (comum) | `AfterSaleRepository` | 2 |

O `AfterSaleMeRepository` recebe o handler no construtor mas não usa nenhuma query: sai a injeção e ele não ganha
handler (a service Me não tem query). Três registros novos na DI.

### 1.5 §3 Scripts DbUp

Hoje são 10 scripts soltos em `SQLServer/`, com as procedures (005) e os jobs (007) das três services num arquivo só,
e um `ALTER` (010).

| Pasta | Script | Conteúdo (origem) |
|---|---|---|
| `Parameters/` | 001 | Schema, `AfterSaleParameters` e `AfterSaleErrors` (001) |
| `Parameters/` | 002 | App 50 (006) |
| `Parameters/` | 003 | Parâmetros (008) |
| `Parameters/` | 004 | `AfterSaleRequests`, já com `integrated_or_updated_at` (009 + **010 incorporado**, 010 excluído) |
| `Me/` | 005–007 | Tabelas (002), as 6 procedures das tabelas Me (de 005) e os jobs de Me (de 007) |
| `Refunds/` | 008–010 | Tabelas (003), as 13 procedures (de 005) e os jobs de Refunds (de 007) |
| `Reverses/` | 011–013 | Tabelas (004), as 17 procedures (de 005) e os jobs de Reverses (de 007) |

* **Decidir:** a tabela `AfterSaleErrors` é gravada pelas duas services (Refunds e Reverses). Proposta: ela, a sua
  procedure e o job `sp_AfterSaleErrors_Sync` (5536) ficam em `Parameters/`, como coisa comum do módulo.
* Os nomes passam a ter o módulo (`Script001_Create_Schema_And_Parameters_Table` →
  `Script001_Create_AfterSale_Schema_And_Parameters_Table`, `Create_Me_Tables` → `Create_AfterSale_Me_Tables`).
* O job `AfterSaleHistoriesReverses` (5010) e a sua procedure (5516) não são referenciados por nenhuma service no
  `EnumJob`; vão para `Reverses/` pela tabela. **Decidir:** se o job ainda é usado (pode ser resto do código antigo).
* `migrator.csx` com `IncludeSubDirectories` e o sorter pelo nome do arquivo (§3.4).
* Atualizar o README do AfterSale, comentários do `EnumJob`/validadores que citam scripts e a premissa 10 do DW
  (cita o `Script010` do AfterSale).

---

## 2. Movidesk (`src/Backend/Modules/General/Movidesk`)

### 2.1 §2 e §2.1

Ok: `SavePeople`, `SaveServices` e `SaveTickets` têm 3 ou 4 pais; `SendPerson`, `SendService` e `SendTicket` também.

### 2.2 §2.2 Ordem dos membros

`MovideskPeopleService`, `MovideskServicesService` e `MovideskTicketsService` já têm os privados no fim, **sem o
separador**.

### 2.3 §2.3 `Validations` espelha os DTOs

| Hoje | Proposta |
|---|---|
| `Validations/People` | `Validations/Reads/People` |
| `Validations/Services` | `Validations/Reads/Services` |
| `Validations/Ticket` | `Validations/Reads/Tickets` (o DTO está em `Dtos/Reads/Tickets`, no plural) |
| `Validations/Writes/Services` | ok |

São 36 arquivos; o namespace acompanha a pasta.

### 2.4 §2.4 Um command handler por service

O `MovideskCommandHandler` tem 8 queries:

| Handler novo | Repositório | Queries |
|---|---|---|
| `MovideskTicketsCommandHandler` | `MovideskTicketsRepository` | 3 |
| `MovideskPeopleCommandHandler` | `MovideskPeopleRepository` | 3 |
| `MovideskCommandHandler` (comum) | `MovideskRepository` | 2 |

O `MovideskServicesRepository` recebe o handler mas não usa nenhuma query: sai a injeção.

### 2.5 §3 Scripts DbUp

Hoje são 13 scripts soltos. Os jobs das três services estão todos no 005; há um `ALTER` (013) e um script que
**renomeia** jobs com `UPDATE` (012).

| Pasta | Script | Conteúdo (origem) |
|---|---|---|
| `Parameters/` | 001 | Schemas e `MovideskParameters` (001) |
| `Parameters/` | 002 | App 30 (004) |
| `Parameters/` | 003 | Parâmetros (011) |
| `Parameters/` | 004 | `MovideskRequests`, já com `integrated_or_updated_at` (006 + **013 incorporado**, 013 excluído) |
| `Tickets/` | 005–007 | Tabelas (002), procedures (003) e jobs de Tickets (de 005) |
| `People/` | 008–010 | Tabelas (007), procedures (008) e jobs de People (de 005) |
| `Services/` | 011–013 | Tabelas (009), procedures (010) e jobs de Services (de 005) |

* **Decidir:** o 012 corrige os títulos de três jobs sob demanda (`MovideskGetTicket` → `MovideskGetTicketById`,
  `MovideskGetPerson` → `MovideskGetPersonById`, `MovideskGetService` → `MovideskGetServiceById`). Pela mesma ideia
  do §3.3, proposta: o insert de jobs já nasce com os títulos novos e o 012 é excluído.
* `migrator.csx` com `IncludeSubDirectories` e o sorter (§3.4).
* Atualizar o README do Movidesk, os comentários que citam scripts e a premissa 10 do DW (cita o `Script013`).

---

## 3. LinxMicrovix Outbound (`src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service`)

### 3.1 §2.2 Ordem dos membros

Os 24 `Save*` reutilizados já estão no fim das classes, mas **sem o separador**, em 14 services:

`B2CCustomers` (1), `B2COrders` (4), `Customers` (1), `FiscalDocuments` (1), `Movements` (1), `Orders` (1),
`Payments` (3), `Prices` (2), `ProductAttributes` (1), `Products` (3), `Returns` (1), `Sellers` (1), `Stocks` (3) e
`Taxes` (1).

### 3.2 §2.3 `Validations` espelha os DTOs

Os DTOs estão em `Dtos/Reads/{B2C,Erp}/...`; os 261 validadores, em `Validations/{B2C,Erp}/...`. Proposta: mover para
`Validations/Reads/B2C` e `Validations/Reads/Erp`, com o namespace acompanhando.

### 3.3 Já conforme

* §2.4: o `LinxMicrovixCommandHandler` só tem as 4 queries comuns, usadas pelo `LinxMicrovixRepository`; os
  repositórios das services não usam queries.
* §3: scripts em `Parameters/`, `Erp/<Service>` e `B2C/<Service>`, `migrator.csx` com subpastas.

---

## 4. Totvs (`src/Backend/Modules/Totvs`)

Conforme em todas as seções. Observação: `TotvsMasking` e `TotvsQuery` estão na pasta `Services` sem serem services
(são utilitários); o padrão já os deixa de fora.

---

## 5. Ordem sugerida

1. AfterSale e Movidesk: §2.1 e §2.2 (pequenos), §2.3, §2.4, §3 — um commit por seção.
2. LinxMicrovix: §2.2 e §2.3.

Em todos, depois de cada passo: build do host Hangfire, comparação dos avisos e, nos scripts, a conferência de que as
tabelas, procedures, índices, FKs e jobs continuam os mesmos.
