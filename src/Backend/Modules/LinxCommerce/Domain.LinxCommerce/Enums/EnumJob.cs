using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.LinxCommerce.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script002_Insert_Into_Jobs_Table.sql
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        LinxCommerceSalesRepresentative = 7012,
        LinxCommerceCustomers = 7016,
        LinxCommerceOrders = 7000,
        LinxCommerceUpdateTrackingNumberOrder = 7017,
        LinxCommerceUpdateSalesRepresentative = 7018,
        LinxCommerceSku = 7013,
        LinxCommerceProduct = 7009
    }
}
