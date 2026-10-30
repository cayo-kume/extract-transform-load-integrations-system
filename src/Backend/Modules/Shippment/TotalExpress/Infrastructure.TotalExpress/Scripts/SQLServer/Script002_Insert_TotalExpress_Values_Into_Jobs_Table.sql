--INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
--VALUES
/*Procedures de Sincronizações*/
/*Range 12500 - 12999*/
--(12000, 12500,'P_TotalExpressEventos_Sincronizacao', 'Procedure de sicronização entre as tabelas TotalExpressEventos raw e trusted', 3, NULL, 'default', 1),
--(12000, 12501,'P_TotalExpressTrackings_Sincronizacao', 'Procedure de sicronização entre as tabelas TotalExpressTrackings raw e trusted', 3, NULL, 'default', 1),
--(12001, 12502,'P_TotalExpressDocumentoFiscais_Sincronizacao', 'Procedure de sicronização entre as tabelas TotalExpressDocumentoFiscais raw e trusted', 3, NULL, 'default', 1),
--(12001, 12503,'P_TotalExpressPedidos_Sincronizacao', 'Procedure de sicronização entre as tabelas TotalExpressPedidos raw e trusted', 3, NULL, 'default', 1),
--(12001, 12504,'P_TotalExpressVolumes_Sincronizacao', 'Procedure de sicronização entre as tabelas TotalExpressVolumes raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 12000 - 12499*/
--(120, 12000, 'TotalExpressTrackingHistory', 'Integração de dados da tabela TotalExpressTrackings', 3, NULL, 'default', 2),
--(120, 12001, 'TotalExpressSendOrders', 'Integração de dados da tabela TotalExpressPedidos', 3, NULL, 'default', 2)
