INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 3500 - 3999 (tickets: 3500 - 3515, pessoas: 3516 - 3525)*/
(3000, 3500, 'sp_MovideskTickets_Sync', 'Procedure de sincronização entre as tabelas MovideskTickets raw e trusted', 3, 1),
(3000, 3501, 'sp_MovideskTicketsTags_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsTags raw e trusted', 3, 1),
(3000, 3502, 'sp_MovideskTicketsClients_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsClients raw e trusted', 3, 1),
(3000, 3503, 'sp_MovideskTicketsRelationships_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsRelationships raw e trusted', 3, 1),
(3000, 3504, 'sp_MovideskTicketsOwnerHistories_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsOwnerHistories raw e trusted', 3, 1),
(3000, 3505, 'sp_MovideskTicketsStatusHistories_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsStatusHistories raw e trusted', 3, 1),
(3000, 3506, 'sp_MovideskTicketsCustomFieldValues_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsCustomFieldValues raw e trusted', 3, 1),
(3000, 3507, 'sp_MovideskTicketsCustomFieldValuesItems_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsCustomFieldValuesItems raw e trusted', 3, 1),
(3000, 3508, 'sp_MovideskTicketsAssets_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsAssets raw e trusted', 3, 1),
(3000, 3509, 'sp_MovideskTicketsActions_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsActions raw e trusted', 3, 1),
(3000, 3510, 'sp_MovideskTicketsActionsTags_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsActionsTags raw e trusted', 3, 1),
(3000, 3511, 'sp_MovideskTicketsActionsTimeAppointments_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsActionsTimeAppointments raw e trusted', 3, 1),
(3000, 3512, 'sp_MovideskTicketsActionsExpenses_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsActionsExpenses raw e trusted', 3, 1),
(3000, 3513, 'sp_MovideskTicketsActionsAttachments_Sync', 'Procedure de sincronização entre as tabelas MovideskTicketsActionsAttachments raw e trusted', 3, 1),

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
/*Range 3000 - 3499*/
(30, 3000, 'MovideskGetTickets', 'Integração de dados das tabelas MovideskTickets e filhas', 3, 2),
(30, 3001, 'MovideskGetPeople', 'Integração de dados das tabelas MovideskPeople e filhas', 3, 2),

/*Sob demanda - executadas pelo MovideskController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(30, 3002, 'MovideskGetTicket', 'Integração de um ticket por número (GET Tickets/{docCompany}/{id})', 0, 2),
(30, 3003, 'MovideskGetPerson', 'Integração de uma pessoa por Cód. Ref. (GET People/{docCompany}/{id})', 0, 2);
