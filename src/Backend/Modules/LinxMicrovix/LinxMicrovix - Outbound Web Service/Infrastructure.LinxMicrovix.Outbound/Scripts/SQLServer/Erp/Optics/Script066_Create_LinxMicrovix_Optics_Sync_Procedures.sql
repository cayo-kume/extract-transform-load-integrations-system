-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Ótica (LinxMicrovixOpticsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxProdutosOpticosTipo') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosOpticosTipo_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosOpticosTipo', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosOpticosTipoAro') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosOpticosTipoAro_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosOpticosTipoAro', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosOpticosFormatoAro') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosOpticosFormatoAro_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosOpticosFormatoAro', @execute = 1;
if object_id(N'[linxmicrovix].LinxDadosOpticosDav') is not null and object_id(N'[linxmicrovix].[sp_LinxDadosOpticosDav_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDadosOpticosDav', @execute = 1;
if object_id(N'[linxmicrovix].LinxOticoPrismaDescricao') is not null and object_id(N'[linxmicrovix].[sp_LinxOticoPrismaDescricao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOticoPrismaDescricao', @execute = 1;
if object_id(N'[linxmicrovix].LinxOticoReceitas') is not null and object_id(N'[linxmicrovix].[sp_LinxOticoReceitas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOticoReceitas', @execute = 1;
if object_id(N'[linxmicrovix].LinxPosicaoOsRamoOptico') is not null and object_id(N'[linxmicrovix].[sp_LinxPosicaoOsRamoOptico_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPosicaoOsRamoOptico', @execute = 1;
if object_id(N'[linxmicrovix].LinxOrdensServicoPosicaoOsRamoOpticoHistorico') is not null and object_id(N'[linxmicrovix].[sp_LinxOrdensServicoPosicaoOsRamoOpticoHistorico_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdensServicoPosicaoOsRamoOpticoHistorico', @execute = 1;
if object_id(N'[linxmicrovix].LinxOrcamentoComponenteFormula') is not null and object_id(N'[linxmicrovix].[sp_LinxOrcamentoComponenteFormula_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrcamentoComponenteFormula', @execute = 1;
if object_id(N'[linxmicrovix].LinxComponenteFormulaDav') is not null and object_id(N'[linxmicrovix].[sp_LinxComponenteFormulaDav_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxComponenteFormulaDav', @execute = 1;
