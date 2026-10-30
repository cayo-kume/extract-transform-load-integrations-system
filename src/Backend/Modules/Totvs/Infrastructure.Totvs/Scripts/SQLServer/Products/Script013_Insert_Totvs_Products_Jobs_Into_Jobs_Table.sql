INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (produtos: 13505 - 13526)*/
(13000, 13505, 'sp_TotvsSkus_Sync', 'Procedure de sincronização entre as tabelas TotvsSkus raw e trusted', 120, 1),
(13000, 13506, 'sp_TotvsSkusGrids_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusGrids raw e trusted', 120, 1),
(13000, 13507, 'sp_TotvsSkusObservations_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusObservations raw e trusted', 120, 1),
(13000, 13508, 'sp_TotvsSkusDetails_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusDetails raw e trusted', 120, 1),
(13000, 13509, 'sp_TotvsSkusCompositions_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusCompositions raw e trusted', 120, 1),
(13000, 13510, 'sp_TotvsSkusReferenceCategories_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusReferenceCategories raw e trusted', 120, 1),
(13000, 13511, 'sp_TotvsSkusIntegrationGroupCodes_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusIntegrationGroupCodes raw e trusted', 120, 1),
(13000, 13512, 'sp_TotvsSkusColors_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColors raw e trusted', 120, 1),
(13000, 13513, 'sp_TotvsSkusColorsMeasurementsTableOmni_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColorsMeasurementsTableOmni raw e trusted', 120, 1),
(13000, 13514, 'sp_TotvsSkusColorsMeasurementsTableOmniMedidas_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColorsMeasurementsTableOmniMedidas raw e trusted', 120, 1),
(13000, 13515, 'sp_TotvsSkusColorsProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsSkusColorsProducts raw e trusted', 120, 1),
(13000, 13516, 'sp_TotvsProductsBarCodes_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsBarCodes raw e trusted', 120, 1),
(13000, 13517, 'sp_TotvsProductsClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsClassifications raw e trusted', 120, 1),
(13000, 13518, 'sp_TotvsProductsAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsAdditionalFields raw e trusted', 120, 1),
(13000, 13519, 'sp_TotvsProductsSuppliers_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsSuppliers raw e trusted', 120, 1),
(13000, 13520, 'sp_TotvsProductsManufacturers_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsManufacturers raw e trusted', 120, 1),
(13000, 13521, 'sp_TotvsProductsReferenceCodeSequences_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsReferenceCodeSequences raw e trusted', 120, 1),
(13000, 13522, 'sp_TotvsProductsWebData_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsWebData raw e trusted', 120, 1),
(13000, 13523, 'sp_TotvsProductsWebDetails_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsWebDetails raw e trusted', 120, 1),
(13000, 13524, 'sp_TotvsProductsBranchesBlocked_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsBranchesBlocked raw e trusted', 120, 1),
(13000, 13525, 'sp_TotvsProductsConservationInstructions_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsConservationInstructions raw e trusted', 120, 1),
(13000, 13526, 'sp_TotvsProductsConservationInstructionsItems_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsConservationInstructionsItems raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13000, 'TotvsPostSearchReferences', 'Integração das referências e produtos das marcas (TotvsSkus e filhas), uma chamada por matriz, a cada 2 horas', 120, 2),

/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13014, 'TotvsGetProductByCode', 'Integração da referência de um produto por código (GET Products/{docCompany}/{code})', 0, 2),

