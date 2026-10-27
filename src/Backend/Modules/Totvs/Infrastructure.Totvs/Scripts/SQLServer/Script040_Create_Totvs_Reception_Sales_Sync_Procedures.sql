/****** Recepção de venda: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.2) ******/
--As recepções nunca são apagadas por este fluxo; as formas de pagamento que saíram de uma recepção que veio no lote são apagadas.
declare @delete_by_reception nvarchar(max) = 'exists (select 1 from [totvs].TotvsReceptionSales p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.movementDate = target.movementDate and p.sequence = target.sequence and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsReceptionSales', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsReceptionSalesDocumentTypes', @execute = 1, @delete_condition = @delete_by_reception;
