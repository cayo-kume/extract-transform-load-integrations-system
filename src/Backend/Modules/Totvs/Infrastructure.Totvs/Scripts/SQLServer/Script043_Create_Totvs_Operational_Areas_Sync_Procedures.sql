/****** Regiões de atuação: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §4.1) ******/
--As regiões nunca são apagadas por este fluxo; o estado, CEP ou cidade que saiu de uma região que veio no lote é apagado.
declare @delete_by_area nvarchar(max) = 'exists (select 1 from [totvs].TotvsOperationalAreas p where p.doc_company = target.doc_company and p.operationalAreaCode = target.operationalAreaCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreas', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreasStates', @execute = 1, @delete_condition = @delete_by_area;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreasCeps', @execute = 1, @delete_condition = @delete_by_area;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreasCities', @execute = 1, @delete_condition = @delete_by_area;
