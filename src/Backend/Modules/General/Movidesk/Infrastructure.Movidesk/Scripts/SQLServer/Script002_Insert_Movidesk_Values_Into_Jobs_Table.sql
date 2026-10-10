--INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
--VALUES
/*Procedures de Sincronizações*/
/*Range 3500 - 3999*/
--(3001, 3500,'P_MovideskPersons_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskPersons raw e trusted', 3, 1),
--(3001, 3501,'P_MovideskPersonsAddresses_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskPersonsAddresses raw e trusted', 3, 1),
--(3001, 3502,'P_MovideskPersonsContacts_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskPersonsContacts raw e trusted', 3, 1),
--(3001, 3503,'P_MovideskPersonsEmails_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskPersonsEmails raw e trusted', 3, 1),
--(3001, 3504,'P_MovideskPersonsRelationships_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskPersonsRelationships raw e trusted', 3, 1),

--(3000, 3505,'P_MovideskTickets_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskTickets raw e trusted', 3, 1),
--(3000, 3506,'P_MovideskActions_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskActions raw e trusted', 3, 1),
--(3000, 3507,'P_MovideskAttachments_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskAttachments raw e trusted', 3, 1),
--(3000, 3508,'P_MovideskCustomFieldsValues_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskCustomFieldsValues raw e trusted', 3, 1),
--(3000, 3509,'P_MovideskCustomFieldsItems_Sincronizacao', 'Procedure de sicronização entre as tabelas MovideskCustomFieldsItems raw e trusted', 3, 1),

/*Integrações*/
/*Range 3000 - 3499*/
--(30, 3000, 'MovideskGetTickets', 'Integração de dados da tabela MovideskTickets', 3, 2),
--(30, 3001, 'MovideskGetPersons', 'Integração de dados da tabela MovideskPersons', 3, 2),
