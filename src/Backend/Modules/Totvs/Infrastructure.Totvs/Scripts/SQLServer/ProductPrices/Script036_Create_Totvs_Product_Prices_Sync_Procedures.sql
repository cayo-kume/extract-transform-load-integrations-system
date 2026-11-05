-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Preços e custos: procedures de sincronização (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9) ******/
--Nenhum valor é apagado por este fluxo; a outra promoção que saiu do produto regravado é apagada.

if object_id(N'[totvs].TotvsPrices') is not null and object_id(N'[totvs].[sp_TotvsPrices_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPrices', @execute = 1;
if object_id(N'[totvs].TotvsPricesOtherPromotions') is not null and object_id(N'[totvs].[sp_TotvsPricesOtherPromotions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPricesOtherPromotions', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPrices p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.productCode = target.productCode and p.priceCode = target.priceCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsCosts') is not null and object_id(N'[totvs].[sp_TotvsCosts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCosts', @execute = 1;

go

/****** Tabelas de preço (itens) e promoções: procedures de sincronização (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2) ******/
--Itens e promoções nunca são apagados por este fluxo (o item que sai da promoção fica no banco com a última situação); o preço
--por loja e escala que saiu do produto regravado é apagado.

if object_id(N'[totvs].TotvsPriceTablesItems') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesItems', @execute = 1;
if object_id(N'[totvs].TotvsPriceTablesItemsPrices') is not null and object_id(N'[totvs].[sp_TotvsPriceTablesItemsPrices_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesItemsPrices', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTablesItems p where p.doc_company = target.doc_company and p.priceTableCode = target.priceTableCode and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
if object_id(N'[totvs].TotvsPromotions') is not null and object_id(N'[totvs].[sp_TotvsPromotions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPromotions', @execute = 1;
