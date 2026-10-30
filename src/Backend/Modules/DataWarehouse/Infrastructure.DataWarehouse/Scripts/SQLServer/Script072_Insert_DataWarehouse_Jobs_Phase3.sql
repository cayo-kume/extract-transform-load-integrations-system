/****** Job da fase 3 em auditing (docs/datawarehouse, 7.5 e 10.2) ******/
--a carga única do legado Linx Microvix (rota RunLinxLegacy do DataWarehouseController), sem agenda própria: roda uma vez
--na fase 3 e de novo só para absorver correções. O mesmo id em Domain.DataWarehouse.Enums.EnumJob.
insert into [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
values
(90, 9004, 'DataWarehouseLinxLegacy', 'Carga única do legado Linx Microvix: landing, snapshot, clientes, dimensões e as linhas Linx dos fatos de venda e pedido', 0, 2);
