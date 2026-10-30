INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 3500 - 3999 (tickets: 3500 - 3515)*/
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

/*Integrações*/
/*Range 3000 - 3499 (leituras: 3000 e 3002, escrita de tickets: 3006 - 3008)*/
(30, 3000, 'MovideskGetTickets', 'Integração de dados das tabelas MovideskTickets e filhas', 3, 2),

/*Sob demanda - executadas pelo MovideskController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(30, 3002, 'MovideskGetTicketById', 'Integração de um ticket por número (GET Tickets/{docCompany}/{id})', 0, 2),
(30, 3006, 'MovideskPostTicket', 'Criação de um ticket (POST Tickets/{docCompany})', 0, 2),
(30, 3007, 'MovideskPatchTicketById', 'Alteração de um ticket por número (PATCH Tickets/{docCompany}/{id})', 0, 2),
(30, 3008, 'MovideskPostTicketFileUploadById', 'Envio de anexos para uma ação (POST Tickets/{docCompany}/{id}/Actions/{actionId}/Files)', 0, 2);
