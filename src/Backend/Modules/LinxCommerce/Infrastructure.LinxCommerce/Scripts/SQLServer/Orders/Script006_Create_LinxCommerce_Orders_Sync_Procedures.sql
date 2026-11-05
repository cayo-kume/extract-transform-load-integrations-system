-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Pedidos e status de pedido: procedures de sincronização (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.3) ******/
--Os pais nunca são apagados por este fluxo; as filhas que saíram de um pai que veio no lote são apagadas.
--A condição depende do pai e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pai rodar antes.
declare @delete_by_order nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceOrders p where p.doc_company = target.doc_company and p.OrderID = target.OrderID and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[linxcommerce].LinxCommerceOrders') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrders_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrders', @execute = 1;
if object_id(N'[linxcommerce].LinxCommerceOrdersItems') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersItems', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersTags') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersTags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersTags', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersAddresses') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersAddresses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersAddresses', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersPaymentMethods') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersPaymentMethods_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersPaymentMethods', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersDeliveryMethods') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersDeliveryMethods_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersDeliveryMethods', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersDiscounts') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersDiscounts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersDiscounts', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersShipments') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersShipments_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersShipments', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersShipmentsPackages') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersShipmentsPackages_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersShipmentsPackages', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersInvoices') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersInvoices_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersInvoices', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrdersProperties') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrdersProperties_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersProperties', @execute = 1, @delete_condition = @delete_by_order;
if object_id(N'[linxcommerce].LinxCommerceOrderStatus') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceOrderStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrderStatus', @execute = 1;
