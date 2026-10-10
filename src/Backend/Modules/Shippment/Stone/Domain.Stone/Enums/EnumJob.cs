using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Stone.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script002_Insert_Into_Jobs_Table.sql
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        StoneLogisticaTrackingHistory = 11000,
        StoneLogisticaSendOrders = 11001,
        StoneLogisticaCancelOrders = 11003,
        StoneLogisticaGetZplLabels = 11002
    }
}
