# Regras de atribuição de prefixo dos pedidos

Levantado em 29/10/2026 a partir das definições **em produção no banco** `NEWBLOOMERS` (`sql-dev-webjobs`)
e do código do `newbloomers-recurring-jobs`.

Não existe um ponto único que define o prefixo. Ele é montado em **três lugares independentes**, cada um
com a sua própria regra e o seu próprio formato:

| # | Onde | Consumidor do número | Formato |
|---|------|----------------------|---------|
| 1 | Procs `general.p_Volo_Integracao_Pedidos_*` | WMS IT4 (`UNTREATED.IT4_WMS_DOCUMENTO.DOCUMENTO`) | `MI-VD12345T` |
| 2 | Funções `general.Gera_Prefixo_Pedidos` / `totvs.Gera_Prefixo_Pedidos` | DW (`dw.P_Dim_Pedido_ETL` e fatos de canal) | `MI-VD0012345T` |
| 3 | `OtoCommandHandler.cs` (C#) | OTO (`GENERAL.OTTOSENTRECORDSLOG`) | `MI-VD-1-12345` |

---

## 1. Estrutura do número

```
<SÉRIE><TIPO><CÓDIGO>[T]
  MI-    VD    12345   T
```

- **Série** (`MI-`, `OA-`, `MX-`): a marca / empresa emitente.
- **Tipo** (`VD`, `LJ`, `VF`, `RC`, `RV`, `ST`, `PI`, `PDV`): a natureza do pedido. **Pedido de e-commerce não tem tipo**: é só `MI-` + código.
- **Sufixo `T`**: marca pedidos originados no **Totvs** (o mesmo pedido no Microvix sai sem `T`).

### 1.1 Série (marca)

| Série | Marca | Microvix (`LINXLOJAS.RAZAO_EMP`) | Totvs — procs (`TOTVS.BRANCHES.DESCRIPTION`) | Totvs — função (raiz do CNPJ) |
|-------|-------|----------------------------------|----------------------------------------------|-------------------------------|
| `MI-` | Misha | `LIKE '%MNR%'` | `LIKE '%MISHA%'` | `38367316` |
| `OA-` | Open Era | `LIKE '%NEWFIT%'` ou `'%NEW FIT%'` | `LIKE '%OPEN ERA%'` ou `'%NEWFIT%'` | `42538267` |
| `MX-` | Margaux | `LIKE '%CVA%'` | — (comentado) | — (comentado) |

No OTO a série vem de `TOTVS.BRANCHES.DESCRIPTION LIKE '%MISHA%' OR '%MNR%'`; qualquer outra coisa vira `OA-`
(não há `MX-` nem caso nulo).

> As procs Totvs têm ainda uma coluna `SERIE` separada (`IIF(G.PERSONNAME LIKE '%MISHA%', 'MI-', 'OA-')`), que
> não participa do `DOCUMENTO`.

### 1.2 Tipos

| Tipo | Significado | Existe em |
|------|-------------|-----------|
| *(nenhum)* | E-commerce | Procs, OTO |
| `VD` | Venda direta (B2B / atacado) | Procs, funções, OTO |
| `LJ` | Reposição de lojas (CD → loja própria) | Procs, funções |
| `VF` | Venda fácil / prateleira infinita | Procs, funções |
| `RC` | Recebimento (fornecedor → CD) | Procs, funções |
| `RV` | Reversa (devolução e-commerce) | Funções |
| `ST` | E-commerce no DW Totvs | `totvs.Gera_Prefixo_Pedidos` |
| `PI` | Prateleira infinita | **Só OTO** |
| `PDV` | Venda de loja física (nota PDV) | **Só OTO** |

`PI` (OTO) e `VF` (WMS/DW) são o **mesmo tipo de pedido** — `OPERATIONNAME LIKE '%VENDA PRATELEIRA%'` — com
siglas diferentes por sistema.

---

## 2. Procs de integração com o WMS IT4

Cada proc monta o `DOCUMENTO` com um `CASE` próprio (não chamam `Gera_Prefixo_Pedidos`). Se nenhum `WHEN`
casa, o `CASE` devolve `NULL` e o `CONCAT` gera o número **sem prefixo**.

### 2.1 Totvs (`*_Totvs`)

Código = `CUSTOMERORDERCODE` quando existe, senão `ORDERCODE`; sempre com sufixo `T`.
Flags calculadas por pedido: `FL_EMP_*` = CNPJ da filial emitente (`TOTVS.BRANCHES G`) está na lista da marca;
`FL_CLI_*` = CNPJ do cliente (`CUSTOMERCPFCNPJ`) está na lista da marca.

| Proc | Prefixo | Condição |
|------|---------|----------|
| `p_Volo_Integracao_Pedidos_Ecommerce_Totvs` | `MI-` / `OA-` | `CUSTOMERORDERCODE IS NOT NULL` e `FL_EMP_MISHA` / `FL_EMP_OPEN` |
| `p_Volo_Integracao_Pedidos_Venda_Direta_Totvs` | `MI-VD` / `OA-VD` | `REPRESENTATIVENAME LIKE '%VENDA DIRETA%' OR '%VD%'` e `FL_EMP_*` |
| `p_Volo_Integracao_Pedidos_Reposicao_Lojas_Totvs` | `MI-LJ` / `OA-LJ` | `REPRESENTATIVENAME LIKE '%REPOSICAO LOJAS%'` e `FL_CLI_*` (marca do **cliente**) |
| `p_Volo_Integracao_Pedidos_Venda_Facil_Totvs` | `MI-VF` / `OA-VF` | `REPRESENTATIVENAME LIKE '%PRATELEIRA%'` e `OPERATIONNAME LIKE '%VENDA PRATELEIRA%'` e `FL_EMP_*` |
| `p_Volo_Integracao_Pedidos_Recebimentos_Totvs` | `MI-RC` / `OA-RC` | CNPJ da filial recebedora na lista da marca; código = `ORDERCODE` + `T` |

Exemplos: `MI-1234567T`, `OA-VD51310T`, `MI-LJ8812T`, `MI-VF4410T`, `OA-RC771T`.

### 2.2 Microvix (legado, sem sufixo `T`)

| Proc | Regra |
|------|-------|
| `p_Volo_Integracao_Pedidos_Venda_Direta` | **Gera VD e LJ na mesma proc**, por ordem: cliente (`DOC_CLIENTE`) é loja Misha → `MI-LJ`; loja Open → `OA-LJ`; senão emitente Misha → `MI-VD`; emitente Open → `OA-VD`. Código = `COD_PEDIDO`. |
| `p_Volo_Integracao_Pedidos_Venda_Facil` | `MI-VF`/`OA-VF` + **2 últimos dígitos do CNPJ emitente** + número da nota. Sem marca → `''`. |
| `p_Volo_Integracao_Pedidos_Recebimentos` | `MI-RC`/`OA-RC` + `COD_PEDIDO`; se o pedido se repete, `COD_PEDIDO-N` (`ROW_NUMBER`). |
| `p_Volo_Integracao_Pedidos_Venda_Direta_Segunda_Qualidade` | Tabela fixa: `MARGAUX → MX-RC`, `MISHA → MI-RC`, `OPENERA → OA-RC`. |

Na lista Open Era de `p_Volo_Integracao_Pedidos_Venda_Direta` há uma exceção fixa: o cliente `42538267000853` (**Open Era Alphaville**, aberta no
Totvs) entra como loja via `LINXCLIENTESFORNEC`.

A view `general.vw_pedidos_erp_unificados` repete as mesmas regras Microvix (VF com 2 dígitos do CNPJ;
LJ/VD pelo documento do cliente).

---

## 3. Funções `Gera_Prefixo_Pedidos` (DW)

Usadas apenas por `dw.P_Dim_Pedido_ETL`, `dw.P_Fato_Canal_Movimentacao_ETL` e
`dw.P_Fato_Canal_Movimentacao_Captado_ETL`. O tipo é decidido pelos **parâmetros informados**, na ordem abaixo
(o primeiro que casa vence).

### 3.1 `general.Gera_Prefixo_Pedidos` (Microvix)

| Ordem | Parâmetros | Tipo |
|-------|-----------|------|
| — | `@ORDER_ID` informado | **Sem prefixo gerado**: devolve o próprio `@ORDER_ID` sem `-CANCELLED` |
| 1 | `@COD_FORNECEDOR` | `RC` |
| 2 | `@REVERSE_TYPE` + `@ORDER_ID` | `RV` — **inalcançável**: o bloco só roda com `@ORDER_ID IS NULL` |
| 3 | `@COD_PEDIDO` + `@DOC_CLIENTE` não é loja do grupo | `VD` |
| 4 | `@COD_PEDIDO` + `@DOC_CLIENTE` é loja do grupo | `LJ` |
| 5 | `@DOCUMENTO` + rotina de origem `LIKE '%VENDA FÁCIL%'` | `VF` + 2 últimos dígitos do CNPJ |

Retorno: `SÉRIE + TIPO + código`, sem zeros e sem `T` (ex.: `MI-VD123`, `OA-VF874410`).

### 3.2 `totvs.Gera_Prefixo_Pedidos`

| Ordem | Parâmetros | Tipo |
|-------|-----------|------|
| — | `@ORDER_ID` informado | `ST` (e-commerce) |
| 1 | `@COD_FORNECEDOR` | `RC` |
| 2 | `@REVERSE_TYPE` | `RV` |
| 3 | `@COD_PEDIDO` + **emitente** é loja do grupo que não é CD (`38367316000199`, `42538267000187`) | `LJ` |
| 4 | `@COD_PEDIDO` + cliente não é loja do grupo | `VD` |
| 5 | `@DOCUMENTO` + nota elegível com representante `PRATELEIRA%` | `VF` + 2 últimos dígitos do CNPJ |

Retorno: `SÉRIE + TIPO + código com 7 dígitos + T` (ex.: `MI-VD0012345T`, `OA-ST0098765T`).

Diferença importante: aqui `LJ` olha o **emitente** (loja que não é CD); na função Microvix e nas procs, `LJ`
olha o **cliente** (o pedido é *para* uma loja).

---

## 4. OTO (`OtoCommandHandler.CreateGetOrdersQuery`)

Quatro fatias num único `UNION ALL`, todas a partir de 26/07/2026 (rollout Microvix → Totvs). Série pela
`DESCRIPTION` da filial (§1.1). O hífen entre filial e pedido evita colisões (`MI-VD-1-1100` ≠ `MI-VD-11-100`).

| Fatia | Origem | Filtro | Número |
|-------|--------|--------|--------|
| PDV | `TOTVS.INVOICES` | `INCLUSIONCOMPONENTCODE LIKE '%PDVFM%'`, operação de venda, não devolução | `MI-PDV-{InvoiceSequence}T` |
| E-commerce | `TOTVS.SALEORDERS` | `INTEGRATIONCODE IS NOT NULL` | `MI-{BranchCode}-{OrderCode}T` |
| Venda direta | `TOTVS.SALEORDERS` | `REPRESENTATIVENAME LIKE '%VENDA DIRETA%' OR '%VD%'` e **não** prateleira | `MI-VD-{BranchCode}-{OrderCode}` |
| Prateleira infinita | `TOTVS.SALEORDERS` | `OPERATIONNAME LIKE '%VENDA PRATELEIRA%'` | `MI-PI-{BranchCode}-{OrderCode}` |

As fatias de pedido exigem ainda `STATUSORDER <> 'Blocked'` e `TOTALAMOUNTORDER > 0`. OTO não recebe `LJ`, `RC`
nem `RV`.

---

## 5. Onde o prefixo é lido

| Local | Uso |
|-------|-----|
| `CompanyStateService.cs:45` (frontend) | Empresa selecionada → `MI-` / `OA-` |
| `PickingCommandHandler.cs:203` (backend) | Remove `MI-VD` e `MI-LJ` para achar o `COD_PEDIDO` no Microvix |
| `StoneService.cs:76` | `MI-` → conta `3294_29_`; senão `3294_30_` |
| `JadlogRepository.cs:153` | `SERIE = 'MI-'` → CNPJ `38367316000199`; senão `42538267000187` |
| `OtoService.cs:210` | Separa os pedidos por marca (`Contains("MI-")` / `"OA-"`) |
| `P_Volo_Integracao_Infos_Pedidos` / `Infos_NFe` | Filtram `SERIE = 'MI-'` e excluem `SERIE = 'MX-'` |
| `general.Verifica_CNPJs_Lojas` | Converte `MI-` → `MISHA`, `OA-` → `OPEN-ERA` |

---

## 6. Inconsistências encontradas

1. **Três formatos para o mesmo pedido.** Uma venda direta Totvs sai como `MI-VD12345T` (WMS),
   `MI-VD0012345T` (DW) e `MI-VD-1-12345` (OTO). Cruzar os sistemas pelo número exige normalização.
2. **`VF` × `PI`.** O mesmo tipo de pedido (venda prateleira) tem sigla `VF` no WMS/DW e `PI` no OTO.
3. **Critério de série diferente** entre Microvix (`RAZAO_EMP`), procs Totvs (`DESCRIPTION`), função Totvs (raiz
   do CNPJ) e OTO (`DESCRIPTION`, com fallback para `OA-`). Uma filial nova com descrição fora do padrão vira
   `OA-` no OTO e fica **sem prefixo** nas procs.
4. **`LJ` olha lados diferentes**: cliente nas procs e na função Microvix; emitente na função Totvs.
5. **`RV` inalcançável** em `general.Gera_Prefixo_Pedidos` (exige `@ORDER_ID` dentro de um bloco que só roda
   com `@ORDER_ID IS NULL`).
6. **OTO e-commerce: deduplicação nunca casa.** O `NUMBER` é gravado como `MI-{Branch}-{Order}T`
   (`OtoCommandHandler.cs:705`), mas o `NOT EXISTS` em `OTTOSENTRECORDSLOG` compara com
   `MI-{Branch}-{Order}` sem `T` (`:956`). Os pedidos de e-commerce voltam em toda execução.
7. **`PickingCommandHandler` só trata Misha**: remove `MI-VD` / `MI-LJ` mas não `OA-VD` / `OA-LJ`, e não
   remove o sufixo `T` dos pedidos Totvs.
8. **`CASE` sem `ELSE`** nas procs Totvs: pedido de filial fora das listas sai com número sem prefixo, em vez
   de falhar.
