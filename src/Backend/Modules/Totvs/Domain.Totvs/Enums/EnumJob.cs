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

        TotvsPostSearchReferences = 13000,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchLegalEntity = 13001,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchCustomers = 13002,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchInvoices = 13003,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchPurchaseOrders = 13004,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchSalesOrders = 13005,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchSellers = 13006,   //Script010_Insert_Totvs_Sellers_Jobs_Into_Jobs_Table.sql
        TotvsGetBranchesList = 13007,      //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchDuplicates = 13008,   //Script022_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchComissionsPaid = 13009,   //Script022_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql

        // sob demanda (TotvsController, com X-User-Name; não recorrentes): a partir de 13010
        TotvsGetCustomerByCode = 13010,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsGetLegalEntityByCode = 13011,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderByKey = 13012,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetInvoiceByKey = 13013,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsGetProductByCode = 13014,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql

        // preços, custos e saldos (fase 3, partes 4 a 6, entregues na fase PR-2), a cada 3 minutos
        TotvsPostSearchPrices = 13015,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchCosts = 13016,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchBalances = 13017,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql

        // representantes (120 min), recepção de venda (3 min) e representante por código (sob demanda): fase PE-2
        TotvsPostSearchRepresentatives = 13030,   //Script031_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql
        TotvsGetSearchReceptionSale = 13031,   //Script031_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql
        TotvsGetRepresentativeByCode = 13039,   //Script031_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql

        // catálogos da API Person (fase PE-1), a cada 2 horas
        TotvsPersonGetClassifications = 13032,   //Script028_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetEmailTypes = 13033,   //Script028_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetPhoneTypes = 13034,   //Script028_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetContactTypes = 13035,   //Script028_Insert_Totvs_Person_Catalogs_Jobs_Into_Jobs_Table.sql

        // leituras sob demanda da onda 2 (fases PE-3, PE-4, PR-3, PO-1 e SO-2); PO-1 grava, as outras não
        TotvsGetBranches = 13036,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsGetPersonStatistics = 13037,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostListBalanceBonus = 13038,   //Script038_Insert_Totvs_Bonus_Jobs_Into_Jobs_Table.sql
        TotvsGetProducts = 13100,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsGetKardexMovement = 13101,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsGetGridFrequency = 13102,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsGetMeasurementConversions = 13103,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsGetCompositionProduct = 13104,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsGetCompositionGroupProduct = 13105,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsGetPurchaseOrderByKey = 13185,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderChangedBatchItems = 13193,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderInvoices = 13195,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderPendingItems = 13196,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql

        // regiões de atuação da API Seller (fase SE-1), a cada 2 horas
        TotvsGetOperationalArea = 13070,   //Script010_Insert_Totvs_Sellers_Jobs_Into_Jobs_Table.sql

        // catálogos da API Product (fase PR-1), a cada 2 horas
        TotvsGetClassificationType = 13080,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsProductGetClassifications = 13081,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetCategory = 13082,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetGrid = 13083,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetMeasurementUnit = 13084,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchColors = 13085,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetAdditionalFieldsTypes = 13086,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetProductGrouperConfiguration = 13087,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetInstructionItems = 13088,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostCompositions = 13089,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetPriceTablesHeaders = 13090,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsGetPriceTableScales = 13091,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql

        // itens das tabelas de preço e promoções (fase PR-2, parte 2), a cada 2 horas
        TotvsPostSearchPriceTables = 13092,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsGetPromotion = 13093,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql

        // similares e lotes (120 min) e a conferência diária das referências (fase PR-2, parte 3); 13096 reservado (PR2)
        TotvsGetSimilar = 13094,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchBatch = 13095,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchProductCodes = 13097,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql

        // catálogos (120 min), lotes de pedidos (120 min) e sugestões de faturamento (3 min) da API Sales Order (fase SO-1); 13193 reservado (SO-2)
        TotvsGetSalesOrderDiscountType = 13190,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderClassifications = 13191,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderBatchItems = 13192,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsGetSalesOrderBillingSuggestions = 13194,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql

        // inutilizações e centros de custo da API Fiscal (fase FI-1), a cada 2 horas
        TotvsGetInvoicesDisable = 13250,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsGetCostCenter = 13251,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql

        // leituras sob demanda sem persistência da API Fiscal (fase FI-2): XML, DANFE e impressão da transação
        TotvsGetXmlContents = 13252,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchDanfe = 13253,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsPostTransactionPrint = 13254,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsGetSearchInvoicesItemDetail = 13255,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsGetInvoicesPendingConditionalProducts = 13256,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsPostSearchInvoiceProducts = 13257,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql
        TotvsPostInvoicesManifestations = 13258,   //Script025_Insert_Totvs_Invoices_Jobs_Into_Jobs_Table.sql

        // escritas (TotvsController, com X-User-Name; não recorrentes): docs/PLANO-TOTVS-METODOS-FALTANTES.md §9.4
        TotvsPostDuplicates = 13018,   //Script022_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql
        TotvsPostGroupDuplicates = 13019,   //Script022_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql
        TotvsPostDuplicatesInstallmentsStage = 13020,   //Script022_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql
        TotvsPostIndividualCustomers = 13040,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostLegalCustomers = 13041,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostCustomerRepresentatives = 13042,   //Script031_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql
        TotvsPostRemoveClassificationRelationship = 13043,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostMessages = 13044,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPutChangeMessages = 13045,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsDeleteMessages = 13046,   //Script007_Insert_Totvs_People_Jobs_Into_Jobs_Table.sql
        TotvsPostBonusDueDate = 13047,   //Script038_Insert_Totvs_Bonus_Jobs_Into_Jobs_Table.sql
        TotvsPostBonusConsume = 13048,   //Script038_Insert_Totvs_Bonus_Jobs_Into_Jobs_Table.sql
        TotvsPostBarcodes = 13120,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostUpdateBarcodes = 13121,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostCreateClassificationRelationship = 13122,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostUpdateClassificationRelationship = 13123,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostColors = 13124,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostClassifications = 13125,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostGrids = 13126,   //Script034_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql
        TotvsPostReferences = 13127,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPutProducts = 13128,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPutBranchInfo = 13129,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPutData = 13130,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostCreateValues = 13131,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsPostUpdateValues = 13132,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsPostUpdatePromotionValues = 13133,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsPostPromotionItems = 13134,   //Script037_Insert_Totvs_Product_Prices_Jobs_Into_Jobs_Table.sql
        TotvsPostCreateBatch = 13135,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPutBatchInfo = 13136,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostBatchPreferentialSalesOrder = 13137,   //Script013_Insert_Totvs_Products_Jobs_Into_Jobs_Table.sql
        TotvsPostPurchaseOrders = 13180,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostPurchaseOrdersQuantityItems = 13181,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostPurchaseOrdersChangeStatus = 13182,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostPurchaseOrdersCancel = 13183,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostPurchaseOrderBatchLink = 13184,   //Script016_Insert_Totvs_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderB2COrders = 13197,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderOrdersCancel = 13198,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderOrdersChangeStatus = 13199,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderHeader = 13200,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderAdditionalOrder = 13201,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderShippingOrder = 13202,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderObservationsOrder = 13203,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderPdvTransactions = 13204,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderItems = 13205,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsDeleteSalesOrderItems = 13206,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderAdditionalItems = 13207,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderQuantityItems = 13208,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderPriceItems = 13209,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderCancelItems = 13210,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderRelationshipCounts = 13211,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderRelationshipBatchItems = 13212,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsDeleteSalesOrderBatchItems = 13213,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
        TotvsPostSalesOrderSuggestBatchItemsNuance = 13214,   //Script019_Insert_Totvs_Sales_Orders_Jobs_Into_Jobs_Table.sql
    }
}
