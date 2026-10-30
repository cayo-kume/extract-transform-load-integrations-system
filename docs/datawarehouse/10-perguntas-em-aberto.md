# DW do NEWBLOOMERS · 10. Perguntas em aberto

> Guia 10 de 12 do guia do DW, versão 7. Seções deste arquivo: 17. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 17. Perguntas da modelagem ⚠️

Só as perguntas que mudam tabelas, views ou procedures. As demais (operação, BI, ambientes, CI/CD, custos e credenciais) saíram do guia na versão 5; a numeração original foi mantida, por isso há lacunas. A seção 21 lista as removidas.

As decididas estão marcadas com ✅ e aplicadas no texto. Em aberto: 33.

**A. Banco e carga**

1. ✅ **Decidido:** o DW começa do zero na primeira carga do banco novo. O histórico do DW atual não entra.
5. ✅ **Decidido:** banco zerado com os schemas `datawarehouse` e `datamart`, junto com os módulos de integração; os demais schemas do DW levam o prefixo `dw_` (3.1 e 4.1).
6. ✅ **Decidido:** transformação em T-SQL, sem dbt (3.4).
8. ✅ **Decidido:** recarga completa do fato de venda a cada execução (9.1); otimizar depois de medir.
9. ✅ **Decidido:** snapshot a cada hora para notas e pedidos, junto com a carga, e diário para cadastros (3.3). O snapshot é incremental (seção 6). Saldos, custos e preços seguem o diário, no fim do dia (7.4).
35. ✅ **Decidido:** a logística precisa da landing a cada 3 minutos para pedidos, notas e clientes; os demais módulos de saída leem o ciclo horário. Virou o ciclo curto da 10.3: só a landing dessas tabelas e a carga de `dw_personal_data` rodam a cada 3 minutos, as views da logística leem `dw_intermediate.vw_OrderParts` sobre a landing, e os snapshots continuam horários (4.2, 8.6).

**B. Modelo**

10. ✅ **Decidido (7.4):** estoque não é "append" nem "substituir o dia". `totvs.TotvsBalances` guarda só a posição atual, então o histórico é o mesmo SCD2 das outras tabelas, com snapshot diário no fim do dia; a posição de qualquer data sai de `dw_history.TotvsBalances`, e `FactMonthlyStock` guarda o fim de cada mês (8.5).
11. ✅ **Decidido:** `DimCustomer` é SCD1 (7.1 e 8.3). Com a regra 6 da 5.2, SCD2 exigiria versionar UF e endereço fora de `dw_personal_data`, que não guarda versões.
13. ✅ **Decidido:** nenhuma loja vende por Linx hoje (descontinuado). O passado entra pela carga única do legado (7.5): a loja Linx é ligada à `DimStore` pelo CNPJ (`LinxLojas.cnpj_emp` = `TotvsBranches.cnpj`), e `dw_reference.StoreErpMap` guarda só as exceções, com período. Só o Totvs gera linhas novas.
14. ✅ **Decidido (7.4):** histórico de estoque desde a primeira carga, sem limite, até a medição do volume na carga de teste (versões por dia de `dw_history.TotvsBalances`). Se o volume exigir, a retenção passa a ser por versão (apagar versões fechadas com mais de N anos), nunca o fato mensal (arquivo 12).
15. ✅ **Decidido:** canal de e-commerce no Totvs é o pedido de `totvs.TotvsSaleOrders` com `integrationCode` não nulo; nulo é loja. A nota herda o canal do pedido por `TotvsInvoicesSalesOrders` (7.1 e 7.2). No legado, `LinxB2CPedidos` e `LinxCommerceOrders` são e-commerce; `LinxMovimento` é loja. O tipo do pedido (`VD`, `LJ`, `VF`, `RC`, `RV`, `ST`, `PDV`) segue `docs/regras-prefixo-pedidos.md` e fica em `DimChannel`; `LJ` e `RC` não são venda.
23. ✅ **Decidido:** `datamart`, sem prefixo, como `datawarehouse` (3.1).
26. ✅ **Decidido:** `DimOperation` a partir dos campos de operação da nota do Totvs (`operationCode`, `operatioName`, `operationType`) e, no Linx, da natureza de operação de `LinxMovimento` (7.1).
33. ⚠️ **Parcialmente decidida.** O catálogo de tipos de valor veio da tela PRDFL003 (29/10/2026) e está em `dw_reference.ValueCodes` (8.7). Custos: 1 `CUSTO ICMS`, 2 `ULTIMA COMPRA`, 3 `MEDIO S/ IMPOSTO`, 4 `MEDIO C/ IMPOSTO`, 5 `REMESSA`, 6 `SEM IMPOSTO`, 7 `CUSTO FINAL`, 8 `CUSTO ICMS`. Preços: 1 `VENDA LOJA`, 2 `VENDA DOLAR`, 3 `VENDA EURO`, 4 `VENDA ECOMMERCE`, 5 `VENDA ORIGINAL`, 6 `VENDA PROMOCIONAL`. Pela API, cada linha de `costs/search`, `prices/search` e `balances/search` traz o código e o nome, então a carga de teste confirma que a tabela e a tela batem.
    - ✅ **Preços, decididos pela descrição:** `list_price` é o código 1 no canal loja e o 4 no e-commerce; `original_price` é o 5 e `promotional_price` é o 6 (7.3, 8.4). Dólar e euro ficam só no histórico.
    - ✅ **Custo da margem, decidido:** código 2 `ULTIMA COMPRA`, papel `margin_cost` em `ValueCodes`; a medida é `unit_cost` em `FactSalesItem` e `FactMonthlyStock` (7.3, 8.4, 8.5). Trocar o código é um `UPDATE` em `ValueCodes`. Os códigos 1 e 8 têm a mesma descrição; conferir na carga de teste qual está preenchido.
    - ⚠️ **Depósito vendável (`stockCode`):** não está na PRDFL003. Sai de `SELECT DISTINCT stockCode, stockDescription FROM totvs.TotvsBalances` depois da primeira carga de saldos (ou da tela de depósitos da Totvs); marcar o papel `sellable_stock`.
34. ✅ **Decidido (7.1, 7.5):** de/para do produto Linx por `LinxProdutos.referencia`, `desc_cor` e `desc_tamanho` × `TotvsSkus.referenceCode`, cor de `TotvsSkusColors` e `size` de `TotvsSkusColorsProducts`, com normalização de texto, na view `dw_intermediate.vw_ProductMatch`. O resultado fica em `DimProduct.matched_product_sk`; sem casamento, o membro Linx fica próprio, e `dw_reference.ProductErpMap` corrige à mão os pares que o texto não resolve. `vw_Check_ProductMatch` mede a proporção casada por marca.

**C. Dado pessoal**

32. ✅ **Decidido:** o `doc_hash` é calculado na procedure `sp_DimCustomer_Load`, com o pepper como parâmetro (12.3). A alternativa, calcular fora do banco com a procedure lendo só o resultado, foi descartada. O mesmo hash liga o cliente entre Totvs e Linx (8.3).

A pergunta 19 (retenção e anonimização de dado pessoal) foi retirada: a premissa é que não haverá pedido de exclusão nem de anonimização no DW (12.1).
