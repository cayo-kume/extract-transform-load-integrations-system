--Filhas e netas apagam as linhas que não vieram no lote da pessoa (docs/MOVIDESK-MODELAGEM-PESSOAS.md seção 5).
--A condição depende da pessoa e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de MovideskPeople rodar antes.
declare @delete_by_person nvarchar(max) = 'exists (select 1 from [general].MovideskPeople p where p.doc_company = target.doc_company and p.id = target.personId and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeople', @execute = 1;

exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleAddresses', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleContacts', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleEmails', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleTeams', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleRelationships', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleRelationshipsServices', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleCustomFieldValues', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleCustomFieldValuesItems', @execute = 1, @delete_condition = @delete_by_person;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskPeopleAssets', @execute = 1, @delete_condition = @delete_by_person;
