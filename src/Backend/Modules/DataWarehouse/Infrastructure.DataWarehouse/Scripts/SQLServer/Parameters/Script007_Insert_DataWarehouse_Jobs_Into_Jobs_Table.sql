/****** App e jobs do DW em auditing (docs/datawarehouse, 9.4 e 10.1) ******/
--app 90, faixa 9000 - 9999. Os mesmos ids em Domain.DataWarehouse.Enums.EnumJob. Os intervalos são ajustáveis por update,
--sem mudar o código; quem dispara é o agendador (ou a rota do DataWarehouseController, até ele existir). IdType 2 (integração),
--o mesmo que o LoggerService grava em auditing.Logs (a FK de Logs cobre IdJob e IdType).
insert into [auditing].[Apps] ([IdParent], [Title], [Description])
values
(90, 'DataWarehouse', 'Data warehouse do NEWBLOOMERS: landing, histórico, dimensões, fatos e agregados');

insert into [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
values
(90, 9000, 'DataWarehouseHourly', 'Carga do DW a cada hora: landing, snapshots de notas e pedidos, dimensões, fatos, agregados e checagens', 60, 2),
(90, 9001, 'DataWarehouseDaily', 'Carga do DW diária: a horária mais os snapshots de cadastros, custos e preços e a detecção de exclusões', 1440, 2),
(90, 9002, 'DataWarehouseShortCycle', 'Ciclo curto da logística: landing de pedidos, notas e clientes e o endereço de entrega', 3, 2);

/****** Job da fase 2 em auditing (docs/datawarehouse, 8.5 e 9.4) ******/
--a posição de estoque do fim do mês roda dentro do ciclo diário, no dia 1 (job 9001). Este job é a recarga manual de um fim de
--mês (rota RunMonthlyStock do DataWarehouseController), com log próprio. O mesmo id em Domain.DataWarehouse.Enums.EnumJob.
insert into [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
values
(90, 9003, 'DataWarehouseMonthlyStock', 'Recarga da posição de estoque de um fim de mês (FactMonthlyStock) a partir do histórico de saldos', 43200, 2);

/****** Job da fase 3 em auditing (docs/datawarehouse, 7.5 e 10.2) ******/
--a carga única do legado Linx Microvix (rota RunLinxLegacy do DataWarehouseController), sem agenda própria: roda uma vez
--na fase 3 e de novo só para absorver correções. O mesmo id em Domain.DataWarehouse.Enums.EnumJob.
insert into [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
values
(90, 9004, 'DataWarehouseLinxLegacy', 'Carga única do legado Linx Microvix: landing, snapshot, clientes, dimensões e as linhas Linx dos fatos de venda e pedido', 0, 2);
