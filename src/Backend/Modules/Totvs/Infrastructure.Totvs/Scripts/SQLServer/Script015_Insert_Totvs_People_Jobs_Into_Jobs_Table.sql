INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (pessoas físicas: 13527 - 13539, pessoas jurídicas: 13540 - 13555)*/
(13002, 13527, 'sp_TotvsPeople_Sync', 'Procedure de sincronização entre as tabelas TotvsPeople raw e trusted', 3, 1),
(13002, 13528, 'sp_TotvsPeopleAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleAddresses raw e trusted', 3, 1),
(13002, 13529, 'sp_TotvsPeoplePhones_Sync', 'Procedure de sincronização entre as tabelas TotvsPeoplePhones raw e trusted', 3, 1),
(13002, 13530, 'sp_TotvsPeopleEmails_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleEmails raw e trusted', 3, 1),
(13002, 13531, 'sp_TotvsPeopleObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleObservations raw e trusted', 3, 1),
(13002, 13532, 'sp_TotvsPeopleCustomerObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleCustomerObservations raw e trusted', 3, 1),
(13002, 13533, 'sp_TotvsPeopleAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleAdditionalFields raw e trusted', 3, 1),
(13002, 13534, 'sp_TotvsPeopleClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleClassifications raw e trusted', 3, 1),
(13002, 13535, 'sp_TotvsPeopleReferences_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleReferences raw e trusted', 3, 1),
(13002, 13536, 'sp_TotvsPeopleRelateds_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleRelateds raw e trusted', 3, 1),
(13002, 13537, 'sp_TotvsPeopleStatistics_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleStatistics raw e trusted', 3, 1),
(13002, 13538, 'sp_TotvsPeopleRepresentatives_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleRepresentatives raw e trusted', 3, 1),
(13002, 13539, 'sp_TotvsPeopleFamiliars_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleFamiliars raw e trusted', 3, 1),

(13001, 13540, 'sp_TotvsLegalEntities_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntities raw e trusted', 3, 1),
(13001, 13541, 'sp_TotvsLegalEntitiesAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesAddresses raw e trusted', 3, 1),
(13001, 13542, 'sp_TotvsLegalEntitiesPhones_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesPhones raw e trusted', 3, 1),
(13001, 13543, 'sp_TotvsLegalEntitiesEmails_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesEmails raw e trusted', 3, 1),
(13001, 13544, 'sp_TotvsLegalEntitiesObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesObservations raw e trusted', 3, 1),
(13001, 13545, 'sp_TotvsLegalEntitiesCustomerObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesCustomerObservations raw e trusted', 3, 1),
(13001, 13546, 'sp_TotvsLegalEntitiesAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesAdditionalFields raw e trusted', 3, 1),
(13001, 13547, 'sp_TotvsLegalEntitiesClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesClassifications raw e trusted', 3, 1),
(13001, 13548, 'sp_TotvsLegalEntitiesReferences_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesReferences raw e trusted', 3, 1),
(13001, 13549, 'sp_TotvsLegalEntitiesRelateds_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesRelateds raw e trusted', 3, 1),
(13001, 13550, 'sp_TotvsLegalEntitiesPartners_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesPartners raw e trusted', 3, 1),
(13001, 13551, 'sp_TotvsLegalEntitiesContacts_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesContacts raw e trusted', 3, 1),
(13001, 13552, 'sp_TotvsLegalEntitiesStatistics_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesStatistics raw e trusted', 3, 1),
(13001, 13553, 'sp_TotvsLegalEntitiesSocialNetworks_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesSocialNetworks raw e trusted', 3, 1),
(13001, 13554, 'sp_TotvsLegalEntitiesRepresentatives_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesRepresentatives raw e trusted', 3, 1),
(13001, 13555, 'sp_TotvsLegalEntitiesPaymentMethods_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesPaymentMethods raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13001, 'TotvsPostSearchLegalEntity', 'Integração das pessoas jurídicas (TotvsLegalEntities e filhas), a cada 3 minutos', 3, 2),
(130, 13002, 'TotvsPostSearchCustomers', 'Integração das pessoas físicas (TotvsPeople e filhas), a cada 3 minutos', 3, 2);
