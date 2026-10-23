INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (vendedores: 13502 - 13504)*/
(13006, 13502, 'sp_TotvsSellers_Sync', 'Procedure de sincronização entre as tabelas TotvsSellers raw e trusted', 120, 1),
(13006, 13503, 'sp_TotvsSellersBranchInformations_Sync', 'Procedure de sincronização entre as tabelas TotvsSellersBranchInformations raw e trusted', 120, 1),
(13006, 13504, 'sp_TotvsSellersCustomers_Sync', 'Procedure de sincronização entre as tabelas TotvsSellersCustomers raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13006, 'TotvsPostSearchSellers', 'Integração dos vendedores de todas as lojas (TotvsSellers e filhas), a cada 2 horas', 120, 2);
