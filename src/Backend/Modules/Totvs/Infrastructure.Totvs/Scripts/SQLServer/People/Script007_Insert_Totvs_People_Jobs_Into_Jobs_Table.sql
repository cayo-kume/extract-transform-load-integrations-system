INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (filiais: 13500 - 13501)*/
(13007, 13500, 'sp_TotvsBranches_Sync', 'Procedure de sincronização entre as tabelas TotvsBranches raw e trusted', 120, NULL, 'default', 1),
(13007, 13501, 'sp_TotvsBranchesAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsBranchesAddresses raw e trusted', 120, NULL, 'default', 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13007, 'TotvsGetBranchesList', 'Integração das empresas da holding (TotvsBranches e TotvsBranchesAddresses), a cada 2 horas', 120, NULL, 'default', 2),

/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (pessoas físicas: 13527 - 13539, pessoas jurídicas: 13540 - 13555)*/
(13002, 13527, 'sp_TotvsPeople_Sync', 'Procedure de sincronização entre as tabelas TotvsPeople raw e trusted', 3, NULL, 'default', 1),
(13002, 13528, 'sp_TotvsPeopleAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleAddresses raw e trusted', 3, NULL, 'default', 1),
(13002, 13529, 'sp_TotvsPeoplePhones_Sync', 'Procedure de sincronização entre as tabelas TotvsPeoplePhones raw e trusted', 3, NULL, 'default', 1),
(13002, 13530, 'sp_TotvsPeopleEmails_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleEmails raw e trusted', 3, NULL, 'default', 1),
(13002, 13531, 'sp_TotvsPeopleObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleObservations raw e trusted', 3, NULL, 'default', 1),
(13002, 13532, 'sp_TotvsPeopleCustomerObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleCustomerObservations raw e trusted', 3, NULL, 'default', 1),
(13002, 13533, 'sp_TotvsPeopleAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleAdditionalFields raw e trusted', 3, NULL, 'default', 1),
(13002, 13534, 'sp_TotvsPeopleClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleClassifications raw e trusted', 3, NULL, 'default', 1),
(13002, 13535, 'sp_TotvsPeopleReferences_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleReferences raw e trusted', 3, NULL, 'default', 1),
(13002, 13536, 'sp_TotvsPeopleRelateds_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleRelateds raw e trusted', 3, NULL, 'default', 1),
(13002, 13537, 'sp_TotvsPeopleStatistics_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleStatistics raw e trusted', 3, NULL, 'default', 1),
(13002, 13538, 'sp_TotvsPeopleRepresentatives_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleRepresentatives raw e trusted', 3, NULL, 'default', 1),
(13002, 13539, 'sp_TotvsPeopleFamiliars_Sync', 'Procedure de sincronização entre as tabelas TotvsPeopleFamiliars raw e trusted', 3, NULL, 'default', 1),
(13001, 13540, 'sp_TotvsLegalEntities_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntities raw e trusted', 3, NULL, 'default', 1),
(13001, 13541, 'sp_TotvsLegalEntitiesAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesAddresses raw e trusted', 3, NULL, 'default', 1),
(13001, 13542, 'sp_TotvsLegalEntitiesPhones_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesPhones raw e trusted', 3, NULL, 'default', 1),
(13001, 13543, 'sp_TotvsLegalEntitiesEmails_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesEmails raw e trusted', 3, NULL, 'default', 1),
(13001, 13544, 'sp_TotvsLegalEntitiesObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesObservations raw e trusted', 3, NULL, 'default', 1),
(13001, 13545, 'sp_TotvsLegalEntitiesCustomerObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesCustomerObservations raw e trusted', 3, NULL, 'default', 1),
(13001, 13546, 'sp_TotvsLegalEntitiesAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesAdditionalFields raw e trusted', 3, NULL, 'default', 1),
(13001, 13547, 'sp_TotvsLegalEntitiesClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesClassifications raw e trusted', 3, NULL, 'default', 1),
(13001, 13548, 'sp_TotvsLegalEntitiesReferences_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesReferences raw e trusted', 3, NULL, 'default', 1),
(13001, 13549, 'sp_TotvsLegalEntitiesRelateds_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesRelateds raw e trusted', 3, NULL, 'default', 1),
(13001, 13550, 'sp_TotvsLegalEntitiesPartners_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesPartners raw e trusted', 3, NULL, 'default', 1),
(13001, 13551, 'sp_TotvsLegalEntitiesContacts_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesContacts raw e trusted', 3, NULL, 'default', 1),
(13001, 13552, 'sp_TotvsLegalEntitiesStatistics_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesStatistics raw e trusted', 3, NULL, 'default', 1),
(13001, 13553, 'sp_TotvsLegalEntitiesSocialNetworks_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesSocialNetworks raw e trusted', 3, NULL, 'default', 1),
(13001, 13554, 'sp_TotvsLegalEntitiesRepresentatives_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesRepresentatives raw e trusted', 3, NULL, 'default', 1),
(13001, 13555, 'sp_TotvsLegalEntitiesPaymentMethods_Sync', 'Procedure de sincronização entre as tabelas TotvsLegalEntitiesPaymentMethods raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13001, 'TotvsPostSearchLegalEntity', 'Integração das pessoas jurídicas (TotvsLegalEntities e filhas), a cada 3 minutos', 3, NULL, 'default', 2),
(130, 13002, 'TotvsPostSearchCustomers', 'Integração das pessoas físicas (TotvsPeople e filhas), a cada 3 minutos', 3, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13010, 'TotvsGetCustomerByCode', 'Integração de uma pessoa física por código (GET Customers/{docCompany}/{code})', 0, NULL, 'default', 2),
(130, 13011, 'TotvsGetLegalEntityByCode', 'Integração de uma pessoa jurídica por código (GET LegalEntities/{docCompany}/{code})', 0, NULL, 'default', 2),

/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13036, 'TotvsGetBranches', 'Dados de uma loja pelo código ou CNPJ (GET Branches/{docCompany}/{branchId})', 0, NULL, 'default', 2),
(130, 13037, 'TotvsGetPersonStatistics', 'Estatísticas de compra e crédito de um cliente (GET PersonStatistics/{docCompany})', 0, NULL, 'default', 2),

