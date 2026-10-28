namespace Domain.Totvs.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir no script de jobs do service correspondente
    /// (um por service, Script0NN_Insert_Totvs_*_Jobs_Into_Jobs_Table.sql). Faixas: integrações 13000 - 13499,
    /// procedures 13500 - 13999 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        TotvsPostSearchReferences = 13000,   //Script012_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchLegalEntity = 13001,   //Script015_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchCustomers = 13002,   //Script015_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchInvoices = 13003,   //Script027_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchPurchaseOrders = 13004,   //Script018_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchSalesOrders = 13005,   //Script021_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchSellers = 13006,   //Script009_Insert_Totvs_Sellers_Jobs_Into_Jobs_Table.sql
        TotvsGetBranchesList = 13007,      //Script006_Insert_Totvs_Branches_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchDuplicates = 13008,   //Script024_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchComissionsPaid = 13009,   //Script024_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql

        // sob demanda (TotvsController, com X-User-Name; não recorrentes): a partir de 13010
        TotvsGetCustomerByCode = 13010,   //Script029_Insert_Totvs_On_Demand_People_Jobs_Into_Jobs_Table.sql
        TotvsGetLegalEntityByCode = 13011,   //Script029_Insert_Totvs_On_Demand_People_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderByKey = 13012,   //Script030_Insert_Totvs_On_Demand_Documents_Jobs_Into_Jobs_Table.sql
        TotvsGetInvoiceByKey = 13013,   //Script030_Insert_Totvs_On_Demand_Documents_Jobs_Into_Jobs_Table.sql
        TotvsGetProductByCode = 13014,   //Script031_Insert_Totvs_On_Demand_Products_Jobs_Into_Jobs_Table.sql

        // preços, custos e saldos (fase 3, partes 4 a 6, entregues na fase PR-2), a cada 3 minutos
        TotvsPostSearchPrices = 13015,   //Script050_Insert_Totvs_Product_Values_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchCosts = 13016,   //Script050_Insert_Totvs_Product_Values_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchBalances = 13017,   //Script050_Insert_Totvs_Product_Values_Jobs_Into_Jobs_Table.sql

        // representantes (120 min), recepção de venda (3 min) e representante por código (sob demanda): fase PE-2
        TotvsPostSearchRepresentatives = 13030,   //Script038_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql
        TotvsGetSearchReceptionSale = 13031,   //Script041_Insert_Totvs_Reception_Sales_Jobs_Into_Jobs_Table.sql
        TotvsGetRepresentativeByCode = 13039,   //Script038_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql

        // catálogos da API Person (fase PE-1), a cada 2 horas
        TotvsPersonGetClassifications = 13032,   //Script035_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetEmailTypes = 13033,   //Script035_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetPhoneTypes = 13034,   //Script035_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetContactTypes = 13035,   //Script035_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql

        // leituras sob demanda da onda 2 (fases PE-3, PE-4, PR-3, PO-1 e SO-2); PO-1 grava, as outras não
        TotvsGetBranches = 13036,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetPersonStatistics = 13037,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsPostListBalanceBonus = 13038,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetProducts = 13100,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetKardexMovement = 13101,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetGridFrequency = 13102,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetMeasurementConversions = 13103,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetCompositionProduct = 13104,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetCompositionGroupProduct = 13105,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetPurchaseOrderByKey = 13185,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderChangedBatchItems = 13193,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderInvoices = 13195,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderPendingItems = 13196,   //Script065_Insert_Totvs_On_Demand_Wave_2_Jobs_Into_Jobs_Table.sql

        // regiões de atuação da API Seller (fase SE-1), a cada 2 horas
        TotvsGetOperationalArea = 13070,   //Script044_Insert_Totvs_Operational_Areas_Jobs_Into_Jobs_Table.sql

        // catálogos da API Product (fase PR-1), a cada 2 horas
        TotvsGetClassificationType = 13080,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsProductGetClassifications = 13081,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetCategory = 13082,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetGrid = 13083,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetMeasurementUnit = 13084,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchColors = 13085,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetAdditionalFieldsTypes = 13086,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetProductGrouperConfiguration = 13087,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetInstructionItems = 13088,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostCompositions = 13089,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetPriceTablesHeaders = 13090,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetPriceTableScales = 13091,   //Script047_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql

        // itens das tabelas de preço e promoções (fase PR-2, parte 2), a cada 2 horas
        TotvsPostSearchPriceTables = 13092,   //Script053_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsGetPromotion = 13093,   //Script053_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql

        // similares e lotes (120 min) e a conferência diária das referências (fase PR-2, parte 3); 13096 reservado (PR2)
        TotvsGetSimilar = 13094,   //Script056_Insert_Totvs_Product_Similar_And_Batches_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchBatch = 13095,   //Script056_Insert_Totvs_Product_Similar_And_Batches_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchProductCodes = 13097,   //Script056_Insert_Totvs_Product_Similar_And_Batches_Jobs_Into_Jobs_Table.sql

        // catálogos (120 min), lotes de pedidos (120 min) e sugestões de faturamento (3 min) da API Sales Order (fase SO-1); 13193 reservado (SO-2)
        TotvsGetSalesOrderDiscountType = 13190,   //Script059_Insert_Totvs_Sales_Order_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderClassifications = 13191,   //Script059_Insert_Totvs_Sales_Order_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderBatchItems = 13192,   //Script059_Insert_Totvs_Sales_Order_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderBillingSuggestions = 13194,   //Script059_Insert_Totvs_Sales_Order_Catalogs_Jobs_Into_Jobs_Table.sql

        // inutilizações e centros de custo da API Fiscal (fase FI-1), a cada 2 horas
        TotvsGetInvoicesDisable = 13250,   //Script062_Insert_Totvs_Fiscal_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetCostCenter = 13251,   //Script062_Insert_Totvs_Fiscal_Catalogs_Jobs_Into_Jobs_Table.sql

        // leituras sob demanda sem persistência da API Fiscal (fase FI-2): XML, DANFE e impressão da transação
        TotvsGetXmlContents = 13252,   //Script063_Insert_Totvs_On_Demand_Fiscal_Files_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchDanfe = 13253,   //Script063_Insert_Totvs_On_Demand_Fiscal_Files_Jobs_Into_Jobs_Table.sql
        TotvsPostTransactionPrint = 13254,   //Script063_Insert_Totvs_On_Demand_Fiscal_Files_Jobs_Into_Jobs_Table.sql
        TotvsGetSearchInvoicesItemDetail = 13255,   //Script064_Insert_Totvs_On_Demand_Fiscal_Queries_Jobs_Into_Jobs_Table.sql
        TotvsGetInvoicesPendingConditionalProducts = 13256,   //Script064_Insert_Totvs_On_Demand_Fiscal_Queries_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchInvoiceProducts = 13257,   //Script064_Insert_Totvs_On_Demand_Fiscal_Queries_Jobs_Into_Jobs_Table.sql

        // escritas (TotvsController, com X-User-Name; não recorrentes): docs/PLANO-TOTVS-METODOS-FALTANTES.md §9.4
        TotvsPostDuplicates = 13018,   //Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostGroupDuplicates = 13019,   //Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostDuplicatesInstallmentsStage = 13020,   //Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostIndividualCustomers = 13040,   //Script071_Insert_Totvs_Customers_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostLegalCustomers = 13041,   //Script071_Insert_Totvs_Customers_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostCustomerRepresentatives = 13042,   //Script071_Insert_Totvs_Customers_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostRemoveClassificationRelationship = 13043,   //Script071_Insert_Totvs_Customers_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostMessages = 13044,   //Script067_Insert_Totvs_People_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPutChangeMessages = 13045,   //Script067_Insert_Totvs_People_Writes_Jobs_Into_Jobs_Table.sql
        TotvsDeleteMessages = 13046,   //Script067_Insert_Totvs_People_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostBarcodes = 13120,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostUpdateBarcodes = 13121,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostCreateClassificationRelationship = 13122,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostUpdateClassificationRelationship = 13123,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostColors = 13124,   //Script068_Insert_Totvs_Products_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostClassifications = 13125,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostGrids = 13126,   //Script068_Insert_Totvs_Products_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostReferences = 13127,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostCreateBatch = 13135,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPutBatchInfo = 13136,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostBatchPreferentialSalesOrder = 13137,   //Script072_Insert_Totvs_Products_Catalog_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostPurchaseOrdersChangeStatus = 13182,   //Script069_Insert_Totvs_Purchase_Orders_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderAdditionalOrder = 13201,   //Script070_Insert_Totvs_Sales_Orders_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderShippingOrder = 13202,   //Script070_Insert_Totvs_Sales_Orders_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderObservationsOrder = 13203,   //Script070_Insert_Totvs_Sales_Orders_Writes_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderAdditionalItems = 13207,   //Script070_Insert_Totvs_Sales_Orders_Writes_Jobs_Into_Jobs_Table.sql
    }
}
