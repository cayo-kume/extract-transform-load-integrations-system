-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Produtos e SKUs: procedures de sincronização (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.3) ******/
--Os pais nunca são apagados por este fluxo; as filhas que saíram de um pai que veio no lote são apagadas.
--A condição depende do pai e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pai rodar antes.
declare @delete_by_product nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceProducts p where p.doc_company = target.doc_company and p.ProductID = target.ProductID and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_sku nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceSkus p where p.doc_company = target.doc_company and p.ProductID = target.ProductID and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[linxcommerce].LinxCommerceProducts') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceProducts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProducts', @execute = 1;
if object_id(N'[linxcommerce].LinxCommerceProductsMetadataValues') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceProductsMetadataValues_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProductsMetadataValues', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[linxcommerce].LinxCommerceProductsMedias') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceProductsMedias_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProductsMedias', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[linxcommerce].LinxCommerceProductsMediaAssociations') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceProductsMediaAssociations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProductsMediaAssociations', @execute = 1, @delete_condition = @delete_by_product;
if object_id(N'[linxcommerce].LinxCommerceSkus') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceSkus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSkus', @execute = 1;
if object_id(N'[linxcommerce].LinxCommerceSkusMetadataValues') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceSkusMetadataValues_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSkusMetadataValues', @execute = 1, @delete_condition = @delete_by_sku;
if object_id(N'[linxcommerce].LinxCommerceSkusParentRelations') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceSkusParentRelations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSkusParentRelations', @execute = 1, @delete_condition = @delete_by_sku;
