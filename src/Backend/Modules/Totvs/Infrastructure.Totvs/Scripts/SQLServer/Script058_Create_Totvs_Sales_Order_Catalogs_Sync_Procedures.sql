/****** Catálogos e sugestões de faturamento da API Sales Order: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7) ******/
--Catálogos e sugestões nunca são apagados por este fluxo; o pedido e o item que saíram da sugestão regravada são apagados.

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleDiscountTypes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleClassifications', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSaleOrderBatches', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBillingSuggestions', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBillingSuggestionsOrders', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsBillingSuggestions p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.suggestionCode = target.suggestionCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBillingSuggestionsOrdersItems', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsBillingSuggestions p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.suggestionCode = target.suggestionCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
