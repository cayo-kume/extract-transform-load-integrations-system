/****** Produtos: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As referências nunca são apagadas por este fluxo; filhas, cores e produtos que saíram de uma referência que veio no lote
--são apagados, e as filhas que saíram de um produto que veio no lote também. A condição depende do mesmo
--integrated_or_updated_at no lote inteiro e da ordem dos merges (a do TotvsProductsRepository).
declare @delete_by_reference nvarchar(max) = 'exists (select 1 from [totvs].TotvsSkus p where p.doc_company = target.doc_company and p.branchInfoCode = target.branchInfoCode and p.referenceId = target.referenceId and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_product nvarchar(max) = 'exists (select 1 from [totvs].TotvsSkusColorsProducts p where p.doc_company = target.doc_company and p.branchInfoCode = target.branchInfoCode and p.code = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkus', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusGrids', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusObservations', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusDetails', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusCompositions', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusReferenceCategories', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusIntegrationGroupCodes', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColors', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColorsMeasurementsTableOmni', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColorsMeasurementsTableOmniMedidas', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColorsProducts', @execute = 1, @delete_condition = @delete_by_reference;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsBarCodes', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsClassifications', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsAdditionalFields', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsSuppliers', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsManufacturers', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsReferenceCodeSequences', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsWebData', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsWebDetails', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsBranchesBlocked', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsConservationInstructions', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsConservationInstructionsItems', @execute = 1, @delete_condition = @delete_by_product;
