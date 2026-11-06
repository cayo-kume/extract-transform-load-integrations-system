-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Lojas (LinxMicrovixStoresService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxGrupoLojas') is not null and object_id(N'[linxmicrovix].[sp_LinxGrupoLojas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxGrupoLojas', @execute = 1;
if object_id(N'[linxmicrovix].LinxLojas') is not null and object_id(N'[linxmicrovix].[sp_LinxLojas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLojas', @execute = 1;
if object_id(N'[linxmicrovix].LinxLojasParametros') is not null and object_id(N'[linxmicrovix].[sp_LinxLojasParametros_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLojasParametros', @execute = 1;
if object_id(N'[linxmicrovix].LinxUsuarios') is not null and object_id(N'[linxmicrovix].[sp_LinxUsuarios_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxUsuarios', @execute = 1;
if object_id(N'[linxmicrovix].LinxMetodos') is not null and object_id(N'[linxmicrovix].[sp_LinxMetodos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMetodos', @execute = 1;
if object_id(N'[linxmicrovix].LinxEntrantes') is not null and object_id(N'[linxmicrovix].[sp_LinxEntrantes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxEntrantes', @execute = 1;
