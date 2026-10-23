INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (notas fiscais: 13575 - 13584)*/
(13003, 13575, 'sp_TotvsInvoices_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoices raw e trusted', 3, 1),
(13003, 13576, 'sp_TotvsInvoicesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItems raw e trusted', 3, 1),
(13003, 13577, 'sp_TotvsInvoicesItemsProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsProducts raw e trusted', 3, 1),
(13003, 13578, 'sp_TotvsInvoicesItemsProductsReferenced_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsProductsReferenced raw e trusted', 3, 1),
(13003, 13579, 'sp_TotvsInvoicesItemsTaxes_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsTaxes raw e trusted', 3, 1),
(13003, 13580, 'sp_TotvsInvoicesItemsBarCodes_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsBarCodes raw e trusted', 3, 1),
(13003, 13581, 'sp_TotvsInvoicesPayments_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesPayments raw e trusted', 3, 1),
(13003, 13582, 'sp_TotvsInvoicesSalesOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesSalesOrders raw e trusted', 3, 1),
(13003, 13583, 'sp_TotvsInvoicesObservationsNF_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesObservationsNF raw e trusted', 3, 1),
(13003, 13584, 'sp_TotvsInvoicesReferencedTaxes_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesReferencedTaxes raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13003, 'TotvsPostSearchInvoices', 'Integração das notas fiscais de todas as lojas (TotvsInvoices e filhas), a cada 3 minutos', 3, 2);
