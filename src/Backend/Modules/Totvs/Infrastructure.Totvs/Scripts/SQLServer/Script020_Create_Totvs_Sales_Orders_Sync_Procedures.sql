/****** Pedidos de venda: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--Os pedidos nunca são apagados por este fluxo; as filhas (e netas) que saíram de um pedido que veio no lote são apagadas.
--A condição depende do pedido e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pedido rodar antes.
declare @delete_by_order nvarchar(max) = 'exists (select 1 from [totvs].TotvsSaleOrders p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.orderCode = target.orderCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrders', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersProducts', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersProductsBatchItems', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersInvoices', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersObservations', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersInvoiceObservations', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersRepresentativeObservations', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersClassifications', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersDiscounts', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersCommissioneds', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrdersCounts', @execute = 1, @delete_condition = @delete_by_order;
