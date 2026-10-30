--INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
--VALUES
/*Procedures de Sincronizações*/
/*Range 10500 - 19999*/
--(, 10500,'P_JadLogTracking_Sincronizacao', 'Procedure de sincronização entre as tabelas JadLogTracking raw e trusted', 3, NULL, 'default', 1),
--(, 10501,'P_JadLogEventos_Sincronizacao', 'Procedure de sincronização entre as tabelas JadLogEventos raw e trusted', 3, NULL, 'default', 1),
--(, 10502,'P_JadLogVolumes_Sincronizacao', 'Procedure de sincronização entre as tabelas JadLogVolumes raw e trusted', 3, NULL, 'default', 1),
--(, 10503,'P_JadLogErros_Sincronizacao', 'Procedure de sicronização entre as tabelas JadLogErros raw e trusted', 3, NULL, 'default', 1),
--(, 10504,'P_JadLogPedidos_Sincronizacao', 'Procedure de sicronização entre as tabelas JadLogPedidos raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 10000 - 10499*/
--(60, 10000, 'JadlogTrackingHistory', 'Integração de dados da tabela JadLogTrackingHistory', 3, NULL, 'default', 2),
--(60, 10001, 'JadlogSendOrders', 'Integração de dados da tabela JadLogPedidos', 3, NULL, 'default', 2),