/*Escritas da API Person - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:People:Enabled = true (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.4, fase PE-5)*/
/*Range 13000 - 13499 (Person: 13030 - 13069)*/
(130, 13044, 'TotvsPostMessages', 'Inclusão de mensagens de uma pessoa (POST Messages/{docCompany})', 0, NULL, 'default', 2),
(130, 13045, 'TotvsPutChangeMessages', 'Alteração de uma mensagem da pessoa (PUT Messages/{docCompany}/Change)', 0, NULL, 'default', 2),
(130, 13046, 'TotvsDeleteMessages', 'Exclusão de uma mensagem da pessoa (DELETE Messages/{docCompany}/{personCode}/{sequence})', 0, NULL, 'default', 2),

/*Escritas de cadastro da API Person - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:People:Enabled = true (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.1 a §3.4.3, fase PE-6)*/
/*Range 13000 - 13499 (Person: 13030 - 13069)*/
(130, 13040, 'TotvsPostIndividualCustomers', 'Inclusão ou alteração de cliente pessoa física (POST IndividualCustomers/{docCompany})', 0, NULL, 'default', 2),
(130, 13041, 'TotvsPostLegalCustomers', 'Inclusão ou alteração de cliente pessoa jurídica (POST LegalCustomers/{docCompany})', 0, NULL, 'default', 2),
(130, 13043, 'TotvsPostRemoveClassificationRelationship', 'Remoção de classificações de uma pessoa (POST ClassificationRelationship/{docCompany}/Remove)', 0, NULL, 'default', 2);
