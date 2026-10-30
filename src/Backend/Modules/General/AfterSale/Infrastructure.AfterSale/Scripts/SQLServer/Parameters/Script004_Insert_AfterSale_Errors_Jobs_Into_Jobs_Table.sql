INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 5500 - 5999*/
(50, 5536, 'sp_AfterSaleErrors_Sync', 'Procedure de sincronização entre as tabelas AfterSaleErrors raw e trusted', 3, NULL, 'default', 1);
