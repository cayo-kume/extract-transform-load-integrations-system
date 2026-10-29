/****** Pedidos e status de pedido: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.3) ******/
--Os pais nunca são apagados por este fluxo; as filhas que saíram de um pai que veio no lote são apagadas.
--A condição depende do pai e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pai rodar antes.
declare @delete_by_order nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceOrders p where p.doc_company = target.doc_company and p.OrderID = target.OrderID and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrders', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersItems', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersTags', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersAddresses', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersPaymentMethods', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersDeliveryMethods', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersDiscounts', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersShipments', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersShipmentsPackages', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersInvoices', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrdersProperties', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceOrderStatus', @execute = 1;
