# DW do NEWBLOOMERS · 07. Ordem de execução e qualidade

> Guia 7 de 12 do guia do DW, versão 7. Seções deste arquivo: 10 e 11. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 10. Ordem de execução das procedures

### 10.1 Sequência de uma carga

A ordem importa porque cada passo lê o que o anterior gravou. O job que dispara os passos, a agenda e o tratamento de falhas ficam fora deste guia; aqui está só o que cada passo exige dos objetos do banco.

| # | Passo | Objetos do banco |
|---|---|---|
| 1 | Lê os watermarks de cada fonte e entidade | `dw_control.Watermark` |
| 2 | Atualiza a `dw_landing` por MERGE, das tabelas confiáveis com `integrated_or_updated_at` acima do watermark menos 3 dias (5.2, regra 1), e avança o watermark após o sucesso de cada tabela | `dw_landing.sp_<Tabela>_Load` |
| 3 | Detecta exclusões de filhas: anti-join entre as chaves da landing e da tabela confiável; chave ausente recebe `missing_since` (5.2, regra 3) | `dw_landing.sp_<Tabela>_DetectMissing` (no ciclo diário) |
| 4 | Roda os snapshots SCD2, só sobre as linhas que o passo 2 gravou (seção 6). A cada hora para notas e pedidos; diária, no fim do dia, para cadastros, saldos, custos e preços (3.3, 7.4) | `dw_history.sp_<Tabela>_Snapshot` |
| 5 | Carrega `dw_personal_data` direto das tabelas de origem (5.2, regra 6) | `dw_personal_data.sp_Customer_Load`, `dw_personal_data.sp_OrderShippingAddress_Load`, `dw_personal_data.sp_ServiceCustomer_Load` |
| 6 | Carrega dimensões (a de cliente recebe `@pepper`, 8.3; `DimDate` só no ciclo diário), depois fatos (os de ticket e reembolso também recebem `@pepper`; o de estoque só no ciclo diário do dia 1), que leem as views de `dw_intermediate` (formato unificado, regras, canal, tipo e partes do número do pedido), depois agregados | `datawarehouse.sp_Dim*_Load`, `dw_intermediate.vw_*`, `datawarehouse.sp_Fact*_Load`, `datamart.sp_*_Refresh` |
| 7 | Roda as checagens: conta as linhas de cada view e grava o resultado | `datawarehouse.vw_Check_*`, `dw_control.CheckResult` |
| 8 | Registra início, mensagens e fim da execução | `auditing.Logs` e `auditing.Messages`, pelo `ILoggerService` do Shared (9.4) |

Vendas e pedidos passam por essa sequência a cada hora; cadastros, saldos, custos e preços, uma vez por dia. A primeira carga usa as janelas da 5.2, regra 5. `FactMonthlyStock` é carregado uma vez por mês, pelo ciclo diário do primeiro dia do mês seguinte; um mês se refaz pela entrada `RunMonthlyStock` (8.5, 9.4). As views de `dw_outbound` não têm passo: as do ciclo horário refletem o passo 6 assim que ele termina; as da logística refletem o ciclo curto da 10.3. Cada consumidor varre quando quiser (4.2).

### 10.2 Carga única do legado Linx

Fora da sequência acima, executada uma vez na fase 3 (7.5) pela entrada `RunLinxLegacy` (job 9004): landing de todas as origens Linx do catálogo (passo 2, sem janela: a origem já tem o histórico inteiro), snapshot único de lojas e produtos (passo 4), clientes Linx em `dw_personal_data` (`sp_LinxCustomer_Load`, passo 5), as dimensões, que refazem Totvs e Linx juntas e aplicam o de/para de produto (`dw_intermediate.vw_ProductMatch`, passo 6), `dw_intermediate.sp_LinxLegacy_Load`, que grava as linhas Linx em `FactSalesItem` e `FactOrderItem`, `DailySales` e as checagens. O watermark fica gravado; se a origem receber correções, a mesma entrada pode ser reexecutada. Antes da primeira execução em produção, cadastrar em `dw_reference.StoreErpMap` o período Linx das lojas que migraram para a Totvs; o que faltar aparece em `vw_Check_ErpOverlap`, que bloqueia.

