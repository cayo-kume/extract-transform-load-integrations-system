using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Oto.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script002_Insert_Into_Jobs_Table.sql
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        OtoSendOrders = 4000,
        OtoSendCustomers = 4001,
        OtoSendProducts = 4002,
        OtoSendOrdersItems = 4003,
        OtoSendStores = 4004,
        OtoSendSellers = 4005
    }
}
