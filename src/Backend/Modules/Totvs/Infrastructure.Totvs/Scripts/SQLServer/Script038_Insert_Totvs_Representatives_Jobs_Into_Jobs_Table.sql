INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (representantes: 13589 - 13599)*/
(13030, 13589, 'sp_TotvsRepresentatives_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentatives raw e trusted', 120, 1),
(13030, 13590, 'sp_TotvsRepresentativesAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesAddresses raw e trusted', 120, 1),
(13030, 13591, 'sp_TotvsRepresentativesPhones_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesPhones raw e trusted', 120, 1),
(13030, 13592, 'sp_TotvsRepresentativesEmails_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesEmails raw e trusted', 120, 1),
(13030, 13593, 'sp_TotvsRepresentativesObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesObservations raw e trusted', 120, 1),
(13030, 13594, 'sp_TotvsRepresentativesAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesAdditionalFields raw e trusted', 120, 1),
(13030, 13595, 'sp_TotvsRepresentativesClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesClassifications raw e trusted', 120, 1),
(13030, 13596, 'sp_TotvsRepresentativesCustomers_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesCustomers raw e trusted', 120, 1),
(13030, 13597, 'sp_TotvsRepresentativesPriceTables_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesPriceTables raw e trusted', 120, 1),
(13030, 13598, 'sp_TotvsRepresentativesSalesRegions_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesSalesRegions raw e trusted', 120, 1),
(13030, 13599, 'sp_TotvsRepresentativesParameters_Sync', 'Procedure de sincronização entre as tabelas TotvsRepresentativesParameters raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Person: 13030 - 13069, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13030, 'TotvsPostSearchRepresentatives', 'Integração dos representantes (TotvsRepresentatives e filhas), a cada 2 horas', 120, 2),

/*Sob demanda - executada pelo TotvsController, não é recorrente (IntervalTimeInMinutes = 0)*/
(130, 13039, 'TotvsGetRepresentativeByCode', 'Integração de um representante por código (GET Representatives/{docCompany}/{code})', 0, 2);
