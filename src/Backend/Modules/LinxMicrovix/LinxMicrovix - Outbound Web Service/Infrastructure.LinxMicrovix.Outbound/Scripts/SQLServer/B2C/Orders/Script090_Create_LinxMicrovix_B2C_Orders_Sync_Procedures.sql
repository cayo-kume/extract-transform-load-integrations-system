-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Pedidos e notas do B2C (LinxMicrovixB2COrdersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaPedidos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPedidos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPedidos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPedidosItens') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPedidosItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPedidosItens', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPedidosPlanos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPedidosPlanos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPedidosPlanos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPedidosStatus') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPedidosStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPedidosStatus', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPedidosIdentificador') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPedidosIdentificador_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPedidosIdentificador', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPedidosTipos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPedidosTipos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPedidosTipos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaStatus') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaStatus', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaCodigoRastreio') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaCodigoRastreio_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaCodigoRastreio', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaNFe') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaNFe_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaNFe', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaNFeSituacao') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaNFeSituacao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaNFeSituacao', @execute = 1;
