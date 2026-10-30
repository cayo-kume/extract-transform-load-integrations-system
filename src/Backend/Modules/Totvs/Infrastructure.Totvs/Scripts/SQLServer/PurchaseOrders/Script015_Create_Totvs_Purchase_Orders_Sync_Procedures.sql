/****** Pedidos de compra: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--Os pedidos nunca são apagados por este fluxo; as filhas (e netas) que saíram de um pedido que veio no lote são apagadas.
--A condição depende do pedido e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pedido rodar antes.
declare @delete_by_order nvarchar(max) = 'exists (select 1 from [totvs].TotvsPurchaseOrders p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.orderCode = target.orderCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPurchaseOrders', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPurchaseOrdersProducts', @execute = 1, @delete_condition = @delete_by_order;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPurchaseOrdersRequests', @execute = 1, @delete_condition = @delete_by_order;
