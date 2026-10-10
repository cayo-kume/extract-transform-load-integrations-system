using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Movidesk.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script002_Insert_Into_Jobs_Table.sql
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        MovideskGetTickets = 3000,
        MovideskGetPersons = 3001
    }
}
