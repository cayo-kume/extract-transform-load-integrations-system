/****** Vendedores: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.3) ******/
--Os pais nunca são apagados por este fluxo; as filhas que saíram de um pai que veio no lote são apagadas.
--A condição depende do pai e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pai rodar antes.
declare @delete_by_salesrepresentative nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceSalesRepresentatives p where p.doc_company = target.doc_company and p.SalesRepresentativeID = target.SalesRepresentativeID and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSalesRepresentatives', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSalesRepresentativesAddresses', @execute = 1, @delete_condition = @delete_by_salesrepresentative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceSalesRepresentativesCustomers', @execute = 1, @delete_condition = @delete_by_salesrepresentative;
