/****** Filiais: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As filiais nunca são apagadas por este fluxo; os endereços removidos de uma filial que veio no lote são apagados.
--A condição depende da filial e dos endereços do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de TotvsBranches rodar antes.
declare @delete_by_branch nvarchar(max) = 'exists (select 1 from [totvs].TotvsBranches p where p.doc_company = target.doc_company and p.code = target.branchCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranches', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranchesAddresses', @execute = 1, @delete_condition = @delete_by_branch;
