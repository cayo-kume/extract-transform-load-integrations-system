-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
if object_id(N'[aftersale].AfterSaleRefunds') is not null and object_id(N'[aftersale].[sp_AfterSaleRefunds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefunds', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsCouldChangeStatusFor') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsCouldChangeStatusFor_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsCouldChangeStatusFor', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsOrdersPaymentsMethods') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsOrdersPaymentsMethods_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsOrdersPaymentsMethods', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsStatusHistories') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsStatusHistories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsStatusHistories', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsTotalAmountHistories') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsTotalAmountHistories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsTotalAmountHistories', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsProducts') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsProducts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsProducts', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsProductsImages') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsProductsImages_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsProductsImages', @execute = 1;
if object_id(N'[aftersale].AfterSaleRefundsProductsImagesUrls') is not null and object_id(N'[aftersale].[sp_AfterSaleRefundsProductsImagesUrls_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleRefundsProductsImagesUrls', @execute = 1;
if object_id(N'[aftersale].AfterSaleSummarizedRefunds') is not null and object_id(N'[aftersale].[sp_AfterSaleSummarizedRefunds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleSummarizedRefunds', @execute = 1;
if object_id(N'[aftersale].AfterSaleStatusRefunds') is not null and object_id(N'[aftersale].[sp_AfterSaleStatusRefunds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleStatusRefunds', @execute = 1;
if object_id(N'[aftersale].AfterSaleActionsRefunds') is not null and object_id(N'[aftersale].[sp_AfterSaleActionsRefunds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleActionsRefunds', @execute = 1;
if object_id(N'[aftersale].AfterSaleBanksRefunds') is not null and object_id(N'[aftersale].[sp_AfterSaleBanksRefunds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleBanksRefunds', @execute = 1;
if object_id(N'[aftersale].AfterSaleTypesRefunds') is not null and object_id(N'[aftersale].[sp_AfterSaleTypesRefunds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'aftersale', @table_name = 'AfterSaleTypesRefunds', @execute = 1;
