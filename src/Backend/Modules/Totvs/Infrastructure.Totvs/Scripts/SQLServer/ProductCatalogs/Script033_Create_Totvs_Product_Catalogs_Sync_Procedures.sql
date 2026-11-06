-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Catálogos da API Product: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.1) ******/
--Os catálogos nunca são apagados por este fluxo (um item removido na Totvs fica no banco, como as filiais); a filha que saiu de
--um catálogo que veio no lote é apagada.

if object_id(N'[totvs].TotvsClassificationTypes') is not null and object_id(N'[totvs].[sp_TotvsClassificationTypes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsClassificationTypes', @execute = 1;
if object_id(N'[totvs].TotvsProductClassifications') is not null and object_id(N'[totvs].[sp_TotvsProductClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductClassifications', @execute = 1;
if object_id(N'[totvs].TotvsCategories') is not null and object_id(N'[totvs].[sp_TotvsCategories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCategories', @execute = 1;
if object_id(N'[totvs].TotvsGrids') is not null and object_id(N'[totvs].[sp_TotvsGrids_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsGrids', @execute = 1;
if object_id(N'[totvs].TotvsGridsSizes') is not null and object_id(N'[totvs].[sp_TotvsGridsSizes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsGridsSizes', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsGrids p where p.doc_company = target.doc_company and p.[code] = target.gridCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsMeasurementUnits') is not null and object_id(N'[totvs].[sp_TotvsMeasurementUnits_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsMeasurementUnits', @execute = 1;
if object_id(N'[totvs].TotvsMeasurementUnitsVariations') is not null and object_id(N'[totvs].[sp_TotvsMeasurementUnitsVariations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsMeasurementUnitsVariations', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsMeasurementUnits p where p.doc_company = target.doc_company and p.[code] = target.measurementUnitCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsColors') is not null and object_id(N'[totvs].[sp_TotvsColors_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsColors', @execute = 1;
if object_id(N'[totvs].TotvsColorsAdditionalInformation') is not null and object_id(N'[totvs].[sp_TotvsColorsAdditionalInformation_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsColorsAdditionalInformation', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsColors p where p.doc_company = target.doc_company and p.colorCode = target.colorCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsAdditionalFieldsTypes') is not null and object_id(N'[totvs].[sp_TotvsAdditionalFieldsTypes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsAdditionalFieldsTypes', @execute = 1;
if object_id(N'[totvs].TotvsProductGrouperConfigurations') is not null and object_id(N'[totvs].[sp_TotvsProductGrouperConfigurations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductGrouperConfigurations', @execute = 1;
if object_id(N'[totvs].TotvsProductGrouperConfigurationsItems') is not null and object_id(N'[totvs].[sp_TotvsProductGrouperConfigurationsItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductGrouperConfigurationsItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsProductGrouperConfigurations p where p.doc_company = target.doc_company and p.grouperCode = target.grouperCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsInstructionItems') is not null and object_id(N'[totvs].[sp_TotvsInstructionItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInstructionItems', @execute = 1;
if object_id(N'[totvs].TotvsCompositions') is not null and object_id(N'[totvs].[sp_TotvsCompositions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCompositions', @execute = 1;
if object_id(N'[totvs].TotvsCompositionsFibers') is not null and object_id(N'[totvs].[sp_TotvsCompositionsFibers_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCompositionsFibers', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsCompositions p where p.doc_company = target.doc_company and p.[code] = target.compositionCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPriceTables') is not null and object_id(N'[totvs].[sp_TotvsPriceTables_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTables', @execute = 1;
if object_id(N'[totvs].TotvsPriceTablesSalesOrderClassifications') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesSalesOrderClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesSalesOrderClassifications', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPriceTablesPersonClassifications') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesPersonClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesPersonClassifications', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPriceTablesPaymentConditions') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesPaymentConditions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesPaymentConditions', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPriceTablesAveragePeriods') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesAveragePeriods_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesAveragePeriods', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPriceTablesAveragePeriodQuantities') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesAveragePeriodQuantities_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesAveragePeriodQuantities', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPriceTableScales') is not null and object_id(N'[totvs].[sp_TotvsPriceTableScales_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTableScales', @execute = 1;
if object_id(N'[totvs].TotvsPriceTableScalesItems') is not null and object_id(N'[totvs].[sp_TotvsPriceTableScalesItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTableScalesItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTableScales p where p.doc_company = target.doc_company and p.scaleCode = target.scaleCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
