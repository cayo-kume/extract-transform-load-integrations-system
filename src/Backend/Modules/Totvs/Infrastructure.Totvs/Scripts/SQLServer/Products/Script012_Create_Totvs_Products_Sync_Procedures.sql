-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Produtos: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As referências nunca são apagadas por este fluxo; filhas, cores e produtos que saíram de uma referência que veio no lote
--são apagados, e as filhas que saíram de um produto que veio no lote também. A condição depende do mesmo
--integrated_or_updated_at no lote inteiro e da ordem dos merges (a do TotvsProductsRepository).
declare @delete_by_reference nvarchar(max) = 'exists (select 1 from [totvs].TotvsSkus p where p.doc_company = target.doc_company and p.branchInfoCode = target.branchInfoCode and p.referenceId = target.referenceId and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_product nvarchar(max) = 'exists (select 1 from [totvs].TotvsSkusColorsProducts p where p.doc_company = target.doc_company and p.branchInfoCode = target.branchInfoCode and p.code = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[totvs].TotvsSkus') is not null and object_id(N'[totvs].[sp_TotvsSkus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkus', @execute = 1;
if object_id(N'[totvs].TotvsSkusGrids') is not null and object_id(N'[totvs].[sp_TotvsSkusGrids_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusGrids', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusObservations') is not null and object_id(N'[totvs].[sp_TotvsSkusObservations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusObservations', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusDetails') is not null and object_id(N'[totvs].[sp_TotvsSkusDetails_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusDetails', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusCompositions') is not null and object_id(N'[totvs].[sp_TotvsSkusCompositions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusCompositions', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusReferenceCategories') is not null and object_id(N'[totvs].[sp_TotvsSkusReferenceCategories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusReferenceCategories', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusIntegrationGroupCodes') is not null and object_id(N'[totvs].[sp_TotvsSkusIntegrationGroupCodes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusIntegrationGroupCodes', @execute = 1, @delete_condition = @delete_by_reference;
--cores e produtos sem exclusão (decisão 34): a API devolve a referência só com os produtos alterados na janela, então "não veio"
--não é "foi excluído"; um produto excluído ou inativado na Totvs fica no banco, como a nota com invoiceStatus = Deleted
if object_id(N'[totvs].TotvsSkusColors') is not null and object_id(N'[totvs].[sp_TotvsSkusColors_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColors', @execute = 1;
if object_id(N'[totvs].TotvsSkusColorsMeasurementsTableOmni') is not null and object_id(N'[totvs].[sp_TotvsSkusColorsMeasurementsTableOmni_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColorsMeasurementsTableOmni', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusColorsMeasurementsTableOmniMedidas') is not null and object_id(N'[totvs].[sp_TotvsSkusColorsMeasurementsTableOmniMedidas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColorsMeasurementsTableOmniMedidas', @execute = 1, @delete_condition = @delete_by_reference;
if object_id(N'[totvs].TotvsSkusColorsProducts') is not null and object_id(N'[totvs].[sp_TotvsSkusColorsProducts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSkusColorsProducts', @execute = 1;
if object_id(N'[totvs].TotvsProductsBarCodes') is not null and object_id(N'[totvs].[sp_TotvsProductsBarCodes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsBarCodes', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsClassifications') is not null and object_id(N'[totvs].[sp_TotvsProductsClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsClassifications', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsAdditionalFields') is not null and object_id(N'[totvs].[sp_TotvsProductsAdditionalFields_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsAdditionalFields', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsSuppliers') is not null and object_id(N'[totvs].[sp_TotvsProductsSuppliers_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsSuppliers', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsManufacturers') is not null and object_id(N'[totvs].[sp_TotvsProductsManufacturers_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsManufacturers', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsReferenceCodeSequences') is not null and object_id(N'[totvs].[sp_TotvsProductsReferenceCodeSequences_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsReferenceCodeSequences', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsWebData') is not null and object_id(N'[totvs].[sp_TotvsProductsWebData_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsWebData', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsWebDetails') is not null and object_id(N'[totvs].[sp_TotvsProductsWebDetails_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsWebDetails', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsBranchesBlocked') is not null and object_id(N'[totvs].[sp_TotvsProductsBranchesBlocked_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsBranchesBlocked', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsConservationInstructions') is not null and object_id(N'[totvs].[sp_TotvsProductsConservationInstructions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsConservationInstructions', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[totvs].TotvsProductsConservationInstructionsItems') is not null and object_id(N'[totvs].[sp_TotvsProductsConservationInstructionsItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsConservationInstructionsItems', @execute = 1, @delete_condition = @delete_by_product;

go

/****** Saldos: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9) ******/
--Nenhum saldo é apagado por este fluxo; o local que saiu do produto regravado é apagado.

if object_id(N'[totvs].TotvsBalances') is not null and object_id(N'[totvs].[sp_TotvsBalances_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBalances', @execute = 1;
if object_id(N'[totvs].TotvsBalancesLocations') is not null and object_id(N'[totvs].[sp_TotvsBalancesLocations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBalancesLocations', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsBalances p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

go

/****** Similares e lotes: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2) ******/
--Produtos e lotes nunca são apagados por este fluxo; o similar, o item e o campo adicional que saíram do registro regravado são apagados.

if object_id(N'[totvs].TotvsProductsSimilar') is not null and object_id(N'[totvs].[sp_TotvsProductsSimilar_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsSimilar', @execute = 1;
if object_id(N'[totvs].TotvsProductsSimilarItems') is not null and object_id(N'[totvs].[sp_TotvsProductsSimilarItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsProductsSimilarItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsProductsSimilar p where p.doc_company = target.doc_company and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsBatches') is not null and object_id(N'[totvs].[sp_TotvsBatches_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBatches', @execute = 1;
if object_id(N'[totvs].TotvsBatchesItems') is not null and object_id(N'[totvs].[sp_TotvsBatchesItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBatchesItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsBatches p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.batchCode = target.batchCode and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsBatchesItemsAdditionalFields') is not null and object_id(N'[totvs].[sp_TotvsBatchesItemsAdditionalFields_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBatchesItemsAdditionalFields', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsBatches p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.batchCode = target.batchCode and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
