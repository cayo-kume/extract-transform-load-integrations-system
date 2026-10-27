INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (regiões de atuação: 13602 - 13605)*/
(13070, 13602, 'sp_TotvsOperationalAreas_Sync', 'Procedure de sincronização entre as tabelas TotvsOperationalAreas raw e trusted', 120, 1),
(13070, 13603, 'sp_TotvsOperationalAreasStates_Sync', 'Procedure de sincronização entre as tabelas TotvsOperationalAreasStates raw e trusted', 120, 1),
(13070, 13604, 'sp_TotvsOperationalAreasCeps_Sync', 'Procedure de sincronização entre as tabelas TotvsOperationalAreasCeps raw e trusted', 120, 1),
(13070, 13605, 'sp_TotvsOperationalAreasCities_Sync', 'Procedure de sincronização entre as tabelas TotvsOperationalAreasCities raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Seller: 13070 - 13079, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13070, 'TotvsGetOperationalArea', 'Integração das regiões de atuação, com estados, CEPs e cidades (TotvsOperationalAreas e filhas), a cada 2 horas', 120, 2);
