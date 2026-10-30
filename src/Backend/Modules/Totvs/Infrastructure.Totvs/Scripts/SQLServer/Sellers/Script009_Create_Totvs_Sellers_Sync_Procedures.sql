/****** Vendedores: procedures de sincronização (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--Os vendedores nunca são apagados por este fluxo; as filhas que saíram de um vendedor que veio no lote são apagadas.
--A condição depende do vendedor e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de TotvsSellers rodar antes.
declare @delete_by_seller nvarchar(max) = 'exists (select 1 from [totvs].TotvsSellers p where p.doc_company = target.doc_company and p.sellerCode = target.sellerCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSellers', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSellersBranchInformations', @execute = 1, @delete_condition = @delete_by_seller;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsSellersCustomers', @execute = 1, @delete_condition = @delete_by_seller;

go

/****** Regiões de atuação: procedures de sincronização (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §4.1) ******/
--As regiões nunca são apagadas por este fluxo; o estado, CEP ou cidade que saiu de uma região que veio no lote é apagado.
declare @delete_by_area nvarchar(max) = 'exists (select 1 from [totvs].TotvsOperationalAreas p where p.doc_company = target.doc_company and p.operationalAreaCode = target.operationalAreaCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreas', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreasStates', @execute = 1, @delete_condition = @delete_by_area;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreasCeps', @execute = 1, @delete_condition = @delete_by_area;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsOperationalAreasCities', @execute = 1, @delete_condition = @delete_by_area;
