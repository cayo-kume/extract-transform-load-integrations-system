/****** Contas a pagar: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--Duplicatas e comissões nunca são apagadas por este fluxo; as filhas que saíram de um registro que veio no lote são apagadas.
--A condição depende do registro e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge do registro rodar antes.
declare @delete_by_duplicate nvarchar(max) = 'exists (select 1 from [totvs].TotvsDuplicates p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.supplierCode = target.supplierCode and p.duplicateCode = target.duplicateCode and p.installmentCode = target.installmentCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_commissionpaid nvarchar(max) = 'exists (select 1 from [totvs].TotvsCommissionsPaid p where p.doc_company = target.doc_company and p.closingBranchCode = target.closingBranchCode and p.closingCode = target.closingCode and p.commissionedCpfCnpj = target.commissionedCpfCnpj and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsDuplicates', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsDuplicatesExpenses', @execute = 1, @delete_condition = @delete_by_duplicate;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsDuplicatesClassifications', @execute = 1, @delete_condition = @delete_by_duplicate;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCommissionsPaid', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsCommissionsPaidAccountMovements', @execute = 1, @delete_condition = @delete_by_commissionpaid;
