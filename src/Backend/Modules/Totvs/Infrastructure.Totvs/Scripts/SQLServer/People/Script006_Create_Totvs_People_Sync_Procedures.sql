/****** Filiais: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As filiais nunca são apagadas por este fluxo; os endereços removidos de uma filial que veio no lote são apagados.
--A condição depende da filial e dos endereços do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de TotvsBranches rodar antes.
declare @delete_by_branch nvarchar(max) = 'exists (select 1 from [totvs].TotvsBranches p where p.doc_company = target.doc_company and p.code = target.branchCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranches', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranchesAddresses', @execute = 1, @delete_condition = @delete_by_branch;

go

/****** Pessoas físicas e jurídicas: procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As pessoas nunca são apagadas por este fluxo; as filhas que saíram de uma pessoa que veio no lote são apagadas.
--A condição depende da pessoa e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge da pessoa rodar antes.
declare @delete_by_customer nvarchar(max) = 'exists (select 1 from [totvs].TotvsPeople p where p.doc_company = target.doc_company and p.code = target.personCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_legalentity nvarchar(max) = 'exists (select 1 from [totvs].TotvsLegalEntities p where p.doc_company = target.doc_company and p.code = target.personCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeople', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleAddresses', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeoplePhones', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleEmails', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleObservations', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleCustomerObservations', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleAdditionalFields', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleClassifications', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleReferences', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleRelateds', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleStatistics', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleRepresentatives', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleFamiliars', @execute = 1, @delete_condition = @delete_by_customer;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntities', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesAddresses', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesPhones', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesEmails', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesObservations', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesCustomerObservations', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesAdditionalFields', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesClassifications', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesReferences', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesRelateds', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesPartners', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesContacts', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesStatistics', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesSocialNetworks', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesRepresentatives', @execute = 1, @delete_condition = @delete_by_legalentity;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesPaymentMethods', @execute = 1, @delete_condition = @delete_by_legalentity;
