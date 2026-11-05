-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Tabelas fiscais (LinxMicrovixTaxesService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxNaturezaOperacao') is not null and object_id(N'[linxmicrovix].[sp_LinxNaturezaOperacao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxNaturezaOperacao', @execute = 1;
if object_id(N'[linxmicrovix].LinxCfopFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxCfopFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCfopFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxCstIcmsFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxCstIcmsFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCstIcmsFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxCstIpiFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxCstIpiFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCstIpiFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxCstPisFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxCstPisFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCstPisFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxCstCofinsFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxCstCofinsFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCstCofinsFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxCsosnFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxCsosnFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCsosnFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxClasseFiscal') is not null and object_id(N'[linxmicrovix].[sp_LinxClasseFiscal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClasseFiscal', @execute = 1;
if object_id(N'[linxmicrovix].LinxCest') is not null and object_id(N'[linxmicrovix].[sp_LinxCest_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCest', @execute = 1;
if object_id(N'[linxmicrovix].LinxNcm') is not null and object_id(N'[linxmicrovix].[sp_LinxNcm_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxNcm', @execute = 1;
if object_id(N'[linxmicrovix].LinxEnquadramentoIpi') is not null and object_id(N'[linxmicrovix].[sp_LinxEnquadramentoIpi_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxEnquadramentoIpi', @execute = 1;
if object_id(N'[linxmicrovix].LinxMotivosDesoneracaoIcms') is not null and object_id(N'[linxmicrovix].[sp_LinxMotivosDesoneracaoIcms_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMotivosDesoneracaoIcms', @execute = 1;
if object_id(N'[linxmicrovix].LinxSpedTipoItem') is not null and object_id(N'[linxmicrovix].[sp_LinxSpedTipoItem_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSpedTipoItem', @execute = 1;
if object_id(N'[linxmicrovix].LinxSpedTipoBaseCredito') is not null and object_id(N'[linxmicrovix].[sp_LinxSpedTipoBaseCredito_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSpedTipoBaseCredito', @execute = 1;
if object_id(N'[linxmicrovix].LinxRotinaOrigem') is not null and object_id(N'[linxmicrovix].[sp_LinxRotinaOrigem_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRotinaOrigem', @execute = 1;
if object_id(N'[linxmicrovix].LinxNfeIbgeMunicipio') is not null and object_id(N'[linxmicrovix].[sp_LinxNfeIbgeMunicipio_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxNfeIbgeMunicipio', @execute = 1;
