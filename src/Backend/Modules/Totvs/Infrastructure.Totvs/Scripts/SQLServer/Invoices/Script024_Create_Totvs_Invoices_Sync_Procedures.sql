-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Notas fiscais: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As notas nunca são apagadas por este fluxo (nota excluída na Totvs fica com invoiceStatus = Deleted); as filhas que saíram de
--uma nota que veio no lote são apagadas. A condição depende do mesmo integrated_or_updated_at no lote e do merge da nota rodar antes.
declare @delete_by_invoice nvarchar(max) = 'exists (select 1 from [totvs].TotvsInvoices p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.invoiceSequence = target.invoiceSequence and p.invoiceDate = target.invoiceDate and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[totvs].TotvsInvoices') is not null and object_id(N'[totvs].[sp_TotvsInvoices_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoices', @execute = 1;
if object_id(N'[totvs].TotvsInvoicesItems') is not null and object_id(N'[totvs].[sp_TotvsInvoicesItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItems', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesItemsProducts') is not null and object_id(N'[totvs].[sp_TotvsInvoicesItemsProducts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsProducts', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesItemsProductsReferenced') is not null and object_id(N'[totvs].[sp_TotvsInvoicesItemsProductsReferenced_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsProductsReferenced', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesItemsTaxes') is not null and object_id(N'[totvs].[sp_TotvsInvoicesItemsTaxes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsTaxes', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesItemsBarCodes') is not null and object_id(N'[totvs].[sp_TotvsInvoicesItemsBarCodes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsBarCodes', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesPayments') is not null and object_id(N'[totvs].[sp_TotvsInvoicesPayments_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesPayments', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesSalesOrders') is not null and object_id(N'[totvs].[sp_TotvsInvoicesSalesOrders_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesSalesOrders', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesObservationsNF') is not null and object_id(N'[totvs].[sp_TotvsInvoicesObservationsNF_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesObservationsNF', @execute = 1, @delete_condition = @delete_by_invoice;
if object_id(N'[totvs].TotvsInvoicesReferencedTaxes') is not null and object_id(N'[totvs].[sp_TotvsInvoicesReferencedTaxes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesReferencedTaxes', @execute = 1, @delete_condition = @delete_by_invoice;

go

/****** Inutilizações e centros de custo: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.2 e §8.3) ******/
--Nada é apagado por este fluxo (uma inutilização não se desfaz; o centro excluído fica no banco, e o isInactive cobre a
--desativação); a classificação que saiu do centro regravado é apagada.

if object_id(N'[totvs].TotvsInvoicesDisable') is not null and object_id(N'[totvs].[sp_TotvsInvoicesDisable_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesDisable', @execute = 1;
if object_id(N'[totvs].TotvsCostCenters') is not null and object_id(N'[totvs].[sp_TotvsCostCenters_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCostCenters', @execute = 1;
if object_id(N'[totvs].TotvsCostCentersClassifications') is not null and object_id(N'[totvs].[sp_TotvsCostCentersClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCostCentersClassifications', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsCostCenters p where p.doc_company = target.doc_company and p.[code] = target.costCenterCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
