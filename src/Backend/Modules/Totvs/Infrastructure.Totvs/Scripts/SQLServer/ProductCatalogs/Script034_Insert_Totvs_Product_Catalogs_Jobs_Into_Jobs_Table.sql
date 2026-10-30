INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (catálogos da API Product: 13606 - 13628)*/
(13080, 13606, 'sp_TotvsClassificationTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsClassificationTypes raw e trusted', 120, 1),
(13081, 13607, 'sp_TotvsProductClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsProductClassifications raw e trusted', 120, 1),
(13082, 13608, 'sp_TotvsCategories_Sync', 'Procedure de sincronização entre as tabelas TotvsCategories raw e trusted', 120, 1),
(13083, 13609, 'sp_TotvsGrids_Sync', 'Procedure de sincronização entre as tabelas TotvsGrids raw e trusted', 120, 1),
(13083, 13610, 'sp_TotvsGridsSizes_Sync', 'Procedure de sincronização entre as tabelas TotvsGridsSizes raw e trusted', 120, 1),
(13084, 13611, 'sp_TotvsMeasurementUnits_Sync', 'Procedure de sincronização entre as tabelas TotvsMeasurementUnits raw e trusted', 120, 1),
(13084, 13612, 'sp_TotvsMeasurementUnitsVariations_Sync', 'Procedure de sincronização entre as tabelas TotvsMeasurementUnitsVariations raw e trusted', 120, 1),
(13085, 13613, 'sp_TotvsColors_Sync', 'Procedure de sincronização entre as tabelas TotvsColors raw e trusted', 120, 1),
(13085, 13614, 'sp_TotvsColorsAdditionalInformation_Sync', 'Procedure de sincronização entre as tabelas TotvsColorsAdditionalInformation raw e trusted', 120, 1),
(13086, 13615, 'sp_TotvsAdditionalFieldsTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsAdditionalFieldsTypes raw e trusted', 120, 1),
(13087, 13616, 'sp_TotvsProductGrouperConfigurations_Sync', 'Procedure de sincronização entre as tabelas TotvsProductGrouperConfigurations raw e trusted', 120, 1),
(13087, 13617, 'sp_TotvsProductGrouperConfigurationsItems_Sync', 'Procedure de sincronização entre as tabelas TotvsProductGrouperConfigurationsItems raw e trusted', 120, 1),
(13088, 13618, 'sp_TotvsInstructionItems_Sync', 'Procedure de sincronização entre as tabelas TotvsInstructionItems raw e trusted', 120, 1),
(13089, 13619, 'sp_TotvsCompositions_Sync', 'Procedure de sincronização entre as tabelas TotvsCompositions raw e trusted', 120, 1),
(13089, 13620, 'sp_TotvsCompositionsFibers_Sync', 'Procedure de sincronização entre as tabelas TotvsCompositionsFibers raw e trusted', 120, 1),
(13090, 13621, 'sp_TotvsPriceTables_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTables raw e trusted', 120, 1),
(13090, 13622, 'sp_TotvsPriceTablesSalesOrderClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesSalesOrderClassifications raw e trusted', 120, 1),
(13090, 13623, 'sp_TotvsPriceTablesPersonClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesPersonClassifications raw e trusted', 120, 1),
(13090, 13624, 'sp_TotvsPriceTablesPaymentConditions_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesPaymentConditions raw e trusted', 120, 1),
(13090, 13625, 'sp_TotvsPriceTablesAveragePeriods_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesAveragePeriods raw e trusted', 120, 1),
(13090, 13626, 'sp_TotvsPriceTablesAveragePeriodQuantities_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTablesAveragePeriodQuantities raw e trusted', 120, 1),
(13091, 13627, 'sp_TotvsPriceTableScales_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTableScales raw e trusted', 120, 1),
(13091, 13628, 'sp_TotvsPriceTableScalesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsPriceTableScalesItems raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Product: 13080 - 13099, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13080, 'TotvsGetClassificationType', 'Integração do catálogo de tipos de classificação de produto da API Product (TotvsClassificationTypes), a cada 2 horas', 120, 2),
(130, 13081, 'TotvsProductGetClassifications', 'Integração do catálogo de classificações de produto da API Product (TotvsProductClassifications), a cada 2 horas', 120, 2),
(130, 13082, 'TotvsGetCategory', 'Integração do catálogo de categorias de produto da API Product (TotvsCategories), a cada 2 horas', 120, 2),
(130, 13083, 'TotvsGetGrid', 'Integração do catálogo de grades da API Product (TotvsGrids e filhas), a cada 2 horas', 120, 2),
(130, 13084, 'TotvsGetMeasurementUnit', 'Integração do catálogo de unidades de medida da API Product (TotvsMeasurementUnits e filhas), a cada 2 horas', 120, 2),
(130, 13085, 'TotvsPostSearchColors', 'Integração do catálogo de cores da API Product (TotvsColors e filhas), a cada 2 horas', 120, 2),
(130, 13086, 'TotvsGetAdditionalFieldsTypes', 'Integração do catálogo de tipos de campo adicional da API Product (TotvsAdditionalFieldsTypes), a cada 2 horas', 120, 2),
(130, 13087, 'TotvsGetProductGrouperConfiguration', 'Integração do catálogo de configurações de agrupador de produto da API Product (TotvsProductGrouperConfigurations e filhas), a cada 2 horas', 120, 2),
(130, 13088, 'TotvsGetInstructionItems', 'Integração do catálogo de itens de instrução da API Product (TotvsInstructionItems), a cada 2 horas', 120, 2),
(130, 13089, 'TotvsPostCompositions', 'Integração do catálogo de composições da API Product (TotvsCompositions e filhas), a cada 2 horas', 120, 2),
(130, 13090, 'TotvsGetPriceTablesHeaders', 'Integração do catálogo de cabeçalhos de tabela de preço da API Product (TotvsPriceTables e filhas), a cada 2 horas', 120, 2),
(130, 13091, 'TotvsGetPriceTableScales', 'Integração do catálogo de escalas de tabela de preço da API Product (TotvsPriceTableScales e filhas), a cada 2 horas', 120, 2),

/*Escritas da API Product - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:Products:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.5, fase PR-4)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13124, 'TotvsPostColors', 'Inclusão ou alteração de uma cor (POST Products/{docCompany}/Colors)', 0, 2),
(130, 13126, 'TotvsPostGrids', 'Inclusão de uma grade de tamanhos (POST Products/{docCompany}/Grids)', 0, 2),

/*Escritas de cadastro da API Product - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:Products:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.5, §5.6 e §5.8, fase PR-5)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13125, 'TotvsPostClassifications', 'Inclusão ou alteração de tipo de classificação de produto (POST Products/{docCompany}/Classifications)', 0, 2);
