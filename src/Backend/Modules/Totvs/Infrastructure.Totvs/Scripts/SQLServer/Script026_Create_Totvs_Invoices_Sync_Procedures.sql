/****** Notas fiscais: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As notas nunca são apagadas por este fluxo (nota excluída na Totvs fica com invoiceStatus = Deleted); as filhas que saíram de
--uma nota que veio no lote são apagadas. A condição depende do mesmo integrated_or_updated_at no lote e do merge da nota rodar antes.
declare @delete_by_invoice nvarchar(max) = 'exists (select 1 from [totvs].TotvsInvoices p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.invoiceSequence = target.invoiceSequence and p.invoiceDate = target.invoiceDate and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoices', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItems', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsProducts', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsProductsReferenced', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsTaxes', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesItemsBarCodes', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesPayments', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesSalesOrders', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesObservationsNF', @execute = 1, @delete_condition = @delete_by_invoice;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesReferencedTaxes', @execute = 1, @delete_condition = @delete_by_invoice;
