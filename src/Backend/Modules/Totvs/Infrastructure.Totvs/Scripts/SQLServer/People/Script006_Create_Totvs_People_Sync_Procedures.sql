-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Filiais: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As filiais nunca são apagadas por este fluxo; os endereços removidos de uma filial que veio no lote são apagados.
--A condição depende da filial e dos endereços do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de TotvsBranches rodar antes.
declare @delete_by_branch nvarchar(max) = 'exists (select 1 from [totvs].TotvsBranches p where p.doc_company = target.doc_company and p.code = target.branchCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[totvs].TotvsBranches') is not null and object_id(N'[totvs].[sp_TotvsBranches_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranches', @execute = 1;
if object_id(N'[totvs].TotvsBranchesAddresses') is not null and object_id(N'[totvs].[sp_TotvsBranchesAddresses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsBranchesAddresses', @execute = 1, @delete_condition = @delete_by_branch;

go

/****** Pessoas físicas e jurídicas: procedures de sincronização (src/Backend/Modules/Totvs/.docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.3) ******/
--As pessoas nunca são apagadas por este fluxo; as filhas que saíram de uma pessoa que veio no lote são apagadas.
--A condição depende da pessoa e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge da pessoa rodar antes.
declare @delete_by_customer nvarchar(max) = 'exists (select 1 from [totvs].TotvsPeople p where p.doc_company = target.doc_company and p.code = target.personCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';
declare @delete_by_legalentity nvarchar(max) = 'exists (select 1 from [totvs].TotvsLegalEntities p where p.doc_company = target.doc_company and p.code = target.personCode and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[totvs].TotvsPeople') is not null and object_id(N'[totvs].[sp_TotvsPeople_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeople', @execute = 1;
if object_id(N'[totvs].TotvsPeopleAddresses') is not null and object_id(N'[totvs].[sp_TotvsPeopleAddresses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleAddresses', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeoplePhones') is not null and object_id(N'[totvs].[sp_TotvsPeoplePhones_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeoplePhones', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleEmails') is not null and object_id(N'[totvs].[sp_TotvsPeopleEmails_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleEmails', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleObservations') is not null and object_id(N'[totvs].[sp_TotvsPeopleObservations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleObservations', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleCustomerObservations') is not null and object_id(N'[totvs].[sp_TotvsPeopleCustomerObservations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleCustomerObservations', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleAdditionalFields') is not null and object_id(N'[totvs].[sp_TotvsPeopleAdditionalFields_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleAdditionalFields', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleClassifications') is not null and object_id(N'[totvs].[sp_TotvsPeopleClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleClassifications', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleReferences') is not null and object_id(N'[totvs].[sp_TotvsPeopleReferences_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleReferences', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleRelateds') is not null and object_id(N'[totvs].[sp_TotvsPeopleRelateds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleRelateds', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleStatistics') is not null and object_id(N'[totvs].[sp_TotvsPeopleStatistics_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleStatistics', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleRepresentatives') is not null and object_id(N'[totvs].[sp_TotvsPeopleRepresentatives_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleRepresentatives', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsPeopleFamiliars') is not null and object_id(N'[totvs].[sp_TotvsPeopleFamiliars_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPeopleFamiliars', @execute = 1, @delete_condition = @delete_by_customer;
if object_id(N'[totvs].TotvsLegalEntities') is not null and object_id(N'[totvs].[sp_TotvsLegalEntities_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntities', @execute = 1;
if object_id(N'[totvs].TotvsLegalEntitiesAddresses') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesAddresses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesAddresses', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesPhones') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesPhones_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesPhones', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesEmails') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesEmails_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesEmails', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesObservations') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesObservations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesObservations', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesCustomerObservations') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesCustomerObservations_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesCustomerObservations', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesAdditionalFields') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesAdditionalFields_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesAdditionalFields', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesClassifications') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesClassifications', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesReferences') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesReferences_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesReferences', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesRelateds') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesRelateds_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesRelateds', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesPartners') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesPartners_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesPartners', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesContacts') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesContacts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesContacts', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesStatistics') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesStatistics_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesStatistics', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesSocialNetworks') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesSocialNetworks_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesSocialNetworks', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesRepresentatives') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesRepresentatives_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesRepresentatives', @execute = 1, @delete_condition = @delete_by_legalentity;
if object_id(N'[totvs].TotvsLegalEntitiesPaymentMethods') is not null and object_id(N'[totvs].[sp_TotvsLegalEntitiesPaymentMethods_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsLegalEntitiesPaymentMethods', @execute = 1, @delete_condition = @delete_by_legalentity;
