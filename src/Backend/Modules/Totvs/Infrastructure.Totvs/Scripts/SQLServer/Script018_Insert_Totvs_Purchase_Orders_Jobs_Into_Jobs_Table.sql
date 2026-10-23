INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (pedidos de compra: 13556 - 13558)*/
(13004, 13556, 'sp_TotvsPurchaseOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrders raw e trusted', 3, 1),
(13004, 13557, 'sp_TotvsPurchaseOrdersProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrdersProducts raw e trusted', 3, 1),
(13004, 13558, 'sp_TotvsPurchaseOrdersRequests_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrdersRequests raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13004, 'TotvsPostSearchPurchaseOrders', 'Integração dos pedidos de compra de todas as lojas (TotvsPurchaseOrders e filhas), a cada 3 minutos', 3, 2);
