INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (preços, custos e saldos: 13629 - 13633)*/
(13015, 13629, 'sp_TotvsPrices_Sync', 'Procedure de sincronização entre as tabelas TotvsPrices raw e trusted', 3, 1),
(13015, 13630, 'sp_TotvsPricesOtherPromotions_Sync', 'Procedure de sincronização entre as tabelas TotvsPricesOtherPromotions raw e trusted', 3, 1),
(13016, 13631, 'sp_TotvsCosts_Sync', 'Procedure de sincronização entre as tabelas TotvsCosts raw e trusted', 3, 1),
(13017, 13632, 'sp_TotvsBalances_Sync', 'Procedure de sincronização entre as tabelas TotvsBalances raw e trusted', 3, 1),
(13017, 13633, 'sp_TotvsBalancesLocations_Sync', 'Procedure de sincronização entre as tabelas TotvsBalancesLocations raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (fase 3: 13015 - 13017)*/
(130, 13015, 'TotvsPostSearchPrices', 'Integração dos preços de todos os tipos, por loja (TotvsPrices e outras promoções), a cada 3 minutos', 3, 2),
(130, 13016, 'TotvsPostSearchCosts', 'Integração dos custos de todos os tipos, por loja (TotvsCosts), a cada 3 minutos', 3, 2),
(130, 13017, 'TotvsPostSearchBalances', 'Integração dos saldos de todos os tipos, por loja (TotvsBalances e locais), a cada 3 minutos', 3, 2);
