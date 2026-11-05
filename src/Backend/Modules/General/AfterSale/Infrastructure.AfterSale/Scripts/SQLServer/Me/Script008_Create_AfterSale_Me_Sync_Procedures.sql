-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
if object_id(N'[aftersale].AfterSaleEcommerces') is not null and object_id(N'[aftersale].[sp_AfterSaleEcommerces_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleEcommerces', @execute = 1;
if object_id(N'[aftersale].AfterSaleEcommercesMessagesPages') is not null and object_id(N'[aftersale].[sp_AfterSaleEcommercesMessagesPages_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleEcommercesMessagesPages', @execute = 1;
if object_id(N'[aftersale].AfterSaleEcommercesMessagesMails') is not null and object_id(N'[aftersale].[sp_AfterSaleEcommercesMessagesMails_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleEcommercesMessagesMails', @execute = 1;
if object_id(N'[aftersale].AfterSaleEcommercesReasons') is not null and object_id(N'[aftersale].[sp_AfterSaleEcommercesReasons_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleEcommercesReasons', @execute = 1;
if object_id(N'[aftersale].AfterSaleEcommercesSubReasons') is not null and object_id(N'[aftersale].[sp_AfterSaleEcommercesSubReasons_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleEcommercesSubReasons', @execute = 1;
if object_id(N'[aftersale].AfterSaleEcommercesStringValues') is not null and object_id(N'[aftersale].[sp_AfterSaleEcommercesStringValues_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleEcommercesStringValues', @execute = 1;
