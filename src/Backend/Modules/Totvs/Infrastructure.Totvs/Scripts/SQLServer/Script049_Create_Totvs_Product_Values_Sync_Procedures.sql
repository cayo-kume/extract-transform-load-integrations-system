/****** Preços, custos e saldos: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9) ******/
--Nenhum valor é apagado por este fluxo; a outra promoção e o local que saíram do produto regravado são apagados.

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPrices', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPricesOtherPromotions', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPrices p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.productCode = target.productCode and p.priceCode = target.priceCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCosts', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBalances', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBalancesLocations', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsBalances p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
