/****** Representantes: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.1) ******/
--Os representantes nunca são apagados por este fluxo; as filhas que saíram de um representante que veio no lote são apagadas.
--A condição depende do mesmo integrated_or_updated_at no lote e do merge de TotvsRepresentatives rodar antes.
declare @delete_by_representative nvarchar(max) = 'exists (select 1 from [totvs].TotvsRepresentatives p where p.doc_company = target.doc_company and p.code = target.personCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentatives', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesAddresses', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesPhones', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesEmails', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesObservations', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesAdditionalFields', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesClassifications', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesCustomers', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesPriceTables', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesSalesRegions', @execute = 1, @delete_condition = @delete_by_representative;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsRepresentativesParameters', @execute = 1, @delete_condition = @delete_by_representative;

go

/****** Recepção de venda: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.2) ******/
--As recepções nunca são apagadas por este fluxo; as formas de pagamento que saíram de uma recepção que veio no lote são apagadas.
declare @delete_by_reception nvarchar(max) = 'exists (select 1 from [totvs].TotvsReceptionSales p where p.doc_company = target.doc_company and p.branchCode = target.branchCode and p.movementDate = target.movementDate and p.sequence = target.sequence and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsReceptionSales', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsReceptionSalesDocumentTypes', @execute = 1, @delete_condition = @delete_by_reception;
