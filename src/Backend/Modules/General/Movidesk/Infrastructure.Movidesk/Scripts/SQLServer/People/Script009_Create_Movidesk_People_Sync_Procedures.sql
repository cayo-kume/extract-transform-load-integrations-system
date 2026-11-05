-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
--Filhas e netas apagam as linhas que não vieram no lote da pessoa (src/Backend/Modules/General/Movidesk/docs/MOVIDESK-MODELAGEM-PESSOAS.md seção 5).
--A condição depende da pessoa e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de MovideskPeople rodar antes.
declare @delete_by_person nvarchar(max) = 'exists (select 1 from [movidesk].MovideskPeople p where p.doc_company = target.doc_company and p.id = target.personId and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[movidesk].MovideskPeople') is not null and object_id(N'[movidesk].[sp_MovideskPeople_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeople', @execute = 1;

if object_id(N'[movidesk].MovideskPeopleAddresses') is not null and object_id(N'[movidesk].[sp_MovideskPeopleAddresses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleAddresses', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleContacts') is not null and object_id(N'[movidesk].[sp_MovideskPeopleContacts_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleContacts', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleEmails') is not null and object_id(N'[movidesk].[sp_MovideskPeopleEmails_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleEmails', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleTeams') is not null and object_id(N'[movidesk].[sp_MovideskPeopleTeams_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleTeams', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleRelationships') is not null and object_id(N'[movidesk].[sp_MovideskPeopleRelationships_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleRelationships', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleRelationshipsServices') is not null and object_id(N'[movidesk].[sp_MovideskPeopleRelationshipsServices_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleRelationshipsServices', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleCustomFieldValues') is not null and object_id(N'[movidesk].[sp_MovideskPeopleCustomFieldValues_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleCustomFieldValues', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleCustomFieldValuesItems') is not null and object_id(N'[movidesk].[sp_MovideskPeopleCustomFieldValuesItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleCustomFieldValuesItems', @execute = 1, @delete_condition = @delete_by_person;
if object_id(N'[movidesk].MovideskPeopleAssets') is not null and object_id(N'[movidesk].[sp_MovideskPeopleAssets_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskPeopleAssets', @execute = 1, @delete_condition = @delete_by_person;
