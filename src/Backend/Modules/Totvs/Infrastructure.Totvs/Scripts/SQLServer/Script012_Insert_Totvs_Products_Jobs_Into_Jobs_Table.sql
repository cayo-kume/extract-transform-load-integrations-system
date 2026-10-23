INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (produtos: 13505 - 13526)*/
(13000, 13505, 'sp_TotvsSkus_Sync', 'Procedure de sincronização entre as tabelas TotvsSkus raw e trusted', 120, 1),
(13000, 13506, 'sp_TotvsSkusGrids_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusGrids raw e trusted', 120, 1),
(13000, 13507, 'sp_TotvsSkusObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusObservations raw e trusted', 120, 1),
(13000, 13508, 'sp_TotvsSkusDetails_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusDetails raw e trusted', 120, 1),
(13000, 13509, 'sp_TotvsSkusCompositions_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusCompositions raw e trusted', 120, 1),
(13000, 13510, 'sp_TotvsSkusReferenceCategories_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusReferenceCategories raw e trusted', 120, 1),
(13000, 13511, 'sp_TotvsSkusIntegrationGroupCodes_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusIntegrationGroupCodes raw e trusted', 120, 1),
(13000, 13512, 'sp_TotvsSkusColors_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColors raw e trusted', 120, 1),
(13000, 13513, 'sp_TotvsSkusColorsMeasurementsTableOmni_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColorsMeasurementsTableOmni raw e trusted', 120, 1),
(13000, 13514, 'sp_TotvsSkusColorsMeasurementsTableOmniMedidas_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColorsMeasurementsTableOmniMedidas raw e trusted', 120, 1),
(13000, 13515, 'sp_TotvsSkusColorsProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColorsProducts raw e trusted', 120, 1),
(13000, 13516, 'sp_TotvsProductsBarCodes_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsBarCodes raw e trusted', 120, 1),
(13000, 13517, 'sp_TotvsProductsClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsClassifications raw e trusted', 120, 1),
(13000, 13518, 'sp_TotvsProductsAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsAdditionalFields raw e trusted', 120, 1),
(13000, 13519, 'sp_TotvsProductsSuppliers_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsSuppliers raw e trusted', 120, 1),
(13000, 13520, 'sp_TotvsProductsManufacturers_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsManufacturers raw e trusted', 120, 1),
(13000, 13521, 'sp_TotvsProductsReferenceCodeSequences_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsReferenceCodeSequences raw e trusted', 120, 1),
(13000, 13522, 'sp_TotvsProductsWebData_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsWebData raw e trusted', 120, 1),
(13000, 13523, 'sp_TotvsProductsWebDetails_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsWebDetails raw e trusted', 120, 1),
(13000, 13524, 'sp_TotvsProductsBranchesBlocked_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsBranchesBlocked raw e trusted', 120, 1),
(13000, 13525, 'sp_TotvsProductsConservationInstructions_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsConservationInstructions raw e trusted', 120, 1),
(13000, 13526, 'sp_TotvsProductsConservationInstructionsItems_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsConservationInstructionsItems raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13000, 'TotvsPostSearchReferences', 'Integração das referências e produtos das marcas (TotvsSkus e filhas), uma chamada por matriz, a cada 2 horas', 120, 2);
