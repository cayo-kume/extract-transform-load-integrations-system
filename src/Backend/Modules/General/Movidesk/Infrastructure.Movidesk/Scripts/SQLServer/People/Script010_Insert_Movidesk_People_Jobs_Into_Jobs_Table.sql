INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 3500 - 3999 (pessoas: 3516 - 3525)*/
(3001, 3516, 'sp_MovideskPeople_Sync', 'Procedure de sincronização entre as tabelas MovideskPeople raw e trusted', 3, 1),
(3001, 3517, 'sp_MovideskPeopleAddresses_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleAddresses raw e trusted', 3, 1),
(3001, 3518, 'sp_MovideskPeopleContacts_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleContacts raw e trusted', 3, 1),
(3001, 3519, 'sp_MovideskPeopleEmails_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleEmails raw e trusted', 3, 1),
(3001, 3520, 'sp_MovideskPeopleTeams_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleTeams raw e trusted', 3, 1),
(3001, 3521, 'sp_MovideskPeopleRelationships_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleRelationships raw e trusted', 3, 1),
(3001, 3522, 'sp_MovideskPeopleRelationshipsServices_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleRelationshipsServices raw e trusted', 3, 1),
(3001, 3523, 'sp_MovideskPeopleCustomFieldValues_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleCustomFieldValues raw e trusted', 3, 1),
(3001, 3524, 'sp_MovideskPeopleCustomFieldValuesItems_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleCustomFieldValuesItems raw e trusted', 3, 1),
(3001, 3525, 'sp_MovideskPeopleAssets_Sync', 'Procedure de sincronização entre as tabelas MovideskPeopleAssets raw e trusted', 3, 1),

/*Integrações*/
/*Range 3000 - 3499 (leituras: 3001 e 3003, escrita de pessoas: 3009 - 3012)*/
(30, 3001, 'MovideskGetPeople', 'Integração de dados das tabelas MovideskPeople e filhas', 3, 2),

/*Sob demanda - executadas pelo MovideskController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(30, 3003, 'MovideskGetPersonById', 'Integração de uma pessoa por Cód. Ref. (GET People/{docCompany}/{id})', 0, 2),
(30, 3009, 'MovideskPostPerson', 'Criação de uma pessoa (POST People/{docCompany})', 0, 2),
(30, 3010, 'MovideskPatchPersonById', 'Alteração de uma pessoa por Cód. Ref. (PATCH People/{docCompany}/{id})', 0, 2),
(30, 3011, 'MovideskPostPersonRelationshipsById', 'Inclusão de organizações numa pessoa (POST People/{docCompany}/{id}/Relationships)', 0, 2),
(30, 3012, 'MovideskDeletePersonById', 'Exclusão de uma pessoa por Cód. Ref. (DELETE People/{docCompany}/{id})', 0, 2);
