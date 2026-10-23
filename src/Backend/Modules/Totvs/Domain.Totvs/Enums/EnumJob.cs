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
        TotvsPostSearchComissionsPaid = 13009   //Script024_Insert_Totvs_Accounts_Payable_Jobs_Into_Jobs_Table.sql
    }
}
