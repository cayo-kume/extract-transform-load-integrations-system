INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (preços e custos: 13629 - 13631)*/
(13015, 13629, 'sp_TotvsPrices_Sync', 'Procedure de sincronização entre as tabelas TotvsPrices raw e trusted', 3, 1),
(13015, 13630, 'sp_TotvsPricesOtherPromotions_Sync', 'Procedure de sincronização entre as tabelas TotvsPricesOtherPromotions raw e trusted', 3, 1),
(13016, 13631, 'sp_TotvsCosts_Sync', 'Procedure de sincronização entre as tabelas TotvsCosts raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (fase 3: 13015 - 13016)*/
(130, 13015, 'TotvsPostSearchPrices', 'Integração dos preços de todos os tipos, por loja (TotvsPrices e outras promoções), a cada 3 minutos', 3, 2),
(130, 13016, 'TotvsPostSearchCosts', 'Integração dos custos de todos os tipos, por loja (TotvsCosts), a cada 3 minutos', 3, 2),

/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (tabelas de preço e promoções: 13634 - 13636)*/
(13092, 13634, 'sp_TotvsPriceTablesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesItems raw e trusted', 120, 1),
(13092, 13635, 'sp_TotvsPriceTablesItemsPrices_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesItemsPrices raw e trusted', 120, 1),
(13093, 13636, 'sp_TotvsPromotions_Sync', 'Procedure de sincronização entre as tabelas TotvsPromotions raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Product: 13080 - 13099, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13092, 'TotvsPostSearchPriceTables', 'Integração dos itens das tabelas de preço vigentes, por produto, loja e escala (TotvsPriceTablesItems e preços), a cada 2 horas', 120, 2),
(130, 13093, 'TotvsGetPromotion', 'Integração dos itens de promoção de todas as lojas (TotvsPromotions), a cada 2 horas', 120, 2),

/*Escritas de dados fiscais, preço, custo e promoção da API Product (efeito comercial e fiscal) - executadas pelo TotvsController, não são
  recorrentes (IntervalTimeInMinutes = 0); só rodam com Totvs:Writes:Products:Enabled = true e, nas de valor, com os códigos e lojas permitidos
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.6 e §5.7, fase PR-6)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13131, 'TotvsPostCreateValues', 'Inclusão de preço ou custo (POST Products/{docCompany}/Values/Create)', 0, 2),
(130, 13132, 'TotvsPostUpdateValues', 'Alteração de preço ou custo (POST Products/{docCompany}/Values/Update)', 0, 2),
(130, 13133, 'TotvsPostUpdatePromotionValues', 'Alteração de valores de promoção (POST Products/{docCompany}/PromotionValues/Update)', 0, 2),
(130, 13134, 'TotvsPostPromotionItems', 'Inclusão de produtos numa promoção (POST Products/{docCompany}/PromotionItems)', 0, 2);
