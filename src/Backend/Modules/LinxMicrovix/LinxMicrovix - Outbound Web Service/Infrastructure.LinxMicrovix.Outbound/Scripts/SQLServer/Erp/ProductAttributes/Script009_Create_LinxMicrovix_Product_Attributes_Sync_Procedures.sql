-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Atributos de produto (LinxMicrovixProductAttributesService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxSetores') is not null and object_id(N'[linxmicrovix].[sp_LinxSetores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSetores', @execute = 1;
if object_id(N'[linxmicrovix].LinxLinhas') is not null and object_id(N'[linxmicrovix].[sp_LinxLinhas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLinhas', @execute = 1;
if object_id(N'[linxmicrovix].LinxMarcas') is not null and object_id(N'[linxmicrovix].[sp_LinxMarcas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMarcas', @execute = 1;
if object_id(N'[linxmicrovix].LinxColecoes') is not null and object_id(N'[linxmicrovix].[sp_LinxColecoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxColecoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxTamanhos') is not null and object_id(N'[linxmicrovix].[sp_LinxTamanhos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxTamanhos', @execute = 1;
if object_id(N'[linxmicrovix].LinxCores') is not null and object_id(N'[linxmicrovix].[sp_LinxCores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCores', @execute = 1;
if object_id(N'[linxmicrovix].LinxEspessuras') is not null and object_id(N'[linxmicrovix].[sp_LinxEspessuras_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxEspessuras', @execute = 1;
if object_id(N'[linxmicrovix].LinxUnidades') is not null and object_id(N'[linxmicrovix].[sp_LinxUnidades_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxUnidades', @execute = 1;
if object_id(N'[linxmicrovix].LinxClassificacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxClassificacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClassificacoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxSubClasses') is not null and object_id(N'[linxmicrovix].[sp_LinxSubClasses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSubClasses', @execute = 1;
