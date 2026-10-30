/****** Catálogos da API Product: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.1) ******/
--Os catálogos nunca são apagados por este fluxo (um item removido na Totvs fica no banco, como as filiais); a filha que saiu de
--um catálogo que veio no lote é apagada.

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsClassificationTypes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductClassifications', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCategories', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsGrids', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsGridsSizes', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsGrids p where p.doc_company = target.doc_company and p.[code] = target.gridCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsMeasurementUnits', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsMeasurementUnitsVariations', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsMeasurementUnits p where p.doc_company = target.doc_company and p.[code] = target.measurementUnitCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsColors', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsColorsAdditionalInformation', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsColors p where p.doc_company = target.doc_company and p.colorCode = target.colorCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsAdditionalFieldsTypes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductGrouperConfigurations', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductGrouperConfigurationsItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsProductGrouperConfigurations p where p.doc_company = target.doc_company and p.grouperCode = target.grouperCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInstructionItems', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCompositions', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCompositionsFibers', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsCompositions p where p.doc_company = target.doc_company and p.[code] = target.compositionCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTables', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesSalesOrderClassifications', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesPersonClassifications', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesPaymentConditions', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesAveragePeriods', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesAveragePeriodQuantities', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTables p where p.doc_company = target.doc_company and p.[code] = target.priceTableCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTableScales', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTableScalesItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTableScales p where p.doc_company = target.doc_company and p.scaleCode = target.scaleCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
