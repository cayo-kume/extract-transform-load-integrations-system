INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (pedidos de venda: 13559 - 13569)*/
(13005, 13559, 'sp_TotvsSaleOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrders raw e trusted', 3, 1),
(13005, 13560, 'sp_TotvsSaleOrdersProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersProducts raw e trusted', 3, 1),
(13005, 13561, 'sp_TotvsSaleOrdersProductsBatchItems_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersProductsBatchItems raw e trusted', 3, 1),
(13005, 13562, 'sp_TotvsSaleOrdersInvoices_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersInvoices raw e trusted', 3, 1),
(13005, 13563, 'sp_TotvsSaleOrdersObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersObservations raw e trusted', 3, 1),
(13005, 13564, 'sp_TotvsSaleOrdersInvoiceObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersInvoiceObservations raw e trusted', 3, 1),
(13005, 13565, 'sp_TotvsSaleOrdersRepresentativeObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersRepresentativeObservations raw e trusted', 3, 1),
(13005, 13566, 'sp_TotvsSaleOrdersClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersClassifications raw e trusted', 3, 1),
(13005, 13567, 'sp_TotvsSaleOrdersDiscounts_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersDiscounts raw e trusted', 3, 1),
(13005, 13568, 'sp_TotvsSaleOrdersCommissioneds_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersCommissioneds raw e trusted', 3, 1),
(13005, 13569, 'sp_TotvsSaleOrdersCounts_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrdersCounts raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13005, 'TotvsPostSearchSalesOrders', 'Integração dos pedidos de venda de todas as lojas (TotvsSaleOrders e filhas), a cada 3 minutos', 3, 2),

/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13012, 'TotvsGetSalesOrderByKey', 'Integração de um pedido de venda por loja e número (GET SalesOrders/{docCompany}/{branchCode}/{orderCode})', 0, 2),

/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (catálogos e sugestões de faturamento da API Sales Order: 13642 - 13647)*/
(13190, 13642, 'sp_TotvsSaleDiscountTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleDiscountTypes raw e trusted', 120, 1),
(13191, 13643, 'sp_TotvsSaleClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleClassifications raw e trusted', 120, 1),
(13192, 13644, 'sp_TotvsSaleOrderBatches_Sync', 'Procedure de sincronização entre as tabelas TotvsSaleOrderBatches raw e trusted', 120, 1),
(13194, 13645, 'sp_TotvsBillingSuggestions_Sync', 'Procedure de sincronização entre as tabelas TotvsBillingSuggestions raw e trusted', 3, 1),
(13194, 13646, 'sp_TotvsBillingSuggestionsOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsBillingSuggestionsOrders raw e trusted', 3, 1),
(13194, 13647, 'sp_TotvsBillingSuggestionsOrdersItems_Sync', 'Procedure de sincronização entre as tabelas TotvsBillingSuggestionsOrdersItems raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249, docs/PLANO-TOTVS-METODOS-FALTANTES.md); 13193 reservado (changed-batch-items, fase SO-2)*/
(130, 13190, 'TotvsGetSalesOrderDiscountType', 'Integração do catálogo de tipos de desconto da API Sales Order (TotvsSaleDiscountTypes), a cada 2 horas', 120, 2),
(130, 13191, 'TotvsGetSalesOrderClassifications', 'Integração do catálogo de classificações de pedido de venda da API Sales Order (TotvsSaleClassifications), a cada 2 horas', 120, 2),
(130, 13192, 'TotvsGetSalesOrderBatchItems', 'Integração do catálogo de lotes de pedidos de venda da API Sales Order (TotvsSaleOrderBatches), a cada 2 horas', 120, 2),
(130, 13194, 'TotvsGetSalesOrderBillingSuggestions', 'Integração das sugestões de faturamento, com pedidos e itens (TotvsBillingSuggestions e filhas), a cada 3 minutos', 3, 2),

/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13193, 'TotvsGetSalesOrderChangedBatchItems', 'Itens de lote ligados a um pedido de venda (GET SalesOrders/.../ChangedBatchItems)', 0, 2),
(130, 13195, 'TotvsGetSalesOrderInvoices', 'Notas de um pedido de venda (GET SalesOrders/.../Invoices)', 0, 2),
(130, 13196, 'TotvsGetSalesOrderPendingItems', 'Itens pendentes de um pedido de venda (GET SalesOrders/.../PendingItems)', 0, 2),

