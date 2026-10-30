namespace Domain.AfterSale.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos scripts de jobs de cada pasta (Script0NN_Insert_AfterSale_*_Jobs_Into_Jobs_Table.sql)
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        /*Me*/
        AfterSaleEcommerces = 5000,

        /*Reverses*/
        AfterSaleReverse = 5001,
        AfterSaleReverses = 5002,
        AfterSaleTransportationsReverses = 5003,
        AfterSaleCourierAttributes = 5004,
        AfterSaleApproveReverse = 5005,
        AfterSaleReciveProducts = 5006,
        AfterSaleReturnedInvoice = 5007,
        AfterSaleReverseInvoice = 5008,
        AfterSaleStatusReverses = 5009,
        AfterSaleAdministrativeRecordsReverse = 5021,
        AfterSaleGetReverseById = 5022,

        /*Refunds*/
        AfterSaleRefund = 5011,
        AfterSaleRefunds = 5012,
        AfterSaleStatusRefunds = 5013,
        AfterSaleActionsRefunds = 5014,
        AfterSaleBanksRefunds = 5015,
        AfterSaleTypesRefunds = 5016,
        AfterSaleApproveRefund = 5017,
        AfterSaleCancelRefund = 5018,
        AfterSalePayRefund = 5019,
        AfterSaleChangeTotalAmountRefund = 5020,
        AfterSaleGetRefundById = 5023
    }
}