/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (saldos: 13632 - 13633)*/
(13017, 13632, 'sp_TotvsBalances_Sync', 'Procedure de sincronização entre as tabelas TotvsBalances raw e trusted', 3, 1),
(13017, 13633, 'sp_TotvsBalancesLocations_Sync', 'Procedure de sincronização entre as tabelas TotvsBalancesLocations raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (fase 3: 13017)*/
(130, 13017, 'TotvsPostSearchBalances', 'Integração dos saldos de todos os tipos, por loja (TotvsBalances e locais), a cada 3 minutos', 3, 2),

/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (similares e lotes: 13637 - 13641)*/
(13094, 13637, 'sp_TotvsProductsSimilar_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsSimilar raw e trusted', 120, 1),
(13094, 13638, 'sp_TotvsProductsSimilarItems_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsSimilarItems raw e trusted', 120, 1),
(13095, 13639, 'sp_TotvsBatches_Sync', 'Procedure de sincronização entre as tabelas TotvsBatches raw e trusted', 120, 1),
(13095, 13640, 'sp_TotvsBatchesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsBatchesItems raw e trusted', 120, 1),
(13095, 13641, 'sp_TotvsBatchesItemsAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsBatchesItemsAdditionalFields raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Product: 13080 - 13099, src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md); 13096 reservado (omni-changed-balances, PR2)*/
(130, 13094, 'TotvsGetSimilar', 'Integração dos produtos similares (TotvsProductsSimilar e itens), a cada 2 horas', 120, 2),
(130, 13095, 'TotvsPostSearchBatch', 'Integração dos lotes de todas as lojas (TotvsBatches, itens e campos adicionais), a cada 2 horas', 120, 2),
(130, 13097, 'TotvsPostSearchProductCodes', 'Conferência diária dos produtos de cada marca com as referências gravadas; recupera as referências dos que faltam', 1440, 2),

/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13100, 'TotvsGetProducts', 'Produto por código, pack, barras, RFID ou item de lote (GET Products/{docCompany}/Codes/{code})', 0, 2),
(130, 13101, 'TotvsGetKardexMovement', 'Kardex de um produto numa loja (GET Products/{docCompany}/Kardex)', 0, 2),
(130, 13102, 'TotvsGetGridFrequency', 'Grade de frequência de um produto (GET Products/{docCompany}/{productCode}/GridFrequency)', 0, 2),
(130, 13103, 'TotvsGetMeasurementConversions', 'Conversões de unidade de um produto ou grupo (GET Products/{docCompany}/MeasurementConversions)', 0, 2),
(130, 13104, 'TotvsGetCompositionProduct', 'Composição por produto (GET Products/{docCompany}/CompositionProduct)', 0, 2),
(130, 13105, 'TotvsGetCompositionGroupProduct', 'Composição por grupo (GET Products/{docCompany}/CompositionGroupProduct)', 0, 2),

/*Escritas de cadastro da API Product - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:Products:Enabled = true (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.5, §5.6 e §5.8, fase PR-5)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13120, 'TotvsPostBarcodes', 'Inclusão de códigos de barras de produtos (POST Products/{docCompany}/BarCodes)', 0, 2),
(130, 13121, 'TotvsPostUpdateBarcodes', 'Alteração de códigos de barras de produtos (POST Products/{docCompany}/BarCodes/Update)', 0, 2),
(130, 13122, 'TotvsPostCreateClassificationRelationship', 'Inclusão de classificações de produtos (POST Products/{docCompany}/ClassificationRelationships/Create)', 0, 2),
(130, 13123, 'TotvsPostUpdateClassificationRelationship', 'Alteração de classificações de produtos (POST Products/{docCompany}/ClassificationRelationships/Update)', 0, 2),
(130, 13127, 'TotvsPostReferences', 'Inclusão de referência com os SKUs (POST Products/{docCompany}/References)', 0, 2),
(130, 13135, 'TotvsPostCreateBatch', 'Inclusão de lotes e itens de lote (POST Products/{docCompany}/Batches/Create)', 0, 2),
(130, 13136, 'TotvsPutBatchInfo', 'Alteração de itens de lote (PUT Products/{docCompany}/BatchInfo)', 0, 2),
(130, 13137, 'TotvsPostBatchPreferentialSalesOrder', 'Pedidos de venda preferenciais de um item de lote (POST Products/{docCompany}/BatchPreferentialSalesOrder)', 0, 2),

/*Escritas de dados fiscais, preço, custo e promoção da API Product (efeito comercial e fiscal) - executadas pelo TotvsController, não são
  recorrentes (IntervalTimeInMinutes = 0); só rodam com Totvs:Writes:Products:Enabled = true e, nas de valor, com os códigos e lojas permitidos
  (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.6 e §5.7, fase PR-6)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13128, 'TotvsPutProducts', 'Dados fiscais e indicadores de um produto numa empresa (PUT Products/{docCompany}/{code}/{branchCode})', 0, 2),
(130, 13129, 'TotvsPutBranchInfo', 'Indicadores dos produtos numa empresa (PUT Products/{docCompany}/BranchInfo/{branchCode})', 0, 2),
(130, 13130, 'TotvsPutData', 'Dados fiscais dos produtos (PUT Products/{docCompany}/Data)', 0, 2);