### 10.3 Ciclo curto de 3 minutos (logística)

A logística precisa do que os jobs da Totvs acabaram de trazer (pergunta 35, decidida): pedidos, notas e clientes a cada 3 minutos. Rodar a sequência inteira da 10.1 nesse intervalo não cabe, então há um ciclo curto, com dois passos da 10.1 e nenhum outro:

| # | Passo | Objetos do banco |
|---|---|---|
| 1 | Passo 2 da 10.1 só para `TotvsSaleOrders`, `TotvsSaleOrdersProducts`, `TotvsInvoices`, `TotvsInvoicesItems`, `TotvsInvoicesSalesOrders`, `TotvsPeople` e `TotvsLegalEntities`, com os mesmos watermarks | `dw_landing.sp_<Tabela>_Load` |
| 2 | Passo 5 da 10.1: clientes e endereços de entrega | `dw_personal_data.sp_Customer_Load`, `dw_personal_data.sp_OrderShippingAddress_Load` |

- As views da logística (`dw_outbound.LogisticsShipments`) leem `dw_intermediate.vw_OrderParts`, uma view sobre a landing que aplica `dw_reference.ChannelRules` inline, e `dw_personal_data`; não dependem de dimensões nem fatos, que só são carregados no passo 6 do ciclo horário (8.6).
- O ciclo horário continua igual: quando ele roda, o passo 2 encontra a landing já atualizada pelo ciclo curto e carrega só o que restou. Os snapshots seguem horários; o `loaded_at` da landing garante que todas as linhas do ciclo curto entrem no snapshot seguinte (seção 6).
- Os dois ciclos não rodam ao mesmo tempo sobre a mesma tabela: o ciclo curto espera o horário terminar o passo 2 (o watermark é a trava: cada carga avança o da própria tabela).
- Os demais consumidores (Oto, Pagarme, Dootax, Mobsim, WMS) leem o ciclo horário.

---

## 11. Qualidade e reconciliação

| Verificação | Onde | Quando | Limite |
|---|---|---|---|
| Schema e tipos | Constraints e DDL | Todo script | Bloqueia |
| Unicidade do grão | View de checagem (9.2) | Toda carga | Bloqueia |
| Integridade fato → dimensão | View de checagem, ou FK se a carga aguentar (8.4) | Toda carga | Bloqueia |
| Proporção de membro desconhecido | View de checagem | Toda carga | Alerta acima de 5% |
| Soma dos produtos de um item = valor do item | `vw_Check_ItemProducts` (9.3) | Toda carga | Alerta acima de 0,1% dos itens |
| Pedido sem tipo reconhecido | `vw_Check_OrderTypeUnknown` (9.3) | Toda carga | Alerta; bloqueia a view do WMS se passar de 0,5% |
| Frescor da landing | Watermark comparado à hora atual | Antes da carga | Alerta se atrasar mais de 2 h |
| Exclusões de filhas | View de checagem sobre `missing_since` (5.2, regra 3) | Diário | Alerta acima de 0,5% das chaves ativas |
| Total mensal por loja vs fechamento de referência | `datamart.vw_Check_InvoicedReconciliation` (9.3) | Diário | Diferença acima de R$ 1,00 por loja e mês |
| Sobreposição de ERPs no mesmo CNPJ e dia | `vw_Check_ErpOverlap` (9.3) | Toda carga | Bloqueia |
| Produtos Linx sem de/para | `vw_Check_ProductMatch` (9.3) | Toda carga (só muda na carga do legado) | Alerta; meta definida depois da primeira execução (pergunta 34) |
| Posição de estoque negativa ou sem custo | `vw_Check_StockNegativeOrNoCost`, sobre a última posição de `FactMonthlyStock`, por loja | Mensal | Alerta |
| Consumidor atrasado | `dw_control.Watermark` de cada módulo de saída comparado ao `updated_at` da view | Diário; a cada hora para a logística | Alerta se o watermark do consumidor ficar mais de 1 dia atrás (4.2); logística, mais de 30 minutos (10.3) |

Os limites são ponto de partida. Ajustar após duas semanas de carga.
