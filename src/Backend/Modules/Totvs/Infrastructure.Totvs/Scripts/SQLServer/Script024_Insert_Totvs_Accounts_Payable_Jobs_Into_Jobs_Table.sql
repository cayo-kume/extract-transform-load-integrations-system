INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (contas a pagar: 13570 - 13574)*/
(13008, 13570, 'sp_TotvsDuplicates_Sync', 'Procedure de sincronização entre as tabelas TotvsDuplicates raw e trusted', 3, 1),
(13008, 13571, 'sp_TotvsDuplicatesExpenses_Sync', 'Procedure de sincronização entre as tabelas TotvsDuplicatesExpenses raw e trusted', 3, 1),
(13008, 13572, 'sp_TotvsDuplicatesClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsDuplicatesClassifications raw e trusted', 3, 1),
(13009, 13573, 'sp_TotvsCommissionsPaid_Sync', 'Procedure de sincronização entre as tabelas TotvsCommissionsPaid raw e trusted', 3, 1),
(13009, 13574, 'sp_TotvsCommissionsPaidAccountMovements_Sync', 'Procedure de sincronização entre as tabelas TotvsCommissionsPaidAccountMovements raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13008, 'TotvsPostSearchDuplicates', 'Integração das duplicatas a pagar de todas as lojas (TotvsDuplicates e filhas), a cada 3 minutos', 3, 2),
(130, 13009, 'TotvsPostSearchComissionsPaid', 'Integração dos fechamentos de comissão de todas as lojas (TotvsCommissionsPaid e filhas), a cada 3 minutos', 3, 2);
