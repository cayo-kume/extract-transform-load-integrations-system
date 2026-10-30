/****** Job da fase 2 em auditing (docs/datawarehouse, 8.5 e 9.4) ******/
--a posição de estoque do fim do mês roda dentro do ciclo diário, no dia 1 (job 9001). Este job é a recarga manual de um fim de
--mês (rota RunMonthlyStock do DataWarehouseController), com log próprio. O mesmo id em Domain.DataWarehouse.Enums.EnumJob.
insert into [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
values
(90, 9003, 'DataWarehouseMonthlyStock', 'Recarga da posição de estoque de um fim de mês (FactMonthlyStock) a partir do histórico de saldos', 43200, 2);
