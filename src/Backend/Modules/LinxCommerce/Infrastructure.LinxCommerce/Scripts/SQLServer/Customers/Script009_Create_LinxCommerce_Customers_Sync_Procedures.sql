-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Clientes: procedures de sincronização (src/Backend/Modules/LinxCommerce/.docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.3) ******/
--Os pais nunca são apagados por este fluxo; as filhas que saíram de um pai que veio no lote são apagadas.
--A condição depende do pai e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do pai rodar antes.
declare @delete_by_customer nvarchar(max) = 'exists (select 1 from [linxcommerce].LinxCommerceCustomers p where p.doc_company = target.doc_company and p.CustomerID = target.CustomerID and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[linxcommerce].LinxCommerceCustomers') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceCustomers_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceCustomers', @execute = 1;
if object_id(N'[linxcommerce].LinxCommerceCustomersAddresses') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceCustomersAddresses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceCustomersAddresses', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[linxcommerce].LinxCommerceCustomersGroups') is not null and object_id(N'[linxcommerce].[sp_LinxCommerceCustomersGroups_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxcommerce', @table_name = 'LinxCommerceCustomersGroups', @execute = 1, @delete_condition = @delete_by_customer;
