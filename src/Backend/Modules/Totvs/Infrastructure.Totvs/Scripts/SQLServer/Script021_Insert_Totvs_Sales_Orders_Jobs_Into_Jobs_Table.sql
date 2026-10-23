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
(130, 13005, 'TotvsPostSearchSalesOrders', 'Integração dos pedidos de venda de todas as lojas (TotvsSaleOrders e filhas), a cada 3 minutos', 3, 2);
