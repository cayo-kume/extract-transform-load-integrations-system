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

        // escritas (TotvsController, com X-User-Name; não recorrentes): docs/PLANO-TOTVS-METODOS-FALTANTES.md §9.4
        TotvsPostDuplicates = 13018,   //Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql (fase AP-2)
        TotvsPostGroupDuplicates = 13019,   //Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql (fase AP-2)
        TotvsPostDuplicatesInstallmentsStage = 13020   //Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql
    }
}
