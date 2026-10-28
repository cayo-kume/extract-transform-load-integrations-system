INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (tabelas de preço e promoções: 13634 - 13636)*/
(13092, 13634, 'sp_TotvsPriceTablesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesItems raw e trusted', 120, 1),
(13092, 13635, 'sp_TotvsPriceTablesItemsPrices_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesItemsPrices raw e trusted', 120, 1),
(13093, 13636, 'sp_TotvsPromotions_Sync', 'Procedure de sincronização entre as tabelas TotvsPromotions raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Product: 13080 - 13099, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13092, 'TotvsPostSearchPriceTables', 'Integração dos itens das tabelas de preço vigentes, por produto, loja e escala (TotvsPriceTablesItems e preços), a cada 2 horas', 120, 2),
(130, 13093, 'TotvsGetPromotion', 'Integração dos itens de promoção de todas as lojas (TotvsPromotions), a cada 2 horas', 120, 2);
