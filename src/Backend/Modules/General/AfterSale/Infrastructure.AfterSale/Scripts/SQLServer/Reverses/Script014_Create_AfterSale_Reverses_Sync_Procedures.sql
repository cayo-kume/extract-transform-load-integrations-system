-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
if object_id(N'[aftersale].AfterSaleCourierAttributes') is not null and object_id(N'[aftersale].[sp_AfterSaleCourierAttributes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleCourierAttributes', @execute = 1;
if object_id(N'[aftersale].AfterSaleCourierAttributesFields') is not null and object_id(N'[aftersale].[sp_AfterSaleCourierAttributesFields_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleCourierAttributesFields', @execute = 1;
if object_id(N'[aftersale].AfterSaleTransportationsReverses') is not null and object_id(N'[aftersale].[sp_AfterSaleTransportationsReverses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleTransportationsReverses', @execute = 1;
if object_id(N'[aftersale].AfterSaleHistoriesReverses') is not null and object_id(N'[aftersale].[sp_AfterSaleHistoriesReverses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleHistoriesReverses', @execute = 1;
if object_id(N'[aftersale].AfterSaleStatusReverses') is not null and object_id(N'[aftersale].[sp_AfterSaleStatusReverses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleStatusReverses', @execute = 1;
if object_id(N'[aftersale].AfterSaleSummarizedReverses') is not null and object_id(N'[aftersale].[sp_AfterSaleSummarizedReverses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleSummarizedReverses', @execute = 1;
if object_id(N'[aftersale].AfterSaleReverses') is not null and object_id(N'[aftersale].[sp_AfterSaleReverses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReverses', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesCustomers') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesCustomers_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesCustomers', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesDestinationsData') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesDestinationsData_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesDestinationsData', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesTrackings') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesTrackings_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesTrackings', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesTrackingHistories') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesTrackingHistories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesTrackingHistories', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesDestinations') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesDestinations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesDestinations', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesEcommerceOrders') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesEcommerceOrders_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesEcommerceOrders', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesEcommerceOrdersPaymentsMethods') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesEcommerceOrdersPaymentsMethods_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesEcommerceOrdersPaymentsMethods', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesProducts') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesProducts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesProducts', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesProductsImages') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesProductsImages_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesProductsImages', @execute = 1;
if object_id(N'[aftersale].AfterSaleReversesStatusHistories') is not null and object_id(N'[aftersale].[sp_AfterSaleReversesStatusHistories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleReversesStatusHistories', @execute = 1;
