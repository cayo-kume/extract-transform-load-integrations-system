namespace Domain.LinxCommerce.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir no script de jobs do service correspondente
    /// (um por service, Script0NN_Insert_LinxCommerce_*_Jobs_Into_Jobs_Table.sql). Faixas: integrações 7000 - 7499,
    /// procedures 7500 - 7999 (ids reservados em src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        LinxCommerceSearchOrders = 7000,   //Script007_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchOrderStatus = 7001,   //Script007_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchCustomer = 7002,   //Script010_Insert_LinxCommerce_Customers_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchProduct = 7003,   //Script013_Insert_LinxCommerce_Catalog_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchSKU = 7004,   //Script013_Insert_LinxCommerce_Catalog_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchSalesRepresentative = 7005,   //Script016_Insert_LinxCommerce_Sales_Representatives_Jobs_Into_Jobs_Table.sql

        // sob demanda (LinxCommerceController, com X-User-Name; não recorrentes): a partir de 7010
        LinxCommerceGetOrder = 7010,   //Script007_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql
        LinxCommerceGetOrderByNumber = 7011,   //Script007_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql
        LinxCommerceGetCustomer = 7012,   //Script010_Insert_LinxCommerce_Customers_Jobs_Into_Jobs_Table.sql
        LinxCommerceGetProduct = 7013,   //Script013_Insert_LinxCommerce_Catalog_Jobs_Into_Jobs_Table.sql
        LinxCommerceGetSKU = 7014,   //Script013_Insert_LinxCommerce_Catalog_Jobs_Into_Jobs_Table.sql
        LinxCommerceGetSalesRepresentative = 7015,   //Script016_Insert_LinxCommerce_Sales_Representatives_Jobs_Into_Jobs_Table.sql
    }
}