/*Escritas da API Sales Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:SalesOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4, fase SO-3)*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249)*/
(130, 13201, 'TotvsPostSalesOrderAdditionalOrder', 'Dados adicionais de um pedido de venda (POST SalesOrders/{docCompany}/Additional)', 0, 2),
(130, 13202, 'TotvsPostSalesOrderShippingOrder', 'Transporte e frete de um pedido de venda (POST SalesOrders/{docCompany}/Shipping)', 0, 2),
(130, 13203, 'TotvsPostSalesOrderObservationsOrder', 'Inclusão de observações num pedido de venda (POST SalesOrders/{docCompany}/Observations)', 0, 2),
(130, 13207, 'TotvsPostSalesOrderAdditionalItems', 'Dados adicionais dos itens de um pedido de venda (POST SalesOrders/{docCompany}/Items/Additional)', 0, 2),

/*Escritas de documento da API Sales Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:SalesOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4, fase SO-4)*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249)*/
(130, 13199, 'TotvsPostSalesOrderOrdersChangeStatus', 'Situação de um pedido de venda (POST SalesOrders/{docCompany}/Status)', 0, 2),
(130, 13200, 'TotvsPostSalesOrderHeader', 'Capa de um pedido de venda (POST SalesOrders/{docCompany}/Header)', 0, 2),
(130, 13205, 'TotvsPostSalesOrderItems', 'Inclusão de itens num pedido de venda (POST SalesOrders/{docCompany}/Items)', 0, 2),
(130, 13206, 'TotvsDeleteSalesOrderItems', 'Remoção de um item do pedido de venda (DELETE SalesOrders/{docCompany}/Items)', 0, 2),
(130, 13208, 'TotvsPostSalesOrderQuantityItems', 'Quantidade dos itens do pedido de venda (POST SalesOrders/{docCompany}/Items/Quantity)', 0, 2),
(130, 13211, 'TotvsPostSalesOrderRelationshipCounts', 'Contagens ligadas ao pedido de venda (POST SalesOrders/{docCompany}/Counts)', 0, 2),
(130, 13212, 'TotvsPostSalesOrderRelationshipBatchItems', 'Itens de lote ligados ao pedido de venda (POST SalesOrders/{docCompany}/BatchItems)', 0, 2),
(130, 13213, 'TotvsDeleteSalesOrderBatchItems', 'Remoção do vínculo de item de lote do pedido de venda (DELETE SalesOrders/{docCompany}/BatchItems)', 0, 2),
(130, 13214, 'TotvsPostSalesOrderSuggestBatchItemsNuance', 'Sugestão de itens de lote pelo nuance (POST SalesOrders/{docCompany}/BatchItems/SuggestNuance)', 0, 2),

/*Escritas da API Sales Order com efeito financeiro (pedido B2C, cancelamentos, preço dos itens, transação PDV) - executadas pelo TotvsController,
  não são recorrentes (IntervalTimeInMinutes = 0); só rodam com Totvs:Writes:SalesOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4,
  fase SO-5)*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249)*/
(130, 13197, 'TotvsPostSalesOrderB2COrders', 'Inclusão de pedido de venda B2C (POST SalesOrders/{docCompany}/B2C)', 0, 2),
(130, 13198, 'TotvsPostSalesOrderOrdersCancel', 'Cancelamento de pedido de venda (POST SalesOrders/{docCompany}/Cancel)', 0, 2),
(130, 13204, 'TotvsPostSalesOrderPdvTransactions', 'Transação PDV ligada ao pedido de venda (POST SalesOrders/{docCompany}/PdvTransactions)', 0, 2),
(130, 13209, 'TotvsPostSalesOrderPriceItems', 'Preço dos itens do pedido de venda (POST SalesOrders/{docCompany}/Items/Price)', 0, 2),
(130, 13210, 'TotvsPostSalesOrderCancelItems', 'Cancelamento de quantidades dos itens do pedido de venda (POST SalesOrders/{docCompany}/Items/Cancel)', 0, 2);
