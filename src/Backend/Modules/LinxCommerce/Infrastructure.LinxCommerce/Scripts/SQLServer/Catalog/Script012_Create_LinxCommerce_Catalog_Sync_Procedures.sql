/****** Produtos e SKUs: procedures de sincronização (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.3) ******/
--Os pais nunca são apagados por este fluxo; as filhas que saíram de um pai que veio no lote são apagadas.
--A condição depende do pai e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pai rodar antes.
declare @delete_by_product nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceProducts p where p.doc_company = target.doc_company and p.ProductID = target.ProductID and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_sku nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceSkus p where p.doc_company = target.doc_company and p.ProductID = target.ProductID and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProducts', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProductsMetadataValues', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProductsMedias', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceProductsMediaAssociations', @execute = 1, @delete_condition = @delete_by_product;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSkus', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSkusMetadataValues', @execute = 1, @delete_condition = @delete_by_sku;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSkusParentRelations', @execute = 1, @delete_condition = @delete_by_sku;
