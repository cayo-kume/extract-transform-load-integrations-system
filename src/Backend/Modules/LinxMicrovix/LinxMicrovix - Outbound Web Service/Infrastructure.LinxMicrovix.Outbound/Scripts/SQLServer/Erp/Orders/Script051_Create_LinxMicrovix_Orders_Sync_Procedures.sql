-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Pedidos de venda (LinxMicrovixOrdersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxPedidosVenda') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosVenda_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosVenda', @execute = 1;
if object_id(N'[linxmicrovix].LinxPlanosPedidoVenda') is not null and object_id(N'[linxmicrovix].[sp_LinxPlanosPedidoVenda_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPlanosPedidoVenda', @execute = 1;
if object_id(N'[linxmicrovix].LinxPedidosVendaChecklistEntregaLocal') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosVendaChecklistEntregaLocal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosVendaChecklistEntregaLocal', @execute = 1;
if object_id(N'[linxmicrovix].LinxPedidosVendaChecklistEntregaArmazenamento') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosVendaChecklistEntregaArmazenamento_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosVendaChecklistEntregaArmazenamento', @execute = 1;
if object_id(N'[linxmicrovix].LinxPedidosVendaChecklistEntregaDificuldade') is not null and object_id(N'[linxmicrovix].[sp_LinxPedidosVendaChecklistEntregaDificuldade_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPedidosVendaChecklistEntregaDificuldade', @execute = 1;
if object_id(N'[linxmicrovix].LinxOrcamentoPosicao') is not null and object_id(N'[linxmicrovix].[sp_LinxOrcamentoPosicao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrcamentoPosicao', @execute = 1;
if object_id(N'[linxmicrovix].LinxB2CPedidos') is not null and object_id(N'[linxmicrovix].[sp_LinxB2CPedidos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxB2CPedidos', @execute = 1;
if object_id(N'[linxmicrovix].LinxB2CPedidosItens') is not null and object_id(N'[linxmicrovix].[sp_LinxB2CPedidosItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxB2CPedidosItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxB2CPedidosStatus') is not null and object_id(N'[linxmicrovix].[sp_LinxB2CPedidosStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxB2CPedidosStatus', @execute = 1;
if object_id(N'[linxmicrovix].LinxB2CStatus') is not null and object_id(N'[linxmicrovix].[sp_LinxB2CStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxB2CStatus', @execute = 1;
