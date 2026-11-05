-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Documentos fiscais (LinxMicrovixFiscalDocumentsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxXMLDocumentos') is not null and object_id(N'[linxmicrovix].[sp_LinxXMLDocumentos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxXMLDocumentos', @execute = 1;
if object_id(N'[linxmicrovix].LinxNFeEvento') is not null and object_id(N'[linxmicrovix].[sp_LinxNFeEvento_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxNFeEvento', @execute = 1;
if object_id(N'[linxmicrovix].LinxNfse') is not null and object_id(N'[linxmicrovix].[sp_LinxNfse_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxNfse', @execute = 1;
if object_id(N'[linxmicrovix].LinxEntradaNFe') is not null and object_id(N'[linxmicrovix].[sp_LinxEntradaNFe_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxEntradaNFe', @execute = 1;
if object_id(N'[linxmicrovix].LinxEntradaNFeProdutos') is not null and object_id(N'[linxmicrovix].[sp_LinxEntradaNFeProdutos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxEntradaNFeProdutos', @execute = 1;
if object_id(N'[linxmicrovix].LinxConhecimentoFretes') is not null and object_id(N'[linxmicrovix].[sp_LinxConhecimentoFretes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxConhecimentoFretes', @execute = 1;
if object_id(N'[linxmicrovix].LinxFreteTipos') is not null and object_id(N'[linxmicrovix].[sp_LinxFreteTipos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxFreteTipos', @execute = 1;
if object_id(N'[linxmicrovix].LinxReducoesZ') is not null and object_id(N'[linxmicrovix].[sp_LinxReducoesZ_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxReducoesZ', @execute = 1;
if object_id(N'[linxmicrovix].LinxECF') is not null and object_id(N'[linxmicrovix].[sp_LinxECF_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxECF', @execute = 1;
if object_id(N'[linxmicrovix].LinxECFFormasPagamento') is not null and object_id(N'[linxmicrovix].[sp_LinxECFFormasPagamento_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxECFFormasPagamento', @execute = 1;
if object_id(N'[linxmicrovix].LinxNfceEstacao') is not null and object_id(N'[linxmicrovix].[sp_LinxNfceEstacao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxNfceEstacao', @execute = 1;
if object_id(N'[linxmicrovix].LinxSeries') is not null and object_id(N'[linxmicrovix].[sp_LinxSeries_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSeries', @execute = 1;
