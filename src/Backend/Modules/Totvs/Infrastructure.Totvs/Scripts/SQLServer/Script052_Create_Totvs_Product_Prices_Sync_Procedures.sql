/****** Tabelas de preço (itens) e promoções: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2) ******/
--Itens e promoções nunca são apagados por este fluxo (o item que sai da promoção fica no banco com a última situação); o preço
--por loja e escala que saiu do produto regravado é apagado.

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesItems', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPriceTablesItemsPrices', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsPriceTablesItems p where p.doc_company = target.doc_company and p.priceTableCode = target.priceTableCode and p.productCode = target.productCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPromotions', @execute = 1;
