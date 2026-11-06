-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Pedidos de compra (LinxMicrovixPurchaseOrdersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxPedidosCompra') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosCompra_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosCompra', @execute = 1;
if object_id(N'[linxmicrovix].LinxPedidosCompraStatus') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosCompraStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosCompraStatus', @execute = 1;
if object_id(N'[linxmicrovix].LinxPedidosCompraNumeroFornecedor') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosCompraNumeroFornecedor_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosCompraNumeroFornecedor', @execute = 1;
