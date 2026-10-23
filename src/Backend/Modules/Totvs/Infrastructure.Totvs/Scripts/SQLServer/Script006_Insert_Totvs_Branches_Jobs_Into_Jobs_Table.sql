INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (filiais: 13500 - 13501)*/
(13007, 13500, 'sp_TotvsBranches_Sync', 'Procedure de sincronização entre as tabelas TotvsBranches raw e trusted', 120, 1),
(13007, 13501, 'sp_TotvsBranchesAddresses_Sync', 'Procedure de sincronização entre as tabelas TotvsBranchesAddresses raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13007, 'TotvsGetBranchesList', 'Integração das empresas da holding (TotvsBranches e TotvsBranchesAddresses), a cada 2 horas', 120, 2);
