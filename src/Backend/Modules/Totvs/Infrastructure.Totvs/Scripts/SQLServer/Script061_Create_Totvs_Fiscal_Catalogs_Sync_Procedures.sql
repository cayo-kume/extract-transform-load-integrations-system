/****** Inutilizações e centros de custo: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.2 e §8.3) ******/
--Nada é apagado por este fluxo (uma inutilização não se desfaz; o centro excluído fica no banco, e o isInactive cobre a
--desativação); a classificação que saiu do centro regravado é apagada.

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsInvoicesDisable', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCostCenters', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCostCentersClassifications', @execute = 1, @delete_condition = 'exists (select 1 from [totvs].TotvsCostCenters p where p.doc_company = target.doc_company and p.[code] = target.costCenterCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
