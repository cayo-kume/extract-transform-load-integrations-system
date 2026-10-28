INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
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
(130, 13194, 'TotvsGetSalesOrderBillingSuggestions', 'Integração das sugestões de faturamento, com pedidos e itens (TotvsBillingSuggestions e filhas), a cada 3 minutos', 3, 2);
