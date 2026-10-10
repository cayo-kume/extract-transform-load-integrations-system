--INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
--VALUES
/*Procedures de Sincronizações*/
/*Range 11500 - 11999*/
--(, 11500,'P_StoneLogisticaAddresses_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaAddresses raw e trusted', 3, 1),
--(, 11501,'P_StoneLogisticaCustomers_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaCustomers raw e trusted', 3, 1),
--(, 11502,'P_StoneLogisticaEvents_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaEvents raw e trusted', 3, 1),
--(, 11503,'P_StoneLogisticaItems_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaItems raw e trusted', 3, 1),
--(, 11504,'P_StoneLogisticaOrders_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaOrders raw e trusted', 3, 1),
--(, 11505,'P_StoneLogisticaOwner_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaOwner raw e trusted', 3, 1),
--(, 11506,'P_StoneLogisticaSenders_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaSenders raw e trusted', 3, 1),
--(, 11507,'P_StoneLogisticaErrors_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneLogisticaErrors raw e trusted', 3, 1),
--(, 11508,'P_StoneOrdersShipped_Sincronizacao', 'Procedure de sicronização entre as tabelas StoneOrdersShipped raw e trusted', 3, 1),

/*Integrações*/
/*Range 11000 - 11499*/
--(110, 11000, 'StoneLogisticaTrackingHistory', 'Integração de dados da tabela StoneLogisticaOrders', 3, 2),
--(110, 11001, 'StoneLogisticaSendOrders', 'Integração de dados da tabela StoneLogisticaOrders', 3, 2),
--(110, 11002, 'StoneLogisticaGetZplLabels', 'Integração de dados da tabela IT4_WMS_DOCUMENTO_ZPL', 3, 2),
--(110, 11003, 'StoneLogisticaCancelOrders', 'Cancelamento de Pedidos Enviados Para Stone', 3, 2),