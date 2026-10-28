INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (inutilizações e centros de custo: 13648 - 13650)*/
(13250, 13648, 'sp_TotvsInvoicesDisable_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesDisable raw e trusted', 120, 1),
(13251, 13649, 'sp_TotvsCostCenters_Sync', 'Procedure de sincronização entre as tabelas TotvsCostCenters raw e trusted', 120, 1),
(13251, 13650, 'sp_TotvsCostCentersClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsCostCentersClassifications raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13250, 'TotvsGetInvoicesDisable', 'Integração das faixas de numeração inutilizadas de todas as lojas (TotvsInvoicesDisable), a cada 2 horas', 120, 2),
(130, 13251, 'TotvsGetCostCenter', 'Integração dos centros de custo, com as classificações (TotvsCostCenters e filha), a cada 2 horas', 120, 2);
