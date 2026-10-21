using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Movidesk.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script005_Insert_Movidesk_Values_Into_Jobs_Table.sql
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        MovideskGetTickets = 3000,
        MovideskGetPeople = 3001,

        /*Sob demanda*/
        MovideskGetTicket = 3002,
        MovideskGetPerson = 3003
    }
}
