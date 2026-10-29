namespace Domain.LinxCommerce.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir no script de jobs do service correspondente
    /// (um por service, Script0NN_Insert_LinxCommerce_*_Jobs_Into_Jobs_Table.sql). Faixas: integrações 7000 - 7499,
    /// procedures 7500 - 7999 (ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        LinxCommerceSearchOrders = 7000,   //Script006_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchOrderStatus = 7001,   //Script006_Insert_LinxCommerce_Orders_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchCustomer = 7002,   //Script009_Insert_LinxCommerce_Customers_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchProduct = 7003,   //Script012_Insert_LinxCommerce_Catalog_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchSKU = 7004,   //Script012_Insert_LinxCommerce_Catalog_Jobs_Into_Jobs_Table.sql
        LinxCommerceSearchSalesRepresentative = 7005,   //Script015_Insert_LinxCommerce_Sales_Representatives_Jobs_Into_Jobs_Table.sql
    }
}
